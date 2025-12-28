using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Http;

[Collection(Collections.HttpCollection)]
public class Service_Resolver_Tests(HttpApplicationFactory application)
{
    [Fact]
    public void Transport_Should_Be_CubbyHttpTransport()
    {
        var transport = application.Services.GetRequiredService<ICubbyStoreTransport>();

        transport.Should().BeAssignableTo<CubbyHttpTransport>();
    }
    
    [Fact]
    public void Client_Should_Be_CubbyClient()
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        client.Should().BeAssignableTo<CubbyClient>();
    }
    
    [Fact]
    public void HttpClient_Should_Be_HttpCubbyClient()
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IHttpCubbyClient>();

        client.Should().BeAssignableTo<HttpCubbyClient>();
    }
    
    [Fact]
    public void CubbyHttpTransport_Should_NotBe_Null()
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetService<CubbyHttpTransport>();

        client.Should().NotBeNull();
    }
    
}