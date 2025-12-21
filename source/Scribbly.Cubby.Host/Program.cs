using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Stores;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.Services.ConfigureHttpJsonOptions(options =>
{
  // TODO: add all JSON context for admin APIs
});

builder.Services.AddOpenApi();
builder.AddCubbyServer(ops =>
{
    ops.Store = Store.Sharded;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCubby();

app.Run();