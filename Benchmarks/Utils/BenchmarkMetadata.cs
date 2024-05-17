using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Benchmarks.Utils
{
    public class BenchmarkMetadata
    {
        public static BenchmarkMetadata Instance { get; } = new BenchmarkMetadata();

        private Dictionary<string, Dictionary<string, Dictionary<string, object>>> _Metadata = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
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
                _Metadata = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(json);
            }
        }
        private void Save(string benchmarkName)
        {
            // directory
            string directory = Path.Combine(_DocumentsPath, _BenchmarkDirectoryName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // file
            string fileName = Path.Combine(directory, $"{benchmarkName}.json");
            string json = JsonSerializer.Serialize(_Metadata);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        public void AddMetadata<TBenchmark>(TBenchmark instance, string name, string benchmarkName, object value)
        {
            Load(typeof(TBenchmark).Name);
            ref var benchmarks = ref CollectionsMarshal.GetValueRefOrAddDefault(_Metadata, name, out bool exists);
            if (!exists)
                benchmarks = new Dictionary<string, Dictionary<string, object>>();

            var strParameters = GetParameters(instance);
            ref var parameters = ref CollectionsMarshal.GetValueRefOrAddDefault(benchmarks, benchmarkName, out exists);
            if (!exists)
                parameters = new Dictionary<string, object>();
            parameters[strParameters] = value;
            Save(typeof(TBenchmark).Name);
        }

        public object GetMetdata(string name, string benchmarkName, string strParameters)
        {
            if (_Metadata.TryGetValue(name, out var benchmarks) &&
                benchmarks.TryGetValue(benchmarkName, out var parameters) &&
                parameters.TryGetValue(strParameters, out var obj))
                return obj;
            return null;
        }

        private static ConcurrentDictionary<Type, Func<object, string>> CachedParametersGet
            = new ConcurrentDictionary<Type, Func<object, string>>();

        private static string GetParameters<TBenchmark>(TBenchmark instance)
        {
            var parametersGetter = CachedParametersGet.GetOrAdd(instance!.GetType(), type =>
            {
                var propertiesWithArguments = type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(pi => pi.GetCustomAttributes<ParamsAttribute>()?.Any() == true &&
                                 pi.CanRead &&
                                 pi.GetGetMethod() is not null)
                    .ToList();

                if (propertiesWithArguments.Count == 0)
                    return Parameterless;

                var methodBuilder = new DynamicMethod(
                    $"{type.Name}ParametersString",
                    MethodAttributes.Public | MethodAttributes.Static,
                    CallingConventions.Standard,
                    typeof(string),
                    new Type[] { typeof(object) },
                    typeof(BenchmarkMetadata),
                    true);

                methodBuilder.DefineParameter(1, ParameterAttributes.None, "instance");
                
                var cil = methodBuilder.GetILGenerator();

                cil.DeclareLocal(type);

                // => var loc = instance as TBenchmark;
                cil.Emit(OpCodes.Ldarg_0);
                cil.Emit(OpCodes.Isinst, type);
                cil.Emit(OpCodes.Stloc_0);
                // <= var loc = instance as TBenchmark;

                // => var dict = new Dictionary<string, object>(propertiesWithArguments.Count);
                EmitInt(cil, propertiesWithArguments.Count);
                cil.Emit(OpCodes.Newobj, DictionaryConstructorInfo);
                // <= var dict = new Dictionary<string, object>(propertiesWithArguments.Count);
                for (int i = 0; i < propertiesWithArguments.Count; i++ )
                {
                    var pi = propertiesWithArguments[i];

                    // => dict.Add(pi.Name, pi.Value);
                    cil.Emit(OpCodes.Dup);
                    cil.Emit(OpCodes.Ldstr, pi.Name);
                    cil.Emit(OpCodes.Ldloc_0);
                    cil.Emit(OpCodes.Callvirt, pi.GetGetMethod()!);
                    if (pi.PropertyType != typeof(string) && pi.PropertyType.IsValueType)
                        cil.Emit(OpCodes.Box, pi.PropertyType);
                    cil.Emit(OpCodes.Callvirt, DictionaryAddMethodInfo);
                    // <= dict.Add(pi.Name, pi.Value);
                }
                // => return ParametersToString(dict);
                cil.Emit(OpCodes.Call, ParametersToStringMethodInfo);
                cil.Emit(OpCodes.Ret);
                // <= return ParametersToString(dict);

                return methodBuilder.CreateDelegate<Func<object, string>>();
            });

            return parametersGetter(instance!);
        }

        private static string ParametersToString(IReadOnlyDictionary<string, object> parameters)
            => string.Join('&', parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        private static void EmitInt(ILGenerator cil, int size)
        {
            var code = size switch
            {
                0 => OpCodes.Ldc_I4_0,
                1 => OpCodes.Ldc_I4_1,
                2 => OpCodes.Ldc_I4_2,
                3 => OpCodes.Ldc_I4_3,
                4 => OpCodes.Ldc_I4_4,
                5 => OpCodes.Ldc_I4_5,
                6 => OpCodes.Ldc_I4_6,
                7 => OpCodes.Ldc_I4_7,
                8 => OpCodes.Ldc_I4_8,
                _ => OpCodes.Ldc_I4_S
            };

            if (code == OpCodes.Ldc_I4_S)
                cil.Emit(code, size);
            else
                cil.Emit(code);
        }

        private static readonly ConstructorInfo DictionaryConstructorInfo = typeof(Dictionary<string, object>).GetConstructor(new Type[] { typeof(int) })!;
        private static readonly MethodInfo DictionaryAddMethodInfo = typeof(Dictionary<string, object>).GetMethod(nameof(Dictionary<string, object>.Add), BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(string), typeof(object) })!;
        private static readonly MethodInfo ParametersToStringMethodInfo = typeof(BenchmarkMetadata).GetMethod(nameof(ParametersToString), BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(IReadOnlyDictionary<string, object>) })!;

        private static string Parameterless(object _)
            => "";
    }
}
