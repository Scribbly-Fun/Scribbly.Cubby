using Scribbly.Aspire;
using Scribbly.Aspire.K6;
using Scribbly.Cubby;

var builder = DistributedApplication.CreateBuilder(args);

// Loads cubby as a direct project reference
var cubbyAot = builder.AddProject<Projects.Scribbly_Cubby_Host>("cubby-project")
    .WithEnvironment("SCRB_CUBBY_HTTPS", "True");

// Builds and loads the cubby docker images 
var cubbyDockerFile = builder
    .AddDockerfile("cubby-dockerfile", "../../", "Dockerfile");

var cubbyAdminPortal = builder
    .AddDockerfile("cubby-portal-dockerfile", "../../portal", "Dockerfile")
    .WithEndpoint(3002, 3000, "http")
    .WithReference(cubbyAot);

// Loads cubby's published docker images using the Aspire resource as a project reference
var cubbyContainer = builder
    .AddCubbyContainer("cubby-published")
    .WithCubbyPortal("cubby-portal-published");

// Starts a client side caching consumer as a project reference
var cookbook = builder.AddProject<Projects.Scribbly_Cubby_Cookbook_ApiService>("scrb-cookbook")
    .WithReference(cubbyAot)
    .WaitFor(cubbyAot)
    .WithReference(cubbyContainer)
    .WaitFor(cubbyContainer);

var portal = builder.AddJavaScriptApp("portal-local", "../../portal")
    .WithPnpm()
    .WithExternalHttpEndpoints()
    .WithReference(cubbyAot)
    .WithHttpEndpoint(name: "ui", port: 5173, targetPort: 5173, isProxied: false)
    .WithEnvironment(context =>
    {
        var url = cubbyAot.Resource.GetEndpoint("http").Url;
        context.EnvironmentVariables.Add("CUBBY_HOST_URL", url);
    });

if (builder.ExecutionContext.IsRunMode)
{
    builder.AddLoadTesting("load-tester", "./scripts", options =>
        {
            options
                .WithBuiltInDashboard()
                .WithOtlpEnvironment();
        })
        .WithDefaultApiResourceForScripts(cubbyContainer)
        .WithApiResourceForScript("docker-tx-http-put-1000b", cubbyContainer)
        .WithApiResourceForScript("docker-tx-grpc-put-1000b", cubbyContainer)
        .WithApiResourceForScript("native-tx-http-put-1000b", cubbyAot)
        .WithApiResourceForScript("native-tx-grpc-put-1000b", cubbyAot);
}

builder.Build().Run();
