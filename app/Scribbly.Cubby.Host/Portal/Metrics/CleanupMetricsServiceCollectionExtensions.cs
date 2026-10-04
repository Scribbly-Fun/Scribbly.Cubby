using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Server.Background;

namespace Scribbly.Cubby.Host.Portal.Metrics;

internal static class CleanupMetricsServiceCollectionExtensions
{
    public static ICubbyServerBuilder AddCubbyCleanupMetrics(this ICubbyServerBuilder builder)
    {
        // Channel is unbounded so a runtime delay shrink cannot drop samples before the store.
        // The store is the capacity authority and resizes via IOptionsMonitor.
        var channel = Channel.CreateUnbounded<CleanupPassMetric>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false,
        });

        builder.HostBuilder.Services.AddSingleton(channel);
        builder.HostBuilder.Services.AddSingleton(channel.Reader);
        builder.HostBuilder.Services.AddSingleton(channel.Writer);

        builder.HostBuilder.Services.AddSingleton<CleanupMetricsStore>();
        builder.HostBuilder.Services.Replace(ServiceDescriptor.Singleton<ICleanupMetricsPublisher, ChannelCleanupMetricsPublisher>());

        builder.HostBuilder.Services.AddHostedService<CleanupMetricsChannelProcessor>();

        return builder;
    }
}
