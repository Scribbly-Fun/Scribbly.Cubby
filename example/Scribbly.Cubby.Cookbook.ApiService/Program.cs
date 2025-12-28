using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Cookbook.ServiceDefaults;
using Scribbly.Cubby.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services
    .AddCubbyClient(ops =>
    {
        var host = Environment.GetEnvironmentVariable("SCRB_CUBBY_HTTPS") ?? Environment.GetEnvironmentVariable("SCRB_CUBBY_HTTP");
        ops.Host = new Uri(host?? throw new InvalidOperationException());
        
        ops.Lifetime = ServiceLifetime.Singleton;
    })
    .WithCubbyHttpClient()
    .WithCubbyGrpcClient();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapPost("/entry/{key}", (IDistributedCache cache, string key, [FromBody] Item item) =>
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

app.MapPost("cubby/client/{key}", async (ICubbyClient cache, string key, [FromBody] Item item, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();

    var results = await cache.PutObject(key, item, new CacheEntryOptions(), token);
    
    watch.Stop();

    return new
    {
        time = watch.Elapsed,
        result = results
    };
});

app.MapGet("cubby/client/{key}", async (ICubbyClient cache, string key, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();
    var item = await cache.GetObject<Item>(key, token);
    
    watch.Stop();
    
    return new
    {
        time = watch.Elapsed,
        item = item,
    };
});

try
{

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

[JsonSerializable(typeof(Item))]
partial class ItemJsonContext : JsonSerializerContext;
record Item(string Key, string Value);