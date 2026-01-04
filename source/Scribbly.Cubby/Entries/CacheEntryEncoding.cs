namespace Scribbly.Cubby;

/// <summary>
/// Describes how the data is being encoded when written to the cache.
/// This will be used for the Admin features to display the data from the cache.
/// </summary>
public enum CacheEntryEncoding : short
{
    /// <summary>
    /// No encoding provided when the entry was set
    /// </summary>
    None            = 0,
    
    /// <summary>
    /// The entry is a UTF string
    /// </summary>
    Utf8String      = 1,

    /// <summary>
    /// The entry is a UTF string
    /// </summary>
    Utf16String     = 2,

    /// <summary>
    /// The entry is JSON encoded array of bytes
    /// </summary>
    Json            = 3,

    /// <summary>
    /// The entry is a MessagePack encoded Object
    /// </summary>
    MessagePack     = 4,
}

/// <summary>
/// Extension methods used to operate on the cache encoding byte
/// </summary>
public static class CacheEntryEncodingExtensions
{
    private const string EncodingNone = "None";
    private const string EncodingUtf8String = "Utf8String";
    private const string EncodingUtf16String = "Utf16String";
    private const string EncodingJson = "Json";
    private const string EncodingMessagePack = "MessagePack";
    
    /// <summary>
    /// Extension methods used to operate on the cache encoding byte
    /// </summary>
    /// <param name="encoding">the encoding</param>
    extension(CacheEntryEncoding encoding)
    {
        /// <summary>
        /// Returns a cached string representing the encoding
        /// </summary>
        /// <returns>A string</returns>
        public string ToEncodingString()
        {
            var value = (short)encoding;

            return value switch
            {
                0 => EncodingNone,
                1 => EncodingUtf8String,
                2 => EncodingUtf16String,
                3 => EncodingJson,
                4 => EncodingMessagePack,
                _ => throw new ArgumentOutOfRangeException(nameof(encoding))
            };
        }
    }
    
    /// <summary>
    /// A value representing the flag
    /// </summary>
    /// <param name="value">A string value representing the enum</param>
    extension(string value)
    {
        /// <summary>
        /// Parses a cached flags string into <see cref="CacheEntryFlags"/>.
        /// </summary>
        public CacheEntryEncoding ToCacheEncoding()
        {
            return value switch
            {
                EncodingNone => CacheEntryEncoding.None,
                EncodingUtf8String => CacheEntryEncoding.Utf8String,
                EncodingUtf16String => CacheEntryEncoding.Utf16String,
                EncodingJson => CacheEntryEncoding.Json,
                EncodingMessagePack => CacheEntryEncoding.MessagePack,
               _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CacheEntryFlags string.")
            };
        }
    }
}