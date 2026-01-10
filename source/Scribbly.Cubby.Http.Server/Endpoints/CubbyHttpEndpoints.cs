using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class CubbyHttpEndpoints
{
    internal static IResult Get(
        [FromQuery] BytesKey key, 
        HttpContext context,
        ICubbyStore store)
    {
        if (!store.TryGet(key, out var entry))
        {
            return Results.NoContent();
        }
        
        return Results.Bytes(entry.ToArray());
    }
    
    internal static async Task<IResult> Put(
        [AsParameters] CacheParameters parameters,
        HttpContext context,
        TimeProvider provider,
        ICubbyStore store,
        CancellationToken token)
    {
        using var ms = new MemoryStream(
            context.Request.ContentLength is { } len ? (int)len : 0);

        await context.Request.Body.CopyToAsync(ms, token);

        var buffer = ms.GetBuffer().AsSpan(0, (int)ms.Length).ToArray();
        
        var results = store.Put(parameters.Key, buffer, parameters.ToEntryOptions(provider));

        return results switch
        {
            PutResult.Undefined => Results.BadRequest(),
            PutResult.Created => Results.Created(),
            PutResult.Updated => Results.Ok(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    internal static IResult Exists(
        [FromQuery] BytesKey key, 
        ICubbyStore store)
    {
        var result = store.Exists(key);
        return result ? Results.Ok() : Results.NotFound();
    }
    
    internal static IResult Refresh(
        [FromQuery] BytesKey key, 
        ICubbyStore store)
    {
        var result = store.Refresh(key);

        return result switch
        {
            RefreshResult.Updated => Results.Accepted(),
            RefreshResult.NotSlidingEntry => Results.BadRequest(),
            RefreshResult.Undefined => Results.NotFound(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    internal static IResult Evict(
        [FromQuery] BytesKey key,
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
}
