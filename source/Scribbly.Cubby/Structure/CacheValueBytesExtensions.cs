using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// Extension methods used parse and process the value.
/// </summary>
public static class CacheValueBytesExtensions
{
    /// <summary>
    /// The cache entry span
    /// </summary>
    /// <param name="cacheArray">An array from the cache</param>
    extension(byte[] cacheArray)
    {
        /// <summary>
        /// Gets the value stored in the buffer.
        /// </summary>
        /// <returns>The header as a span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span<byte> GetCachedValue()
        {
            var span = cacheArray.AsSpan();
            var length = BinaryPrimitives.ReadInt32LittleEndian(span[EntryLayout.ValueLengthPosition..]);
            return span.Slice(EntryLayout.HeaderSize, length);
        }
    }
}