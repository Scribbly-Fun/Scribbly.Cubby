using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Pooled_Cache_Entry_Tests;

public class Entry_Key_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(200)]
    [InlineData(119)]
    [InlineData(10000)]
    [InlineData(35535)]
    public void Given_ByteArrayValue_Key_Should_Be_Array(int length)
    {
        byte[] key = new byte[length];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        var entry = PooledCacheEntry.CreateNeverExpiring(key, value);

        entry.Key.ToArray().SequenceEqual(key).Should().BeTrue();
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(200)]
    [InlineData(119)]
    [InlineData(10000)]
    [InlineData(32767)]
    public void Given_ByteArrayValue_KeyLength_Should_Be_Array(int length)
    {
        byte[] key = new byte[length];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        Random.Shared.NextBytes(value);
        
        var entry = PooledCacheEntry.CreateNeverExpiring(key, value);

        entry.KeyLength.Should().Be(length);
    }
}