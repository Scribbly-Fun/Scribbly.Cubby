using System.Text.Json.Serialization;

namespace Scribbly.Cubby.Host.Portal;

[JsonSerializable(typeof(IEnumerable<CacheResponse>))]
internal partial class CacheResponseListJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(CacheResponse))]
internal partial class CacheResponseJsonContext : JsonSerializerContext;

public record CacheResponse(
    [property: JsonPropertyName("key")] string CacheKey,
    [property: JsonPropertyName("flags")] CacheEntryFlags Flags,
    [property: JsonPropertyName("encoding")] string Encoding,
    [property: JsonPropertyName("expiration")] DateTimeOffset? Expiration,
    [property: JsonPropertyName("sliding_duration")] TimeSpan? Duration,
    [property: JsonPropertyName("size")] int Size);

public static class CacheResponseMapping
{
    extension(KeyValuePair<BytesKey, byte[]> entry)
    {
        public CacheResponse Response
        {
            get
            {
                var span = entry.Value.AsSpan();
                
                var length = span.GetValueLength();
                var header = span.GetHeader();
                
                var flags = header.GetFlags();
                var encoding = header.GetEncoding();

                var expiration = header.GetExpiration();
                var duration = header.GetSlidingDuration();
                
                return new CacheResponse(entry.Key, flags, encoding.ToEncodingString(), expiration > 0 ? new DateTime(expiration) : null, TimeSpan.FromTicks(duration),  length);
            }
        }
    }
}

