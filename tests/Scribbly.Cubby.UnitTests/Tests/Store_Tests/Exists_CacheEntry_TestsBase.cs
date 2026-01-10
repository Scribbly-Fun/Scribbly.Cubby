using FluentAssertions;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.UnitTests.Store_Tests;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class Exists_CacheEntry_TestsBase : CubbyStore_CacheEntry_TestsBase
{
    [Fact]
    public void Exists_Unknown_Key_Should_Return_False()
    {
        BytesKey key = "My Unknown Key";

        var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);
        store.Exists(key).Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Exists_Known_Key_WithActiveEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.Exists(key).Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Exists_Known_Key_WithTombstoneEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.From(TimeProvider.System, CacheEntryFlags.Tombstone, null));
        
        store.Exists(key).Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Exists_Known_Key_WithAbsoluteExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));

        time.AddMilliseconds(2);
        
        store.Exists(key).Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Exists_Known_Key_WithSlidingExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));

        time.AddMilliseconds(2);
        
        store.Exists(key).Should().BeFalse();
    }
    
    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Exists_Known_Key_WithAbsoluteFutureEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromSeconds(-10));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));
        
        store.Exists(key).Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Exists_Known_Key_SlidingFutureEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.Zero);
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromSeconds(10)));
        
        store.Exists(key).Should().BeTrue();
    }
}