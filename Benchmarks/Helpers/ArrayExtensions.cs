using System.Runtime.CompilerServices;
using System.Security.Cryptography;

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
        {
            Quicksort(values, 0, values.Length - 1);
        }

        public static void OptimalQuicksort(Span<int> values)
        {
            OptimalQuicksort(values, 0, values.Length - 1);
        }

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
        }

        private static void OptimalQuicksort(Span<int> values, int left, int right)
        {
            while (right > left)
            {
                int p = PickPivotAndPartitionate(values, left, right);
                OptimalQuicksort(values, p + 1, right);
                right = p - 1;
            }
        }

        private static int Partitionate(Span<int> values, int left, int right)
        {
            var pivotIndex = left + (right - left) / 2;
            var pivot = values[pivotIndex];
            Swap(values, pivotIndex, right);
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

        private static int PickPivotAndPartitionate(Span<int> values, int left, int right)
        {
            var pivotIndex = left + (right - left) / 2;

            SwapIfGreater(values, left, pivotIndex);
            SwapIfGreater(values, left, right);
            SwapIfGreater(values, pivotIndex, right);

            var pivot = values[pivotIndex];
            Swap(values, pivotIndex, right - 1);

            int lo = left, hi = right - 1;

            while (lo < hi)
            {
                while (values[++lo] < pivot) ;
                while (pivot < values[--hi]) ;

                if (lo >= hi)
                    break;

                Swap(values, lo, hi);
            }

            Swap(values, lo, right - 1);
            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(Span<int> a, int i, int j)
        {
            if (i != j)
            {
                int t = a[i];
                a[i] = a[j];
                a[j] = t;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapIfGreater(Span<int> a, int i, int j)
        {
            if (i != j && a[i] > a[j])
            {
                int t = a[i];
                a[i] = a[j];
                a[j] = t;
            }
        }
    }
}
