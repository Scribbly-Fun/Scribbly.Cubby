using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Stores;

/// <summary>
/// Factory used to create stores
/// </summary>
public sealed class CubbyStoreFactory
{
    /// <summary>
    /// Creates  a new cubby store
    /// </summary>
    /// <param name="store">The type of store. <remarks>This will become part of the builder</remarks></param>
    /// <returns>A store used to store the cached data</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested store type doesn't exist.</exception>
    public ICubbyStore CreateStore(Store store)
    {
        return store switch
        {
            Store.Sharded => ShardedConcurrentStore.FromCores,
            _ => throw new ArgumentOutOfRangeException(nameof(store), store, null)
        };
    }
}