using Microsoft.Extensions.Caching.Distributed;

namespace Scribbly.Cubby.Client;

internal class CubbyDistributedCache(ICubbyStoreTransport transport) : IDistributedCache
{
    /// <inheritdoc />
    public byte[] Get(string key)
    {
        var entry = transport.Get(key);
        var value = entry.Span.GetValue();
        return value.ToArray();
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var entry = await transport.GetAsync(key, token);
        var value = entry.Span.GetValue();
        return value.ToArray();
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        transport.Put(key, value, options.CubbyOptions);
    }

    /// <inheritdoc />
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        await transport.PutAsync(key, value, options.CubbyOptions, token);
    }
    
    /// <inheritdoc />
    public void Refresh(string key)
    {
        transport.Refresh(key);
    }
    
    /// <inheritdoc />
    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        await transport.RefreshAsync(key, token);
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        transport.Evict(key);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        await transport.EvictAsync(key, token);
    }
}