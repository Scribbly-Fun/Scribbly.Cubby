using Google.Protobuf;
using Grpc.Core;
using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Store;

internal sealed class CubbyGrpcServer(ICubbyStore store) : CacheService.CacheServiceBase
{
    /// <inheritdoc />
    public override Task<GetResponse> Get(
        GetRequest request,
        ServerCallContext context)
    {
        var keyBytes = request.Key.CopyFrom();
        var key = new BytesKey(keyBytes);

        if (store.TryGet(key, out var value))
        {
            return Task.FromResult(new GetResponse
            {
                Found = true,
                Value = ByteString.CopyFrom(value.Data.Span)
            });
        }

        return Task.FromResult(new GetResponse { Found = false });
    }

    /// <inheritdoc />
    public override Task<PutResponse> Put(
        PutRequest request,
        ServerCallContext context)
    {
        var keyBytes = request.Key.CopyFrom();
        var valueBytes = request.Value.CopyFrom();

        var key = new BytesKey(keyBytes);
        var value = new BytesValue(valueBytes);

        store.Put(key, value);

        return Task.FromResult(new PutResponse());
    }
}
