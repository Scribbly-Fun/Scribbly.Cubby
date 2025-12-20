using System.Diagnostics.CodeAnalysis;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Builder;

/// <summary>
/// Application setup configuration options.
/// </summary>
public class CubbyOptions
{
    /// <summary>
    ///     Defines the type of store to use.
    /// </summary>
    /// <remarks>
    ///     For the time being this is being used to help benchmark and do regression testing.
    ///     Long term it may be benificial
    /// </remarks>
    public Stores.Store Store { get; set; } = Stores.Store.Sharded;
}