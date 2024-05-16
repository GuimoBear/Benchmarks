using System.Runtime.CompilerServices;
using System.Text;

namespace Benchmarks.Bloomfilter
{
    internal static class Utils
    {
        internal static readonly Lazy<byte[][]> Indexes;

        static Utils()
        {
            Indexes = new Lazy<byte[][]>(() => File.ReadAllLines(Path.Combine("C:", "projetos", "Benchmarks", "Benchmarks", "Files", "indexes.txt")).Select(Encoding.UTF8.GetBytes).ToArray());
        }

        public static void SaveBloomFilter(int hashingCount, int bitsCount, ReadOnlySpan<byte> bytes, string filename)
        {
            var hashingCountBytes = BitConverter.GetBytes(hashingCount);
            var bitsCountBytes = BitConverter.GetBytes(bitsCount);
            var byteArrayLengthBytes = BitConverter.GetBytes(bytes.Length);
            using (var file = File.Create(filename, bytes.Length + 8))
            {
                file.Write(hashingCountBytes);
                file.Write(bitsCountBytes);
                file.Write(byteArrayLengthBytes);
                file.Write(bytes);
                file.Flush();
                file.Close();
            }
        }

        public static (int HashingCount, int BitsCount, byte[] Bytes) ReadBloomFilter(string filename)
        {
            int hashingCount = -1;
            int bitsCount = -1;
            byte[] buffer;
            using (var file = File.OpenRead(filename))
            {
                Span<byte> numbers = new byte[4];
                file.Read(numbers);
                hashingCount = BitConverter.ToInt32(numbers);
                file.Read(numbers);
                bitsCount = BitConverter.ToInt32(numbers);
                file.Read(numbers);
                var byteArrayLength = BitConverter.ToInt32(numbers);
                buffer = new byte[byteArrayLength];
                file.Read(buffer);
                file.Close();
            }
            return (hashingCount, bitsCount, buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] GetBits(this byte pByte)
        {
            var bools = new bool[8];
            for(int i = 0; i < 8; i++)
            {
                byte b = (byte)(1 << i);
                bools[i] = (pByte & b) == b;
            }
            return bools;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte(Span<bool> bools)
        {
            byte ret = 0;
            var length = bools.Length;
            for (int i = 0; i < length; i++)
            {
                if (bools[i])
                    ret |= (byte)(1 << i);
            }
            return ret;
        }
    }
}
