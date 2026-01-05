using Microsoft.Extensions.Caching.Distributed;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Extends MS distributed cache for Cubby
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    /// The cache options passed to the Distributed cache
    /// </summary>
    /// <param name="options">The options</param>
    extension(DistributedCacheEntryOptions options)
    {
        /// <summary>
        /// Converts the options to cubby option
        /// </summary>
        public CacheEntryOptions CubbyOptions =>
            options switch
            {
                { AbsoluteExpiration: not null } 
                    => CacheEntryOptions.Absolute(options.AbsoluteExpiration.Value),
                { AbsoluteExpirationRelativeToNow: not null } 
                    => CacheEntryOptions.Absolute(DateTimeOffset.UtcNow + options.AbsoluteExpirationRelativeToNow.Value),
                { SlidingExpiration: not null } 
                    => CacheEntryOptions.Sliding(DateTimeOffset.UtcNow, options.SlidingExpiration.Value),
                _ => CacheEntryOptions.None
            };

        /// <summary>
        /// Converts the options to cubby option
        /// </summary>
        /// <param name="provider">A time provider</param>
        /// <returns>The cubby options</returns>
        public CacheEntryOptions ToCubbyOptions(TimeProvider provider) =>
            options switch
            {
                { AbsoluteExpiration: not null } 
                    => CacheEntryOptions.Absolute(options.AbsoluteExpiration.Value),
                { AbsoluteExpirationRelativeToNow: not null } 
                    => CacheEntryOptions.Absolute(provider.GetUtcNow() + options.AbsoluteExpirationRelativeToNow.Value),
                { SlidingExpiration: not null } 
                    => CacheEntryOptions.Sliding(provider.GetUtcNow(), options.SlidingExpiration.Value),
                _ => CacheEntryOptions.None
            };
    }
}