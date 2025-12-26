using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="hostBuilder">The Host Builder</param>
    extension(IHostApplicationBuilder hostBuilder)
    {
        /// <summary>
        /// Adds cubby services and configures the store.
        /// </summary>
        /// <param name="optionsCallback">Options to configure cubby</param>
        /// <returns>The configured host.</returns>
        public ICubbyClientBuilder AddCubbyClient(Action<CubbyClientOptions>? optionsCallback = null)
        {
            var options = new CubbyClientOptions();
        
            optionsCallback?.Invoke(options);

            hostBuilder.Services.AddSingleton(options);

            var cubbyBuilder = new CubbyClientBuilder(options, hostBuilder);
            
            hostBuilder.Services.Add(ServiceDescriptor.Singleton(typeof(ICubbySerializer), options.SerializerImplementation));
            hostBuilder.Services.AddScoped<ICubbyClient, CubbyClient>();
            
            return cubbyBuilder;
        }
    }
}