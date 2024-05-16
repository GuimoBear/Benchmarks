```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.4291/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-KXICYI : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


```
| Description                          | Method                     | Mean     | StdDev   | Error     | Ratio | Gen0   | Gen1   | Allocated |
|------------------------------------- |--------------------------- |---------:|---------:|----------:|------:|-------:|-------:|----------:|
| Search on dictionary with 60 entries | &#39;Using CollectionsMarshal&#39; | 181.8 ns |  7.66 ns |  14.64 ns |  0.77 | 0.0037 | 0.0017 |      23 B |
| Search on dictionary with 60 entries | &#39;Using TryAdd&#39;             | 193.1 ns | 19.87 ns |  33.40 ns |  0.80 | 0.0037 | 0.0017 |      24 B |
| Search on dictionary with 60 entries | &#39;Common way&#39;               | 237.9 ns | 19.43 ns |  37.16 ns |  1.00 | 0.0037 | 0.0017 |      23 B |
| Search on dictionary with 60 entries | &#39;Using this[]&#39;             | 440.9 ns | 65.41 ns | 109.92 ns |  1.80 | 0.0037 | 0.0017 |      24 B |
