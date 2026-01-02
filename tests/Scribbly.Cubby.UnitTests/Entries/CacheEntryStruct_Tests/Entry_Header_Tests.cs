using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryStruct_Tests;

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
        
        var entry = array.LayoutEntry(CacheEntryOptions.None);
        
        var str = new CacheEntryStruct(entry);

        var span = entry.AsSpan();
        
        var lastIndexOf = span.LastIndexOf((byte)0xFF) + 1;

        var slice = span[..lastIndexOf];
        
        slice.ToArray().Length.Should().Be(str.ValueLength + 24);
        
        ArrayPool<byte>.Shared.Return(entry);
    }
}