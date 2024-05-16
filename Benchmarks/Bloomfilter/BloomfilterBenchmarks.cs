using BenchmarkDotNet.Attributes;
using Benchmarks.Bloomfilter.Impl;
using Benchmarks.Utils;
using Bogus;
using Bogus.Extensions.Brazil;
using System.Text;

namespace Benchmarks.Bloomfilter
{
    [MemoryDiagnoser(true)]
    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
    public class BloomfilterBenchmarks
    {
        //private delegate IBloomFilter Factory(double probabilityOfFalsePositives, int expectedElementsInTheFilter);
        //private static readonly Factory[] FactoryLookup = [StringBloomFilter.Create, Bloomfilter.BloomFilter.Create, SpanBloomFilter.Create];

        private decimal MaybeContains;
        private decimal NotContains;

        public Faker<Person>? Faker { get; private set; }

        //public IBloomFilter? BloomFilter { get; private set; }

        public BitArrayBloomFilter? BitArrayBloomFilter { get; private set; }
        public ByteArrayBloomFilter? ByteArrayBloomFilter { get; private set; }
        public ByteSpanBloomFilter? ByteSpanBloomFilter { get; private set; }
        public UnsafeByteArrayBloomFilter? UnsafeByteArrayBloomFilter { get; private set; }
        public BoolSpanBloomFilter? BoolSpanBloomFilter { get; private set; }

        //[Params(Implementation.BitArray, Implementation.ByteArray, Implementation.BoolSpan)]
        //public Implementation Implementation { get; set; }
        //[Params(false, true)]
        public bool UsingExistingKeys { get; set; } = false;
        [Params(/*0.001, 0.002, */0.005, 0.05)]
        public double ProbabilityOfFalsePositives { get; set; }
        [Params(5_000_000, 10_000_000/*, 20_000_000, 50_000_000*/)]
        public int ExpectedElementsInTheFilter { get; set; }

        private void SetupBogus()
        {
            this.Faker = new Faker<Person>()
                .UseSeed(this.UsingExistingKeys ? 914274 : 6546414)
                .RuleFor(p => p.Id, f => f.Random.Uuid())
                .RuleFor(p => p.CompanyId, f => f.Random.Uuid())
                .RuleFor(p => p.Document, f => f.Company.Cnpj())
                .RuleFor(p => p.Name, f => f.Person.FullName)
                .RuleFor(p => p.CoorporateName, f => f.Company.CompanyName());
        }

        private void AddKeys(IBloomFilter bloomFilter)
        {
            var indexes = Utils.Indexes.Value;
            var length = indexes.Length;
            for (int i = 0; i < length; i++)
            {
                bloomFilter.Add(indexes[i]);
            }
        }

        private void PostSetup()
        {
            this.MaybeContains = this.NotContains = 0;
            //if (this.UsingExistingKeys)
            //{
            //    this.Faker = new Faker<Person>()
            //        .UseSeed(914274)
            //        .RuleFor(p => p.Id, f => f.Random.Uuid())
            //        .RuleFor(p => p.CompanyId, f => f.Random.Uuid())
            //        .RuleFor(p => p.Document, f => f.Company.Cnpj())
            //        .RuleFor(p => p.Name, f => f.Person.FullName)
            //        .RuleFor(p => p.CoorporateName, f => f.Company.CompanyName());
            //}
        }

        [GlobalSetup(Targets = [nameof(AddInByteArrayBloomFilterBenchmark), nameof(SearchInByteArrayBloomFilterBenchmark)])]
        public void Setup() 
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"ByteArrayBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!ByteArrayBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new ByteArrayBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.ByteArrayBloomFilter = filter;
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInByteSpanBloomFilterBenchmark), nameof(SearchInByteSpanBloomFilterBenchmark)])]
        public void ByteSpanSetup()
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"ByteSpanBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!ByteSpanBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new ByteSpanBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.ByteSpanBloomFilter = filter;
            AddKeys(this.ByteSpanBloomFilter);
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInUnsafeBloomFilterBenchmark), nameof(SearchInUnsafeBloomFilterBenchmark)])]
        public void UnsafeSetup()
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"UnsafeByteArrayBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!UnsafeByteArrayBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new UnsafeByteArrayBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.UnsafeByteArrayBloomFilter = filter;
            AddKeys(this.UnsafeByteArrayBloomFilter);
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInBitArrayBloomFilterBenchmark), nameof(SearchInBitArrayBloomFilterBenchmark)])]
        public void BitArraySetup()
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"BitArrayBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!BitArrayBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new BitArrayBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.BitArrayBloomFilter = filter;
            AddKeys(this.BitArrayBloomFilter);
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInBoolSpanBloomFilterBenchmark), nameof(SearchInBoolSpanBloomFilterBenchmark)])]
        public void BoolSpanSetup()
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"BoolSpanBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!BoolSpanBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new BoolSpanBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.BoolSpanBloomFilter = filter;
            AddKeys(this.BoolSpanBloomFilter);
            PostSetup();
        }

        public Guid? CompanyId { get; set; }
        public string? Document { get; set; }
        public string? Name { get; set; }
        public string? CoorporateName { get; set; }

        public string? Index { get; set; }
        public byte[]? Bytes { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            var person = this.Faker!.Generate();
            this.CompanyId = person.CompanyId;
            this.Document = person.Document;
            this.Name = person.Name;
            this.CoorporateName = person.CoorporateName;
            this.Index = $"{person.CompanyId}{person.Document}{person.Name}{person.CoorporateName}";
            this.Bytes = Encoding.UTF8.GetBytes(this.Index);
        }

        [Benchmark(Description = "Add in byte[] BloomFilter", Baseline = true)]
        [BenchmarkCategory("Add")]
        public bool AddInByteArrayBloomFilterBenchmark()
        {
            return this.ByteArrayBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Add in Span<byte> BloomFilter")]
        [BenchmarkCategory("Add")]
        public bool AddInByteSpanBloomFilterBenchmark()
        {
            return this.ByteSpanBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Add in Unsafe byte[] BloomFilter")]
        [BenchmarkCategory("Add")]
        public bool AddInUnsafeBloomFilterBenchmark()
        {
            return this.UnsafeByteArrayBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Add in BitArray BloomFilter")]
        [BenchmarkCategory("Add")]
        public bool AddInBitArrayBloomFilterBenchmark()
        {
            return this.BitArrayBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Add in Span<bool> BloomFilter")]
        [BenchmarkCategory("Add")]
        public bool AddInBoolSpanBloomFilterBenchmark()
        {
            return this.BoolSpanBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Search in byte[] BloomFilter", Baseline = true)]
        [BenchmarkCategory("Contains")]
        public bool SearchInByteArrayBloomFilterBenchmark()
        {
            var res = this.ByteArrayBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [Benchmark(Description = "Search in Span<byte> BloomFilter")]
        [BenchmarkCategory("Contains")]
        public bool SearchInByteSpanBloomFilterBenchmark()
        {
            var res = this.ByteSpanBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [Benchmark(Description = "Search in Unsafe byte[] BloomFilter")]
        [BenchmarkCategory("Contains")]
        public bool SearchInUnsafeBloomFilterBenchmark()
        {
            var res = this.UnsafeByteArrayBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [Benchmark(Description = "Search in BitArray BloomFilter")]
        [BenchmarkCategory("Contains")]
        public bool SearchInBitArrayBloomFilterBenchmark()
        {
            var res = this.BitArrayBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [Benchmark(Description = "Search in Span<bool> BloomFilter")]
        [BenchmarkCategory("Contains")]
        public bool SearchInBoolSpanBloomFilterBenchmark()
        {
            var res = this.BoolSpanBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (this.MaybeContains + this.NotContains > 0)
            {
                var maybeExistsPercentage = this.MaybeContains / (this.MaybeContains + this.NotContains) * 100.0m;
                var notExistsPercentage = this.NotContains / (this.MaybeContains + this.NotContains) * 100.0m;
                BenchmarkMetadata.Instance.AddMetadata("Iterations count", $"{(int)(this.MaybeContains + this.NotContains)}"); // add benchmark metadata
                BenchmarkMetadata.Instance.AddMetadata("Maybe exists", $"{maybeExistsPercentage:F4} %"); // add benchmark metadata
                BenchmarkMetadata.Instance.AddMetadata("Not exists", $"{notExistsPercentage:F4} %"); // add benchmark metadata
                BenchmarkMetadata.Instance.Save(nameof(BloomfilterBenchmarks));
            }
        }

        //[Benchmark(Description = "Using SortedSet")]
        //[BenchmarkCategory("Contains")]
        //public bool SortedSetBenchmark()
        //{
        //    return this.SortedSet!.Contains(this.Index!);
        //}

        //[Benchmark(Description = "Using BloomFilter and SortedSet")]
        //public bool BloomFilterAndSortedSetBenchmark()
        //{
        //    if (this.StringBloomFilter!.MaybeContains(this.Bytes!))
        //        return this.SortedSet!.Contains(this.Index!);
        //    return false;
        //}

        //[Benchmark(Description = "Using HashSet")]
        //[BenchmarkCategory("Contains")]
        //public bool HashSetBenchmark()
        //{
        //    return this.HashSet!.Contains(this.Index!);
        //}

        public class Person
        {
            public Guid Id { get; set; }
            public Guid CompanyId { get; set; }
            public string Document { get; set; }
            public string Name { get; set; }
            public string CoorporateName { get; set; }
        }
    }
}
