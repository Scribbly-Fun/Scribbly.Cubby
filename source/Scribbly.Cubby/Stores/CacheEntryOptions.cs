namespace Scribbly.Cubby.Stores;

/// <summary>
/// Options used to configure a cache entry
/// </summary>
public sealed record CacheEntryOptions
{
    /// <summary>
    /// A time to live from the current datetime UTC now in ticks.
    /// </summary>
    public long Tll { get; } = 0;

    /// <summary>
    /// Creates a entry option with no TTL.
    /// </summary>
    public static CacheEntryOptions Never => new ();
}