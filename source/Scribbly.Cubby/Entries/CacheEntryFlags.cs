using System.Runtime.CompilerServices;

namespace Scribbly.Cubby;

/// <summary>
/// Flags declaring how the cache entry is used.
/// </summary>
[Flags]
public enum CacheEntryFlags : short
{
    /// <summary>
    /// Basic manual eviction cached record
    /// </summary>
    None            = 0,
    /// <summary>
    /// When set the data in the cache will be compressed.
    /// </summary>
    Compressed      = 1 << 0,
    /// <summary>
    /// When sliding each read on the cache will push the time out.
    /// </summary>
    Sliding         = 1 << 1,
    /// <summary>
    /// Marked tombstone when the cache entry should be evicted.
    /// </summary>
    Tombstone       = 1 << 2
}

/// <summary>
/// Extension methods used to operate on the cache flags
/// </summary>
public static class CacheEntryFlagExtensions
{
    private const string FlagNone = "None";
    private const string FlagCompressed = "Compressed";
    private const string FlagSliding = "Sliding";
    private const string FlagCompressedSliding = "Compressed,Sliding";
    private const string FlagTombstone = "Tombstone";
    private const string FlagCompressedTombstone = "Compressed,Tombstone";
    private const string FlagSlidingTombstone = "Sliding,Tombstone";
    private const string FlagCompressedSlidingTombstone = "Compressed,Sliding,Tombstone";

    /// <summary>
    /// Extends the flags
    /// </summary>
    /// <param name="flags">The flags value</param>
    extension(CacheEntryFlags flags)
    {
        /// <summary>
        /// Returns a cached string representing the flags
        /// </summary>
        /// <returns>A string</returns>
        public string ToFlagsString()
        {
            var value = (short)flags;

            return value switch
            {
                0 => FlagNone,
                1 => FlagCompressed,
                2 => FlagSliding,
                3 => FlagCompressedSliding,
                4 => FlagTombstone,
                5 => FlagCompressedTombstone,
                6 => FlagSlidingTombstone,
                7 => FlagCompressedSlidingTombstone,
                _ => throw new ArgumentOutOfRangeException(nameof(flags)),
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsTombstone() => (flags & CacheEntryFlags.Tombstone) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsSliding() => (flags & CacheEntryFlags.Sliding) != 0;
    }
    
    /// <summary>
    /// A value representing the flag
    /// </summary>
    /// <param name="value">A string value representing the enum</param>
    extension(string value)
    {
        /// <summary>
        /// Parses a cached flags string into <see cref="CacheEntryFlags"/>.
        /// </summary>
        public CacheEntryFlags ToCacheEntryFlags()
        {
            return value switch
            {
                FlagNone => CacheEntryFlags.None,
                FlagCompressed => CacheEntryFlags.Compressed,
                FlagSliding => CacheEntryFlags.Sliding,
                FlagCompressedSliding => CacheEntryFlags.Compressed | CacheEntryFlags.Sliding,
                FlagTombstone => CacheEntryFlags.Tombstone,
                FlagCompressedTombstone => CacheEntryFlags.Compressed | CacheEntryFlags.Tombstone,
                FlagSlidingTombstone => CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone,
                FlagCompressedSlidingTombstone => CacheEntryFlags.Compressed | CacheEntryFlags.Sliding | CacheEntryFlags.Tombstone,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CacheEntryFlags string.")
            };
        }
    }
}