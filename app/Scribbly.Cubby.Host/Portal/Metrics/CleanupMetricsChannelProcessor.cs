using System.Threading.Channels;
using Scribbly.Cubby.Server.Background;

namespace Scribbly.Cubby.Host.Portal.Metrics;

/// <summary>
/// Consumes cleanup metrics from the channel and stores them for portal queries.
/// </summary>
internal sealed class CleanupMetricsChannelProcessor(
    ChannelReader<CleanupPassMetric> reader,
    CleanupMetricsStore store,
    ILogger<CleanupMetricsChannelProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var metric in reader.ReadAllAsync(stoppingToken))
            {
                store.Push(metric);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("Cleanup metrics channel processor shutting down");
        }
    }
}
