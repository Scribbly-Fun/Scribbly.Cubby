using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Cache;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Proto;

namespace Scribbly.Cubby.Builder;

/// <summary>
/// Extensions to help configure the application setup
/// </summary>
public static class CubbyClientBuilderExtensions
{
    /// <summary>
    /// Extends the cubby client host builder
    /// </summary>
    /// <param name="builder">The Cubby Host Builder</param>
    extension(ICubbyClientBuilder builder)
    {
        /// <summary>
        /// Adds cubby services and configures the store.
        /// </summary>
        /// <returns>The configured cubby client host.</returns>
        public ICubbyClientBuilder WithCubbyGrpcClient()
        {
            builder.HostBuilder.Services.AddGrpcClient<CacheService.CacheServiceClient>((sp, grpcClientOptions) =>
            {
                var clientOptions = sp.GetRequiredService<CubbyClientOptions>();
                grpcClientOptions.Address = clientOptions.Host;
            });
            
            builder.HostBuilder.Services.AddSingleton<IDistributedCache, CubbyDistributedCache>();
            
            return builder;
        }
    }
}