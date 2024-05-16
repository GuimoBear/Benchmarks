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

        public static void SaveBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter, int hashingCount, int bitsCount, ReadOnlySpan<byte> bytes, string filename)
        {
            using (var file = File.Create(filename, bytes.Length + 8))
            {
                Write(file, BitConverter.GetBytes(probabilityOfFalsePositives));
                Write(file, BitConverter.GetBytes(expectedElementsInTheFilter));
                Write(file, BitConverter.GetBytes(hashingCount));
                Write(file, BitConverter.GetBytes(bitsCount));
                Write(file, BitConverter.GetBytes(bytes.Length));
                Write(file, bytes);
                file.Flush();
                file.Close();
            }
        }

        private static void Write(FileStream file, ReadOnlySpan<byte> bytes)
            => file.Write(bytes);

        public static (double ProbabilityOfFalsePositives, int ExpectedElementsInTheFilter, int HashingCount, int BitsCount, byte[] Bytes) ReadBloomFilter(string filename)
        {
            double probabilityOfFalsePositives;
            int expectedElementsInTheFilter;
            int hashingCount;
            int bitsCount;
            byte[] buffer;
            using (var file = File.OpenRead(filename))
            {
                probabilityOfFalsePositives = file.Read(sizeof(double), BitConverter.ToDouble);
                expectedElementsInTheFilter = file.Read(sizeof(int), BitConverter.ToInt32);
                hashingCount = file.Read(sizeof(int), BitConverter.ToInt32);
                bitsCount = file.Read(sizeof(int), BitConverter.ToInt32);
                var byteArrayLength = file.Read(sizeof(int), BitConverter.ToInt32);
                buffer = new byte[byteArrayLength];
                file.Read(buffer);
                file.Close();
            }
            return (probabilityOfFalsePositives, expectedElementsInTheFilter, hashingCount, bitsCount, buffer);
        }

        private delegate TValue Factory<TValue>(ReadOnlySpan<byte> bytes);

        private static TValue Read<TValue>(this FileStream file, int size, Factory<TValue> factory)
        {
            var buffer = new byte[size];
            file.Read(buffer);
            return factory(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool[] GetBits(this byte pByte)
        {
            var bools = new bool[8];

            //var index = 0;
            //for (int i = 7; i >= 0; i--)
            //{
            //    byte b = (byte)(1 << i);
            //    bools[index++] = (pByte & b) == b;
            //}
            //return bools;

            for (int i = 0; i < 8; i++)
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
            //var shift = 7;
            //for (int i = 0; i < length; i++)
            //{
            //    if (bools[i])
            //        ret |= (byte)(1 << shift);
            //    shift--;
            //}
            //return ret;

            for (int i = 0; i < length; i++)
            {
                if (bools[i])
                    ret |= (byte)(1 << i);
            }
            return ret;
        }
    }
}
