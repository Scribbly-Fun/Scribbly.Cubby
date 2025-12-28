using System.Buffers;
using Google.Protobuf;

namespace Scribbly.Cubby;

/// <summary>
/// Extensions on a gRPC byte string array
/// </summary>
public static class ByteStringExtensions
{
    /// <summary>
    /// The byte string to operate on
    /// </summary>
    /// <param name="source">the source byte string</param>
    extension(ByteString source)
    {
        /// <summary>
        /// Copies the value from source bytes using an Array pool
        /// </summary>
        /// <returns>A rented byt array</returns>
        public byte[] CopyFrom()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(source.Length);
            source.Span.CopyTo(buffer);
            return buffer;
        }
    }
}