```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.4170/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-HPRWAO : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


```
| Description         | Method           | Mean         | StdDev     | Error      | Allocated |
|-------------------- |----------------- |-------------:|-----------:|-----------:|----------:|
| Sort implementation | NativeSort       |     50.02 μs |   3.407 μs |   5.150 μs |         - |
| Sort implementation | Quicksort        |     82.85 μs |   1.706 μs |   2.867 μs |         - |
| Sort implementation | OptimalQuicksort |     98.35 μs |  21.767 μs |  32.909 μs |         - |
| Sort implementation | Sort             | 11,761.49 μs | 110.858 μs | 167.601 μs |       1 B |
| Sort implementation | DefaultSort      | 12,613.15 μs | 285.741 μs | 480.169 μs |       1 B |
