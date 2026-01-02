using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Concurrent;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
internal sealed class ConcurrentStore : ICubbyStore
{
    private readonly ConcurrentDictionary<BytesKey, ICacheEntry> _store;

    internal static ConcurrentStore FromOptions(CubbyOptions options) => new(options);
    
    private ConcurrentStore(CubbyOptions options)
    {
        _store = options.Capacity == int.MinValue 
                ? new ConcurrentDictionary<BytesKey, ICacheEntry>()
                : new ConcurrentDictionary<BytesKey, ICacheEntry>(
                    concurrencyLevel: Environment.ProcessorCount,
                    capacity: options.Capacity);
    }
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => _store.ContainsKey(key);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(BytesKey key) => _store[key].ValueMemory;

    /// <inheritdoc />
    public bool TryGet(BytesKey key, out ReadOnlySpan<byte> value)
    {
        if (!_store.TryGetValue(key, out var entry))
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
        var newEntry = PooledCacheEntry.Create(value, options.SlidingDuration);
        
        if (!_store.TryGetValue(key, out var existing))
        {
            return _store.TryAdd(key, newEntry) ? PutResult.Created : PutResult.Undefined;
        }
        
        if(_store.TryUpdate(key, newEntry, existing))
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
    public RefreshResult Refresh(BytesKey key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EvictResult Evict(BytesKey key)
    {
        if (_store.TryRemove(key, out var entry))
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
        if (!_store.TryRemove(key, out var entry))
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
    
    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var entry in _store.Values)
        {
            if (entry is not IDisposable disposable)
            {
                continue;
            }
            
            disposable.Dispose();
        }
        
        _store.Clear();
    }
}