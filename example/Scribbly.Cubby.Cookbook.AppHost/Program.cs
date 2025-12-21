var builder = DistributedApplication.CreateBuilder(args);

var cacheService = builder.AddProject<Projects.Scribbly_Cubby_Host>("scrb-cubby")
    .WithEnvironment("SCRB_CUBBY_HTTPS", "True");

var cookbook = builder.AddProject<Projects.Scribbly_Cubby_Cookbook_ApiService>("scrb-cookbook")
    .WithReference(cacheService)
    .WaitFor(cacheService);

builder.Build().Run();
