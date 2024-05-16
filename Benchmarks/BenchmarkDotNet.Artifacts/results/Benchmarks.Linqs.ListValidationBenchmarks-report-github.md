```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.4291/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-DFEXHE : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


```
| Description    | Params | Method          | Mean         | StdDev      | Error       | Ratio | Gen0   | Allocated |
|--------------- |------- |---------------- |-------------:|------------:|------------:|------:|-------:|----------:|
| ListValidation | 5      | Bestest         |     6.241 ns |   0.3277 ns |   0.4955 ns |  0.03 |      - |         - |
| ListValidation | 5      | Best            |     9.707 ns |   0.8310 ns |   1.2563 ns |  0.04 |      - |         - |
| ListValidation | 5      | New             |   109.046 ns |   9.2558 ns |  13.9934 ns |  0.50 | 0.0841 |     528 B |
| ListValidation | 5      | &#39;Using HashSet&#39; |   147.959 ns |  15.0084 ns |  22.6905 ns |  0.68 | 0.0408 |     256 B |
| ListValidation | 5      | Current         |   224.007 ns |  27.1527 ns |  41.0510 ns |  1.00 | 0.1743 |    1096 B |
|                |        |                 |              |             |             |       |        |           |
| ListValidation | 12     | Bestest         |    43.999 ns |   4.6748 ns |   7.0677 ns |  0.07 |      - |         - |
| ListValidation | 12     | Best            |    66.017 ns |   7.0087 ns |  10.5962 ns |  0.10 |      - |         - |
| ListValidation | 12     | New             |   333.840 ns |  24.5147 ns |  41.1953 ns |  0.52 | 0.1821 |    1144 B |
| ListValidation | 12     | &#39;Using HashSet&#39; |   337.721 ns |  29.3747 ns |  44.4103 ns |  0.52 | 0.0723 |     456 B |
| ListValidation | 12     | Current         |   652.326 ns |  51.9892 ns |  78.6003 ns |  1.00 | 0.3975 |    2496 B |
|                |        |                 |              |             |             |       |        |           |
| ListValidation | 18     | Bestest         |   109.328 ns |  11.6592 ns |  17.6270 ns |  0.10 |      - |         - |
| ListValidation | 18     | Best            |   161.962 ns |  17.7971 ns |  26.9067 ns |  0.14 |      - |         - |
| ListValidation | 18     | &#39;Using HashSet&#39; |   429.416 ns |  12.2768 ns |  18.5608 ns |  0.38 | 0.0918 |     576 B |
| ListValidation | 18     | New             |   573.586 ns |   9.4946 ns |  15.9551 ns |  0.50 | 0.2656 |    1672 B |
| ListValidation | 18     | Current         | 1,160.653 ns | 163.5009 ns | 247.1900 ns |  1.00 | 0.5879 |    3696 B |
