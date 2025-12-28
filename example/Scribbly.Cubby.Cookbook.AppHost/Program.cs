using Scribbly.Cubby;

var builder = DistributedApplication.CreateBuilder(args);

var cacheService = builder.AddProject<Projects.Scribbly_Cubby_Host>("scrb-cubby")
    .WithEnvironment("SCRB_CUBBY_HTTPS", "True");

var cubbyContainer = builder
    .AddCubbyContainer("scrb-cubby-container");

var cookbook = builder.AddProject<Projects.Scribbly_Cubby_Cookbook_ApiService>("scrb-cookbook")
    .WithReference(cacheService)
    .WaitFor(cacheService)
    .WithReference(cubbyContainer)
    .WaitFor(cubbyContainer);

builder.Build().Run();
