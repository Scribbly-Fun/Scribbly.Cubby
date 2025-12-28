using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Endpoints;

internal static class CacheEntryExtensions
{
    extension(CacheParameters parameters)
    {
        public CacheEntryOptions ToEntryOptions()
        {
            return new CacheEntryOptions
            {
                Flags = parameters.Flags,
                TimeToLive = parameters.Expiration,
                Encoding = CacheEntryEncoding.None
            };
        }
    }
}