namespace Benchmarks.Bloomfilter.Impl;

public interface IBloomFilter
{
    void Add(ref ReadOnlySpan<byte> bytes);
    bool MaybeContains(ref ReadOnlySpan<byte> bytes);

    void Save(string filename);
}
