using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Concurrent;

internal sealed class DictionaryCacheStore
{
    private readonly ConcurrentDictionary<BytesKey, CacheEntry> _store = new();

    public bool TryGet(BytesKey key, out CacheEntry value)
        => _store.TryGetValue(key, out value!);

    public void Put(BytesKey key, CacheEntry value)
        => _store[key] = value;
}