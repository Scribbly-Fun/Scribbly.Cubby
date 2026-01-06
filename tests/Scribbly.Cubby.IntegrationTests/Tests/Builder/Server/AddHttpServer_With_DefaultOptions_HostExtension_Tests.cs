using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Builder.Server;

public class AddHttpServer_With_DefaultOptions_HostExtension_Tests : IAsyncLifetime
{
    private IHost _host = null!;
    private AsyncServiceScope _scope;
    private IServiceProvider _provider => _scope.ServiceProvider;
    
    [Fact]
    public void AddCubbyServer_Should_Register_ICubbyServerOptions() 
        => _provider.GetService<IOptions<CubbyServerOptions>>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyServer_Should_Register_CubbyStoreFactory() 
        => _provider.GetService<CubbyStoreFactory>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyServer_Should_Register_ICubbyStore()
        => _provider.GetService<ICubbyStore>().Should().NotBeNull();
    
    [Fact]
    public void AddCubbyServer_Should_Register_TimeProvider() 
        => _provider.GetService<TimeProvider>().Should().NotBeNull();
    

    /// <inheritdoc />
    public Task InitializeAsync()
    {
        var builder = new HostApplicationBuilder();

        builder.AddCubbyServer()
            .WithCubbyHttpServer();

        builder.Services.RemoveAll<IHostedService>();
        
        _host = builder.Build();
        var serviceProvider = _host.Services;
        
        _scope = serviceProvider.CreateAsyncScope();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
        _host.Dispose();
    }
}