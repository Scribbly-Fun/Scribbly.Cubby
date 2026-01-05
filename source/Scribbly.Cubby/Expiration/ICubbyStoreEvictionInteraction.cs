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
}