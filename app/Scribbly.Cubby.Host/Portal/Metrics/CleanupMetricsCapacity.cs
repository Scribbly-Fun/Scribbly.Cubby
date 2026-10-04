using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal.Metrics;

/// <summary>
/// Computes how many cleanup samples to retain from the active cleanup delay.
/// Shorter delays require a larger ring to cover the same dashboard retention window.
/// </summary>
internal static class CleanupMetricsCapacity
{
    private const int MinCapacity = 64;
    
    /// <summary>
    /// Upper bound so aggressive sampling cannot allocate unbounded memory.
    /// </summary>
    private const int MaxCapacity = 100_000;
    
    /// <summary>
    /// Widest portal chart range (Last 3 Days).
    /// </summary>
    private static readonly TimeSpan RetentionWindow = TimeSpan.FromDays(3);
    

    public static int Compute(CacheCleanupOptions cleanup)
    {
        var interval = ResolveMinimumSampleInterval(cleanup);
        if (interval <= TimeSpan.Zero)
        {
            return MinCapacity;
        }

        var samples = RetentionWindow / interval;
        if (double.IsNaN(samples) || double.IsInfinity(samples))
        {
            return MaxCapacity;
        }

        var capacity = (int)Math.Ceiling(samples);
        return Math.Clamp(capacity, MinCapacity, MaxCapacity);
    }

    /// <summary>
    /// Resolves the shortest sampling interval implied by the current cleanup strategy.
    /// Used for capacity planning so Random/Aggressive cannot under-allocate.
    /// </summary>
    public static TimeSpan ResolveMinimumSampleInterval(CacheCleanupOptions cleanup) =>
        cleanup.Strategy switch
        {
            CacheCleanupOptions.AsyncStrategy.Hourly
                => TimeSpan.FromTicks(CacheCleanupOptions.Hour),
            CacheCleanupOptions.AsyncStrategy.Aggressive
                => TimeSpan.FromTicks(CacheCleanupOptions.Aggressive),
            CacheCleanupOptions.AsyncStrategy.Duration
                => cleanup.Delay > TimeSpan.Zero ? cleanup.Delay : TimeSpan.FromMinutes(1),
            CacheCleanupOptions.AsyncStrategy.Random
                => TimeSpan.FromTicks(CacheCleanupOptions.MinRandom),
            _ => TimeSpan.FromMinutes(1),
        };
}
