using System.IO.Compression;

namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// Default compression methods used to compress and decompress data
/// </summary>
internal static class GzipCompressionExtensions
{
    extension(ReadOnlySpan<byte> source)
    {
        internal ReadOnlySpan<byte> GzipCompress()
        {
            using var ms = new MemoryStream();
            using (var gzip = new GZipStream(ms, CompressionLevel.Fastest, leaveOpen: true))
            {
                gzip.Write(source);
                gzip.Flush();
            }
            
            return ms.ToArray();
        }

        internal ReadOnlySpan<byte> GzipDecompress()
        {
            using var ms = new MemoryStream(source.ToArray());
            using var gzip = new GZipStream(ms, CompressionMode.Decompress);
            using var outMs = new MemoryStream();
            gzip.CopyTo(outMs);
            return outMs.ToArray();
        }
    }
}