using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Marshalled;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests.Marshalled_Dictionary_Tests;

public sealed class ActiveWriters_Writer_CacheEntry_Tests : ActiveWriters_CacheEntry_TestsBase
{
    /// <inheritdoc />
    protected override ICubbyStore CreateStore(CubbyServerOptions options, TimeProvider provider)
    {
        return MarshalledStore.FromOptions(options, provider);
    }
}