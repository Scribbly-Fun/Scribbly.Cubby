using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Public interface used to communicate with cubby
/// </summary>
public interface ICubbyClient
{
    /// <summary>
    /// checks if an item exists in the cache
    /// </summary>
    ValueTask<bool> Exists(BytesKey key, CancellationToken token = default);
    
    /// <summary>
    /// Inserts or creates a new cached record.
    /// </summary>
    ValueTask<PutResult> Put(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options = null, CancellationToken token = default);
    
    /// <summary>
    /// Inserts or creates a serialized object
    /// </summary>
    /// <remarks>
    ///     Data will be serialized using the select serializer.
    /// </remarks>
    ValueTask<PutResult> PutObject<T>(BytesKey key, T value, CacheEntryOptions? options = null, CancellationToken token = default) where T : notnull;
    
    /// <summary>
    /// Gets data from the cache for a specific key
    /// </summary>
    ValueTask<EntryResponse> Get(BytesKey key, CancellationToken token = default);
    
    /// <summary>
    /// Gets decoded data from the cache.
    /// </summary>
   ValueTask<EntryResponse<T>> GetObject<T>(BytesKey key, CancellationToken token = default) where T : notnull;
}