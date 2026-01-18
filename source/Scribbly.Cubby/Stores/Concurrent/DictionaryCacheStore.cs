using System.Buffers;
using System.Collections.Concurrent;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Stores.Concurrent;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
internal sealed class ConcurrentStore : ICubbyStore, ICubbyStoreEvictionInteraction
{
    private readonly TimeProvider _provider;

    /// <summary>
    /// Creates a new sharded dictionary where the number of shards is equal to the number of processors. 
    /// </summary>
    internal static ConcurrentStore FromOptions(CubbyServerOptions serverOptions, TimeProvider provider)
        => new(serverOptions, provider);
    
    private int _activeWriters;
    
    private readonly ConcurrentDictionary<BytesKey, byte[]> _store;

    private ConcurrentStore(CubbyServerOptions serverOptions, TimeProvider provider)
    {
        _provider = provider;
        _store = serverOptions.Capacity == int.MinValue 
            ? new ConcurrentDictionary<BytesKey, byte[]>()
            : new ConcurrentDictionary<BytesKey, byte[]>(
                concurrencyLevel: Environment.ProcessorCount,
                capacity: serverOptions.Capacity);
    }
    
    /// <inheritdoc />
    int ICubbyStoreEvictionInteraction.ActiveWriters
        => Volatile.Read(ref _activeWriters);

    /// <inheritdoc />
    IEnumerable<KeyValuePair<BytesKey, byte[]>> ICubbyStoreIterator.Entries
    {
        get
        {
            foreach (var kvp in _store)
            {
                yield return kvp;
            }
        }
    }
    
    /// <inheritdoc />
    public bool Exists(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);

        try
        {
            if (!_store.TryGetValue(key, out var buffer))
            {
                return false;
            }
            
            var header = buffer.GetHeader();
            var flags = header.GetFlags();

            if (flags.IsTombstone() && _store.TryRemoveRentedArray(key))
            {
                return false;
            }
        
            if (header.IsNeverExpiringEntry(out var expirationTicks))
            {
                return true;
            }

            var now = _provider.GetUtcNow().UtcTicks;
            return !expirationTicks.IsExpired(now) || !_store.TryRemoveRentedArray(key);
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
            var entry = _store[key].GetEntryFromBuffer();

            var header = entry.GetHeader();
            var flags = header.GetFlags();

            if (flags.IsTombstone() && _store.TryRemoveRentedArray(key))
            {
                return null;
            }
        
            if (header.IsNeverExpiringEntry(out var expirationTicks))
            {
                return entry;
            }

            var now = _provider.GetUtcNow().UtcTicks;
            if (expirationTicks.IsExpired(now) && _store.TryRemoveRentedArray(key))
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
            if (!_store.TryGetValue(key, out var buffer))
            {
                value = null;
                return false;
            }

            var entry = buffer.GetEntryFromBuffer();
            var header = entry.GetHeader();
            var flags = header.GetFlags();

            if (flags.IsTombstone() && _store.TryRemoveRentedArray(key))
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
            if (expirationTicks.IsExpired(now) && _store.TryRemoveRentedArray(key))
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
            var buffer = value.RentCacheEntryArray(options);

            if (!_store.TryGetValue(key, out var existing))
            {
                return _store.TryAdd(key, buffer) ? PutResult.Created : PutResult.Undefined;
            }
            
            var header = existing.GetHeader();
            if (header.IsSlidingEntry())
            {
                var now = _provider.GetUtcNow().UtcTicks;
                header.UpdateSlidingTime(now);
            }
            
            if(_store.TryUpdate(key, buffer, existing))
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
            if (!_store.TryGetValue(key, out var entry))
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
    public EvictResult Evict(in BytesKey key) => _store.TryRemoveRentedArray(key) ? EvictResult.Removed : EvictResult.Unknown;
    
    /// <inheritdoc />
    public CacheEntryFlags Tombstone(in BytesKey key)
    {
        Interlocked.Increment(ref _activeWriters);

        try
        {
            if (!_store.TryGetValue(key, out var entry))
            {
                return CacheEntryFlags.None;
            }

            var header = entry.GetHeader();
            var flags = header.GetFlags();
            
            header.UpdateFlags(flags |= CacheEntryFlags.Tombstone);
            return flags;
        }
        finally
        {
            Interlocked.Decrement(ref _activeWriters);
        }
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var entry in _store)
        {
            ArrayPool<byte>.Shared.Return(entry.Value, clearArray: false);
        }
        _store.Clear();
    }
}