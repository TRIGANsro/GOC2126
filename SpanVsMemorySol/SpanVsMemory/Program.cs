using BenchmarkDotNet.Running;

namespace SpanVsMemory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<SpanVsMemoryBenchmark>();

            //BenchmarkRunner.Run<SpanVsMemoryPoolBenchmark>();

            BenchmarkRunner.Run<SpanVsCustomMemoryManagerBenchmark>();
        }
    }
}
