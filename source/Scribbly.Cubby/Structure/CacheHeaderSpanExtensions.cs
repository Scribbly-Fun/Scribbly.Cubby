using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// Extension methods used parse and process the header.
/// </summary>
public static class CacheHeaderSpanExtensions
{
    /// <summary>
    /// The cache entry span
    /// </summary>
    /// <param name="cacheSpan">A span pointing to the cache buffer</param>
    extension(Span<byte> cacheSpan)
    {
        internal CacheEntryFlags Flags => cacheSpan.GetFlags();

        /// <summary>
        /// Gets the first 24 bytes from the span.
        /// </summary>
        /// <returns>The header as a span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span<byte> GetHeader()
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(cacheSpan.Length, EntryLayout.HeaderSize);
            
            return cacheSpan[..EntryLayout.HeaderSize];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetValueLength() 
            => BinaryPrimitives.ReadInt32LittleEndian(cacheSpan[EntryLayout.ValueLengthPosition..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CacheEntryFlags GetFlags() => 
            (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(cacheSpan[EntryLayout.FlagsPosition..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CacheEntryEncoding GetEncoding() 
            => (CacheEntryEncoding)BinaryPrimitives.ReadInt16LittleEndian(cacheSpan[EntryLayout.EncodingPosition..]);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long GetExpiration() 
            => BinaryPrimitives.ReadInt64LittleEndian(cacheSpan);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long GetSlidingDuration() 
            => BinaryPrimitives.ReadInt64LittleEndian(cacheSpan[EntryLayout.DurationPosition..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsNeverExpiringEntry() 
            => BinaryPrimitives.ReadInt64LittleEndian(cacheSpan) == 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsNeverExpiringEntry([NotNullWhen(returnValue: false)] out long? expirationTicks)
        {
            expirationTicks = cacheSpan.GetExpiration();
            return expirationTicks == 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsExpired(long nowUtcTicks)
        {
            var expiresAt = cacheSpan.GetExpiration();
            return expiresAt != 0 && expiresAt <= nowUtcTicks;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsSlidingEntry()
        {
            var flags = cacheSpan.GetFlags();
            return (flags & CacheEntryFlags.Sliding) != 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void UpdateSlidingTime(long nowUtcTicks)
        {
            var duration = BinaryPrimitives.ReadInt64LittleEndian(cacheSpan.Slice(EntryLayout.DurationPosition, 8));
            BinaryPrimitives.WriteInt64LittleEndian(cacheSpan, nowUtcTicks + duration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void UpdateFlags(CacheEntryFlags flags)
        {
            BinaryPrimitives.WriteInt16LittleEndian(cacheSpan[EntryLayout.FlagsPosition..], (short)flags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsTombstoneOrExpired(long nowUtcTicks)
        {
            var flags = cacheSpan.GetFlags();

            if ((flags & CacheEntryFlags.Tombstone) != 0)
                return true;

            var expiresAt = cacheSpan.GetExpiration();

            return expiresAt != 0 && expiresAt <= nowUtcTicks;
        }
    }

    /// <summary>
    /// The cache entry span
    /// </summary>
    /// <param name="cacheSpan">A span pointing to the cache buffer</param>
    extension(ReadOnlySpan<byte> cacheSpan)
    {
        internal CacheEntryFlags Flags => cacheSpan.GetFlags();

        /// <summary>
        /// Gets the first 24 bytes from the span.
        /// </summary>
        /// <returns>The header as a span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlySpan<byte> GetHeader()
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(cacheSpan.Length, EntryLayout.HeaderSize);
            
            return cacheSpan[..EntryLayout.HeaderSize];
        }

        /// <summary>
        /// Gets the value stored in the buffer.
        /// </summary>
        /// <returns>The header as a span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlySpan<byte> GetValue()
        {
            var length = BinaryPrimitives.ReadInt32LittleEndian(cacheSpan[EntryLayout.ValueLengthPosition..]);
            return cacheSpan.Slice(EntryLayout.HeaderSize, length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CacheEntryFlags GetFlags() => 
            (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(cacheSpan[EntryLayout.FlagsPosition..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CacheEntryEncoding GetEncoding() 
            => (CacheEntryEncoding)BinaryPrimitives.ReadInt16LittleEndian(cacheSpan[EntryLayout.EncodingPosition..]);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long GetExpiration() 
            => BinaryPrimitives.ReadInt64LittleEndian(cacheSpan);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long GetSlidingDuration() 
            => BinaryPrimitives.ReadInt64LittleEndian(cacheSpan[EntryLayout.DurationPosition..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsNeverExpiringEntry() 
            => BinaryPrimitives.ReadInt64LittleEndian(cacheSpan) == 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsNeverExpiringEntry([NotNullWhen(returnValue: false)] out long? expirationTicks)
        {
            expirationTicks = cacheSpan.GetExpiration();
            return expirationTicks == 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsExpired(long nowUtcTicks)
        {
            var expiresAt = cacheSpan.GetExpiration();
            return expiresAt != 0 && expiresAt <= nowUtcTicks;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsSlidingEntry()
        {
            var flags = cacheSpan.GetFlags();
            return (flags & CacheEntryFlags.Sliding) != 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsTombstoneOrExpired(long nowUtcTicks)
        {
            var flags = cacheSpan.GetFlags();

            if ((flags & CacheEntryFlags.Tombstone) != 0)
                return true;

            var expiresAt = cacheSpan.GetExpiration();

            return expiresAt != 0 && expiresAt <= nowUtcTicks;
        }
    }
}