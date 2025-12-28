using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Cache;
using Scribbly.Cubby.Client;

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
        /// <param name="clientBuilderCallback">Optional callback to modify the http client.</param>
        /// <returns>The configured cubby client host.</returns>
        public ICubbyClientBuilder WithCubbyHttpClient(Action<IHttpClientBuilder>? clientBuilderCallback = null)
        {
            var clientBuilder = builder.HostBuilder.Services.AddHttpClient(nameof(CubbyHttpTransport), (sp, httpOptions) =>
            {
                var clientOptions = sp.GetRequiredService<CubbyClientOptions>();
                httpOptions.BaseAddress = clientOptions.Host;
            });
            
            builder.HostBuilder.Services.AddSingleton<CubbyHttpTransport>();
            
            builder.HostBuilder.Services.AddSingleton<ICubbyStoreTransport, CubbyHttpTransport>(
                sp => sp.GetRequiredService<CubbyHttpTransport>());
            
            builder.HostBuilder.Services.AddSingleton<IHttpCubbyStoreTransport, CubbyHttpTransport>(
                sp => sp.GetRequiredService<CubbyHttpTransport>());
            
            clientBuilderCallback?.Invoke(clientBuilder);
            
            return builder;
        }
    }
}