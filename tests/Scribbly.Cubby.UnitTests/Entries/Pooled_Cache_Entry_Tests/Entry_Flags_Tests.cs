using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.Pooled_Cache_Entry_Tests;

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
        byte[] key = new byte[length];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        var entry = PooledCacheEntry.Create(key, value, 0, flag);

        entry.Flags.Should().Be(flag);
    }

    [Theory]
    [InlineData(0, CacheEntryFlags.None & CacheEntryFlags.Sliding & CacheEntryFlags.Tombstone)]
    public void Given_Combined_FlagBytes_Should_Be_FlagValue(int length, CacheEntryFlags flags)
    {
        byte[] key = new byte[length];
        byte[] value = new byte[length];
        
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(value);
        
        var entry = PooledCacheEntry.Create(key, value, 0, flags);

        entry.Flags.Should().Be(flags);
    }
}