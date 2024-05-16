using BenchmarkDotNet.Attributes;
using Benchmarks.Helpers;
using System.ComponentModel;

namespace Benchmarks.Lists
{
    [Description("Sort implementation")]
    [MemoryDiagnoser]
    public class SortBenchmarks
    {
        private Random Random;
        public int[] Items = Enumerable.Range(0, 10_000).ToArray();

        [Benchmark]
        public int DefaultSort()
        {
            ArrayExtensions.DefaultSort(Items);
            return Items[1_000];
        }

        [Benchmark]
        public int Sort()
        {
            ArrayExtensions.Sort(Items);
            return Items[1_000];
        }

        [Benchmark]
        public int Quicksort()
        {
            ArrayExtensions.Quicksort(Items);
            return Items[1_000];
        }

        [Benchmark]
        public int OptimalQuicksort()
        {
            ArrayExtensions.OptimalQuicksort(Items);
            return Items[1_000];
        }

        [Benchmark]
        public int NativeSort()
        {
            Array.Sort(Items);
            return Items[1_000];
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            Random = new Random(1911512744);
        }

        [IterationSetup]
        public void IterationSetup() 
        {
            Random.Shuffle(Items);
        }
    }
}
