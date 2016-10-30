// Accord Math Library
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

namespace Accord.Math
{
    using System.Globalization;
    using System;
    using System.Linq;
    using System.Collections.Generic;

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
        /// <param name="intercept">Whether an intercept term should be added
        ///   at the beginning of the vector.</param>
        ///
        public static Sparse<double> Parse(string[] values, double? intercept = null)
        {
            var result = new Sparse<double>(values.Length);

            int offset = intercept.HasValue ? 1 : 0;
            for (int i = 0; i < values.Length; i++)
            {
                string[] element = values[i].Split(':');
                int index = Int32.Parse(element[0], CultureInfo.InvariantCulture) - 1;
                double value = Double.Parse(element[1], CultureInfo.InvariantCulture);

                result.Indices[i] = index + offset;
                result.Values[i] = value;
            }

            if (intercept.HasValue)
                result.Values[0] = intercept.Value;

            return result;
        }

        /// <summary>
        ///   Converts an array of sparse vectors into a jagged matrix.
        /// </summary>
        /// 
        public static T[][] ToDense<T>(this Sparse<T>[] vectors)
            where T : IEquatable<T>
        {
            int max = 0;
            for (int i = 0; i < vectors.Length; i++)
            {
                int lastIndex = vectors[i].Indices[vectors[i].Indices.Length - 1];
                if (lastIndex > max)
                    max = lastIndex;
            }

            return ToDense(vectors, max + 1);
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
        public static Sparse<T> FromDense<T>(T[] dense)
            where T : IEquatable<T>
        {
            int[] idx = new int[dense.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;
            return new Sparse<T>(idx, dense);
        }

        /// <summary>
        ///   Creates sparse vectors from dense arrays.
        /// </summary>
        /// 
        public static Sparse<T>[] FromDense<T>(T[][] dense)
            where T : IEquatable<T>
        {
            var result = new Sparse<T>[dense.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = FromDense(dense[i]);
            return result;
        }

        /// <summary>
        ///   Gets the maximum number of columns (dimensions) 
        ///   that can be inferred from the given sparse vectors.
        /// </summary>
        /// 
        public static int Columns(this Sparse<double>[] inputs)
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
        ///   Adds the a sparse vector to a dense vector.
        /// </summary>
        /// 
        public static void Add(this Sparse<double> a, double[] b, double[] result)
        {
            for (int j = 0; j < b.Length; j++)
                result[j] = b[j];

            for (int j = 0; j < a.Indices.Length; j++)
                result[a.Indices[j]] += a.Values[j];
        }
    }
}