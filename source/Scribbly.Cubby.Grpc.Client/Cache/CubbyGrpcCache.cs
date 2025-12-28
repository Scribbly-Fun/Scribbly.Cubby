using Google.Protobuf;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;
using PutResult = Scribbly.Cubby.Stores.PutResult;

namespace Scribbly.Cubby.Cache;

internal class CubbyGrpcCache(CacheService.CacheServiceClient client) : ICubbyStoreTransport
{
    /// <inheritdoc />
    public async ValueTask<bool> Exists(BytesKey key, CancellationToken token = default)
    {
        var entry = await client.GetAsync(new GetRequest
        {
            Key = ByteString.CopyFromUtf8(key)
            
        }, cancellationToken: token);
        
        return entry?.Value.ToByteArray() is {Length: > 0};
    }

    /// <inheritdoc />
    public async ValueTask<PutResult> Put(BytesKey key, byte[] value, CacheEntryOptions options, CancellationToken token = default)
    {
        var result = await client.PutAsync(new PutRequest
        {
            Key = ByteString.CopyFrom(key),
            Value = ByteString.CopyFrom(value),
            
        }, cancellationToken: token);

        return result switch
        {
            { Result: Proto.PutResult.Undefined } => PutResult.Undefined,
            { Result: Proto.PutResult.Created } => PutResult.Created,
            { Result: Proto.PutResult.Updated } => PutResult.Updated,

            _ => PutResult.Undefined
        };
    }

    /// <inheritdoc />
    public async ValueTask<byte[]> Get(BytesKey key, CancellationToken token = default)
    {
        var entry = await client.GetAsync(new GetRequest
        {
            Key = ByteString.CopyFromUtf8(key)
            
        }, cancellationToken: token);
        
        return entry?.Value.ToByteArray() ?? [];
    }
}