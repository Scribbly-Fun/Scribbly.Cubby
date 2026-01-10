using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Concurrent;
using Scribbly.Cubby.Stores.Marshalled;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Get_Store_Unique_Keys_Benchmarks
{
    [Params(4, 8, 12)] public int Threads;
    [Params(100, 300, 1000)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private ShardedConcurrentStore _shardedEntries = null!;
    private MarshalledStore _marshalledStore = null!;
    private ConcurrentStore _concurrentStore = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        var options = new CubbyServerOptions();
        
        _keys = new BytesKey[EntryCount];
        _values = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _keys[i] = new BytesKey(BitConverter.GetBytes(i));
            _values[i] = new byte[64];
        }

        _shardedEntries = ShardedConcurrentStore.FromOptions(options, TimeProvider.System);
        _marshalledStore = MarshalledStore.FromOptions(options, TimeProvider.System);
        _concurrentStore = ConcurrentStore.FromOptions(options, TimeProvider.System);

        for (int i = 0; i < EntryCount; i++)
        {
            _shardedEntries.Put(_keys[i], _values[i], CacheEntryOptions.None);
            _marshalledStore.Put(_keys[i], _values[i], CacheEntryOptions.None);
            _concurrentStore.Put(_keys[i], _values[i], CacheEntryOptions.None);
        }
    }

    [Benchmark]
    public void Get_Sharded_Concurrent_Store()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            var _ = _shardedEntries.Get(_keys[i]);
        });
    }
    
    [Benchmark]
    public void Get_Marshalled_Store()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            var _ = _marshalledStore.Get(_keys[i]);
        });
    }
    
    [Benchmark]
    public void Get_Concurrent_Store()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            var _ = _concurrentStore.Get(_keys[i]);
        });
    }
}