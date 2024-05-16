```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.4291/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-WAHIBZ : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


```
| Description | Method                                                          | Mean      | StdDev    | Error     | Ratio | Allocated |
|------------ |---------------------------------------------------------------- |----------:|----------:|----------:|------:|----------:|
| MethodImpl  | &#39;With AggressiveOptimization MethodImpl&#39;                        |  5.701 μs | 0.1690 μs | 0.2555 μs |  0.99 |         - |
| MethodImpl  | &#39;Without MethodImpl&#39;                                            |  5.811 μs | 0.5490 μs | 0.8300 μs |  1.00 |         - |
| MethodImpl  | &#39;With AggressiveInlining MethodImpl&#39;                            |  5.875 μs | 0.6031 μs | 0.9118 μs |  1.02 |         - |
| MethodImpl  | &#39;With AggressiveInlining and AggressiveOptimization MethodImpl&#39; |  5.948 μs | 0.2039 μs | 0.3427 μs |  1.03 |         - |
| MethodImpl  | &#39;With NoInlining MethodImpl&#39;                                    | 10.589 μs | 1.3628 μs | 2.0604 μs |  1.83 |         - |
| MethodImpl  | &#39;With NoInlining and NoOptimization MethodImpl&#39;                 | 11.834 μs | 1.0687 μs | 1.6157 μs |  2.04 |         - |
