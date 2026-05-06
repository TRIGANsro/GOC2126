using BenchmarkDotNet.Running;

namespace SumSpeed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SumTest>();
        }
    }
}
