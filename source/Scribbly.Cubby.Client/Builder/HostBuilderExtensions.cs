using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Extensions to help configure the application setup
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Extends the host builder
    /// </summary>
    /// <param name="services">The Host Builder</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds cubby services and configures the store.
        /// </summary>
        /// <param name="optionsCallback">Options to configure cubby</param>
        /// <returns>The configured host.</returns>
        [RequiresUnreferencedCode("To support AOT ensure all JSON types have been provided")]
        [RequiresDynamicCode("To support AOT ensure all JSON types have been provided")]
        public ICubbyClientBuilder AddCubbyClient(Action<CubbyClientOptions>? optionsCallback = null)
        {
            var options = new CubbyClientOptions();
        
            optionsCallback?.Invoke(options);

            services.AddSingleton(options);

            var cubbyBuilder = new CubbyClientBuilder(options, services);

            services.AddSingleton<ICubbyCompressor>(options.Compressor);
            services.AddSingleton<ICubbySerializer>(options.Serializer);
            
            services.AddScoped<ICubbyClient, CubbyClient>();

            if (options.RegisterDistributedCache)
            {
                services.TryAddSingleton(TimeProvider.System);
                services.TryAddSingleton<IDistributedCache, CubbyDistributedCache>();
            }
            
            return cubbyBuilder;
        }
    }
}