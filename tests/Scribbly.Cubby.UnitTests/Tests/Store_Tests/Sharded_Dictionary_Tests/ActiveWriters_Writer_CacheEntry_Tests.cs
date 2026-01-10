using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests.Sharded_Dictionary_Tests;

public sealed class ActiveWriters_Writer_CacheEntry_Tests : ActiveWriters_CacheEntry_TestsBase
{
    /// <inheritdoc />
    protected override ICubbyStore CreateStore(CubbyServerOptions options, TimeProvider provider)
    {
        return ShardedConcurrentStore.FromOptions(options, provider);
    }
}