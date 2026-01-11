using System.Diagnostics.CodeAnalysis;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Public interface used to communicate with cubby
/// </summary>
public interface ICubbyClient : IGetOrCreateCubbyClient
{
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
    
    /// <summary>
    /// checks if an item exists in the cache
    /// </summary>
    ValueTask<bool> Exists(BytesKey key, CancellationToken token = default);

    /// <summary>
    /// checks if an item exists in the cache
    /// </summary>
    ValueTask<RefreshResult> Refresh(BytesKey key, CancellationToken token = default);

    /// <summary>
    /// checks if an item exists in the cache
    /// </summary>
    ValueTask<EvictResult> Evict(BytesKey key, CancellationToken token = default);
}

/// <summary>
/// Methods used to get or create a new cache entry
/// </summary>
public interface IGetOrCreateCubbyClient
{
    /// <summary>
    ///     Checks the cache for an existing entry.
    ///     When the entry is not found a new entry will be created.
    /// </summary>
    /// <remarks>
    ///     A new entry will only be created when the factory returns a non-null object.
    ///     Should the factory throw an exception all errors will bubble up to the caller and an extry will not be created
    /// </remarks>
    /// <param name="key">The cache key to use</param>
    /// <param name="input">
    ///     An object passed to the factory method. Can help reduce closures and should be used when the factory requires input data.
    /// </param>
    /// <param name="factory">
    ///     A factory delegate responsible for creating the object to be stored in the cache.
    /// </param>
    /// <param name="token">
    ///     Cancellation token passed back to your factory as well as the cubby transport.
    /// </param>
    /// <typeparam name="TInput">
    ///     The type of data to pass to the factory
    /// </typeparam>
    /// <typeparam name="TReturn">
    ///     The data returned from the cache.
    /// </typeparam>
    /// <returns>The data from the factory after being stored or returned from the cache</returns>
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    ValueTask<EntryResponse<TReturn>> GetOrCreateAsync<TInput, TReturn>(
        BytesKey key, 
        TInput input, 
        Func<TInput, CancellationToken, Task<FactoryResponse<TReturn>>> factory, 
        CancellationToken token = default) 
        where TReturn : notnull;
    
    /// <summary>
    ///     Checks the cache for an existing entry.
    ///     When the entry is not found a new entry will be created.
    /// </summary>
    /// <remarks>
    ///     A new entry will only be created when the factory returns a non-null object.
    ///     Should the factory throw an exception all errors will bubble up to the caller and an extry will not be created
    /// </remarks>
    /// <param name="key">The cache key to use</param>
    /// <param name="factory">
    ///     A factory delegate responsible for creating the object to be stored in the cache.
    /// </param>
    /// <param name="token">
    ///     Cancellation token passed back to your factory as well as the cubby transport.
    /// </param>
    /// <typeparam name="TReturn">
    ///     The data returned from the cache.
    /// </typeparam>
    /// <returns>The data from the factory after being stored or returned from the cache</returns>
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    ValueTask<EntryResponse<TReturn>> GetOrCreateAsync<TReturn>(
        BytesKey key, 
        Func<CancellationToken, Task<FactoryResponse<TReturn>>> factory, 
        CancellationToken token = default) 
        where TReturn : notnull;
    
    /// <summary>
    ///     Checks the cache for an existing entry.
    ///     When the entry is not found a new entry will be created.
    /// </summary>
    /// <remarks>
    ///     A new entry will only be created when the factory returns a non-null object.
    ///     Should the factory throw an exception all errors will bubble up to the caller and an extry will not be created
    /// </remarks>
    /// <param name="key">The cache key to use</param>
    /// <param name="input">
    ///     An object passed to the factory method. Can help reduce closures and should be used when the factory requires input data.
    /// </param>
    /// <param name="factory">
    ///     A factory delegate responsible for creating the object to be stored in the cache.
    /// </param>
    /// <param name="token">
    ///     Cancellation token passed back to your factory as well as the cubby transport.
    /// </param>
    /// <typeparam name="TInput">
    ///     The type of data to pass to the factory
    /// </typeparam>
    /// <typeparam name="TReturn">
    ///     The data returned from the cache.
    /// </typeparam>
    /// <returns>The data from the factory after being stored or returned from the cache</returns>
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    ValueTask<EntryResponse<TReturn>> GetOrCreate<TInput, TReturn>(
        BytesKey key, 
        TInput input, 
        Func<TInput, FactoryResponse<TReturn>> factory, 
        CancellationToken token = default) 
        where TReturn : notnull;
    
    /// <summary>
    ///     Checks the cache for an existing entry.
    ///     When the entry is not found a new entry will be created.
    /// </summary>
    /// <remarks>
    ///     A new entry will only be created when the factory returns a non-null object.
    ///     Should the factory throw an exception all errors will bubble up to the caller and an extry will not be created
    /// </remarks>
    /// <param name="key">The cache key to use</param>
    /// <param name="factory">
    ///     A factory delegate responsible for creating the object to be stored in the cache.
    /// </param>
    /// <param name="token">
    ///     Cancellation token passed back to your factory as well as the cubby transport.
    /// </param>
    /// <typeparam name="TReturn">
    ///     The data returned from the cache.
    /// </typeparam>
    /// <returns>The data from the factory after being stored or returned from the cache</returns>
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    ValueTask<EntryResponse<TReturn>> GetOrCreate<TReturn>(
        BytesKey key, 
        Func<FactoryResponse<TReturn>> factory, 
        CancellationToken token = default) 
        where TReturn : notnull;
}
    

