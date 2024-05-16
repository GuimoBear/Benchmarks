using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Benchmarks.Strings
{
    [MemoryDiagnoser(true)]
    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
    public class StringBenchmarks
    {
        public DateTime Date { get; private set; }

        [GlobalSetup()] 
        public void Setup() 
        {
            this.Date = new DateTime(2016, 08, 03);
        }

        [Benchmark(Description = "ToString(\"yyyy-MM-dd\")", Baseline = true)]
        [BenchmarkCategory("Stringify")]
        public void UsingToString()
            => this.Date.ToString("yyyy-MM-dd");

        [Benchmark(Description = "ToCoreString()")]
        [BenchmarkCategory("Stringify")]
        public void UsingToCoreString()
            => this.Date.ToCoreString();
    }

    public static class DateTimeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToCoreString(this DateTime datetime)
        {
            Span<char> chars = stackalloc char[10];
            var index = WriteYear(chars, 0, datetime.Year);
            chars[index++] = '-';
            index = WritePart(chars, index, datetime.Month);
            chars[index++] = '-';
            index = WritePart(chars, index, datetime.Day);
            return new string(chars);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static int WriteYear(Span<char> chars, int index, int year)
        {
            var result = Math.DivRem(year, 1_000, out var remaining);
            WriteDigit(chars, index++, result);
            result = Math.DivRem(remaining, 100, out remaining);
            WriteDigit(chars, index++, result);
            result = Math.DivRem(remaining, 10, out remaining);
            WriteDigit(chars, index++, result);
            WriteDigit(chars, index++, remaining);
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static int WritePart(Span<char> chars, int index, int part)
        {
            var result = Math.DivRem(part, 10, out var remaining);
            WriteDigit(chars, index++, result);
            WriteDigit(chars, index++, remaining);
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void WriteDigit(Span<char> chars, int index, int number)
        {
            chars[index] = (char)('0' + number);
        }
    }
}
