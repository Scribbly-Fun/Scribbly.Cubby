using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Scribbly.Cubby.Client.Serializer;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

internal class CubbyClient(ICubbyStoreTransport store, ICubbySerializer serializer) : ICubbyClient
{
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<bool> Exists(BytesKey key, CancellationToken token = default)
    {
        return store.Exists(key, token);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<PutResult> Put(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options, CancellationToken token = default)
    {
        return store.Put(key, value, options, token);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<PutResult> PutObject<T>(BytesKey key, T value, CacheEntryOptions? options, CancellationToken token = default)
        where T : notnull
    {
        var encodedValue = serializer.Serialize<T>(value);
        
        return await store.Put(key, encodedValue.ToArray(), options, token);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ReadOnlyMemory<byte>> Get(BytesKey key, CancellationToken token = default)
    {
        return store.Get(key, token);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> GetObject<T>(BytesKey key, CancellationToken token = default)
        where T : notnull
    {
        var data = await store.Get(key, token);

        var value = serializer.Deserialize<T>(data.Span);
        
        return value ?? throw new SerializationException("Failed to convert the stored bytes to the requested object");
    }
}