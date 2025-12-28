using System.Buffers;
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
    
    internal static async Task<IResult> Put(
        [FromRoute] string key, 
        [AsParameters] CacheParameters parameters,
        HttpContext context,
        ICubbyStore store,
        CancellationToken token)
    {
        using var ms = new MemoryStream(
            context.Request.ContentLength is { } len ? (int)len : 0);

        await context.Request.Body.CopyToAsync(ms, token);

        var buffer = ms.GetBuffer().AsSpan(0, (int)ms.Length).ToArray();
        
        var results = store.Put(key, buffer, parameters.ToEntryOptions());

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
