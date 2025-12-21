using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Cache_Entry_Tests;

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
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);

        var now = DateTimeOffset.UtcNow.UtcTicks;
        var expected = now + ticks;
        
        var entry = CacheEntry.CreateWithTtl(array, TimeSpan.FromTicks(ticks), now);

        entry.ExpirationUtcTicks.Should().Be(expected);
    }
}