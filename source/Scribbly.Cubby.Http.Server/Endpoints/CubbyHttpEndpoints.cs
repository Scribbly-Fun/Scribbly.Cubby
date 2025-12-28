using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class CubbyHttpEndpoints
{
    internal static IResult Get(
        [FromRoute] string key, 
        ICubbyStore store)
    {
        return store.TryGet(key, out var value) 
            ? Results.Bytes(value.ToArray()) 
            : Results.NoContent();
    }
    
    internal static IResult Put(
        [FromRoute] string key, 
        [AsParameters] CacheParameters parameters,
        [FromBody] byte[] bytes,
        ICubbyStore store)
    {
        var results = store.Put(key, bytes, parameters.ToEntryOptions());

        return results switch
        {
            PutResult.Undefined => Results.BadRequest(),
            PutResult.Created => Results.Created(),
            PutResult.Updated => Results.Ok(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    internal static IResult Evict(
        [FromRoute] string key,
        ICubbyStore store)
    {
        var results = store.Evict(key);

        return results switch
        {
            EvictResult.Undefined => Results.BadRequest(),
            EvictResult.Unknown => Results.BadRequest(),
            EvictResult.Removed => Results.Ok(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal static IResult Refresh(
        [FromRoute] string key, 
        [AsParameters] CacheParameters parameters,
        ICubbyStore store)
    {
        throw new NotImplementedException();
    }
}
