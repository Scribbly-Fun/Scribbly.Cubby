using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Cache;
using Scribbly.Cubby.Proto;

namespace Scribbly.Cubby.Builder;

/// <summary>
/// Extensions to help configure the application setup
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Extends the host builder
    /// </summary>
    /// <param name="builder">The Host Builder</param>
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Adds cubby services and configures the store.
        /// </summary>
        /// <param name="optionsCallback">Options to configure cubby</param>
        /// <returns>The configured host.</returns>
        public IHostApplicationBuilder AddCubbyClient(Action<CubbyClientOptions>? optionsCallback = null)
        {
            var options = new CubbyClientOptions();
        
            optionsCallback?.Invoke(options);

            builder.Services.AddSingleton(options);
            
            builder.Services.AddGrpcClient<CacheService.CacheServiceClient>((sp, grpcClientOptions) =>
            {
                var clientOptions = sp.GetRequiredService<CubbyClientOptions>();
                grpcClientOptions.Address = clientOptions.Host;
            });
            
            builder.Services.AddSingleton<IDistributedCache, CubbyDistributedCache>();
            
            return builder;
        }
    }
}