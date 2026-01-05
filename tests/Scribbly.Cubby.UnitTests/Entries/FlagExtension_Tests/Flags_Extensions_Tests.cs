using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.FlagExtension_Tests;

public class Flags_Extensions_Tests
{
    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.Compressed | CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.Compressed | CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone)]
    public void Given_Flag_ToFlagsString_Should_Return_CorrectStringValue(CacheEntryFlags flag)
    {
        var result = flag.ToString();

        var concat = string.Concat(result).Replace(" ", "");

        flag.ToFlagsString().Should().Be(concat);
    }

    [Theory]
    [InlineData("None", CacheEntryFlags.None)]
    [InlineData("Compressed", CacheEntryFlags.Compressed)]
    [InlineData("Sliding", CacheEntryFlags.Sliding)]
    [InlineData("Tombstone", CacheEntryFlags.Tombstone)]
    [InlineData("Compressed,Sliding", CacheEntryFlags.Compressed | CacheEntryFlags.Sliding)]
    [InlineData("Compressed,Tombstone", CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    [InlineData("Sliding,Tombstone", CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone)]
    [InlineData("Compressed,Sliding,Tombstone", CacheEntryFlags.Compressed | CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone)]
    public void Given_String_ToFlagsString_Should_Return_CorrectFlags(string value, CacheEntryFlags flag)
    {
        value.ToCacheEntryFlags().Should().Be(flag);
    }

    [Theory]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.Tombstone | CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Tombstone | CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone | CacheEntryFlags.Compressed | CacheEntryFlags.Sliding)]
    public void Given_TombstoneFlag_IsTombstone_Should_BeTrue(CacheEntryFlags flags)
    {
        flags.IsTombstone().Should().BeTrue();
    }

    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Compressed | CacheEntryFlags.Sliding)]
    public void Given_FlagWithoutTombstone_IsTombstone_Should_BeFalse(CacheEntryFlags flags)
    {
        flags.IsTombstone().Should().BeFalse();
    }

    [Theory]
    [InlineData(CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Tombstone | CacheEntryFlags.Sliding)]
    [InlineData(CacheEntryFlags.Sliding | CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone | CacheEntryFlags.Compressed | CacheEntryFlags.Sliding)]
    public void Given_SlidingFlag_IsSliding_Should_BeTrue(CacheEntryFlags flags)
    {
        flags.IsSliding().Should().BeTrue();
    }

    [Theory]
    [InlineData(CacheEntryFlags.None)]
    [InlineData(CacheEntryFlags.Compressed)]
    [InlineData(CacheEntryFlags.Tombstone)]
    [InlineData(CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone)]
    public void Given_FlagWithoutSliding_IsSliding_Should_BeFalse(CacheEntryFlags flags)
    {
        flags.IsSliding().Should().BeFalse();
    }
}
