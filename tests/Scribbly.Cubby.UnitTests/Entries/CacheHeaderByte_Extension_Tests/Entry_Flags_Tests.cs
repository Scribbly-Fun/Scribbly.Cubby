using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.CacheHeaderByte_Extension_Tests;

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

        var entry = array.LayoutEntry(CacheEntryOptions.From(TimeProvider.System, flag, TimeSpan.FromSeconds(1)));
        
        var header = entry.GetHeader();
        var flagBytes = header.GetFlags();
        
        flagBytes.Should().Be(flag);
    }

    [Theory]
    [InlineData(0, CacheEntryFlags.None | CacheEntryFlags.Tombstone)]
    public void Given_Combined_FlagBytes_Should_Be_FlagValue(int length, CacheEntryFlags flags)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        var entry = array.LayoutEntry(CacheEntryOptions.Never(flags));
        
        var header = entry.GetHeader();
        var flagBytes = header.GetFlags();

        flagBytes.Should().Be(flags);
        
        ArrayPool<byte>.Shared.Return(entry);
    }
}