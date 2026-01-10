using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Marshalled;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;


[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Put_Store_Duplicate_Keys_Benchmarks
{
    [Params(4, 8, 12)] public int Threads;
    [Params(100, 300, 1000)] public int EntryCount;
    
    private BytesKey _key;
    private byte[][] _values = null!;

    private ShardedConcurrentStore _shardedEntries = null!;
    private MarshalledStore _marshalledStore = null!;
    private ConcurrentStore _concurrentStore = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        var options = new CubbyServerOptions();

        _values = new byte[EntryCount][];
        _key = new BytesKey(BitConverter.GetBytes(1));

        for (int i = 0; i < EntryCount; i++)
        {
            _values[i] = new byte[64];
        }

        _shardedEntries = ShardedConcurrentStore.FromOptions(options, TimeProvider.System);
        _marshalledStore = MarshalledStore.FromOptions(options, TimeProvider.System);
        _concurrentStore = ConcurrentStore.FromOptions(options, TimeProvider.System);
    }
    
    [Benchmark(Baseline = true)]
    public void Put_Sharded_Concurrent_Store()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _shardedEntries.Put(_key, _values[i], CacheEntryOptions.None);
        });
    }

    [Benchmark]
    public void Put_Marshalled_Store()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _marshalledStore.Put(_key, _values[i], CacheEntryOptions.None);
        });
    }

    [Benchmark]
    public void Put_Concurrent_Store()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _concurrentStore.Put(_key, _values[i], CacheEntryOptions.None);
        });
    }
}