namespace Scribbly.Cubby;

public sealed class BytesValue
{
    public readonly ReadOnlyMemory<byte> Data;

    internal BytesValue(ReadOnlyMemory<byte> data)
    {
        Data = data;
    }
}