using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Stores.Marshalled;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
internal sealed class MarshalledStore : ICubbyStore, ICubbyStoreEvictionInteraction
{
    private readonly TimeProvider _provider;

    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static MarshalledStore FromOptions(CubbyServerOptions serverOptions, TimeProvider provider)
        => new(serverOptions, provider);
    
    private int _activeWriters;
    
    private readonly Dictionary<BytesKey, byte[]>[] _shards;
    private readonly Lock[] _locks;

    private MarshalledStore(CubbyServerOptions serverOptions, TimeProvider provider)
    {
        _provider = provider;
        _locks = new Lock[serverOptions.Cores];
        _shards = new Dictionary<BytesKey, byte[]>[serverOptions.Cores];
        
        for (var i = 0; i < serverOptions.Cores; i++)
        {
            _locks[i] = new Lock();
            _shards[i] = serverOptions.Capacity == int.MinValue
                ? new Dictionary<BytesKey, byte[]>()
                : new Dictionary<BytesKey, byte[]>(
                    capacity: serverOptions.Capacity);
        }
    }
    
    /// <inheritdoc />
    int ICubbyStoreEvictionInteraction.ActiveWriters
        => Volatile.Read(ref _activeWriters);

    /// <inheritdoc />
    IEnumerable<KeyValuePair<BytesKey, byte[]>> ICubbyStoreIterator.Entries
    {
        get
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                KeyValuePair<BytesKey, byte[]>[] snapshot;

                lock (_locks[i])
                {
                    snapshot = _shards[i].ToArray();
                }

                for (int j = 0; j < snapshot.Length; j++)
                {
                    yield return snapshot[j];
                }
            }
        }
    }
    
    /// <inheritdoc />
    public ReadOnlySpan<byte> Get(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);
        var idx = GetShardIndex(key);
        var shard = GetShard(idx);
        
        try
        {
            lock (_locks[idx])
            {
                ref var buffer  = ref CollectionsMarshal.GetValueRefOrNullRef(shard, key);

                if (Unsafe.IsNullRef(ref buffer))
                {
                    throw new KeyNotFoundException();
                }
                
                var entry = buffer.GetEntryFromBuffer();
                var header = entry.GetHeader();
                var flags = header.GetFlags();

                if (flags.IsTombstone() && shard.TryRemoveRentedArray(key))
                {
                    return null;
                }

                if (header.IsNeverExpiringEntry(out var expirationTicks))
                {
                    return entry;
                }

                var now = _provider.GetUtcNow().UtcTicks;
                if (expirationTicks.IsExpired(now) && shard.TryRemoveRentedArray(key))
                {
                    return null;
                }

                if (flags.IsSliding())
                {
                    header.UpdateSlidingTime(now);
                }

                return entry;
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }

    /// <inheritdoc />
    public bool TryGet(in BytesKey key, out ReadOnlySpan<byte> value)
    {
        Interlocked.Increment(ref _activeWriters);
        var idx = GetShardIndex(key);
        var shard = GetShard(idx);

        try
        {
            lock (_locks[idx])
            {
                ref var buffer = ref CollectionsMarshal.GetValueRefOrNullRef(shard, key);

                if (Unsafe.IsNullRef(ref buffer))
                {
                    value = default;
                    return false;
                }

                var entry = buffer.GetEntryFromBuffer();
                var header = entry.GetHeader();
                var flags = header.GetFlags();

                if (flags.IsTombstone() && shard.TryRemoveRentedArray(key))
                {
                    value = default;
                    return false;
                }

                if (header.IsNeverExpiringEntry(out var expirationTicks))
                {
                    value = entry;
                    return true;
                }

                var now = _provider.GetUtcNow().UtcTicks;
                if (expirationTicks.IsExpired(now) && shard.TryRemoveRentedArray(key))
                {
                    value = default;
                    return false;
                }

                if (flags.IsSliding())
                {
                    header.UpdateSlidingTime(now);
                }

                value = entry;
                return true;
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }
    
    /// <inheritdoc />
    public PutResult Put(in BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        Interlocked.Increment(ref _activeWriters);
        var idx = GetShardIndex(key);
        var shard = GetShard(idx);

        try
        {
            lock (_locks[idx])
            {
                ref var buffer = ref CollectionsMarshal.GetValueRefOrAddDefault(shard, key, out var exists);

                if (exists)
                {
                    ArrayPool<byte>.Shared.Return(buffer!, false);
                }

                buffer = value.RentCacheEntryArray(options);
                var header = buffer.GetHeader();
                
                if (header.IsSlidingEntry())
                {
                    var now = _provider.GetUtcNow().UtcTicks;
                    header.UpdateSlidingTime(now);
                }

                return exists ? PutResult.Updated : PutResult.Created;
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }
    
    /// <inheritdoc />
    public bool Exists(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);
        var idx = GetShardIndex(key);
        var shard = GetShard(idx);

        try
        {
            lock (_locks[idx])
            {
                ref var buffer = ref CollectionsMarshal.GetValueRefOrNullRef(shard, key);
                if (Unsafe.IsNullRef(ref buffer))
                {
                    return false;
                }

                var header = buffer.GetHeader();
                var flags = header.GetFlags();

                if (flags.IsTombstone())
                {
                    ArrayPool<byte>.Shared.Return(buffer, false);
                    shard.Remove(key);
                    return false;
                }

                if (header.IsNeverExpiringEntry(out var expirationTicks))
                {
                    return true;
                }

                var now = _provider.GetUtcNow().UtcTicks;

                if (expirationTicks.IsExpired(now))
                {
                    ArrayPool<byte>.Shared.Return(buffer, false);
                    shard.Remove(key);
                    return false;
                }

                return true;
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }
    
    /// <inheritdoc />
    public RefreshResult Refresh(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);
        var shardIndex = GetShardIndex(key);
        var shard = _shards[shardIndex];
        try
        {
            lock (_locks[shardIndex])
            {
                ref var buffer = ref CollectionsMarshal.GetValueRefOrNullRef(shard, key);
                if (Unsafe.IsNullRef(ref buffer))
                {
                    return RefreshResult.Undefined;
                }
                
                var header = buffer.GetHeader();

                if (header.IsSlidingEntry())
                {
                    var now = _provider.GetUtcNow().UtcTicks;
                    header.UpdateSlidingTime(now);
                    return RefreshResult.Updated;
                }

                return RefreshResult.NotSlidingEntry;
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }

    /// <inheritdoc />
    public EvictResult Evict(in BytesKey key)
    {
        var idx = GetShardIndex(key);
        return GetShard(idx).TryRemoveRentedArray(key) ? EvictResult.Removed : EvictResult.Unknown;
    }
    
    /// <inheritdoc />
    public CacheEntryFlags Tombstone(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);
        var shardIndex = GetShardIndex(key);
        var shard = _shards[shardIndex];
        try
        {
            lock (_locks[shardIndex])
            {
                ref var buffer = ref CollectionsMarshal.GetValueRefOrNullRef(shard, key);
                if (Unsafe.IsNullRef(ref buffer))
                {
                    return CacheEntryFlags.None;
                }
                
                var header = buffer.GetHeader();
                var flags = header.GetFlags();
            
                header.UpdateFlags(flags |= CacheEntryFlags.Tombstone);
                return flags;
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetShardIndex(BytesKey key)
        => (key[0] & int.MaxValue) % _shards.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Dictionary<BytesKey, byte[]> GetShard(int index)
        => _shards[index];

    /// <inheritdoc />
    public void Dispose()
    {
        for (int i = 0; i < _shards.Length; i++)
        {
            lock (_locks[i])
            {
                var shard = _shards[i];
                foreach (var (_, entry) in  shard)
                {
                    ArrayPool<byte>.Shared.Return(entry, clearArray: false);
                }
                
                shard.Clear();
            }
        }
    }
}