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
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
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
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
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
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_WithAbsoluteExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));

        time.AddMilliseconds(2);
        
        store.TryGet(key, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_WithSlidingExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));

        time.AddMilliseconds(2);
        
        store.TryGet(key, out _).Should().BeFalse();
    }
    
    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_WithAbsoluteFutureEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));
        
        store.TryGet(key, out _).Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_With_SlidingFutureEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.Zero);
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromSeconds(10)));
        
        store.TryGet(key, out _).Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_WithActiveEntry_Outputs_Cache(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.TryGet(key, out var entry).Should().BeTrue();

        entry.Length.Should().Be(array.Length + 24);
    }
    
    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_WithAbsoluteFutureEntry_Outputs_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));
        
        store.TryGet(key, out var entry).Should().BeTrue();
        
        entry.Length.Should().Be(array.Length + 24);
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_With_SlidingFutureEntry_Outputs_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));
        
        store.TryGet(key, out var entry).Should().BeTrue();
        
        entry.Length.Should().Be(array.Length + 24);
    }

    [Theory(Skip = "Determine a good way to test sliding times")]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void TryGet_Known_Key_With_SlidingFutureEntry_SlidesExpiration(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));
        
        store.TryGet(key, out var entry).Should().BeTrue();
        
        entry.GetHeader().GetExpiration().Should().BeGreaterOrEqualTo(time.GetUtcNow().UtcTicks);
    }
}