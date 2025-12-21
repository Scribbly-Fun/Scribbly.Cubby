using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Concurrent;

internal sealed class DictionaryCacheStore
{
    private readonly ConcurrentDictionary<BytesKey, PooledCacheEntry> _store = new();

    public bool TryGet(BytesKey key, out PooledCacheEntry value)
        => _store.TryGetValue(key, out value!);

    public void Put(BytesKey key, PooledCacheEntry value)
        => _store[key] = value;
}