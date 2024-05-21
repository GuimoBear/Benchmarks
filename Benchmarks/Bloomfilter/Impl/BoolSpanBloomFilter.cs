using HashDepot;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Benchmarks.Bloomfilter.Impl;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public sealed class BoolSpanBloomFilter : IBloomFilter, IEquatable<BoolSpanBloomFilter>
{
    private readonly double _probabilityOfFalsePositives;
    private readonly int _expectedElementsInTheFilter;

    private readonly Memory<bool> _filter;
    private readonly int _bitsCount;
    private readonly int _hashingCount;

    public BoolSpanBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
    {
        _probabilityOfFalsePositives = probabilityOfFalsePositives;
        _expectedElementsInTheFilter = expectedElementsInTheFilter;

        var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
        var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

        _bitsCount = filterSize;
        _filter = new Memory<bool>(new bool[filterSize]);
        _hashingCount = numberOfHashFunctions;
    }

    private BoolSpanBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter, int hashingCount, int bitsCount, byte[] bytes)
    {
        _probabilityOfFalsePositives = probabilityOfFalsePositives;
        _expectedElementsInTheFilter = expectedElementsInTheFilter;

        _hashingCount = hashingCount;
        _bitsCount = bitsCount;

        _filter = new Memory<bool>(bytes
            .SelectMany(b => b.GetBits())
            .Take(bitsCount)
            .ToArray());
    }

    public void Add(ReadOnlySpan<byte> bytes)
    {
        var hashingCount = this._hashingCount;
        var filter = this._filter;
        var filterSize = this._bitsCount;
        for (uint i = 0; i < hashingCount; i++)
        {
            var index = XXHash.Hash32(bytes, i) % filterSize;
            filter.Span[(int)index] = true;
        }
    }

    public bool MaybeContains(ReadOnlySpan<byte> bytes)
    {
        var hashingCount = this._hashingCount;
        var filter = this._filter;
        var filterSize = this._bitsCount;
        for (uint i = 0; i < hashingCount; i++)
        {
            var index = XXHash.Hash32(bytes, i) % filterSize;
            if (filter.Span[(int)index] == false)
                return false;
        }

        return true;
    }

    public bool Equals(BoolSpanBloomFilter? other)
    {
        if (other == null) return false;
        if (_probabilityOfFalsePositives != other._probabilityOfFalsePositives ||
            _expectedElementsInTheFilter != other._expectedElementsInTheFilter ||
            _bitsCount != other._bitsCount ||
            _hashingCount != other._hashingCount ||
            _filter.Length != other._filter.Length) return false;
        var length = _filter.Length;
        for (int i = 0; i < length; i++)
        {
            if (_filter.Span[i] != other._filter.Span[i])
                return false;
        }
        return true;
    }

    public void Save(string filename)
    {
        const int BitShiftPerByte = 3;
        var size = (int)((uint)(this._bitsCount - 1 + (1 << BitShiftPerByte)) >> BitShiftPerByte);
        var bytes = new byte[size];
        var slice = this._filter.Span;

        var length = bytes.Length;
        for (int i = 0; i < length; i++)
        {
            var newSlice = slice.Length >= 8 ? slice.Slice(0, 8) : slice;
            bytes[i] = Utils.GetByte(newSlice);
            slice = slice.Slice(newSlice.Length);
        }
        Utils.SaveBloomFilter(_probabilityOfFalsePositives, _expectedElementsInTheFilter, _hashingCount, _bitsCount, bytes, filename);
    }

    public static BoolSpanBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        => new BoolSpanBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

    public static bool TryLoad(string filename, [NotNullWhen(true)] out BoolSpanBloomFilter? filter)
    {
        if (!File.Exists(filename))
        {
            filter = null;
            return false;
        }
        var (probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, bytes) = Utils.ReadBloomFilter(filename);
        filter = new BoolSpanBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, bytes);
        return true;
    }
}
