using Microsoft.Extensions.Options;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Stores;

/// <summary>
/// Factory used to create stores
/// </summary>
/// <param name="options">
///     The type of store.
///     <remarks>
///         This will become part of the builder
///     </remarks>
/// </param>
/// <param name="provider">
///     A time provider used to generate and computer time related events
/// </param>
public sealed class CubbyStoreFactory(IOptions<CubbyServerOptions> options, TimeProvider provider)
{
    /// <summary>
    ///     Creates  a new cubby store
    /// </summary>

    /// <returns>
    ///     A store used to store the cached data
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the requested store type doesn't exist.
    /// </exception>
    public ICubbyStore CreateStore()
    {
        var cubbyOptions = options.Value;
        return cubbyOptions switch
        {
            { Store: CubbyServerOptions.StoreType.Sharded } => SharedConcurrentStore.FromOptions(cubbyOptions, provider),
            { Store: CubbyServerOptions.StoreType.Concurrent } => ConcurrentStore.FromOptions(cubbyOptions, provider),
            
            _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
        };
    }
}