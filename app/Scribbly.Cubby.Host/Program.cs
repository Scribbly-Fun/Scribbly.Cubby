using Microsoft.Extensions.Options;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Host.Setup;
using Scribbly.Cubby.Server;
using Scribbly.Cubby.Stores;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddLogging();

var useHttps = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(CubbyEnvironment.UseHttpsEnv));

if (useHttps)
{
    builder.WebHost.UseKestrelHttpsConfiguration();
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
  // TODO: add all JSON context for admin APIs;
});

builder.Services.AddOpenApi();

builder
    .AddCubbyServer(ops =>
    {
        ops.Store = CubbyServerOptions.StoreType.Sharded;
        ops.Capacity = 2000;
        ops.Cores = Environment.ProcessorCount;

        ops.Cleanup.Strategy = CacheCleanupOptions.AsyncStrategy.Random;
    })
    .WithCubbyGrpcServer()
    .WithCubbyHttpServer();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (logger.IsEnabled(LogLevel.Information))
{
    var options = app.Services.GetRequiredService<IOptions<CubbyServerOptions>>().Value;
    app.Services.GetRequiredService<ILogger<Program>>().LogApplicationStartup(
        options.Store,
        options.Transports,
        options.Cores,
        options.Capacity);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCubbyGrpc();
app.MapCubbyHttp();

app.Run();