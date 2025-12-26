using System.Text.Json;

namespace Scribbly.Cubby.Client.Serializer;

internal sealed class SystemTextCubbySerializer : ICubbySerializer
{
    /// <inheritdoc />
    public ReadOnlySpan<byte> Serialize<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes<T>(value);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(ReadOnlySpan<byte> data)
    {
        return JsonSerializer.Deserialize<T>(data);
    }
}