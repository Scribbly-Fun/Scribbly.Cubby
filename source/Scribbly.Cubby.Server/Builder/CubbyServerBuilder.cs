using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Server;

internal class CubbyServerBuilder : ICubbyServerBuilder
{
    /// <inheritdoc />
    public CubbyOptions Options { get; }

    /// <inheritdoc />
    public IHostApplicationBuilder HostBuilder { get; }
    
    internal CubbyServerBuilder(CubbyOptions options, IHostApplicationBuilder hostBuilder)
    {
        Options = options;
        HostBuilder = hostBuilder;
    }
}