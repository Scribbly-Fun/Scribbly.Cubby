using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// A cached entry is a collection of bytes with a formatted header.
/// [0-7 Expiration][8-11 Value Length][12-16 Flags][15 ^ Value]
/// </summary>
public sealed class PooledCacheEntry : ICacheEntry, IDisposable
{
    private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;
    
    private byte[]? _buffer;

    /// <inheritdoc />
    public long ExpirationUtcTicks 
        => BinaryPrimitives.ReadInt64LittleEndian(_buffer);

    /// <inheritdoc />
    public int ValueLength 
        => BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(EntryLayout.ValueLengthPosition));

    /// <inheritdoc />
    public CacheEntryFlags Flags 
        => (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(EntryLayout.FlagsPosition));

    /// <inheritdoc />
    public CacheEntryEncoding Encoding 
        => (CacheEntryEncoding)BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(EntryLayout.EncodingPosition));
    
    /// <inheritdoc />
    public ReadOnlySpan<byte> Value 
        => _buffer.AsSpan(EntryLayout.HeaderSize, ValueLength);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ValueMemory
        => new(_buffer, EntryLayout.HeaderSize, ValueLength);
    
    private PooledCacheEntry(byte[] buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new cache entry with custom configuration.
    /// </summary>
    /// <param name="value">The data to cache</param>
    /// <param name="expirationUtcTicks">An optional expiration</param>
    /// <param name="flags">Flags to define how the cache will be used.</param>
    /// <param name="encoding">Encoding flag</param>
    /// <returns>The new cache entry</returns>
    public static PooledCacheEntry Create(
        ReadOnlySpan<byte> value,
        long expirationUtcTicks = 0,
        CacheEntryFlags flags = CacheEntryFlags.None,
        CacheEntryEncoding encoding = CacheEntryEncoding.None)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(expirationUtcTicks);
        
        var buffer = Pool.Rent(EntryLayout.HeaderSize + value.Length);

        var span = buffer.AsSpan();

        BinaryPrimitives.WriteInt64LittleEndian(span, expirationUtcTicks);
        BinaryPrimitives.WriteInt32LittleEndian(span[EntryLayout.ValueLengthPosition..], value.Length);
        BinaryPrimitives.WriteInt16LittleEndian(span[EntryLayout.FlagsPosition..], (short)flags);
        BinaryPrimitives.WriteInt16LittleEndian(span[EntryLayout.EncodingPosition..], (short)encoding);
        
        value.CopyTo(span[EntryLayout.HeaderSize..]);
        return new PooledCacheEntry(buffer);
    }

    /// <summary>
    /// Creates a new cache entry that never expires
    /// </summary>
    /// <param name="value">The value to cache</param>
    /// <param name="encoding"></param>
    /// <returns>The new cache entry</returns>
    public static PooledCacheEntry CreateNeverExpiring(
        ReadOnlySpan<byte> value,
        CacheEntryEncoding encoding = CacheEntryEncoding.None) 
        => Create(value, expirationUtcTicks: 0, encoding: encoding);

    /// <summary>
    /// Creates a new cached entry with a time to live.
    /// </summary>
    /// <param name="value">The data to cache</param>
    /// <param name="ttl">The time in ticks before the cache should be marked for eviction</param>
    /// <param name="nowUtcTicks">The current time</param>
    /// <param name="encoding"></param>
    /// <returns>The new cache entry</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the ttl provided is less than 0</exception>
    public static PooledCacheEntry CreateWithTtl(
        ReadOnlySpan<byte> value,
        TimeSpan ttl,
        long nowUtcTicks,
        CacheEntryEncoding encoding = CacheEntryEncoding.None) 
        => ttl <= TimeSpan.Zero 
            ? throw new ArgumentOutOfRangeException(nameof(ttl)) 
            : Create(value, nowUtcTicks + ttl.Ticks, encoding: encoding);

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