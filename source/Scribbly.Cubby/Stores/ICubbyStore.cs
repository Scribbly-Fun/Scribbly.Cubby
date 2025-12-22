using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Stores;

/// <summary>
/// A common abstraction used by all cubby stores.
/// Stores hold the cached values matched against keys.
/// </summary>
public interface ICubbyStore : IDisposable
{
    /// <summary>
    /// True when the value exists.
    /// </summary>
    /// <param name="key">The key to query the store</param>
    /// <returns>True when found, false when not.</returns>
    bool Exists(BytesKey key);
    
    /// <summary>
    /// Gets the value from the cache
    /// </summary>
    /// <param name="key">The key for the value requested</param>
    /// <returns>The value from the store</returns>
    /// <exception cref="InvalidOperationException">When the key is not found in the store</exception>
    ReadOnlyMemory<byte> Get(BytesKey key);

    /// <summary>
    /// Attempts to Get the value from the cache
    /// </summary>
    /// <param name="key">The key for the value requested</param>
    /// <param name="value">The value from the store</param>
    /// <returns>True when found, false when not</returns>
    bool TryGet(BytesKey key, out ReadOnlySpan<byte> value);

    /// <summary>
    /// Inserts or updates a cached value.
    /// </summary>
    /// <param name="key">The key used from the cache</param>
    /// <param name="value">The cached value to update or create </param>
    /// <param name="options">Options declaring how the cache will be stored.</param>
    void Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options);

    /// <summary>
    /// removes the cached value
    /// </summary>
    /// <param name="key">The key</param>
    void Evict(BytesKey key);

    /// <summary>
    /// removes the cached value
    /// </summary>
    /// <param name="key">The key</param>
    bool TryEvict(BytesKey key);
}