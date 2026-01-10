using FluentAssertions;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.UnitTests.Setup;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class Refresh_CacheEntry_TestsBase : CubbyStore_CacheEntry_TestsBase
{
    [Fact]
    public void Refresh_Unknown_Key_Returns_Undefined()
    {
        BytesKey key = "My Unknown Key";

        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Refresh(key).Should().Be(RefreshResult.Undefined);
    }

    [Fact]
    public void Refresh_Known_Key_When_Entry_NotSliding_Returns_NotSlidingEntry()
    {
        byte[] array = new byte[50];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put("MY_VALID_KEY", array, CacheEntryOptions.None);
        
        store.Refresh("MY_VALID_KEY").Should().Be(RefreshResult.NotSlidingEntry);
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Refresh_Known_Key_When_Entry_Sliding_Returns_Updated(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, TimeSpan.FromMilliseconds(500));
        
        store.Refresh(key).Should().Be(RefreshResult.Updated);
    }

    [Theory]
    [InlineData("key", 1000, 1000000)]
    [InlineData("the key for the cache", 10_000, 99_773)]
    [InlineData("some key value i can't think of", 121, 65_366)]
    [InlineData("💪💪💪💪💪💪", 77_987, 255)]
    [InlineData("12341123", 65_535, 99_354_887)]
    public void Refresh_Known_Key_When_Entry_Sliding_Should_SlideExpiration(string key, int length, long duration)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        var timeProvider = new StaticTimeProvider();
        using var store = CreateStore(new CubbyServerOptions(), timeProvider);

        var timespan = TimeSpan.FromTicks(duration);
        var now = timeProvider.GetUtcNow();
        
        store.Put(key, array, CacheEntryOptions.Sliding(timeProvider, timespan));
        
        store.Refresh(key).Should().Be(RefreshResult.Updated);

        var entry = store.Get(key);

        entry.GetHeader().GetExpiration().Should().Be(now.Ticks + timespan.Ticks);
    }

    [Theory]
    [InlineData("key", 1000, 1000000)]
    [InlineData("the key for the cache", 10_000, 99_773)]
    [InlineData("some key value i can't think of", 121, 65_366)]
    [InlineData("💪💪💪💪💪💪", 77_987, 255)]
    [InlineData("12341123", 65_535, 99_354_887)]
    public void Refresh_Known_Key_When_Entry_Sliding_Should_NotChange_Slide(string key, int length, long duration)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        var timeProvider = new StaticTimeProvider();
        using var store = CreateStore(new CubbyServerOptions(), timeProvider);

        var timespan = TimeSpan.FromTicks(duration);
        var now = timeProvider.GetUtcNow();
        
        store.Put(key, array, CacheEntryOptions.Sliding(timeProvider, timespan));
        
        store.Refresh(key).Should().Be(RefreshResult.Updated);

        var entry = store.Get(key);

        entry.GetHeader().GetSlidingDuration().Should().Be(duration);
    }
    
}