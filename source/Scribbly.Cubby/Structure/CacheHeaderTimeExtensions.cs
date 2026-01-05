using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// Extension methods used to compute time.
/// </summary>
public static class CacheHeaderTimeExtensions
{
    /// <summary>
    /// An expiration time in total UTC ticks
    /// </summary>
    /// <param name="expiration">The total UTC ticks</param>
    extension(long expiration)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsExpired(long nowUtcTicks)
        { 
            return expiration != 0 && expiration <= nowUtcTicks;
        }
    }

    /// <summary>
    /// An expiration time in total UTC ticks
    /// </summary>
    /// <param name="expiration">The total UTC ticks</param>
    extension(long? expiration)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsExpired(long nowUtcTicks)
        { 
            return expiration != 0 && expiration <= nowUtcTicks;
        }
    }
}