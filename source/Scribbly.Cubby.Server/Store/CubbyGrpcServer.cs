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
        var key = new BytesKey(request.Key.ToByteArray());

        if (store.TryGet(key, out var value))
        {
            return Task.FromResult(new GetResponse
            {
                Found = true,
                Value = ByteString.CopyFrom(value)
            });
        }

        return Task.FromResult(new GetResponse { Found = false });
    }

    /// <inheritdoc />
    public override Task<PutResponse> Put(
        PutRequest request,
        ServerCallContext context)
    {
        var key = new BytesKey(request.Key.ToByteArray());

        store.Put(key, request.Value.ToByteArray(), CacheEntryOptions.Never);

        return Task.FromResult(new PutResponse
        {
            Result = PutResult.Updated
        });
    }

    /// <inheritdoc />
    public override Task<EvictResponse> Evict(EvictRequest request, ServerCallContext context)
    {
        var key = new BytesKey(request.Key.ToByteArray());
        store.Evict(key);

        return Task.FromResult(new EvictResponse
        {
            Result = EvictResult.Removed
        });
    }
}
