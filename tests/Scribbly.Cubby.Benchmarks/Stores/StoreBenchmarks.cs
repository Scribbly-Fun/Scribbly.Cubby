using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.LockFree;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StoreBenchmarks
{
    [Params(1, 4, 8, 16)] public int Threads;
    [Params(100_000)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private ConcurrentDictionary<BytesKey, PooledCacheEntry> _concurrent = null!;
    
    private ShardedConcurrentStore _shardedConcurrent = null!;
    private ShardedConcurrentStoreNoPool _shardedNoPoolConcurrent = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _keys = new BytesKey[EntryCount];
        _values = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _keys[i] = new BytesKey(BitConverter.GetBytes(i));
            _values[i] = new byte[64];
        }

        _concurrent = new ConcurrentDictionary<BytesKey, PooledCacheEntry>();
        _shardedConcurrent = ShardedConcurrentStore.FromCores;
        _shardedNoPoolConcurrent = ShardedConcurrentStoreNoPool.FromCores;

        for (int i = 0; i < EntryCount; i++)
        {
            _concurrent[_keys[i]] = PooledCacheEntry.CreateNeverExpiring(_values[i]);
            _shardedConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
            _shardedNoPoolConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
        }
    }
    
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _concurrent.TryGetValue(_keys[i], out _);
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
    public void ShardedNoPoolConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _shardedNoPoolConcurrent.TryGet(_keys[i], out _);
        });
    }
}


[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StorePutBenchmarks
{
    [Params(1, 4, 8, 16)] public int Threads;
    [Params(100, 1_000, 10_000, 100_000)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private ConcurrentDictionary<BytesKey, PooledCacheEntry> _concurrent = null!;
    private ShardedConcurrentStore _shardedConcurrent = null!;
    private ShardedConcurrentStoreNoPool _shardedNoPoolConcurrent = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _keys = new BytesKey[EntryCount];
        _values = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _keys[i] = new BytesKey(BitConverter.GetBytes(i));
            _values[i] = new byte[64];
        }

        _concurrent = new ConcurrentDictionary<BytesKey, PooledCacheEntry>();
        _shardedConcurrent = ShardedConcurrentStore.FromCores;
        _shardedNoPoolConcurrent = ShardedConcurrentStoreNoPool.FromCores;

        for (int i = 0; i < EntryCount; i++)
        {
            _concurrent[_keys[i]] = PooledCacheEntry.CreateNeverExpiring(_values[i]);
            _shardedConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
            _shardedNoPoolConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
        }
    }
    
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionary_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _concurrent[_keys[i]] = PooledCacheEntry.CreateNeverExpiring(_values[i]);
        });
    }
    
    [Benchmark]
    public void ShardedConcurrentDictionary_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _shardedConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
        });
    }

    [Benchmark]
    public void ShardedNoPoolConcurrentDictionary_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _shardedNoPoolConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
        });
    }
}