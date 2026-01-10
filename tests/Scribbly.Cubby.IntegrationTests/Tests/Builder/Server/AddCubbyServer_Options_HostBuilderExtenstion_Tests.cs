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
    [Theory]
    [InlineData(100)]
    [InlineData(98)]
    [InlineData(100_999)]
    [InlineData(22_684)]
    public void Given_Capacity_Value_Options_Should_HaveCapacity(int cap)
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Capacity = cap;
            })
            .WithCubbyHttpServer();
        
        using var host = builder.Build();
        var serviceProvider = host.Services;

        serviceProvider.GetRequiredService<IOptions<CubbyServerOptions>>().Value.Capacity.Should().Be(cap);
    }

    [Theory]
    [InlineData(65)]
    [InlineData(98)]
    [InlineData(100_999)]
    [InlineData(22_684)]
    public void Given_Cores_Value_GreaterThan_64_Options_Validation_Should_Throw_OutOfRange(int cores)
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Cores = cores;
            })
            .WithCubbyHttpServer();

        var act = () =>
        {
            using var host = builder.Build();
            host.Run();
        };

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-64)]
    public void Given_Cores_Value_LessThanOrEqual_0_Options_Validation_Should_Throw_OutOfRange(int cores)
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Cores = cores;
            })
            .WithCubbyHttpServer();

        var act = () =>
        {
            using var host = builder.Build();
            host.Run();
        };

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(22)]
    [InlineData(64)]
    public void Given_Cores_Value_InRange_Options_Should_HaveCores(int cores)
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Cores = cores;
            })
            .WithCubbyHttpServer();
        
        using var host = builder.Build();
        var serviceProvider = host.Services;

        serviceProvider.GetRequiredService<IOptions<CubbyServerOptions>>().Value.Cores.Should().Be(cores);
    }
    
    [Fact]
    public void Given_ZeroDelay_WithDurationStrategy_Options_Validation_Should_Throw_OutOfRange()
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        builder.AddCubbyServer(ops =>
            {
                ops.Cleanup = new CacheCleanupOptions
                {
                    Strategy = CacheCleanupOptions.AsyncStrategy.Duration,
                    Delay = TimeSpan.Zero
                };
            })
            .WithCubbyHttpServer();

        var act = () =>
        {
            using var host = builder.Build();
            host.Run();
        };

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
    
    [Theory]
    [InlineData("00:02:55")]
    [InlineData("12:55:59")]
    [InlineData("00:00:00:22")]
    public void Given_Delay_Value_Options_Should_HaveDelay(string delay)
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();

        var time = TimeSpan.Parse(delay);

        builder.AddCubbyServer(ops =>
            {
                ops.Cleanup = new CacheCleanupOptions
                {
                    Strategy = CacheCleanupOptions.AsyncStrategy.Duration,
                    Delay = time
                };
            })
            .WithCubbyHttpServer();
        
        using var host = builder.Build();
        var serviceProvider = host.Services;

        serviceProvider.GetRequiredService<IOptions<CubbyServerOptions>>().Value.Cleanup.Delay.Should().Be(time);
    }

    [Theory]
    [InlineData(CacheCleanupOptions.AsyncStrategy.Disabled)]
    [InlineData(CacheCleanupOptions.AsyncStrategy.Hourly)]
    [InlineData(CacheCleanupOptions.AsyncStrategy.Random)]
    [InlineData(CacheCleanupOptions.AsyncStrategy.Aggressive)]
    [InlineData(CacheCleanupOptions.AsyncStrategy.Duration)]
    [InlineData(CacheCleanupOptions.AsyncStrategy.Manual)]
    public void Given_Strategy_Value_Options_Should_HaveStrategy(CacheCleanupOptions.AsyncStrategy strategy)
    {
        var builder = new HostApplicationBuilder();
        builder.Configuration.Sources.Clear();
        
        builder.AddCubbyServer(ops =>
            {
                ops.Cleanup = new CacheCleanupOptions
                {
                    Strategy = strategy,
                    Delay = TimeSpan.FromMilliseconds(10)
                };
            })
            .WithCubbyHttpServer();
        
        using var host = builder.Build();
        var serviceProvider = host.Services;

        serviceProvider.GetRequiredService<IOptions<CubbyServerOptions>>().Value.Cleanup.Strategy.Should().Be(strategy);
    }
}