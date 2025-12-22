using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// A cached entry is a collection of bytes with a formatted header.
/// [0-8 Expiration][9-12 Value Length][13-16 Flags][Value]
/// </summary>
public sealed class CacheEntryNotPool : ICacheEntry
{
    /// <summary>
    /// The size of the header.
    /// </summary>
    private const int HeaderSize = 16;
    
    private const int ValueLengthPosition = 8;
    private const int KeyLengthPosition = 12;
    private const int FlagPosition = 14;
    
    private readonly byte[] _buffer;
    
    /// <inheritdoc />
    public byte[] Buffer => _buffer ?? [];
    
    public long ExpirationUtcTicks 
        => BinaryPrimitives.ReadInt64LittleEndian(_buffer);

    public bool NeverExpires 
        => ExpirationUtcTicks == 0;

    public int ValueLength 
        => BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(ValueLengthPosition));

    public int KeyLength 
        => BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(KeyLengthPosition));
    
    public CacheEntryFlags Flags 
        => (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(FlagPosition));

    /// <inheritdoc />
    public ReadOnlySpan<byte> Key
        => _buffer.AsSpan(HeaderSize, ValueLength);

    public ReadOnlySpan<byte> Value 
        => _buffer.AsSpan(HeaderSize + KeyLength, ValueLength);

    public ReadOnlyMemory<byte> ValueMemory
        => new(_buffer, HeaderSize, ValueLength);


    private CacheEntryNotPool(byte[] buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new cache entry with custom configuration.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value">The data to cache</param>
    /// <param name="expirationUtcTicks">An optional expiration</param>
    /// <param name="flags">Flags to define how the cache will be used.</param>
    /// <returns>The new cache entry</returns>
    public static CacheEntryNotPool Create(
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> value,
        long expirationUtcTicks = 0,
        CacheEntryFlags flags = CacheEntryFlags.None)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(expirationUtcTicks);

        var buffer = new byte[HeaderSize + value.Length];

        var span = buffer.AsSpan();

        BinaryPrimitives.WriteInt64LittleEndian(span, expirationUtcTicks);
        BinaryPrimitives.WriteInt32LittleEndian(span[8..], value.Length);
        BinaryPrimitives.WriteInt16LittleEndian(span[12..], (short)flags);
        
        value.CopyTo(span[HeaderSize..]);
        return new CacheEntryNotPool(buffer);
    }

    /// <summary>
    /// Creates a new cache entry that never expires
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value">The value to cache</param>
    /// <returns>The new cache entry</returns>
    public static CacheEntryNotPool CreateNeverExpiring(
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> value) 
        => Create(key, value, expirationUtcTicks: 0);

    /// <summary>
    /// Creates a new cached entry with a time to live.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value">The data to cache</param>
    /// <param name="ttl">The time in ticks before the cache should be marked for eviction</param>
    /// <param name="nowUtcTicks">The current time</param>
    /// <returns>The new cache entry</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the ttl provided is less than 0</exception>
    public static CacheEntryNotPool CreateWithTtl(
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> value,
        TimeSpan ttl,
        long nowUtcTicks) 
        => ttl <= TimeSpan.Zero 
            ? throw new ArgumentOutOfRangeException(nameof(ttl)) 
            : Create(key, value, nowUtcTicks + ttl.Ticks);

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
        // TODO release managed resources here
    }
}