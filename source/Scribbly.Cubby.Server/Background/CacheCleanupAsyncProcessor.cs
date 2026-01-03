using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Stores;
using static System.TimeSpan;

namespace Scribbly.Cubby.Server.Background;

/// <summary>
/// An async background worker used to poll and revoke cache entries.
/// </summary>
/// <param name="logger">A logger used to log the status of the processor</param>
/// <param name="provider">A time provider used to generate time queries.</param>
/// <param name="evictionService">A class responsible for clearing and querying the cache</param>
/// <param name="optionsMonitor">Options that can be updated at runtime.</param>
/// <remarks>
///     The cubby options are utilized to create several different strategies for cache revoking and memory cleanup.
///     Each configuration should be tested for your specific use case.
///     Exessive cleanup could cause locking contentions, while infrequent clean up could impact GC pressure.
/// </remarks>
internal class CacheCleanupAsyncProcessor(
    ILogger<CacheCleanupAsyncProcessor> logger, 
    TimeProvider provider, 
    IExpirationEvictionService evictionService, 
    IOptionsMonitor<CubbyOptions> optionsMonitor) : BackgroundService
{
    private TimeSpan _delay;
    
    private CancellationTokenSource? _delayCts;
    private readonly Lock _lock = new();
    
    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _delay = CalculateDelay(optionsMonitor.CurrentValue.Cleanup);
        
        _delayCts = new CancellationTokenSource();
        
        optionsMonitor.OnChange(OptionsUpdated);
        return base.StartAsync(cancellationToken);
    }

    private void OptionsUpdated(CubbyOptions options)
    {
        lock (_lock)
        {
            _delay = CalculateDelay(options.Cleanup);
            
            _delayCts?.Cancel();
            _delayCts?.Dispose();
            _delayCts = new CancellationTokenSource();

            logger.LogOptionsUpdated(_delay);
        }
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStartupMessage(optionsMonitor.CurrentValue.Store, _delay);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            CancellationToken delayToken;
            
            lock (_lock)
            {
                if (_delayCts is null)
                    return;

                delayToken = CancellationTokenSource
                    .CreateLinkedTokenSource(stoppingToken, _delayCts.Token)
                    .Token;
            }
            
            try
            {
                await Task.Delay(_delay, delayToken);
                
                if (delayToken.IsCancellationRequested)
                {
                    continue;
                }

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    ClearExpiredCacheAndTrace();
                    continue;
                }

                ClearExpiredCache();
            }
            catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Delay was updated, Resetting Loop");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cache Background Processor Encountered an Unhandled Error");
            }
        }
    }

    private void ClearExpiredCacheAndTrace()
    {
        var watch = Stopwatch.StartNew();
        var now = provider.GetUtcNow();
                
        evictionService.CleanCacheStorage(now.UtcTicks);
        
        watch.Stop();
        
        logger.LogIterationMessage(_delay, watch.Elapsed);
    }
    
    private void ClearExpiredCache()
    {
        var now = provider.GetUtcNow();
        evictionService.CleanCacheStorage(now.UtcTicks);
    }
    
    private static TimeSpan CalculateDelay(CacheCleanupOptions cleanupOptions) =>
        cleanupOptions switch
        {
            { Strategy: CacheCleanupOptions.AsyncStrategy.Hourly }
                => FromTicks(CacheCleanupOptions.Hour),
            
            { Strategy: CacheCleanupOptions.AsyncStrategy.Random }
                => FromTicks(Random.Shared.NextInt64(CacheCleanupOptions.MinRandom, CacheCleanupOptions.MaxRandom)),
            
            { Strategy: CacheCleanupOptions.AsyncStrategy.Aggressive }
                => FromTicks(CacheCleanupOptions.Aggressive),

            { Strategy: CacheCleanupOptions.AsyncStrategy.Duration }
                => cleanupOptions.Delay,
            
            _ => throw new InvalidOperationException("Background Service is Running with Strategy Disabled"),
        };
}