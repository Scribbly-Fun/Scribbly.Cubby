using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Stores;

/// <summary>
/// Factory used to create stores
/// </summary>
public sealed class CubbyStoreFactory
{
    /// <summary>
    ///     Creates  a new cubby store
    /// </summary>
    /// <param name="options">
    ///     The type of store.
    ///     <remarks>
    ///         This will become part of the builder
    ///     </remarks>
    /// </param>
    /// <returns>
    ///     A store used to store the cached data
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the requested store type doesn't exist.
    /// </exception>
    public ICubbyStore CreateStore(CubbyOptions options)
    {
        return options switch
        {
            { Store: CubbyOptions.StoreType.Sharded } => SharedConcurrentStore.FromOptions(options),
            { Store: CubbyOptions.StoreType.Concurrent } => ConcurrentStore.FromOptions(options),
            
            _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
        };
    }
}