using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryOptions_Tests;

public class Sliding_Options_Factory_Tests
{
    private class StaticTimeProvider : TimeProvider
    {
        /// <inheritdoc />
        public override DateTimeOffset GetUtcNow()
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromSeconds(0));
        }
    }
    
    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.None | CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    public void GivenFlag_Flags_Should_HaveSliding(CacheEntryFlags flag)
    {
        CacheEntryOptions
            .Sliding(TimeProvider.System, TimeSpan.FromSeconds(2), flag)
            .Flags.HasFlag(CacheEntryFlags.Sliding)
            .Should().BeTrue();
    }
    
    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.None | CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    public void GivenFlag_Flags_Should_BeSlidingAndFlag(CacheEntryFlags flag)
    {
        var sliding = CacheEntryOptions
            .Sliding(TimeProvider.System, TimeSpan.FromSeconds(2), flag);

        sliding.Flags.Should().Be(CacheEntryFlags.Sliding | flag);
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
            .Sliding(TimeProvider.System, TimeSpan.FromSeconds(2), CacheEntryFlags.None, encoding)
            .Encoding.Should().Be(encoding);
    }

    [Fact]
    public void GivenDuration_LessThan_Zero_Should_Throw_ArgumentOutOfRangeException()
    {
        var act = () =>
        {
            CacheEntryOptions.Sliding(TimeProvider.System, TimeSpan.FromSeconds(-1));
        };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(12544)]
    [InlineData(22)]
    [InlineData(125_344)]
    [InlineData(123_125_344)]
    [InlineData(99_987)]
    public void GivenDuration_SlidingExpiration_Should_Be_Duration(long duration)
    {
        var timespan = TimeSpan.FromTicks(duration);
        var timeProvider = new StaticTimeProvider();
        CacheEntryOptions.Sliding(timeProvider, timespan).SlidingDuration.Should().Be(duration);
    }

    [Theory]
    [InlineData(12544)]
    [InlineData(22)]
    [InlineData(125_344)]
    [InlineData(123_125_344)]
    [InlineData(99_987)]
    public void GivenDuration_AbsoluteExpiration_Should_Be_NowPlusDuration(long duration)
    {
        var timespan = TimeSpan.FromTicks(duration);
        var timeProvider = new StaticTimeProvider();
        var now = timeProvider.GetUtcNow();
        CacheEntryOptions.Sliding(timeProvider, timespan).AbsoluteExpiration.Should().Be(now.Ticks + timespan.Ticks);
    }
}