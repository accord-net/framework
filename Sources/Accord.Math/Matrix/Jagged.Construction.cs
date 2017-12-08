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
    using Accord.Math.Comparers;
    using Accord.Math.Random;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Jagged matrices.
    /// </summary>
    /// 
    /// <seealso cref="Matrix"/>
    /// <seealso cref="Vector"/>
    /// 
    public static partial class Jagged
    {
        /// <summary>
        ///   Creates a zero-valued matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the matrix to be created.</typeparam>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Zeros<T>(int rows, int columns)
        {
            T[][] matrix = new T[rows][];
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = new T[columns];
            return matrix;
        }

        /// <summary>
        ///   Creates a zero-valued rank-3 tensor.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the matrix to be created.</typeparam>
        /// <param name="rows">The number of rows in the tensor.</param>
        /// <param name="columns">The number of columns in the tensor.</param>
        /// <param name="depth">The number of channels in the tensor.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][][] Zeros<T>(int rows, int columns, int depth)
        {
            T[][][] matrix = new T[rows][][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new T[columns][];
                for (int j = 0; j < matrix[i].Length; j++)
                    matrix[i][j] = new T[depth];
            }

            return matrix;
        }

        /// <summary>
        ///   Creates a zero-valued matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the matrix to be created.</typeparam>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Ones<T>(int rows, int columns)
        {
            return Create<T>(rows, columns, Constants.One<T>());
        }

        /// <summary>
        ///   Creates a zero-valued matrix.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] Zeros(int rows, int columns)
        {
            return Zeros<double>(rows, columns);
        }

        /// <summary>
        ///   Creates a zero-valued rank-3 tensor.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the tensor.</param>
        /// <param name="columns">The number of columns in the tensor.</param>
        /// <param name="depth">The number of channels in the tensor.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] Zeros(int rows, int columns, int depth)
        {
            return Zeros<double>(rows, columns, depth);
        }

        /// <summary>
        ///   Creates a zero-valued matrix.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// 
        /// <returns>A vector of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] Ones(int rows, int columns)
        {
            return Ones<double>(rows, columns);
        }


        /// <summary>
        ///   Creates a jagged matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(int rows, int columns, T value)
        {
            var matrix = new T[rows][];
            for (int i = 0; i < rows; i++)
            {
                var row = matrix[i] = new T[columns];
                for (int j = 0; j < row.Length; j++)
                    row[j] = value;
            }

            return matrix;
        }

        /// <summary>
        ///   Creates a jagged matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="elementType">The type of the elements to be contained in the matrix.</param>
        /// <param name="shape">The number of dimensions that the matrix should have.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array Create(Type elementType, int[] shape, object value)
        {
            return create(elementType, shape, 0, value);
        }

        private static Array create(Type elementType, int[] shape, int dimension, object value)
        {
            Type arrayType = elementType.MakeArrayType(shape.Length - dimension - 1, jagged: true);
            Array array = Array.CreateInstance(arrayType, shape[dimension]);

            if (dimension < shape.Length - 1)
            {
                for (int i = 0; i < array.Length; i++)
                    array.SetValue(create(elementType, shape, dimension + 1, value), i);
            }
            else
            {
                for (int i = 0; i < array.Length; i++)
                    array.SetValue(value, i);
            }

            return array;
        }

        /// <summary>
        ///   Creates a jagged matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="shape">The number of dimensions that the matrix should have.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array Create<T>(int[] shape, T value)
        {
            return Create(typeof(T), shape, value);
        }

        /// <summary>
        ///   Creates a jagged matrix with all values set to zero.
        /// </summary>
        /// 
        /// <param name="elementType">The type of the elements to be contained in the matrix.</param>
        /// <param name="shape">The number of dimensions that the matrix should have.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array Zeros(Type elementType, params int[] shape)
        {
#if !NETSTANDARD1_4
            return Create(elementType, shape, elementType.GetDefaultValue());
#else
            return Create(elementType, shape, 0);
#endif
        }


        /// <summary>
        ///   Creates a jagged matrix with all values set to zero.
        /// </summary>
        /// 
        /// <param name="shape">The number of dimensions that the matrix should have.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array Zeros<T>(params int[] shape)
        {
            return Create(typeof(T), shape, 0);
        }

        /// <summary>
        ///   Creates a jagged matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="size">The number of rows and columns in the matrix.</param>
        /// <param name="value">The initial values for the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
        /// <seealso cref="Matrix.Create{T}(int, int, T)"/>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Square<T>(int size, T value)
        {
            return Create(size, size, value);
        }

        /// <summary>
        ///   Creates a matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="values">The initial values for the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(int rows, int columns, params T[] values)
        {
            if (values.Length == 0)
                return Zeros<T>(rows, columns);
            return values.Reshape(rows, columns).ToJagged();
        }

        /// <summary>
        ///   Creates a matrix with the given rows.
        /// </summary>
        /// 
        /// <param name="rows">The row vectors in the matrix.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(params T[][] rows)
        {
            return rows.Copy();
        }

        /// <summary>
        ///   Creates a matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="values">The initial values for the matrix.</param>
        /// <param name="transpose">Whether to transpose the matrix when copying or not. Default is false.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(int rows, int columns, T[][] values, bool transpose = false)
        {
            var result = Zeros<T>(rows, columns);
            Matrix.CopyTo(values, destination: result, transpose: transpose);
            return result;
        }

        /// <summary>
        ///   Creates a matrix with the given values.
        /// </summary>
        /// 
        /// <param name="values">The values in the matrix.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(T[,] values)
        {
            return values.ToJagged();
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the ones in the positions where <paramref name="mask"/>
        ///   are true, which are set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        ///   is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] OneHot<T>(bool[] mask)
        {
            return OneHot<T>(mask, Jagged.Create<T>(mask.Length, 2));
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] OneHot<T>(int[] indices)
        {
            return OneHot<T>(indices, indices.Max() + 1);
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
        /// </summary>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] OneHot(int[] indices)
        {
            return OneHot(indices, indices.Max() + 1);
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
        public static T[][] OneHot<T>(int[] indices, int columns)
        {
            return OneHot<T>(indices, Jagged.Create<T>(indices.Length, columns));
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
        /// </summary>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] OneHot(int[] indices, int columns)
        {
            return OneHot(indices, Jagged.Create<double>(indices.Length, columns));
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the ones in the positions where <paramref name="mask"/>
        ///   are true, which are set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        ///   is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] OneHot<T>(bool[] mask, T[][] result)
        {
            var one = Constants.One<T>();
            for (int i = 0; i < mask.Length; i++)
                if (mask[i])
                    result[i][0] = one;
                else
                    result[i][1] = one;
            return result;
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] OneHot<T>(int[] indices, T[][] result)
        {
            var one = Constants.One<T>();
            for (int i = 0; i < indices.Length; i++)
                result[i][indices[i]] = one;
            return result;
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which is set to one.
        /// </summary>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] OneHot(int[] indices, double[][] result)
        {
            for (int i = 0; i < indices.Length; i++)
                result[i][indices[i]] = 1;
            return result;
        }


        /// <summary>
        ///   Creates a matrix of k-hot vectors, where all values at each row are 
        ///   zero except for the ones in the positions where <paramref name="mask"/>
        ///   are true, which are set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        ///   is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] KHot<T>(bool[][] mask)
        {
            return KHot<T>(mask, Jagged.CreateAs<bool, T>(mask));
        }

        /// <summary>
        ///   Creates a matrix of k-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] KHot<T>(int[][] indices, int columns)
        {
            return KHot<T>(indices, Jagged.Create<T>(indices.Length, columns));
        }

        /// <summary>
        ///   Creates a matrix of k-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] KHot(int[][] indices, int columns)
        {
            return KHot(indices, Jagged.Create<double>(indices.Length, columns));
        }

        /// <summary>
        ///   Creates a matrix of k-hot vectors, where all values at each row are 
        ///   zero except for the ones in the positions where <paramref name="mask"/>
        ///   are true, which are set to one.
        /// </summary>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        ///   is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] KHot(bool[][] mask, int columns)
        {
            return KHot(mask, Jagged.Create<double>(mask.Length, columns));
        }

        /// <summary>
        ///   Creates a matrix of one-hot vectors, where all values at each row are 
        ///   zero except for the ones in the positions where <paramref name="mask"/>
        ///   are true, which are set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="mask">The boolean mask determining where ones will be placed.</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        ///   is one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] KHot<T>(bool[][] mask, T[][] result)
        {
            var one = Constants.One<T>();
            for (int i = 0; i < mask.Length; i++)
                for (int j = 0; j < mask[0].Length; j++)
                    if (mask[i][j])
                        result[i][j] = one;
            return result;
        }

        /// <summary>
        ///   Creates a matrix of k-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type for the matrix.</typeparam>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] KHot<T>(int[][] indices, T[][] result)
        {
            var one = Constants.One<T>();
            for (int i = 0; i < indices.Length; i++)
                for (int j = 0; j < indices[0].Length; j++)
                    result[i][indices[i][j]] = one;
            return result;
        }

        /// <summary>
        ///   Creates a matrix of k-hot vectors, where all values at each row are 
        ///   zero except for the indicated <paramref name="indices"/>, which are set to one.
        /// </summary>
        /// 
        /// <param name="indices">The rows's dimension which will be marked as one.</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] KHot(int[][] indices, double[][] result)
        {
            for (int i = 0; i < indices.Length; i++)
                for (int j = 0; j < indices[i].Length; j++)
                    result[i][indices[i][j]] = 1;
            return result;
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array CreateAs(Array matrix, Type type)
        {
            int[] outputShape = Matrix.GetShape(matrix, type);

#if !NETSTANDARD1_4
            // multidimensional or jagged -> jagged
            return Jagged.Create(elementType: type.GetInnerMostType(), shape: outputShape, value: type.GetDefaultValue());
#else
            return Jagged.Create(elementType: type.GetInnerMostType(), shape: outputShape, value: 0);
#endif
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] CreateAs<T>(T[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            T[][] r = new T[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new T[cols];
            return r;
        }

        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] CreateAs<T>(T[][] matrix)
        {
            T[][] r = new T[matrix.Length][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new T[matrix[i].Length];
            return r;
        }

        /// <summary>
        ///   Creates a 1xN matrix with a single row vector of size N.
        /// </summary>
        /// 
        public static T[][] RowVector<T>(params T[] values)
        {
            return new T[][] { values };
        }

        /// <summary>
        ///   Creates a Nx1 matrix with a single column vector of size N.
        /// </summary>
        /// 
        public static T[][] ColumnVector<T>(params T[] values)
        {
            T[][] column = new T[values.Length][];
            for (int i = 0; i < column.Length; i++)
                column[i] = new[] { values[i] };

            return column;
        }

        /// <summary>
        ///   Creates a square matrix with ones across its diagonal.
        /// </summary>
        /// 
        public static double[][] Identity(int size)
        {
            return Diagonal(size, 1.0);
        }

        /// <summary>
        ///   Creates a square matrix with ones across its diagonal.
        /// </summary>
        /// 
        public static T[][] Identity<T>(int size)
        {
            return Diagonal(size, Constants.One<T>());
        }

        /// <summary>
        ///   Creates a jagged magic square matrix.
        /// </summary>
        /// 
        public static double[][] Magic(int size)
        {
            return Matrix.Magic(size).ToJagged();
        }

        #region Diagonal matrices
        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int size, T value)
        {
            return Diagonal(size, value, Jagged.Create<T>(size, size));
        }

        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int size, T value, T[][] result)
        {
            for (int i = 0; i < size; i++)
                result[i][i] = value;
            return result;
        }

        /// <summary>
        ///   Returns a matrix of the given size with value on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int rows, int cols, T value)
        {
            return Diagonal(rows, cols, value, Jagged.Create<T>(rows, cols));
        }

        /// <summary>
        ///   Returns a matrix of the given size with value on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int rows, int cols, T value, T[][] result)
        {
            int min = Math.Min(rows, cols);
            for (int i = 0; i < min; i++)
                result[i][i] = value;
            return result;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(T[] values)
        {
            return Diagonal(values, Jagged.Create<T>(values.Length, values.Length));
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(T[] values, T[][] result)
        {
            for (int i = 0; i < values.Length; i++)
                result[i][i] = values[i];
            return result;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int size, T[] values)
        {
            return Diagonal(size, size, values);
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int size, T[] values, T[][] result)
        {
            return Diagonal(size, size, values, result);
        }

        /// <summary>
        ///   Returns a matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int rows, int cols, T[] values)
        {
            return Diagonal(rows, cols, values, Jagged.Create<T>(rows, cols));
        }

        /// <summary>
        ///   Returns a matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(int rows, int cols, T[] values, T[][] result)
        {
            int size = Math.Min(rows, Math.Min(cols, values.Length));
            for (int i = 0; i < size; i++)
                result[i][i] = values[i];
            return result;
        }

        /// <summary>
        ///   Returns a block-diagonal matrix with the given matrices on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Diagonal<T>(T[][][] blocks)
        {
            int rows = 0;
            int cols = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                rows += blocks[i].Rows();
                cols += blocks[i].Columns();
            }

            var result = Jagged.Create<T>(rows, cols);
            int currentRow = 0;
            int currentCol = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                for (int r = 0; r < blocks[i].Length; r++)
                {
                    for (int c = 0; c < blocks[i][r].Length; c++)
                        result[currentRow + r][currentCol + c] = blocks[i][r][c];
                }

                currentRow = blocks[i].Length;
                currentCol = blocks[i][0].Length;
            }

            return result;
        }
        #endregion


        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[][] CreateAs<TInput, TOutput>(TInput[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var r = new TOutput[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new TOutput[cols];
            return r;
        }

        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[][] CreateAs<TInput, TOutput>(TInput[][] matrix)
        {
            var r = new TOutput[matrix.Length][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new TOutput[matrix[i].Length];
            return r;
        }

        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[][][] CreateAs<TInput, TOutput>(TInput[][][] matrix)
        {
            var r = new TOutput[matrix.Length][][];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new TOutput[matrix[i].Length][];
                for (int j = 0; j < r[i].Length; j++)
                    r[i][j] = new TOutput[matrix[i][j].Length];
            }
            return r;
        }


        /// <summary>
        ///   Transforms a vector into a matrix of given dimensions.
        /// </summary>
        /// 
        public static T[][] Reshape<T>(T[] array, int rows, int cols, MatrixOrder order = MatrixOrder.Default)
        {
            return Jagged.Reshape(array, rows, cols, Jagged.Create<T>(rows, cols), order);
        }

        /// <summary>
        ///   Transforms a vector into a matrix of given dimensions.
        /// </summary>
        /// 
        public static T[][] Reshape<T>(this T[] array, int rows, int cols, T[][] result, MatrixOrder order = MatrixOrder.Default)
        {
            if (order == MatrixOrder.CRowMajor)
            {
                int k = 0;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        result[i][j] = array[k++];
            }
            else
            {
                int k = 0;
                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        result[i][j] = array[k++];
            }

            return result;
        }



        #region Random matrices
        /// <summary>
        ///   Creates a square matrix matrix with random data.
        /// </summary>
        /// 
        public static T[][] Random<T>(int size, IRandomNumberGenerator<T> generator,
            bool symmetric = false, T[][] result = null)
        {
            if (result == null)
                result = Jagged.Create<T>(size, size);

            if (!symmetric)
            {
                for (int i = 0; i < size; i++)
                    result[i] = generator.Generate(size);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    T[] row = generator.Generate(size / 2, result[i]);
                    for (int start = 0, end = size - 1; start < size / 2; start++, end--)
                        row[end] = row[start];
                }
            }

            return result;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix with random data.
        /// </summary>
        /// 
        public static T[][] Random<T>(int rows, int cols,
            IRandomNumberGenerator<T> generator, T[][] result = null)
        {
            if (result == null)
                result = Jagged.Create<T>(rows, cols);

            for (int i = 0; i < rows; i++)
                result[i] = generator.Generate(cols);
            return result;
        }

        #endregion




        /// <summary>
        ///   Enumerates through all elements in a matrix.
        /// </summary>
        /// 
        /// <param name="array">The array to be iterated.</param>
        /// <param name="shape">The full shape of <paramref name="array"/> .</param>
        /// 
        public static IEnumerable Enumerate(this Array array, int[] shape)
        {
            if (array.IsMatrix())
            {
                if (array.Rank != shape.Length)
                    throw new NotSupportedException();

                foreach (var e in array)
                    yield return e;
                yield break;
            }

            var arrays = new Stack<Array>();
            var counters = new Stack<int>();

            arrays.Push(array);
            counters.Push(0);
            int depth = 1;

            Array a = array;
            int i = 0;

            while (arrays.Count > 0)
            {
                if (i >= shape[depth - 1])
                {
                    a = arrays.Pop();
                    i = counters.Pop() + 1;
                    depth--;
                }
                else
                {
                    if (a == null || i >= a.Length)
                    {
                        if (depth == shape.Length)
                        {
                            yield return null;
                            i++;
                        }
                        else
                        {
                            arrays.Push(a);
                            counters.Push(i);
                            a = null;
                            i = 0;
                            depth++;
                        }
                    }
                    else
                    {
                        if (depth == shape.Length)
                        {
                            yield return a.GetValue(i);
                            i++;
                        }
                        else
                        {
                            arrays.Push(a);
                            counters.Push(i);
                            a = (Array)a.GetValue(i);
                            i = 0;
                            depth++;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Enumerates through all elements in a matrix.
        /// </summary>
        /// 
        /// <param name="array">The array to be iterated.</param>
        /// <param name="shape">The full shape of <paramref name="array"/> .</param>
        /// 
        public static IEnumerable<T> Enumerate<T>(this Array array, int[] shape)
        {
            if (array.IsMatrix())
            {
                if (array.Rank != shape.Length)
                    throw new NotSupportedException();

                foreach (var e in array)
                    yield return (T)e;
                yield break;
            }

            var arrays = new Stack<Array>();
            var counters = new Stack<int>();

            arrays.Push(array);
            counters.Push(0);
            int depth = 1;

            Array a = array;
            int i = 0;

            while (arrays.Count > 0)
            {
                if (i >= shape[depth - 1])
                {
                    a = arrays.Pop();
                    i = counters.Pop() + 1;
                    depth--;
                }
                else
                {
                    if (a == null || i >= a.Length)
                    {
                        if (depth == shape.Length)
                        {
                            int n = shape[shape.Length - 1];
                            for (; i < n; i++)
                                yield return default(T);
                        }
                        else
                        {
                            arrays.Push(a);
                            counters.Push(i);
                            a = null;
                            i = 0;
                            depth++;
                        }
                    }
                    else
                    {
                        if (depth == shape.Length)
                        {
                            T[] t = (T[])a;
                            for (; i < t.Length; i++)
                                yield return t[i];
                        }
                        else
                        {
                            arrays.Push(a);
                            counters.Push(i);
                            a = (Array)a.GetValue(i);
                            i = 0;
                            depth++;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Enumerates through all elements in a matrix.
        /// </summary>
        /// 
        /// <param name="array">The array to be iterated.</param>
        /// 
        public static IEnumerable<T> Enumerate<T>(this Array array)
        {
            if (array.IsMatrix())
            {
                foreach (var e in array)
                    yield return (T)e;
                yield break;
            }

            var arrays = new Stack<Array>();
            var counters = new Stack<int>();

            arrays.Push(array);
            counters.Push(0);
            int depth = 1;

            Array a = array;
            int i = 0;

            while (arrays.Count > 0)
            {
                if (i >= a.Length)
                {
                    a = arrays.Pop();
                    i = counters.Pop() + 1;
                    depth--;
                }
                else
                {
                    Object e = a.GetValue(i);
                    T[] next = e as T[];
                    if (next != null)
                    {
                        foreach (T t in next)
                            yield return t;
                        i++;
                    }
                    else
                    {
                        arrays.Push(a);
                        counters.Push(i);
                        a = (Array)e;
                        i = 0;
                        depth++;
                    }
                }
            }
        }

        /// <summary>
        ///   Enumerates through all elements in a matrix.
        /// </summary>
        /// 
        /// <param name="array">The array to be iterated.</param>
        /// 
        public static IEnumerable Enumerate(this Array array)
        {
            if (array.IsMatrix())
            {
                foreach (var e in array)
                    yield return e;
                yield break;
            }

            var arrays = new Stack<Array>();
            var counters = new Stack<int>();

            arrays.Push(array);
            counters.Push(0);
            int depth = 1;

            Array a = array;
            int i = 0;

            while (arrays.Count > 0)
            {
                if (i >= a.Length)
                {
                    a = arrays.Pop();
                    i = counters.Pop() + 1;
                    depth--;
                }
                else
                {
                    Object e = a.GetValue(i);
                    Array next = e as Array;
                    if (next == null)
                    {
                        yield return e;
                        i++;
                    }
                    else
                    {
                        arrays.Push(a);
                        counters.Push(i);
                        a = next;
                        i = 0;
                        depth++;
                    }
                }
            }
        }


        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static T[][] Transpose<T>(T[,] matrix)
        {
            int rows = matrix.Rows();
            if (rows == 0)
                return new T[rows][];
            int cols = matrix.Columns();

            T[][] result = new T[cols][];
            for (int j = 0; j < result.Length; j++)
            {
                result[j] = new T[rows];
                for (int i = 0; i < result[j].Length; i++)
                    result[j][i] = matrix[i, j];
            }

            return result;
        }
    }
}
