using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Stores;

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
        public IHostApplicationBuilder AddCubbyServer(Action<CubbyOptions>? optionsCallback = null)
        {
            var options = new CubbyOptions();
        
            optionsCallback?.Invoke(options);

            builder.Services.AddSingleton(options);

            builder.Services.AddSingleton<ICubbyStore>(sp =>
            {
                var ops = sp.GetRequiredService<CubbyOptions>();
                var factory = new CubbyStoreFactory();
                return factory.CreateStore(ops.Store);
            });
        
            builder.Services.AddGrpc(ops =>
            {
                ops.EnableDetailedErrors = false;
            });

            return builder;
        }
    }
}