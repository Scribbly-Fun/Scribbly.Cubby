using FluentAssertions;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Factory_Response_Tests;

public class Factory_Response_Create_Tests
{
    record Value(int V);
    
    [Fact]
    public void Given_Default_Constructor_Should_SetOptions_None()
    {
        FactoryResponse<string> response = default;
        response.Options.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Default_Constructor_Should_SetValue_Null()
    {
        FactoryResponse<string> response = default;
        response.Value.Should().BeNull();
    }
    
    [Fact]
    public void Given_Options_And_Value_Constructor_Should_SetOptions()
    {
        FactoryResponse<Value> response = new FactoryResponse<Value>
        {
            Options = CacheEntryOptions.None,
            Value = new Value(1)
        };

        response.Options.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Options_And_Value_Constructor_Should_SetValue()
    {
        FactoryResponse<Value> response = new FactoryResponse<Value>
        {
            Options = CacheEntryOptions.None,
            Value = new Value(1)
        };

        response.Value.Should().Be(new Value(1));
    }

    [Fact]
    public void Given_Options_And_Value_Create_Should_SetOptions()
    {
        FactoryResponse<Value> response = FactoryResponse<Value>.Create(CacheEntryOptions.None, new Value(1));

        response.Options.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Options_And_Value_Create_Should_SetValue()
    {
        FactoryResponse<Value> response = FactoryResponse<Value>.Create(CacheEntryOptions.None, new Value(1));

        response.Value.Should().Be(new Value(1));
    }

    [Fact]
    public void Given_Value_Create_Should_DefaultOptions()
    {
        FactoryResponse<Value> response = FactoryResponse<Value>.Create(new Value(1));

        response.Options.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Value_Create_Should_SetValue()
    {
        FactoryResponse<Value> response = FactoryResponse<Value>.Create(new Value(1));

        response.Value.Should().Be(new Value(1));
    }

    [Fact]
    public void Given_Options_And_Value_Tuple_Should_SetOptions()
    {
        FactoryResponse<Value> response = (CacheEntryOptions.None, new Value(1));

        response.Options.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Options_And_Value_Tuple_Should_SetValue()
    {
        FactoryResponse<Value> response = (CacheEntryOptions.None, new Value(1));

        response.Value.Should().Be(new Value(1));
    }

    [Fact]
    public void Given_Value_Implicate_Should_DefaultOptions()
    {
        FactoryResponse<Value> response = (new Value(1));

        response.Options.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Value_Implicate_Should_SetValue()
    {
        FactoryResponse<Value> response = (new Value(1));

        response.Value.Should().Be(new Value(1));
    }

}