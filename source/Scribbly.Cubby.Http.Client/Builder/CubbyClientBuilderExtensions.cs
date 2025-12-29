using Microsoft.Extensions.DependencyInjection;
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
            var clientBuilder = builder.Services.AddHttpClient(nameof(CubbyHttpTransport), (sp, httpOptions) =>
            {
                var clientOptions = sp.GetRequiredService<CubbyClientOptions>();
                httpOptions.BaseAddress = clientOptions.Host;
            });
            
            clientBuilderCallback?.Invoke(clientBuilder);
            
            builder.Services.AddSingleton<CubbyHttpTransport>();
            
            builder.Services.AddSingleton<ICubbyStoreTransport, CubbyHttpTransport>(
                sp => sp.GetRequiredService<CubbyHttpTransport>());
            
            builder.Services.AddSingleton<IHttpCubbyStoreTransport, CubbyHttpTransport>(
                sp => sp.GetRequiredService<CubbyHttpTransport>());
            
            
            builder.Services.AddScoped<IHttpCubbyClient, HttpCubbyClient>();
            
            return builder;
        }
    }
}