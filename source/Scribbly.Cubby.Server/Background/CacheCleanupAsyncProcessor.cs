using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Server.Background;

internal class CacheCleanupAsyncProcessor(ILogger<CacheCleanupAsyncProcessor> logger, TimeProvider provider, IExpirationEvictionService evictionService, IOptionsMonitor<CubbyOptions> options) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = CalculateDelay(options.CurrentValue.CacheCleanupOptions);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = provider.GetUtcNow();
            if (delay == 0)
            {
                evictionService.CleanCacheStorage(now.UtcTicks);
            }
            else
            {
                    
            } 
        }
    }

    private long CalculateDelay(CacheCleanupOptions cleanupOptions) =>
        cleanupOptions switch
        {
            { Strategy: CacheCleanupOptions.AsyncStrategy.Hourly }
                => CacheCleanupOptions.Hour,
            { Strategy: CacheCleanupOptions.AsyncStrategy.Random }
                => Random.Shared.NextInt64(CacheCleanupOptions.MinRandom, CacheCleanupOptions.MaxRandom),
            { Strategy: CacheCleanupOptions.AsyncStrategy.Aggressive }
                => 0,
            _ => throw new InvalidOperationException("Background Service is Running with Strategy Disabled"),
        };
}

internal static partial class CacheCleanupAsyncLogger
{
    [LoggerMessage(EventId = 2000, Level = LogLevel.Information,Message = "Executing Heartbeat Processor Delay: {Interval} Quantity: {Agents}")]
    public static partial void LogHeartbeatRun(
        this ILogger logger,
        TimeSpan interval,
        int agents);

    [LoggerMessage(EventId = 2001, Level = LogLevel.Debug,Message = "Executing Heartbeat Processor {Heartbeat}")]
    public static partial void LogHeartbeatQuery(
        this ILogger logger,
        HeartbeatState heartbeat);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Debug, Message = "Calculated Heartbeat Processor {Reason}")]
    public static partial void LogHeartbeatOutcome(
        this ILogger logger,
        HeartbeatState.Reason reason);
}