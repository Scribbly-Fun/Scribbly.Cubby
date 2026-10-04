using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Tests.Structure.ExpirationHeader_Extension_Tests;

public class Expiration_Header_Time_Extensions_Tests
{
    [Fact]
    public void Given_Zero_IsExpired_Should_Return_False()
    {
        long expiration = 0;
        
        expiration.IsExpired(TimeProvider.System.GetUtcNow().UtcTicks).Should().BeFalse();
    }

    [Fact]
    public void Given_Zero_IsExpiredNullable_Should_Return_False()
    {
        long? expiration = 0;
        
        expiration.IsExpired(TimeProvider.System.GetUtcNow().UtcTicks).Should().BeFalse();
    }

    [Fact]
    public void Given_Null_IsExpiredNullable_Should_Return_False()
    {
        long? expiration = null;
        
        expiration.IsExpired(TimeProvider.System.GetUtcNow().UtcTicks).Should().BeFalse();
    }
    
    [Fact]
    public void Given_PastExpiration_IsExpired_Should_Return_True()
    {
        long expiration = 200;
        
        expiration.IsExpired(201).Should().BeTrue();
    }
    
    [Fact]
    public void Given_PastExpiration_IsExpiredNullable_Should_Return_True()
    {
        long? expiration = 200;
        
        expiration.IsExpired(201).Should().BeTrue();
    }

    [Fact]
    public void Given_Zero_ToUtcDateTimeOffset_Should_Return_Null()
    {
        long expiration = 0;

        expiration.ToUtcDateTimeOffset().Should().BeNull();
    }

    [Fact]
    public void Given_UtcTicks_ToUtcDateTimeOffset_Should_Keep_Zero_Offset()
    {
        var utc = new DateTimeOffset(2026, 10, 4, 13, 52, 43, TimeSpan.Zero);
        var mapped = utc.UtcTicks.ToUtcDateTimeOffset();

        mapped.Should().NotBeNull();
        mapped!.Value.Offset.Should().Be(TimeSpan.Zero);
        mapped.Value.UtcTicks.Should().Be(utc.UtcTicks);
        mapped.Value.ToString("o").Should().EndWith("+00:00");
    }

}
