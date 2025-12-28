using Aspire.Hosting.ApplicationModel;

namespace Scribbly.Cubby;

/// <summary>
/// Aspire resource used run cubby as an AOT native application
/// </summary>
public class CubbyExecutableResource : ExecutableResource
{
    /// <inheritdoc />
    internal CubbyExecutableResource([ResourceName] string name, string command, string workingDirectory) 
        : base(name, command, workingDirectory)
    {
    }
}