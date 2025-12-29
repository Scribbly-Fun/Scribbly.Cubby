namespace Scribbly.Cubby;

/// <summary>
/// Header keys used to exchange information across HTTP requests
/// </summary>
public sealed class Headers
{
    /// <summary>
    /// Header key used for the cache entry flags
    /// </summary>
    public const string CubbyHeaderFlags = "x-cubby-flags";
    
    /// <summary>
    /// Header key used for the cache entry encoding flags.
    /// </summary>
    public const string CubbyHeaderEncoding = "x-cubby-encoding";
    
    /// <summary>
    /// Key used for the expiration value
    /// </summary>
    public const string CubbyHeaderExpiration = "x-cubby-expiry";
}