using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            
            return cubbyBuilder;
        }
    }
}