using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests.Concurrent_Dictionary_Tests;

public sealed class Concurrent_Exists_CacheEntry_Tests : Exists_CacheEntry_TestsBase
{
    /// <inheritdoc />
    protected override ICubbyStore CreateStore(CubbyServerOptions options, TimeProvider provider)
    {
        return ConcurrentStore.FromOptions(options, provider);
    }
}