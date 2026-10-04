using Microsoft.AspNetCore.Mvc;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal.Cache;

internal static class CacheEndpoints
{
    extension(IEndpointRouteBuilder endpointGroup)
    {
        internal IEndpointRouteBuilder MapCubbyPortalCacheEndpoints()
        {
            endpointGroup.MapGet("/caches", IEnumerable<CacheResponse> (ICubbyStore store) =>
            {
                return store is not ICubbyStoreIterator storeIterator 
                    ? [] 
                    : storeIterator.Entries.Select<KeyValuePair<BytesKey, byte[]>, CacheResponse>(e => e.Response);
            });

            endpointGroup.MapGet("/caches/value", (ICubbyStore store, [FromQuery(Name = "key")] BytesKey key) =>
            {
                if (!store.TryGet(key, out var entry))
                {
                    return  Results.NotFound();
                }   
                
                return Results.Bytes(entry.GetValue().ToArray());
            });

            endpointGroup.MapDelete("/caches/tombstone", (ICubbyStore store, [FromQuery(Name = "key")] BytesKey key) =>
            {
                if (!store.Exists(key))
                {
                    return Results.NotFound();
                }
                
                var flags = store.Tombstone(key);
                return flags.HasFlag(CacheEntryFlags.Tombstone) ? Results.Ok() : Results.BadRequest();
            });

            endpointGroup.MapDelete("/caches/evict", (ICubbyStore store, [FromQuery(Name = "key")] BytesKey key) =>
            {
                var result = store.Evict(key);

                return result switch
                {
                    EvictResult.Undefined => Results.BadRequest(),
                    EvictResult.Removed => Results.NoContent(),
                    EvictResult.Unknown => Results.NotFound(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });

            return endpointGroup;
        }
    }
}