using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Expiration;

/// <summary>
/// Combines store functionality required to support automated eviction of cache entries.
/// </summary>
internal interface ICubbyStoreEvictionInteraction: ICubbyStoreIterator, ICubbyStoreEviction
{
    /// <summary>
    /// A volatile integer used to track active cache writers and prevent background processing lock contention.
    /// </summary>
    int ActiveWriters { get; }

    /// <summary>
    /// Approximate number of entries currently held by the store.
    /// Maintained with interlocked updates on insert/remove to avoid Count scans.
    /// </summary>
    int TotalCount { get; }
}