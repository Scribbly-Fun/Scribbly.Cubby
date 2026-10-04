using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal.Metrics;

internal static class Endpoints
{
    extension(IEndpointRouteBuilder endpointGroup)
    {
        internal IEndpointRouteBuilder MapCubbyPortalMetricsEndpoints()
        {
            endpointGroup.MapGet("/metrics", CleanupMetricsResponse (
                CleanupMetricsStore store,
                IOptions<CubbyServerOptions> options,
                [FromQuery(Name = "from")] DateTimeOffset? from,
                [FromQuery(Name = "to")] DateTimeOffset? to) =>
            {
                var fallback = CleanupMetricsMapping.ResolveFallbackDelay(options.Value.Cleanup);
                return CleanupMetricsMapping.ToResponse(store, fallback, from, to);
            });
            
            return endpointGroup;
        }
    }
}