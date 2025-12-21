namespace Scribbly.Cubby;

/// <summary>
/// Flags declaring how the cache entry is used.
/// </summary>
[Flags]
public enum CacheEntryFlags : short
{
    /// <summary>
    /// Basic manual eviction cached record
    /// </summary>
    None            = 0,
    /// <summary>
    /// When set the data in the cache will be compressed.
    /// </summary>
    Compressed      = 1 << 0,
    /// <summary>
    /// When sliding each read on the cache will push the time out.
    /// </summary>
    Sliding         = 1 << 1,
    /// <summary>
    /// Marked tombstone when the cache entry should be evicted.
    /// </summary>
    Tombstone       = 1 << 2
}