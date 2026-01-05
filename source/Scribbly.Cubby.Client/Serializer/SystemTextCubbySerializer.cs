using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// A System.Text.Json serializer for the Cubby cache client.
/// </summary>
internal sealed class SystemTextCubbySerializer(JsonSerializerOptions jsonOptions) : ICubbySerializer
{
    /// <inheritdoc />
#pragma warning disable SCRB011
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull
#pragma warning restore SCRB011
    {
        return JsonSerializer.SerializeToUtf8Bytes<T>(value, jsonOptions);
    }

    /// <inheritdoc />
#pragma warning disable SCRB011
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull
#pragma warning restore SCRB011
    {
        return JsonSerializer.Deserialize<T>(data, jsonOptions);
    }
}