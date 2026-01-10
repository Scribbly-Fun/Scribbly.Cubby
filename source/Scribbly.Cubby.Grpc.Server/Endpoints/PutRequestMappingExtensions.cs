using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class PutRequestMappingExtensions
{
    extension(PutRequest request)
    {
        internal CacheEntryOptions ToOptions(TimeProvider provider)
        {
            var flags = (CacheEntryFlags)request.Flags;
            
            return request switch
            {
                { Duration: > 0, Expiration: > 0 } when flags.HasFlag(CacheEntryFlags.Sliding) => CacheEntryOptions.Sliding(
                    provider,
                    flags: flags,
                    encoding: (CacheEntryEncoding)request.Encoding,
                    duration: TimeSpan.FromTicks(request.Duration)),

                { Expiration: > 0, Duration: 0 } when !flags.HasFlag(CacheEntryFlags.Sliding) => CacheEntryOptions.Absolute(
                    provider,
                    flags: flags,
                    encoding: (CacheEntryEncoding)request.Encoding,
                    expirationTime: new DateTimeOffset(request.Expiration, TimeSpan.Zero)),

                _ => CacheEntryOptions.From(
                    provider,
                    flags: flags,
                    encoding: (CacheEntryEncoding)request.Encoding,
                    expiration: TimeSpan.FromTicks(request.Expiration))
            };
        }
    }
}