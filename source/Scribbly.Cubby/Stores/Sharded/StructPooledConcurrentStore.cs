using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Stores.Sharded;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
/// <remarks>As of 12.20.2025 this will be the initial store implementation as it seems to be the safest while still being fast</remarks>
internal sealed class StructPooledConcurrentStore : ICubbyStore
{
    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static StructPooledConcurrentStore FromCores => new(Environment.ProcessorCount);
    
    private readonly ConcurrentDictionary<BytesKey, byte[]>[] _shards;

    private StructPooledConcurrentStore(int shardCount)
    {
        _shards = new ConcurrentDictionary<BytesKey, byte[]>[shardCount];
        
        for (var i = 0; i < shardCount; i++)
        {
            _shards[i] = new ConcurrentDictionary<BytesKey, byte[]>();
        }
    }
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => GetShard(key).ContainsKey(key);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(BytesKey key)
    {
        var shared = GetShard(key)[key];
        return new PooledCacheEntryStruct(shared).ValueMemory;
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

        value = new PooledCacheEntryStruct(entry).Value;
        return true;
    }
    
    /// <inheritdoc />
    public void Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        var shard = GetShard(key);

        var buffer = ArrayPool<byte>.Shared.Rent(16 + value.Length);
        var span = buffer.AsSpan();

        BinaryPrimitives.WriteInt64LittleEndian(span, options.Tll);
        BinaryPrimitives.WriteInt32LittleEndian(span[8..], value.Length);
        BinaryPrimitives.WriteInt16LittleEndian(span[12..], 0);
        value.CopyTo(span[16..]);
        
        shard.AddOrUpdate(
            key,
            addValueFactory: static (_, entry) => entry,
            updateValueFactory: static (_, oldEntry, entry) =>
            {
                ArrayPool<byte>.Shared.Return(oldEntry, clearArray: false);
                return entry;
            },
            buffer);
    }

    /// <inheritdoc />
    public void Evict(BytesKey key)
    {
        var shard = GetShard(key);
        if (shard.TryRemove(key, out var entry))
        {
            ArrayPool<byte>.Shared.Return(entry, clearArray: false);
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
        
        ArrayPool<byte>.Shared.Return(entry, clearArray: false);
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