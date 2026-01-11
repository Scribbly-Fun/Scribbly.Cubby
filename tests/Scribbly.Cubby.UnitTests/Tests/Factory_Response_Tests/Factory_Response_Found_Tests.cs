using FluentAssertions;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Factory_Response_Tests;

public class Factory_Response_Found_Tests
{
    record Value(int V);
    
    [Fact]
    public void Given_Null_Value_Found_Should_BeFalse()
    {
        FactoryResponse<Value> response = new FactoryResponse<Value>
        {
            Options = CacheEntryOptions.None,
            Value = null
        };

        response.Found.Should().BeFalse();
    }

    [Fact]
    public void Given_Value_Found_Should_BeTrue()
    {
        FactoryResponse<Value> response = new FactoryResponse<Value>
        {
            Options = CacheEntryOptions.None,
            Value = new Value(1)
        };

        response.Found.Should().BeTrue();
    }

    [Fact]
    public void Given_Null_Value_Implicate_Found_Should_BeFalse()
    {
        bool found  = new FactoryResponse<Value>
        {
            Options = CacheEntryOptions.None,
            Value = null
        };

        found.Should().BeFalse();
    }

    [Fact]
    public void Given_Value_Found_Implicate_Should_BeTrue()
    {
        bool found = new FactoryResponse<Value>
        {
            Options = CacheEntryOptions.None,
            Value = new Value(1)
        };

        found.Should().BeTrue();
    }
}