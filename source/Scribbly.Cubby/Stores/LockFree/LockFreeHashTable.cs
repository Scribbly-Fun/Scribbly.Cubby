using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Scribbly.Cubby.Stores.LockFree;

[Experimental("SCRB001", Message = "Not yet implemented")]
sealed class LockFreeHashTable
{
    internal readonly Entry[] Entries;
    internal readonly int Mask;

    public LockFreeHashTable(int capacityPowerOfTwo)
    {
        Entries = new Entry[capacityPowerOfTwo];
        Mask = capacityPowerOfTwo - 1;
    }
    
    public void Put(
        byte[] key,
        byte[] value,
        int hash,
        long expirationTicks)
    {
        int index = hash & Mask;

        for (int i = 0; i < Entries.Length; i++)
        {
            ref var entry = ref Entries[(index + i) & Mask];

            if (entry.State == 0)
            {
                if (Interlocked.CompareExchange(ref entry.State, 1, 0) == 0)
                {
                    entry.Hash = hash;
                    entry.Key = key;
                    entry.Value = value;
                    entry.ExpirationTicks = expirationTicks;
                    return;
                }
            }

            if (entry.State == 1 && entry.Hash == hash &&
                entry.Key.AsSpan().SequenceEqual(key))
            {
                entry.Value = value;
                entry.ExpirationTicks = expirationTicks;
                return;
            }
        }
    }
    
    public bool TryGet(
        ReadOnlySpan<byte> key,
        int hash,
        out byte[] value)
    {
        int index = hash & Mask;
        long now = DateTime.UtcNow.Ticks;

        for (int i = 0; i < Entries.Length; i++)
        {
            ref var entry = ref Entries[(index + i) & Mask];

            if (entry.State == 0)
                break;

            if (entry.State == 1 && entry.Hash == hash &&
                entry.ExpirationTicks >= now &&
                entry.Key.AsSpan().SequenceEqual(key))
            {
                value = entry.Value;
                return true;
            }
        }

        value = null!;
        return false;
    }
}

[Experimental("SCRB001", Message = "Not yet implemented")]
sealed class CacheEngine
{
    private readonly CacheShard[] _shards;

    public CacheEngine(int shardCount)
    {
        _shards = new CacheShard[shardCount];
        for (int i = 0; i < shardCount; i++)
            _shards[i] = new CacheShard();
    }

    public CacheShard GetShard(ReadOnlySpan<byte> key)
        => _shards[GetShard(key, _shards.Length)];
    
    public static int GetShard(ReadOnlySpan<byte> key, int shardCount)
    {
        unchecked
        {
            int hash = 17;
            foreach (var b in key)
                hash = hash * 31 + b;
            return (hash & int.MaxValue) % shardCount;
        }
    }
}

[Experimental("SCRB001", Message = "Not yet implemented")]
sealed class CacheShard
{
    private readonly LockFreeHashTable _table = new LockFreeHashTable(2);
    private int _clockHand;
    
    internal void EvictIfNeeded()
    {
        for (int i = 0; i < 8; i++) // bounded work
        {
            int idx = (_clockHand++) & _table.Mask;
            ref var entry = ref _table.Entries[idx];

            if (entry.State == 1 &&
                entry.ExpirationTicks < DateTime.UtcNow.Ticks)
            {
                entry.State = 2; // tombstone
                ReturnToPool(entry.Key);
                ReturnToPool(entry.Value);
            }
        }
    }
    
    static byte[] RentAndCopy(ReadOnlySpan<byte> src)
    {
        var buf = ArrayPool<byte>.Shared.Rent(src.Length);
        src.CopyTo(buf);
        return buf;
    }

    static void ReturnToPool(byte[] buffer)
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
