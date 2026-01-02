using Google.Protobuf;
using Grpc.Core;
using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;
using CSharpPutResult = Scribbly.Cubby.Stores.PutResult;
using GrpcPutResult = Scribbly.Cubby.Proto.PutResult;
using CSharpEvictResult = Scribbly.Cubby.Stores.EvictResult;
using GrpcEvictResult = Scribbly.Cubby.Proto.EvictResult;

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

        var result = store.Put(key, request.Value.ToByteArray(), CacheEntryOptions.None);

        return Task.FromResult(new PutResponse
        {
            Result = result switch
            {
                CSharpPutResult.Undefined => GrpcPutResult.Undefined,
                CSharpPutResult.Created => GrpcPutResult.Created,
                CSharpPutResult.Updated => GrpcPutResult.Updated,
                _ => throw new ArgumentOutOfRangeException()
            }
        });
    }

    /// <inheritdoc />
    public override Task<EvictResponse> Evict(EvictRequest request, ServerCallContext context)
    {
        var key = new BytesKey(request.Key.ToByteArray());
        var result = store.Evict(key);

        return Task.FromResult(new EvictResponse
        {
            Result = result switch
            {
                CSharpEvictResult.Undefined => GrpcEvictResult.Undefined,
                CSharpEvictResult.Removed => GrpcEvictResult.Removed,
                CSharpEvictResult.Unknown => GrpcEvictResult.Unknown,
                _ => throw new ArgumentOutOfRangeException()
            }
        });
    }
}
