using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
        public ICubbyServerBuilder AddCubbyServer(Action<CubbyOptions>? optionsCallback = null)
        {
            var options = new CubbyOptions();
        
            optionsCallback?.Invoke(options);

            hostBuilder.Services.AddSingleton(options);

            var cubbyBuilder = new CubbyServerBuilder(options, hostBuilder);
            
            hostBuilder.Services.AddSingleton<ICubbyStore>(sp =>
            {
                var ops = sp.GetRequiredService<CubbyOptions>();
                var factory = new CubbyStoreFactory();
                return factory.CreateStore(ops);
            });
            
            hostBuilder.Services.TryAddSingleton(TimeProvider.System);

            if (options.CacheCleanupOptions.Strategy is not CacheCleanupOptions.AsyncStrategy.Disabled)
            {
                hostBuilder.Services.AddSingleton<IExpirationEvictionService>(sp =>
                {
                    var store = sp.GetRequiredService<ICubbyStore>();

                    return store is not ICubbyStoreEvictionInteraction interaction 
                        ? throw new InvalidOperationException("The supported store does not support Eviction interaction") 
                        : new ExpirationEvictionService(interaction);
                });
            }
            
            if (options.CacheCleanupOptions.Strategy is CacheCleanupOptions.AsyncStrategy.Disabled or CacheCleanupOptions.AsyncStrategy.Manual)
            {
                hostBuilder.Services.AddHostedService<CacheCleanupAsyncProcessor>();
            }
            
            return cubbyBuilder;
        }
    }
}