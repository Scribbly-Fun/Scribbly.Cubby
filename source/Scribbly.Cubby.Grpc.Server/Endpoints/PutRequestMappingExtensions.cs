using Scribbly.Cubby.Proto;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class PutRequestMappingExtensions
{
    extension(PutRequest request)
    {
        internal CacheEntryOptions ToOptions(TimeProvider provider)
        {
            return CacheEntryOptions.From(
                provider,
                flags: (CacheEntryFlags)request.Flags,
                encoding:(CacheEntryEncoding)request.Encoding,
                expiration: TimeSpan.FromTicks(request.Expiration));
        }
    }
}