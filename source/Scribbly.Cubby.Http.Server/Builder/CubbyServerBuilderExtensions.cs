using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Builder;

/// <summary>
/// Extensions to help configure the application setup
/// </summary>
public static class CubbyServerBuilderExtensions
{
    /// <summary>
    /// Extends the host builder
    /// </summary>
    /// <param name="cubbyBuilder">The Host Builder</param>
    extension(ICubbyServerBuilder cubbyBuilder)
    {
        /// <summary>
        /// Adds cubby services and configures the store.
        /// </summary>
        /// <returns>The configured host.</returns>
        public ICubbyServerBuilder WithCubbyHttpServer()
        {
            cubbyBuilder.ServerOptions.InternalTransports |= CubbyServerOptions.EnabledTransports.Http;
            return cubbyBuilder;
        }
    }
}