using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Scribbly.Cubby;

/// <summary>
/// Extends the Aspire AppHost builder
/// </summary>
public static class CubbyResourceBuilderExtensions
{
    /// <summary>
    /// Extensions used to configure the Aspire AppHost
    /// </summary>
    /// <param name="builder">Aspire AppHost builder</param>
    extension(IDistributedApplicationBuilder builder)
    {
        /// <summary>
        /// Adds cubby as a containerized application.
        /// </summary>
        /// <param name="name">An optional name used for the resource.</param>
        /// <returns>A cubby container resource with service discovery</returns>
        public IResourceBuilder<CubbyContainerResource> AddCubbyContainer([ResourceName] string name = "cubby")
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(name);
        
            var resource = new CubbyContainerResource(name);
            
            var healthCheckKey = $"{name}_check";
        
            builder.Services.AddHealthChecks().AddCheck(healthCheckKey, token => HealthCheckResult.Healthy());
            
            var cubbyResource = builder
                .AddResource(resource)
                .WithIconName("money")
                .WithImage(CubbyContainerImageTags.CubbyImage, CubbyContainerImageTags.CubbyTag)
                .WithImageRegistry(CubbyContainerImageTags.Registry)
                .WithEndpoint(name: "http", targetPort: 5000, scheme: "http")
                .WithHealthCheck(healthCheckKey);
            
            return cubbyResource;
        }

        /// <summary>
        /// Adds cubby as an executable application.
        /// </summary>
        /// <param name="name">An optional name used for the resource.</param>
        /// <param name="workingDirectory">The working directory for the executable</param>
        /// <returns>A cubby container resource with service discovery</returns>
        public IResourceBuilder<CubbyExecutableResource> AddCubbyExecutable(
            [ResourceName] string name = "cubby", 
            string? workingDirectory = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(name);
        
            var resource = new CubbyExecutableResource(name, "cubby", workingDirectory ?? "/");
            
            var healthCheckKey = $"{name}_check";
        
            builder.Services.AddHealthChecks().AddCheck(healthCheckKey, token => HealthCheckResult.Healthy());
            
            var cubbyResource = builder
                .AddResource(resource)
                .WithIconName("money")
                .WithEndpoint(name: "http", targetPort: 5000, scheme: "http")
                .WithHealthCheck(healthCheckKey);
            
            return cubbyResource;
        }
    }

    extension(IResourceBuilder<CubbyContainerResource> cubbyResourceBuilder)
    {
        /// <summary>
        /// Adds the portal UI to the cubby cache server
        /// </summary>
        /// <param name="name">A resource name</param>
        /// <returns>The portal builder</returns>
        public IResourceBuilder<CubbyContainerResource> WithCubbyPortal(
            [ResourceName] string name = "cubby-portal")
        {
            ArgumentNullException.ThrowIfNull(cubbyResourceBuilder);
            ArgumentException.ThrowIfNullOrEmpty(name);
        
            var resource = new CubbyPortalResource(cubbyResourceBuilder.Resource, name);
            
            var healthCheckKey = $"{name}_check";
        
            cubbyResourceBuilder.ApplicationBuilder.Services.AddHealthChecks().AddCheck(healthCheckKey, token => HealthCheckResult.Healthy());
            
            var portalResource = cubbyResourceBuilder.ApplicationBuilder
                .AddResource(resource)
                .WithIconName("money")
                .WithImage(CubbyContainerImageTags.PortalImage, CubbyContainerImageTags.PortalTag)
                .WithImageRegistry(CubbyContainerImageTags.Registry)
                .WithEndpoint(name: "http", targetPort: 3000, scheme: "http")
                .WithHealthCheck(healthCheckKey)
                .WithReference(cubbyResourceBuilder);

            return cubbyResourceBuilder;
        }
    }
}