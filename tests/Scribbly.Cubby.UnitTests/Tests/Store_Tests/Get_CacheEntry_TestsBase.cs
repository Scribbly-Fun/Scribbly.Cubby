using FluentAssertions;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.UnitTests.Store_Tests;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class Get_CacheEntry_TestsBase : CubbyStore_CacheEntry_TestsBase
{
    [Fact]
    public void Get_Unknown_Key_Should_Throw_KeyNotFound()
    {
        BytesKey key = "My Unknown Key";

        var act = () =>
        {
            var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);
            try
            {
                store.Get(key);

            }
            finally
            {
                store.Dispose();
            }
        };

        act.Should().ThrowExactly<KeyNotFoundException>();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_WithActiveEntry_Returns_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.Get(key).ToArray().Length.Should().Be(array.Length + 24);
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_WithTombstoneEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.From(TimeProvider.System, CacheEntryFlags.Tombstone, null));
        
        store.Get(key).IsEmpty.Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_WithAbsoluteExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));

        time.AddMilliseconds(2);
        
        store.Get(key).IsEmpty.Should().BeTrue();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_WithSlidingExpiredEntry_Returns_False(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));

        time.AddMilliseconds(2);
        
        store.Get(key).IsEmpty.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_WithAbsoluteFutureEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));
        
        store.Get(key).IsEmpty.Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_SlidingFutureEntry_Returns_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));
        
        store.Get(key).IsEmpty.Should().BeFalse();
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_WithActiveEntry_Outputs_Cache(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.Get(key).Length.Should().Be(array.Length + 24);
    }
    
    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 37_000)]
    public void Get_Known_Key_WithAbsoluteFutureEntry_Outputs_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Absolute(time, DateTimeOffset.UtcNow));
        
        store.Get(key).Length.Should().Be(array.Length + 24);
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Get_Known_Key_SlidingFutureEntry_Outputs_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var time = new AdjustableTimeProvider(TimeSpan.FromMilliseconds(-1));
        using var store = CreateStore(new CubbyServerOptions(), time);

        store.Put(key, array, CacheEntryOptions.Sliding(time, TimeSpan.FromMilliseconds(1)));
        
        store.Get(key).Length.Should().Be(array.Length + 24);
    }
}