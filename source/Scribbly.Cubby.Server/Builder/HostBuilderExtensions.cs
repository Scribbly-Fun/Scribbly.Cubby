using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Server.Background;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Server;

/// <summary>
/// Extensions to help configure the application setup
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Extends the host builder
    /// </summary>
    /// <param name="hostBuilder">The Host Builder</param>
    extension(IHostApplicationBuilder hostBuilder)
    {
        /// <summary>
        /// Adds cubby services and configures the store.
        /// </summary>
        /// <param name="optionsCallback">Options to configure cubby</param>
        /// <returns>The configured host.</returns>
        public ICubbyServerBuilder AddCubbyServer(Action<CubbyServerOptions>? optionsCallback = null)
        {
            var cubbyBuilder = hostBuilder.AddCubbyConfiguration(optionsCallback);

            cubbyBuilder.HostBuilder.Services.AddSingleton<CubbyStoreFactory>();
            cubbyBuilder.HostBuilder.Services.AddSingleton<ICubbyStore>(sp =>
            {
                var factory = sp.GetRequiredService<CubbyStoreFactory>();
                return factory.CreateStore();
            });
            
            cubbyBuilder.HostBuilder.Services.TryAddSingleton(TimeProvider.System);

            if (cubbyBuilder.ServerOptions.Cleanup.Strategy is not CacheCleanupOptions.AsyncStrategy.Disabled)
            {
                cubbyBuilder.HostBuilder.Services.AddSingleton<IExpirationEvictionService>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<IExpirationEvictionService>>();
                    var store = sp.GetRequiredService<ICubbyStore>();

                    return store is not ICubbyStoreEvictionInteraction interaction 
                        ? throw new InvalidOperationException("The supported store does not support Eviction interaction") 
                        : new ExpirationEvictionService(logger, interaction);
                });
            }
            
            if (cubbyBuilder.ServerOptions.Cleanup.Strategy is not CacheCleanupOptions.AsyncStrategy.Disabled and not CacheCleanupOptions.AsyncStrategy.Manual)
            {
                cubbyBuilder.HostBuilder.Services.AddHostedService<CacheCleanupAsyncProcessor>();
            }
            
            return cubbyBuilder;
        }

        private CubbyServerBuilder AddCubbyConfiguration(Action<CubbyServerOptions>? optionsCallback = null)
        {
            var options = new CubbyServerOptions();

            optionsCallback?.Invoke(options);

            hostBuilder.Configuration
                .GetSection(CubbyServerOptions.SectionName)
                .Bind(options);
            
            hostBuilder.Services
                .AddOptions<CubbyServerOptions>()
                .Bind(hostBuilder.Configuration.GetSection(CubbyServerOptions.SectionName));

            return new CubbyServerBuilder(options, hostBuilder);
        }
    }
}