using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryOptions_Tests;

public class Never_Options_Factory_Tests
{
    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.None | CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    public void GivenFlag_Should_Set_Flags_ToValue_WhenNotSliding(CacheEntryFlags flag) => 
        CacheEntryOptions.Never(flag).Flags.Should().Be(flag);
    
    [Theory]
    [InlineData(CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.Sliding | CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Sliding | CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    public void Given_SlidingFlag_Should_Throw_InvalidOperation(CacheEntryFlags flags)
    {
        var act = () =>
        {
            CacheEntryOptions.Never(flags).Flags.Should().Be(flags);
        };

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Given_Nothing_Should_Set_Flags_None() => 
        CacheEntryOptions.Never().Flags.Should().Be(CacheEntryFlags.None);

    [Fact]
    public void Given_Nothing_Should_Set_Encoding_None() => 
        CacheEntryOptions.Never().Encoding.Should().Be(CacheEntryEncoding.None);
    
    [Theory]
    [InlineData(CacheEntryEncoding.None)]
    [InlineData(CacheEntryEncoding.Json)]
    [InlineData(CacheEntryEncoding.MessagePack)]
    [InlineData(CacheEntryEncoding.Utf8String)]
    [InlineData(CacheEntryEncoding.Utf16String)]
    public void GivenEncoding_Should_Set_Flags_ToValue_WhenNotSliding(CacheEntryEncoding encoding) => 
        CacheEntryOptions.Never(encoding: encoding).Encoding.Should().Be(encoding);

    [Fact]
    public void NewNever_Should_Set_SlidingDuration_Zero() => 
        CacheEntryOptions.Never().SlidingDuration.Should().Be(0);

    [Fact]
    public void NewNever_Should_Set_AbsoluteExpiration_Zero() => 
        CacheEntryOptions.Never().AbsoluteExpiration.Should().Be(0);
}