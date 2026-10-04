using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scribbly.Cubby.Server.Background;

namespace Scribbly.Cubby.Host.Portal.Metrics;

internal static class CleanupMetricsServiceCollectionExtensions
{
    public static IServiceCollection AddCubbyCleanupMetrics(this IServiceCollection services)
    {
        var channel = Channel.CreateBounded<CleanupPassMetric>(new BoundedChannelOptions(CleanupMetricsStore.DefaultCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false,
        });

        services.AddSingleton(channel);
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);
        services.AddSingleton<CleanupMetricsStore>();
        services.Replace(ServiceDescriptor.Singleton<ICleanupMetricsPublisher, ChannelCleanupMetricsPublisher>());
        services.AddHostedService<CleanupMetricsChannelProcessor>();
        return services;
    }
}
