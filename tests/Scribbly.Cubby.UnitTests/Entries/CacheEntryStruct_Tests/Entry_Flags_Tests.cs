using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheEntryStruct_Tests;

public class Entry_Flags_Tests
{
    [Theory]
    [InlineData(0, CacheEntryFlags.None)]
    [InlineData(0, CacheEntryFlags.Compressed)]
    [InlineData(0, CacheEntryFlags.Sliding)]
    [InlineData(0, CacheEntryFlags.Tombstone)]
    [InlineData(123, CacheEntryFlags.None)]
    [InlineData(123, CacheEntryFlags.Compressed)]
    [InlineData(123, CacheEntryFlags.Sliding)]
    [InlineData(123, CacheEntryFlags.Tombstone)]
    [InlineData(1000520, CacheEntryFlags.None)]
    [InlineData(1000520, CacheEntryFlags.Compressed)]
    [InlineData(1000520, CacheEntryFlags.Sliding)]
    [InlineData(1000520, CacheEntryFlags.Tombstone)]
    public void Given_Flag_FlagBytes_Should_Be_FlagValue(int length, CacheEntryFlags flag)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        var entry = array.LayoutEntry(new CacheEntryOptions
        {
            Flags = flag
        });
        
        var str = new CacheEntryStruct(entry);

        str.Flags.Should().Be(flag);
    }

    [Theory]
    [InlineData(0, CacheEntryFlags.None & CacheEntryFlags.Sliding & CacheEntryFlags.Tombstone)]
    public void Given_Combined_FlagBytes_Should_Be_FlagValue(int length, CacheEntryFlags flags)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        var entry = array.LayoutEntry(new CacheEntryOptions
        {
            Flags = flags
        });
        
        var str = new CacheEntryStruct(entry);

        str.Flags.Should().Be(flags);
    }
}