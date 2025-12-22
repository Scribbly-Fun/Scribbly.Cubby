using System.Buffers;
using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StorePutBenchmarks
{
    [Params(1, 4)] public int Threads;
    [Params(100, 1_000)] public int EntryCount;
    
    private byte[][] _keys = null!;
    private byte[][] _values = null!;
    private byte[][] _buffers = null!;

    private ConcurrentStore _concurrent = null!;
    private ShardedConcurrentStore _shardedEntries = null!;
    private ShardedConcurrentStore _shardedPooledEntries = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _keys = new byte[EntryCount][];
        _values = new byte[EntryCount][];
        _buffers = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _keys[i] = BitConverter.GetBytes(i);
            _values[i] = new byte[64];

            _buffers[i] = ArrayPool<byte>.Shared.Rent(16 + _keys[i].Length + _values[i].Length);
        }

        _concurrent = new ConcurrentStore();
        _shardedEntries = ShardedConcurrentStore.FromCores;
        _shardedPooledEntries = ShardedConcurrentStore.FromCores;
    }
    
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionary_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _concurrent.Put(PooledCacheEntry.CreateNeverExpiring(_keys[i], _values[i]));
        });
    }
    
    [Benchmark]
    public void ShardedConcurrentDictionary_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _shardedEntries.Put(PooledCacheEntry.CreateNeverExpiring(_keys[i], _values[i]));
        });
    }

    [Benchmark]
    public void ShardedPooledConcurrentDictionary_Put()
    {
        
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _shardedPooledEntries.Put(BufferedCacheEntry.Create(_buffers[i], _keys[i], _values[i]));
        });
    }
}