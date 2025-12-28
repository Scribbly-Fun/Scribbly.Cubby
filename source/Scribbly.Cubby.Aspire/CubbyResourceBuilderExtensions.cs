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
        public IResourceBuilder<CubbyContainerResource> AddCubbyContainer([ResourceName] string name = "scrb-cubby")
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(name);
        
            var resource = new CubbyContainerResource(name);
            
            var healthCheckKey = $"{name}_check";
        
            builder.Services.AddHealthChecks().AddCheck(healthCheckKey, token => HealthCheckResult.Healthy());
            
            var cubbyResource = builder
                .AddResource(resource)
                .WithIconName("money")
                .WithImage(CubbyContainerImageTags.Image, CubbyContainerImageTags.Tag)
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
            [ResourceName] string name = "scrb-cubby", 
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
}