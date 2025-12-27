namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// Abstract used to serialize data in and out of the cache.
/// </summary>
public interface ICubbySerializer
{
    /// <summary>
    /// Serializes data with optional configuration to be stored in the cubby cache store.
    /// </summary>
    /// <param name="value">The data value to store in the cache.</param>
    /// <param name="options">Options to configure how the data will be encoded.</param>
    /// <typeparam name="T">The type to encode</typeparam>
    /// <returns>A readonly byte array of data representing the object serialized.</returns>
    ReadOnlySpan<byte> Serialize<T>(T value, SerializerOptions options = default) where T : notnull;

    /// <summary>
    /// Deserializes data with optional configuration from the stored value in the cubby cache store.
    /// </summary>
    /// <param name="data">The data value stored in the cache.</param>
    /// <param name="options">Options to configure how the data will be encoded.</param>
    /// <typeparam name="T">The type to decode</typeparam>
    /// <returns>A materialized type from the stored bytes</returns>
    T? Deserialize<T>(ReadOnlySpan<byte> data, SerializerOptions options = default) where T : notnull;
}