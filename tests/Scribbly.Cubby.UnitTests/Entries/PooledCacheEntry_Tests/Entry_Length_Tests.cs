using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.PooledCacheEntry_Tests;

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
        
        using var entry = PooledCacheEntry.CreateNeverExpiring(array);

        entry.ValueLength.Should().Be(length);
    }
}