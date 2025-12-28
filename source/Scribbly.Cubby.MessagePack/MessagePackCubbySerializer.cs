using MessagePack;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.MessagePack;

/// <summary>
/// Serializes data using the Message Pack Protocol
/// </summary>
public class MessagePackCubbySerializer(MessagePackSerializerOptions messagePackOptions) : ICubbySerializer
{
    /// <inheritdoc />
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
    {
        if (options.Compression != SerializerCompression.Compress)
        {
            return MessagePackSerializer.Serialize<T>(value, messagePackOptions);
        }
        
        var lz4Options = messagePackOptions.WithCompression(MessagePackCompression.Lz4BlockArray);
        return MessagePackSerializer.Serialize<T>(value, lz4Options);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
    {
        if (options.Compression != SerializerCompression.Compress)
        {
            return MessagePackSerializer.Deserialize<T>(data.ToArray(), messagePackOptions);
        }
        
        var lz4Options = messagePackOptions.WithCompression(MessagePackCompression.Lz4BlockArray);
        return MessagePackSerializer.Deserialize<T>(data.ToArray(), lz4Options);
    }
}