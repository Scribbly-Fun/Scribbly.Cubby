using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Concurrent;

internal sealed class DictionaryCacheStore
{
    private readonly ConcurrentDictionary<BytesKey, BytesValue> _store = new();

    public bool TryGet(BytesKey key, out BytesValue value)
        => _store.TryGetValue(key, out value!);

    public void Put(BytesKey key, BytesValue value)
        => _store[key] = value;
}