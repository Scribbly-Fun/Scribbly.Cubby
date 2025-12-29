using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
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
            builder.Services.AddGrpcClient<CacheService.CacheServiceClient>((sp, grpcClientOptions) =>
            {
                var clientOptions = sp.GetRequiredService<CubbyClientOptions>();
                grpcClientOptions.Address = clientOptions.Host;
            });
            
            builder.Services.AddSingleton<CubbyGrpcTransport>();
            
            builder.Services.AddSingleton<ICubbyStoreTransport, CubbyGrpcTransport>(
                sp => sp.GetRequiredService<CubbyGrpcTransport>());
            
            builder.Services.AddSingleton<IGrpcCubbyStoreTransport, CubbyGrpcTransport>(
                sp => sp.GetRequiredService<CubbyGrpcTransport>());
            
            builder.Services.AddScoped<IGrpcCubbyClient, GrpcCubbyClient>();
            
            return builder;
        }
    }
}