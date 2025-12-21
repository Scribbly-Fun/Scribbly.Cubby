using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Cache_Entry_Tests;

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
        
        var entry = CacheEntry.CreateNeverExpiring(array);

        entry.ValueMemory.ToArray().SequenceEqual(array).Should().BeTrue();
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
        
        var entry = CacheEntry.CreateNeverExpiring(array);

        entry.Value.ToArray().SequenceEqual(array).Should().BeTrue();
    }
}