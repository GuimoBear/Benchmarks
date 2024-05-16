using BenchmarkDotNet.Attributes;
using Benchmarks.Helpers.PooledObjects;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Benchmarks.StringBuilders
{
    [Description("StringBuilder tests")]
    [MemoryDiagnoser]
    public class StringBuilderBenchmarks
    {
        private static readonly List<List<string>> Strings = new List<List<string>>
            {
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus laoreet nulla faucibus sollicitudin condimentum. Donec vehicula blandit malesuada. Maecenas eleifend leo vitae vehicula congue. Morbi sed varius ante. Suspendisse vitae nunc vehicula, mattis mauris sed, tincidunt mauris. Morbi et massa ut mi ultricies lacinia. Maecenas mattis ex id tempor malesuada. Sed vulputate, purus nec facilisis semper, ex felis feugiat nulla, et sodales mi lectus quis est. Aenean gravida cursus feugiat. Cras consequat auctor fermentum".Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                "Sed ac lorem vestibulum, dictum augue eu, dapibus tortor. Morbi sed finibus diam. Aliquam id interdum enim, vel euismod massa. Integer viverra sollicitudin tortor non auctor. In ut hendrerit odio, non ultricies sapien. Nam rutrum consectetur magna. Mauris rhoncus ipsum odio, vel efficitur elit facilisis vitae. Curabitur at mauris sed nunc imperdiet vestibulum. Nam sodales, sem vel semper vehicula, libero dolor vehicula diam, non rhoncus est arcu vitae augue".Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                "Proin urna dolor, volutpat sit amet semper at, ultrices eu libero. Sed tempus semper imperdiet. Quisque ac gravida lectus. Sed sit amet orci sagittis, sollicitudin ligula a, tempus nisi. Donec vehicula nisl ut nisl cursus iaculis. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce imperdiet nec sem nec tristique. Quisque vel purus eget ex venenatis luctus. Praesent volutpat in velit a hendrerit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Interdum et malesuada fames ac ante ipsum primis in faucibus. Etiam vel venenatis mi, ut porta velit. Sed vitae tristique quam, quis elementum quam".Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                "Aenean posuere nibh eget efficitur tincidunt. Mauris et fermentum ligula, vel scelerisque sapien. Donec odio nisi, pretium ut vulputate id, congue et ipsum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Nunc sodales mi vitae quam pharetra vulputate. Mauris mollis ipsum in nibh rutrum, a gravida orci vehicula. Nam sit amet massa malesuada, ultrices ex et, pulvinar diam. Sed maximus, mi vel posuere tempor, dui tortor dictum justo, commodo semper nunc metus sit amet libero. Duis aliquam, velit lacinia maximus egestas, elit est scelerisque ex, sed maximus erat orci a elit. Aliquam accumsan dolor libero, in viverra risus interdum non".Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                "Praesent quis ipsum nibh. Duis tempor eleifend mi elementum rhoncus. Vivamus eget mi nisl. Pellentesque tempus purus felis. Fusce ac bibendum nunc. Mauris ut felis scelerisque, ultrices metus sit amet, fermentum tortor. Vivamus in ultrices enim. Nulla vestibulum nulla eget lorem sodales, sed efficitur nibh suscipit. Nullam iaculis interdum justo nec malesuada. Nunc suscipit sit amet eros a semper. Nunc lorem sapien, eleifend vitae commodo a, porttitor gravida tortor. Fusce a lacus lectus. Sed aliquam, eros sit amet vulputate laoreet, urna leo accumsan tortor, id imperdiet lacus justo eget sem. Donec porta urna dolor. Sed consequat suscipit tempor. Integer vehicula lacus vitae tortor finibus condimentum id et lorem".Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
            };


        [Benchmark(Description = "Using only one StringBuilder", Baseline = true)]
        public DateTime CurrentWay()
        {
            var sb = new StringBuilder();
            for(int i = 0; i < Strings.Count; i++)
            {
                _ = Join(Strings[i], sb);
                sb.Clear();
            }
            return DateTime.UtcNow;
        }

        [Benchmark(Description = "Using one StringBuilder per yield return")]
        public DateTime WorsestWay()
        {
            for (int i = 0; i < Strings.Count; i++)
            {
                var sb = new StringBuilder();
                _ = Join(Strings[i], sb);
            }
            return DateTime.UtcNow;
        }

        [Benchmark(Description = "Using PooledStringBuilder per yield return")]
        public DateTime PooledPerYieldWay()
        {
            for (int i = 0; i < Strings.Count; i++)
            {
                var pooled = PooledStringBuilder.GetInstance();
                try
                {
                    _ = Join(Strings[i], pooled.Builder);
                }
                finally
                {
                    pooled.Free();
                }
            }
            return DateTime.UtcNow;
        }

        [Benchmark(Description = "Using PooledStringBuilder")]
        public DateTime PooledWay()
        {
            var pooled = PooledStringBuilder.GetInstance();
            try
            {
                for (int i = 0; i < Strings.Count; i++)
                {
                    _ = Join(Strings[i], pooled.Builder);
                    pooled.Builder.Clear();
                }
            }
            finally
            {
                pooled.Free();
            }
            return DateTime.UtcNow;
        }

        private static string Join(List<string> data, StringBuilder sb)
        {
            var span = CollectionsMarshal.AsSpan(data);
            for (int i = 0; i < span.Length; i++)
            {
                sb.Append(span[i]);
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }
}
