using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Scribbly.Cubby.Expiration;

namespace Scribbly.Cubby.Stores.Sharded;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
internal sealed class ShardedConcurrentStore : ICubbyStore, ICubbyStoreEvictionInteraction
{
    private readonly TimeProvider _provider;

    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static ShardedConcurrentStore FromOptions(CubbyServerOptions serverOptions, TimeProvider provider)
        => new(serverOptions, provider);
    
    private int _activeWriters;
    
    private readonly ConcurrentDictionary<BytesKey, byte[]>[] _shards;

    private ShardedConcurrentStore(CubbyServerOptions serverOptions, TimeProvider provider)
    {
        _provider = provider;
        _shards = new ConcurrentDictionary<BytesKey, byte[]>[serverOptions.Cores];
        
        for (var i = 0; i < serverOptions.Cores; i++)
        {
            _shards[i] = serverOptions.Capacity == int.MinValue
                ? new ConcurrentDictionary<BytesKey, byte[]>()
                : new ConcurrentDictionary<BytesKey, byte[]>(
                    concurrencyLevel: Environment.ProcessorCount,
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
            foreach (var shard in _shards)
            {
                foreach (var kvp in shard)
                {
                    yield return kvp;
                }
            }
        }
    }
    
    /// <inheritdoc />
    public bool Exists(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);

        try
        {
            var shard = GetShard(key);
            if (!shard.TryGetValue(key, out var buffer))
            {
                return false;
            }
            
            var header = buffer.GetHeader();
            var flags = header.GetFlags();

            if (flags.IsTombstone() && shard.TryRemoveRentedArray(key))
            {
                return false;
            }
        
            if (header.IsNeverExpiringEntry(out var expirationTicks))
            {
                return true;
            }

            var now = _provider.GetUtcNow().UtcTicks;
            
            if (expirationTicks.IsExpired(now) && shard.TryRemoveRentedArray(key))
            {
                return false;
            }
            
            return true;
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Get(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);
        
        try
        {
            ConcurrentDictionary<BytesKey, byte[]> shard = GetShard(key);
        
            var entry = shard[key].GetEntryFromBuffer();

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
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }

    /// <inheritdoc />
    public bool TryGet(in BytesKey key, out ReadOnlySpan<byte> value)
    {
        Interlocked.Increment(ref _activeWriters);
        
        try
        {
            var shard = GetShard(key);
            if (!shard.TryGetValue(key, out var buffer))
            {
                value = null;
                return false;
            }

            var entry = buffer.GetEntryFromBuffer();
            var header = entry.GetHeader();
            var flags = header.GetFlags();

            if (flags.IsTombstone() && shard.TryRemoveRentedArray(key))
            {
                value = null;
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
                value = null;
                return false;
            }

            if (flags.IsSliding())
            {
                header.UpdateSlidingTime(now);
            }
        
            value = entry;
            return true;
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

        try
        {
            var shard = GetShard(key);
            var buffer = value.RentCacheEntryArray(options);

            if (!shard.TryGetValue(key, out var existing))
            {
                return shard.TryAdd(key, buffer) ? PutResult.Created : PutResult.Undefined;
            }
            
            var header = existing.GetHeader();
            if (header.IsSlidingEntry())
            {
                var now = _provider.GetUtcNow().UtcTicks;
                header.UpdateSlidingTime(now);
            }
            
            if(shard.TryUpdate(key, buffer, existing))
            {
                ArrayPool<byte>.Shared.Return(existing, clearArray: false);
                return PutResult.Updated;
            }

            return PutResult.Undefined;
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

        try
        {
            var shard = GetShard(key);
            if (!shard.TryGetValue(key, out var entry))
            {
                return RefreshResult.Undefined;
            }

            var header = entry.GetHeader();

            if (header.IsSlidingEntry())
            {
                var now = _provider.GetUtcNow().UtcTicks;
                header.UpdateSlidingTime(now);
                return RefreshResult.Updated;
            }

            return RefreshResult.NotSlidingEntry;
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }

    /// <inheritdoc />
    public EvictResult Evict(in BytesKey key) => GetShard(key).TryRemoveRentedArray(key) ? EvictResult.Removed : EvictResult.Unknown;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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