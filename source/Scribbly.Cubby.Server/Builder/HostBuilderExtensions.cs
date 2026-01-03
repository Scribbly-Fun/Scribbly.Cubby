using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
            var cubbyBuilder = hostBuilder.AddCubbyConfiguration(optionsCallback);
            
            cubbyBuilder.HostBuilder.Services.AddSingleton<ICubbyStore>(sp =>
            {
                var ops = sp.GetRequiredService<IOptions<CubbyOptions>>();
                var factory = new CubbyStoreFactory();
                return factory.CreateStore(ops.Value);
            });
            
            cubbyBuilder.HostBuilder.Services.TryAddSingleton(TimeProvider.System);

            if (cubbyBuilder.Options.Cleanup.Strategy is not CacheCleanupOptions.AsyncStrategy.Disabled)
            {
                cubbyBuilder.HostBuilder.Services.AddSingleton<IExpirationEvictionService>(sp =>
                {
                    var store = sp.GetRequiredService<ICubbyStore>();

                    return store is not ICubbyStoreEvictionInteraction interaction 
                        ? throw new InvalidOperationException("The supported store does not support Eviction interaction") 
                        : new ExpirationEvictionService(interaction);
                });
            }
            
            if (cubbyBuilder.Options.Cleanup.Strategy is not CacheCleanupOptions.AsyncStrategy.Disabled and not CacheCleanupOptions.AsyncStrategy.Manual)
            {
                cubbyBuilder.HostBuilder.Services.AddHostedService<CacheCleanupAsyncProcessor>();
            }
            
            return cubbyBuilder;
        }

        private CubbyServerBuilder AddCubbyConfiguration(Action<CubbyOptions>? optionsCallback = null)
        {
            var options = new CubbyOptions();

            optionsCallback?.Invoke(options);

            hostBuilder.Configuration
                .GetSection(nameof(CubbyOptions))
                .Bind(options);
            
            hostBuilder.Services
                .AddOptions<CubbyOptions>()
                .Bind(hostBuilder.Configuration.GetSection(nameof(CubbyOptions)));

            return new CubbyServerBuilder(options, hostBuilder);
        }
    }
}