using FluentAssertions;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.UnitTests.Serializers.Compression;

public class Brotli_Compression_Tests
{
    [Theory]
    [InlineData(5_599)]
    [InlineData(65_535)]
    [InlineData(199_000)]
    [InlineData(2_199_999)]
    public void Given_ByteArray_Compress_Should_Reduce_Size(int length)
    {
        byte[] array = new byte[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = (byte)(i % 4);
        }

        var result = array.AsSpan().BrotliCompress();
        var resultLength = result.ToArray().Length;
        
        resultLength.Should().BeLessThan(length).And.NotBe(0);

    }

    [Theory]
    [InlineData(5_599)]
    [InlineData(65_535)]
    [InlineData(199_000)]
    [InlineData(2_199_999)]
    public void Given_Compressed_ByteArray_Decompress_Should_Match_Compressed(int length)
    {
        byte[] array = new byte[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = (byte)(i % 4);
        }

        var result = array.AsSpan().BrotliCompress();

        var decompressed = result.BrotliDecompress();

        decompressed.ToArray().Should().BeEquivalentTo(array);

    }
}