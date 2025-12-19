var builder = DistributedApplication.CreateBuilder(args);

var cacheService = builder.AddProject<Projects.Scribbly_Cubby_Hosted>("scrb-cubby");

var cookbook = builder.AddProject<Projects.Scribbly_Cubby_Cookbook_ApiService>("scrb-cookbook");

builder.Build().Run();
