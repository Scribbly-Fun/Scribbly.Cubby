using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Tests.Structure.LayoutExtension_Tests;

public class GetHeader_ByteArray_Extensions_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(23)]
    public void Given_ArrayShorterThan_HeaderRequirements_Should_Throw_ArgOutOfRange(int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);

        var act = () =>
        {
            array.GetHeader();
        };

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(24)]
    [InlineData(1000)]
    [InlineData(24_000)]
    public void Given_ArrayGreaterOrEqualTo_HeaderRequirements_Should_Not_Throw_ArgOutOfRange(int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        var act = () =>
        {
            var entry = array.RentCacheEntryArray(CacheEntryOptions.None);
            try
            {
                entry.GetHeader();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(entry);
            }
        };
        
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(24)]
    [InlineData(1000)]
    [InlineData(24_000)]
    public void Given_ArrayGreaterOrEqualTo_HeaderRequirements_Should_Return_24Bytes(int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
                
        var entry = array.RentCacheEntryArray(CacheEntryOptions.None);
        var header = entry.GetHeader();
        header.Length.Should().Be(24);
    }

    [Theory]
    [InlineData(24)]
    [InlineData(1000)]
    [InlineData(24_000)]
    public void Given_ArrayGreaterOrEqualTo_HeaderRequirements_Should_Return_First24Bytes(int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        var entry = array.RentCacheEntryArray(CacheEntryOptions.None);
        var slice = entry.AsSpan()[..24];
        
        var header = entry.GetHeader();
        header.ToArray().Should().BeEquivalentTo(slice.ToArray());
    }
}
