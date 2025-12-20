using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scribbly.Cubby.Builder;
using Scribbly.Cubby.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{

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