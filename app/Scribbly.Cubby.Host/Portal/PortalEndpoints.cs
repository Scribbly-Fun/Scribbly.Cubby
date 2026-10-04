using Microsoft.Extensions.Options;
using Scribbly.Cubby.Host.Portal.Cache;
using Scribbly.Cubby.Host.Portal.Metrics;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal;

/// <summary>
/// Maps endpoints required for the Portal UI
/// </summary>
public static class PortalEndpointExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        /// <summary>
        /// Maps all portal endpoints
        /// </summary>
        /// <returns>The endpoint group with new portal endpoints.</returns>
        public IEndpointRouteBuilder MapCubbyPortal()
        {
            var portalGroup = builder.MapGroup("cubby/portal");
            
            portalGroup.MapGet("/options", (IOptions<CubbyServerOptions> options) => options.Value);

            portalGroup.MapCubbyPortalCacheEndpoints();
            portalGroup.MapCubbyPortalMetricsEndpoints();

            return portalGroup;
        }
    }
}