namespace Scribbly.Cubby;

public interface ICacheEntry : IDisposable
{
    long ExpirationUtcTicks { get; }
    bool NeverExpires { get; }
    int ValueLength { get; }
    CacheEntryFlags Flags { get; }
    ReadOnlySpan<byte> Value { get; }
    ReadOnlyMemory<byte> ValueMemory { get; }

    /// <summary>
    /// True when the cache entry has expired.
    /// </summary>
    /// <param name="nowUtcTicks">The current time in ticks</param>
    /// <returns>True if expired.</returns>
    bool IsExpired(long nowUtcTicks);
}