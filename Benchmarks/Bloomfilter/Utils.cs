using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public static void SaveBloomFilter(double probabilityOfFalsePositives, int expectedElementsInTheFilter, int hashingCount, int bitsCount, ReadOnlySpan<byte> bytes, string filename, bool tryCompress)
        {
            using (var file = File.Create(filename, 1024 * 16))
            {
                file.Write(BitConverter.GetBytes(probabilityOfFalsePositives));
                file.Write(BitConverter.GetBytes(expectedElementsInTheFilter));
                file.Write(BitConverter.GetBytes(hashingCount));
                file.Write(BitConverter.GetBytes(bitsCount));
                file.Write(BitConverter.GetBytes(bytes.Length));
                Write(file, bytes, tryCompress);
                file.Flush();
                file.Close();
            }
        }

        const int MSB = 1 << 31;

        private static void Write(FileStream file, ReadOnlySpan<byte> bytes, bool tryCompress)
        {
            if (!tryCompress)
            {
                file.WriteByte(0);
                file.Write(bytes);
                return;
            }

            const int halfOfInt = 1 << 15;

            int numberOfBytes = 0;
            Dictionary<byte, List<int>> bytesIndexes = new Dictionary<byte, List<int>>();
            for (int i = 0; i < bytes.Length; i++)
            {
                var @byte = bytes[i];
                if (@byte > 0)
                {
                    ref var counter = ref CollectionsMarshal.GetValueRefOrAddDefault(bytesIndexes, @byte, out var exists);
                    int previousIndex;
                    if (!exists)
                    {
                        previousIndex = 0;
                        counter = new List<int> { i };
                        numberOfBytes += sizeof(byte) + sizeof(int);
                    }
                    else
                    {
                        previousIndex = counter[counter.Count - 1];
                        counter.Add(i);
                    }
                    numberOfBytes += (i - previousIndex) >= halfOfInt ? sizeof(int) : 2;
                }
            }
            if (numberOfBytes < bytes.Length)
            {
                file.WriteByte(1);
                foreach (var (@byte, indexes) in bytesIndexes)
                {
                    file.WriteByte(@byte);
                    var span = CollectionsMarshal.AsSpan(indexes);
                    int previousIndex = 0;
                    for (int i = 0; i < span.Length; i++)
                    {
                        var index = span[i];
                        if ((index - previousIndex) < halfOfInt && span.Length > i + 1)
                        {
                            var newIndex = index - previousIndex;
                            var nextIndex = span[i + 1];
                            if ((nextIndex - index) < halfOfInt)
                            {
                                var newNextIndex = nextIndex - index;
                                newIndex <<= 16;
                                newIndex |= newNextIndex;
                                newIndex |= MSB;
                                index = newIndex;
                                previousIndex = span[i + 1];
                                i++;
                                goto Write;
                            }
                        }
                        previousIndex = index;
                        Write:
                        file.Write(BitConverter.GetBytes(index));
                    }
                    file.Write(BitConverter.GetBytes(-1));
                }
            }
            else
            {
                file.WriteByte(0);
                file.Write(bytes);
            }
        }

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
                Read(file, buffer);
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

        private static void Read(FileStream file, Span<byte> buffer)
        {
            var compressed = Convert.ToBoolean(file.ReadByte());
            if (compressed)
            {
                Span<byte> indexBytes = stackalloc byte[4];
                Span<byte> bytes = stackalloc byte[1];
                while (file.Position < file.Length)
                {
                    file.Read(bytes);
                    byte currentByte = bytes[0];
                    file.Read(indexBytes);
                    int readedIndex = BitConverter.ToInt32(indexBytes);
                    int previousIndex = 0;
                    do
                    {
                        if (readedIndex < 0)
                        {
                            readedIndex &= ~MSB;

                            var newIndex = ((readedIndex & ~0x0000ffff) >> 16) + previousIndex;
                            var newNextIndex = (readedIndex & 0x0000ffff) + newIndex;

                            buffer[newIndex] = currentByte;
                            buffer[newNextIndex] = currentByte;
                            previousIndex = newNextIndex;
                        }
                        else
                        {
                            buffer[readedIndex] = currentByte;
                            previousIndex = readedIndex;
                        }
                        file.Read(indexBytes);
                        readedIndex = BitConverter.ToInt32(indexBytes);
                    }
                    while (readedIndex != -1);
                }
            }
            else
            {
                file.Read(buffer);
            }
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
