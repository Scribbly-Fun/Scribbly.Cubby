using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryOptions_Tests;

public class Absolute_Options_Factory_Tests
{
    private class StaticTimeProvider : TimeProvider
    {
        /// <inheritdoc />
        public override DateTimeOffset GetUtcNow()
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromSeconds(0));
        }
    }
    
    [Fact]
    public void GivenSlidingFlag_Should_Throw_InvalidOperationException()
    {
        var act = () =>
        {
            CacheEntryOptions.Absolute(DateTimeOffset.UtcNow.AddDays(1), CacheEntryFlags.Sliding);
        };
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.None | CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    public void GivenFlag_Flags_Should_BeFlags(CacheEntryFlags flag)
    {
        var sliding = CacheEntryOptions
            .Absolute(DateTimeOffset.UtcNow.AddDays(1), flag);

        sliding.Flags.Should().Be(flag);
    }
    
    [Theory]
    [InlineData(CacheEntryEncoding.None)]
    [InlineData(CacheEntryEncoding.Json)]
    [InlineData(CacheEntryEncoding.MessagePack)]
    [InlineData(CacheEntryEncoding.Utf8String)]
    [InlineData(CacheEntryEncoding.Utf16String)]
    public void GivenEncoding_Encoding_Should_HaveEncoding(CacheEntryEncoding encoding)
    {
        CacheEntryOptions
            .Absolute(DateTimeOffset.UtcNow.AddDays(1), CacheEntryFlags.None, encoding)
            .Encoding.Should().Be(encoding);
    }

    [Fact]
    public void GivenPastExpiration_Should_Throw_ArgumentOutOfRangeException()
    {
        var act = () =>
        {
            CacheEntryOptions.Absolute(DateTimeOffset.UtcNow.AddSeconds(-1));
        };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenExpiration_SlidingExpiration_Should_Be_Zero()
    {
        CacheEntryOptions.Absolute(DateTimeOffset.UtcNow.AddDays(1))
            .SlidingDuration.Should()
            .Be(0);
    }

    [Theory]
    [InlineData(12544)]
    [InlineData(22)]
    [InlineData(125_344)]
    [InlineData(123_125_344)]
    [InlineData(99_987)]
    public void GivenExpiration_AbsoluteExpiration_Should_Expiration(long duration)
    {
        var absolute = DateTimeOffset.UtcNow.Add(TimeSpan.FromTicks(duration));
        
        CacheEntryOptions.Absolute(absolute).AbsoluteExpiration.Should().Be(absolute.Ticks);
    }
}