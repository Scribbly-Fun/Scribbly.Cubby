using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Client;

namespace Scribbly.Cubby.IntegrationTests.Setup;

public class GrpcApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    /// <summary>
    /// Gives a fixture an opportunity to configure the application before it gets built.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> for the application.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        try
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IHostedService>();

                services.AddCubbyClient(ops =>
                    {
                        ops.Host = new Uri("http://localhost");
                    })
                    .WithCubbyGrpcClient();

            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"FAILED STARTING HOSTED APPLICATION {e.Message}");
        }
    }
    
    /// <summary>
    /// Called immediately after the class has been created, before it is used.
    /// </summary>
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}