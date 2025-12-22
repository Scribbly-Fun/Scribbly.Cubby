using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Pooled_Cache_Entry_Tests;

public class Entry_Header_Tests
{
    [Fact]
    public void Header_Should_Be_16_Bytes()
    {
        var entry = PooledCacheEntry.CreateNeverExpiring([1, 2, 3], [1, 2, 3]);

        entry.Value.Length.Should().Be(3);
        entry.ValueMemory.Length.Should().Be(3);
    }
}