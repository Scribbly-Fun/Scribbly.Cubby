using System.Buffers;
using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StoreGetBenchmarks
{
    [Params(1, 4)] public int Threads;
    [Params(100, 1_000)] public int EntryCount;
    
    private byte[][] _keys = null!;
    private byte[][] _values = null!;

    private ConcurrentStore _concurrent = null!;
    private ShardedConcurrentStore _shardedEntries = null!;
    private ShardedConcurrentStore _shardedPooledEntries = null!;
    
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

        _concurrent = new ConcurrentStore();
        _shardedEntries = ShardedConcurrentStore.FromCores;
        _shardedEntries = ShardedConcurrentStore.FromCores;

        for (int i = 0; i < EntryCount; i++)
        {
            _concurrent.Put(PooledCacheEntry.CreateNeverExpiring(_keys[i], _values[i]));
            _shardedEntries.Put(PooledCacheEntry.CreateNeverExpiring(_keys[i], _values[i]));
            
            var buffer = ArrayPool<byte>.Shared.Rent(16 + _keys[i].Length + _values[i].Length);
            _shardedPooledEntries.Put(BufferedCacheEntry.CreateNeverExpiring(buffer, _keys[i], _values[i]));
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
            _ = _shardedEntries.TryGet(_keys[i], out _);
        });
    }

    [Benchmark]
    public void ShardedPooledConcurrentDictionary_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _ = _shardedPooledEntries.TryGet(_keys[i], out _);
        });
    }
}