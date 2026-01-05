using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.IntegrationTests.Tests.Builder.Client;

public class AddGrpcCubbyClient_With_DefaultOptions_ServiceExtension_Tests : IAsyncLifetime
{
    private AsyncServiceScope _scope;
    private IServiceProvider _provider => _scope.ServiceProvider;
    
    [Fact]
    public void AddCubbyClient_Should_Register_IOptionsCubby() 
        => _provider.GetService<IOptions<CubbyClientOptions>>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyClient_Should_Register_ICubbyCompressor() 
        => _provider.GetService<ICubbyCompressor>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyClient_Should_Register_ICubbyCompressor_As_BrotliCubbyCompressor()
        => _provider.GetService<ICubbyCompressor>().Should().BeAssignableTo<BrotliCubbyCompressor>();
    
    [Fact]
    public void AddCubbyClient_Should_Register_ICubbySerializer() 
        => _provider.GetService<ICubbySerializer>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyClient_Should_Register_ICubbySerializer_As_JsonCubbySerializer()
        => _provider.GetService<ICubbySerializer>().Should().BeAssignableTo<JsonCubbySerializer>();
    
    [Fact]
    public void AddCubbyClient_Should_Register_ICubbyClient() 
        => _provider.GetService<ICubbyClient>().Should().NotBeNull();

    [Fact]
    public void AddCubbyClient_Should_Register_ICubbyClient_As_CubbyClient()
        => _provider.GetService<ICubbyClient>().Should().BeAssignableTo<CubbyClient>();

    [Fact]
    public void AddCubbyClient_Should_Register_IGrpcCubbyClient() 
        => _provider.GetService<IGrpcCubbyClient>().Should().NotBeNull();

    [Fact]
    public void AddCubbyClient_Should_Register_IGrpcCubbyClient_As_GrpcCubbyClient()
        => _provider.GetService<IGrpcCubbyClient>().Should().BeAssignableTo<GrpcCubbyClient>();

    [Fact]
    public void AddCubbyClient_Should_Register_ICubbyStoreTransport()
        => _provider.GetService<ICubbyStoreTransport>().Should().NotBeNull();

    [Fact]
    public void AddCubbyClient_Should_Register_IDistributedCache()
        => _provider.GetService<IDistributedCache>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyClient_Should_Register_IDistributedCache_As_CubbyDistributedCache()
        => _provider.GetService<IDistributedCache>().Should().BeAssignableTo<CubbyDistributedCache>();

    /// <inheritdoc />
    public Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddCubbyClient(cb =>
        {
        }).WithCubbyGrpcClient();

        _scope = services.BuildServiceProvider().CreateAsyncScope();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}