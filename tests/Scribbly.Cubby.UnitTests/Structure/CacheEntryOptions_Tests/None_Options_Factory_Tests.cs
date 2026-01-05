using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryOptions_Tests;

public class None_Options_Factory_Tests
{
    [Fact]
    public void NewNone_Should_Set_Flags_None() => 
        CacheEntryOptions.None.Flags.Should().Be(CacheEntryFlags.None);

    [Fact]
    public void NewNone_Should_Set_Encoding_None() => 
        CacheEntryOptions.None.Encoding.Should().Be(CacheEntryEncoding.None);

    [Fact]
    public void NewNone_Should_Set_SlidingDuration_Zero() => 
        CacheEntryOptions.None.SlidingDuration.Should().Be(0);

    [Fact]
    public void NewNone_Should_Set_AbsoluteExpiration_Zero() => 
        CacheEntryOptions.None.AbsoluteExpiration.Should().Be(0);
}