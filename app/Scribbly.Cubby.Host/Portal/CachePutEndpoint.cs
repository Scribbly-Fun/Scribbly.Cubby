using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal;

internal static class CachePutEndpoint
{
    public static async Task<IResult> Put(
        ICubbyStore store,
        HttpContext context,
        [FromQuery(Name = "key")] BytesKey key,
        [FromQuery] string? encoding,
        [FromQuery] bool compressed = false,
        CancellationToken token = default)
    {
        using var ms = new MemoryStream(context.Request.ContentLength is { } len ? (int)len : 0);
        await context.Request.Body.CopyToAsync(ms, token);

        var buffer = ms.GetBuffer().AsSpan(0, (int)ms.Length).ToArray();
        var flags = compressed ? CacheEntryFlags.Compressed : CacheEntryFlags.None;
        var cacheEncoding = (encoding ?? string.Empty).ToCacheEncoding();

        if (compressed && buffer.Length > 0)
        {
            buffer = BrotliCompress(buffer);
        }

        var result = store.Put(key, buffer, CacheEntryOptions.Never(flags, cacheEncoding));

        return result switch
        {
            PutResult.Created => Results.Created(),
            PutResult.Updated => Results.Ok(),
            PutResult.Undefined => Results.BadRequest(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static byte[] BrotliCompress(ReadOnlySpan<byte> source)
    {
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionLevel.Fastest, leaveOpen: true))
        {
            brotli.Write(source);
        }

        return output.ToArray();
    }
}
