namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// Options used when serializing.
/// </summary>
/// <param name="Compression">Compression options.</param>
public readonly record struct SerializerOptions(SerializerCompression Compression = SerializerCompression.None);

/// <summary>
/// Tells the serializer to compress the data before sending it to the cache
/// </summary>
public enum SerializerCompression : byte
{
    /// <summary>
    /// No compression will be used and raw data will be transmitted to the cache.
    /// </summary>
    None = 0,
    /// <summary>
    /// When set the data will be compressed using the selected serializers compression.
    /// </summary>
    Compress = 1
}
