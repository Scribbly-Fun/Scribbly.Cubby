using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// Extension methods used parse and process the value.
/// </summary>
public static class CacheValueSpanExtensions
{
    /// <summary>
    /// The cache entry span
    /// </summary>
    /// <param name="cacheSpan">A span pointing to the cache buffer</param>
    extension(Span<byte> cacheSpan)
    {
        /// <summary>
        /// Gets the value stored in the buffer.
        /// </summary>
        /// <returns>The header as a span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span<byte> GetValue()
        {
            var length = BinaryPrimitives.ReadInt32LittleEndian(cacheSpan[EntryLayout.ValueLengthPosition..]);
            return cacheSpan.Slice(EntryLayout.HeaderSize, length);
        }
    }
}