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
    ValueTask<PutResult> Put(BytesKey key, ReadOnlySpan<byte> value, CacheEntryOptions options, CancellationToken token = default);
    
    /// <summary>
    /// Inserts or creates a serialized object
    /// </summary>
    /// <remarks>
    ///     Data will be serialized using the select serializer.
    /// </remarks>
    ValueTask<PutResult> Put<T>(BytesKey key, T value, CacheEntryOptions options, CancellationToken token = default) where T;
    
    /// <summary>
    /// Gets data from the cache for a specific key
    /// </summary>
    ValueTask<byte[]> Get(BytesKey key, CancellationToken token = default);
    
    /// <summary>
    /// Gets decoded data from the cache.
    /// </summary>
    // TODO: In order to handle encoded or compressed data the cubby store must return the flags for a given record so we know how to decompress and decode
    ValueTask<T> Get<T>(BytesKey key, CancellationToken token = default);
}