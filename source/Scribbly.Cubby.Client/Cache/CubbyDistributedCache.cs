using Google.Protobuf;
using Microsoft.Extensions.Caching.Distributed;
using Scribbly.Cubby.Proto;

namespace Scribbly.Cubby.Cache;

internal class CubbyDistributedCache(CacheService.CacheServiceClient client) : IDistributedCache
{
    /// <inheritdoc />
    public byte[]? Get(string key)
    {
        var entry = client.Get(new GetRequest
        {
            Key = ByteString.CopyFromUtf8(key)
        });

        return entry?.Value.ToByteArray();
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var entry = await client.GetAsync(new GetRequest
        {
            Key = ByteString.CopyFromUtf8(key)
            
        }, cancellationToken: token);
        
        return entry?.Value.ToByteArray();
    }

    /// <inheritdoc />
    public void Refresh(string key)
    {
    }

    /// <inheritdoc />
    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(string key)
    {

    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        client.Put(new PutRequest
        {
            Key = ByteString.CopyFromUtf8(key),
            Value = ByteString.CopyFrom(value),
        });
    }

    /// <inheritdoc />
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        await client.PutAsync(new PutRequest
        {
            Key = ByteString.CopyFromUtf8(key),
            Value = ByteString.CopyFrom(value),
            
        }, cancellationToken: token);
    }
}