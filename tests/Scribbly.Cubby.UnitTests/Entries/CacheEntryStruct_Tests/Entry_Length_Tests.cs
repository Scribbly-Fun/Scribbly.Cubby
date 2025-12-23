using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryStruct_Tests;

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
        
        var entry = array.LayoutEntry(new CacheEntryOptions());
        
        var str = new CacheEntryStruct(entry);

        str.ValueLength.Should().Be(length);
    }
}