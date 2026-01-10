using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Scribbly.Cubby.Client.Serializer;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

internal class CubbyClient(ICubbyStoreTransport transport, ICubbySerializer serializer, ICubbyCompressor compressor) 
    : ICubbyClient
{
    /// <inheritdoc />
    public ValueTask<PutResult> Put(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options, CancellationToken token = default)
    {
        if ((options?.Flags & CacheEntryFlags.Compressed) != 0)
        {
            var compressed = compressor.Compress(value.Span);
            return transport.PutAsync(key, compressed.ToArray(), options, token);
        }
        
        return transport.PutAsync(key, value, options, token);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Serialize<T>(T, SerializerOptions)")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Serialize<T>(T, SerializerOptions)")]
    public async ValueTask<PutResult> PutObject<T>(BytesKey key, T value, CacheEntryOptions? options, CancellationToken token = default)
        where T : notnull
    {
        var encodedValue = serializer.Serialize(value);
        
        if ((options?.Flags & CacheEntryFlags.Compressed) != 0)
        {
            var compressed = compressor.Compress(encodedValue);
            return await transport.PutAsync(key, compressed.ToArray(), options, token);
        }
        
        return await transport.PutAsync(key, encodedValue.ToArray(), options, token);
    }

    /// <inheritdoc />
    public async ValueTask<EntryResponse> Get(BytesKey key, CancellationToken token = default)
    {
        var entry = await transport.GetAsync(key, token);

        if (entry.IsEmpty)
        {
            return EntryResponse.Empty;
        }
        
        var span = entry.Span;
        
        var flags = span.GetFlags();
        var encoding = span.GetEncoding();
        var expiration = span.GetExpiration();
        var value = span.GetValue();
        
        if ((flags & CacheEntryFlags.Compressed) != 0)
        {
            var decompressed = compressor.Decompress(value);
            return EntryResponse.Create(flags, encoding, expiration, decompressed.ToArray());
        }
        
        return EntryResponse.Create(flags, encoding, expiration, value.ToArray());
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    [RequiresDynamicCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    [RequiresUnreferencedCode("Calls Scribbly.Cubby.Client.Serializer.ICubbySerializer.Deserialize<T>(ReadOnlySpan<Byte>, SerializerOptions)")]
    public async ValueTask<EntryResponse<T>> GetObject<T>(BytesKey key, CancellationToken token = default)
        where T : notnull
    {
        var entry = await transport.GetAsync(key, token);
        
        if (entry.IsEmpty)
        {
            return EntryResponse<T>.Empty;
        }
        
        var span = entry.Span;

        var flags = span.GetFlags();
        var encoding = span.GetEncoding();
        var expiration = span.GetExpiration();
        var value = span.GetValue();

        if ((flags & CacheEntryFlags.Compressed) != 0)
        {
            var decompressed = compressor.Decompress(value);
            return EntryResponse<T>.Create(flags, encoding, expiration, 
                serializer.Deserialize<T>(decompressed) 
                ?? throw new SerializationException("Failed to convert the stored bytes to the requested object"));
        }
        
        return EntryResponse<T>.Create(flags, encoding, expiration,
            serializer.Deserialize<T>(value) 
            ?? throw new SerializationException("Failed to convert the stored bytes to the requested object")
        );
    }
    
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<bool> Exists(BytesKey key, CancellationToken token = default)
    {
        return transport.ExistsAsync(key, token);
    }
    
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<RefreshResult> Refresh(BytesKey key, CancellationToken token = default)
    {
        return transport.RefreshAsync(key, token);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<EvictResult> Evict(BytesKey key, CancellationToken token = default)
    {
        return transport.EvictAsync(key, token);
    }

}