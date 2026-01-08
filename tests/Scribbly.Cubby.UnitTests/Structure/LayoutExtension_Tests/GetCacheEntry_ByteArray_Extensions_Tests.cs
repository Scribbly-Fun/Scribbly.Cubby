using System.Buffers;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Structure.LayoutExtension_Tests;

public class GetCacheEntry_ByteArray_Extensions_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(23)]
    public void Given_ArrayShorterThan_HeaderRequirements_Should_Throw_ArgOutOfRange(int length)
    {
        byte[] buffer = new byte[length];
        Random.Shared.NextBytes(buffer);

        var act = () =>
        {
            buffer.GetEntryFromBuffer();
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
            var buffer = array.RentCacheEntryArray(CacheEntryOptions.None);
            try
            {
                buffer.GetEntryFromBuffer();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        };
        
        act.Should().NotThrow();
    }
    
    [Theory]
    [InlineData(24)]
    [InlineData(1000)]
    [InlineData(24_000)]
    public void Given_ArrayGreaterOrEqualTo_HeaderRequirements_Should_Return_AllRequiredData(int length)
    {
        byte[] array = new byte[length];
        Random.Shared.NextBytes(array);
        
        var buffer = array.RentCacheEntryArray(CacheEntryOptions.None);
        
        var entry = buffer.GetEntryFromBuffer();
        
        entry.Length.Should().Be(length + 24);
    }
}
