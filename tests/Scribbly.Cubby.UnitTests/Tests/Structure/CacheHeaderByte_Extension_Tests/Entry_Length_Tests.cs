using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Structure.CacheHeaderByte_Extension_Tests;

public class Entry_Length_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(200)]
    [InlineData(119)]
    [InlineData(10000)]
    [InlineData(98752)]
    public void Given_ByteArrayValue_LengthBytes_Should_Be_ArrayLength(int length)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        var entry = array.RentCacheEntryArray(CacheEntryOptions.None);
        
        var header = entry.GetHeader();
        var valueLength = header.GetValueLength();

        valueLength.Should().Be(length);
        
        ArrayPool<byte>.Shared.Return(entry);
    }
}