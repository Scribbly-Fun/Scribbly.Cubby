using Microsoft.Extensions.Options;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Host.Portal;
using Scribbly.Cubby.Host.Portal.Metrics;
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
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CubbyOptionsJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CacheResponseJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CleanupMetricsJsonContext.Default);
});

builder.Services.AddOpenApi();

builder
    .AddCubbyServer(ops =>
    {
        ops.Store = CubbyServerOptions.StoreType.Sharded;
        ops.Capacity = 2000;
        ops.Cores = Environment.ProcessorCount;

        // Duration strategy keeps portal cleanup charts continuously sampling.
        ops.Cleanup.Strategy = CacheCleanupOptions.AsyncStrategy.Duration;
        ops.Cleanup.Delay = TimeSpan.FromMinutes(1);
    })
    .WithCubbyGrpcServer()
    .WithCubbyHttpServer();

builder.Services.AddCubbyCleanupMetrics();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (logger.IsEnabled(LogLevel.Information))
{
    var options = app.Services.GetRequiredService<CubbyServerOptions>();
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

app.MapCubbyPortal();

app.Run();