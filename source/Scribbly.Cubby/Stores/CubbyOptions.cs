using System.Text.Json.Serialization;

namespace Scribbly.Cubby.Stores;

/// <summary>
/// Used for configuration bindings.
/// </summary>
[JsonSerializable(typeof(CubbyOptions))]
internal partial class CubbyOptionsJsonContext : JsonSerializerContext;

/// <summary>
/// Application setup configuration options.
/// </summary>
public class CubbyOptions
{
    /// <summary>
    /// The supported store types
    /// </summary>
    public enum StoreType
    {
        /// <summary>
        /// A store that handles everything as stack allocated structs
        /// </summary>
        RefStruct,

        /// <summary>
        /// The sharded store
        /// </summary>
        Sharded,
        
        /// <summary>
        /// The basic concurrent dictionary
        /// </summary>
        Concurrent,
        
        /// <summary>
        /// The lock free store
        /// </summary>
        LockFree,
    }
    
    /// <summary>
    ///     Defines the type of store to use.
    /// </summary>
    /// <remarks>
    ///     For the time being this is being used to help benchmark and do regression testing.
    ///     Long term it may be beneficial
    /// </remarks>
    public StoreType Store { get; set; } = StoreType.RefStruct;

    /// <summary>
    ///     The total capacity of the cache store.
    ///     A micro optimization useful when the caller knows roughly how much stuff them plan to cache.
    /// </summary>
    public int Capacity { get; set; } = int.MinValue;

    /// <summary>
    ///     The number of CPUs to use.
    ///     This will default to the Executing machines processor count.
    /// </summary>
    public int Cores { get; set; } = Environment.ProcessorCount;

    /// <summary>
    ///     Options to define how and when the cache is cleaned up.
    /// </summary>
    public CacheCleanupOptions CacheCleanupOptions { get; set; } = new();
}

/// <summary>
///     Options to define how and when the cache is cleaned up.
/// </summary>
public sealed class CacheCleanupOptions
{
    internal const long Hour = TimeSpan.TicksPerMinute * 60;
    internal const long MinRandom = TimeSpan.TicksPerMinute * 20;
    internal const long MaxRandom = TimeSpan.TicksPerHour * 3;
    
    /// <summary>
    /// Selects a strategy for the asynchronous cache eviction polling routine.
    /// </summary>
    public enum AsyncStrategy
    {
        /// <summary>
        ///     Disables the background service entirely.
        /// </summary>
        Disabled,
        /// <summary>
        ///     Runs once per hour from the time the application starts.
        /// </summary>
        Hourly,
        /// <summary>
        ///     Runs with randomness.  This is the default mode.
        /// </summary>
        /// <remarks>
        ///     A random delay time will be selected between 20 minutes and 3 hours.
        /// </remarks>
        Random,
        /// <summary>
        ///     Runs as frequently as possible provided there is not any lock contention.
        /// </summary>
        Aggressive,
        /// <summary>
        ///     Only runs when manually triggered.
        /// </summary>
        Manual
    }

    /// <summary>
    ///     Selects a strategy for the asynchronous cache eviction polling routine.
    /// </summary>
    /// <remarks>
    ///     Defaults to a randomness strategy
    /// </remarks>
    public AsyncStrategy Strategy { get; set; } = AsyncStrategy.Random;
}