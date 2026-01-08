using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Structure.CacheHeaderSpan_Extension_Tests;

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

        var now = DateTimeOffset.UtcNow;
        var expected = now.Ticks + ticks;

        var entry = array.RentCacheEntryArray(CacheEntryOptions.Sliding(now, TimeSpan.FromTicks(ticks)));
        var span = entry.AsSpan();
        var header = span.GetHeader();
        var expirationByte = header.GetExpiration();
        
        expirationByte.Should().Be(expected);
        
        ArrayPool<byte>.Shared.Return(entry);
    }
}