using Microsoft.AspNetCore.Http;

namespace Scribbly.Cubby.Endpoints;

internal static class HttpResponseHeaderExtensions
{
    extension(HttpResponse response)
    {
        internal void ApplyCubbyResponseHeaders(ReadOnlySpan<byte> buffer)
        {
            var header = buffer.GetHeader();

            response.Headers[Headers.CubbyHeaderFlags] = header.GetFlags().ToFlagsString();
            response.Headers[Headers.CubbyHeaderEncoding] = header.GetEncoding().ToEncodingString();
            response.Headers[Headers.CubbyHeaderExpiration] = header.GetExpiration().ToString();
        }
    }
}