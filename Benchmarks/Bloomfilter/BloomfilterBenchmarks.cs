using BenchmarkDotNet.Attributes;
using Benchmarks.Utils;
using Bogus;
using Bogus.Extensions.Brazil;
using HashDepot;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public StringBloomFilter? StringBloomFilter { get; private set; }
        public BloomFilter? BloomFilter { get; private set; }
        public ByteSpanBloomFilter? ByteSpanBloomFilter { get; private set; }
        public UnsafeBloomFilter? UnsafeBloomFilter { get; private set; }
        public SpanBloomFilter? SpanBloomFilter { get; private set; }

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
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"BloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!BloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new BloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.BloomFilter = filter;
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
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"UnsafeBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!UnsafeBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new UnsafeBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.UnsafeBloomFilter = filter;
            AddKeys(this.UnsafeBloomFilter);
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInBitArrayBloomFilterBenchmark), nameof(SearchInBitArrayBloomFilterBenchmark)])]
        public void BitArraySetup()
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"StringBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!StringBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new StringBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.StringBloomFilter = filter;
            AddKeys(this.StringBloomFilter);
            PostSetup();
        }

        [GlobalSetup(Targets = [nameof(AddInBoolSpanBloomFilterBenchmark), nameof(SearchInBoolSpanBloomFilterBenchmark)])]
        public void BoolSpanSetup()
        {
            SetupBogus();
            var filename = Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", $"SpanBloomFilter with {this.ProbabilityOfFalsePositives}-{this.ExpectedElementsInTheFilter}.bloom");
            if (!SpanBloomFilter.TryLoad(filename, out var filter))
            {
                Console.WriteLine($"======================================  ARQUIVO '{filename}' NÃO ENCONTRADO  ======================================");
                filter = new SpanBloomFilter(this.ProbabilityOfFalsePositives, this.ExpectedElementsInTheFilter);
                AddKeys(filter);
            }
            else
                Console.WriteLine($"**************************************  ARQUIVO '{filename}' ENCONTRADO  **************************************");
            this.SpanBloomFilter = filter;
            AddKeys(this.SpanBloomFilter);
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
            return this.BloomFilter!.MaybeContains(this.Bytes!);
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
            return this.UnsafeBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Add in BitArray BloomFilter")]
        [BenchmarkCategory("Add")]
        public bool AddInBitArrayBloomFilterBenchmark()
        {
            return this.StringBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Add in Span<bool> BloomFilter")]
        [BenchmarkCategory("Add")]
        public bool AddInBoolSpanBloomFilterBenchmark()
        {
            return this.SpanBloomFilter!.MaybeContains(this.Bytes!);
        }

        [Benchmark(Description = "Search in byte[] BloomFilter", Baseline = true)]
        [BenchmarkCategory("Contains")]
        public bool SearchInByteArrayBloomFilterBenchmark()
        {
            var res = this.BloomFilter!.MaybeContains(this.Bytes!);
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
            var res = this.UnsafeBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [Benchmark(Description = "Search in BitArray BloomFilter")]
        [BenchmarkCategory("Contains")]
        public bool SearchInBitArrayBloomFilterBenchmark()
        {
            var res = this.StringBloomFilter!.MaybeContains(this.Bytes!);
            if (res) this.MaybeContains++; else this.NotContains++;
            return res;
        }

        [Benchmark(Description = "Search in Span<bool> BloomFilter")]
        [BenchmarkCategory("Contains")]
        public bool SearchInBoolSpanBloomFilterBenchmark()
        {
            var res = this.SpanBloomFilter!.MaybeContains(this.Bytes!);
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

    public enum Implementation : byte
    {
        BitArray, 
        ByteArray,
        BoolSpan
    }

    public interface IBloomFilter
    {
        void Add(ReadOnlySpan<byte> bytes);
        bool MaybeContains(ReadOnlySpan<byte> bytes);

        void Save(string filename);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class StringBloomFilter : IBloomFilter, IEquatable<StringBloomFilter>
    {
        private readonly BitArray _filter;
        private readonly int _hashingCount;

        public StringBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        {
            var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
            var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

            _filter = new BitArray(filterSize);
            _hashingCount = numberOfHashFunctions;
        }

        private StringBloomFilter(byte[] bytes, int bitsCount, int hashingCount)
        {
            _filter = new BitArray(bytes
                .SelectMany(b => b.GetBits())
                .Take(bitsCount)
                .ToArray());
            _hashingCount = hashingCount;
        }

        public void Add(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var filterSize = this._filter.Count;
            for (uint i = 0; i < hashingCount; i++)
            {
                var index = XXHash.Hash32(bytes, i) % filterSize;
                filter[(int)index] = true;
            }
        }

        public bool MaybeContains(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var filterSize = this._filter.Count;
            for (uint i = 0; i < hashingCount; i++)
            {
                var index = XXHash.Hash32(bytes, i) % filterSize;
                if (filter[(int)index] == false)
                    return false;
            }

            return true;
        }

        public bool Equals(StringBloomFilter? other)
        {
            if (other == null) return false;
            if (_hashingCount != other._hashingCount || 
                _filter.Length != other._filter.Length ||
                _filter.Count != other._filter.Count) return false;
            var length = _filter.Count;
            for (int i = 0; i < length; i++)
            {
                if (_filter[i] != other._filter[i])
                    return false;
            }
            return true;
        }

        public void Save(string filename)
        {
            byte[] ret = new byte[(_filter.Length - 1) / 8 + 1];
            _filter.CopyTo(ret, 0);
            Utils.SaveBloomFilter(_hashingCount, _filter.Count, ret, filename);
        }

        public static StringBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
            => new StringBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

        public static bool TryLoad(string filename, [NotNullWhen(true)] out StringBloomFilter? filter)
        {
            if (!File.Exists(filename))
            {
                filter = null;
                return false;
            }
            var (hashingCount, bitsCount, bytes) = Utils.ReadBloomFilter(filename);
            filter = new StringBloomFilter(bytes, bitsCount, hashingCount);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class BloomFilter : IBloomFilter, IEquatable<BloomFilter>
    {
        private readonly byte[] _filter;
        private readonly int _hashingCount;

        public BloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        {
            var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
            var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

            this._filter = new byte[(filterSize / 8) + 1];
            this._hashingCount = numberOfHashFunctions;
        }

        private BloomFilter(byte[] bytes, int hashingCount)
        {
            _filter = bytes;
            _hashingCount = hashingCount;
        }

        /// <summary>
        /// Three least significant bits
        /// </summary>
        const int THREE_LSB = 7;
        public void Add(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var size = filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var hash = XXHash.Hash32(bytes, i);
                var index = hash % size;
                var shift = (byte)(THREE_LSB & hash);
                filter[index] |= (byte)(0x1 << shift);
            }
        }

        public bool Equals(BloomFilter? other)
        {
            if (other == null) return false;
            if (_hashingCount != other._hashingCount ||
                _filter.Length != other._filter.Length ) return false;
            var length = _filter.Length;
            for (int i = 0; i < length; i++)
            {
                if (_filter[i] != other._filter[i])
                    return false;
            }
            return true;
        }

        public bool MaybeContains(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var size = filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var hash = XXHash.Hash32(bytes, i);
                var index = hash % size;
                var shift = (byte)(hash % 8);
                var value = (byte)(0x1 << shift);
                if ((filter[index] & value) != value)
                    return false;
            }

            return true;
        }

        public void Save(string filename)
        {
            Utils.SaveBloomFilter(_hashingCount, _filter.Length * 8, _filter, filename);
        }

        public static BloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
            => new BloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

        public static bool TryLoad(string filename, [NotNullWhen(true)] out BloomFilter? filter)
        {
            if (!File.Exists(filename))
            {
                filter = null;
                return false;
            }
            var (hashingCount, _, bytes) = Utils.ReadBloomFilter(filename);
            filter = new BloomFilter(bytes, hashingCount);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class ByteSpanBloomFilter : IBloomFilter, IEquatable<ByteSpanBloomFilter>
    {
        private readonly byte[] _filter;
        private readonly int _hashingCount;

        public ByteSpanBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        {
            var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
            var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

            this._filter = new byte[(filterSize / 8) + 1];
            this._hashingCount = numberOfHashFunctions;
        }

        private ByteSpanBloomFilter(byte[] bytes, int hashingCount)
        {
            _filter = bytes;
            _hashingCount = hashingCount;
        }

        /// <summary>
        /// Three least significant bits
        /// </summary>
        const int THREE_LSB = 7;
        public void Add(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            Span<byte> filter = this._filter;
            var size = filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var hash = XXHash.Hash32(bytes, i);
                var index = hash % size;
                var shift = (byte)(THREE_LSB & hash);
                filter[(int)index] |= (byte)(0x1 << shift);
            }
        }

        public bool MaybeContains(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var size = filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var hash = XXHash.Hash32(bytes, i);
                var index = hash % size;
                var shift = (byte)(hash % 8);
                var value = (byte)(0x1 << shift);
                if ((filter[index] & value) != value)
                    return false;
            }

            return true;
        }

        public bool Equals(ByteSpanBloomFilter? other)
        {
            if (other == null) return false;
            if (_hashingCount != other._hashingCount ||
                _filter.Length != other._filter.Length) return false;
            var length = _filter.Length;
            for (int i = 0; i < length; i++)
            {
                if (_filter[i] != other._filter[i])
                    return false;
            }
            return true;
        }

        public void Save(string filename)
        {
            Utils.SaveBloomFilter(_hashingCount, _filter.Length * 8, _filter, filename);
        }

        public static ByteSpanBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
            => new ByteSpanBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

        public static bool TryLoad(string filename, [NotNullWhen(true)] out ByteSpanBloomFilter? filter)
        {
            if (!File.Exists(filename))
            {
                filter = null;
                return false;
            }
            var (hashingCount, _, bytes) = Utils.ReadBloomFilter(filename);
            filter = new ByteSpanBloomFilter(bytes, hashingCount);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class UnsafeBloomFilter : IBloomFilter, IEquatable<UnsafeBloomFilter>
    {
        private readonly byte[] _filter;
        private readonly int _hashingCount;

        public UnsafeBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        {
            var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
            var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

            this._filter = new byte[(filterSize / 8) + 1];
            this._hashingCount = numberOfHashFunctions;
        }

        private UnsafeBloomFilter(byte[] bytes, int hashingCount)
        {
            _filter = bytes;
            _hashingCount = hashingCount;
        }

        /// <summary>
        /// Three least significant bits
        /// </summary>
        const int THREE_LSB = 7;
        public void Add(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            ref var first = ref this._filter[0];
            var size = this._filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var hash = XXHash.Hash32(bytes, i);
                var index = hash % size;
                var shift = (byte)(THREE_LSB & hash);
                Unsafe.Add(ref first, (int)index) |= (byte)(0x1 << shift);
            }
        }

        public bool MaybeContains(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var size = filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var hash = XXHash.Hash32(bytes, i);
                var index = hash % size;
                var shift = (byte)(hash % 8);
                var value = (byte)(0x1 << shift);
                if ((filter[index] & value) != value)
                    return false;
            }

            return true;
        }

        public bool Equals(UnsafeBloomFilter? other)
        {
            if (other == null) return false;
            if (_hashingCount != other._hashingCount ||
                _filter.Length != other._filter.Length) return false;
            var length = _filter.Length;
            for (int i = 0; i < length; i++)
            {
                if (_filter[i] != other._filter[i])
                    return false;
            }
            return true;
        }

        public void Save(string filename)
        {
            Utils.SaveBloomFilter(_hashingCount, _filter.Length * 8, _filter, filename);
        }

        public static UnsafeBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
            => new UnsafeBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

        public static bool TryLoad(string filename, [NotNullWhen(true)] out UnsafeBloomFilter? filter)
        {
            if (!File.Exists(filename))
            {
                filter = null;
                return false;
            }
            var (hashingCount, _, bytes) = Utils.ReadBloomFilter(filename);
            filter = new UnsafeBloomFilter(bytes, hashingCount);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class SpanBloomFilter : IBloomFilter, IEquatable<SpanBloomFilter>
    {
        private readonly Memory<bool> _filter;
        private readonly int _hashingCount;

        public SpanBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        {
            var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
            var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

            this._filter = new Memory<bool>(new bool[filterSize]);
            this._hashingCount = numberOfHashFunctions;
        }

        private SpanBloomFilter(int hashingCount, int bitsCount, byte[] bytes)
        {
            _filter = new Memory<bool>(bytes
                .SelectMany(b => b.GetBits())
                .Take(bitsCount)
                .ToArray());
            _hashingCount = hashingCount;
        }

        public void Add(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var filterSize = this._filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var index = XXHash.Hash32(bytes, i) % filterSize;
                filter.Span[(int)index] = true;
            }
        }

        public bool Equals(SpanBloomFilter? other)
        {
            if (other == null) return false;
            if (_hashingCount != other._hashingCount ||
                _filter.Length != other._filter.Length) return false;
            var length = _filter.Length;
            for (int i = 0; i < length; i++)
            {
                if (_filter.Span[i] != other._filter.Span[i])
                    return false;
            }
            return true;
        }

        public bool MaybeContains(ReadOnlySpan<byte> bytes)
        {
            var hashingCount = this._hashingCount;
            var filter = this._filter;
            var filterSize = this._filter.Length;
            for (uint i = 0; i < hashingCount; i++)
            {
                var index = XXHash.Hash32(bytes, i) % filterSize;
                if (filter.Span[(int)index] == false)
                    return false;
            }

            return true;
        }

        public void Save(string filename)
        {
            var bytes = new byte[_filter.Span.Length % 8 == 0 ? _filter.Span.Length / 8 : (_filter.Span.Length / 8) + 1];
            var slice = _filter.Span;

            var length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                var newSlice = slice.Length >= 8 ? slice.Slice(0, 8) : slice;
                bytes[i] = Utils.GetByte(newSlice);
                slice = slice.Slice(newSlice.Length);
            }
            Utils.SaveBloomFilter(_hashingCount, _filter.Length, bytes, filename);
        }

        public static SpanBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
            => new SpanBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

        public static bool TryLoad(string filename, [NotNullWhen(true)] out SpanBloomFilter? filter)
        {
            if (!File.Exists(filename))
            {
                filter = null;
                return false;
            }
            var (hashingCount, bitsCount, bytes) = Utils.ReadBloomFilter(filename);
            filter = new SpanBloomFilter(hashingCount, bitsCount, bytes);
            return true;
        }
    }
}
