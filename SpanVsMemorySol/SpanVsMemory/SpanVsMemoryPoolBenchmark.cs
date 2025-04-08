using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class SpanVsMemoryPoolBenchmark
{
    private IMemoryOwner<byte> pooledMemory;

    [GlobalSetup]
    public void Setup()
    {
        pooledMemory = MemoryPool<byte>.Shared.Rent(1024);
    }

    [Benchmark(Baseline = true)]
    public void Stackalloc_Span()
    {
        Span<byte> buffer = stackalloc byte[1024];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i]++;
    }

    [Benchmark]
    public void MemoryPool_Memory()
    {
        Memory<byte> memory = pooledMemory.Memory;
        Span<byte> buffer = memory.Span;
        for (int i = 0; i < buffer.Length; i++)
            buffer[i]++;
    }
}