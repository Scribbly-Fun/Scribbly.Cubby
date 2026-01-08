using FluentAssertions;

namespace Scribbly.Cubby.UnitTests.Tests.BytesKey_Tests.Index;

public class BytesKey_IndexingTests
{
    [Theory]
    [InlineData(0x02)]
    [InlineData(0xFF)]
    [InlineData(0xCC)]
    [InlineData(0xAA)]
    public void Given_Index_Zero_Byte_ShouldBe_FirstByte(byte b)
    {
        var key = new BytesKey([ b, 0x06, 0x0F, 0xFF ]);

        key[0].Should().Be(b);
    }

    [Theory]
    [InlineData(0x02)]
    [InlineData(0xFF)]
    [InlineData(0xCC)]
    [InlineData(0xAA)]
    public void Given_Max_Index_Byte_ShouldBe_LastByte(byte b)
    {
        var key = new BytesKey([ 0x00, 0x06, 0x0F, b ]);

        key[3].Should().Be(b);
    }

    [Fact]
    public void Given_Index_Below_Zero_ShouldThrow_IndexOutOfRange()
    {
        var key = new BytesKey([ 0x00, 0x06, 0x0F]);

        var act = () =>
        {
            var b = key[-1];
        };

        act.Should().ThrowExactly<IndexOutOfRangeException>();
    }

    [Fact]
    public void Given_Index_Above_Length_ShouldThrow_IndexOutOfRange()
    {
        var key = new BytesKey([ 0x00, 0x06, 0x0F, 0x88]);

        var act = () =>
        {
            var b = key[4];
        };

        act.Should().ThrowExactly<IndexOutOfRangeException>();
    }
}