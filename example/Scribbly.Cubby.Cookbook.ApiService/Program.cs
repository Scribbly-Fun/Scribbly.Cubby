using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Scribbly.Cubby.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.AddCubbyClient(ops =>
{
    var host = Environment.GetEnvironmentVariable("SCRB_CUBBY_HTTPS") ?? Environment.GetEnvironmentVariable("SCRB_CUBBY_HTTP");
    ops.Host = new Uri(host?? throw new InvalidOperationException());
    
    ops.Lifetime = ServiceLifetime.Singleton;
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapPost("/entry/{key}", (IDistributedCache cache, string key, Item item) =>
{
    var value = JsonSerializer.SerializeToUtf8Bytes(item);
    
    var test = JsonSerializer.Deserialize<Item>(value);
    
    cache.Set(key, value);
});

app.MapGet("/entry/{key}", (IDistributedCache cache, string key) =>
{
    var entry = cache.Get(key);

    return JsonSerializer.Deserialize<Item>(entry);
});

app.Run();

record Item(string Key, string Value);