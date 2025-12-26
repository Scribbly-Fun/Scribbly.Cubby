using System.Runtime.Serialization;
using Scribbly.Cubby.Client.Serializer;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

internal sealed class CubbyClient(ICubbyStoreTransport store, ICubbySerializer serializer) : ICubbyClient
{
    /// <inheritdoc />
    public ValueTask<bool> Exists(BytesKey key, CancellationToken token = default)
    {
        return store.Exists(key, token);
    }

    /// <inheritdoc />
    public ValueTask<PutResult> Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options, CancellationToken token = default)
    {
        return store.Put(key, value.ToArray(), options, token);
    }

    /// <inheritdoc />
    public async ValueTask<PutResult> Put<T>(BytesKey key, T value, CacheEntryOptions options, CancellationToken token = default)
    {
        var encodedValue = serializer.Serialize<T>(value);
        
        return await store.Put(key, encodedValue.ToArray(), options, token);
    }

    /// <inheritdoc />
    public ValueTask<byte[]> Get(BytesKey key, CancellationToken token = default)
    {
        return store.Get(key, token);
    }

    /// <inheritdoc />
    public async ValueTask<T> Get<T>(BytesKey key, CancellationToken token = default)
    {
        var data = await store.Get(key, token);

        var value = serializer.Deserialize<T>(data);
        
        return value ?? throw new SerializationException("Failed to convert the stored bytes to the requested object");
    }
}