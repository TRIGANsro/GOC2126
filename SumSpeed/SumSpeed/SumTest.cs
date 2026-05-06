using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace SumSpeed
{
    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net10_0)]
    public class SumTest
    {
        private int[] data;
        private Random generator;

        [Params(1_000_000, 10_000_000)] public int N;

        [GlobalSetup]
        public void Setup()
        {
            generator = new Random(42);
            data = new int[N];
            for (int i = 0; i < N; i++)
            {
                data[i] = generator.Next(1, 10);
            }
        }

        [Benchmark]
        public int ForEachSum()
        {
            int sum = 0;
            foreach (var item in data)
            {
                sum += item;
            }

            return sum;
        }

        [Benchmark]
        public int LinqSum()
        {
            int sum = data.Sum();
            return sum;
        }

        [Benchmark]
        public int ParallelLinqSum()
        {
            int sum = data.AsParallel().Sum();
            return sum;
        }

    }
}
