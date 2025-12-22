using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Scribbly.Cubby;

/// <summary>
/// 
/// </summary>
public static class CubbyResourceBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    extension(IDistributedApplicationBuilder builder)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IResourceBuilder<CubbyResource> AddCubby([ResourceName] string name = "scrb-cubby")
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(name);
        
            var resource = new CubbyResource(name);
        
            var resourceBuilder = builder
                .AddResource(resource)
                .WithIconName("TopSpeed");
        
            return resourceBuilder;
        }
    }
}