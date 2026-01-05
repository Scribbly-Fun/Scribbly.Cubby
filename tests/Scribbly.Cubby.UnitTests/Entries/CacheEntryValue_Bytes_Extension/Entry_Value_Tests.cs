using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryValue_Bytes_Extension;

public class Entry_Value_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(200)]
    [InlineData(119)]
    [InlineData(10000)]
    [InlineData(98752)]
    public void Given_ByteArrayValue_ValueMemory_Should_Be_Array(int length)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        var entry = array.LayoutEntry(CacheEntryOptions.None);

        var valueBytes = entry.GetCachedValue();

        valueBytes.ToArray().SequenceEqual(array).Should().BeTrue();
        
        ArrayPool<byte>.Shared.Return(entry);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(200)]
    [InlineData(119)]
    [InlineData(10000)]
    [InlineData(98752)]
    public void Given_ByteArrayValue_Value_Should_Be_Array(int length)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        var entry = array.LayoutEntry(CacheEntryOptions.None);
        
        var valueBytes = entry.GetCachedValue();

        valueBytes.ToArray().SequenceEqual(array).Should().BeTrue();
        
        ArrayPool<byte>.Shared.Return(entry);
    }
}