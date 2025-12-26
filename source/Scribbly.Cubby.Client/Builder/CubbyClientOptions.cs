using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Options used to setup the client.
/// </summary>
public class CubbyClientOptions
{
    internal Type SerializerImplementation { get; private set; } = typeof(SystemTextCubbySerializer);
    
    /// <summary>
    /// The URL for the Cubby host.
    /// </summary>
    public Uri Host { get; set; } = new Uri("https://localhost:5001");

    /// <summary>
    /// The caching service's scope
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

    /// <summary>
    /// Configures the Cubby serializer implementation.
    /// </summary>
    /// <typeparam name="TSerializer"></typeparam>
    public void AddSerializer<TSerializer>() where TSerializer : ICubbySerializer
    {
        SerializerImplementation = typeof(TSerializer);
    }
}