using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Scribbly.Cubby.Stores;
using PutResult = Scribbly.Cubby.Stores.PutResult;

namespace Scribbly.Cubby;

internal class CubbyHttpTransport(IHttpClientFactory factory) : IHttpCubbyStoreTransport
{
    /// <inheritdoc />
    public PutResult Put(in BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options = null)
    {
        options ??= CacheEntryOptions.None;
        
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby",
            Query = $"key={key}" 
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Put, uri.Uri);
        request.Content = new ByteArrayContent(value.ToArray());
        request.Headers.Add(Headers.CubbyHeaderFlags, options.Flags.ToFlagsString());
        request.Headers.Add(Headers.CubbyHeaderEncoding, options.Encoding.ToEncodingString());
        request.Headers.Add(Headers.CubbyHeaderExpiration, options.SlidingDuration.ToString());

        using var response = client.Send(request);

        return response switch
        {
            { StatusCode: HttpStatusCode.BadRequest } => PutResult.Undefined,
            { StatusCode: HttpStatusCode.Created } => PutResult.Created,
            { StatusCode: HttpStatusCode.OK } => PutResult.Updated,

            _ => PutResult.Undefined
        };
    }
    
    /// <inheritdoc />
    public async ValueTask<PutResult> PutAsync(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options, CancellationToken token = default)
    {
        options ??= CacheEntryOptions.None;
        
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby",
            Query = $"key={key}" 
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Put, uri.Uri);
        request.Content = new ByteArrayContent(value.ToArray());
        request.Headers.Add(Headers.CubbyHeaderFlags, options.Flags.ToFlagsString());
        request.Headers.Add(Headers.CubbyHeaderEncoding, options.Encoding.ToEncodingString());
        request.Headers.Add(Headers.CubbyHeaderExpiration, TimeSpan.FromTicks(options.SlidingDuration).ToString());

        using var response = await client.SendAsync(request, token);

        return response switch
        {
            { StatusCode: HttpStatusCode.BadRequest } => PutResult.Undefined,
            { StatusCode: HttpStatusCode.Created } => PutResult.Created,
            { StatusCode: HttpStatusCode.OK } => PutResult.Updated,

            _ => PutResult.Undefined
        };
    }
    
    /// <inheritdoc />
    public ReadOnlyMemory<byte> Get(in BytesKey key)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby",
            Query = $"key={key}" 
        };
     
        using var request = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
        using var response = client.Send(request);

        using var stream = response.Content.ReadAsStream();

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <inheritdoc />
    public async ValueTask<ReadOnlyMemory<byte>> GetAsync(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby",
            Query = $"key={key}" 
        };
        
        using var response = await client.GetAsync(uri.Uri, token);

        return await response.Content.ReadAsByteArrayAsync(token);
    }
    
    /// <inheritdoc />
    public bool Exists(in BytesKey key)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby/exists",
            Query = $"key={key}" 
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Put, uri.Uri);
        
        using var response = client.Send(request);

        return response.StatusCode == HttpStatusCode.OK;
    }
    
    /// <inheritdoc />
    public async ValueTask<bool> ExistsAsync(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby/exists",
            Query = $"key={key}" 
        };
        
        using var response = await client.PutAsync(
            uri.Uri,
            null, 
            token);

        return response.StatusCode == HttpStatusCode.OK;
    }
    
    /// <inheritdoc />
    public RefreshResult Refresh(in BytesKey key)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby/refresh",
            Query = $"key={key}" 
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Put, uri.Uri);
        
        using var response = client.Send(request);

        return response.StatusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Accepted => RefreshResult.Updated,
            HttpStatusCode.BadRequest => RefreshResult.NotSlidingEntry,
            _ => RefreshResult.Undefined
        };
    }
    
    /// <inheritdoc />
    public async ValueTask<RefreshResult> RefreshAsync(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby/refresh",
            Query = $"key={key}" 
        };
        
        using var response = await client.PutAsync(
            uri.Uri,
            null, 
            token);

        return response.StatusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Accepted => RefreshResult.Updated,
            HttpStatusCode.BadRequest => RefreshResult.NotSlidingEntry,
            _ => RefreshResult.Undefined
        };
    }

    /// <inheritdoc />
    public EvictResult Evict(in BytesKey key)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby",
            Query = $"key={key}" 
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Put, uri.Uri);
        
        using var response = client.Send(request);

        return response.StatusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Accepted => EvictResult.Removed,
            HttpStatusCode.BadRequest => EvictResult.Unknown,
            _ => EvictResult.Undefined
        };
    }
    
    /// <inheritdoc />
    public async ValueTask<EvictResult> EvictAsync(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/cubby",
            Query = $"key={key}" 
        };
        
        using var response = await client.DeleteAsync(
            uri.Uri,
            token);

        return response.StatusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Accepted => EvictResult.Removed,
            HttpStatusCode.BadRequest => EvictResult.Unknown,
            _ => EvictResult.Undefined
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string? GetSingleHeader(
        HttpResponseHeaders headers,
        string name)
    {
        if (!headers.TryGetValues(name, out var values))
            return null;

        using var e = values.GetEnumerator();
        return e.MoveNext() ? e.Current : null;
    }
}

