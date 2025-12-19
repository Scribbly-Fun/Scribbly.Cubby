using Google.Protobuf;
using Grpc.Core;
using Scribbly.Cubby.Keys;
using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Values;

namespace Scribbly.Cubby.Hosted.Store;

public sealed class CacheServiceImpl : CacheService.CacheServiceBase
{
    private readonly DictionaryCacheStore _store = new();

    public override Task<GetResponse> Get(
        GetRequest request,
        ServerCallContext context)
    {
        var keyBytes = request.Key.CopyFrom();
        var key = new ByteKey(keyBytes);

        if (_store.TryGet(key, out var value))
        {
            return Task.FromResult(new GetResponse
            {
                Found = true,
                Value = ByteString.CopyFrom(value.Data.Span)
            });
        }

        return Task.FromResult(new GetResponse { Found = false });
    }

    public override Task<PutResponse> Put(
        PutRequest request,
        ServerCallContext context)
    {
        var keyBytes = request.Key.CopyFrom();
        var valueBytes = request.Value.CopyFrom();

        var key = new ByteKey(keyBytes);
        var value = new CacheValue(valueBytes);

        _store.Put(key, value);

        return Task.FromResult(new PutResponse());
    }
}
