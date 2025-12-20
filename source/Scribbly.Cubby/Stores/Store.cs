namespace Scribbly.Cubby.Stores;

/// <summary>
/// The supported store types
/// </summary>
public enum Store
{
    /// <summary>
    /// The lock free store
    /// </summary>
    LockFree,
    /// <summary>
    /// The sharded store
    /// </summary>
    Sharded,
    /// <summary>
    /// The basic concurrent dictionary
    /// </summary>
    Concurrent
}