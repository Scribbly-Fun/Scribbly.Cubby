using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.ExpirationHeader_Extension_Tests;

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
}
