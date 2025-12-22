using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Pooled_Cache_Entry_Tests;

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
        byte[] key = new byte[12];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        var entry = PooledCacheEntry.CreateNeverExpiring(key, value);

        entry.ValueLength.Should().Be(length);
    }
}