namespace Scribbly.Cubby.Client.Serializer;

internal sealed class GzipCubbyCompressor : ICubbyCompressor
{
    public ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> source) => source.GzipCompress();
    public ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> source) => source.GzipDecompress();
}