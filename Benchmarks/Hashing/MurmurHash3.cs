using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Numerics.BitOperations;

namespace Benchmarks.Hashing
{
    internal static class MurmurHash3
    {
        /// <summary>
        /// Hashes the <paramref name="bytes"/> into a MurmurHash3 as a <see cref="uint"/>.
        /// </summary>
        /// <param name="bytes">The span.</param>
        /// <param name="seed">The seed for this algorithm.</param>
        /// <returns>The MurmurHash3 as a <see cref="uint"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash32(ref ReadOnlySpan<byte> bytes, uint seed)
        {
            ref byte bp = ref MemoryMarshal.GetReference(bytes);
            ref uint endPoint = ref Unsafe.Add(ref Unsafe.As<byte, uint>(ref bp), bytes.Length >> 2);
            if (bytes.Length >= sizeof(uint))
            {
                do
                {
                    seed = RotateLeft(seed ^ RotateLeft(Unsafe.ReadUnaligned<uint>(ref bp) * 3432918353U, 15) * 461845907U, 13) * 5 - 430675100;
                    bp = ref Unsafe.Add(ref bp, sizeof(uint));
                } while (Unsafe.IsAddressLessThan(ref Unsafe.As<byte, uint>(ref bp), ref endPoint));
            }

            var remainder = bytes.Length & 3;
            if (remainder > 0)
            {
                uint num = 0;

                switch (remainder)
                {
                    case 1:
                        num ^= endPoint;
                        break;
                    case 2:
                        num ^= num ^= Unsafe.Add(ref endPoint, 1) << 8;
                        goto case 1;
                    case 3:
                        num ^= Unsafe.Add(ref endPoint, 2) << 16;
                        goto case 2;
                }

                seed ^= RotateLeft(num * 3432918353U, 15) * 461845907U;
            }

            seed ^= (uint)bytes.Length;
            seed = (uint)((seed ^ (seed >> 16)) * -2048144789);
            seed = (uint)((seed ^ (seed >> 13)) * -1028477387);
            return seed ^ seed >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash32(ReadOnlySpan<byte> bytes, uint seed)
        {
            var length = bytes.Length;
            var h1 = seed;
            var remainder = length & 3;
            var position = length - remainder;
            for (var start = 0; start < position; start += 4)
                h1 = (uint)((int)RotateLeft(h1 ^ RotateLeft(BitConverter.ToUInt32(bytes.Slice(start, 4)) * 3432918353U, 15) * 461845907U, 13) * 5 - 430675100);

            if (remainder > 0)
            {
                uint num = 0;
                switch (remainder)
                {
                    case 1:
                        num ^= bytes[position];
                        break;
                    case 2:
                        num ^= (uint)bytes[position + 1] << 8;
                        goto case 1;
                    case 3:
                        num ^= (uint)bytes[position + 2] << 16;
                        goto case 2;
                }

                h1 ^= RotateLeft(num * 3432918353U, 15) * 461845907U;
            }

            h1 = FMix(h1 ^ (uint)length);

            return h1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint FMix(uint h)
        {
            h = (uint)(((int)h ^ (int)(h >> 16)) * -2048144789);
            h = (uint)(((int)h ^ (int)(h >> 13)) * -1028477387);
            return h ^ h >> 16;
        }
    }
}
