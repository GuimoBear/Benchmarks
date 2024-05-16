using BenchmarkDotNet.Running;
using Benchmarks;
using Benchmarks.Bloomfilter;
using Benchmarks.Bloomfilter.Impl;
using Benchmarks.Strings;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

//byte @byte = 173;
//var bits = @byte.GetBits();
//var newByte = Utils.GetByte(bits);
//Console.WriteLine();

//var bm = new BloomfilterBenchmarks();
//bm.UsingExistingKeys = false;
//bm.ProbabilityOfFalsePositives = 0.001;
//bm.ExpectedElementsInTheFilter = 50_000_000;
//bm.ByteSpanSetup();
//bm.IterationSetup();
//bm.AddInByteSpanBloomFilterBenchmark();

BenchmarkRunner.Run<BloomfilterBenchmarks>(new Config());

//var hashes = new Dictionary<string, List<string>>();
//var sizes = new Dictionary<long, List<string>>();

//var buffer = new byte[1024 * 16];

//foreach (var filename in Directory.GetFiles(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files")))
//{
//    var size = new FileInfo(filename).Length;
//    ref var filenames = ref CollectionsMarshal.GetValueRefOrAddDefault(sizes, size, out bool exists);
//    if (!exists)
//        filenames = new List<string>();
//    filenames!.Add(filename);

//    using (var file = File.OpenRead(filename))
//    {
//        using (var hashing = IncrementalHash.CreateHash(HashAlgorithmName.SHA512))
//        {
//            int count = 0;
//            while ((count = file.Read(buffer, 0, buffer.Length)) > 0)
//            {
//                hashing.AppendData(buffer, 0, count);
//            }
//            var hash = Convert.ToBase64String(hashing.GetHashAndReset());

//            ref var newFilenames = ref CollectionsMarshal.GetValueRefOrAddDefault(hashes, hash, out exists);
//            if (!exists)
//                newFilenames = new List<string>();
//            newFilenames!.Add(filename);
//        }
//    }
//}

//Console.WriteLine();


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


//using (var indexes = File.CreateText(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", "indexes.txt")))
//{
//    var faker = new Faker<BloomfilterBenchmarks.Person>()
//        .UseSeed(914274)
//        .RuleFor(p => p.Id, f => f.Random.Uuid())
//        .RuleFor(p => p.CompanyId, f => f.Random.Uuid())
//        .RuleFor(p => p.Document, f => f.Company.Cnpj())
//        .RuleFor(p => p.Name, f => f.Person.FullName)
//        .RuleFor(p => p.CoorporateName, f => f.Company.CompanyName());
//    for (int i = 0; i < 20_000_000; i++)
//    {
//        var person = faker.Generate();
//        var index = $"{person.CompanyId}{person.Document}{person.Name}{person.CoorporateName}";
//        indexes.WriteLine(index);
//        if (i % 50_000 == 0)
//        {
//            Console.WriteLine($"{i:N0} de 20.000.000");
//            indexes.Flush();
//        }
//    }
//    Console.WriteLine($"20.000.000 de 20.000.000");
//    indexes.Flush();
//    indexes.Close();
//}

//byte[][] indexes = File
//    .ReadAllLines(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", "ten million indexes.txt"))
//    .Take(10_000_000)
//    .Select(Encoding.UTF8.GetBytes)
//    .ToArray();

//var parameters = new double[] { 0.001, 0.002, 0.005, 0.05 }
//    .SelectMany(n => new int[] { 5_000_000, 10_000_000, 20_000_000, 50_000_000, 100_000_000 }.Select(n2 => (ProbabilityOfFalsePositives: n, ExpectedElementsInTheFilter: n2)))
//    .ToArray();

//int count = 1;
//foreach (var (probabilityOfFalsePositives, expectedElementsInTheFilter) in parameters)
//{
//    //var bitArrayBloomFilter = BitArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    //var byteArrayBloomFilter = ByteArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    //var byteSpanBloomFilter = ByteSpanBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    //var unsafeByteArrayBloomFilter = UnsafeByteArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    //var boolSpanBloomFilter = BoolSpanBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);

//    var tenMillionKeysBloomFilter = ByteArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    var fiveMillionKeysBloomFilter = ByteArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    var oneMillionKeysBloomFilter = ByteArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);
//    var halfMillionKeysBloomFilter = ByteArrayBloomFilter.Create(probabilityOfFalsePositives, expectedElementsInTheFilter);

//    var length = 10_000_000;
//    for (int i = 0; i < length; i++)
//    {
//        var bytes = indexes[i];

//        //bitArrayBloomFilter.Add(bytes);
//        //byteArrayBloomFilter.Add(bytes);
//        //byteSpanBloomFilter.Add(bytes);
//        //unsafeByteArrayBloomFilter.Add(bytes);
//        //boolSpanBloomFilter.Add(bytes);

//        tenMillionKeysBloomFilter.Add(bytes);
//        if (i % 2 == 0) fiveMillionKeysBloomFilter.Add(bytes);
//        if (i % 10 == 0) oneMillionKeysBloomFilter.Add(bytes);
//        if (i % 20 == 0) halfMillionKeysBloomFilter.Add(bytes);

//        if (i % 1_000_000 == 0)
//        {
//            Console.WriteLine($"{count} de {parameters.Length} - {i:N0} de {length:N0}");
//        }
//    }
//    Console.WriteLine($"{count} de {parameters.Length} - {length:N0} de {length:N0}");

//    //BitArrayBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(BitArrayBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newBitArrayBloomFilter);

//    //if (!bitArrayBloomFilter.Equals(newBitArrayBloomFilter))
//    //{
//    //    Console.WriteLine("1");
//    //    throw new ArgumentException();
//    //}

//    //ByteArrayBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(ByteArrayBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newByteArrayBloomFilter);

//    //if (!byteArrayBloomFilter.Equals(newByteArrayBloomFilter))
//    //{
//    //    Console.WriteLine("2");
//    //    throw new ArgumentException();
//    //}

//    //ByteSpanBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(ByteSpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newByteSpanBloomFilter);

//    //if (!byteSpanBloomFilter.Equals(newByteSpanBloomFilter))
//    //{
//    //    Console.WriteLine("3");
//    //    throw new ArgumentException();
//    //}

//    //UnsafeByteArrayBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(UnsafeByteArrayBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newUnsafeByteArrayBloomFilter);

//    //if (!unsafeByteArrayBloomFilter.Equals(newUnsafeByteArrayBloomFilter))
//    //{
//    //    Console.WriteLine("4");
//    //    throw new ArgumentException();
//    //}

//    //BoolSpanBloomFilter.TryLoad(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(BoolSpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"), out var newBoolSpanBloomFilter);

//    //if (!boolSpanBloomFilter.Equals(newBoolSpanBloomFilter))
//    //{
//    //    Console.WriteLine("5");
//    //    throw new ArgumentException();
//    //}
//    //continue;

//    Console.WriteLine($"{count} de {parameters.Length} - Excrevendo os arquivos");
//    //bitArrayBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(BitArrayBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//    //byteArrayBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys.bloom"));
//    //byteSpanBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(ByteSpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//    //unsafeByteArrayBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(UnsafeByteArrayBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));
//    //boolSpanBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{nameof(BoolSpanBloomFilter)} with {probabilityOfFalsePositives}-{expectedElementsInTheFilter}.bloom"));

//    tenMillionKeysBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"10.000.000 keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom"));
//    fiveMillionKeysBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"5.000.000 keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom"));
//    oneMillionKeysBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"1.000.000 keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom"));
//    halfMillionKeysBloomFilter.Save(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"500.000 keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom"));


//    Console.WriteLine($"{count} de {parameters.Length} - Arquivos escritos");
//    count++;
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