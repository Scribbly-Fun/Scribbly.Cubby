using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Pooled_Cache_Entry_Tests;

public class Entry_Expiration_Tests
{
    [Theory]
    [InlineData(0, 22)]
    [InlineData(1, 100054)]
    [InlineData(200, 99855742)]
    [InlineData(119, 44885215)]
    [InlineData(10000, 11254542)]
    [InlineData(98752, 78846381)]
    public void Given_ExpirationValue_ExpirationBytes_Should_Be_ExpirationTicks(int length, long ticks)
    {
        byte[] key = new byte[length];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);

        var now = DateTimeOffset.UtcNow.UtcTicks;
        var expected = now + ticks;
        
        var entry = PooledCacheEntry.CreateWithTtl(key, value, TimeSpan.FromTicks(ticks), now);

        entry.ExpirationUtcTicks.Should().Be(expected);
    }
}