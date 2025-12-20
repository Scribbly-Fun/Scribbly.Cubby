using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

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
    internal static ShardedConcurrentStore FromCores => new ShardedConcurrentStore(Environment.ProcessorCount);
    
    private readonly ConcurrentDictionary<BytesKey, BytesValue>[] _shards;

    private ShardedConcurrentStore(int shardCount)
    {
        _shards = new ConcurrentDictionary<BytesKey, BytesValue>[shardCount];
        
        for (var i = 0; i < shardCount; i++)
        {
            _shards[i] = new ConcurrentDictionary<BytesKey, BytesValue>();
        }
    }
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => GetShard(key).ContainsKey(key);

    /// <inheritdoc />
    public BytesValue Get(BytesKey key) => GetShard(key)[key];

    /// <inheritdoc />
    public bool TryGet(BytesKey key, [NotNullWhen(true)] out BytesValue? value)
    {
        if (!GetShard(key).TryGetValue(key, out var bytesValue))
        {
            value = null;
            return false;
        }

        value = bytesValue;
        return true;
    }

    /// <inheritdoc />
    public void Put(BytesKey key, BytesValue value) => GetShard(key)[key] = value;

    /// <inheritdoc />
    public void Evict(BytesKey key) => GetShard(key).Remove(key, out _);

    /// <inheritdoc />
    public bool TryEvict(BytesKey key) => GetShard(key).TryRemove(key, out _);

    private ConcurrentDictionary<BytesKey, BytesValue> GetShard(BytesKey key)
        => _shards[(key[0] & int.MaxValue) % _shards.Length];
}