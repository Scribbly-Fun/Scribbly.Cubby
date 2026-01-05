using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheHeaderByte_Extension_Tests;

public class Entry_Header_Tests
{
    [Theory]
    [InlineData(20)]
    [InlineData(123_661)]
    [InlineData(99_987)]
    [InlineData(1)]
    public void Header_Should_Be_24_Bytes(int length)
    {
        byte[] array = new byte[length + 1];
        
        Random.Shared.NextBytes(array);

        array[length] = 0xFF;
        
        var entry = array.RentCacheEntryArray(CacheEntryOptions.None);
        
        var header = entry.GetHeader();
        
        header.ToArray().Length.Should().Be(24);
        
        ArrayPool<byte>.Shared.Return(entry);
    }
}