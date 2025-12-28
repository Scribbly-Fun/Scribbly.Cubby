using Microsoft.Extensions.Hosting;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Builder used to configure and setup the cubby client interactions.
/// </summary>
public interface ICubbyClientBuilder
{
    /// <summary>
    /// The cubby options.
    /// </summary>
    CubbyClientOptions Options { get; }
    
    /// <summary>
    /// The Application Host Builder.
    /// </summary>
    IHostApplicationBuilder HostBuilder { get; }
}