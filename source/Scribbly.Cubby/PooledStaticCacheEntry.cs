using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// A cached entry is a collection of bytes with a formatted header.
/// [0-8 Expiration][9-12 Value Length][13-16 Flags][Value]
/// </summary>
public sealed class PooledStaticCacheEntry : ICacheEntry
{
    private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;
    
    /// <summary>
    /// The size of the header.
    /// </summary>
    private const int HeaderSize = 16;
    
    private byte[]? _buffer;

    /// <inheritdoc />
    public long ExpirationUtcTicks 
        => BinaryPrimitives.ReadInt64LittleEndian(_buffer);

    /// <inheritdoc />
    public bool NeverExpires 
        => ExpirationUtcTicks == 0;

    /// <inheritdoc />
    public CacheEntryEncoding Encoding 
        => CacheEntryEncoding.None;

    /// <inheritdoc />
    public int ValueLength 
        => BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(8));

    /// <inheritdoc />
    public CacheEntryFlags Flags 
        => (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(12));
    
    /// <inheritdoc />
    public ReadOnlySpan<byte> Value 
        => _buffer.AsSpan(HeaderSize, ValueLength);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ValueMemory
        => new(_buffer, HeaderSize, ValueLength);
    
    private PooledStaticCacheEntry(byte[] buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new cache entry with custom configuration.
    /// </summary>
    /// <param name="value">The data to cache</param>
    /// <param name="expirationUtcTicks">An optional expiration</param>
    /// <param name="flags">Flags to define how the cache will be used.</param>
    /// <param name="pool">An array pool used for the cached buffer</param>
    /// <returns>The new cache entry</returns>
    public static PooledStaticCacheEntry Create(
        ReadOnlySpan<byte> value,
        long expirationUtcTicks = 0,
        CacheEntryFlags flags = CacheEntryFlags.None)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(expirationUtcTicks);
        
        var buffer = Pool.Rent(HeaderSize + value.Length);

        var span = buffer.AsSpan();

        BinaryPrimitives.WriteInt64LittleEndian(span, expirationUtcTicks);
        BinaryPrimitives.WriteInt32LittleEndian(span[8..], value.Length);
        BinaryPrimitives.WriteInt16LittleEndian(span[12..], (short)flags);
        
        value.CopyTo(span[HeaderSize..]);
        return new PooledStaticCacheEntry(buffer);
    }

    /// <summary>
    /// Creates a new cache entry that never expires
    /// </summary>
    /// <param name="value">The value to cache</param>
    /// <param name="pool">An array pool</param>
    /// <returns>The new cache entry</returns>
    public static PooledStaticCacheEntry CreateNeverExpiring(
        ReadOnlySpan<byte> value, 
        ArrayPool<byte>? pool = null) 
        => Create(value, expirationUtcTicks: 0);

    /// <summary>
    /// Creates a new cached entry with a time to live.
    /// </summary>
    /// <param name="value">The data to cache</param>
    /// <param name="ttl">The time in ticks before the cache should be marked for eviction</param>
    /// <param name="nowUtcTicks">The current time</param>
    /// <param name="pool">An array pool</param>
    /// <returns>The new cache entry</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the ttl provided is less than 0</exception>
    public static PooledStaticCacheEntry CreateWithTtl(
        ReadOnlySpan<byte> value,
        TimeSpan ttl,
        long nowUtcTicks,
        ArrayPool<byte>? pool = null) 
        => ttl <= TimeSpan.Zero 
            ? throw new ArgumentOutOfRangeException(nameof(ttl)) 
            : Create(value, nowUtcTicks + ttl.Ticks);

    /// <summary>
    /// True when the cache entry has expired.
    /// </summary>
    /// <param name="nowUtcTicks">The current time in ticks</param>
    /// <returns>True if expired.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsExpired(long nowUtcTicks)
    {
        var span = _buffer.AsSpan();
        var expiresAt = BinaryPrimitives.ReadInt64LittleEndian(span);
        return expiresAt != 0 && expiresAt <= nowUtcTicks;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer != null)
        {
            Pool.Return(buffer, clearArray: false);
        }
    }
}