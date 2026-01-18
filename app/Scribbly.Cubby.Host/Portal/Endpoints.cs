using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal;

public static class Endpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapCubbyPortal()
        {
            var portalGroup = builder.MapGroup("cubby/portal");
            
            portalGroup.MapGet("/options", (IOptions<CubbyServerOptions> options) => options.Value);

            portalGroup.MapGet("/caches", IEnumerable<CacheResponse> (ICubbyStore store) =>
            {
                return store is not ICubbyStoreIterator storeIterator 
                    ? [] 
                    : storeIterator.Entries.Select(e => e.Response);
            });

            portalGroup.MapDelete("/caches/tombstone", (ICubbyStore store, [FromQuery(Name = "key")] BytesKey key) =>
            {
                if (!store.Exists(key))
                {
                    return Results.NotFound();
                }
                
                var flags = store.Tombstone(key);
                return flags.HasFlag(CacheEntryFlags.Tombstone) ? Results.Ok() : Results.BadRequest();
            });

            portalGroup.MapDelete("/caches/evict", (ICubbyStore store, [FromQuery(Name = "key")] BytesKey key) =>
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

            return portalGroup;
        }
    }
}