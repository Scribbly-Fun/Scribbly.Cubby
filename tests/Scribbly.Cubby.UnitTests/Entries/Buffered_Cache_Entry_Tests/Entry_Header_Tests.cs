using System.Buffers;
using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Buffered_Cache_Entry_Tests;

public class Entry_Header_Tests
{
    [Fact]
    public void Header_Should_Be_16_Bytes()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(16 + 3 + 3);
        var entry = BufferedCacheEntry.CreateNeverExpiring(buffer, [1, 2, 3], [1, 2, 3]);

        entry.Value.Length.Should().Be(3);
        entry.ValueMemory.Length.Should().Be(3);
    }
}