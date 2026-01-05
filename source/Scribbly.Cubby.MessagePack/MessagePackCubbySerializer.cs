using System.Runtime.CompilerServices;
using Nerdbank.MessagePack;
using PolyType;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.MessagePack;
#pragma warning disable SCRB011

/// <summary>
/// Serializes data using the Message Pack Protocol
/// </summary>
public class MessagePackCubbySerializer(MessagePackSerializer serializer, ITypeShapeProvider shapeProvider) : ICubbySerializer
{
    
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
    {
        var shape = shapeProvider.GetTypeShape<T>();
        if (shape is null)
        {
            throw new CubbySerializerException<T>();
        }
        return serializer.Serialize(value, shape);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
    {
        var shape = shapeProvider.GetTypeShape<T>();
        if (shape is null)
        {
            throw new CubbySerializerException<T>();
        }
        return serializer.Deserialize(data.ToArray(), shape);
    }
}
#pragma warning restore SCRB011
