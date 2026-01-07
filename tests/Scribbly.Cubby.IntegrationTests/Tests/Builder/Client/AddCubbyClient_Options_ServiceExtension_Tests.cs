using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Client;

namespace Scribbly.Cubby.IntegrationTests.Tests.Builder.Client;

public class AddCubbyClient_Options_ServiceExtension_Tests
{
    [Theory]
    [InlineData("http://test.com/")]
    [InlineData("https://127.0.0.1:8080/")]
    [InlineData("http://scribbly.fun/")]
    public void AddCubbyClient_Should_Register_IOptionsCubby(string url)
    {
        var services = new ServiceCollection();

        services.AddCubbyClient(cb =>
        {
            cb.Host = new Uri(url);
            
        }).WithCubbyHttpClient();

        var sp = services.BuildServiceProvider();

        sp.GetRequiredService<CubbyClientOptions>().Host.ToString().Should().Be(url);
    }
}