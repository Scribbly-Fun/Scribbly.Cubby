using System.Text;
using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Tests.BytesKey_Tests.Equality;

public class BytesKey_ToStringTests
{
    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_Array_ToString_ShouldEqual_Value(string value)
    {
        var key = new BytesKey(Encoding.UTF8.GetBytes(value));

        key.ToString().Should().Be(value);
    }
}

public class BytesKey_ImplicateStringTests
{
    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_Array_Conversion_ToString_ShouldEqual_Value(string value)
    {
        string key = new BytesKey(Encoding.UTF8.GetBytes(value));

        key.Should().Be(value);
    }

    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_Array_Conversion_ToBytes_ShouldEqual_Value(string value)
    {
        BytesKey key = value;

        key.ToString().Should().Be(value);
    }
}

public class BytesKey_ImplicateArrayTests
{
    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_ByteArray_Conversion_ToKey_ShouldEqual_Value(string value)
    {
        BytesKey key = Encoding.UTF8.GetBytes(value);

        key.ToString().Should().Be(value);
    }

    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_SpanArray_Conversion_ToBytes_ShouldEqual_Value(string value)
    {
        BytesKey key = Encoding.UTF8.GetBytes(value).AsSpan();

        key.ToString().Should().Be(value);
    }

    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_Key_Conversion_ToByteArray_ShouldEqual_Value(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        byte[] key = new BytesKey(bytes);

        key.Should().BeEquivalentTo(bytes);
    }

    [Theory]
    [InlineData("This is my Key")]
    [InlineData("With 🎯🎯🎯")]
    [InlineData("I can do it")]
    public void Given_Key_Conversion_ToSpan_ShouldEqual_Value(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        ReadOnlySpan<byte> key = new BytesKey(bytes);

        key.ToArray().Should().BeEquivalentTo(bytes);
    }
}