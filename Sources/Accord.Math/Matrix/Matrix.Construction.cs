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
    using System;
    using System.Collections.Generic;
    using Accord.Math.Random;
    using Accord.Compat;
    using System.Runtime.CompilerServices;
    using System.Linq;

    /// <summary>
    ///   Matrix major order. The default is to use C-style Row-Major order.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In computing, row-major order and column-major order describe methods for arranging 
    ///   multidimensional arrays in linear storage such as memory. In row-major order, consecutive 
    ///   elements of the rows of the array are contiguous in memory; in column-major order, 
    ///   consecutive elements of the columns are contiguous. Array layout is critical for correctly
    ///   passing arrays between programs written in different languages. It is also important for 
    ///   performance when traversing an array because accessing array elements that are contiguous 
    ///   in memory is usually faster than accessing elements which are not, due to caching. In some
    ///   media such as tape or NAND flash memory, accessing sequentially is orders of magnitude faster 
    ///   than nonsequential access.</para>
    /// 
    ///   <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="https://en.wikipedia.org/wiki/Row-major_order">
    ///         Wikipedia contributors. "Row-major order." Wikipedia, The Free Encyclopedia. Wikipedia, 
    ///         The Free Encyclopedia, 13 Feb. 2016. Web. 22 Mar. 2016.</a>
    ///       </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    public enum MatrixOrder
    {
        /// <summary>
        ///   Row-major order (C, C++, C#, SAS, Pascal, NumPy default).
        /// </summary>
        CRowMajor = 1,

        /// <summary>
        ///   Column-major oder (Fotran, MATLAB, R).
        /// </summary>
        /// 
        FortranColumnMajor = 0,

        /// <summary>
        ///   Default (Row-Major, C/C++/C# order).
        /// </summary>
        /// 
        Default = CRowMajor
    }

    public static partial class Matrix
    {

        #region Generic matrices
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
        public static T[,] Zeros<T>(int rows, int columns)
        {
            return new T[rows, columns];
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
        public static T[,,] Zeros<T>(int rows, int columns, int depth)
        {
            return new T[rows, columns, depth];
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
        public static T[,] Ones<T>(int rows, int columns)
        {
            return Create(rows, columns, Constants.One<T>());
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
        public static double[,] Zeros(int rows, int columns)
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
        public static double[,,] Zeros(int rows, int columns, int depth)
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
        public static double[,] Ones(int rows, int columns)
        {
            return Ones<double>(rows, columns);
        }

        /// <summary>
        ///   Creates a matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="value">The initial values for the vector.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
        /// <seealso cref="Jagged.Create{T}(int, int, T)"/>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Create<T>(int rows, int columns, T value)
        {
            var matrix = new T[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    matrix[i, j] = value;
            return matrix;
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
        public static T[,] Create<T>(int rows, int columns, T[,] values, bool transpose = false)
        {
            var result = Zeros<T>(rows, columns);
            Matrix.CopyTo(values, destination: result, transpose: transpose);
            return result;
        }

        /// <summary>
        ///   Creates a matrix with all values set to a given value.
        /// </summary>
        /// 
        /// <param name="size">The number of rows and columns in the matrix.</param>
        /// <param name="value">The initial values for the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
        /// <seealso cref="Jagged.Create{T}(int, int, T)"/>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Square<T>(int size, T value)
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
        public static T[,] Create<T>(int rows, int columns, params T[] values)
        {
            if (values.Length == 0)
                return Zeros<T>(rows, columns);
            return values.Reshape(rows, columns);
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
        public static T[,] Create<T>(params T[][] rows)
        {
            return rows.ToMatrix();
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
        public static T[,] Create<T>(T[,] values)
        {
            return (T[,])values.Clone();
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
        public static T[,] OneHot<T>(int[] indices)
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
        public static double[,] OneHot(int[] indices)
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
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] OneHot<T>(int[] indices, int columns)
        {
            return OneHot(indices, new T[indices.Length, columns]);
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
        public static double[,] OneHot(int[] indices, int columns)
        {
            return OneHot(indices, new double[indices.Length, columns]);
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
        public static T[,] OneHot<T>(int[] indices, T[,] result)
        {
            var one = Constants.One<T>();
            for (int i = 0; i < indices.Length; i++)
                result[i, indices[i]] = one;
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
        public static double[,] OneHot(int[] indices, double[,] result)
        {
            for (int i = 0; i < indices.Length; i++)
                result[i, indices[i]] = 1;
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
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] KHot<T>(int[][] indices, int columns)
        {
            return KHot(indices, new T[indices.Length, columns]);
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
        public static double[,] KHot(int[][] indices, int columns)
        {
            return KHot(indices, new double[indices.Length, columns]);
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
        public static T[,] KHot<T>(int[][] indices, T[,] result)
        {
            var one = Constants.One<T>();
            for (int i = 0; i < indices.Length; i++)
                for (int j = 0; j < indices[i].Length; j++)
                    result[i, indices[i][j]] = one;
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
        public static double[,] KHot(int[][] indices, double[,] result)
        {
            for (int i = 0; i < indices.Length; i++)
                for (int j = 0; j < indices[i].Length; j++)
                    result[i, indices[i][j]] = 1;
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
            int[] outputShape = GetShape(matrix, type);

            // multidimensional -> multidimensional
            return Array.CreateInstance(type.GetInnerMostType(), outputShape);
        }

        internal static int[] GetShape(Array matrix, Type type)
        {
            int[] outputShape = matrix.GetLength(deep: true);

            if (type.IsArray)
            {
                int inputRank = matrix.GetRank(deep: true);
                int outputRank = type.GetArrayRank(deep: true);

                if (inputRank > outputRank)
                {
                    outputShape = outputShape.Where(i => i != 1).ToArray();
                    int extra = outputRank - outputShape.Length;
                    if (extra > 0)
                        outputShape = outputShape.Concatenate(Accord.Math.Vector.Ones<int>(extra));
                }
                else if (inputRank < outputRank)
                {
                    int extra = outputRank - inputRank;
                    outputShape = outputShape.Concatenate(Accord.Math.Vector.Ones<int>(extra));
                }
            }

            return outputShape;
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] CreateAs<T>(T[,] matrix)
        {
            return new T[matrix.GetLength(0), matrix.GetLength(1)];
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] CreateAs<T>(T[][] matrix)
        {
            return new T[matrix.Length, matrix[0].Length];
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[,] CreateAs<TInput, TOutput>(TInput[,] matrix)
        {
            return new TOutput[matrix.GetLength(0), matrix.GetLength(1)];
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[,,] CreateAs<TInput, TOutput>(TInput[,,] matrix)
        {
            return new TOutput[matrix.GetLength(0), matrix.GetLength(1), matrix.GetLength(2)];
        }

        /// <summary>
        ///   Creates a new multidimensional matrix with the same shape as another matrix.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[,] CreateAs<TInput, TOutput>(TInput[][] matrix)
        {
            return new TOutput[matrix.Length, matrix[0].Length];
        }


        #endregion


        #region Diagonal matrices
        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(int size, T value)
        {
            return Diagonal(size, value, new T[size, size]);
        }

        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(int size, T value, T[,] result)
        {
            for (int i = 0; i < size; i++)
                result[i, i] = value;
            return result;
        }

        /// <summary>
        ///   Returns a matrix of the given size with value on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(int rows, int cols, T value)
        {
            return Diagonal(rows, cols, value, new T[rows, cols]);
        }

        /// <summary>
        ///   Returns a matrix of the given size with value on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(int rows, int cols, T value, T[,] result)
        {
            int min = Math.Min(rows, cols);
            for (int i = 0; i < min; i++)
                result[i, i] = value;
            return result;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(T[] values)
        {
            return Diagonal(values, new T[values.Length, values.Length]);
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(T[] values, T[,] result)
        {
            for (int i = 0; i < values.Length; i++)
                result[i, i] = values[i];
            return result;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(int size, T[] values)
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
        public static T[,] Diagonal<T>(int size, T[] values, T[,] result)
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
        public static T[,] Diagonal<T>(int rows, int cols, T[] values)
        {
            return Diagonal(rows, cols, values, new T[rows, cols]);
        }

        /// <summary>
        ///   Returns a matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(int rows, int cols, T[] values, T[,] result)
        {
            int size = Math.Min(rows, Math.Min(cols, values.Length));
            for (int i = 0; i < size; i++)
                result[i, i] = values[i];
            return result;
        }

        /// <summary>
        ///   Return a jagged matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
        [Obsolete("Please use Jagged.Diagonal instead.")]
        public static T[][] JaggedDiagonal<T>(T[] values)
        {
            return Accord.Math.Jagged.Diagonal(values);
        }

        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
        [Obsolete("Please use Jagged.Diagonal instead.")]
        public static T[][] JaggedDiagonal<T>(int size, T value)
        {
            return Accord.Math.Jagged.Diagonal<T>(size, value);
        }

        /// <summary>
        ///   Returns a block-diagonal matrix with the given matrices on its diagonal.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[,] Diagonal<T>(T[][,] blocks)
        {
            int rows = 0;
            int cols = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                rows += blocks[i].Rows();
                cols += blocks[i].Columns();
            }

            var result = new T[rows, cols];
            int currentRow = 0;
            int currentCol = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                for (int r = 0; r < blocks[i].GetLength(0); r++)
                {
                    for (int c = 0; c < blocks[i].GetLength(1); c++)
                        result[currentRow + r, currentCol + c] = blocks[i][r, c];
                }

                currentRow = blocks[i].GetLength(0);
                currentCol = blocks[i].GetLength(1);
            }

            return result;
        }

        #endregion

        #region Special matrices
        /// <summary>
        ///   Creates a square matrix with ones across its diagonal.
        /// </summary>
        /// 
        public static double[,] Identity(int size)
        {
            return Diagonal(size, 1.0);
        }

        /// <summary>
        ///   Creates a square matrix with ones across its diagonal.
        /// </summary>
        /// 
        public static T[,] Identity<T>(int size)
        {
            return Diagonal(size, Constants.One<T>());
        }

        /// <summary>
        ///   Returns the Identity matrix of the given size.
        /// </summary>
        /// 
        [Obsolete("Please use Jagged.Identity instead.")]
        public static double[][] JaggedIdentity(int size)
        {
            return Jagged.Identity(size);
        }

        /// <summary>
        ///   Creates a jagged magic square matrix.
        /// </summary>
        /// 
        [Obsolete("Please use Jagged.Magic instead.")]
        public static double[][] JaggedMagic(int size)
        {
            return Jagged.Magic(size);
        }

        /// <summary>
        ///   Creates a magic square matrix.
        /// </summary>
        /// 
        public static double[,] Magic(int size)
        {
            if (size < 3) throw new ArgumentOutOfRangeException("size", size,
                "The square size must be greater or equal to 3.");

            double[,] matrix = new double[size, size];


            // First algorithm: Odd order
            if ((size % 2) == 1)
            {
                int a = (size + 1) / 2;
                int b = (size + 1);

                for (int j = 0; j < size; j++)
                    for (int i = 0; i < size; i++)
                        matrix[i, j] = size * ((i + j + a) % size) + ((i + 2 * j + b) % size) + 1;
            }

            // Second algorithm: Even order (double)
            else if ((size % 4) == 0)
            {
                for (int j = 0; j < size; j++)
                    for (int i = 0; i < size; i++)
                        if (((i + 1) / 2) % 2 == ((j + 1) / 2) % 2)
                            matrix[i, j] = size * size - size * i - j;
                        else
                            matrix[i, j] = size * i + j + 1;
            }

            // Third algorithm: Even order (single)
            else
            {
                int n = size / 2;
                int p = (size - 2) / 4;
                double t;

                double[,] block = Matrix.Magic(n);

                for (int j = 0; j < n; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        double e = block[i, j];
                        matrix[i, j] = e;
                        matrix[i, j + n] = e + 2 * n * n;
                        matrix[i + n, j] = e + 3 * n * n;
                        matrix[i + n, j + n] = e + n * n;
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    // Swap M[i,j] and M[i+n,j]
                    for (int j = 0; j < p; j++)
                    {
                        t = matrix[i, j];
                        matrix[i, j] = matrix[i + n, j];
                        matrix[i + n, j] = t;
                    }
                    for (int j = size - p + 1; j < size; j++)
                    {
                        t = matrix[i, j];
                        matrix[i, j] = matrix[i + n, j];
                        matrix[i + n, j] = t;
                    }
                }

                // Continue swapping in the boundary
                t = matrix[p, 0];
                matrix[p, 0] = matrix[p + n, 0];
                matrix[p + n, 0] = t;

                t = matrix[p, p];
                matrix[p, p] = matrix[p + n, p];
                matrix[p + n, p] = t;
            }

            return matrix;
        }

        /// <summary>
        ///   Creates a centering matrix of size <c>N x N</c> in the
        ///   form <c>(I - 1N)</c> where <c>1N</c> is a matrix with 
        ///   all elements equal to <c>1 / N</c>.
        /// </summary>
        /// 
        public static double[,] Centering(int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                    "The size of the centering matrix must be a positive integer.");
            }

            double[,] C = Matrix.Square(size, -1.0 / size);

            for (int i = 0; i < size; i++)
                C[i, i] = 1.0 - 1.0 / size;

            return C;
        }
        #endregion

        #region Random matrices
        /// <summary>
        ///   Creates a square matrix matrix with random data.
        /// </summary>
        /// 
        public static T[,] Random<T>(int size, IRandomNumberGenerator<T> generator, bool symmetric = false, T[,] result = null)
        {
            if (result == null)
                result = new T[size, size];

            if (!symmetric)
            {
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        result[i, j] = generator.Generate();
            }
            else
            {
                for (int i = 0; i < size; i++)
                    for (int j = i; j < size; j++)
                        result[j, i] = result[i, j] = generator.Generate();
            }

            return result;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix with random data.
        /// </summary>
        /// 
        public static T[,] Random<T>(int rows, int cols, IRandomNumberGenerator<T> generator, T[,] result = null)
        {
            if (result == null)
                result = new T[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = generator.Generate();
            return result;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix random data drawn from a given distribution.
        /// </summary>
        /// 
        [Obsolete("Please use INumberGenerator<T> instead.")]
#pragma warning disable 0618
        public static double[,] Random(int rows, int cols, IRandomNumberGenerator generator)
        {
            return Random<double>(rows, cols, new RandomNumberGeneratorAdapter(generator));
        }
#pragma warning restore 0618

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Random instead.")]
        public static float[] Random(int size, float minValue, float maxValue)
        {
            return Accord.Math.Vector.Random(size, minValue, maxValue);
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Random instead.")]
        public static double[] Random(int size, double minValue, double maxValue)
        {
            return Accord.Math.Vector.Random(size, minValue, maxValue);
        }

        /// <summary>
        ///   Creates a vector with random data drawn from a given distribution.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Random instead.")]
        public static double[] Random(int size, IRandomNumberGenerator generator)
        {
            if (generator == null)
                throw new ArgumentNullException("generator");

            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            double[] vector = new double[size];
            for (int i = 0; i < size; i++)
                vector[i] = generator.Next();

            return vector;
        }
        #endregion


        #region Vector creation
        /// <summary>
        ///   Creates a 1xN matrix with a single row vector of size N.
        /// </summary>
        /// 
        public static T[,] RowVector<T>(params T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T[,] matrix = new T[1, values.Length];
            for (int i = 0; i < values.Length; i++)
                matrix[0, i] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Creates a Nx1 matrix with a single column vector of size N.
        /// </summary>
        /// 
        public static T[,] ColumnVector<T>(params T[] values)
        {
            T[,] matrix = new T[values.Length, 1];
            for (int i = 0; i < values.Length; i++)
                matrix[i, 0] = values[i];
            return matrix;
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Create instead.")]
        public static T[] Vector<T>(int n, T[] values)
        {
            return Accord.Math.Vector.Create(values);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Create instead.")]
        public static T[] Vector<T>(int n, T value)
        {
            return Accord.Math.Vector.Create(n, value);

        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static double[] Vector(double a, double b, double increment = 1)
        {
            return Accord.Math.Vector.Interval(a, b, (double)increment);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static int[] Vector(int a, int b, int increment = 1)
        {
            return Accord.Math.Vector.Interval(a, b, (double)increment);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static double[] Vector(double a, double b, int points)
        {
            return Accord.Math.Vector.Interval(a, b, points);
        }
        #endregion

        #region Special vectors
        /// <summary>
        ///   Creates a index vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Range instead.")]
        public static int[] Indices(int from, int to)
        {
            if (from > to)
                return Accord.Math.Vector.Range(from - 1, to - 1);
            return Accord.Math.Vector.Range(from, to);
        }

        /// <summary>
        ///   Creates a index vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Range instead.")]
        public static int[] Indices(int to)
        {
            return Accord.Math.Vector.Range(0, to);
        }

        /// <summary>
        ///   Gets the total length over all dimensions of an array.
        /// </summary>
        /// 
        public static int GetTotalLength(this Array array, bool deep = true, bool rectangular = true)
        {
            if (deep && IsJagged(array))
            {
                if (rectangular)
                {
                    int rest = GetTotalLength(array.GetValue(0) as Array, deep);
                    return array.Length * rest;
                }
                else
                {
                    int sum = 0;
                    for (int i = 0; i < array.Length; i++)
                        sum += GetTotalLength(array.GetValue(i) as Array, deep);
                    return sum;
                }
            }

            return array.Length;
        }

        /// <summary>
        ///   Gets the length of each dimension of an array.
        /// </summary>
        /// 
        /// <param name="array">The array.</param>
        /// <param name="deep">Pass true to retrieve all dimensions of the array,
        ///   even if it contains nested arrays (as in jagged matrices)</param>
        /// <param name="max">Gets the maximum length possible for each dimension (in case
        ///   the jagged matrices has different lengths).</param>
        /// 
        public static int[] GetLength(this Array array, bool deep = true, bool max = false)
        {
            if (array == null)
                return new int[] { -1 };
            if (array.Rank == 0)
                return new int[0];

            if (deep && IsJagged(array))
            {
                if (array.Length == 0)
                    return new int[0];

                int[] rest;
                if (!max)
                {
                    rest = GetLength(array.GetValue(0) as Array, deep);
                }
                else
                {
                    // find the max
                    rest = GetLength(array.GetValue(0) as Array, deep);
                    for (int i = 1; i < array.Length; i++)
                    {
                        int[] r = GetLength(array.GetValue(i) as Array, deep);

                        for (int j = 0; j < r.Length; j++)
                        {
                            if (r[j] > rest[j])
                                rest[j] = r[j];
                        }
                    }
                }

                return array.Length.Concatenate(rest);
            }

            int[] vector = new int[array.Rank];
            for (int i = 0; i < vector.Length; i++)
                vector[i] = array.GetUpperBound(i) + 1;
            return vector;
        }

        /// <summary>
        ///   Gets the rank of an array type.
        /// </summary>
        /// 
        /// <param name="type">The type of the array.</param>
        /// <param name="deep">Pass true to retrieve all dimensions of the array,
        ///   even if it contains nested arrays (as in jagged matrices)</param>
        /// 
        public static int GetArrayRank(this Type type, bool deep = true)
        {
            if (type.IsArray == false || type.GetArrayRank() == 0)
                return 0;

            if (deep && IsJagged(type))
                return type.GetArrayRank() + GetArrayRank(type.GetElementType(), deep);

            return type.GetArrayRank();
        }

        /// <summary>
        ///   Gets the rank of an array.
        /// </summary>
        /// 
        /// <param name="array">The array.</param>
        /// <param name="deep">Pass true to retrieve all dimensions of the array,
        ///   even if it contains nested arrays (as in jagged matrices)</param>
        /// 
        public static int GetRank(this Array array, bool deep = true)
        {
            return array.GetType().GetArrayRank(deep: deep);
        }



        /// <summary>
        ///   Trims the specified array, removing zero and empty entries from arrays.
        /// </summary>
        /// 
        /// <param name="array">The array to be trimmed.</param>
        /// 
        public static Array Trim(this Array array)
        {
            if (array.IsMatrix())
                throw new Exception();

            var list = new List<object>();
            for (int i = 0; i < array.Length; i++)
            {
                var element = array.GetValue(i);
                if (element != null)
                {
                    Array a = element as Array;
                    if (a != null)
                    {
                        a = Trim(a);
                        if (a != element)
                        {
                            array.SetValue(a, i);
                            element = a;
                        }
                    }
                    list.Add(element);
                }
            }

            if (list.Count == array.Length)
                return array;

            Array newArray = Array.CreateInstance(array.GetType().GetElementType(), list.Count);
            for (int i = 0; i < list.Count; i++)
                newArray.SetValue(list[i], i);

            return newArray;
        }

        /// <summary>
        ///   Determines whether an array is a jagged array 
        ///   (containing inner arrays as its elements).
        /// </summary>
        /// 
        public static bool IsJagged(this Array array)
        {
            if (array.Length == 0)
                return array.Rank == 1;
            return array.GetType().GetElementType().IsArray;
        }

        /// <summary>
        ///   Determines whether an array is an multidimensional array.
        /// </summary>
        /// 
        public static bool IsMatrix(this Array array)
        {
            return array.Rank > 1;
        }

        /// <summary>
        ///   Determines whether an array is a vector.
        /// </summary>
        /// 
        public static bool IsVector(this Array array)
        {
            return array.Rank == 1 && !IsJagged(array);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static int[] Interval(int from, int to)
        {
            return Accord.Math.Vector.Interval(from, to);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static double[] Interval(DoubleRange range, double stepSize)
        {
            return Interval(range.Min, range.Max, stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static double[] Interval(double from, double to, double stepSize)
        {
            return Accord.Math.Vector.Interval(from, to, stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static float[] Interval(float from, float to, double stepSize)
        {
            return Accord.Math.Vector.Interval(from, to, (float)stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static double[] Interval(DoubleRange range, int steps)
        {
            return Accord.Math.Vector.Interval(range, steps);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Interval instead.")]
        public static double[] Interval(double from, double to, int steps)
        {
            return Accord.Math.Vector.Interval(from, to, steps);
        }

        /// <summary>
        ///   Obsolete. Please specify the number of steps instead of the step size for the rows and columns.
        /// </summary>
        /// 
        [Obsolete("Please specify the number of steps instead of the step size for the rows and columns.")]
        public static double[][] Mesh(
            double rowMin, double rowMax, double rowStepSize,
            double colMin, double colMax, double colStepSize)
        {
            double[] x = Accord.Math.Vector.Interval(rowMin, rowMax, rowStepSize);
            double[] y = Accord.Math.Vector.Interval(colMin, colMax, colStepSize);
            double[][] mesh = Matrix.Cartesian(x, y);
            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        public static int[][] Mesh(
            int rowMin, int rowMax,
            int colMin, int colMax)
        {
            int[] x = Accord.Math.Vector.Interval(rowMin, rowMax);
            int[] y = Accord.Math.Vector.Interval(colMin, colMax);
            int[][] mesh = Matrix.Cartesian(x, y);
            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method can be used to generate all
        /// // possible (x,y) pairs between two ranges. 
        /// 
        /// // We can create a grid as
        /// double[][] grid = Matrix.Mesh
        /// (
        ///     rowMin: 0, rowMax: 1, rowSteps: 10,
        ///     colMin: 0, colMax: 1, colSteps: 5
        /// );
        ///
        /// // Now we can plot the points on-screen
        /// ScatterplotBox.Show("Grid (fixed steps)", grid).Hold();
        /// </code>
        /// 
        /// <para>
        ///   The resulting image is shown below. </para>
        ///   <img src="..\images\grid-fixed-steps.png" /> 
        /// </example>
        /// 
        public static double[][] Mesh(
            double rowMin, double rowMax, int rowSteps,
            double colMin, double colMax, int colSteps)
        {
            double[] x = Accord.Math.Vector.Interval(rowMin, rowMax, rowSteps);
            double[] y = Accord.Math.Vector.Interval(colMin, colMax, colSteps);
            double[][] mesh = Matrix.Cartesian(x, y);
            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method can be used to generate all
        /// // possible (x,y) pairs between two ranges. 
        /// 
        /// // We can create a grid as
        /// double[][] grid = Matrix.Mesh
        /// (
        ///     rowMin: 0, rowMax: 1, rowSteps: 10,
        ///     colMin: 0, colMax: 1, colSteps: 5
        /// );
        ///
        /// // Now we can plot the points on-screen
        /// ScatterplotBox.Show("Grid (fixed steps)", grid).Hold();
        /// </code>
        /// 
        /// <para>
        ///   The resulting image is shown below. </para>
        ///   <img src="..\images\grid-fixed-steps.png" /> 
        /// </example>
        /// 
        public static double[][] Mesh(
            DoubleRange rowRange, int rowSteps,
            DoubleRange colRange, int colSteps)
        {
            double[] x = Accord.Math.Vector.Interval(rowRange, rowSteps);
            double[] y = Accord.Math.Vector.Interval(colRange, colSteps);
            double[][] mesh = Matrix.Cartesian(x, y);
            return mesh;
        }

        /// <summary>
        ///   Obsolete. Please specify the number of steps instead of the step size for the rows and columns.
        /// </summary>
        /// 
        [Obsolete("Please specify the number of steps instead of the step size for the rows and columns.")]
        public static double[][] Mesh(
            DoubleRange rowRange, DoubleRange colRange,
            double rowStepSize, double colStepSize)
        {
            double[] x = Accord.Math.Vector.Interval(rowRange, rowStepSize);
            double[] y = Accord.Math.Vector.Interval(colRange, colStepSize);
            double[][] mesh = Matrix.Cartesian(x, y);
            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <param name="x">The values to be replicated vertically.</param>
        /// <param name="y">The values to be replicated horizontally.</param>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method generates all possible (x,y) pairs
        /// // between two vector of points. For example, let's
        /// // suppose we have the values:
        /// //
        /// double[] a = { 0, 1 };
        /// double[] b = { 0, 1 };
        /// 
        /// // We can create a grid as
        /// double[][] grid = a.Mesh(b);
        /// 
        /// // the result will be:
        /// double[][] expected =
        /// {
        ///     new double[] { 0, 0 },
        ///     new double[] { 0, 1 },
        ///     new double[] { 1, 0 },
        ///     new double[] { 1, 1 },
        /// };
        /// </code>
        /// </example>
        /// 
        public static T[][] Mesh<T>(this T[] x, T[] y)
        {
            return Matrix.Cartesian(x, y);
        }

        /// <summary>
        ///   Generates a 2-D mesh grid from two vectors <c>a</c> and <c>b</c>,
        ///   generating two matrices <c>len(a)</c> x <c>len(b)</c> with all
        ///   all possible combinations of values between the two vectors. This
        ///   method is analogous to MATLAB/Octave's <c>meshgrid</c> function.
        /// </summary>
        ///
        /// <returns>A tuple containing two matrices: the first containing values
        /// for the x-coordinates and the second for the y-coordinates.</returns>
        /// 
        /// <example>
        /// // The MeshGrid method generates two matrices that can be
        /// // used to generate all possible (x,y) pairs between two
        /// // vector of points. For example, let's suppose we have
        /// // the values:
        /// //
        /// double[] a = { 1, 2, 3 };
        /// double[] b = { 4, 5, 6 };
        /// 
        /// // We can create a grid
        /// var grid = a.MeshGrid(b);
        /// 
        /// // get the x-axis values     //        | 1   1   1 |
        /// double[,] x = grid.Item1;    //  x =   | 2   2   2 |
        ///                              //        | 3   3   3 |
        /// 
        /// // get the y-axis values     //        | 4   5   6 |
        /// double[,] y = grid.Item2;    //  y =   | 4   5   6 |
        ///                              //        | 4   5   6 |
        /// 
        /// // we can either use those matrices separately (such as for plotting 
        /// // purposes) or we can also generate a grid of all the (x,y) pairs as
        /// //
        /// double[,][] xy = x.ApplyWithIndex((v, i, j) => new[] { x[i, j], y[i, j] });
        ///
        /// // The result will be
        /// // 
        /// //         |  (1, 4)   (1, 5)   (1, 6)  |
        /// //  xy  =  |  (2, 4)   (2, 5)   (2, 6)  |
        /// //         |  (3, 4)   (3, 5)   (3, 6)  |
        /// </example>
        ///
        public static Tuple<T[,], T[,]> MeshGrid<T>(this T[] x, T[] y)
        {
            var X = new T[x.Length, y.Length];
            var Y = new T[x.Length, y.Length];
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < y.Length; j++)
                {
                    X[i, j] = x[i];
                    Y[i, j] = y[j];
                }
            }

            return Tuple.Create(X, Y);
        }
        #endregion


        #region Combine
        /// <summary>
        ///   Combines two vectors horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T[] a, params T[] b)
        {
            T[] r = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                r[i + a.Length] = b[i];

            return r;
        }

        /// <summary>
        ///   Combines a vector and a element horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T[] vector, T element)
        {
            T[] r = new T[vector.Length + 1];
            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i];

            r[vector.Length] = element;

            return r;
        }

        /// <summary>
        ///   Combines a vector and a element horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T element, T[] vector)
        {
            T[] r = new T[vector.Length + 1];

            r[0] = element;

            for (int i = 0; i < vector.Length; i++)
                r[i + 1] = vector[i];

            return r;
        }

        /// <summary>
        ///   Combines a matrix and a vector horizontally.
        /// </summary>
        /// 
        public static T[,] Concatenate<T>(this T[,] matrix, T[] vector)
        {
            return matrix.InsertColumn(vector);
        }

        /// <summary>
        ///   Combines two matrices horizontally.
        /// </summary>
        /// 
        public static T[,] Concatenate<T>(this T[,] a, T[,] b)
        {
            return Concatenate(new[] { a, b });
        }

        /// <summary>
        ///   Combines two matrices horizontally.
        /// </summary>
        /// 
        public static T[][] Concatenate<T>(this T[][] a, T[][] b)
        {
            return Concatenate(new[] { a, b });
        }

        /// <summary>
        ///   Combines a matrix and a vector horizontally.
        /// </summary>
        /// 
        public static T[,] Concatenate<T>(params T[][,] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                cols += matrices[i].GetLength(1);
                if (matrices[i].GetLength(0) > rows)
                    rows = matrices[i].GetLength(0);
            }

            T[,] r = new T[rows, cols];


            int c = 0;
            for (int k = 0; k < matrices.Length; k++)
            {
                int currentRows = matrices[k].GetLength(0);
                int currentCols = matrices[k].GetLength(1);

                for (int j = 0; j < currentCols; j++)
                {
                    for (int i = 0; i < currentRows; i++)
                    {
                        r[i, c] = matrices[k][i, j];
                    }
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combines a matrix and a vector horizontally.
        /// </summary>
        /// 
        public static T[][] Concatenate<T>(params T[][][] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                cols += matrices[i][0].Length;
                if (matrices[i].Length > rows)
                    rows = matrices[i].Length;
            }

            T[][] r = new T[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new T[cols];


            int c = 0;
            for (int k = 0; k < matrices.Length; k++)
            {
                int currentRows = matrices[k].Length;
                int currentCols = matrices[k][0].Length;

                for (int j = 0; j < currentCols; j++)
                {
                    for (int i = 0; i < currentRows; i++)
                    {
                        r[i][c] = matrices[k][i][j];
                    }
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combine vectors horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T[][] vectors)
        {
            int size = 0;
            for (int i = 0; i < vectors.Length; i++)
                size += vectors[i].Length;

            T[] r = new T[size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++)
                    r[c++] = vectors[i][j];

            return r;
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(this T[] a, T[] b)
        {
            return Stack(new[] { a, b });
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[][] Stack<T>(this T[][] a, T[][] b)
        {
            return Stack(new T[][][] { a, b });
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(params T[][] vectors)
        {
            return vectors.ToMatrix();
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(params T[] elements)
        {
            return elements.Transpose();
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(this T[] vector, T element)
        {
            return vector.Concatenate(element).Transpose();
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(params T[][,] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                rows += matrices[i].GetLength(0);
                if (matrices[i].GetLength(1) > cols)
                    cols = matrices[i].GetLength(1);
            }

            T[,] r = new T[rows, cols];

            int c = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].GetLength(0); j++)
                {
                    for (int k = 0; k < matrices[i].GetLength(1); k++)
                        r[c, k] = matrices[i][j, k];
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(this T[,] matrix, T[] vector)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[,] r = new T[rows + 1, cols];

            Array.Copy(matrix, r, matrix.Length);

            for (int i = 0; i < vector.Length; i++)
                r[rows, i] = vector[i];

            return r;
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        public static T[][] Stack<T>(params T[][][] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                rows += matrices[i].Length;
                if (matrices[i].Length == 0)
                    continue;

                if (matrices[i][0].Length > cols)
                    cols = matrices[i][0].Length;
            }

            T[][] r = new T[rows][];
            for (int i = 0; i < rows; i++)
                r[i] = new T[cols];

            int c = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].Length; j++)
                {
                    for (int k = 0; k < matrices[i][j].Length; k++)
                        r[c][k] = matrices[i][j][k];
                    c++;
                }
            }

            return r;
        }
        #endregion

        #region Expand


        /// <summary>
        ///   Expands a data vector given in summary form.
        /// </summary>
        /// 
        /// <param name="vector">A base vector.</param>
        /// <param name="count">An array containing by how much each line should be replicated.</param>
        /// 
        public static T[] Expand<T>(T[] vector, int[] count)
        {
            var expansion = new List<T>();
            for (int i = 0; i < count.Length; i++)
                for (int j = 0; j < count[i]; j++)
                    expansion.Add(vector[i]);

            return expansion.ToArray();
        }

        /// <summary>
        ///   Expands a data matrix given in summary form.
        /// </summary>
        /// 
        /// <param name="matrix">A base matrix.</param>
        /// <param name="count">An array containing by how much each line should be replicated.</param>
        /// 
        public static T[,] Expand<T>(T[,] matrix, int[] count)
        {
            var expansion = new List<T[]>();
            for (int i = 0; i < count.Length; i++)
                for (int j = 0; j < count[i]; j++)
                    expansion.Add(matrix.GetRow(i));

            return expansion.ToArray().ToMatrix();
        }
        #endregion

        #region Split
        /// <summary>
        ///   Splits a given vector into a smaller vectors of the given size.
        ///   This operation can be reverted using <see cref="Merge{T}(T[][], int)"/>.
        /// </summary>
        /// 
        /// <param name="vector">The vector to be splitted.</param>
        /// <param name="size">The size of the resulting vectors.</param>
        /// 
        /// <returns>An array of vectors containing the subdivisions of the given vector.</returns>
        /// 
        public static T[][] Split<T>(this T[] vector, int size)
        {
            int n = vector.Length / size;
            T[][] r = new T[n][];
            for (int i = 0; i < n; i++)
            {
                T[] ri = r[i] = new T[size];
                for (int j = 0; j < size; j++)
                    ri[j] = vector[j * n + i];
            }
            return r;
        }

        /// <summary>
        ///   Merges a series of vectors into a single vector. This
        ///   operation can be reverted using <see cref="Split{T}(T[], int)"/>.
        /// </summary>
        /// 
        /// <param name="vectors">The vectors to be merged.</param>
        /// <param name="size">The size of the inner vectors.</param>
        /// 
        /// <returns>A single array containing the given vectors.</returns>
        /// 
        public static T[] Merge<T>(this T[][] vectors, int size)
        {
            int n = vectors.Length * size;
            T[] r = new T[n * size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++, c++)
                    r[c] = vectors[i][j];

            return r;
        }

        /// <summary>
        ///   Merges a series of vectors into a single vector. This
        ///   operation can be reverted using <see cref="Split{T}(T[], int)"/>.
        /// </summary>
        /// 
        /// <param name="vectors">The vectors to be merged.</param>
        /// 
        /// <returns>A single array containing the given vectors.</returns>
        /// 
        public static T[] Merge<T>(this T[][] vectors)
        {
            int size = 0;
            for (int i = 0; i < vectors.Length; i++)
                size += vectors[i].Length;
            T[] r = new T[size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++, c++)
                    r[c] = vectors[i][j];

            return r;
        }
        #endregion

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="all">How many rows and columns to add at each side of the matrix.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int all)
        {
            return Pad(matrix, all, all, all, all);
        }

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="rightLeft">How many columns to add at the sides of the matrix.</param>
        /// <param name="topBottom">How many rows to add at the bottom and top of the matrix.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int topBottom, int rightLeft)
        {
            return Pad(matrix, topBottom, rightLeft, topBottom, rightLeft);
        }

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="bottom">How many rows to add at the bottom.</param>
        /// <param name="top">How many rows to add at the top.</param>
        /// <param name="sides">How many columns to add at the sides.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int top, int sides, int bottom)
        {
            return Pad(matrix, top, sides, bottom, sides);
        }

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="bottom">How many rows to add at the bottom.</param>
        /// <param name="top">How many rows to add at the top.</param>
        /// <param name="left">How many columns to add at the left side.</param>
        /// <param name="right">How many columns to add at the right side.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int top, int right, int bottom, int left)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new T[rows + top + bottom, cols + left + right];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i + top, j + left] = matrix[i, j];

            return r;
        }

        /// <summary>
        ///   Transforms a vector into a matrix of given dimensions.
        /// </summary>
        /// 
        public static T[,] Reshape<T>(this T[] array, int rows, int cols, MatrixOrder order = MatrixOrder.Default)
        {
            return Reshape(array, rows, cols, new T[rows, cols], order);
        }

        /// <summary>
        ///   Transforms a vector into a matrix of given dimensions.
        /// </summary>
        /// 
        public static T[,] Reshape<T>(this T[] array, int rows, int cols, T[,] result, MatrixOrder order = MatrixOrder.Default)
        {
            if (order == MatrixOrder.CRowMajor)
            {
                int k = 0;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        result[i, j] = array[k++];
            }
            else
            {
                int k = 0;
                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        result[i, j] = array[k++];
            }

            return result;
        }
    }
}
