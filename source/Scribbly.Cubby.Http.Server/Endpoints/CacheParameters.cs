using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Cubby.Endpoints;

internal sealed record CacheParameters(
    [FromHeader(Name = Headers.CubbyHeaderExpiration)] TimeSpan Expiration,
    [FromHeader(Name = Headers.CubbyHeaderFlags)] CacheEntryFlags Flags = CacheEntryFlags.None,
    [FromHeader(Name = Headers.CubbyHeaderEncoding)] CacheEntryEncoding Encoding = CacheEntryEncoding.None);