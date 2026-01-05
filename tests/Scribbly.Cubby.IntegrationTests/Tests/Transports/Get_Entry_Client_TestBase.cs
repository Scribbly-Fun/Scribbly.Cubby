using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class Get_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    [Theory]
    [InlineData("SOME UNKNOWN KEY")]
    [InlineData("WHY 🎉🎉🎉🎉 NOT")]
    [InlineData("SOME OTHER UNKNOWN VALUE")]
    public async Task Given_Unknown_Key_Should_Return_EmptyArray(string key)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        var result = await client.Get(key, CancellationToken.None);

        result.Value.ToArray().Should().BeEmpty();
    }

    [Theory]
    [InlineData("key", 2000)]
    [InlineData("a fancy key", 199_999)]
    [InlineData("scribbles for scribbly", 2)]
    [InlineData("cache keys all day", 65_535)]
    [InlineData("Emoji Keys 👀👀👀", 199_999)]
    public async Task Given_Known_Key_Get_Should_Return_Value(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<ICubbyStore>();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        store.Put(key, array, CacheEntryOptions.None);
        
        var entry = await client.Get(key, CancellationToken.None);
        
        entry.Value.ToArray().Should().BeEquivalentTo(array);
    }
}