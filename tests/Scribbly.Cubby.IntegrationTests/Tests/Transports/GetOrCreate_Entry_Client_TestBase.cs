using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports;

public abstract class GetOrCreate_Entry_Client_TestBase<T>(WebApplicationFactory<T> application) where T : class
{
    public record DataInput(int Value);
    public record StoredItem(string Key, int Value);
    
    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreate<DataInput, StoredItem>(key, new DataInput(length), (input) =>
        {
            return new StoredItem("Random Result", input.Value);
        });

        entry.Found.Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry_Value) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry_Value) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry_Value) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry_Value) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry_Value) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreate_WithInput_Should_CreateEntry_Value(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreate<DataInput, StoredItem>(key, new DataInput(length), (input) =>
        {
            return new StoredItem("Random Result", input.Value);
        });

        entry.Value.Should().Be(new StoredItem("Random Result", length));
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreate<StoredItem>(key, () =>
        {
            return new StoredItem(key, length);
        });

        entry.Found.Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreate_WithoutInput_Should_CreateEntry_Value(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreate<StoredItem>(key, () =>
        {
            return new StoredItem(key, length);
        });

        entry.Value.Should().Be(new StoredItem(key, length));
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreateAsync<DataInput, StoredItem>(key, new DataInput(length), async (input, ctx) =>
        {
            await Task.Delay(TimeSpan.FromTicks(1), ctx);
            
            return new StoredItem("Random Result", input.Value);
        });

        entry.Found.Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry_Value) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry_Value) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry_Value) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry_Value) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry_Value) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreateAsync_WithInput_Should_CreateEntry_Value(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreateAsync<DataInput, StoredItem>(key, new DataInput(length), async (input, ctx) =>
        {
            await Task.Delay(TimeSpan.FromTicks(1), ctx);

            return new StoredItem("Random Result", input.Value);
        });

        entry.Value.Should().Be(new StoredItem("Random Result", length));
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreateAsync<StoredItem>(key, async (ctx) =>
        {
            await Task.Delay(TimeSpan.FromTicks(1), ctx);

            return new StoredItem(key, length);
        });

        entry.Found.Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "1", 2000)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "2", 199_999)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "3", 2)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "4", 65_535)]
    [InlineData(nameof(Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry_Value) + nameof(T) + "5", 199_999)]
    public async Task Given_UnKnown_Key_GetOrCreateAsync_WithoutInput_Should_CreateEntry_Value(string key, int length)
    {
        using var scope = application.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ICubbyClient>();

        EntryResponse<StoredItem> entry = await client.GetOrCreateAsync<StoredItem>(key, async (ctx) =>
        {
            await Task.Delay(TimeSpan.FromTicks(1), ctx);

            return new StoredItem(key, length);
        });

        entry.Value.Should().Be(new StoredItem(key, length));
    }
}