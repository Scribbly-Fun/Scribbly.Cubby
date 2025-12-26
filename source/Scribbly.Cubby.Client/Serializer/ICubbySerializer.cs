namespace Scribbly.Cubby.Client.Serializer;

public interface ICubbySerializer
{
    ReadOnlySpan<byte> Serialize<T>(T value);

    T Deserialize<T>(ReadOnlySpan<byte> data);
}