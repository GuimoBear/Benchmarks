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

        private ulong MaybeContains;
        private ulong NotContains;

        private int numberOfInsertedKeys;
        private bool usingExistingKeys;
        private double probabilityOfFalsePositives;
        private int expectedElementsInTheFilter;

        public Faker<Person>? Faker { get; private set; }

        //public IBloomFilter? BloomFilter { get; private set; }

        public BitArrayBloomFilter? BitArrayBloomFilter { get; private set; }
        public ByteArrayBloomFilter? ByteArrayBloomFilter { get; private set; }
        public ByteSpanBloomFilter? ByteSpanBloomFilter { get; private set; }
        public UnsafeByteArrayBloomFilter? UnsafeByteArrayBloomFilter { get; private set; }
        public BoolSpanBloomFilter? BoolSpanBloomFilter { get; private set; }

        [Params(/*10_000_000, 5_000_000, */1_000_000, 500_000)]
        public int NumberOfInsertedKeys
        {
            get => numberOfInsertedKeys;
            set
            {
                numberOfInsertedKeys = value;
                this.Faker = CreateFaker(usingExistingKeys);
                PostSetup();
            }
        }

        [Params(false, true)]
        public bool UsingExistingKeys 
        { 
            get => usingExistingKeys;
            set
            {
                usingExistingKeys = value;
                this.Faker = CreateFaker(usingExistingKeys);
                PostSetup();
            }
        }
        [Params(/*0.001, 0.002, */0.005, 0.05)]
        public double ProbabilityOfFalsePositives 
        { 
            get => probabilityOfFalsePositives;
            set
            {
                probabilityOfFalsePositives = value;
                this.Faker = CreateFaker(usingExistingKeys);
                PostSetup();
            }
        }
        [Params(5_000_000, 10_000_000, /*20_000_000, 50_000_000, */100_000_000)]
        public int ExpectedElementsInTheFilter 
        { 
            get => expectedElementsInTheFilter;
            set
            {
                expectedElementsInTheFilter = value;
                this.Faker = CreateFaker(usingExistingKeys);
                PostSetup();
            }
        }

        private static Faker<Person> CreateFaker(bool usingExistingKeys)
        {
            //if (usingExistingKeys)
            //{
                return new Faker<Person>()
                    .UseSeed(usingExistingKeys ? 914274 : 2125876541)
                    .RuleFor(p => p.Id, f => f.Random.Uuid())
                    .RuleFor(p => p.CompanyId, f => f.Random.Uuid())
                    .RuleFor(p => p.Document, f => f.Company.Cnpj())
                    .RuleFor(p => p.Name, f => f.Person.FullName)
                    .RuleFor(p => p.CoorporateName, f => f.Company.CompanyName());
            //}
            //else
            //{
            //    return new Faker<Person>()
            //        .UseSeed(2125876541)
            //        .RuleFor(p => p.Id, f => f.Random.Guid())
            //        .RuleFor(p => p.CompanyId, f => f.Random.Guid())
            //        .RuleFor(p => p.Document, f => f.Person.Cpf())
            //        .RuleFor(p => p.Name, f => f.Person.FullName)
            //        .RuleFor(p => p.CoorporateName, f => $"{f.Company.CompanySuffix()} - {f.Company.CompanyName()}");
            //}
        }

        //private void AddKeys(IBloomFilter bloomFilter)
        //{
        //    var indexes = Utils.Indexes.Value;
        //    var length = indexes.Length;
        //    for (int i = 0; i < length; i++)
        //    {
        //        bloomFilter.Add(indexes[i]);
        //    }
        //}

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
            //SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{numberOfInsertedKeys:N0} keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom");
            if (!ByteArrayBloomFilter.TryLoad(filename, out var filter))
            {
                throw new Exception();
                //Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                //filter = new ByteArrayBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                //AddKeys(filter);
            }
            this.ByteArrayBloomFilter = filter;
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInByteSpanBloomFilterBenchmark), nameof(SearchInByteSpanBloomFilterBenchmark)])]
        public void ByteSpanSetup()
        {
            //SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{numberOfInsertedKeys:N0} keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom");
            if (!ByteSpanBloomFilter.TryLoad(filename, out var filter))
            {
                throw new Exception();
                //Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                //filter = new ByteSpanBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                //AddKeys(filter);
            }
            this.ByteSpanBloomFilter = filter;
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInUnsafeBloomFilterBenchmark), nameof(SearchInUnsafeBloomFilterBenchmark)])]
        public void UnsafeSetup()
        {
            //SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{numberOfInsertedKeys:N0} keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom");
            if (!UnsafeByteArrayBloomFilter.TryLoad(filename, out var filter))
            {
                throw new Exception();
                //Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                //filter = new UnsafeByteArrayBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                //AddKeys(filter);
            }
            this.UnsafeByteArrayBloomFilter = filter;
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInBitArrayBloomFilterBenchmark), nameof(SearchInBitArrayBloomFilterBenchmark)])]
        public void BitArraySetup()
        {
            //SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{numberOfInsertedKeys:N0} keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom");
            if (!BitArrayBloomFilter.TryLoad(filename, out var filter))
            {
                throw new Exception();
                //Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                //filter = new BitArrayBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                //AddKeys(filter);
            }
            this.BitArrayBloomFilter = filter;
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInBoolSpanBloomFilterBenchmark), nameof(SearchInBoolSpanBloomFilterBenchmark)])]
        public void BoolSpanSetup()
        {
            //SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"{numberOfInsertedKeys:N0} keys with {probabilityOfFalsePositives:P1} of false positive and {expectedElementsInTheFilter:N0} keys capacity.bloom");
            if (!BoolSpanBloomFilter.TryLoad(filename, out var filter))
            {
                throw new Exception();
                //Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                //filter = new BoolSpanBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                //AddKeys(filter);
            }
            this.BoolSpanBloomFilter = filter;
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
            if (!usingExistingKeys)
            {
                this.Index = string.Create(this.Index.Length, this.Index, (chars, str) =>
                {
                    var index = str.Length - 1;
                    var size = chars.Length;
                    for (int i = 0; i < size; i++) 
                    {
                        chars[i] = str[index--];
                    }
                });
            }
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
                var maybeContains = this.MaybeContains * 1.0m;
                var notContains = this.NotContains * 1.0m;

                var maybeExistsPercentage = maybeContains / (maybeContains + notContains);
                BenchmarkMetadata.Instance.AddMetadata("Iterations count", $"{(int)(this.MaybeContains + this.NotContains)}"); // add benchmark metadata
                BenchmarkMetadata.Instance.AddMetadata("Maybe exists", $"{this.MaybeContains:N0}"); // add benchmark metadata
                BenchmarkMetadata.Instance.AddMetadata("Not exists", $"{this.NotContains:N0}"); // add benchmark metadata
                BenchmarkMetadata.Instance.AddMetadata("Continue percentage", $"{maybeExistsPercentage:P3}"); // add benchmark metadata
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
