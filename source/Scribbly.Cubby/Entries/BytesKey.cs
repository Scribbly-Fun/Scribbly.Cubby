using System.Text;

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
    public BytesKey(byte[] data)
    {
        _data = data;
        _hash = ComputeHash(data);
    }
    
    /// <summary>
    /// Gets the requested byte at the index provided
    /// </summary>
    /// <param name="index">The bytes index</param>
    public byte this[int index] => _data[index];


    /// <inheritdoc />
    public bool Equals(BytesKey other)
        => _hash == other._hash && _data.AsSpan().SequenceEqual(other._data);

    /// <inheritdoc />
    public override bool Equals(object? obj) => 
        obj is BytesKey key && Equals(key);
    
    /// <summary>
    /// Compares the two items
    /// </summary>
    /// <param name="left">Item 1</param>
    /// <param name="right">Item 2</param>
    /// <returns>True when the bytes are the same</returns>
    public static bool operator ==(BytesKey left, BytesKey right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares the two items
    /// </summary>
    /// <param name="left">Item 1</param>
    /// <param name="right">Item 2</param>
    /// <returns>True when the bytes are not the same</returns>
    public static bool operator !=(BytesKey left, BytesKey right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Assigns a string from the value of the internal buffer in the key.
    /// </summary>
    /// <param name="key">The bytes key to covnert to a string</param>
    /// <returns>A string encoded as UTF8</returns>
    public static implicit operator string(BytesKey key) => Encoding.UTF8.GetString(key._data);
    
    /// <summary>
    /// Assigns a key from the value of a string.
    /// </summary>
    /// <param name="value">A string used to encode a UTF 8 byte array used as the key.</param>
    /// <returns>A new bytes key</returns>
    public static implicit operator BytesKey(string value) => new BytesKey(Encoding.UTF8.GetBytes(value));
}
