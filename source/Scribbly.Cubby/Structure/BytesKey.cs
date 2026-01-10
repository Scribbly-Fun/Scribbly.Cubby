using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Scribbly.Cubby;

/// <summary>
/// A key used to locate an item in the cache.
/// </summary>
public readonly struct BytesKey : IEquatable<BytesKey>
{
    private readonly ReadOnlyMemory<byte> _bytes;
    private readonly int _hash;
    
    /// <summary>
    /// Creates a new key from a byte array
    /// </summary>
    /// <param name="bytes">The bytes</param>
    public BytesKey(byte[] bytes)
    {
        _bytes = bytes;
        _hash = ComputeHash(bytes);
    }

    /// <summary>
    /// Creates a new key from a byte array
    /// </summary>
    /// <param name="bytes">The bytes</param>
    public BytesKey(ReadOnlyMemory<byte> bytes)
    {
        _bytes = bytes;
        _hash = ComputeHash(bytes.Span);
    }

    /// <summary>
    /// Creates a new key from a byte array
    /// </summary>
    /// <param name="bytes">The bytes</param>
    public BytesKey(ReadOnlySpan<byte> bytes)
    {
        _bytes = bytes.ToArray();
        _hash = ComputeHash(bytes);
    }

    
    /// <summary>
    /// Gets the requested byte at the index provided
    /// </summary>
    /// <param name="index">The bytes index</param>
    public byte this[int index] => _bytes.Span[index];


    /// <inheritdoc />
    public bool Equals(BytesKey other)
        => _hash == other._hash && _bytes.Span.SequenceEqual(other._bytes.Span);

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
    public override string ToString() => Encoding.UTF8.GetString(_bytes.Span);
    
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
    /// <param name="key">The bytes key to covert to a string</param>
    /// <returns>A string encoded as UTF8</returns>
    public static implicit operator string(BytesKey key) => Encoding.UTF8.GetString(key._bytes.Span);
    
    /// <summary>
    /// Assigns a key from the value of a string.
    /// </summary>
    /// <param name="value">A string used to encode a UTF 8 byte array used as the key.</param>
    /// <returns>A new bytes key</returns>
    public static implicit operator BytesKey(string value)
    {
        var chars = value.AsSpan();
        var byteCount = Encoding.UTF8.GetByteCount(chars);
        
        Span<byte> bytes = new byte[byteCount]; 
        Encoding.UTF8.GetBytes(chars, bytes);
        
        return new BytesKey(bytes);
    }

    /// <summary>
    /// Assigns a string from the value of the internal buffer in the key.
    /// </summary>
    /// <param name="key">The bytes key to covert to a string</param>
    /// <returns>A string encoded as UTF8</returns>
    public static implicit operator byte[](BytesKey key) => key._bytes.ToArray();
    
    /// <summary>
    /// Assigns a key from the value of a string.
    /// </summary>
    /// <param name="value">A string used to encode a UTF 8 byte array used as the key.</param>
    /// <returns>A new bytes key</returns>
    public static implicit operator BytesKey(byte[] value) => new BytesKey(value);

    /// <summary>
    /// Assigns a string from the value of the internal buffer in the key.
    /// </summary>
    /// <param name="key">The bytes key to covert to a string</param>
    /// <returns>A string encoded as UTF8</returns>
    public static implicit operator ReadOnlySpan<byte>(BytesKey key) => key._bytes.Span;
    
    /// <summary>
    /// Assigns a key from the value of a string.
    /// </summary>
    /// <param name="value">A string used to encode a UTF 8 byte array used as the key.</param>
    /// <returns>A new bytes key</returns>
    public static implicit operator BytesKey(ReadOnlySpan<byte> value) => new BytesKey(value.ToArray());
    
    public static bool TryParse(
        string? value,
        IFormatProvider? _,
        out BytesKey result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }
        
        result= value;
        return true;
    }
}
