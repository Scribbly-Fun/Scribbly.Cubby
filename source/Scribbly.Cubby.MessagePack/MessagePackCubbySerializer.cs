using MessagePack;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.MessagePack;

/// <summary>
/// Serializes data using the Message Pack Protocol
/// </summary>
public class MessagePackCubbySerializer(MessagePackSerializerOptions messagePackOptions) : ICubbySerializer
{
    /// <inheritdoc />
#pragma warning disable SCRB011
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
#pragma warning restore SCRB011
    {
        return MessagePackSerializer.Serialize<T>(value, messagePackOptions);
    }

    /// <inheritdoc />
#pragma warning disable SCRB011
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
#pragma warning restore SCRB011
    {
        return MessagePackSerializer.Deserialize<T>(data.ToArray(), messagePackOptions);
    }
}