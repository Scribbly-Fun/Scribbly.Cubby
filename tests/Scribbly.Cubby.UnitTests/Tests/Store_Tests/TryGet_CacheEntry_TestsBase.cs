using FluentAssertions;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.UnitTests.Store_Tests;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class TryGet_CacheEntry_TestsBase : CubbyStore_CacheEntry_TestsBase
{
    [Fact]
    public void TryGet_Unknown_Key_Returns_False()
    {
        BytesKey key = "My Unknown Key";

        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.TryGet(key, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("more random crap", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 123_000_212)]
    public void TryGet_Known_Key_WithActiveEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.TryGet(key, out _).Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("more random crap", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 123_000_212)]
    public void TryGet_Known_Key_WithTombstoneEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.From(TimeProvider.System, CacheEntryFlags.Tombstone, null));
        
        store.TryGet(key, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("more random crap", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 123_000_212)]
    public void TryGet_Known_Key_WithExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));

        time.AddMilliseconds(2);
        
        store.TryGet(key, out _).Should().BeFalse();
    }
}