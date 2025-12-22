using System.Buffers;
using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StoreGetBenchmarks
{
    [Params(1, 4)] public int Threads;
    [Params(100, 1_000)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private ConcurrentStore _concurrent = null!;
    private ShardedConcurrentStore _pooledEntries = null!;
    private ShardedConcurrentStore _staticPooledEntries = null!;
    
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

        _concurrent = new ConcurrentStore();
        _pooledEntries = ShardedConcurrentStore.FromCores;
        _staticPooledEntries = ShardedConcurrentStore.FromCores;

        for (int i = 0; i < EntryCount; i++)
        {
            _concurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
            _pooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
            _staticPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
        }
    }
    
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _concurrent.TryGet(_keys[i], out _);
        });
    }
    
    [Benchmark]
    public void ShardedConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _pooledEntries.TryGet(_keys[i], out _);
        });
    }

    [Benchmark]
    public void ShardedPooledConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _staticPooledEntries.TryGet(_keys[i], out _);
        });
    }
}