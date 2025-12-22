namespace Scribbly.Cubby.Stores;

/// <summary>
/// Options used when inserting elements into the cache.
/// </summary>
public sealed record CacheEntryOptions
{
    /// <summary>
    /// The amount of time in ticks before the cache will expire.
    /// </summary>
    public long Tll { get; } = 0;

    /// <summary>
    /// Creates a new option with no TTL
    /// </summary>
    public static CacheEntryOptions Never => new ();
}