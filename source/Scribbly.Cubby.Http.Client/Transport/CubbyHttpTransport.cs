using System.Net;
using Scribbly.Cubby.Stores;
using PutResult = Scribbly.Cubby.Stores.PutResult;

namespace Scribbly.Cubby;

internal class CubbyHttpTransport(IHttpClientFactory factory) : IHttpCubbyStoreTransport
{
    /// <inheritdoc />
    public async ValueTask<bool> Exists(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        using var response = await client.PutAsync(
            $"/cubby/{key}/exists",
            null, 
            token);

        return response.StatusCode == HttpStatusCode.Accepted;
    }

    /// <inheritdoc />
    public async ValueTask<PutResult> Put(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options, CancellationToken token = default)
    {
        options ??= CacheEntryOptions.None;
        
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        using var request = new HttpRequestMessage(HttpMethod.Put, $"/cubby/{key}");
        request.Content = new ByteArrayContent(value.ToArray());
        request.Headers.Add(Headers.CubbyHeaderFlags, options.Flags.ToString());
        request.Headers.Add(Headers.CubbyHeaderEncoding, options.Encoding.ToString());
        request.Headers.Add(Headers.CubbyHeaderExpiration, options.SlidingDuration.ToString());

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
    public async ValueTask<ReadOnlyMemory<byte>> Get(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        using var response = await client.GetAsync($"/cubby/{key}", token);

        return await response.Content.ReadAsByteArrayAsync(token);
    }
}