namespace Scribbly.Cubby;

internal static class CubbyContainerImageTags
{
    /// <remarks>docker.io</remarks>
    public const string Registry = "docker.io";

    /// <remarks>scribbly/cubby</remarks>
    public const string CubbyImage = "scribbly/cubby";

    /// <remarks>latest</remarks>
    public const string CubbyTag = CubbyResources.CubbyContainerImageTag;
    
    /// <remarks>scribbly/cubby-portal</remarks>
    public const string PortalImage = "scribbly/cubby-portal";
    
    /// <remarks>scribbly/cubby</remarks>
    public const string PortalTag = CubbyResources.PortalContainerImageTag;

}
