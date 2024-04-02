using BenchmarkDotNet.Running;
using Benchmarks;
using Benchmarks.Helpers;
using Benchmarks.Lists;

//var t1 = new QuicksortBenchmarks();
//t1.GlobalSetup();
//t1.IterationSetup();
//t1.DefaultSort();

//var t2 = new QuicksortBenchmarks();
//t2.GlobalSetup();
//t2.IterationSetup();
//t2.Sort();

//var t3 = new QuicksortBenchmarks();
//t3.GlobalSetup();
//t3.IterationSetup();
//t3.Quicksort();

//var t4 = new QuicksortBenchmarks();
//t4.GlobalSetup();
//t4.IterationSetup();
//t4.NativeSort();

//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t1.Items, t4.Items));
//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t2.Items, t4.Items));
//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t3.Items, t4.Items));

BenchmarkRunner.Run<QuicksortBenchmarks>(new Config());

//var random = new Random(564754121);

//var numbers = Enumerable.Range(0, 20000).ToArray();
//random.Shuffle(numbers);

//await SortAsync(numbers, 0, numbers.Length - 1);

//Console.WriteLine(numbers[20000 - 1]);

//async Task SortAsync(int[] values, int left, int right)
//{
//    if (left < right)
//    {
//        var pivot = Partitionate(values, left, right);
//        await Task.WhenAll(SortAsync(values, left, pivot - 1), SortAsync(values, pivot + 1, right));
//    }
//}

//void Sort(int[] values, int left, int right)
//{
//    if (left < right)
//    {
//        var pivot = Partitionate(values, left, right);
//        Sort(values, left, pivot - 1);
//        Sort(values, pivot + 1, right);
//    }
//}

//int Partitionate(int[] values, int left, int right) 
//{
//    var pivot = values[right];
//    var i = left - 1;
//    for (int j = left;  j < right; j++)
//    {
//        if (values[j] <= pivot)
//        {
//            i++;
//            (values[i], values[j]) = (values[j], values[i]);
//        }
//    }
//    i++;
//    (values[i], values[right]) = (values[right], values[i]);
//    return i;
//}