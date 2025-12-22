using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

/// <summary>
/// This benchmark populates the value and creates an initial value.  It then updates the item for the n or Entries.
/// This demonstrates our performance updating an existing item.
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StorePutUpdateBenchmarks
{
    [Params(1, 4)] public int Threads;
    [Params(100, 1_000)] public int EntryCount;

    private BytesKey _key;
    private byte[][] _values = null!;

    private ConcurrentStore _concurrent = null!;
    private ShardedConcurrentStore _pooledEntries = null!;
    private StaticPooledConcurrentStore _staticPooledEntries = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _key = new BytesKey([0x01, 0x02, 0x01]);
        _values = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _values[i] = new byte[64];
        }

        _concurrent = new ConcurrentStore();
        _pooledEntries = ShardedConcurrentStore.FromCores;
        _staticPooledEntries = StaticPooledConcurrentStore.FromCores;
        
        _concurrent.Put(_key, _values[0], new CacheEntryOptions());
        _pooledEntries.Put(_key, _values[0], new CacheEntryOptions());
        _staticPooledEntries.Put(_key, _values[0], new CacheEntryOptions());
    }
    
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionary_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _concurrent.Put(_key, _values[i], new CacheEntryOptions());
        });
    }
    
    [Benchmark]
    public void PooledArrayStore_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _pooledEntries.Put(_key, _values[i], new CacheEntryOptions());
        });
    }

    [Benchmark]
    public void StaticPooledArrayStore_Put()
    {
        
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _staticPooledEntries.Put(_key, _values[i], new CacheEntryOptions());
        });
    }
}