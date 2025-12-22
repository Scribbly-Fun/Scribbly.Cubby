using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryStruct_Tests;

public class Entry_Header_Tests
{
    [Fact]
    public void Header_Should_Be_16_Bytes()
    {
        byte[] array = [1, 2, 3];
        
        var entry = array.LayoutEntry(new CacheEntryOptions());
        
        var str = new CacheEntryStruct(entry);

        str.Value.Length.Should().Be(3);
        str.ValueMemory.Length.Should().Be(3);
    }
}