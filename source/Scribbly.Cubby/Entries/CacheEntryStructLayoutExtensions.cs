using System.Buffers;
using System.Buffers.Binary;
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
    }
}