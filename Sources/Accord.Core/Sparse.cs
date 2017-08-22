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

namespace Accord.Math
{
    using System.Globalization;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Collections.Concurrent;

    /// <summary>
    ///   Extension methods for <see cref="Sparse{T}">sparse vectors</see>.
    /// </summary>
    /// 
    public static class Sparse
    {
        /// <summary>
        ///   Parses a string containing a sparse array in LibSVM
        ///   format into a <see cref="Sparse{T}"/> vector.
        /// </summary>
        ///
        /// <param name="values">An array of "index:value" strings indicating
        ///   where each value belongs in the sparse vector.</param>
        /// <param name="insertValueAtBeginning">Whether an intercept term should be added
        ///   at the beginning of the vector.</param>
        ///
        public static Sparse<double> Parse(string values, double? insertValueAtBeginning = null)
        {
            return Parse(values.Split(' '), insertValueAtBeginning: insertValueAtBeginning);
        }

        /// <summary>
        ///   Parses a string containing a sparse array in LibSVM
        ///   format into a <see cref="Sparse{T}"/> vector.
        /// </summary>
        ///
        /// <param name="values">An array of "index:value" strings indicating
        ///   where each value belongs in the sparse vector.</param>
        /// <param name="insertValueAtBeginning">Whether an intercept term should be added
        ///   at the beginning of the vector.</param>
        ///
        public static Sparse<double> Parse(string[] values, double? insertValueAtBeginning = null)
        {
            bool addIntercept = insertValueAtBeginning.HasValue && insertValueAtBeginning.Value != 0;

            int offset = addIntercept ? 1 : 0;

            var result = new Sparse<double>(values.Length + offset);

            for (int i = 0; i < values.Length; i++)
            {
                string[] element = values[i].Split(':');
                int oneBasedindex = Int32.Parse(element[0], System.Globalization.CultureInfo.InvariantCulture);
                if (oneBasedindex <= 0)
                    throw new FormatException("The given string contains 0 or negative indices (indices of sparse vectors in LibSVM format should begin at 1).");
                int zeroBasedIndex = oneBasedindex - 1; // LibSVM uses 1-based array format
                double value = Double.Parse(element[1], System.Globalization.CultureInfo.InvariantCulture);

                result.Indices[i + offset] = zeroBasedIndex + offset;
                result.Values[i + offset] = value;
            }

            if (addIntercept)
                result.Values[0] = insertValueAtBeginning.Value;

            return result;
        }

        /// <summary>
        ///   Converts an array of sparse vectors into a jagged matrix.
        /// </summary>
        /// 
        public static T[][] ToDense<T>(this Sparse<T>[] vectors)
            where T : IEquatable<T>
        {
            return ToDense(vectors, vectors.Columns());
        }

        /// <summary>
        ///   Converts an array of sparse vectors into a jagged matrix.
        /// </summary>
        /// 
        public static T[][] ToDense<T>(this Sparse<T>[] vectors, int length)
            where T : IEquatable<T>
        {
            T[][] dense = new T[vectors.Length][];
            for (int i = 0; i < dense.Length; i++)
                dense[i] = vectors[i].ToDense(length);
            return dense;
        }

        /// <summary>
        ///   Creates a sparse vector from a dense array.
        /// </summary>
        /// 
        public static Sparse<T> FromDense<T>(T[] dense, bool removeZeros = true)
            where T : IEquatable<T>
        {
            if (removeZeros)
            {
                T zero = default(T);

                int nonZeros = 0;
                for (int i = 0; i < dense.Length; i++)
                    if (!zero.Equals(dense[i]))
                        nonZeros++;

                var idx = new int[nonZeros];
                var values = new T[nonZeros];
                for (int i = 0, c = 0; i < dense.Length; i++)
                {
                    if (!zero.Equals(dense[i]))
                    {
                        idx[c] = i;
                        values[c] = dense[i];
                        c++;
                    }
                }

                return new Sparse<T>(idx, values);
            }
            else
            {
                int[] idx = new int[dense.Length];
                for (int i = 0; i < idx.Length; i++)
                    idx[i] = i;
                return new Sparse<T>(idx, dense);
            }
        }

        /// <summary>
        ///   Creates sparse vectors from dense arrays.
        /// </summary>
        /// 
        public static Sparse<T>[] FromDense<T>(T[][] dense, bool removeZeros = true)
            where T : IEquatable<T>
        {
            var result = new Sparse<T>[dense.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = FromDense(dense[i], removeZeros);
            return result;
        }

        /// <summary>
        ///   Gets the maximum number of columns (dimensions) 
        ///   that can be inferred from the given sparse vectors.
        /// </summary>
        /// 
        public static int Columns<T>(this Sparse<T>[] inputs)
            where T : IEquatable<T>
        {
            int max = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                int c = inputs[i].Length;
                if (c > max)
                    max = c;
            }

            return max;
        }

        /// <summary>
        ///   Creates a sparse vector from a dictionary mapping indices to values.
        /// </summary>
        /// 
        public static Sparse<double> FromDictionary(IDictionary<int, int> dictionary)
        {
            var indices = dictionary.Keys.ToArray();

            Array.Sort(indices);

            var values = new double[indices.Length];
            for (int i = 0; i < indices.Length; i++)
                values[i] = dictionary[indices[i]];

            return new Sparse<double>(indices, values);
        }
    }
}