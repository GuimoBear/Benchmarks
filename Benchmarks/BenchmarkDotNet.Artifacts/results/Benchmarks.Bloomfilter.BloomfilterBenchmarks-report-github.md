```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.4291/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-OKHWTQ : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


```
| Description | Params          | Method                                | Mean      | StdDev    | Error     | Ratio | Iterations count | Maybe exists | Not exists | Allocated |
|------------ |---------------- |-------------------------------------- |----------:|----------:|----------:|------:|-----------------:|-------------:|-----------:|----------:|
| Bloomfilter | 0,005, 5000000  | &#39;Add in byte[] BloomFilter&#39;           | 113.54 ns |  6.210 ns |  40.13 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Add in Span&lt;byte&gt; BloomFilter&#39;       | 114.88 ns | 13.561 ns |  52.22 ns |  0.97 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Add in BitArray BloomFilter&#39;         | 133.20 ns | 12.501 ns |  48.14 ns |  1.19 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Add in Unsafe byte[] BloomFilter&#39;    | 137.37 ns |  9.874 ns |  38.02 ns |  1.23 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Add in Span&lt;bool&gt; BloomFilter&#39;       | 147.29 ns | 18.190 ns |  70.04 ns |  1.26 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,005, 10000000 | &#39;Add in byte[] BloomFilter&#39;           | 126.05 ns |  5.888 ns |  38.05 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Add in Unsafe byte[] BloomFilter&#39;    | 138.35 ns | 17.863 ns |  68.78 ns |  1.10 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Add in Span&lt;bool&gt; BloomFilter&#39;       | 140.76 ns | 16.677 ns |  64.22 ns |  1.15 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Add in BitArray BloomFilter&#39;         | 146.91 ns | 24.745 ns |  95.28 ns |  1.14 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Add in Span&lt;byte&gt; BloomFilter&#39;       | 165.36 ns | 18.682 ns |  71.94 ns |  1.30 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,05, 5000000   | &#39;Add in Unsafe byte[] BloomFilter&#39;    |  72.35 ns |  7.188 ns |  27.68 ns |  0.93 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Add in Span&lt;bool&gt; BloomFilter&#39;       |  74.95 ns |  8.538 ns |  32.88 ns |  0.96 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Add in Span&lt;byte&gt; BloomFilter&#39;       |  77.17 ns |  9.295 ns |  35.79 ns |  0.98 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Add in byte[] BloomFilter&#39;           |  78.23 ns |  3.230 ns |  12.44 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Add in BitArray BloomFilter&#39;         |  78.74 ns | 15.140 ns |  58.30 ns |  1.01 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,05, 10000000  | &#39;Add in Span&lt;byte&gt; BloomFilter&#39;       |  64.85 ns |  3.850 ns |  24.88 ns |  0.81 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Add in Span&lt;bool&gt; BloomFilter&#39;       |  70.19 ns |  9.733 ns |  37.48 ns |  0.91 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Add in BitArray BloomFilter&#39;         |  71.41 ns |  5.364 ns |  34.66 ns |  0.90 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Add in Unsafe byte[] BloomFilter&#39;    |  77.54 ns | 18.075 ns |  69.60 ns |  0.99 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Add in byte[] BloomFilter&#39;           |  78.12 ns | 12.181 ns |  46.91 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,005, 5000000  | &#39;Search in byte[] BloomFilter&#39;        | 132.08 ns |  5.480 ns |  35.41 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Search in Unsafe byte[] BloomFilter&#39; | 144.81 ns | 19.927 ns |  76.73 ns |  1.05 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Search in Span&lt;byte&gt; BloomFilter&#39;    | 146.13 ns | 11.944 ns |  45.99 ns |  1.08 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Search in BitArray BloomFilter&#39;      | 150.07 ns |  9.965 ns |  64.40 ns |  1.14 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 5000000  | &#39;Search in Span&lt;bool&gt; BloomFilter&#39;    | 153.08 ns | 13.977 ns |  53.82 ns |  1.12 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,005, 10000000 | &#39;Search in Span&lt;bool&gt; BloomFilter&#39;    | 133.04 ns |  4.332 ns |  27.99 ns |  0.93 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Search in Span&lt;byte&gt; BloomFilter&#39;    | 135.11 ns | 11.603 ns |  44.68 ns |  0.91 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Search in byte[] BloomFilter&#39;        | 150.20 ns | 18.228 ns |  70.19 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Search in Unsafe byte[] BloomFilter&#39; | 152.22 ns | 34.291 ns | 132.04 ns |  1.01 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,005, 10000000 | &#39;Search in BitArray BloomFilter&#39;      | 163.99 ns | 12.223 ns |  47.06 ns |  1.10 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,05, 5000000   | &#39;Search in byte[] BloomFilter&#39;        |  69.13 ns |  4.118 ns |  26.61 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Search in BitArray BloomFilter&#39;      |  71.71 ns |  2.849 ns |  18.41 ns |  1.04 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Search in Unsafe byte[] BloomFilter&#39; |  75.14 ns | 12.691 ns |  48.87 ns |  1.02 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Search in Span&lt;byte&gt; BloomFilter&#39;    |  77.34 ns |  9.553 ns |  36.79 ns |  1.08 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 5000000   | &#39;Search in Span&lt;bool&gt; BloomFilter&#39;    |  80.44 ns |  8.561 ns |  32.96 ns |  1.17 |         52428800 |   100,0000 % |   0,0000 % |         - |
|             |                 |                                       |           |           |           |       |                  |              |            |           |
| Bloomfilter | 0,05, 10000000  | &#39;Search in Unsafe byte[] BloomFilter&#39; |  73.79 ns |  4.555 ns |  29.43 ns |  0.97 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Search in byte[] BloomFilter&#39;        |  79.59 ns |  9.470 ns |  36.47 ns |  1.00 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Search in Span&lt;byte&gt; BloomFilter&#39;    |  82.67 ns | 11.613 ns |  44.72 ns |  1.04 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Search in Span&lt;bool&gt; BloomFilter&#39;    |  83.43 ns | 13.056 ns |  50.27 ns |  1.04 |         52428800 |   100,0000 % |   0,0000 % |         - |
| Bloomfilter | 0,05, 10000000  | &#39;Search in BitArray BloomFilter&#39;      |  87.00 ns |  5.374 ns |  20.69 ns |  1.10 |         52428800 |   100,0000 % |   0,0000 % |         - |