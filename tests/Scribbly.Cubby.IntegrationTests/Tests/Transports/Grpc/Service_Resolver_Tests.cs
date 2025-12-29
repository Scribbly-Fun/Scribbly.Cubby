using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;

[Collection(Collections.GrpcCollection)]
public class Service_Resolver_Tests(GrpcApplicationFactory application)
{
    [Fact]
    public void Transport_Should_Be_CubbyGrpcTransport()
    {
        var transport = application.Services.GetRequiredService<ICubbyStoreTransport>();

        transport.Should().BeAssignableTo<CubbyGrpcTransport>();
    }
    
    [Fact]
    public void Client_Should_Be_CubbyClient()
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        client.Should().BeAssignableTo<CubbyClient>();
    }
    
    [Fact]
    public void GrpcClient_Should_Be_GrpcCubbyClient()
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IGrpcCubbyClient>();

        client.Should().BeAssignableTo<GrpcCubbyClient>();
    }
    
    [Fact]
    public void CubbyGrpcTransport_Should_NotBe_Null()
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetService<CubbyGrpcTransport>();

        client.Should().NotBeNull();
    }
    
}