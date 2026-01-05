using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class HttpCacheParametersExtensions
{
    extension(CacheParameters parameters)
    {
        public CacheEntryOptions ToEntryOptions(TimeProvider provider)
        {
            return CacheEntryOptions.From(
                provider: provider,
                flags: parameters.Flags ?? CacheEntryFlags.None,
                expiration: parameters.Expiration,
                encoding: parameters.Encoding ?? CacheEntryEncoding.None
            );
        }
    }
}