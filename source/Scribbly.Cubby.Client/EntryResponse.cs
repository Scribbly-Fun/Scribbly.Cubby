using System.Diagnostics.CodeAnalysis;
using Scribbly.Cubby.Stores;

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

/// <summary>
/// An object returned from the <see cref="IGetOrCreateCubbyClient"/> factory methods.
/// </summary>
public readonly struct FactoryResponse<T> where T : notnull
{
    /// <summary>
    /// True when the cache was found and populated with a value
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Value))]
    public bool Found => Value is not null;

    /// <summary>
    /// The options used by the cache when inserting a new entry
    /// </summary>
    public required CacheEntryOptions Options { get; init; }
    
    /// <summary>
    /// The value of the cached data
    /// </summary>
    public required T? Value { get; init; }

    /// <summary>
    /// Creates a new instance with defaults.
    /// </summary>
    public FactoryResponse()
    {
        Options = CacheEntryOptions.None;
        Value = default(T);
    }
    
    /// <summary>
    /// Deconstructs the factory result.
    /// </summary>
    public void Deconstruct(out CacheEntryOptions options, out T? value)
    {
        value = Value;
        options = Options;
    }
    
    /// <summary>
    /// Creates a new entry response
    /// </summary>
    public static FactoryResponse<T> Create(CacheEntryOptions options, T value) => new ()
    {
        Options = options,
        Value = value
    };

    /// <summary>
    /// Creates a new entry response
    /// </summary>
    public static FactoryResponse<T> Create(T value) => new ()
    {
        Options = CacheEntryOptions.None,
        Value = value
    };
    
    /// <summary>
    /// An empty response
    /// </summary>
    public static FactoryResponse<T> Empty { get; } = new()
    {
        Options = CacheEntryOptions.None,
        Value = default(T)
    };

    /// <summary>
    /// Creates a new response from a tuple
    /// </summary>
    /// <param name="tuple">The tuple with the options and value</param>
    /// <returns>A new instance of the factory response</returns>
    public static implicit operator FactoryResponse<T>((CacheEntryOptions Options, T Value) tuple) => Create(tuple.Options, tuple.Value);
    
    /// <summary>
    /// Creates a new result with default cache options.
    /// </summary>
    public static implicit operator FactoryResponse<T>(T value) => Create(value);
    
    /// <summary>
    /// Returns true when the value is populated
    /// </summary>
    /// <param name="result">The result from the factory</param>
    /// <returns>True when the value is not null</returns>
    public static implicit operator bool(FactoryResponse<T> result) => result.Found;
}