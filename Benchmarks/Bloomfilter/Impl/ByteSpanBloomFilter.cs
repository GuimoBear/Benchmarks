using HashDepot;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Benchmarks.Bloomfilter.Impl;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public sealed class ByteSpanBloomFilter : IBloomFilter, IEquatable<ByteSpanBloomFilter>
{
    private readonly double _probabilityOfFalsePositives;
    private readonly int _expectedElementsInTheFilter;

    private readonly byte[] _filter;
    private readonly int _bitsCount;
    private readonly int _hashingCount;

    public ByteSpanBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
    {
        _probabilityOfFalsePositives = probabilityOfFalsePositives;
        _expectedElementsInTheFilter = expectedElementsInTheFilter;

        var filterSize = (int)Math.Ceiling(expectedElementsInTheFilter * Math.Log(probabilityOfFalsePositives) / Math.Log(1.0 / Math.Pow(2.0, Math.Log(2.0))));
        var numberOfHashFunctions = (int)Math.Round(filterSize / expectedElementsInTheFilter * Math.Log(2.0));

        _bitsCount = filterSize;
        _filter = new byte[filterSize % 8 == 0 ? (filterSize / 8) : (filterSize / 8) + 1];
        _hashingCount = numberOfHashFunctions;
    }

    private ByteSpanBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter, int hashingCount, int bitsCount, byte[] bytes)
    {
        _probabilityOfFalsePositives = probabilityOfFalsePositives;
        _expectedElementsInTheFilter = expectedElementsInTheFilter;
        _hashingCount = hashingCount;
        _bitsCount = bitsCount;
        _filter = bytes;
    }

    /// <summary>
    /// Three least significant bits
    /// </summary>
    const int THREE_LSB = 7;
    public void Add(ReadOnlySpan<byte> bytes)
    {
        var hashingCount = this._hashingCount;
        var bitsCount = this._bitsCount;
        Span<byte> filter = this._filter;
        for (uint i = 0; i < hashingCount; i++)
        {
            var bitIndex = XXHash.Hash32(bytes, i) % bitsCount;
            var index = bitIndex / 8;
            var shift = (byte)(THREE_LSB & bitIndex);
            ref var @byte = ref filter[(int)index];
            @byte |= (byte)(1 << shift);
        }
    }

    public bool MaybeContains(ReadOnlySpan<byte> bytes)
    {
        var hashingCount = this._hashingCount;
        var bitsCount = this._bitsCount;
        var filter = this._filter;
        for (uint i = 0; i < hashingCount; i++)
        {
            var bitIndex = XXHash.Hash32(bytes, i) % bitsCount;
            var index = bitIndex / 8;
            var shift = (byte)(THREE_LSB & bitIndex);
            var value = (byte)(1 << shift);
            if ((filter[index] & value) != value)
                return false;
        }

        return true;
    }

    public bool Equals(ByteSpanBloomFilter? other)
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
            if (_filter[i] != other._filter[i])
                return false;
        }
        return true;
    }

    public void Save(string filename)
    {
        Utils.SaveBloomFilter(_probabilityOfFalsePositives, _expectedElementsInTheFilter, _hashingCount, _bitsCount, _filter, filename);
    }

    public static ByteSpanBloomFilter Create(double probabilityOfFalsePositives, int expectedElementsInTheFilter)
        => new ByteSpanBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter);

    public static bool TryLoad(string filename, [NotNullWhen(true)] out ByteSpanBloomFilter? filter)
    {
        if (!File.Exists(filename))
        {
            filter = null;
            return false;
        }
        var (probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, bytes) = Utils.ReadBloomFilter(filename);
        filter = new ByteSpanBloomFilter(probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, bytes);
        return true;
    }
}
