using FluentAssertions;
using Scribbly.Cubby.Client;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Factory_Response_Tests;

public class Factory_Response_Deconstruct_Tests
{
    record Value(int V);
    
    [Fact]
    public void Given_Options_And_Value_Deconstruct_Should_SetOptions()
    {
        var (o, _) = FactoryResponse<Value>.Create(CacheEntryOptions.None, new Value(1));
        
        o.Should().Be(CacheEntryOptions.None);
    }

    [Fact]
    public void Given_Options_And_Value_Deconstruct_Should_SetValue()
    {
        var (_, v) = FactoryResponse<Value>.Create(CacheEntryOptions.None, new Value(1));
        
        v.Should().Be(new Value(1));
    }
}