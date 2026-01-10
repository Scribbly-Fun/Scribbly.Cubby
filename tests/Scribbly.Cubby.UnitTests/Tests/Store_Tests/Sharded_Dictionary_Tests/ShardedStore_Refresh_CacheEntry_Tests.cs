using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests.Sharded_Dictionary_Tests;

public sealed class ShardedStore_Refresh_CacheEntry_Tests : Refresh_CacheEntry_TestsBase
{
    /// <inheritdoc />
    protected override ICubbyStore CreateStore(CubbyServerOptions options, TimeProvider provider)
    {
        return ShardedConcurrentStore.FromOptions(options, provider);
    }
}