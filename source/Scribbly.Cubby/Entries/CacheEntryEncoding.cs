namespace Scribbly.Cubby;

/// <summary>
/// Describes how the data is being encoded when written to the cache.
/// This will be used for the Admin features to display the data from the cache.
/// </summary>
[Flags]
public enum CacheEntryEncoding : short
{
    /// <summary>
    /// No encoding provided when the entry was set
    /// </summary>
    None            = 0,

    /// <summary>
    /// The entry is JSON encoded array of bytes
    /// </summary>
    Json            = 1 << 0,

    /// <summary>
    /// The entry is a UTF string
    /// </summary>
    Utf8String      = 1 << 1,

    /// <summary>
    /// The entry is a UTF string
    /// </summary>
    Utf16String     = 1 << 2
}