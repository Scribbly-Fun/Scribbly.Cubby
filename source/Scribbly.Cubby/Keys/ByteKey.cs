namespace Scribbly.Cubby.Keys;

internal readonly struct ByteKey : IEquatable<ByteKey>
{
    private readonly byte[] _data;
    private readonly int _hash;

    public ByteKey(byte[] data)
    {
        _data = data;
        _hash = ComputeHash(data);
    }

    public bool Equals(ByteKey other)
        => _hash == other._hash && _data.AsSpan().SequenceEqual(other._data);

    public override int GetHashCode() => _hash;

    private static int ComputeHash(ReadOnlySpan<byte> data)
    {
        unchecked
        {
            int hash = 17;
            foreach (var b in data)
                hash = hash * 31 + b;
            return hash;
        }
    }
}
