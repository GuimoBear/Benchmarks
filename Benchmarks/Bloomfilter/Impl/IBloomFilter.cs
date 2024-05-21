namespace Benchmarks.Bloomfilter.Impl;

public interface IBloomFilter
{
    void Add(ReadOnlySpan<byte> bytes);
    bool MaybeContains(ReadOnlySpan<byte> bytes);

    void Save(string filename);
}
