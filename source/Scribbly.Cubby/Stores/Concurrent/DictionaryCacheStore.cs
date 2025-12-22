using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Stores.Concurrent;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
internal sealed class ConcurrentStore : ICubbyStore
{
    private readonly ConcurrentDictionary<BytesKey, ICacheEntry> _store = new();
    
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
    public void Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        var newEntry = PooledCacheEntry.Create(value, options.Tll);
        
        _store.AddOrUpdate(
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
        if (_store.TryRemove(key, out var entry))
        {
            entry.Dispose();
        }
    }

    /// <inheritdoc />
    public bool TryEvict(BytesKey key)
    {
        if (!_store.TryRemove(key, out var entry))
        {
            return false;
        }
        
        entry.Dispose();
        return true;
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var entry in _store.Values)
        {
            entry.Dispose();
        }
        
        _store.Clear();
    }
}