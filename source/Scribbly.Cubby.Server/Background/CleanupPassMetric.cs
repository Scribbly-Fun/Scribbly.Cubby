namespace Scribbly.Cubby.Server.Background;

/// <summary>
/// A cleanup pass metric published by <see cref="CacheCleanupAsyncProcessor"/> for asynchronous aggregation.
/// </summary>
/// <param name="Timestamp">UTC timestamp when the pass completed.</param>
/// <param name="SampleDelay">The processor delay that preceded this pass (drives chart resolution).</param>
/// <param name="ItemsTotal">Approximate number of items in the store at the start of the pass.</param>
/// <param name="ItemsServiced">Number of items visited before an early exit or completion.</param>
/// <param name="RemovedTombstone">Entries removed because they were tombstoned.</param>
/// <param name="RemovedExpired">Entries removed because of absolute expiry.</param>
/// <param name="RemovedSliding">Entries removed because a sliding window expired.</param>
/// <param name="SkippedDueToWriters">True when the pass was skipped to avoid writer contention.</param>
/// <param name="HitDeadline">True when the pass exited early after exceeding its time budget.</param>
/// <param name="Duration">Wall-clock duration of the pass.</param>
internal readonly record struct CleanupPassMetric(
    DateTimeOffset Timestamp,
    TimeSpan SampleDelay,
    int ItemsTotal,
    int ItemsServiced,
    int RemovedTombstone,
    int RemovedExpired,
    int RemovedSliding,
    bool SkippedDueToWriters,
    bool HitDeadline,
    TimeSpan Duration);
