using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby;

/// <summary>
/// Extensions used to layout and create the backing buffer
/// </summary>
internal static class CacheEntryStructLayoutExtensions
{
    extension(ReadOnlySpan<byte> value)
    {
        internal byte[] LayoutEntry(CacheEntryOptions options)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(EntryLayout.HeaderSize + value.Length);
            var span = buffer.AsSpan(0, EntryLayout.HeaderSize + value.Length);

            BinaryPrimitives.WriteInt64LittleEndian(span, options.AbsoluteExpiration);
            BinaryPrimitives.WriteInt64LittleEndian(span[EntryLayout.DurationPosition..], options.SlidingDuration);
            BinaryPrimitives.WriteInt32LittleEndian(span[EntryLayout.ValueLengthPosition..], value.Length);
            BinaryPrimitives.WriteInt16LittleEndian(span[EntryLayout.FlagsPosition..], (short)options.Flags);
            BinaryPrimitives.WriteInt16LittleEndian(span[EntryLayout.EncodingPosition..], (short)options.Encoding);
        
            value.CopyTo(span[EntryLayout.HeaderSize..]);

            return buffer;
        }

        internal Span<byte> LayoutEntryAsSpan(CacheEntryOptions options)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(EntryLayout.HeaderSize + value.Length);
            var span = buffer.AsSpan(0, EntryLayout.HeaderSize + value.Length);

            BinaryPrimitives.WriteInt64LittleEndian(span, options.AbsoluteExpiration);
            BinaryPrimitives.WriteInt64LittleEndian(span[EntryLayout.DurationPosition..], options.SlidingDuration);
            BinaryPrimitives.WriteInt32LittleEndian(span[EntryLayout.ValueLengthPosition..], value.Length);
            BinaryPrimitives.WriteInt16LittleEndian(span[EntryLayout.FlagsPosition..], (short)options.Flags);
            BinaryPrimitives.WriteInt16LittleEndian(span[EntryLayout.EncodingPosition..], (short)options.Encoding);
        
            value.CopyTo(span[EntryLayout.HeaderSize..]);

            return span;
        }
    }

    extension(byte[] cacheData)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span<byte> GetHeader()
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(cacheData.Length, EntryLayout.HeaderSize);
            
            return cacheData.AsSpan(0, EntryLayout.HeaderSize);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsTombstoneOrExpired(long nowUtcTicks)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(cacheData.Length, EntryLayout.HeaderSize);
            
            var span = cacheData.AsSpan(0, EntryLayout.HeaderSize);

            var flags = (CacheEntryFlags)BinaryPrimitives.ReadInt16LittleEndian(span[EntryLayout.FlagsPosition..]);

            if ((flags & CacheEntryFlags.Tombstone) != 0)
                return true;

            var expiresAt = BinaryPrimitives.ReadInt64LittleEndian(span);

            return expiresAt != 0 && expiresAt <= nowUtcTicks;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsExpired(long nowUtcTicks, ReadOnlySpan<byte> span)
        {
            var expiresAt = BinaryPrimitives.ReadInt64LittleEndian(span);
            return expiresAt != 0 && expiresAt <= nowUtcTicks;
        }
    }
}