using BenchmarkDotNet.Attributes;
using Scribbly.Cubby.Stores;
using Scribbly.Cubby.Stores.Sharded;

namespace Scribbly.Cubby.Benchmarks.Stores;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StorePutStructBenchmarks
{
    [Params(4)] public int Threads;
    [Params(100)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private StaticPooledConcurrentStore _classPooledEntries = null!;
    private StructPooledConcurrentStore _structPooledEntries = null!;
    
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

        _classPooledEntries = StaticPooledConcurrentStore.FromCores;
        _structPooledEntries = StructPooledConcurrentStore.FromCores;
    }
    
    [Benchmark(Baseline = true)]
    public void Class_PooledArrayStore_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _classPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
        });
    }

    [Benchmark]
    public void Struct_PooledArrayStore_Put()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _structPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
        });
    }
}

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class StoreGetStructBenchmarks
{
    [Params(4)] public int Threads;
    [Params(100)] public int EntryCount;
    
    private BytesKey[] _keys = null!;
    private byte[][] _values = null!;

    private StaticPooledConcurrentStore _classPooledEntries = null!;
    private StructPooledConcurrentStore _structPooledEntries = null!;
    
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

        _classPooledEntries = StaticPooledConcurrentStore.FromCores;
        _structPooledEntries = StructPooledConcurrentStore.FromCores;
        
        for (int i = 0; i < EntryCount; i++)
        {
            _classPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
            _structPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
        }
    }
    
    [Benchmark(Baseline = true)]
    public void Class_PooledArrayStore_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _classPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
        });
    }

    [Benchmark]
    public void Struct_PooledArrayStore_Get()
    {
        Parallel.For(0, EntryCount, new ParallelOptions { MaxDegreeOfParallelism = Threads }, i =>
        {
            _structPooledEntries.Put(_keys[i], _values[i], new CacheEntryOptions());
        });
    }
}