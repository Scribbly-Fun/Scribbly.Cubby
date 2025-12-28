using System.Text.Json;

namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// A System.Text.Json serializer for the Cubby cache client.
/// </summary>
internal sealed class SystemTextCubbySerializer(JsonSerializerOptions jsonOptions, ICubbyCompressor compressor) : ICubbySerializer
{
    /// <inheritdoc />
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
    {
        if (options.Compression != SerializerCompression.Compress)
        {
            return JsonSerializer.SerializeToUtf8Bytes<T>(value, jsonOptions);
        }
        
        var source = JsonSerializer.SerializeToUtf8Bytes<T>(value, jsonOptions);
        return compressor.Compress(source);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
    {
        if (options.Compression != SerializerCompression.Compress)
        {
            return JsonSerializer.Deserialize<T>(data, jsonOptions);
        }

        var decompressed = compressor.Decompress(data);
        return JsonSerializer.Deserialize<T>(decompressed, jsonOptions);
    }
}