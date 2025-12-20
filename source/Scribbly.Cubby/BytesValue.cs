namespace Scribbly.Cubby;

/// <summary>
/// A value stored in cache
/// </summary>
public sealed class BytesValue
{
    /// <summary>
    /// The data in the entry
    /// </summary>
    public readonly ReadOnlyMemory<byte> Data;

    /// <summary>
    /// Creates a new entry
    /// </summary>
    /// <param name="data">With this data</param>
    public BytesValue(ReadOnlyMemory<byte> data)
    {
        Data = data;
    }
}