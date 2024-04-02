using System.Runtime.CompilerServices;

namespace Benchmarks.Helpers
{
    internal static class ArrayExtensions
    {
        public static void DefaultSort(Span<int> values)
        {
            var first = 0;
            while (first < values.Length)
            {
                for (var i = first + 1; i < values.Length; i++)
                {
                    if (values[i] < values[first])
                        Swap(values, first, i);
                }
                first++;
            }
        }

        public static void Sort(Span<int> values)
        {
            int minIndex, maxIndex;

            var first = minIndex = 0;
            var last = maxIndex = values.Length - 1;

            var minValue = values[minIndex];
            var maxValue = values[maxIndex];
            while (first < last)
            {
                for (var i = first + 1; i < last; i++)
                {
                    var value = values[i];
                    if (minValue > value)
                    {
                        minIndex = i;
                        minValue = values[minIndex];
                    }
                    else if (maxValue < value)
                    {
                        maxIndex = i;
                        maxValue = values[maxIndex];
                    }
                }
                if (first != minIndex)
                    Swap(values, first, minIndex);
                if (last != maxIndex)
                    Swap(values, last, maxIndex);
                minIndex = ++first;
                maxIndex = --last;

                if (values[minIndex] > values[maxIndex] && first < last)
                    Swap(values, minIndex, maxIndex);

                minValue = values[minIndex];
                maxValue = values[maxIndex];
            }
        }

        public static void Quicksort(Span<int> values)
            => Quicksort(values, 0, values.Length - 1);

        public static bool IsEquivalentTo(Span<int> left, Span<int> right)
        {
            if (left.Length != right.Length)
                return false;
            for(var i = 0; i < left.Length; i++)
            {
                if (right[i] != left[i]) 
                    return false;
            }
            return true;
        }

        private static void Quicksort(Span<int> values, int left, int right)
        {
            while (left < right)
            {
                var pivot = Partitionate(values, left, right);
                if (pivot - left < right - pivot)
                {
                    Quicksort(values, left, pivot - 1);
                    left = pivot + 1;
                }
                else
                {
                    Quicksort(values, pivot + 1, right);
                    right = pivot - 1;
                }
            }
            //if (left < right)
            //{
            //    var pivot = Partitionate(values, left, right);
            //    Quicksort(values, left, pivot - 1);
            //    Quicksort(values, pivot + 1, right);
            //}
        }

        private static int Partitionate(Span<int> values, int left, int right)
        {
            var pivotIndex = (left + right) / 2;
            //var pivotIndex = Random.Shared.Next(left, right + 1);
            //var pivotIndex = right;
            var pivot = values[pivotIndex];
            (values[pivotIndex], values[right]) = (values[right], pivot);
            var i = left - 1;
            for (int j = left; j < right; j++)
            {
                if (values[j] <= pivot)
                {
                    i++;
                    Swap(values, i, j);
                }
            }
            i++;
            Swap(values, i, right);
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(Span<int> a, int i, int j)
        {
            int t = a[i];
            a[i] = a[j];
            a[j] = t;
        }
    }
}
