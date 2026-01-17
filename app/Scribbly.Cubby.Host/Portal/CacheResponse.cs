using System.Text.Json.Serialization;

namespace Scribbly.Cubby.Host.Portal;

[JsonSerializable(typeof(IEnumerable<CacheResponse>))]
internal partial class CacheResponseListJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(CacheResponse))]
internal partial class CacheResponseJsonContext : JsonSerializerContext;

public record CacheResponse(
    [property: JsonPropertyName("key")] string CacheKey,
    [property: JsonPropertyName("flags")] string Flags,
    [property: JsonPropertyName("encoding")] string Encoding,
    [property: JsonPropertyName("size")] int Size);

public static class CacheResponseMapping
{
    extension(KeyValuePair<BytesKey, byte[]> entry)
    {
        public CacheResponse Response
        {
            get
            {
                var header = entry.Value.GetHeader();
                
                var flags = header.GetFlags();
                var encoding = header.GetEncoding();
                return new CacheResponse(entry.Key, flags.ToFlagsString(), encoding.ToEncodingString(), entry.Value.Length);
            }
        }
    }
}

