using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Scribbly.Cubby.Stores;

/// <summary>
/// Used for configuration bindings.
/// </summary>
[JsonSerializable(typeof(CubbyServerOptions))]
internal partial class CubbyOptionsJsonContext : JsonSerializerContext;

/// <summary>
/// Application setup configuration options.
/// </summary>
public class CubbyServerOptions
{
    /// <summary>
    /// The env and appsettings.json section name
    /// </summary>
    public const string SectionName = "Cubby";
    
    /// <summary>
    /// The supported store types
    /// </summary>
    public enum StoreType
    {
        /// <summary>
        /// A cubby store that uses a concurrent dictionary per CPU Core (or specified)
        /// </summary>
        Sharded,
        
        /// <summary>
        /// The basic concurrent dictionary
        /// </summary>
        Concurrent,
        
        /// <summary>
        /// A cubby store that utilized manually locks and collection marshal
        /// </summary>
        Marshalled,
        
        /// <summary>
        /// The lock free store
        /// </summary>
        [Experimental("SCRB003", Message = "Not yet implemented")]
        LockFree,
    }
    
    /// <summary>
    ///     Flags indicating what transports have been enabled.
    /// </summary>
    [Flags]
    internal enum EnabledTransports : byte
    {
        None =    0,
        Http =    1 << 0,
        Grpc =    1 << 1,
        [Experimental("SCRB004", Message = "Not yet implemented")]
        Tcp =     1 << 2,
        Custom =  1 << 3
    }

    /// <summary>
    ///     The enabled transports.
    /// </summary>
    internal EnabledTransports Transports { get; set; } = EnabledTransports.None;
    
    /// <summary>
    ///     Defines the type of store to use.
    /// </summary>
    /// <remarks>
    ///     For the time being this is being used to help benchmark and do regression testing.
    ///     Long term it may be beneficial
    /// </remarks>
    public StoreType Store { get; set; } = StoreType.Sharded;

    /// <summary>
    ///     The total capacity of the cache store.
    ///     A micro optimization useful when the caller knows roughly how much stuff them plan to cache.
    /// </summary>
    public int Capacity { get; set; } = 0;

    /// <summary>
    ///     The number of CPUs to use.
    ///     This will default to the Executing machines processor count.
    /// </summary>
    public int Cores { get; set; } = Environment.ProcessorCount;

    /// <summary>
    ///     Options to define how and when the cache is cleaned up.
    /// </summary>
    public CacheCleanupOptions Cleanup { get; set; } = new();
}

/// <summary>
///     Options to define how and when the cache is cleaned up.
/// </summary>
public sealed class CacheCleanupOptions
{
    internal const long Hour = TimeSpan.TicksPerMinute * 60;
    internal const long Aggressive = TimeSpan.TicksPerMillisecond * 250;
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
        ///     Runs at the specified duration
        /// </summary>
        Duration,
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

    /// <summary>
    ///     A static duration used with the <see cref="AsyncStrategy"/> Duration option
    /// </summary>
    /// <remarks>
    ///     This duration value will onl be honored when the strategy is set to Duration
    /// </remarks>
    public TimeSpan Delay { get; set; }
}