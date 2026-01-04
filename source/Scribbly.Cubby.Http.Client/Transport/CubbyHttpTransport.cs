using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Scribbly.Cubby.Client;
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
        request.Headers.Add(Headers.CubbyHeaderFlags, options.Flags.ToFlagsString());
        request.Headers.Add(Headers.CubbyHeaderEncoding, options.Encoding.ToEncodingString());
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
    public async ValueTask<EntryResponse> Get(BytesKey key, CancellationToken token = default)
    {
        using var client = factory.CreateClient(nameof(CubbyHttpTransport));
        
        using var response = await client.GetAsync($"/cubby/{key}", token);

        var value = await response.Content.ReadAsByteArrayAsync(token);
        
        var flagsHeader = GetSingleHeader(response.Headers, Headers.CubbyHeaderFlags);
        var encodingHeader = GetSingleHeader(response.Headers, Headers.CubbyHeaderEncoding);
        var expirationHeader = GetSingleHeader(response.Headers, Headers.CubbyHeaderExpiration);
        
        return new EntryResponse
        {
            Flags =  flagsHeader?.ToCacheEntryFlags() ?? CacheEntryFlags.None,
            Encoding = encodingHeader?.ToCacheEncoding() ?? CacheEntryEncoding.None,
            Expiration = expirationHeader is not null
                ? long.Parse(expirationHeader, CultureInfo.InvariantCulture)
                : 0,
            Value = value
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

