namespace Scribbly.Cubby;

/// <summary>
/// An entry stored in the cache.
/// </summary>
/// <remarks>The primary purpose of this abstraction is to facilitate benchmarking storage operations.</remarks>
public interface ICacheEntry
{
    /// <summary>
    /// The time when the entry should be considered stale and removed.
    /// Note that a Zero value means this should never expire
    /// </summary>
    long ExpirationUtcTicks { get; }
    
    /// <summary>
    /// True when the cache will never expire
    /// </summary>
    bool NeverExpires => ExpirationUtcTicks == 0;
    
    /// <summary>
    /// Flags used to determine how the cache will be used.
    /// </summary>
    CacheEntryFlags Flags { get; }
    
    /// <summary>
    /// How the data was encoded before being serialized into bytes
    /// </summary>
    CacheEntryEncoding Encoding { get; }
    
    /// <summary>
    /// The length of the bytes in the value
    /// </summary>
    int ValueLength { get; }
    
    /// <summary>
    /// The value bytes slice from the internal buffer
    /// </summary>
    ReadOnlySpan<byte> Value { get; }
    
    /// <summary>
    /// The value bytes slice from the internal buffer
    /// </summary>
    ReadOnlyMemory<byte> ValueMemory { get; }

    /// <summary>
    /// True when the cache entry has expired.
    /// </summary>
    /// <param name="nowUtcTicks">The current time in ticks</param>
    /// <returns>True if expired.</returns>
    bool IsExpired(long nowUtcTicks);
}