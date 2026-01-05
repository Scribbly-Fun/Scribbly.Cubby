using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Scribbly.Cubby.Client.Serializer;

#pragma warning disable SCRB011

/// <summary>
/// A System.Text.Json serializer for the Cubby cache client.
/// </summary>
internal sealed class JsonCubbySerializer(JsonSerializerOptions jsonOptions) : ICubbySerializer
{
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
    {
        if (!jsonOptions.TryGetTypeInfo(typeof(T), out var jsonTypeInfo) || jsonTypeInfo is not JsonTypeInfo<T> converter)
        {
            throw new CubbySerializerException<T>();
        }
        
        return JsonSerializer.SerializeToUtf8Bytes<T>(value, converter);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
    {
        if (!jsonOptions.TryGetTypeInfo(typeof(T), out var jsonTypeInfo) || jsonTypeInfo is not JsonTypeInfo<T> converter)
        {
            throw new CubbySerializerException<T>();
        }

        return JsonSerializer.Deserialize(data, converter);
    }
}

#pragma warning restore SCRB011