using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Benchmarks.Utils
{
    public class BenchmarkMetadata
    {
        public static BenchmarkMetadata Instance { get; } = new BenchmarkMetadata();

        private Dictionary<string, SortedList<int, object>> _Metadata = new Dictionary<string, SortedList<int, object>>();
        private string _DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private const string _BenchmarkDirectoryName = "Custom Benchmark Data";

        public void Load(string benchmarkName)
        {
            // directory
            string directory = Path.Combine(_DocumentsPath, _BenchmarkDirectoryName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // file
            string fileName = Path.Combine(directory, $"{benchmarkName}.json");
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                _Metadata = JsonSerializer.Deserialize<Dictionary<string, SortedList<int, object>>>(json);
            }
        }
        public void Save(string benchmarkName)
        {
            // directory
            string directory = Path.Combine(_DocumentsPath, _BenchmarkDirectoryName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // file
            string fileName = Path.Combine(directory, $"{benchmarkName}.json");
            string json = JsonSerializer.Serialize(_Metadata);
            File.WriteAllText(fileName, json);
        }

        public void AddMetadata(string name, object value)
        {
            ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(_Metadata, name, out bool exists);
            if (!exists)
                list = new SortedList<int, object>();
            list.Add(list.Count, value);
        }

        public SortedList<int, object> GetMetdata(string name)
        {
            if (_Metadata.TryGetValue(name, out var values))
                return values;
            return null;
        }
    }
}
