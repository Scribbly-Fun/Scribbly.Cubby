using System.Buffers;
using Google.Protobuf;

namespace Scribbly.Cubby.Values;

internal sealed class CacheValue
{
    public readonly ReadOnlyMemory<byte> Data;

    public CacheValue(ReadOnlyMemory<byte> data)
    {
        Data = data;
    }
}

public static class ByteStringExtensions
{
    extension(ByteString source)
    {
        public byte[] CopyFrom()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(source.Length);
            source.Span.CopyTo(buffer);
            return buffer;
        }
    }
}