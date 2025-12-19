using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Scribbly.Cubby.UnitTests")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.IntegrationTests")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.Cookbook.Tests")]

// ReSharper disable once CheckNamespace
namespace Scribbly.Cubby.Contract;

/// <summary>
/// Marker to locate this assembly using reflection.
/// </summary>
public interface IAssemblyMarker;