using Scribbly.Aspire;
using Scribbly.Aspire.K6;
using Scribbly.Cubby;

var builder = DistributedApplication.CreateBuilder(args);

var cubbyAot = builder.AddProject<Projects.Scribbly_Cubby_Host>("scrb-cubby")
    .WithEnvironment("SCRB_CUBBY_HTTPS", "True");

var cubbyDockerFile = builder
    .AddDockerfile("scrb-cubby-dockerfile", "../../", "Dockerfile");

var cubbyContainer = builder
    .AddCubbyContainer("scrb-cubby-container");

var cookbook = builder.AddProject<Projects.Scribbly_Cubby_Cookbook_ApiService>("scrb-cookbook")
    .WithReference(cubbyAot)
    .WaitFor(cubbyAot)
    .WithReference(cubbyContainer)
    .WaitFor(cubbyContainer);

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
