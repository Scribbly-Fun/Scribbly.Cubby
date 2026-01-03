namespace Scribbly.Cubby.Stores;

/// <summary>
/// Options used when inserting elements into the cache.
/// </summary>
public sealed record CacheEntryOptions
{
    /// <summary>
    /// The amount of time in ticks before the cache will expire.
    /// </summary>
    /// <remarks>This will remain as a zero value when the entry is absolute</remarks>
    public long SlidingDuration { get; init; } = 0;
    
    /// <summary>
    /// A specified expiration time.
    /// </summary>
    /// <remarks>The absolute time will be used to calculate and determine expiration.</remarks>
    public long AbsoluteExpiration { get; init; } = 0;

    /// <summary>
    /// Stores an encoding value in the cache store.
    /// When stored this will allow the admin portal to display and parse the cached values.
    /// </summary>
    public CacheEntryEncoding Encoding { get; init; } = CacheEntryEncoding.None;
    
    /// <summary>
    /// 
    /// </summary>
    public CacheEntryFlags Flags { get; init; } = CacheEntryFlags.None;
    
    private CacheEntryOptions()
    {
        
    }
    
    /// <summary>
    /// Creates a new option with no TTL
    /// </summary>
    public static CacheEntryOptions None => new();

    /// <summary>
    /// Converts the raw value components to a new cache entry option.
    /// </summary>
    /// <param name="provider">A datetime provider to create time related data</param>
    /// <param name="flags">Cache entry flags</param>
    /// <param name="encoding">Optional encoding.</param>
    /// <param name="expiration">Optional expiration</param>
    /// <returns></returns>
    public static CacheEntryOptions From(TimeProvider provider, CacheEntryFlags flags,TimeSpan? expiration, CacheEntryEncoding encoding = CacheEntryEncoding.None) =>
        (flags, expiration) switch
        {
            (_, expiration: { Ticks: > 0 }) when flags.HasFlag(CacheEntryFlags.Sliding)
                => Sliding(provider, expiration.Value, flags, encoding),
            (_, expiration: { Ticks: > 0 })
                => Absolute(provider.GetUtcNow().Add(expiration.Value), flags, encoding),
            _ => Never(flags, encoding)
        };

    /// <summary>
    /// Creates a never expiring cache entry
    /// </summary>
    /// <param name="flags">Optional flags</param>
    /// <param name="encoding">Optional encoding</param>
    /// <returns>When the flags includes sliding</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static CacheEntryOptions Never(CacheEntryFlags flags = CacheEntryFlags.None, CacheEntryEncoding encoding = CacheEntryEncoding.None)
    {
        if (flags.HasFlag(CacheEntryFlags.Sliding))
        {
            throw new InvalidOperationException("Cannot specify a sliding time with a never expiration");
        }

        return new CacheEntryOptions
        {
            Flags = flags,
            Encoding = encoding
        };
    }

    /// <summary>
    /// Creates a sliding cache entry
    /// </summary>
    /// <param name="provider">A time provider used to generate time.</param>
    /// <param name="duration">The duration of time before the cache will expire</param>
    /// <param name="flags">Additional flags</param>
    /// <param name="encoding">Optional encoding information</param>
    /// <returns>A cache entry configuration</returns>
    /// <exception cref="ArgumentOutOfRangeException">The duration is zero</exception>
    public static CacheEntryOptions Sliding(TimeProvider provider, TimeSpan duration, CacheEntryFlags flags = CacheEntryFlags.None, CacheEntryEncoding encoding = CacheEntryEncoding.None)
        => Sliding(provider.GetUtcNow(), duration, flags, encoding);

    /// <summary>
    /// Creates a sliding cache entry
    /// </summary>
    /// <param name="now">The current time used to calculate the duration.</param>
    /// <param name="duration">The duration of time before the cache will expire</param>
    /// <param name="flags">Additional flags</param>
    /// <param name="encoding">Optional encoding information</param>
    /// <returns>A cache entry configuration</returns>
    /// <exception cref="ArgumentOutOfRangeException">The duration is zero</exception>
    public static CacheEntryOptions Sliding(DateTimeOffset now, TimeSpan duration, CacheEntryFlags flags = CacheEntryFlags.None, CacheEntryEncoding encoding = CacheEntryEncoding.None)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(duration, TimeSpan.Zero);

        return new CacheEntryOptions
        {
            Flags = CacheEntryFlags.Sliding | (flags & ~CacheEntryFlags.Sliding),
            Encoding = encoding,
            SlidingDuration = duration.Ticks,
            AbsoluteExpiration = now.Ticks + duration.Ticks
        };
    }
    
    /// <summary>
    /// Creates an Absolute cache entry
    /// </summary>
    /// <param name="provider">A time provider</param>
    /// <param name="expirationTime">The specific time the cache should expire</param>
    /// <param name="flags">Additional flags</param>
    /// <param name="encoding">Optional encoding information</param>
    /// <returns>A cache entry configuration</returns>
    /// <exception cref="ArgumentOutOfRangeException">The time is in the past</exception>
    public static CacheEntryOptions Absolute(TimeProvider provider, DateTimeOffset expirationTime, CacheEntryFlags flags = CacheEntryFlags.None, CacheEntryEncoding encoding = CacheEntryEncoding.None) 
        => Absolute(provider.GetUtcNow(), expirationTime, flags, encoding);
    
    /// <summary>
    /// Creates an Absolute cache entry
    /// </summary>
    /// <param name="expirationTime">The specific time the cache should expire</param>
    /// <param name="flags">Additional flags</param>
    /// <param name="encoding">Optional encoding information</param>
    /// <returns>A cache entry configuration</returns>
    /// <exception cref="ArgumentOutOfRangeException">The time is in the past</exception>
    public static CacheEntryOptions Absolute(DateTimeOffset expirationTime, CacheEntryFlags flags = CacheEntryFlags.None, CacheEntryEncoding encoding = CacheEntryEncoding.None)
        => Absolute(DateTimeOffset.UtcNow, expirationTime, flags, encoding);

    /// <summary>
    /// Creates an Absolute cache entry
    /// </summary>
    /// <param name="expirationTime">The specific time the cache should expire</param>
    /// <param name="now">The current time</param>
    /// <param name="flags">Additional flags</param>
    /// <param name="encoding">Optional encoding information</param>
    /// <returns>A cache entry configuration</returns>
    /// <exception cref="ArgumentOutOfRangeException">The time is in the past</exception>
    public static CacheEntryOptions Absolute(DateTimeOffset now, DateTimeOffset expirationTime, CacheEntryFlags flags = CacheEntryFlags.None, CacheEntryEncoding encoding = CacheEntryEncoding.None)
    {
        if (flags.HasFlag(CacheEntryFlags.Sliding))
        {
            throw new InvalidOperationException("Cannot specify a sliding absolute expiration entry");
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(expirationTime, now);

        return new CacheEntryOptions
        {
            Flags = flags,
            Encoding = encoding,
            AbsoluteExpiration = expirationTime.Ticks
        };
    }

    /// <summary>
    /// Converts the timespan to a sliding entry
    /// </summary>
    /// <param name="timespan">The sliding expiration time.</param>
    /// <returns>The sliding entry options</returns>
    public static implicit operator CacheEntryOptions(TimeSpan timespan) => Sliding(DateTimeOffset.UtcNow, timespan);
    
    /// <summary>
    /// Converts the timespan to an absolute expiration entry
    /// </summary>
    /// <param name="expiration">The expiration time</param>
    /// <returns>The absolute entry options</returns>
    public static implicit operator CacheEntryOptions(DateTimeOffset expiration) => Absolute(expiration);

    /// <summary>
    /// Converts the flags to a never expiration entry
    /// </summary>
    /// <param name="flags">The flags</param>
    /// <returns>The never expiration entry</returns>
    public static implicit operator CacheEntryOptions(CacheEntryFlags flags) => Never(flags);
}
