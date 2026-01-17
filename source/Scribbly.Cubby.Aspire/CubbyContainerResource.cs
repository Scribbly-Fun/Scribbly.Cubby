using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Scribbly.Cubby;

/// <summary>
/// Aspire resource used to run cubby as a docker container.
/// </summary>
public class CubbyContainerResource : ContainerResource, IResourceWithServiceDiscovery
{
    internal CubbyContainerResource([ResourceName] string name = "cubby") : base(name)
    {
        
    }
}

/// <summary>
/// Aspire resource used to run cubby as a docker container.
/// </summary>
public class CubbyPortalResource : ContainerResource, IResourceWithParent<CubbyContainerResource>
{
    /// <summary>
    /// The cubby server the portal application will communicate with 
    /// </summary>
    public CubbyContainerResource Parent { get; }
    
    internal CubbyPortalResource(CubbyContainerResource parent, [ResourceName] string name = "cubby-portal") : base(name)
    {
        Parent = parent;
    }
}