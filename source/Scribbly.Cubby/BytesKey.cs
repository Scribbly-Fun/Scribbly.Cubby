namespace Scribbly.Cubby;

/// <summary>
/// A key used to locate an item in the cache.
/// </summary>
public readonly struct BytesKey : IEquatable<BytesKey>
{
    private readonly byte[] _data;
    private readonly int _hash;

    /// <summary>
    /// Creates a new key from a byte array
    /// </summary>
    /// <param name="data">The bytes</param>
    internal BytesKey(byte[] data)
    {
        _data = data;
        _hash = ComputeHash(data);
    }
    
    public byte this[int index] => _data[index];

    public bool Equals(BytesKey other)
        => _hash == other._hash && _data.AsSpan().SequenceEqual(other._data);

    public override bool Equals(object? obj) => 
        obj is BytesKey key && Equals(key);

    public override int GetHashCode() => _hash;

    private static int ComputeHash(ReadOnlySpan<byte> data)
    {
        unchecked
        {
            var hash = 17;
            foreach (var b in data)
                hash = hash * 31 + b;
            
            return hash;
        }
    }
}
