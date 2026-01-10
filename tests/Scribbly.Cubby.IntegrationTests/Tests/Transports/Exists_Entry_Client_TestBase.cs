using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class Exists_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    [Theory]
    [InlineData(nameof(Given_UnknownKey_Exists_Should_Return_False) + "key 1" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Exists_Should_Return_False) + "key 2" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Exists_Should_Return_False) + "key 3" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Exists_Should_Return_False) + "key 4" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Exists_Should_Return_False) + "key 5" + nameof(T))]
    public async Task Given_UnknownKey_Exists_Should_Return_False(string key)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        var result = await client.Exists(key, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(nameof(Given_ExistingKey_Exists_Should_Return_True) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_ExistingKey_Exists_Should_Return_True) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_ExistingKey_Exists_Should_Return_True) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_ExistingKey_Exists_Should_Return_True) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_ExistingKey_Exists_Should_Return_True) + "key 5" + nameof(T), 199_999)]
    public async Task Given_ExistingKey_Exists_Should_Return_True(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        _ = await client.Put(key, array, CacheEntryOptions.None, CancellationToken.None);
        var result = await client.Exists(key, CancellationToken.None);

        result.Should().BeTrue();
    }
}