using Benchmarks.Hashing;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Benchmarks.Bloomfilter.Impl;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public sealed class Murmur3BitArrayBloomFilter : IBloomFilter, IEquatable<Murmur3BitArrayBloomFilter>
{
    private readonly double _probabilityOfFalsePositives;
    private readonly int _expectedElementsInTheFilter;

    private readonly BitArray _filter;
    private readonly int _bitsCount;
    private readonly int _hashingCount;

    public Murmur3BitArrayBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
    {
        _probabilityOfFalsePositives = probabilityOfFalsePositives;
        _expectedElementsInTheFilter = expectedElementsInTheFilter;

        var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
        var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

        _bitsCount = filterSize;
        _filter = new BitArray(filterSize);
        _hashingCount = numberOfHashFunctions;
    }

    private Murmur3BitArrayBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter, int hashingCount, int bitsCount, byte[] bytes)
    {
        _probabilityOfFalsePositives = probabilityOfFalsePositives;
        _expectedElementsInTheFilter = expectedElementsInTheFilter;

        _hashingCount = hashingCount;
        _bitsCount = bitsCount;

        _filter = new BitArray(bytes
            .SelectMany(b => b.GetBits())
            .Take(bitsCount)
            .ToArray());
    }

    public void Add(ref ReadOnlySpan<byte> bytes)
    {
        var hashingCount = this._hashingCount;
        var filter = this._filter;
        var filterSize = this._bitsCount;
        for (uint i = 0; i < hashingCount; i++)
        {
            var hash = MurmurHash3.Hash32(ref bytes, i);
            var index = hash % filterSize;
            filter[(int)index] = true;
        }
    }

    public bool MaybeContains(ref ReadOnlySpan<byte> bytes)
    {
        var hashingCount = this._hashingCount;
        var filter = this._filter;
        var filterSize = this._bitsCount;
        for (uint i = 0; i < hashingCount; i++)
        {
            var hash = MurmurHash3.Hash32(ref bytes, i);
            var index = hash % filterSize;
            if (filter[(int)index] == false)
                return false;
        }

        return true;
    }

    public bool Equals(Murmur3BitArrayBloomFilter? other)
    {
        if (other == null) return false;
        if (_probabilityOfFalsePositives != other._probabilityOfFalsePositives ||
            _expectedElementsInTheFilter != other._expectedElementsInTheFilter ||
            _bitsCount != other._bitsCount ||
            _hashingCount != other._hashingCount ||
            _filter.Length != other._filter.Length ||
            _filter.Count != other._filter.Count) return false;
        var length = _filter.Count;
        for (int i = 0; i < length; i++)
        {
            if (_filter[i] != other._filter[i])
                return false;
        }
        return true;
    }

    public void Save(string filename)
    {
        const int BitShiftPerByte = 3;
        var size = (int)((uint)(this._bitsCount - 1 + (1 << BitShiftPerByte)) >> BitShiftPerByte);
        var bytes = new byte[size];
        _filter.CopyTo(bytes, 0);
        Utils.SaveBloomFilter(_probabilityOfFalsePositives, _expectedElementsInTheFilter, _hashingCount, _bitsCount, bytes, filename);
    }

    public static Murmur3BitArrayBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        => new Murmur3BitArrayBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

    public static bool TryLoad(string filename, [NotNullWhen(true)] out Murmur3BitArrayBloomFilter? filter)
    {
        if (!File.Exists(filename))
        {
            filter = null;
            return false;
        }
        var (probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, bytes) = Utils.ReadBloomFilter(filename);
        filter = new Murmur3BitArrayBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, bytes);
        return true;
    }
}
