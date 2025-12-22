using System.Buffers;
using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Sharded;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
/// <remarks>As of 12.20.2025 this will be the initial store implementation as it seems to be the safest while still being fast</remarks>
internal sealed class ShardedConcurrentStore : ICubbyStore
{
    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static ShardedConcurrentStore FromOptions(CubbyOptions options) => new(options);
    
    private readonly ConcurrentDictionary<BytesKey, ICacheEntry>[] _shards;

    private ShardedConcurrentStore(CubbyOptions options)
    {
        _shards = new ConcurrentDictionary<BytesKey, ICacheEntry>[options.Cores];
        
        for (var i = 0; i < options.Cores; i++)
        {
            if (options.Capacity == int.MinValue)
            {
                _shards[i] = new ConcurrentDictionary<BytesKey, ICacheEntry>();
            }
            else
            {
                _shards[i] = new ConcurrentDictionary<BytesKey, ICacheEntry>(
                    concurrencyLevel: Environment.ProcessorCount,
                    capacity: options.Capacity);
            }
        }
    }
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => GetShard(key).ContainsKey(key);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(BytesKey key) => GetShard(key)[key].ValueMemory;

    /// <inheritdoc />
    public bool TryGet(BytesKey key, out ReadOnlySpan<byte> value)
    {
        var shard = GetShard(key);
        if (!shard.TryGetValue(key, out var entry))
        {
            value = null;
            return false;
        }

        value = entry.Value;
        return true;
    }
    
    /// <inheritdoc />
    public PutResult Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        var shard = GetShard(key);
        var newEntry = PooledCacheEntry.Create(value, options.TimeToLive);
        
        if (!shard.TryGetValue(key, out var existing))
        {
            return shard.TryAdd(key, newEntry) ? PutResult.Created : PutResult.Undefined;
        }
        
        if(shard.TryUpdate(key, newEntry, existing))
        {
            if (existing is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            return PutResult.Updated;
        }

        return PutResult.Undefined;
    }

    /// <inheritdoc />
    public EvictResult Evict(BytesKey key)
    {
        var shard = GetShard(key);
        if (shard.TryRemove(key, out var entry))
        {
            if (entry is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return EvictResult.Removed;
        }

        return EvictResult.Unknown;
    }

    /// <inheritdoc />
    public bool TryEvict(BytesKey key, out EvictResult result)
    {
        var shard = GetShard(key);
        if (!shard.TryRemove(key, out var entry))
        {
            result = EvictResult.Unknown;
            return false;
        }
        
        if (entry is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        result = EvictResult.Removed;
        return true;
    }

    private ConcurrentDictionary<BytesKey, ICacheEntry> GetShard(BytesKey key)
        => _shards[(key[0] & int.MaxValue) % _shards.Length];

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var shard in _shards)
        {
            foreach (var (_, entry) in shard)
            {
                if (entry is not IDisposable disposable)
                {
                    continue;
                }
                
                disposable.Dispose();
            }

            shard.Clear();
        }
    }
}