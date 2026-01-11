using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Client;

/// <summary>
/// The value from the transport response for a cache entry
/// </summary>
public readonly struct EntryResponse
{
    /// <summary>
    /// True when the cache was found and populated with a value
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Value))]
    public bool Found => Value is { Length: > 0 };
    
    /// <summary>
    /// The cache flags
    /// </summary>
    public required CacheEntryFlags Flags { get; init; }
    
    /// <summary>
    /// Cached encoding
    /// </summary>
    public required CacheEntryEncoding Encoding { get; init; }
    
    /// <summary>
    /// How long before the cache will be expired
    /// </summary>
    /// <remarks>0 is never</remarks>
    public required long Expiration { get; init; }
    
    /// <summary>
    /// The value of the cached data
    /// </summary>
    public required ReadOnlyMemory<byte> Value { get; init; }
    
    /// <summary>
    /// Creates a new entry response
    /// </summary>
    public static EntryResponse Create(CacheEntryFlags flags, CacheEntryEncoding encoding, long expiration, ReadOnlyMemory<byte> value) => new ()
    {
        Flags = flags,
        Encoding = encoding,
        Expiration = expiration,
        Value = value
    };

    /// <summary>
    /// An empty response
    /// </summary>
    public static EntryResponse Empty { get; } = new()
    {
        Flags = CacheEntryFlags.None,
        Encoding = CacheEntryEncoding.None,
        Expiration = 0,
        Value = ReadOnlyMemory<byte>.Empty
    };
}

/// <summary>
/// The value from the transport response for a cache entry
/// </summary>
public readonly struct EntryResponse<T> where T : notnull
{
    /// <summary>
    /// True when the cache was found and populated with a value
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Value))]
    public bool Found => Value is not null;
    
    /// <summary>
    /// The cache flags
    /// </summary>
    public required CacheEntryFlags Flags { get; init; }
    
    /// <summary>
    /// Cached encoding
    /// </summary>
    public required CacheEntryEncoding Encoding { get; init; }
    
    /// <summary>
    /// How long before the cache will be expired
    /// </summary>
    /// <remarks>0 is never</remarks>
    public required long Expiration { get; init; }
    
    /// <summary>
    /// The value of the cached data
    /// </summary>
    public required T? Value { get; init; }
    
    /// <summary>
    /// Creates a new entry response
    /// </summary>
    public static EntryResponse<T> Create(CacheEntryFlags flags, CacheEntryEncoding encoding, long expiration, T value) => new ()
    {
        Flags = flags,
        Encoding = encoding,
        Expiration = expiration,
        Value = value
    };
    
    /// <summary>
    /// An empty response
    /// </summary>
    public static EntryResponse<T> Empty { get; } = new()
    {
        Flags = CacheEntryFlags.None,
        Encoding = CacheEntryEncoding.None,
        Expiration = 0,
        Value = default(T)
    };
}