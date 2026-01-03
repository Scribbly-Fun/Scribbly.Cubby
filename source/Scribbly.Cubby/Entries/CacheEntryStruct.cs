using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// A ref struct used to align the bytes in the buffer stored in a cache store
/// </summary>
public readonly ref struct CacheEntryStruct : ICacheEntry
{
    private readonly byte[] _buffer;

    /// <inheritdoc />
    public long ExpirationUtcTicks
        => BinaryPrimitives.ReadInt64LittleEndian(_buffer);

    /// <inheritdoc />
    public bool NeverExpires
        => ExpirationUtcTicks == 0;
    
    /// <inheritdoc />
    public CacheEntryFlags Flags
        => (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(EntryLayout.FlagsPosition));
 
    /// <inheritdoc />
    public CacheEntryEncoding Encoding 
        => (CacheEntryEncoding)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(EntryLayout.EncodingPosition));
    
    /// <inheritdoc />
    public int ValueLength
        => BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(EntryLayout.ValueLengthPosition));

    /// <inheritdoc />
    public ReadOnlySpan<byte> Value
        => _buffer.AsSpan(EntryLayout.HeaderSize, ValueLength);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ValueMemory
        => new(_buffer, EntryLayout.HeaderSize, ValueLength);

    /// <summary>
    /// Creates a ref struct used to parse and slice the byte array into segments.
    /// </summary>
    /// <param name="buffer"></param>
    public CacheEntryStruct(byte[] buffer)
    {
        _buffer = buffer;
    }
    
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsExpired(long nowUtcTicks)
    {
        var expiresAt = BinaryPrimitives.ReadInt64LittleEndian(_buffer);
        return expiresAt != 0 && expiresAt <= nowUtcTicks;
    }
    
    /// <summary>
    /// Updates the sliding time stored in the internal buffer
    /// </summary>
    /// <param name="time">Timespan in Ticks</param>
    public void UpdateSlidingTime(long time)
    {
        var span = _buffer.AsSpan();
        BinaryPrimitives.WriteInt64LittleEndian(span, time);
    }
    
    /// <summary>
    /// Returns a span for the backing buffer
    /// </summary>
    public Span<byte> AsSpan => _buffer.AsSpan();
}