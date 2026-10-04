using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Expiration;

/// <summary>
/// Queries a cache store for records marked for removal.
/// </summary>
/// <param name="logger">A logger to log all evictions</param>
/// <param name="store">The cubby store to query</param>
internal class ExpirationEvictionService(ILogger<IExpirationEvictionService> logger, ICubbyStoreEvictionInteraction store) : IExpirationEvictionService
{
    private const long MaxTicks = TimeSpan.TicksPerMillisecond * 2;
    
    /// <summary>
    ///     Removes all entries from the cache that are either marked tombstone or expired
    /// </summary>
    /// <param name="nowUtcTicks">
    ///     The current datetime used to evaluate expiration
    /// </param>
    /// <remarks>
    ///     Iterates through all the entries in the cache when there no more than 4 active writers.
    ///     If the elapsed time is exceeded we can assume there are lots of cache hits active and exit the process.
    /// </remarks>
    public CleanupPassResult CleanCacheStorage(long nowUtcTicks)
    {
        var timestamp = new DateTimeOffset(nowUtcTicks, TimeSpan.Zero);
        var itemsTotal = store.TotalCount;
        var start = Stopwatch.GetTimestamp();

        if (store.ActiveWriters > 0 && Random.Shared.Next(4) != 0)
        {
            logger.LogCleanupSkipped();
            return new CleanupPassResult(
                timestamp,
                itemsTotal,
                ItemsServiced: 0,
                RemovedTombstone: 0,
                RemovedExpired: 0,
                RemovedSliding: 0,
                SkippedDueToWriters: true,
                HitDeadline: false,
                Duration: Stopwatch.GetElapsedTime(start));
        }
        
        var deadline = start + MaxTicks;

        var iterations = 0;
        var removedTombstone = 0;
        var removedExpired = 0;
        var removedSliding = 0;

        foreach (var dict in store.Entries)
        {
            if ((++iterations & 0x3F) == 0 &&
                Stopwatch.GetTimestamp() > deadline)
            {
                logger.LogCleanupDeadline(iterations, deadline);
                return new CleanupPassResult(
                    timestamp,
                    itemsTotal,
                    ItemsServiced: iterations,
                    removedTombstone,
                    removedExpired,
                    removedSliding,
                    SkippedDueToWriters: false,
                    HitDeadline: true,
                    Duration: Stopwatch.GetElapsedTime(start));
            }

            var header = dict.Value.GetHeader();
            var flags = header.GetFlags();

            if (flags.IsTombstone())
            {
                var eviction = store.Evict(dict.Key);
                if (eviction == EvictResult.Removed)
                {
                    removedTombstone++;
                    logger.LogEntryCleared(dict.Key, eviction);
                }
                continue;
            }

            if (!header.IsExpired(nowUtcTicks))
            {
                continue;
            }

            var evictionResult = store.Evict(dict.Key);
            if (evictionResult != EvictResult.Removed)
            {
                continue;
            }

            if (flags.IsSliding())
            {
                removedSliding++;
            }
            else
            {
                removedExpired++;
            }

            logger.LogEntryCleared(dict.Key, evictionResult);
        }

        return new CleanupPassResult(
            timestamp,
            itemsTotal,
            ItemsServiced: iterations,
            removedTombstone,
            removedExpired,
            removedSliding,
            SkippedDueToWriters: false,
            HitDeadline: false,
            Duration: Stopwatch.GetElapsedTime(start));
    }
}
