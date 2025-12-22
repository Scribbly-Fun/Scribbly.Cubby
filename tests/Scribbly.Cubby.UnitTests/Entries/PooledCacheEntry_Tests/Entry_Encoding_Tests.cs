using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Entries.PooledCacheEntry_Tests;

public class Entry_Encoding_Tests
{
    [Theory]
    [InlineData(0, CacheEntryEncoding.None)]
    [InlineData(0, CacheEntryEncoding.Json)]
    [InlineData(0, CacheEntryEncoding.Utf8String)]
    [InlineData(0, CacheEntryEncoding.Utf16String)]
    [InlineData(125, CacheEntryEncoding.None)]
    [InlineData(125, CacheEntryEncoding.Json)]
    [InlineData(125, CacheEntryEncoding.Utf8String)]
    [InlineData(125, CacheEntryEncoding.Utf16String)]
    [InlineData(10554, CacheEntryEncoding.None)]
    [InlineData(10554, CacheEntryEncoding.Json)]
    [InlineData(10554, CacheEntryEncoding.Utf8String)]
    [InlineData(10554, CacheEntryEncoding.Utf16String)]
    public void Given_Flag_FlagBytes_Should_Be_FlagValue(int length, CacheEntryEncoding encoding)
    {
        byte[] array = new byte[length];
        
        Random.Shared.NextBytes(array);
        
        using var entry = PooledCacheEntry.Create(array, 0, CacheEntryFlags.None, encoding);

        entry.Encoding.Should().Be(encoding);
    }
}