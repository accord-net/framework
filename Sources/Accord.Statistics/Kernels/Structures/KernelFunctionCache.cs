﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Statistics.Kernels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Collections;

    /// <summary>
    ///   Value cache for kernel function evaluations.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class works as a least-recently-used cache for elements
    ///   computed from a the kernel (Gram) matrix. Elements which have
    ///   not been needed for some time are discarded from the cache;
    ///   while elements which are constantly requested remains cached.</para>
    ///   
    /// <para>
    ///   The use of cache may speedup learning by a large factor; however
    ///   the actual speedup may vary according to the choice of cache size.</para>
    /// </remarks>
    /// 
    public class KernelFunctionCache : KernelFunctionCache<IKernel, double[]>
    {
        /// <summary>
        ///   Constructs a new <see cref="KernelFunctionCache"/>.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The inputs values.</param>
        /// 
        public KernelFunctionCache(IKernel kernel, double[][] inputs)
            : base(kernel, inputs)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="KernelFunctionCache"/>.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The inputs values.</param>
        /// <param name="cacheSize">
        ///   The size for the cache, measured in number of 
        ///   elements from the <paramref name="inputs"/> set.
        ///   Default is to use all elements.</param>
        /// 
        public KernelFunctionCache(IKernel kernel, double[][] inputs, int cacheSize)
            : base(kernel, inputs, cacheSize)
        {
        }
    }

    /// <summary>
    ///   Value cache for kernel function evaluations.
    /// </summary>
    /// 
    public class KernelFunctionCache<TKernel, TInput>
        where TKernel : IKernel<TInput>
    {

        private int size;
        private int capacity;

        private Dictionary<ulong, double> data;
        private LinkedList<ulong> lruIndices;
        private Dictionary<ulong, LinkedListNode<ulong>> lruIndicesLookupTable;

        private double[] diagonal;

        private TInput[] inputs;
        private TKernel kernel;

        private int misses;
        private int hits;

        double[][] matrix;



        /// <summary>
        ///   Gets the size of the cache,
        ///   measured in number of samples.
        /// </summary>
        /// 
        /// <value>The size of this cache.</value>
        /// 
        public int Size { get { return size; } }

        /// <summary>
        ///   Gets the total number of cache hits.
        /// </summary>
        /// 
        public int Hits { get { return hits; } }

        /// <summary>
        ///   Gets the total number of cache misses.
        /// </summary>
        /// 
        public int Misses { get { return misses; } }

        /// <summary>
        ///   Gets the percentage of the cache currently in use.
        /// </summary>
        /// 
        public double Usage
        {
            get
            {
                if (data == null)
                    return 0;

                return data.Count / (double)capacity;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the cache is enabled. If the value
        /// is false, it means the kernel function is being evaluated on-the-fly.
        /// </summary>
        /// 
        public bool Enabled { get; private set; }


        /// <summary>
        ///   Constructs a new <see cref="KernelFunctionCache"/>.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The inputs values.</param>
        /// 
        public KernelFunctionCache(TKernel kernel, TInput[] inputs)
            : this(kernel, inputs, inputs.Length)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="KernelFunctionCache"/>.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function.</param>
        /// <param name="inputs">The inputs values.</param>
        /// <param name="cacheSize">
        ///   The size for the cache, measured in number of 
        ///   elements from the <paramref name="inputs"/> set.
        ///   Default is to use all elements.</param>
        /// 
        public KernelFunctionCache(TKernel kernel, TInput[] inputs, int cacheSize)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            if (cacheSize < 0)
            {
                throw new ArgumentOutOfRangeException("cacheSize",
                    "The cache size must be non-negative.");
            }

            this.kernel = kernel;
            this.inputs = inputs;

            this.size = cacheSize;
            if (cacheSize > inputs.Length)
                this.size = inputs.Length;


            if (cacheSize >= inputs.Length)
            {
                // Create whole cache.
                matrix = new double[inputs.Length][];
                for (int i = 0; i < inputs.Length; i++)
                {
                    matrix[i] = new double[i];
                    for (int j = 0; j < matrix[i].Length; j++)
                        matrix[i][j] = kernel.Function(inputs[i], inputs[j]);
                }

#if DEBUG
                int total = matrix.GetNumberOfElements();
                int expected = (inputs.Length * (inputs.Length - 1)) / 2;
                Accord.Diagnostics.Debug.Assert(total == expected);
#endif
                Enabled = true;
            }

            else if (cacheSize > 0)
            {
                this.capacity = (size * (size - 1)) / 2;
                int collectionCapacity = (int)(1.1f * capacity);

                // Create lookup tables
                this.lruIndices = new LinkedList<ulong>();
                this.lruIndicesLookupTable =
                    new Dictionary<ulong, LinkedListNode<ulong>>(collectionCapacity);

                // Create cache for off-diagonal elements
                this.data = new Dictionary<ulong, double>(collectionCapacity);

                Enabled = true;
            }
            else
            {
                Enabled = false; // Values will be computed on-the-fly
            }

            // Create cache for diagonal elements
            this.diagonal = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                this.diagonal[i] = kernel.Function(inputs[i], inputs[i]);
        }

        /// <summary>
        ///   Attempts to retrieve the value of the kernel function
        ///   from the diagonal of the kernel matrix. If the value
        ///   is not available, it is immediately computed and inserted
        ///   in the cache.
        /// </summary>
        /// 
        /// <param name="i">Index of the point to compute.</param>
        /// 
        /// <remarks>The result of the kernel function k(p[i], p[i]).</remarks>
        /// 
        public double GetOrCompute(int i)
        {
            return diagonal[i];
        }


        /// <summary>
        ///   Attempts to retrieve the kernel function evaluated between point at index i
        ///   and j. If it is not cached, it will be computed and the cache will be updated.
        /// </summary>
        /// 
        /// <param name="i">The index of the first point <c>p</c> to compute.</param>
        /// <param name="j">The index of the second point <c>p</c> to compute.</param>
        /// 
        /// <remarks>The result of the kernel function k(p[i], p[j]).</remarks>
        /// 
        public double GetOrCompute(int i, int j)
        {
            if (i == j)
                return diagonal[i];

            if (matrix != null)
                return (j > i) ? matrix[j][i] : matrix[i][j];

            if (data == null)
                return kernel.Function(inputs[i], inputs[j]);


            // Keys should always be given as in
            // the order (i, j) with i > j, so we
            // always have (higher{i}, lower{j})

            if (j > i)
            {
                int t = i;
                i = j;
                j = t;
            }


            ulong key = (ulong)((i * (i - 1L)) / 2L) + (ulong)j;

            double value;

            // Check if the data is in the cache
            if (!data.TryGetValue(key, out value))
            {
                // It is not. Compute the function and update
                value = kernel.Function(inputs[i], inputs[j]);

                // Save evaluation
                data[key] = value;

                // If we are over capacity,
                if (data.Count > capacity)
                {
                    // The first entry must be removed to leave
                    // room for the previously computed value

                    LinkedListNode<ulong> first = lruIndices.First;
                    ulong discardedKey = first.Value;

                    // Remove the cached value for
                    // the least recently used key

                    data.Remove(discardedKey);
                    lruIndicesLookupTable.Remove(discardedKey);

                    // Avoid allocating memory by reusing the
                    // previously first node to hold the new
                    // data value and re-insert it at the end

                    lruIndices.RemoveFirst();
                    first.Value = key;
                    lruIndices.AddLast(first);

                    // Update the index lookup table
                    lruIndicesLookupTable[key] = first;
                }
                else
                {
                    // Register the use of the variable in the LRU list
                    lruIndicesLookupTable[key] = lruIndices.AddLast(key);
                }

                misses++;
            }
            else
            {
                // It is. Update the LRU list to indicate that the item has been used.
                LinkedListNode<ulong> node = lruIndicesLookupTable[key];

                // Remove from middle and add to the end
                lruIndices.Remove(node);
                lruIndices.AddLast(node);

                // Update the lookup table
                lruIndicesLookupTable[key] = node;

                hits++;
            }

            return value;
        }

        /// <summary>
        ///   Attempts to retrieve the value of the kernel function
        ///   from the diagonal of the kernel matrix. If the value
        ///   is not available, it is immediately computed and inserted
        ///   in the cache.
        /// </summary>
        /// 
        /// <param name="i">Index of the point to compute.</param>
        /// 
        /// <remarks>The result of the kernel function k(p[i], p[i]).</remarks>
        /// 
        public double this[int i]
        {
            get { return GetOrCompute(i); }
        }

        /// <summary>
        ///   Attempts to retrieve the kernel function evaluated between point at index i
        ///   and j. If it is not cached, it will be computed and the cache will be updated.
        /// </summary>
        /// 
        /// <param name="i">The index of the first point <c>p</c> to compute.</param>
        /// <param name="j">The index of the second point <c>p</c> to compute.</param>
        /// 
        /// <remarks>The result of the kernel function k(p[i], p[j]).</remarks>
        /// 
        public double this[int i, int j]
        {
            get { return GetOrCompute(i, j); }
        }

        /// <summary>
        ///   Clears the cache.
        /// </summary>
        /// 
        public void Clear()
        {
            if (data != null)
            {
                data.Clear();
                lruIndices.Clear();
                lruIndicesLookupTable.Clear();
            }

            if (matrix != null)
            {
                matrix = null;
            }
        }

        /// <summary>
        ///   Resets cache statistics.
        /// </summary>
        /// 
        public void Reset()
        {
            hits = 0;
            misses = 0;
        }

        /// <summary>
        ///   Gets the pair of indices associated with a given key.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// 
        /// <remarks>
        ///   This method has a very inefficient implementation and 
        ///   is intended to be used only for debugging purposes.
        /// </remarks>
        /// 
        /// <returns>A pair of indices of indicating which
        /// element from the Kernel matrix is associated
        /// with the given key.</returns>
        /// 
        public Tuple<int, int> GetIndexFromKey(ulong key)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    ulong exp = (ulong)((i * (i - 1L)) / 2L) + (ulong)j;
                    if (key == exp)
                        return Tuple.Create(i, j);
                }
            }

            return Tuple.Create(-1, -1);
        }

        /// <summary>
        ///   Gets the key from the given indices.
        /// </summary>
        /// 
        /// <param name="i">The index i.</param>
        /// <param name="j">The index j.</param>
        /// 
        /// <returns>The key associated with the given indices.</returns>
        /// 
        public ulong GetKeyFromIndex(int i, int j)
        {
            if (i < 0 || i >= inputs.Length)
                throw new ArgumentOutOfRangeException("i");

            if (j < 0 || j >= inputs.Length)
                throw new ArgumentOutOfRangeException("j");

            if (j > i)
            {
                int t = i;
                i = j;
                j = t;
            }

            return (ulong)((i * (i - 1L)) / 2L) + (ulong)j;
        }


        /// <summary>
        ///   Gets a copy of the data cache.
        /// </summary>
        /// 
        /// <returns>A copy of the data cache.</returns>
        /// 
        public IDictionary<Tuple<int, int>, double> GetDataCache()
        {
            var dict = new Dictionary<Tuple<int, int>, double>();

            if (data == null)
            {
                foreach (int[] idx in matrix.GetIndices(deep: true))
                    dict.Add(Tuple.Create(idx[0], idx[1]), matrix[idx[0]][idx[1]]);
            }
            else
            {
                foreach (var entry in data)
                    dict.Add(GetIndexFromKey(entry.Key), entry.Value);
            }
            return dict;
        }

        /// <summary>
        ///   Gets a copy of the Least Recently Used (LRU) List of
        ///   Kernel Matrix elements. Elements on the start of the
        ///   list have been used most; elements at the end are
        ///   about to be discarded from the cache.
        /// </summary>
        /// 
        /// <returns>The Least Recently Used list of kernel matrix elements.</returns>
        /// 
        public IList<Tuple<int, int>> GetLeastRecentlyUsedList()
        {
            if (lruIndices == null)
                throw new InvalidOperationException("The cache is not using a LRU list.");

            var list = new List<Tuple<int, int>>();
            foreach (ulong key in lruIndices)
                list.Add(GetIndexFromKey(key));

            return list;
        }
    }

}
