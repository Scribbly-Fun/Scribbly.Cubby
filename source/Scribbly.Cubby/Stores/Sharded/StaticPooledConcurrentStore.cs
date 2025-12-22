using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Stores.Sharded;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
/// <remarks>As of 12.20.2025 this will be the initial store implementation as it seems to be the safest while still being fast</remarks>
internal sealed class StaticPooledConcurrentStore : ICubbyStore
{
    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static StaticPooledConcurrentStore FromCores => new(Environment.ProcessorCount);
    
    private readonly ConcurrentDictionary<BytesKey, ICacheEntry>[] _shards;

    private StaticPooledConcurrentStore(int shardCount)
    {
        _shards = new ConcurrentDictionary<BytesKey, ICacheEntry>[shardCount];
        
        for (var i = 0; i < shardCount; i++)
        {
            _shards[i] = new ConcurrentDictionary<BytesKey, ICacheEntry>();
        }
    }
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => GetShard(key).ContainsKey(key);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(BytesKey key) => GetShard(key)[key].ValueMemory;

    /// <inheritdoc />
    public bool TryGet(BytesKey key, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value)
    {
        var shard = GetShard(key);
        if (!shard.TryGetValue(key, out var entry))
        {
            value = null;
            return false;
        }

        value = entry.ValueMemory;
        return true;
    }
    
    /// <inheritdoc />
    public void Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        var shard = GetShard(key);
        var newEntry = PooledStaticCacheEntry.Create(value, options.Tll);
        
        shard.AddOrUpdate(
            key,
            static (_, entry) => entry,
            static (_, oldEntry, entry) =>
            {
                oldEntry.Dispose();
                return entry;
            },
            newEntry);
    }

    /// <inheritdoc />
    public void Evict(BytesKey key)
    {
        var shard = GetShard(key);
        if (shard.TryRemove(key, out var entry))
        {
            entry.Dispose();
        }
    }

    /// <inheritdoc />
    public bool TryEvict(BytesKey key)
    {
        var shard = GetShard(key);
        if (!shard.TryRemove(key, out var entry))
        {
            return false;
        }
        
        entry.Dispose();
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
                entry.Dispose();
            }

            shard.Clear();
        }
    }
}