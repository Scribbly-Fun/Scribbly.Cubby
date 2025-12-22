using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// A cached entry is a collection of bytes with a formatted header.
/// [0-7 Expiration][8-11 Value Length][12-13 Key Length][14-15 Flags][Key ^ Length][Value ^ Length]
/// </summary>
public sealed class BufferedCacheEntry : ICacheEntry
{
    /// <summary>
    /// The size of the header.
    /// </summary>
    private const int HeaderSize = 16;
    
    private const int ValueLengthPosition = 8;
    private const int KeyLengthPosition = 12;
    private const int FlagPosition = 14;

    private byte[]? _buffer;

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
    

    private BufferedCacheEntry(Span<byte> buffer)
    {
        _buffer = buffer.ToArray();
    }

    /// <summary>
    /// Creates a new cache entry with custom configuration.
    /// </summary>
    /// <returns>The new cache entry</returns>
    public static BufferedCacheEntry Create(
        Span<byte> buffer,
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> value,
        long expirationUtcTicks = 0,
        CacheEntryFlags flags = CacheEntryFlags.None)
    {
        if (buffer.Length < (HeaderSize + key.Length + value.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "The buffer provided is not large enough to hold all data.");
        }
        
        ArgumentOutOfRangeException.ThrowIfNegative(expirationUtcTicks);
        
        BinaryPrimitives.WriteInt64LittleEndian(buffer, expirationUtcTicks);
        BinaryPrimitives.WriteInt32LittleEndian(buffer[ValueLengthPosition..], value.Length);
        BinaryPrimitives.WriteInt32LittleEndian(buffer[KeyLengthPosition..], key.Length);
        BinaryPrimitives.WriteInt16LittleEndian(buffer[FlagPosition..], (short)flags);
        
        key.CopyTo(buffer[HeaderSize..]);
        value.CopyTo(buffer[(HeaderSize + key.Length)..]);
        
        return new BufferedCacheEntry(buffer);
    }

    /// <summary>
    /// Creates a new cache entry that never expires
    /// </summary>
    public static BufferedCacheEntry CreateNeverExpiring(Span<byte> buffer, ReadOnlySpan<byte> key, ReadOnlySpan<byte> value) 
        => Create(buffer, key, value, expirationUtcTicks: 0);

    /// <summary>
    /// Creates a new cached entry with a time to live.
    /// </summary>
    public static BufferedCacheEntry CreateWithTtl(
        Span<byte> buffer, 
        ReadOnlySpan<byte> key, 
        ReadOnlySpan<byte> value,
        TimeSpan ttl,
        long nowUtcTicks) 
        => ttl <= TimeSpan.Zero 
            ? throw new ArgumentOutOfRangeException(nameof(ttl)) 
            : Create(buffer, key, value, nowUtcTicks + ttl.Ticks);

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
        _buffer = null;
    }
}