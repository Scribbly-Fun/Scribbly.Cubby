using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Server.Background;

namespace Scribbly.Cubby.Host.Portal.Metrics;

internal static class CleanupMetricsServiceCollectionExtensions
{
    public static ICubbyServerBuilder AddCubbyCleanupMetrics(this ICubbyServerBuilder builder)
    {
        var channel = Channel.CreateBounded<CleanupPassMetric>(new BoundedChannelOptions(CleanupMetricsStore.DefaultCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
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
