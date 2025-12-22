using System.Buffers;
using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Buffered_Cache_Entry_Tests;

public class Invalid_Buffer_Tests
{
    [Fact]
    public void Given_ExpirationValue_ExpirationBytes_Should_Be_ExpirationTicks()
    {
        byte[] key = new byte[12];
        byte[] value = new byte[200];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        var act = () =>
        {
            var entry = BufferedCacheEntry.CreateNeverExpiring(new Span<byte>(new byte[(16 + key.Length + value.Length) - 1]), key, value);
        };

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}