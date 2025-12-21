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
    // [Params(1, 4, 8, 16)] public int Threads;
    //
    // [Params(100_000)] public int EntryCount;

    [Params(1, 4)] public int Threads;

    [Params(100, 1000)] public int EntryCount;

    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private ConcurrentDictionary<BytesKey, CacheEntry> _concurrent = null!;
    
    private ShardedConcurrentStore _shardedConcurrent = null!;
    
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

        _concurrent = new ConcurrentDictionary<BytesKey, CacheEntry>();
        _shardedConcurrent = ShardedConcurrentStore.FromCores;

        for (int i = 0; i < EntryCount; i++)
        {
            _concurrent[_keys[i]] = CacheEntry.CreateNeverExpiring(_values[i]);
            _shardedConcurrent.Put(_keys[i], _values[i], new CacheEntryOptions());
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
}