using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder
    .AddCubbyClient(ops =>
    {
        var host = Environment.GetEnvironmentVariable("SCRB_CUBBY_HTTPS") ?? Environment.GetEnvironmentVariable("SCRB_CUBBY_HTTP");
        ops.Host = new Uri(host?? throw new InvalidOperationException());
        
        ops.Lifetime = ServiceLifetime.Singleton;
    })
    .WithCubbyGrpcClient();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapPost("/entry/{key}", (IDistributedCache cache, string key, Item item) =>
{
    var watch = Stopwatch.StartNew();
    var value = JsonSerializer.SerializeToUtf8Bytes(item);
    cache.Set(key, value);
    
    watch.Stop();

    return watch.Elapsed;
});

app.MapGet("/entry/{key}", (IDistributedCache cache, string key) =>
{
    var watch = Stopwatch.StartNew();
    var entry = cache.Get(key);
    
    watch.Stop();

    var item = JsonSerializer.Deserialize<Item>(entry);
    return new
    {
        time = watch.Elapsed,
        item = item,
    };
});

app.Run();

record Item(string Key, string Value);