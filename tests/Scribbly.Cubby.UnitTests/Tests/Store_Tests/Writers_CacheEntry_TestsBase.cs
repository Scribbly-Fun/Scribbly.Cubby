using FluentAssertions;
using Scribbly.Cubby.Expiration;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.UnitTests.Setup;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class ActiveWriters_CacheEntry_TestsBase : CubbyStore_CacheEntry_TestsBase
{
    [Fact(Skip = "Need to sort of threading to test the active writers")]
    public async Task ActiveWriters_Increments_During_Concurrent_Access()
    {
        var store = CreateStore(new CubbyServerOptions(), TimeProvider.System);
        store.Put("key", new byte[10], CacheEntryOptions.None);

        var slim = new SemaphoreSlim(1, 1);

        try
        {

            var tasks = Enumerable.Range(0, 10).Select(_ =>
            {
                var locker = slim;
                var s2 = store;
                return Task.Run(() =>
                {
                    locker.Wait();
                    s2.Exists("key");

                    locker.Release();
                });
            }).ToArray();
            
            await Task.WhenAll(tasks);

            await slim.WaitAsync();
            ((ICubbyStoreEvictionInteraction)store).ActiveWriters.Should().BeGreaterThan(0);

        }
        finally
        {
            store.Dispose();
            slim.Dispose();
        }
    }
}