using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Marshalled;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.IntegrationTests.Tests.Builder.Server;

public class CubbyStore_Factory_Tests
{
    [Fact]
    public void Given_Sharded_Option_Store_Should_BeSharded()
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Store = CubbyServerOptions.StoreType.Sharded;
            })
            .WithCubbyHttpServer();

        var app = builder.Build();

        var store = app.Services.GetRequiredService<CubbyStoreFactory>().CreateStore();

        store.Should().BeAssignableTo<ShardedConcurrentStore>();
    }

    [Fact]
    public void Given_Concurrent_Option_Store_Should_BeConcurrent()
    {
        var builder = new HostApplicationBuilder();
        
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Store = CubbyServerOptions.StoreType.Concurrent;
            })
            .WithCubbyHttpServer();

        var app = builder.Build();

        var store = app.Services.GetRequiredService<CubbyStoreFactory>().CreateStore();

        store.Should().BeAssignableTo<ConcurrentStore>();
    }

    [Fact]
    public void Given_Marshalled_Option_Store_Should_BeMarshalled()
    {
        var builder = new HostApplicationBuilder();
        
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Store = CubbyServerOptions.StoreType.Marshalled;
            })
            .WithCubbyHttpServer();

        var app = builder.Build();

        var store = app.Services.GetRequiredService<CubbyStoreFactory>().CreateStore();

        store.Should().BeAssignableTo<MarshalledStore>();
    }

    [Fact]
    public void Given_LockFree_Option_Store_Should_ThrowOutOfRangeException()
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
#pragma warning disable SCRB003
                ops.Store = CubbyServerOptions.StoreType.LockFree;
#pragma warning restore SCRB003
            })
            .WithCubbyHttpServer();

        var app = builder.Build();

        var act = () =>
        {
            app.Services.GetRequiredService<CubbyStoreFactory>().CreateStore();
        };

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}