using BenchmarkDotNet.Attributes;
//using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Benchmarks.Strings;

[Description("Validate array of days")]
[MemoryDiagnoser]
public partial class ArrayOfDaysValidationBenchmarks
{
    [Params(
        "[1]",
        "   [   1   ]   ",
        "[1,2]",
        "[   1   ,   2   ]",
        "[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31]"
        )]
    public string Value { get; set; }

    private Regex ValidArrayOfDays;

    [GeneratedRegex(@"^ *\[ *(?:[1-9]|[1-2]\d|[3][0-1])(?: *, *(?:[1-9]|[1-2]\d|[3][0-1]))* *\] *$", RegexOptions.Compiled)]
    private static partial Regex ValidArrayOfDaysRegex();

    [GlobalSetup]
    public void Setup()
    {
        ValidArrayOfDays = new Regex(@"^ *\[ *(?:[1-9]|[1-2]\d|[3][0-1])(?: *, *(?:[1-9]|[1-2]\d|[3][0-1]))* *\] *$", RegexOptions.Compiled);
        
        ValidArrayOfDays.IsMatch(Value);
        ValidArrayOfDaysRegex().IsMatch(Value);
    }

    [Benchmark(Description = "using regex", Baseline = true)]
    public bool UsingRegex()
        => ValidArrayOfDays.IsMatch(Value);

    [Benchmark(Description = "using source generation regex")]
    public bool UsingSourceGenerationRegex()
        => ValidArrayOfDaysRegex().IsMatch(Value);

    //[Benchmark(Description = "using newtonsoft deserialization")]
    //public bool UsingNewtonsoft()
    //{
    //    var array = JsonConvert.DeserializeObject<int[]>(Value)!;
    //    return Array.TrueForAll(array, day => day > 0 && day < 32);
    //}

    [Benchmark(Description = "using System.Text deserialization")]
    public bool UsingSystemText()
    {
        var array = System.Text.Json.JsonSerializer.Deserialize<int[]>(Value)!;
        return Array.TrueForAll(array, day => day > 0 && day < 32);
    }

    [Benchmark(Description = "using System.Text source generation deserializer")]
    public bool UsingSystemTextSourceGenerator()
    {
        var array = System.Text.Json.JsonSerializer.Deserialize(Value, SourceGenerationContext.Default.Int32Array)!;
        return Array.TrueForAll(array, day => day > 0 && day < 32);
    }
}

[JsonSourceGenerationOptions()]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(int))]
internal partial class SourceGenerationContext : JsonSerializerContext
{

}