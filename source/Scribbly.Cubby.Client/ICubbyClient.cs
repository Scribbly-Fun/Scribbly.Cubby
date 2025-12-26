using Scribbly.Cubby.Client.Serializer;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

internal interface ICubbyStoreTransport
{
    ValueTask<bool> Exists(BytesKey key, CancellationToken token = default);
    
    ValueTask<PutResult> Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options);
}

public interface ICubbyClient
{
    ValueTask<bool> Exists(BytesKey key, CancellationToken token = default);
    
    ValueTask<PutResult> Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options);
    
    ValueTask<PutResult> Put<T>(BytesKey key, T value, CacheEntryOptions options);
}

internal sealed class CubbyClient(ICubbyStoreTransport store, ICubbySerializer serializer) : ICubbyClient
{
    /// <inheritdoc />
    public ValueTask<bool> Exists(BytesKey key, CancellationToken token = default)
    {
        return store.Exists(key, token);
    }

    /// <inheritdoc />
    public ValueTask<PutResult> Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options)
    {
        return store.Put(key, value, options);
    }

    /// <inheritdoc />
    public async ValueTask<PutResult> Put<T>(BytesKey key, T value, CacheEntryOptions options)
    {
        var encodedValue = serializer.Serialize<T>(value);
        
        return await store.Put(key, encodedValue, options);
    }
}