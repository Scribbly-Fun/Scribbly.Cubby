using Microsoft.Extensions.DependencyInjection;

namespace Scribbly.Cubby.Client;

internal class CubbyClientBuilder : ICubbyClientBuilder
{
    public CubbyClientOptions Options { get; }

    public IServiceCollection Services { get; }
    
    public CubbyClientBuilder(CubbyClientOptions options, IServiceCollection services)
    {
        Options = options;
        Services = services;
    }
}