using Microsoft.Extensions.DependencyInjection;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Options used to setup the client.
/// </summary>
public class CubbyClientOptions
{
    /// <summary>
    /// The URL for the Cubby host.
    /// </summary>
    public Uri Host { get; set; } = new Uri("https://localhost:5001");

    /// <summary>
    /// The caching service's scope
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;
}