using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StoreGetStructBenchmarks
{
    [Params(4)] public int Threads;
    [Params(100)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private SharedConcurrentStore _sharedEntries = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        var options = new CubbyOptions();
        
        _keys = new BytesKey[EntryCount];
        _values = new byte[EntryCount][];

        for (int i = 0; i < EntryCount; i++)
        {
            _keys[i] = new BytesKey(BitConverter.GetBytes(i));
            _values[i] = new byte[64];
        }

        _sharedEntries = SharedConcurrentStore.FromOptions(options);
        
        for (int i = 0; i < EntryCount; i++)
        {
            _sharedEntries.Put(_keys[i], _values[i], CacheEntryOptions.None);
        }
    }

    [Benchmark]
    public void Struct_PooledArrayStore_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _sharedEntries.Put(_keys[i], _values[i], CacheEntryOptions.None);
        });
    }
}