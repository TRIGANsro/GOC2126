using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SpanVsMemory;

[MemoryDiagnoser]
public class SpanVsCustomMemoryManagerBenchmark
{
    private MyMemoryManager memoryManager;

    [GlobalSetup]
    public void Setup()
    {
        memoryManager = new MyMemoryManager(1024);
    }

    [Benchmark(Baseline = true)]
    public void Stackalloc_Span()
    {
        Span<byte> buffer = stackalloc byte[1024];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i]++;
    }

    [Benchmark]
    public void CustomMemoryManager_Memory()
    {
        Memory<byte> memory = memoryManager.Memory;
        Span<byte> span = memory.Span;
        for (int i = 0; i < span.Length; i++)
            span[i]++;
    }
}