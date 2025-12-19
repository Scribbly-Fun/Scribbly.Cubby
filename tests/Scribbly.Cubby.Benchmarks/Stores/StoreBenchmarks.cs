using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Keys;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StoreBenchmarks
{
    [Params(1, 4, 8, 16)] public int Threads;

    [Params(100_000)] public int EntryCount;

    private byte[][] _keys = null!;
    private byte[][] _values = null!;

    private ConcurrentDictionary<ByteKey, byte[]> _concurrent = null!;
    
    private ShardedConcurrentCache _shardedConcurrent = null!;
    
    private LockFreeShardedCache _lockFree = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _keys = new byte[EntryCount][];
        _values = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _keys[i] = BitConverter.GetBytes(i);
            _values[i] = new byte[64];
        }

        _concurrent = new ConcurrentDictionary<ByteKey, byte[]>();
        _shardedConcurrent = new ShardedConcurrentCache(Environment.ProcessorCount);
        _lockFree = new LockFreeShardedCache(Environment.ProcessorCount);

        for (int i = 0; i < EntryCount; i++)
        {
            _concurrent[new ByteKey(_keys[i])] = _values[i];
            _shardedConcurrent.Put(_keys[i], _values[i]);
            _lockFree.Put(_keys[i], _values[i]);
        }
    }
    
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _concurrent.TryGetValue(new ByteKey(_keys[i]), out _);
        });
    }
    
    [Benchmark]
    public void ShardedConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _shardedConcurrent.TryGet(_keys[i], out _);
        });
    }
    
    [Benchmark]
    public void LockFreeHashTable_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _lockFree.TryGet(_keys[i], out _);
        });
    }
}

sealed class ShardedConcurrentCache
{
    private readonly ConcurrentDictionary<ByteKey, byte[]>[] _shards;

    public ShardedConcurrentCache(int shardCount)
    {
        _shards = new ConcurrentDictionary<ByteKey, byte[]>[shardCount];
        for (int i = 0; i < shardCount; i++)
            _shards[i] = new ConcurrentDictionary<ByteKey, byte[]>();
    }

    public void Put(byte[] key, byte[] value)
    {
        GetShard(key)[new ByteKey(key)] = value;
    }

    public bool TryGet(byte[] key, out byte[] value)
    {
        return GetShard(key).TryGetValue(new ByteKey(key), out value!);
    }

    private ConcurrentDictionary<ByteKey, byte[]> GetShard(byte[] key)
        => _shards[(key[0] & int.MaxValue) % _shards.Length];
}

sealed class LockFreeShardedCache
{
    private readonly LockFreeHashTable[] _shards;

    public LockFreeShardedCache(int shardCount)
    {
        _shards = new LockFreeHashTable[shardCount];
        for (int i = 0; i < shardCount; i++)
            _shards[i] = new LockFreeHashTable(1 << 18);
    }

    public void Put(byte[] key, byte[] value)
    {
        int shard = key[0] % _shards.Length;
        _shards[shard].Put(key, value, key.GetHashCode(), 0);
    }

    public bool TryGet(byte[] key, out byte[] value)
    {
        int shard = key[0] % _shards.Length;
        return _shards[shard].TryGet(key, key.GetHashCode(), out value);
    }
}
