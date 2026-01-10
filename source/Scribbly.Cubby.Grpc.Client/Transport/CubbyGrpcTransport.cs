using Google.Protobuf;
using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;
using PutResult = Scribbly.Cubby.Stores.PutResult;
using EvictResult = Scribbly.Cubby.Stores.EvictResult;
using RefreshResult = Scribbly.Cubby.Stores.RefreshResult;

namespace Scribbly.Cubby;

internal class CubbyGrpcTransport(CacheService.CacheServiceClient client) : IGrpcCubbyStoreTransport
{
    /// <inheritdoc />
    public PutResult Put(in BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options = null)
    {
        options ??= CacheEntryOptions.None;
        
        var result = client.Put(new PutRequest
        {
            Key = ByteString.CopyFrom(key),
            Value = ByteString.CopyFrom(value.Span),
            Encoding = (short)options.Encoding,
            Flags = (short)options.Encoding,
            Expiration = options.AbsoluteExpiration,
            Duration = options.SlidingDuration
        });

        return result switch
        {
            { Result: Proto.PutResult.Undefined } => PutResult.Undefined,
            { Result: Proto.PutResult.Created } => PutResult.Created,
            { Result: Proto.PutResult.Updated } => PutResult.Updated,

            _ => PutResult.Undefined
        };
    }

    /// <inheritdoc />
    public async ValueTask<PutResult> PutAsync(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options, CancellationToken token = default)
    {
        options ??= CacheEntryOptions.None;

        var result = await client.PutAsync(new PutRequest
        {
            Key = ByteString.CopyFrom(key),
            Value = ByteString.CopyFrom(value.Span),
            Encoding = (short)options.Encoding,
            Flags = (short)options.Flags,
            Expiration = options.AbsoluteExpiration,
            Duration = options.SlidingDuration
            
        }, cancellationToken: token);

        return result switch
        {
            { Result: Proto.PutResult.Undefined } => PutResult.Undefined,
            { Result: Proto.PutResult.Created } => PutResult.Created,
            { Result: Proto.PutResult.Updated } => PutResult.Updated,

            _ => PutResult.Undefined
        };
    }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(in BytesKey key)
    {
        var entry = client.Get(new GetRequest
        {
            Key = ByteString.CopyFromUtf8(key)
            
        });

        return entry.Value.ToByteArray();
    }

    /// <inheritdoc />
    public async ValueTask<ReadOnlyMemory<byte>> GetAsync(BytesKey key, CancellationToken token = default)
    {
        var entry = await client.GetAsync(new GetRequest
        {
            Key = ByteString.CopyFromUtf8(key)
            
        }, cancellationToken: token);

        return entry.Value.ToByteArray();
    }
    
    /// <inheritdoc />
    public bool Exists(in BytesKey key)
    {
        var entry = client.Exists(new ExistsRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        });

        return entry is { Found: true };
    }

    /// <inheritdoc />
    public async ValueTask<bool> ExistsAsync(BytesKey key, CancellationToken token = default)
    {
        var entry = await client.ExistsAsync(new ExistsRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        }, cancellationToken: token);

        return entry is { Found: true };
    }

    /// <inheritdoc />
    public RefreshResult Refresh(in BytesKey key)
    {
        var result = client.Refresh(new RefreshRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        });

        return result switch
        {
            { Result: Proto.RefreshResult.Updated } => RefreshResult.Updated,
            { Result: Proto.RefreshResult.NotSliding } => RefreshResult.NotSlidingEntry,
            { Result: Proto.RefreshResult.Undefined } => RefreshResult.Undefined,
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }

    /// <inheritdoc />
    public async ValueTask<RefreshResult> RefreshAsync(BytesKey key, CancellationToken token = default)
    {
        var result = await client.RefreshAsync(new RefreshRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        }, cancellationToken: token);

        return result switch
        {
            { Result: Proto.RefreshResult.Updated } => RefreshResult.Updated,
            { Result: Proto.RefreshResult.NotSliding } => RefreshResult.NotSlidingEntry,
            { Result: Proto.RefreshResult.Undefined } => RefreshResult.Undefined,
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }

    /// <inheritdoc />
    public EvictResult Evict(in BytesKey key)
    {
        var result = client.Evict(new EvictRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        });

        return result switch
        {
            { Result: Proto.EvictResult.Removed } => EvictResult.Removed,
            { Result: Proto.EvictResult.Unknown } => EvictResult.Unknown,
            { Result: Proto.EvictResult.Undefined } => EvictResult.Undefined,
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }
    
    /// <inheritdoc />
    public async ValueTask<EvictResult> EvictAsync(BytesKey key, CancellationToken token = default)
    {
        var result = await client.EvictAsync(new EvictRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        });

        return result switch
        {
            { Result: Proto.EvictResult.Removed } => EvictResult.Removed,
            { Result: Proto.EvictResult.Unknown } => EvictResult.Unknown,
            { Result: Proto.EvictResult.Undefined } => EvictResult.Undefined,
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }
}