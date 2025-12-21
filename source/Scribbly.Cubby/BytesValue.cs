using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// A cached entry is a collection of bytes with a formatted header.
/// [0-8 Expiration][9-12 Value Length][13-16 Flags][Value]
/// </summary>
public readonly record struct CacheEntry
{
    public const int HeaderSize = 16;
    
    private readonly byte[] _buffer;
    
    private CacheEntry(byte[] buffer)
    {
        _buffer = buffer;
    }
    
    public static CacheEntry Create(
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
        return new CacheEntry(buffer);
    }

    public static CacheEntry CreateNeverExpiring(ReadOnlySpan<byte> value)
        => Create(value, expirationUtcTicks: 0);

    public static CacheEntry CreateWithTtl(
        ReadOnlySpan<byte> value,
        TimeSpan ttl,
        long nowUtcTicks)
    {
        return ttl <= TimeSpan.Zero 
            ? throw new ArgumentOutOfRangeException(nameof(ttl)) 
            : Create(value, nowUtcTicks + ttl.Ticks);
    }

    /* =========================
       Header Accessors
       ========================= */

    public long ExpirationUtcTicks
        => BinaryPrimitives.ReadInt64LittleEndian(_buffer);

    public bool NeverExpires => ExpirationUtcTicks == 0;

    public int ValueLength
        => BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(8));

    public CacheEntryFlags Flags
        => (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(12));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsExpired(long nowUtcTicks)
    {
        var span = _buffer.AsSpan();
        var expiresAt = BinaryPrimitives.ReadInt64LittleEndian(span);
        return expiresAt != 0 && expiresAt <= nowUtcTicks;
    }

    public ReadOnlySpan<byte> Value
        => _buffer.AsSpan(HeaderSize, ValueLength);

    public ReadOnlyMemory<byte> ValueMemory
        => new(_buffer, HeaderSize, ValueLength);
}

[Flags]
public enum CacheEntryFlags : short
{
    None            = 0,
    Compressed      = 1 << 0,
    Sliding         = 1 << 1,
    Tombstone       = 1 << 2
}