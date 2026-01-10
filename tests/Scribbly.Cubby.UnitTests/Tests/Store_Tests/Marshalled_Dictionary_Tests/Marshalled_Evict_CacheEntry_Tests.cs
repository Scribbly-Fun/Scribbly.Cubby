using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Marshalled;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests.Marshalled_Dictionary_Tests;

public sealed class Marshalled_Evict_CacheEntry_Tests : Evict_CacheEntry_TestsBase
{
    /// <inheritdoc />
    protected override ICubbyStore CreateStore(CubbyServerOptions options, TimeProvider provider)
    {
        return MarshalledStore.FromOptions(options, provider);
    }
}