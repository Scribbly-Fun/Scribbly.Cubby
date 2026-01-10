using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Store_Tests;

public abstract class CubbyStore_CacheEntry_TestsBase
{
    protected abstract ICubbyStore CreateStore(CubbyServerOptions options, TimeProvider provider);
}