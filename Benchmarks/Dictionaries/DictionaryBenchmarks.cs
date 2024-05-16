using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Benchmarks.Dictionaries
{
    [Description("Search on dictionary with 60 entries")]
    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
    [MemoryDiagnoser(true)]
    public class DictionaryBenchmarks
    {
        //#region static fields
        //private static readonly Dictionary<byte[], DictionaryValue> OldDictionary = Enumerable
        //    .Range(1, 60)
        //    .Select(idx => (Key: Guid.NewGuid().ToByteArray(), Value: new DictionaryValue($"value {idx}")))
        //    .ToDictionary(t => t.Key, t => t.Value);

        //private static readonly Dictionary<ReadOnlyMemory<byte>, DictionaryValue> NewDictionary = OldDictionary
        //    .Select(t => (Key: new ReadOnlyMemory<byte>(t.Key), t.Value))
        //    .ToDictionary(t => t.Key, t => t.Value);

        //private static readonly Dictionary<string, DictionaryValue> StringDictionary = OldDictionary
        //    .Select(t => (Key: new Guid(t.Key).ToString(), t.Value))
        //    .ToDictionary(t => t.Key, t => t.Value);

        //private static readonly ImmutableArray<byte[]> Keys =
        //    ImmutableArray<byte[]>
        //        .Empty
        //        .Add(OldDictionary.Keys.First())
        //        .Add(OldDictionary.Keys.Skip(2).First())
        //        .Add(OldDictionary.Keys.Skip(5).First())
        //        .Add(OldDictionary.Keys.Skip(10).First())
        //        .Add(OldDictionary.Keys.Skip(19).First())
        //        .Add(OldDictionary.Keys.Skip(39).First())
        //        .Add(OldDictionary.Keys.Skip(59).First());
        //#endregion

        //private string entryPosition;

        //[Params("1", "2", "5", "10", "20", "40", "60", "Not in dictionary")]
        //public string EntryPosition 
        //{ 
        //    get => this.EntryPosition;
        //    set
        //    {
        //        this.entryPosition = value;
        //        this.Key = this.entryPosition switch
        //        {
        //            "1" => Keys[0],
        //            "2" => Keys[1],
        //            "5" => Keys[2],
        //            "10" => Keys[3],
        //            "20" => Keys[4],
        //            "40" => Keys[5],
        //            "60" => Keys[6],
        //            _ => Guid.NewGuid().ToByteArray()
        //        };
        //    }
        //}

        //public byte[] Key { get; private set; }

        //[Benchmark(Description = "Current way: Iterating on all entries", Baseline = true)]
        //[BenchmarkCategory("Search")]
        //public DictionaryValue? OldWay()
        //{
        //    var key = this.Key.AsSpan();
        //    DictionaryValue? value = null;

        //    foreach (var tuple in OldDictionary)
        //    {
        //        var currentKey = tuple.Key.AsSpan();
        //        if (currentKey.SequenceEqual(key))
        //        {
        //            value = tuple.Value;
        //        }
        //    }
        //    return value;
        //}

        //[Benchmark(Description = "Simple optimization: Iterating until not find and break after")]
        //[BenchmarkCategory("Search")]
        //public DictionaryValue? IntermediateWay()
        //{
        //    var key = this.Key.AsSpan();
        //    DictionaryValue? value = null;

        //    foreach (var tuple in OldDictionary)
        //    {
        //        var currentKey = tuple.Key.AsSpan();
        //        if (currentKey.SequenceEqual(key))
        //        {
        //            value = tuple.Value;
        //            break;
        //        }
        //    }
        //    return value;
        //}

        //[Benchmark(Description = "Bestest way: Create a ReadOnlyMemory and use TryGetValue")]
        //[BenchmarkCategory("Search")]
        //public DictionaryValue? NewWay()
        //{
        //    var key = new ReadOnlyMemory<byte>(this.Key.AsSpan().ToArray());
        //    NewDictionary.TryGetValue(key, out var value);
        //    return value;
        //}

        //[Benchmark(Description = "Bestest way: Create a string and use TryGetValue")]
        //[BenchmarkCategory("Search")]
        //public DictionaryValue? StringWay()
        //{
        //    var key = new Guid(this.Key).ToString();
        //    StringDictionary.TryGetValue(key, out var value);
        //    return value;
        //}

        public Random Random { get; private set; }
        public Dictionary<int, DictionaryValue> Dictionary { get; private set; }

        [GlobalSetup] 
        public void Setup()
        {
            this.Random = new Random(568531);
            this.Dictionary = new Dictionary<int, DictionaryValue>(100_000);
        }

        [Benchmark(Description = "Common way", Baseline = true)]
        [BenchmarkCategory("Add")]
        public void Add() 
        {
            var key = this.Random.Next();
            if (!this.Dictionary.ContainsKey(key))
                this.Dictionary.Add(key, new DictionaryValue("Test"));
        }

        [Benchmark(Description = "Using this[]")]
        [BenchmarkCategory("Add")]
        public void This()
        {
            this.Dictionary[this.Random.Next()] = new DictionaryValue("Test");
        }

        [Benchmark(Description = "Using TryAdd")]
        [BenchmarkCategory("Add")]
        public void TryAdd()
        {
            this.Dictionary.TryAdd(this.Random.Next(), new DictionaryValue("Test"));
        }

        [Benchmark(Description = "Using CollectionsMarshal")]
        [BenchmarkCategory("Add")]
        public void UsinhCollectionsMarshal()
        {
            ref var reference = ref CollectionsMarshal.GetValueRefOrAddDefault(this.Dictionary, this.Random.Next(), out bool exists);
            if (!exists)
                reference = new DictionaryValue("Test");
        }
    }

    public sealed class DictionaryValue(string name)
    {
        public readonly string Name = name;
    }
}
