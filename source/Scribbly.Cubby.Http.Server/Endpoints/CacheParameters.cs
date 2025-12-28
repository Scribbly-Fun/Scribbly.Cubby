using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Cubby.Endpoints;

internal sealed record CacheParameters(
    [FromHeader(Name = Headers.CubbyHeaderExpiration)] long Expiration = 0,
    [FromHeader(Name = Headers.CubbyHeaderFlags)] CacheEntryFlags Flags = CacheEntryFlags.None,
    [FromHeader(Name = Headers.CubbyHeaderEncoding)] CacheEntryEncoding Encoding = CacheEntryEncoding.None);