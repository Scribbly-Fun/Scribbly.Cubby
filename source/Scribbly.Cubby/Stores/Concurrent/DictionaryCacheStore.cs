using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Stores.Concurrent;

/// <summary>
/// A cache storage that uses arrays of concurrent dictionaries to improve multithreaded locking contention.
/// </summary>
/// <remarks>As of 12.20.2025 this will be the initial store implementation as it seems to be the safest while still being fast</remarks>
internal sealed class ConcurrentStore : ICubbyStore
{
    
    private readonly ConcurrentDictionary<BytesKey, ICacheEntry> _store = new();
    
    /// <inheritdoc />
    public bool Exists(BytesKey key) => _store.ContainsKey(key);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(BytesKey key) => _store[key].ValueMemory;

    /// <inheritdoc />
    public bool TryGet(BytesKey key, [NotNullWhen(true)] out ReadOnlyMemory<byte>? value)
    {
        if (!_store.TryGetValue(key, out var entry))
        {
            value = null;
            return false;
        }

        value = entry.ValueMemory;
        return true;
    }
    
    /// <inheritdoc />
    public void Put(ICacheEntry entry)
    {
        var key = new BytesKey(entry.Key.ToArray());
        
        _store.AddOrUpdate(
            key,
            static (_, entry) => entry,
            static (_, oldEntry, entry) =>
            {
                ArrayPool<byte>.Shared.Return(oldEntry.Buffer);
                return entry;
            },
            entry);
    }

    /// <inheritdoc />
    public void Evict(BytesKey key)
    {
        if (_store.TryRemove(key, out var entry))
        {
            ArrayPool<byte>.Shared.Return(entry.Buffer);
        }
    }

    /// <inheritdoc />
    public bool TryEvict(BytesKey key)
    {
        if (!_store.TryRemove(key, out var entry))
        {
            return false;
        }
        
        ArrayPool<byte>.Shared.Return(entry.Buffer);
        return true;
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var entry in _store)
        {
            ArrayPool<byte>.Shared.Return(entry.Value.Buffer);
        }
        _store.Clear();
    }
}