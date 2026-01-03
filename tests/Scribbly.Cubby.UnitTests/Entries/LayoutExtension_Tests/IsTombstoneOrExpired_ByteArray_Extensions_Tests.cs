using System.Buffers;
using System.Buffers.Binary;
using FluentAssertions;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.UnitTests.Entries.LayoutExtension_Tests;

public class IsTombstoneOrExpired_ByteArray_Extensions_Tests
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
            array.IsTombstoneOrExpired(DateTimeOffset.UtcNow.Ticks);
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
            var entry = array.LayoutEntry(CacheEntryOptions.None);
            try
            {
                entry.IsTombstoneOrExpired(DateTimeOffset.UtcNow.Ticks);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(entry);
            }
        };
        
        act.Should().NotThrow();
        
    }

    [Fact]
    public void Given_TombstoneFlag_IsTombstoneOrExpired_Should_Return_True()
    {
        byte[] array = new byte[80];
        Random.Shared.NextBytes(array);
        
        var entry = array.LayoutEntry(CacheEntryOptions.Never(CacheEntryFlags.Tombstone));

        entry.IsTombstoneOrExpired(DateTimeOffset.UtcNow.Ticks).Should().BeTrue();
        ArrayPool<byte>.Shared.Return(entry);
    }

    [Fact]
    public void Given_NoExpiration_IsTombstoneOrExpired_Should_Return_False()
    {
        byte[] array = new byte[80];
        Random.Shared.NextBytes(array);

        var entry = array.LayoutEntry(CacheEntryOptions.Never());
        
        entry.IsTombstoneOrExpired(DateTimeOffset.UtcNow.Ticks).Should().BeFalse();
        ArrayPool<byte>.Shared.Return(entry);
    }

    [Fact]
    public void Given_ExpirationInTheFuture_IsTombstoneOrExpired_Should_Return_False()
    {
        byte[] array = new byte[80];
        Random.Shared.NextBytes(array);

        var now = DateTimeOffset.UtcNow;
        var entry = array.LayoutEntry(CacheEntryOptions.Absolute(now, now.AddMicroseconds(1)));
        
        entry.IsTombstoneOrExpired(now.Ticks).Should().BeFalse();
        ArrayPool<byte>.Shared.Return(entry);
    }

    [Fact]
    public void Given_ExpirationEqualToNow_IsTombstoneOrExpired_Should_Return_True()
    {
        byte[] array = new byte[80];
        Random.Shared.NextBytes(array);

        var now = DateTimeOffset.UtcNow;
        var entry = array.LayoutEntry(CacheEntryOptions.Absolute(now, now));
        
        entry.IsTombstoneOrExpired(now.Ticks).Should().BeTrue();
        ArrayPool<byte>.Shared.Return(entry);
    }

    /// <remarks>Here we need to manually insert a past expiration so we dont need to wait to wait.</remarks>
    [Fact]
    public void Given_ExpirationInThePast_IsTombstoneOrExpired_Should_Return_True()
    {
        byte[] array = new byte[80];
        Random.Shared.NextBytes(array);

        var now = DateTimeOffset.UtcNow;
        var expire = now.AddMicroseconds(-1);
        
        var entry = array.LayoutEntry(CacheEntryOptions.None);

        var span = entry.AsSpan(0, 8);
        
        BinaryPrimitives.WriteInt64LittleEndian(span, expire.Ticks);
        
        entry.IsTombstoneOrExpired(now.Ticks).Should().BeTrue();
        ArrayPool<byte>.Shared.Return(entry);
    }
}
