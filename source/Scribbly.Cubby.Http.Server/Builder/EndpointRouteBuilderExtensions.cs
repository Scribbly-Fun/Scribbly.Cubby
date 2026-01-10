using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Scribbly.Cubby.Endpoints;

namespace Scribbly.Cubby.Builder;

/// <summary>
/// Extensions used to setup and configure the applications gRPC mapping.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// The endpoint route builder to extend.
    /// </summary>
    /// <param name="builder">Endpoints to bind the GRPC service to.</param>
    extension(IEndpointRouteBuilder builder)
    {
        /// <summary>
        /// Maps the Cubby services to the application host.
        /// </summary>
        /// <returns></returns>
        public IEndpointConventionBuilder MapCubbyHttp(string prefix = "cubby")
        {
            var group = builder.MapGroup(prefix);
            
            group.MapGet("/", CubbyHttpEndpoints.Get);
            
            group.MapPut("/", CubbyHttpEndpoints.Put);
            
            group.MapDelete("/", CubbyHttpEndpoints.Evict);

            group.MapPut("/exists", CubbyHttpEndpoints.Exists);
            
            group.MapPut("/refresh", CubbyHttpEndpoints.Refresh);
            
            return group;
        }
    }
}