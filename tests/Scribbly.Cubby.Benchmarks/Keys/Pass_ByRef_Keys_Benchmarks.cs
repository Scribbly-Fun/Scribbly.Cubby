using BenchmarkDotNet.Attributes;

namespace Scribbly.Cubby.Benchmarks.Keys;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Pass_ByRef_Keys_Benchmarks
{
    private BytesKey _key;

    private void Key(BytesKey key)
    {
        Span<byte> buf = key;
    } 
    
    private void KeyIn(in BytesKey key)
    {
        Span<byte> buf = key;
    } 

    private void KeyRef(ref BytesKey key)
    {
        Span<byte> buf = key;
    } 
    
    [GlobalSetup]
    public void Setup()
    {
        _key = new BytesKey([0x01, 0x02, 0x03, 0x04]);
    }

    [Benchmark(Baseline = true)]
    public void Pass_Key()
    {
        Key(_key);
    }
    
    [Benchmark]
    public void Pass_Key_In()
    {
        KeyIn(_key);
    }
    
    [Benchmark]
    public void Pass_Key_Ref()
    {
        KeyRef(ref _key);
    }
}