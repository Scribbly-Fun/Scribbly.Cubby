using System.Diagnostics;
using Microsoft.Extensions.Logging;

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
    public void CleanCacheStorage(long nowUtcTicks)
    {
        if (store.ActiveWriters > 0 && Random.Shared.Next(4) != 0)
        {
            logger.LogCleanupSkipped();
            return;
        }
        
        var start = Stopwatch.GetTimestamp();
        var deadline = start + MaxTicks;

        var iterations = 0;

        foreach (var dict in store.Entries)
        {
            if ((++iterations & 0x3F) == 0 &&
                Stopwatch.GetTimestamp() > deadline)
            {
                logger.LogCleanupDeadline(iterations, deadline);
                return;
            }
            
            if (dict.Value.IsTombstoneOrExpired(nowUtcTicks))
            {
                var eviction = store.Evict(dict.Key);
                logger.LogEntryCleared(dict.Key, eviction);
            }
        }
    }
}