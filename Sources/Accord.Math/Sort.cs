// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Extension methods for sorting operations.
    /// </summary>
    /// 
    public static partial class Sort
    {
#if DEBUG
        public static int INTROSORT_THRESHOLD = 32;
#else
        const int INTROSORT_THRESHOLD = 32;
#endif

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<T>(T[] keys, bool asc = true)
            where T : IComparable<T>
        {
            return Insertion(keys, 0, keys.Length, asc);
        }

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<T, U>(T[] keys, U[] items, bool asc = true)
            where T : IComparable<T>
        {
            return Insertion(keys, items, 0, keys.Length, asc);
        }

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<T>(T[] keys, Func<T, T, int> comparer, bool asc = true)
        {
            return Insertion(keys, 0, keys.Length, comparer, asc);
        }

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<TKey, TValue>(TKey[] keys, TValue[] items, Func<TKey, TKey, int> comparer, bool asc = true)
        {
            return Insertion(keys, items, 0, keys.Length, comparer, asc);
        }

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<T>(T[] keys, int first, int last, bool asc = true)
            where T : IComparable<T>
        {
            Debug.Assert(last >= first);

            if (first == last)
                return last;

            if (asc)
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    int j = i;

                    if (key.CompareTo(keys[first]) < 0)
                    {
                        for (; first != j; j--)
                            keys[j] = keys[j - 1];
                    }
                    else
                    {
                        for (; key.CompareTo(keys[j - 1]) < 0; j--)
                            keys[j] = keys[j - 1];
                    }

                    keys[j] = key;
                }
            }
            else
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    int j = i;

                    if (key.CompareTo(keys[first]) > 0)
                    {
                        for (; first != j; j--)
                            keys[j] = keys[j - 1];
                    }
                    else
                    {
                        for (; key.CompareTo(keys[j - 1]) > 0; j--)
                            keys[j] = keys[j - 1];
                    }

                    keys[j] = key;
                }
            }

            return last;
        }

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<TKey, TValue>(TKey[] keys, TValue[] items, int first, int last, bool asc = true)
            where TKey : IComparable<TKey>
        {
            Debug.Assert(last >= first);

            if (first == last)
                return last;

            if (asc)
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    var val = items[i];
                    int j = i;

                    if (key.CompareTo(keys[first]) < 0)
                    {
                        for (; first != j; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }
                    else
                    {
                        for (; key.CompareTo(keys[j - 1]) < 0; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }

                    keys[j] = key;
                    items[j] = val;
                }
            }
            else
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    var val = items[i];
                    int j = i;

                    if (key.CompareTo(keys[first]) > 0)
                    {
                        for (; first != j; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }
                    else
                    {
                        for (; key.CompareTo(keys[j - 1]) > 0; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }

                    keys[j] = key;
                    items[j] = val;
                }
            }

            return last;
        }

        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<T>(T[] keys, int first, int last, Func<T, T, int> comparer, bool asc = true)
        {
            Debug.Assert(last >= first);

            if (first == last)
                return last;

            if (asc)
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    int j = i;

                    if (comparer(key, keys[first]) < 0)
                    {
                        for (; first != j; j--)
                            keys[j] = keys[j - 1];
                    }
                    else
                    {
                        for (; comparer(key, keys[j - 1]) < 0; j--)
                            keys[j] = keys[j - 1];
                    }

                    keys[j] = key;
                }
            }
            else
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    int j = i;

                    if (comparer(key, keys[first]) > 0)
                    {
                        for (; first != j; j--)
                            keys[j] = keys[j - 1];
                    }
                    else
                    {
                        for (; comparer(key, keys[j - 1]) > 0; j--)
                            keys[j] = keys[j - 1];
                    }

                    keys[j] = key;
                }
            }

            return last;
        }


        /// <summary>
        ///   Insertion sort.
        /// </summary>
        /// 
        public static int Insertion<TKeys, TValue>(TKeys[] keys, TValue[] items, int first, int last, Func<TKeys, TKeys, int> comparer, bool asc = true)
        {
            Debug.Assert(last >= first);

            if (first == last)
                return last;

            if (asc)
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    var val = items[i];
                    int j = i;

                    if (comparer(key, keys[first]) < 0)
                    {
                        for (; first != j; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }
                    else
                    {
                        for (; comparer(key, keys[j - 1]) < 0; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }

                    keys[j] = key;
                    items[j] = val;
                }
            }
            else
            {
                for (int i = first + 1; i != last; i++)
                {
                    var key = keys[i];
                    var val = items[i];
                    int j = i;

                    if (comparer(key, keys[first]) > 0)
                    {
                        for (; first != j; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }
                    else
                    {
                        for (; comparer(key, keys[j - 1]) > 0; j--)
                        {
                            keys[j] = keys[j - 1];
                            items[j] = items[j - 1];
                        }
                    }

                    keys[j] = key;
                    items[j] = val;
                }
            }

            return last;
        }

        /// <summary>
        ///   Partially orders a collection, making sure every element smaller 
        ///   than the n-th smaller element are in the beginning of the array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type for the items in the array.</typeparam>
        /// 
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements in the beginning of the 
        ///   array.</param>
        /// 
        public static void Partial<T>(T[] items, int n, bool asc = true)
            where T : IComparable<T>
        {
            NthElement<T>(items, n, asc);

            Array.Sort(items, 0, n);

            if (!asc)
                Array.Reverse(items, 0, n);
        }

        /// <summary>
        ///   Partially orders a collection, making sure every element smaller 
        ///   than the n-th smaller element are in the beginning of the array.
        /// </summary>
        /// 
        /// <typeparam name="TKey">The type for the keys associated with each value in the items array.</typeparam>
        /// <typeparam name="TValue">The type for the items in the array.</typeparam>
        /// 
        /// <param name="keys">The keys that will be used to determine the order of elements in <paramref name="items"/>.</param>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements in the beginning of the 
        ///   array.</param>
        /// 
        public static void Partial<TKey, TValue>(TKey[] keys, TValue[] items, int n, bool asc = true)
            where TKey : IComparable<TKey>
        {
            NthElement<TKey, TValue>(keys, items, n, asc);

            Array.Sort(keys, items, 0, n);

            if (!asc)
            {
                Array.Reverse(keys, 0, n);
                Array.Reverse(items, 0, n);
            }
        }





        /// <summary>
        ///   Reorders the elements in the range [left, right) in such a way that all elements that
        ///   are smaller than the pivot precede those that are greater than the pivot. Relative order 
        ///   of the elements is not preserved. This function should be equivalent to C++'s std::partition.
        /// </summary>
        /// 
        /// <param name="keys">The list to be reordered.</param>
        /// <param name="items">An array of keys associated with each element in the list.</param>
        /// <param name="first">The beginning of the range to be reordered.</param>
        /// <param name="last">The end of the range to be reordered.</param>
        /// <param name="asc">Whether to sort in ascending or descending order.</param>
        /// 
        /// <returns>The index of the new pivot.</returns>
        /// 
        public static int Partition<TKey, TValue>(this TKey[] keys, TValue[] items, int first, int last, bool asc = true)
            where TKey : IComparable<TKey>
        {
            if (first >= last)
                return first;

            int pivotIndex = pivot(keys, items, first, last - 1, asc);
            var pivotValue = keys[pivotIndex];

            // Move pivot to end
            keys.Swap(pivotIndex, last - 1);
            items.Swap(pivotIndex, last - 1);
            int storeIndex = first;

            if (asc)
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (keys[i].CompareTo(pivotValue) < 0)
                    {
                        keys.Swap(storeIndex, i);
                        items.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }
            else
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (keys[i].CompareTo(pivotValue) > 0)
                    {
                        keys.Swap(storeIndex, i);
                        items.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }

            // Move pivot to its final place
            keys.Swap(last - 1, storeIndex);
            items.Swap(last - 1, storeIndex);
            return storeIndex;
        }

        /// <summary>
        ///   Reorders the elements in the range [left, right) in such a way that all elements that
        ///   are smaller than the pivot precede those that are greater than the pivot. Relative order 
        ///   of the elements is not preserved. This function should be equivalent to C++'s std::partition.
        /// </summary>
        /// 
        /// <param name="keys">The list to be reordered.</param>
        /// <param name="first">The beginning of the range to be reordered.</param>
        /// <param name="last">The end of the range to be reordered.</param>
        /// <param name="asc">Whether to sort in ascending or descending order.</param>
        /// 
        /// <returns>The index of the new pivot.</returns>
        /// 
        public static int Partition<T>(this T[] keys, int first, int last, bool asc = true)
            where T : IComparable<T>
        {
            if (first >= last)
                return first;

            int pivotIndex = pivot(keys, first, last - 1, asc);
            var pivotValue = keys[pivotIndex];

            // Move pivot to end
            keys.Swap(pivotIndex, last - 1);
            int storeIndex = first;

            if (asc)
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (keys[i].CompareTo(pivotValue) < 0)
                    {
                        keys.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }
            else
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (keys[i].CompareTo(pivotValue) > 0)
                    {
                        keys.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }

            // Move pivot to its final place
            keys.Swap(last - 1, storeIndex);
            return storeIndex;
        }



        /// <summary>
        ///   Reorders the elements in the range [left, right) in such a way that all elements for which 
        ///   the function <paramref name="compare"/> returns true precede the elements for which <paramref name="compare"/>
        ///   returns false. Relative order of the elements is not preserved. This function should be equivalent 
        ///   to C++'s std::partition.
        /// </summary>
        /// 
        /// <param name="keys">The list to be reordered.</param>
        /// <param name="items">An array of keys associated with each element in the list.</param>
        /// <param name="first">The beginning of the range to be reordered.</param>
        /// <param name="last">The end of the range to be reordered.</param>
        /// <param name="compare">Function to use in the comparison.</param>
        /// <param name="asc">Whether to sort in ascending or descending order.</param>
        /// 
        /// <returns>The index of the new pivot.</returns>
        /// 
        public static int Partition<TKey, TValue>(this TKey[] keys, TValue[] items, int first, int last, Func<TKey, TKey, int> compare, bool asc = true)
        {
            if (first >= last)
                return first;

            int pivotIndex = pivot(keys, items, first, last - 1, compare, asc);
            var pivotValue = keys[pivotIndex];

            // Move pivot to end
            keys.Swap(pivotIndex, last - 1);
            items.Swap(pivotIndex, last - 1);
            int storeIndex = first;

            if (asc)
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (compare(keys[i], pivotValue) < 0)
                    {
                        keys.Swap(storeIndex, i);
                        items.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }
            else
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (compare(keys[i], pivotValue) > 0)
                    {
                        keys.Swap(storeIndex, i);
                        items.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }

            // Move pivot to its final place
            keys.Swap(last - 1, storeIndex);
            items.Swap(last - 1, storeIndex);
            return storeIndex;

        }

        /// <summary>
        ///   Reorders the elements in the range [left, right) in such a way that all elements for which 
        ///   the function <paramref name="compare"/> returns true precede the elements for which <paramref name="compare"/>
        ///   returns false. Relative order of the elements is not preserved. This function should be equivalent
        ///   to C++'s std::partition.
        /// </summary>
        /// 
        /// <param name="items">The list to be reordered.</param>
        /// <param name="first">The beginning of the range to be reordered.</param>
        /// <param name="last">The end of the range to be reordered.</param>
        /// <param name="compare">Function to use in the comparison.</param>
        /// <param name="asc">Whether to sort in ascending or descending order.</param>
        /// 
        /// <returns>The index of the new pivot.</returns>
        /// 
        public static int Partition<T>(this T[] items, int first, int last, Func<T, T, int> compare, bool asc = true)
        {
            if (first >= last)
                return first;

            int pivotIndex = pivot(items, first, last - 1, compare, asc);
            var pivotValue = items[pivotIndex];

            // Move pivot to end
            items.Swap(pivotIndex, last - 1);
            int storeIndex = first;

            if (asc)
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (compare(items[i], pivotValue) < 0)
                    {
                        items.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }
            else
            {
                for (int i = first; i < last - 1; i++)
                {
                    if (compare(items[i], pivotValue) > 0)
                    {
                        items.Swap(storeIndex, i);
                        storeIndex++;
                    }
                }
            }

            // Move pivot to its final place
            items.Swap(last - 1, storeIndex);
            return storeIndex;
        }



        /// <summary>
        ///   Retrieves the n-th smallest element in an array. See remarks for more info.
        /// </summary>
        /// 
        /// <remarks>
        ///   As a side-effect, partially orders the collection, making sure every element smaller than the n-th 
        ///   smaller element are in the beginning of the array (but not necessarily in order). If you need the
        ///   values that come before the n-th element to also be in order, please use the 
        ///   <see cref="Partial{T}(T[], int, bool)"/> method instead.
        /// </remarks>
        /// 
        /// <typeparam name="TKey">The type for the keys associated with each value in the items array.</typeparam>
        /// <typeparam name="TValue">The type for the items in the array.</typeparam>
        /// 
        /// <param name="keys">The keys that will be used to determine the order of elements in <paramref name="items"/>.</param>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="first">The beginning of the search interval.</param>
        /// <param name="last">The end of the search interval.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements.</param>
        /// <param name="compare">The comparison function to be used to sort elements.</param>
        /// 
        /// <returns>If <paramref name="asc"/> is true, returns the n-th smallest element in 
        ///   the array. Otherwise, returns the n-th largest.</returns>
        /// 
        public static TKey NthElement<TKey, TValue>(this TKey[] keys, TValue[] items, int first, int last,
            int n, Func<TKey, TKey, int> compare, bool asc = true)
        {
            while (last - first >= INTROSORT_THRESHOLD)
            {
                int pivotIndex = Partition(keys, items, first, last, compare, asc: asc);

                if (n == pivotIndex)
                    return keys[n];
                else if (n > pivotIndex)
                    first = pivotIndex + 1;
                else // if (n < pivotIndex)
                    last = pivotIndex;
                Debug.Assert(last >= first);
            }

            Insertion(keys, items, first, last, compare, asc);
            return keys[n];
        }

        /// <summary>
        ///   Retrieves the n-th smallest element in an array. See remarks for more info.
        /// </summary>
        /// 
        /// <remarks>
        ///   As a side-effect, partially orders the collection, making sure every element smaller than the n-th 
        ///   smaller element are in the beginning of the array (but not necessarily in order). If you need the
        ///   values that come before the n-th element to also be in order, please use the 
        ///   <see cref="Partial{T}(T[], int, bool)"/> method instead.
        /// </remarks>
        /// 
        /// <typeparam name="T">The type for the items in the array.</typeparam>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="first">The beginning of the search interval.</param>
        /// <param name="last">The end of the search interval.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements.</param>
        /// <param name="compare">The comparison function to be used to sort elements.</param>
        /// 
        /// <returns>If <paramref name="asc"/> is true, returns the n-th smallest element in 
        ///   the array. Otherwise, returns the n-th largest.</returns>
        /// 
        public static T NthElement<T>(this T[] items, int first, int last,
            int n, Func<T, T, int> compare, bool asc = true)
        {
            while (last - first >= INTROSORT_THRESHOLD)
            {
                int pivotIndex = Partition(items, first, last, compare, asc: asc);

                if (n == pivotIndex)
                    return items[n];
                else if (n > pivotIndex)
                    first = pivotIndex + 1;
                else // if (n < pivotIndex)
                    last = pivotIndex;
                Debug.Assert(last >= first);
            }

            Insertion(items, first, last, compare, asc);
            return items[n];
        }

        /// <summary>
        ///   Retrieves the n-th smallest element in an array. See remarks for more info.
        /// </summary>
        /// 
        /// <remarks>
        ///   As a side-effect, partially orders the collection, making sure every element smaller than the n-th 
        ///   smaller element are in the beginning of the array (but not necessarily in order). If you need the
        ///   values that come before the n-th element to also be in order, please use the 
        ///   <see cref="Partial{T}(T[], int, bool)"/> method instead.
        /// </remarks>
        /// 
        /// <typeparam name="TKey">The type for the keys associated with each value in the items array.</typeparam>
        /// <typeparam name="TValue">The type for the items in the array.</typeparam>
        /// 
        /// <param name="keys">The keys that will be used to determine the order of elements in <paramref name="items"/>.</param>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="first">The beginning of the search interval.</param>
        /// <param name="last">The end of the search interval.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements.</param>
        /// 
        /// <returns>If <paramref name="asc"/> is true, returns the n-th smallest element in 
        ///   the array. Otherwise, returns the n-th largest.</returns>
        /// 
        public static TKey NthElement<TKey, TValue>(this TKey[] keys, TValue[] items, int first, int last, int n, bool asc = true)
            where TKey : IComparable<TKey>
        {
            while (last - first >= INTROSORT_THRESHOLD)
            {
                int pivotIndex = Partition(keys, items, first, last, asc: asc);

                if (n == pivotIndex)
                    return keys[n];
                else if (n > pivotIndex)
                    first = pivotIndex + 1;
                else // if (n < pivotIndex)
                    last = pivotIndex;
                Debug.Assert(last >= first);
            }

            Insertion(keys, items, first, last, asc);
            return keys[n];
        }

        /// <summary>
        ///   Retrieves the n-th smallest element in an array. See remarks for more info.
        /// </summary>
        /// 
        /// <remarks>
        ///   As a side-effect, partially orders the collection, making sure every element smaller than the n-th 
        ///   smaller element are in the beginning of the array (but not necessarily in order). If you need the
        ///   values that come before the n-th element to also be in order, please use the 
        ///   <see cref="Partial{T}(T[], int, bool)"/> method instead.
        /// </remarks>
        /// 
        /// <typeparam name="T">The type for the items in the array.</typeparam>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements.</param>
        /// 
        /// <returns>If <paramref name="asc"/> is true, returns the n-th smallest element in 
        ///   the array. Otherwise, returns the n-th largest.</returns>
        /// 
        public static T NthElement<T>(this T[] items, int n, bool asc = true)
            where T : IComparable<T>
        {
            return NthElement(items, 0, items.Length, n, asc);
        }

        /// <summary>
        ///   Retrieves the n-th smallest element in an array. See remarks for more info.
        /// </summary>
        /// 
        /// <remarks>
        ///   As a side-effect, partially orders the collection, making sure every element smaller than the n-th 
        ///   smaller element are in the beginning of the array (but not necessarily in order). If you need the
        ///   values that come before the n-th element to also be in order, please use the 
        ///   <see cref="Partial{T}(T[], int, bool)"/> method instead.
        /// </remarks>
        /// 
        /// <typeparam name="TKey">The type for the keys associated with each value in the items array.</typeparam>
        /// <typeparam name="TValue">The type for the items in the array.</typeparam>
        /// 
        /// <param name="keys">The keys that will be used to determine the order of elements in <paramref name="items"/>.</param>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements.</param>
        /// 
        /// <returns>If <paramref name="asc"/> is true, returns the n-th smallest element in 
        ///   the array. Otherwise, returns the n-th largest.</returns>
        /// 
        public static TKey NthElement<TKey, TValue>(this TKey[] keys, TValue[] items, int n, bool asc = true)
            where TKey : IComparable<TKey>
        {
            return NthElement(keys, items, 0, keys.Length, n, asc);
        }

        /// <summary>
        ///   Retrieves the n-th smallest element in an array. See remarks for more info.
        /// </summary>
        /// 
        /// <remarks>
        ///   As a side-effect, partially orders the collection, making sure every element smaller than the n-th 
        ///   smaller element are in the beginning of the array (but not necessarily in order). If you need the
        ///   values that come before the n-th element to also be in order, please use the 
        ///   <see cref="Partial{T}(T[], int, bool)"/> method instead.
        /// </remarks>
        /// 
        /// <typeparam name="T">The type for the items in the array.</typeparam>
        /// <param name="items">The array of elements from which the n-th element should be extracted.</param>
        /// <param name="first">The beginning of the search interval.</param>
        /// <param name="last">The end of the search interval.</param>
        /// <param name="n">The position to look for (0 returns the smallest element, 1 the second smallest, and so on).</param>
        /// <param name="asc">Whether to take the smallest or the largest element. If set to false, instead
        ///   of returning the smallest, the method will return the largest elements.</param>
        /// 
        /// <returns>If <paramref name="asc"/> is true, returns the n-th smallest element in 
        ///   the array. Otherwise, returns the n-th largest.</returns>
        /// 
        public static T NthElement<T>(this T[] items, int first, int last, int n, bool asc = true)
            where T : IComparable<T>
        {
            while (last - first >= INTROSORT_THRESHOLD)
            {
                int pivotIndex = Partition(items, first, last, asc: asc);

                if (n == pivotIndex)
                    return items[n];
                else if (n > pivotIndex)
                    first = pivotIndex + 1;
                else // if (n < pivotIndex)
                    last = pivotIndex;
                Debug.Assert(last >= first);
            }

            Insertion(items, first, last, asc);
            return items[n];
        }














        // To get the full effect of median-of-three, we have to actually
        // sort the first, middle and last elements to prevent cases that
        // would trigger worst-case performance in the sorting algorithm.

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int pivot<T, U>(T[] keys, U[] items, int first, int last, bool asc)
            where T : IComparable<T>
        {
            int dir = asc ? 1 : -1;
            int middle = first + (last - first) / 2;
            if (dir * keys[middle].CompareTo(keys[first]) < 0)
            {
                Matrix.Swap(keys, middle, first);
                Matrix.Swap(items, middle, first);
            }
            if (dir * keys[last].CompareTo(keys[middle]) < 0)
            {
                Matrix.Swap(keys, last, middle);
                Matrix.Swap(items, last, middle);

                if (dir * keys[middle].CompareTo(keys[first]) < 0)
                {
                    Matrix.Swap(keys, middle, first);
                    Matrix.Swap(items, middle, first);
                }
            }

            return middle;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int pivot<T>(T[] keys, int first, int last, bool asc)
            where T : IComparable<T>
        {
            int dir = asc ? 1 : -1;
            int middle = first + (last - first) / 2;
            if (dir * keys[middle].CompareTo(keys[first]) < 0)
                Matrix.Swap(keys, middle, first);

            if (dir * keys[last].CompareTo(keys[middle]) < 0)
            {
                Matrix.Swap(keys, last, middle);
                if (dir * keys[middle].CompareTo(keys[first]) < 0)
                    Matrix.Swap(keys, middle, first);
            }

            return middle;
        }


#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int pivot<T, U>(T[] keys, U[] items, int first, int last, Func<T, T, int> comparer, bool asc)
        {
            int dir = asc ? 1 : -1;
            int middle = first + (last - first) / 2;
            if (dir * comparer(keys[middle], keys[first]) < 0)
            {
                Matrix.Swap(keys, middle, first);
                Matrix.Swap(items, middle, first);
            }

            if (dir * comparer(keys[last], keys[middle]) < 0)
            {
                Matrix.Swap(keys, last, middle);
                Matrix.Swap(items, last, middle);

                if (dir * comparer(keys[middle], keys[first]) < 0)
                {
                    Matrix.Swap(keys, middle, first);
                    Matrix.Swap(items, middle, first);
                }
            }

            return middle;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int pivot<T>(T[] keys, int first, int last, Func<T, T, int> comparer, bool asc)
        {
            int dir = asc ? 1 : -1;
            int middle = first + (last - first) / 2;
            if (dir * comparer(keys[middle], keys[first]) < 0)
                Matrix.Swap(keys, middle, first);

            if (dir * comparer(keys[last], keys[middle]) < 0)
            {
                Matrix.Swap(keys, last, middle);
                if (dir * comparer(keys[middle], keys[first]) < 0)
                    Matrix.Swap(keys, middle, first);
            }

            return middle;
        }
    }
}
