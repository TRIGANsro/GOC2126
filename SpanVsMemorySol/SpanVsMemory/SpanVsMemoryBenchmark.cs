using System;
using System.Buffers;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class SpanVsMemoryBenchmark
{
    private byte[] data;

    [GlobalSetup]
    public void Setup()
    {
        data = Enumerable.Repeat((byte)1, 1000).ToArray();
    }

    [Benchmark]
    public void ProcessWithSpan()
    {
        Span<byte> span = data;
        for (int i = 0; i < span.Length; i++)
        {
            span[i] += 1;
        }
    }

    [Benchmark]
    public void ProcessWithMemory()
    {
        Memory<byte> memory = data;
        Span<byte> span = memory.Span;
        for (int i = 0; i < span.Length; i++)
        {
            span[i] += 1;
        }
    }
}