using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class Put_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    [Theory]
    [InlineData("new key 1", 2000)]
    [InlineData("new key 2", 199_999)]
    [InlineData("new key 3", 2)]
    [InlineData("new key 4", 65_535)]
    [InlineData("new key 5", 199_999)]
    public async Task Given_NewKey_Put_Should_Return_Created(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        var result = await client.Put(key, array, new CacheEntryOptions(), CancellationToken.None);

        result.Should().Be(PutResult.Created);
    }

    [Theory]
    [InlineData("new key 1", 2000)]
    [InlineData("new key 2", 199_999)]
    [InlineData("new key 3", 2)]
    [InlineData("new key 4", 65_535)]
    [InlineData("new key 5", 199_999)]
    public async Task Given_NewKey_Put_Should_Create_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        var store = scope.ServiceProvider.GetRequiredService<ICubbyStore>();

        await client.Put(key, array, new CacheEntryOptions(), CancellationToken.None);
        
        store.Get(key).ToArray().Should().BeEquivalentTo(array);
    }

    [Theory]
    [InlineData("key", 2000)]
    [InlineData("key", 199_999)]
    [InlineData("key", 2)]
    [InlineData("key", 65_535)]
    [InlineData("key", 7_994)]
    public async Task Given_ExistingKey_Put_Should_Return_Updated(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        var result = await client.Put(key, array, new CacheEntryOptions(), CancellationToken.None);

        result.Should().Be(PutResult.Updated);
    }

    [Theory]
    [InlineData("key", 2000)]
    [InlineData("key", 199_999)]
    [InlineData("key", 2)]
    [InlineData("key", 65_535)]
    [InlineData("key", 7_994)]
    public async Task Given_NewKey_Put_Should_Update_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        var store = scope.ServiceProvider.GetRequiredService<ICubbyStore>();
        
        await client.Put(key, array, new CacheEntryOptions(), CancellationToken.None);

        store.Get(key).ToArray().Should().BeEquivalentTo(array);
    }
}