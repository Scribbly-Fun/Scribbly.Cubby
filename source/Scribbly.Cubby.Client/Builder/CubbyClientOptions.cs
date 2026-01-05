using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Options used to setup the client.
/// </summary>
public class CubbyClientOptions
{
    internal ICubbySerializer Serializer { get; private set; } 
        = new SystemTextCubbySerializer(JsonSerializerOptions.Default);

    internal ICubbyCompressor Compressor { get; private set; } 
        = new BrotliCubbyCompressor();
    
    /// <summary>
    /// The URL for the Cubby host.
    /// </summary>
    public Uri Host { get; set; } = new Uri("https://localhost:5001");

    /// <summary>
    /// The caching service's scope
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

    /// <summary>
    /// Configures the Cubby serializer implementation.
    /// </summary>
    /// <typeparam name="TSerializer"></typeparam>
    internal void AddSerializer<TSerializer>(
        TSerializer serializer, 
        ICubbyCompressor? compressor = null) where TSerializer : ICubbySerializer
    {
        Compressor = compressor ?? new BrotliCubbyCompressor();
        Serializer = serializer;
    }

    /// <summary>
    /// Configures the Cubby JSON serializer implementation.
    /// </summary>
    internal void AddSerializer<TSerializer>(
        Action<JsonSerializerOptions>? optionsCallback = null,
        ICubbyCompressor? compressor = null) where TSerializer : ICubbySerializer
    {
        Compressor = compressor ?? new BrotliCubbyCompressor();
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            DictionaryKeyPolicy = null,
            WriteIndented = false,
            
            ReadCommentHandling = JsonCommentHandling.Disallow,
            AllowTrailingCommas = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            
            TypeInfoResolver = JsonTypeInfoResolver.Combine()
        };
        
        optionsCallback?.Invoke(options);

        Serializer = new SystemTextCubbySerializer(options);
    }
}