using System.IO.Compression;

namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// Default compression methods used to compress and decompress data
/// </summary>
internal static class BrotliCompressionExtensions
{
    extension(ReadOnlySpan<byte> source)
    {
        internal ReadOnlySpan<byte> BrotliCompress()
        {
            using var ms = new MemoryStream();
            using (var brotli = new BrotliStream(ms, CompressionLevel.Fastest, leaveOpen: true))
            {
                brotli.Write(source);
                brotli.Flush();
            }
            return ms.ToArray();
        }

        internal ReadOnlySpan<byte> BrotliDecompress()
        {
            using var ms = new MemoryStream(source.ToArray());
            using var brotli = new BrotliStream(ms, CompressionMode.Decompress);
            using var outMs = new MemoryStream();
            brotli.CopyTo(outMs);
            return outMs.ToArray();
        }
    }
}