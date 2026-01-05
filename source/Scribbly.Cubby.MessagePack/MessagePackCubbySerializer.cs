using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MessagePack;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.MessagePack;
#pragma warning disable SCRB011

/// <summary>
/// Serializes data using the Message Pack Protocol
/// </summary>
public class MessagePackCubbySerializer(MessagePackSerializerOptions messagePackOptions) : ICubbySerializer
{
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("May invoke serialize with unregistered type.  Ensure cubby configuration supports AOT")]
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
    {
        return MessagePackSerializer.Serialize<T>(value, messagePackOptions);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("May invoke serialize with unregistered type.  Ensure cubby configuration supports AOT")]
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
    {
        return MessagePackSerializer.Deserialize<T>(data.ToArray(), messagePackOptions);
    }
}
#pragma warning restore SCRB011
