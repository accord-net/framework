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
    using Accord;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static partial class Matrix
    {
        /// <summary>
        ///   Adds a new dimension to an array with length 1.
        /// </summary>
        /// 
        /// <param name="array">The array.</param>
        /// <param name="dimension">The index where the dimension should be added.</param>
        /// 
        public static Array ExpandDimensions(this Array array, int dimension)
        {
            List<int> dimensions = array.GetLength().ToList();
            dimensions.Insert(dimension, 1);
            Array res = Array.CreateInstance(array.GetInnerMostType(), dimensions.ToArray());
            Buffer.BlockCopy(array, 0, res, 0, res.GetNumberOfBytes());
            return res;
        }

        /// <summary>
        ///   Removes dimensions of length 1 from the array.
        /// </summary>
        /// 
        /// <param name="array">The array.</param>
        /// 
        public static Array Squeeze(this Array array)
        {
            int[] dimensions = array.GetLength().Where(x => x != 1).ToArray();
            if (dimensions.Length == 0)
                dimensions = new[] { 1 };

            Array res;
            if (array.IsJagged())
            {
#if NETSTANDARD1_4
                throw new NotSupportedException("Squeeze with jagged arrays is not supported in .NET Standard 1.4.");
#else
                res = Jagged.Zeros(array.GetInnerMostType(), dimensions);
                Copy(array, res);
#endif
            }
            else
            {
                res = Matrix.Zeros(array.GetInnerMostType(), dimensions);
                Buffer.BlockCopy(array, 0, res, 0, res.GetNumberOfBytes());
            }

            return res;
        }

        /// <summary>
        ///   Transforms a tensor into a single vector.
        /// </summary>
        /// 
        /// <param name="array">An array.</param>
        /// <param name="order">The direction to perform copying. Pass
        ///   1 to perform a copy by reading the matrix in row-major order.
        ///   Pass 0 to perform a copy in column-major copy. Default is 1 
        ///   (row-major, c-style order).</param>
        /// 
        public static Array Flatten(this Array array, MatrixOrder order = MatrixOrder.CRowMajor)
        {
            Type t = array.GetInnerMostType();

            if (order == MatrixOrder.CRowMajor)
            {
                Array dst = Array.CreateInstance(t, array.Length);
#pragma warning disable CS0618 // Type or member is obsolete
                Buffer.BlockCopy(array, 0, dst, 0, dst.Length * Marshal.SizeOf(t));
#pragma warning restore CS0618 // Type or member is obsolete
                return dst;
            }
            else
            {
                Array r = Array.CreateInstance(t, array.Length);

                int c = 0;
                foreach (int[] idx in array.GetIndices(order: order))
                    r.SetValue(value: array.GetValue(idx), index: c++);

                return r;
            }
        }

        /// <summary>
        ///   Changes the length of individual dimensions in an array.
        /// </summary>
        /// 
        /// <param name="array">The array.</param>
        /// <param name="shape">The new shape.</param>
        /// <param name="order">The direction to perform copying. Pass
        ///   1 to perform a copy by reading the matrix in row-major order.
        ///   Pass 0 to perform a copy in column-major copy. Default is 1 
        ///   (row-major, c-style order).</param>
        /// 
        public static Array Reshape(this Array array, int[] shape, MatrixOrder order = MatrixOrder.CRowMajor)
        {
            Type t = array.GetInnerMostType();

            Array r = Array.CreateInstance(t, shape);

            IEnumerator<int[]> c = array.GetIndices().GetEnumerator();
            foreach (int[] idx in r.GetIndices(order: order))
            {
                c.MoveNext();
                r.SetValue(value: array.GetValue(c.Current), indices: idx);
            }

            return r;
        }

        /// <summary>
        ///   Converts the values of a tensor.
        /// </summary>
        /// 
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="array">The tensor to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static Array Convert<TOutput>(this Array array)
        {
            return Convert(array, typeof(TOutput));
        }

        /// <summary>
        ///   Converts the values of a tensor.
        /// </summary>
        /// 
        /// <param name="type">The type of the output.</param>
        /// <param name="array">The tensor to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static Array Convert(this Array array, Type type)
        {
            Array r = Matrix.Zeros(type, array.GetLength(deep: true));

            foreach (int[] idx in r.GetIndices(deep: true))
            {
                var value = ExtensionMethods.To(array.GetValue(deep: true, indices: idx), type);
                r.SetValue(value: value, deep: true, indices: idx);
            }
            return r;
        }

        /// <summary>
        ///   Returns a subtensor extracted from the current tensor.
        /// </summary>
        /// 
        /// <param name="source">The tensor to return the subvector from.</param>
        /// <param name="dimension">The dimension from which the indices should be extracted.</param>
        /// <param name="indices">Array of indices.</param>
        /// 
        public static Array Get(this Array source, int dimension, int[] indices)
        {
            int[] lengths = source.GetLength();
            lengths[dimension] = indices.Length;

            Type type = source.GetInnerMostType();
            Array r = Array.CreateInstance(type, lengths);

            for (int i = 0; i < indices.Length; i++)
                Set(r, dimension: dimension, index: i, value: Get(source, dimension: dimension, index: indices[i]));

            return r;
        }

        /// <summary>
        ///   Returns a subtensor extracted from the current tensor.
        /// </summary>
        /// 
        /// <param name="source">The tensor to return the subvector from.</param>
        /// <param name="dimension">The dimension from which the indices should be extracted.</param>
        /// <param name="index">The index.</param>
        /// 
        public static Array Get(this Array source, int dimension, int index)
        {
            return Get(source, dimension, index, index + 1);
        }

        /// <summary>
        ///   Returns a subtensor extracted from the current tensor.
        /// </summary>
        /// 
        /// <param name="source">The tensor to return the subvector from.</param>
        /// <param name="dimension">The dimension from which the indices should be extracted.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// 
        public static Array Get(this Array source, int dimension, int start, int end)
        {
            if (dimension != 0)
            {
                throw new NotImplementedException("Retrieving dimensions higher than zero has not been implemented" +
                    " yet. Please open a new issue at the issue tracker if you need such functionality.");
            }

            int[] length = source.GetLength();
            length = length.RemoveAt(dimension);
            int rows = end - start;
            if (length.Length == 0)
                length = new int[] { rows
    };

            Type type = source.GetInnerMostType();
            Array r = Array.CreateInstance(type, length);
            int rowSize = source.Length / source.GetLength(dimension);
#pragma warning disable CS0618 // Type or member is obsolete
            Buffer.BlockCopy(source, start * rowSize * Marshal.SizeOf(type), r, 0, rows * rowSize * Marshal.SizeOf(type));
#pragma warning restore CS0618 // Type or member is obsolete
            return r;
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="dimension">The dimension where indices refer to.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="index">The index.</param>
        /// 
        public static void Set(this Array destination, int dimension, int index, Array value)
        {
            Set(destination, dimension, index, index + 1, value);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="dimension">The dimension where indices refer to.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// 
        public static void Set(this Array destination, int dimension, int start, int end, Array value)
        {
            if (dimension != 0)
            {
                throw new NotImplementedException("Retrieving dimensions higher than zero has not been implemented" +
                    " yet. Please open a new issue at the issue tracker if you need such functionality.");
            }

            Type type = destination.GetInnerMostType();
            int rowSize = destination.Length / destination.GetLength(0);
            int length = end - start;
#pragma warning disable CS0618 // Type or member is obsolete
            Buffer.BlockCopy(value, 0, destination, start * rowSize * Marshal.SizeOf(type), length * rowSize * Marshal.SizeOf(type));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        ///   Returns true if a tensor is square.
        /// </summary>
        /// 
        public static bool IsSquare(this Array array)
        {
            int first = array.GetLength(0);
            for (int i = 1; i < array.Rank; i++)
                if (array.GetLength(i) != first)
                    return false;
            return true;
        }

        /// <summary>
        ///   Creates a zero-valued tensor.
        /// </summary>
        /// 
        /// <param name="shape">The number of dimensions that the tensor should have.</param>
        /// 
        /// <returns>A tensor of the specified shape.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array Zeros<T>(params int[] shape)
        {
            return Array.CreateInstance(typeof(T), shape);
        }

        /// <summary>
        ///   Creates a zero-valued tensor.
        /// </summary>
        /// 
        /// <param name="type">The type of the elements to be contained in the tensor.</param>
        /// <param name="shape">The number of dimensions that the tensor should have.</param>
        /// 
        /// <returns>A tensor of the specified shape.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array Zeros(Type type, params int[] shape)
        {
            return Array.CreateInstance(type, shape);
        }

        /// <summary>
        ///   Creates a tensor with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="shape">The number of dimensions that the matrix should have.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
        public static Array Create<T>(int[] shape, T value)
        {
            return Create(typeof(T), shape, value);
        }

        /// <summary>
        ///   Creates a tensor with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="elementType">The type of the elements to be contained in the matrix.</param>
        /// <param name="shape">The number of dimensions that the matrix should have.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
        public static Array Create(Type elementType, int[] shape, object value)
        {
            Array arr = Array.CreateInstance(elementType, shape);
            foreach (int[] idx in arr.GetIndices())
                arr.SetValue(value, idx);
            return arr;
        }

    }
}
