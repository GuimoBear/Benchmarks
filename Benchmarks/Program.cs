using BenchmarkDotNet.Running;
using Benchmarks;
using Benchmarks.Bloomfilter;
using System.Text;

//var t1 = new SortBenchmarks();
//t1.GlobalSetup();
//t1.IterationSetup();
//t1.DefaultSort();

//var t2 = new SortBenchmarks();
//t2.GlobalSetup();
//t2.IterationSetup();
//t2.Sort();

//var t3 = new SortBenchmarks();
//t3.GlobalSetup();
//t3.IterationSetup();
//t3.Quicksort();

//var t4 = new SortBenchmarks();
//t4.GlobalSetup();
//t4.IterationSetup();
//t4.OptimalQuicksort();

//var t5 = new SortBenchmarks();
//t5.GlobalSetup();
//t5.IterationSetup();
//t5.NativeSort();

//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t1.Items, t4.Items));
//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t2.Items, t4.Items));
//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t3.Items, t4.Items));
//Console.WriteLine(ArrayExtensions.IsEquivalentTo(t3.Items, t5.Items));

//using (var indexes = File.CreateText(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "indexes.txt")))
//{
//    var faker = new Faker<BloomfilterBenchmarks.Person>()
//        .UseSeed(914274)
//        .RuleFor(p => p.Id, f => f.Random.Uuid())
//        .RuleFor(p => p.CompanyId, f => f.Random.Uuid())
//        .RuleFor(p => p.Document, f => f.Company.Cnpj())
//        .RuleFor(p => p.Name, f => f.Person.FullName)
//        .RuleFor(p => p.CoorporateName, f => f.Company.CompanyName());
//    for (int i = 0; i < 10_000_000; i++)
//    {
//        var person = faker.Generate();
//        var index = $"{person.CompanyId}{person.Document}{person.Name}{person.CoorporateName}";
//        indexes.WriteLine(index);
//        if (i % 10_000 == 0)
//        {
//            Console.WriteLine($"{i:N0} de 10.000.000");
//            indexes.Flush();
//        }
//    }
//    indexes.Flush();
//    indexes.Close();
//}

//byte[][] indexes = File
//    .ReadAllLines(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", "indexes.txt"))
//    .Select(Encoding.UTF8.GetBytes)
//    .ToArray();

//var parameters = new double[] { 0.001, 0.002, 0.005, 0.05 }
//    .SelectMany(n => new int[] { 5_000_000, 10_000_000, 20_000_000, 50_000_000 }.Select(n2 => (ProbabilityOfFalsePositives: n, ExpectedElementsInTheFilter: n2)))
//    .ToArray();

//foreach (var (probabilityOfFalsePositives, expectedElementsInTheFilter) in parameters)
//{
//var stringBloomFilter = StringBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//var bloomFilter = BloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//var byteSpanBloomFilter = ByteSpanBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//var unsafeBloomFilter = UnsafeBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//var spanBloomFilter = SpanBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);

//var length = indexes.Length;
//for (int i = 0; i < length; i++)
//{
//    var bytes = indexes[i];

//    stringBloomFilter.Add(bytes);
//    bloomFilter.Add(bytes);
//    byteSpanBloomFilter.Add(bytes);
//    unsafeBloomFilter.Add(bytes);
//    spanBloomFilter.Add(bytes);
//}

//StringBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(StringBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newStringBloomFilter);

//if (!stringBloomFilter.Equals(newStringBloomFilter))
//{
//    Console.WriteLine("1");
//}

//BloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(BloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newBloomFilter);

//if (!bloomFilter.Equals(newBloomFilter))
//{
//    Console.WriteLine("2");
//}

//ByteSpanBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(ByteSpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newByteSpanBloomFilter);

//if (!byteSpanBloomFilter.Equals(newByteSpanBloomFilter))
//{
//    Console.WriteLine("3");
//}

//UnsafeBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(UnsafeBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newUnsafeBloomFilter);

//if (!unsafeBloomFilter.Equals(newUnsafeBloomFilter))
//{
//    Console.WriteLine("4");
//}

//SpanBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(SpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newSpanBloomFilter);

//if (!spanBloomFilter.Equals(newSpanBloomFilter))
//{
//    Console.WriteLine("5");
//}
//break;
//stringBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(StringBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//bloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(BloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//byteSpanBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(ByteSpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//unsafeBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(UnsafeBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//spanBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(SpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//}








//byte b = 156;
//var bools = Utils.GetBits(b);
//var newB = Utils.GetByte(bools);

//var bm = new BloomfilterBenchmarks();
//bm.ProbabilityOfFalsePositives = 0.005;
//bm.ExpectedElementsInTheFilter = 50_000_000;
//bm.Setup();
//bm.IterationSetup();
//bm.AddInByteArrayBloomFilterBenchmark();
//bm.AddInByteSpanBloomFilterBenchmark();
//bm.AddInUnsafeBloomFilterBenchmark();
//bm.AddInBitArrayBloomFilterBenchmark();
//bm.AddInBoolSpanBloomFilterBenchmark();
//bm.SearchInByteArrayBloomFilterBenchmark();
//bm.SearchInByteSpanBloomFilterBenchmark();
//bm.SearchInUnsafeBloomFilterBenchmark();
//bm.SearchInBitArrayBloomFilterBenchmark();
//bm.SearchInBoolSpanBloomFilterBenchmark();

BenchmarkRunner.Run<BloomfilterBenchmarks>(new Config());

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