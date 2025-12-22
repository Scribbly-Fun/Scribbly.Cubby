using System.Buffers;
using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Buffered_Cache_Entry_Tests;

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
        byte[] key = new byte[22];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        var buffer = ArrayPool<byte>.Shared.Rent(16 + key.Length + value.Length);
        var entry = BufferedCacheEntry.CreateNeverExpiring(buffer, key, value);

        entry.ValueMemory.ToArray().SequenceEqual(value).Should().BeTrue();
        
        ArrayPool<byte>.Shared.Return(buffer);
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
        byte[] key = new byte[22];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        Random.Shared.NextBytes(value);
        
        var buffer = ArrayPool<byte>.Shared.Rent(16 + key.Length + value.Length);
        var entry = BufferedCacheEntry.CreateNeverExpiring(buffer, key, value);

        entry.Value.ToArray().SequenceEqual(value).Should().BeTrue();
        
        ArrayPool<byte>.Shared.Return(buffer);
    }
}