using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class Put_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    [Theory]
    [InlineData(nameof(Given_NewKey_Put_Should_Return_Created) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_NewKey_Put_Should_Return_Created) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_NewKey_Put_Should_Return_Created) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_NewKey_Put_Should_Return_Created) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_NewKey_Put_Should_Return_Created) + "key 5" + nameof(T), 199_999)]
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
    [InlineData(nameof(Given_NewKey_Put_Should_Create_Entry) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_NewKey_Put_Should_Create_Entry) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_NewKey_Put_Should_Create_Entry) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_NewKey_Put_Should_Create_Entry) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_NewKey_Put_Should_Create_Entry) + "key 5" + nameof(T), 199_999)]
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
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 2000)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 199_999)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 2)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 65_535)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 7_994)]
    public async Task Given_ExistingKey_Put_Should_Return_Updated(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        await client.Put(key, array, new CacheEntryOptions(), CancellationToken.None);
        
        var result = await client.Put(key, array, new CacheEntryOptions(), CancellationToken.None);

        result.Should().Be(PutResult.Updated);
    }

    [Theory]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 2000)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 199_999)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 2)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 65_535)]
    [InlineData(nameof(Given_ExistingKey_Put_Should_Return_Updated) + "key" + nameof(T), 7_994)]
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