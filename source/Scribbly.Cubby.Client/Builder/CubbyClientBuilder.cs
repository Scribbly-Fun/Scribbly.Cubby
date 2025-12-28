using Microsoft.Extensions.Hosting;

namespace Scribbly.Cubby.Client;

internal class CubbyClientBuilder : ICubbyClientBuilder
{
    public CubbyClientOptions Options { get; }

    public IHostApplicationBuilder HostBuilder { get; }
    
    public CubbyClientBuilder(CubbyClientOptions options, IHostApplicationBuilder hostBuilder)
    {
        Options = options;
        HostBuilder = hostBuilder;
    }
}