using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Scribbly.Cubby.UnitTests")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.IntegrationTests")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.Cookbook.Tests")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.Hosted")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.Benchmarks")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.MicrosoftHosting")]
[assembly: InternalsVisibleTo("Scribbly.Cubby.MicrosoftHosting")]

namespace Scribbly.Cubby;

/// <summary>
/// Marker to locate this assembly using reflection.
/// </summary>
public interface IAssemblyMarker;