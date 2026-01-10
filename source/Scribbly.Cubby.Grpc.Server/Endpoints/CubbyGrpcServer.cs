using Google.Protobuf;
using Grpc.Core;
using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;
using CSharpPutResult = Scribbly.Cubby.Stores.PutResult;
using GrpcPutResult = Scribbly.Cubby.Proto.PutResult;
using CSharpEvictResult = Scribbly.Cubby.Stores.EvictResult;
using GrpcEvictResult = Scribbly.Cubby.Proto.EvictResult;
using CsharpRefreshResult = Scribbly.Cubby.Stores.RefreshResult;
using GrpcRefreshResult = Scribbly.Cubby.Proto.RefreshRequest;
using RefreshResult = Scribbly.Cubby.Proto.RefreshResult;

namespace Scribbly.Cubby.Endpoints;

internal sealed class CubbyGrpcServer(ICubbyStore store, TimeProvider provider) : CacheService.CacheServiceBase
{
    /// <inheritdoc />
    public override Task<GetResponse> Get(
        GetRequest request,
        ServerCallContext context)
    {
        var key = new BytesKey(request.Key.ToByteArray());

        if (store.TryGet(key, out var entry))
        {
            return Task.FromResult(new GetResponse
            {
                Found = true,
                Value = ByteString.CopyFrom(entry)
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

        var result = store.Put(key, request.Value.ToByteArray(), request.ToOptions(provider));

        return Task.FromResult(new PutResponse
        {
            Result = result switch
            {
                CSharpPutResult.Undefined => GrpcPutResult.Undefined,
                CSharpPutResult.Created => GrpcPutResult.Created,
                CSharpPutResult.Updated => GrpcPutResult.Updated,
                _ => throw new ArgumentOutOfRangeException(nameof(result))
            }
        });
    }

    /// <inheritdoc />
    public override Task<ExistsResponse> Exists(ExistsRequest request, ServerCallContext context)
    {
        var key = new BytesKey(request.Key.ToByteArray());
        var result = store.Exists(key);

        return Task.FromResult(new ExistsResponse
        {
            Found = result
        });
    }

    /// <inheritdoc />
    public override Task<RefreshResponse> Refresh(GrpcRefreshResult request, ServerCallContext context)
    {
        var key = new BytesKey(request.Key.ToByteArray());
        var result = store.Refresh(key);

        return Task.FromResult(new RefreshResponse
        {
            Result = result switch
            {
                CsharpRefreshResult.Undefined => RefreshResult.Undefined,
                CsharpRefreshResult.Updated => RefreshResult.Updated ,
                CsharpRefreshResult.NotSlidingEntry => RefreshResult.NotSliding,
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
