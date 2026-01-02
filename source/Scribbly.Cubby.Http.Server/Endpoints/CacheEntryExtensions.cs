using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class CacheEntryExtensions
{
    extension(CacheParameters parameters)
    {
        public CacheEntryOptions ToEntryOptions(TimeProvider provider)
        {
            return CacheEntryOptions.From(
                provider: provider,
                flags: parameters.Flags,
                expiration: parameters.Expiration,
                encoding: CacheEntryEncoding.None
            );
        }
    }
}