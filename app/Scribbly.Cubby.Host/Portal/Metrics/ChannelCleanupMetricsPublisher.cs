using System.Threading.Channels;
using Scribbly.Cubby.Server.Background;

namespace Scribbly.Cubby.Host.Portal.Metrics;

/// <summary>
/// Publishes cleanup pass metrics onto a bounded channel without blocking the cleanup processor.
/// </summary>
internal sealed class ChannelCleanupMetricsPublisher(ChannelWriter<CleanupPassMetric> writer) : ICleanupMetricsPublisher
{
    public void Publish(in CleanupPassMetric metric)
    {
        writer.TryWrite(metric);
    }
}
