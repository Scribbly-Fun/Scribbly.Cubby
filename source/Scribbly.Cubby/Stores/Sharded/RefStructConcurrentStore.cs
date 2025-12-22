using System.Buffers;
using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Sharded;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
internal sealed class RefStructConcurrentStore : ICubbyStore
{
    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static RefStructConcurrentStore FromOptions(CubbyOptions options) => new(options);
    
    private readonly ConcurrentDictionary<BytesKey, byte[]>[] _shards;

    private RefStructConcurrentStore(CubbyOptions options)
    {
        _shards = new ConcurrentDictionary<BytesKey, byte[]>[options.Cores];
        
        for (var i = 0; i < options.Cores; i++)
        {
            if (options.Capacity == int.MinValue)
            {
                _shards[i] = new ConcurrentDictionary<BytesKey, byte[]>();
            }
            else
            {
                _shards[i] = new ConcurrentDictionary<BytesKey, byte[]>(
                    concurrencyLevel: Environment.ProcessorCount,
                    capacity: options.Capacity);
            }
        }
    }
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => GetShard(key).ContainsKey(key);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(BytesKey key)
    {
        var shared = GetShard(key)[key];
        return new CacheEntryStruct(shared).ValueMemory;
    }

    /// <inheritdoc />
    public bool TryGet(BytesKey key, out ReadOnlySpan<byte> value)
    {
        var shard = GetShard(key);
        if (!shard.TryGetValue(key, out var entry))
        {
            value = null;
            return false;
        }

        value = new CacheEntryStruct(entry).Value;
        return true;
    }
    
    /// <inheritdoc />
    public PutResult Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        var shard = GetShard(key);
        var buffer = value.LayoutEntry(options);

        if (!shard.TryGetValue(key, out var existing))
        {
            return shard.TryAdd(key, buffer) ? PutResult.Created : PutResult.Undefined;
        }
        
        if(shard.TryUpdate(key, buffer, existing))
        {
            ArrayPool<byte>.Shared.Return(existing, clearArray: false);
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
            ArrayPool<byte>.Shared.Return(entry, clearArray: false);
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
        
        ArrayPool<byte>.Shared.Return(entry, clearArray: false);
        result = EvictResult.Removed;
        return true;
    }

    private ConcurrentDictionary<BytesKey, byte[]> GetShard(BytesKey key)
        => _shards[(key[0] & int.MaxValue) % _shards.Length];

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var shard in _shards)
        {
            foreach (var (_, entry) in shard)
            {
                ArrayPool<byte>.Shared.Return(entry, clearArray: false);
            }

            shard.Clear();
        }
    }
}