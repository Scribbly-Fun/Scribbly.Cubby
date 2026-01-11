using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PolyType;
using Scribbly.Cubby;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Cookbook.ServiceDefaults;
using Scribbly.Cubby.MessagePack;
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
        
        ops.AddSystemTextSerializer(jsonOptions =>
        {
            jsonOptions.TypeInfoResolverChain.Insert(0, ItemJsonContext.Default);
        });
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

    var results = await cache.PutObject(key, item, CacheEntryOptions.None, token);
    
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

app.MapPost("cubby/http/{key}", async (IHttpCubbyClient cache, string key, [FromQuery] bool? compress, [FromBody] Item item, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();

    var results = await cache.PutObject(
        key, item, 
        compress is true 
            ? CacheEntryOptions.Never(CacheEntryFlags.Compressed) 
            : CacheEntryOptions.None, 
        token);
    
    watch.Stop();

    return new
    {
        time = watch.Elapsed,
        result = results
    };
});

app.MapPost("cubby/http/get-or-create-async/{key}", async (IHttpCubbyClient cache, string key, [FromBody] Item item, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();

    var results = await cache.GetOrCreateAsync<Item, Item>(key, item, async (input, ctx) =>
    {
        await Task.Delay(TimeSpan.FromMilliseconds(5), ctx);
        
        return (TimeSpan.FromSeconds(15), input with
        {
            Value = input.Value + "Value from the factory"
        });
    }, token);
    
    watch.Stop();

    return new
    {
        time = watch.Elapsed,
        result = results
    };
});

app.MapPost("cubby/http/get-or-create/{key}", async (IHttpCubbyClient cache, string key, [FromBody] Item item, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();

    var results = await cache.GetOrCreate<Item, Item>(key, item, input 
        => (TimeSpan.FromSeconds(15), input with
        {
            Value = input.Value + "Value from the factory"
        }),     
        token);
    
    watch.Stop();

    return new
    {
        time = watch.Elapsed,
        result = results
    };
});

app.MapGet("cubby/http/{key}", async (IHttpCubbyClient cache, string key, CancellationToken token) =>
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

app.MapDelete("cubby/http/{key}", async (IHttpCubbyClient cache, string key, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();
    var result = await cache.Evict(key, token);
    
    watch.Stop();
    
    return new
    {
        time = watch.Elapsed,
        result = result,
    };
});

app.MapPost("cubby/grpc/{key}", async (IGrpcCubbyClient cache, string key, [FromBody] Item item, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();

    var results = await cache.PutObject(key, item, CacheEntryOptions.None, token);
    
    watch.Stop();

    return new
    {
        time = watch.Elapsed,
        result = results
    };
});

app.MapGet("cubby/grpc/{key}", async (IGrpcCubbyClient cache, string key, CancellationToken token) =>
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

app.MapDelete("cubby/grpc/{key}", async (IGrpcCubbyClient cache, string key, CancellationToken token) =>
{
    var watch = Stopwatch.StartNew();
    var result = await cache.Evict(key, token);
    
    watch.Stop();
    
    return new
    {
        time = watch.Elapsed,
        result = result,
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

[GenerateShapeFor<Item>]
public partial class Witness;
