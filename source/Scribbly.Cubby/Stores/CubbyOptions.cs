namespace Scribbly.Cubby.Stores;

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
}