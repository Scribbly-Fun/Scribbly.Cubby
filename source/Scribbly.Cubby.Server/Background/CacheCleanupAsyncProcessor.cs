using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Stores;
using static System.TimeSpan;

namespace Scribbly.Cubby.Server.Background;

internal class CacheCleanupAsyncProcessor(ILogger<CacheCleanupAsyncProcessor> logger, TimeProvider provider, IExpirationEvictionService evictionService, IOptionsMonitor<CubbyOptions> options) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = CalculateDelay(options.CurrentValue.CacheCleanupOptions);

        logger.LogStartupMessage(delay, options.CurrentValue.Store);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = provider.GetUtcNow();
            if (delay == MinValue)
            {
                evictionService.CleanCacheStorage(now.UtcTicks);
                continue;
            }
            await Task.Delay(delay, stoppingToken);
            
            logger.LogStartupMessage(delay, options.CurrentValue.Store);

            evictionService.CleanCacheStorage(now.UtcTicks);
            
            delay = CalculateDelay(options.CurrentValue.CacheCleanupOptions);
        }
    }

    private static TimeSpan CalculateDelay(CacheCleanupOptions cleanupOptions) =>
        cleanupOptions switch
        {
            { Strategy: CacheCleanupOptions.AsyncStrategy.Hourly }
                => FromTicks(CacheCleanupOptions.Hour),
            { Strategy: CacheCleanupOptions.AsyncStrategy.Random }
                => FromTicks(Random.Shared.NextInt64(CacheCleanupOptions.MinRandom, CacheCleanupOptions.MaxRandom)),
            { Strategy: CacheCleanupOptions.AsyncStrategy.Aggressive }
                => MinValue,
            _ => throw new InvalidOperationException("Background Service is Running with Strategy Disabled"),
        };
}

internal static partial class CacheCleanupAsyncLogger
{
    [LoggerMessage(EventId = 2000, Level = LogLevel.Information, Message = "Starting Cache Cleanup Background Processor: {IntervalDelay} Store: {Store}")]
    public static partial void LogStartupMessage(
        this ILogger logger,
        TimeSpan intervalDelay,
        CubbyOptions.StoreType store);

    [LoggerMessage(EventId = 2000, Level = LogLevel.Information, Message = "Executing Cache Cleanup Background Processor: {IntervalDelay}")]
    public static partial void LogIterationMessage(
        this ILogger logger,
        TimeSpan intervalDelay);
}