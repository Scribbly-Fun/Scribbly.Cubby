using System.Collections.Concurrent;
using Scribbly.Cubby.Keys;
using Scribbly.Cubby.Values;

namespace Scribbly.Cubby.Stores;

internal sealed class DictionaryCacheStore
{
    private readonly ConcurrentDictionary<ByteKey, CacheValue> _store = new();

    public bool TryGet(ByteKey key, out CacheValue value)
        => _store.TryGetValue(key, out value!);

    public void Put(ByteKey key, CacheValue value)
        => _store[key] = value;
}