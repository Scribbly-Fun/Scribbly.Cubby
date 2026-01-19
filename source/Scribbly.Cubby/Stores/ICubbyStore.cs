namespace Scribbly.Cubby.Stores;

/// <summary>
/// A common abstraction used by all cubby stores.
/// Stores hold the cached values matched against keys.
/// </summary>
public interface ICubbyStore : ICubbyStoreEviction, IDisposable
{
    /// <summary>
    /// True when the value exists.
    /// </summary>
    /// <param name="key">The key to query the store</param>
    /// <returns>True when found, false when not.</returns>
    bool Exists(in BytesKey key);
    
    /// <summary>
    /// Gets the value from the cache
    /// </summary>
    /// <param name="key">The key for the value requested</param>
    /// <returns>The value from the store</returns>
    /// <exception cref="InvalidOperationException">When the key is not found in the store</exception>
    ReadOnlySpan<byte> Get(in BytesKey key);

    /// <summary>
    /// Attempts to Get the value from the cache
    /// </summary>
    /// <param name="key">The key for the value requested</param>
    /// <param name="value">The value from the store</param>
    /// <returns>True when found, false when not</returns>
    bool TryGet(in BytesKey key, out ReadOnlySpan<byte> value);

    /// <summary>
    /// Inserts or updates a cached value.
    /// </summary>
    /// <param name="key">The key used from the cache</param>
    /// <param name="value">The cached value to update or create </param>
    /// <param name="options">Options declaring how the cache will be stored.</param>
    PutResult Put(in BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options);

    /// <summary>
    /// Refreshed a cache entry when
    /// </summary>
    /// <param name="key">The key used from the cache</param>
    RefreshResult Refresh(in BytesKey key);
}

/// <summary>
/// Evicts cache entries
/// </summary>
public interface ICubbyStoreEviction
{
    /// <summary>
    /// removes the cached value
    /// </summary>
    /// <param name="key">The key</param>
    EvictResult Evict(in BytesKey key);
    
    /// <summary>
    /// Marks the cache for revoke
    /// A tombstoned cache will be removed the next time it's queried.
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>The current flags</returns>
    CacheEntryFlags Tombstone(in BytesKey key);
}

/// <summary>
/// Result returned to the caller when a new or existing cache entry is added or updated
/// </summary>
public enum PutResult : byte
{
    /// <summary>
    /// Unknown result
    /// </summary>
    Undefined = 0,
    /// <summary>
    /// A new entry was created
    /// </summary>
    Created = 1,
    /// <summary>
    /// An existing entry was updated
    /// </summary>
    Updated = 2
}

/// <summary>
/// Result returned to the caller when cache was evicted
/// </summary>
public enum EvictResult : byte
{
    /// <summary>
    /// Entry was not found.
    /// </summary>
    Undefined = 0,
    /// <summary>
    /// Cache entry was removed
    /// </summary>
    Removed = 1,
    /// <summary>
    /// Unknown results.
    /// </summary>
    Unknown = 2
}

/// <summary>
/// Result returned to the caller when cache entry was refreshed
/// </summary>
public enum RefreshResult : byte
{
    /// <summary>
    /// Entry was not found.
    /// </summary>
    Undefined = 0,
    /// <summary>
    /// Cache entry was updated and sliding expiration was updated.
    /// </summary>
    Updated = 1,
    /// <summary>
    /// When the entry in the cache is not in fact a sliding entry and can't be refreshed.
    /// </summary>
    NotSlidingEntry = 2
}