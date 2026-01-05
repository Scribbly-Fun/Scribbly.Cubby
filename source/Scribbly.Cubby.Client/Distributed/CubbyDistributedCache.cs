using Microsoft.Extensions.Caching.Distributed;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

internal class CubbyDistributedCache(ICubbyStoreTransport transport) : IDistributedCache
{
    /// <inheritdoc />
    public byte[]? Get(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var entry = await transport.Get(key, token);
        var value = entry.Span.GetValue();
        return value.ToArray();
    }

    /// <inheritdoc />
    public void Refresh(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        // TODO: Update transport APIs to support all required APIs
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        await transport.Put(key, value, options.CubbyOptions, token);
    }
}