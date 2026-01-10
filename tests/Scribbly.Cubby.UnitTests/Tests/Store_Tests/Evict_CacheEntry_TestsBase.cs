using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class Evict_CacheEntry_TestsBase : CubbyStore_CacheEntry_TestsBase
{
    [Fact]
    public void Evict_Unknown_Key_Returns_Unknown()
    {
        BytesKey key = "My Unknown Key";

        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Evict(key).Should().Be(EvictResult.Unknown);
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Evict_Known_Key_Returns_Removed(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.Evict(key).Should().Be(EvictResult.Removed);
    }

    [Theory]
    [InlineData("key", 1000)]
    [InlineData("the key for the cache", 10_000)]
    [InlineData("some key value i can't think of", 121)]
    [InlineData("💪💪💪💪💪💪", 77_987)]
    [InlineData("12341123", 65_535)]
    public void Evict_Known_Key_Removes_Key(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);

        store.Put(key, array, CacheEntryOptions.None);
        
        store.Evict(key).Should().Be(EvictResult.Removed);
        store.Exists(key).Should().BeFalse();
    }
}