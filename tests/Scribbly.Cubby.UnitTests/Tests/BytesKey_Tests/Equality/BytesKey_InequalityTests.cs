using System.Text;
using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Tests.BytesKey_Tests.Equality;

public class BytesKey_InequalityTests
{
    [Theory]
    [InlineData(0x00)]
    [InlineData(0x77)]
    [InlineData(0x2D)]
    [InlineData(0xAA)]
    [InlineData(0xFB)]
    public void Given_Byte_Matching_Byte_Keys_ShouldBeOperatorEqual(byte key)
    {
        var key1 = new BytesKey([ key ]);
        var key2 = new BytesKey([ key ]);

        (key1 != key2).Should().BeFalse();
    }

    [Theory]
    [InlineData(0x00, 0x21)]
    [InlineData(0x77, 0x33)]
    [InlineData(0x2D, 0x77)]
    [InlineData(0xAA, 0xFF)]
    [InlineData(0xFB, 0xFC)]
    public void Given_Byte_MisMatching_Byte_Keys_ShouldNotBeOperatorEqual(byte a, byte b)
    {
        var key1 = new BytesKey([ a ]);
        var key2 = new BytesKey([ b ]);

        (key1 != key2).Should().BeTrue();
    }

    [Theory]
    [InlineData("!3I")]
    [InlineData("ALPHABET")]
    [InlineData("Soup")]
    [InlineData("Cubby will be awesome")]
    public void Given_Bytes_Matching_Bytes_Keys_ShouldBeOperatorEqual(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var key1 = new BytesKey(bytes);
        var key2 = new BytesKey(bytes);

        (key1 != key2).Should().BeFalse();
    }

    [Theory]
    [InlineData("!3I", "Is not this")]
    [InlineData("ALPHABET", "Soup")]
    [InlineData("Soup", "Is Great")]
    [InlineData("Cubby will be awesome", "At least I hope")]
    public void Given_Bytes_MisMatching_Bytes_Keys_ShouldNotBeOperatorEqual(string a, string b)
    {
        var key1 = new BytesKey(Encoding.UTF8.GetBytes(a));
        var key2 = new BytesKey(Encoding.UTF8.GetBytes(b));

        (key1 != key2).Should().BeTrue();
    }
}