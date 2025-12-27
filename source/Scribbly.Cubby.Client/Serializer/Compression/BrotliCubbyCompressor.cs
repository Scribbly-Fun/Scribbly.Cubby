namespace Scribbly.Cubby.Client.Serializer;

/// <inheritdoc />
internal sealed class BrotliCubbyCompressor : ICubbyCompressor
{
    public ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> source) => source.BrotliCompress();
    public ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> source) => source.BrotliDecompress();
}