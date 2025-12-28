namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// Compresses and decompresses data.
/// </summary>
public interface ICubbyCompressor
{
    /// <summary>
    /// Compresses the source byte array
    /// </summary>
    /// <param name="source">The source data to compress</param>
    /// <returns>Compressed data</returns>
    ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> source);
    
    /// <summary>
    /// Decompresses the source data using the same algorithm it used to compress the data.
    /// </summary>
    /// <param name="source">The compressed source array</param>
    /// <returns>Uncompressed data</returns>
    ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> source);
}