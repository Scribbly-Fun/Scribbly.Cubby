using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Server;

/// <summary>
/// Cubby Server builder used to setup the server aspects of the cache store.
/// </summary>
public interface ICubbyServerBuilder
{
    /// <summary>
    /// The options used during setup.
    /// </summary>
    CubbyServerOptions ServerOptions { get; }
    
    /// <summary>
    /// The hosted builder.
    /// </summary>
    IHostApplicationBuilder HostBuilder { get; }
}