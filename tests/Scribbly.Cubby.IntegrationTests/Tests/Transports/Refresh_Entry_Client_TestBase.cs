using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class Refresh_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    [Theory]
    [InlineData(nameof(Given_UnknownKey_Refresh_Should_Return_Unknown) + "key 1" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Refresh_Should_Return_Unknown) + "key 2" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Refresh_Should_Return_Unknown) + "key 3" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Refresh_Should_Return_Unknown) + "key 4" + nameof(T))]
    [InlineData(nameof(Given_UnknownKey_Refresh_Should_Return_Unknown) + "key 5" + nameof(T))]
    public async Task Given_UnknownKey_Refresh_Should_Return_Unknown(string key)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        var result = await client.Refresh(key, CancellationToken.None);

        result.Should().Be(RefreshResult.Undefined);
    }

    [Theory]
    [InlineData(nameof(Given_NonSlidingEntry_Refresh_Should_Return_NotSliding) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_NonSlidingEntry_Refresh_Should_Return_NotSliding) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_NonSlidingEntry_Refresh_Should_Return_NotSliding) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_NonSlidingEntry_Refresh_Should_Return_NotSliding) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_NonSlidingEntry_Refresh_Should_Return_NotSliding) + "key 5" + nameof(T), 199_999)]
    public async Task Given_NonSlidingEntry_Refresh_Should_Return_NotSliding(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
        
        _ = await client.Put(key, array, CacheEntryOptions.None, CancellationToken.None);
        var result = await client.Refresh(key, CancellationToken.None);

        result.Should().Be(RefreshResult.NotSlidingEntry);
    }

    [Theory]
    [InlineData(nameof(Given_SlidingEntry_Refresh_Should_Return_Updated) + "key 1" + nameof(T), 2000)]
    [InlineData(nameof(Given_SlidingEntry_Refresh_Should_Return_Updated) + "key 2" + nameof(T), 199_999)]
    [InlineData(nameof(Given_SlidingEntry_Refresh_Should_Return_Updated) + "key 3" + nameof(T), 2)]
    [InlineData(nameof(Given_SlidingEntry_Refresh_Should_Return_Updated) + "key 4" + nameof(T), 65_535)]
    [InlineData(nameof(Given_SlidingEntry_Refresh_Should_Return_Updated) + "key 5" + nameof(T), 199_999)]
    public async Task Given_SlidingEntry_Refresh_Should_Return_Updated(string key, int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();
    
        await client.Put(key, array, CacheEntryOptions.Sliding(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10)), CancellationToken.None);
        var result = await client.Refresh(key, CancellationToken.None);

        result.Should().Be(RefreshResult.Updated);
    }
}