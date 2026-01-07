using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Builder.Server;

public class AddCubbyServer_Options_HostBuilderExtenstion_Tests
{
    [Theory(Skip = "Currently the host application appsettings.json are overriding this and not allowing an isolated test")]
    [InlineData(100)]
    public void Given_Capacity_Value_Options_Should_HaveCapacity(int cap)
    {
        var builder = new HostApplicationBuilder();

        builder.AddCubbyServer(ops =>
            {
                ops.Capacity = cap;
            })
            .WithCubbyHttpServer();
        
        using var host = builder.Build();
        var serviceProvider = host.Services;

        serviceProvider.GetRequiredService<IOptions<CubbyServerOptions>>().Value.Capacity.Should().Be(cap);
    }
}