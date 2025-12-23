namespace Scribbly.Cubby.Stores;

/// <summary>
/// Options used when inserting elements into the cache.
/// </summary>
public sealed record CacheEntryOptions
{
    /// <summary>
    /// The amount of time in ticks before the cache will expire.
    /// </summary>
    public long TimeToLive { get; set; } = 0;

    /// <summary>
    /// Stores an encoding value in the cache store.
    /// When stored this will allow the admin portal to display and parse the cached values.
    /// </summary>
    public CacheEntryEncoding Encoding { get; set; } = CacheEntryEncoding.None;
    
    /// <summary>
    /// 
    /// </summary>
    public CacheEntryFlags Flags { get; set; } = CacheEntryFlags.None;

    /// <summary>
    /// Creates a new option with no TTL
    /// </summary>
    public static CacheEntryOptions Never => new ();
}