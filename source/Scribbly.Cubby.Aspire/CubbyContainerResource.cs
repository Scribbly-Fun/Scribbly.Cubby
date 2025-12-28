using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Scribbly.Cubby;

/// <summary>
/// Aspire resource used to run cubby as a docker container.
/// </summary>
public class CubbyContainerResource : ContainerResource, IResourceWithServiceDiscovery
{
    internal CubbyContainerResource(string name = "scrb-cubby") : base(name)
    {
        
    }
}