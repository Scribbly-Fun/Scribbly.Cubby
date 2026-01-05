namespace Scribbly.Cubby.Expiration;

internal interface IExpirationEvictionService
{
    /// <summary>
    ///     Removes all entries from the cache that are either marked tombstone or expired
    /// </summary>
    /// <param name="nowUtcTicks">
    ///     The current datetime used to evaluate expiration
    /// </param>
    /// <remarks>
    ///     Iterates through all the entries in the cache when there no more than 4 active writers.
    ///     If the elapsed time is exceeded we can assume there are lots of cache hits active and exit the process.
    /// </remarks>
    void CleanCacheStorage(long nowUtcTicks);
}