using System.Diagnostics.CodeAnalysis;
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
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Serialize<T>(T, SerializerOptions)")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Serialize<T>(T, SerializerOptions)")]
    ValueTask<PutResult> PutObject<T>(BytesKey key, T value, CacheEntryOptions? options = null, CancellationToken token = default) where T : notnull;
    
    /// <summary>
    /// Gets data from the cache for a specific key
    /// </summary>
    ValueTask<EntryResponse> Get(BytesKey key, CancellationToken token = default);
    
    /// <summary>
    /// Gets decoded data from the cache.
    /// </summary>
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    ValueTask<EntryResponse<T>> GetObject<T>(BytesKey key, CancellationToken token = default) where T : notnull;
}