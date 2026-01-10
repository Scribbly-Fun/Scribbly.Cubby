using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class Evict_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    [Theory]
    [InlineData(nameof(Given_UnknownKey_Evict_Should_Return_Unknown) + "key 1" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Evict_Should_Return_Unknown) + "key 2" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Evict_Should_Return_Unknown) + "key 3" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Evict_Should_Return_Unknown) + "key 4" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Evict_Should_Return_Unknown) + "key 5" + nameof(T))]
    public async Task Given_UnknownKey_Evict_Should_Return_Unknown(string key)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        var result = await client.Evict(key, CancellationToken.None);

        result.Should().Be(EvictResult.Unknown);
    }

    [Theory]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Return_Removed) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Return_Removed) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Return_Removed) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Return_Removed) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Return_Removed) + "key 5" + nameof(T), 199_999)]
    public async Task Given_ExistingKey_Evict_Should_Return_Removed(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        _ = await client.Put(key, array, CacheEntryOptions.None, CancellationToken.None);
        var result = await client.Evict(key, CancellationToken.None);

        result.Should().Be(EvictResult.Removed);
    }

    [Theory]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Remove_Entry) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Remove_Entry) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Remove_Entry) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Remove_Entry) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_ExistingKey_Evict_Should_Remove_Entry) + "key 5" + nameof(T), 199_999)]
    public async Task Given_ExistingKey_Evict_Should_Remove_Entry(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
       
        _ = await client.Put(key, array, CacheEntryOptions.None, CancellationToken.None);
        _ = await client.Evict(key, CancellationToken.None);

        var value = await client.Get(key, CancellationToken.None);
        
        value.Found.Should().BeFalse();
    }
}