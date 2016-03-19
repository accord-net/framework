// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System;
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
    public static class Jagged
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
#if NET45
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
        ///   Creates a zero-valued matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the matrix to be created.</typeparam>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Ones<T>(int rows, int columns)
            where T : struct
        {
            var one = (T)System.Convert.ChangeType(1, typeof(T));
            return Create<T>(rows, columns, one);
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] Zeros(int rows, int columns)
        {
            return Zeros<double>(rows, columns);
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
#if NET45
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
#if NET45
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
        /// <param name="size">The number of rows and columns in the matrix.</param>
        /// <param name="value">The initial values for the matrix.</param>
        /// 
        /// <returns>A matrix of the specified size.</returns>
        /// 
        /// <seealso cref="Matrix.Create{T}(int, int, T)"/>
        /// 
#if NET45
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(int rows, int columns, params T[] values)
        {
            if (values.Length == 0)
                return Zeros<T>(rows, columns);
            return values.Reshape(rows, columns).ToArray();
        }

        /// <summary>
        ///   Creates a matrix with the given rows.
        /// </summary>
        /// 
        /// <param name="rows">The row vectors in the matrix.</param>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(params T[][] rows)
        {
            return rows;
        }

        /// <summary>
        ///   Creates a matrix with the given values.
        /// </summary>
        /// 
        /// <param name="values">The values in the matrix.</param>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] Create<T>(T[,] values)
        {
            return values.ToArray();
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] OneHot<T>(int[] indices)
        {
            return OneHot<T>(indices, indices.DistinctCount());
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] OneHot(int[] indices)
        {
            return OneHot(indices, indices.DistinctCount());
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
            return OneHot<T>(indices, columns, Jagged.Create<T>(indices.Length, columns));
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] OneHot(int[] indices, int columns)
        {
            return OneHot(indices, columns, Jagged.Create<double>(indices.Length, columns));
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
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] OneHot<T>(int[] indices, int columns, T[][] result)
        {
            var one = (T)System.Convert.ChangeType(1, typeof(T));
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
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing one-hot vectors where only a single position
        /// is one and the others are zero.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] OneHot(int[] indices, int columns, double[][] result)
        {
            for (int i = 0; i < indices.Length; i++)
                result[i][indices[i]] = 1;
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] KHot<T>(int[][] indices, int columns)
        {
            return KHot<T>(indices, columns, Jagged.Create<T>(indices.Length, columns));
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] KHot(int[][] indices, int columns)
        {
            return KHot(indices, columns, Jagged.Create<double>(indices.Length, columns));
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
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[][] KHot<T>(int[][] indices, int columns, T[][] result)
        {
            var one = (T)System.Convert.ChangeType(1, typeof(T));
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
        /// <param name="columns">The size (length) of the vectors (columns of the matrix).</param>
        /// <param name="result">The matrix where the one-hot should be marked.</param>
        /// 
        /// <returns>A matrix containing k-hot vectors where only elements at the indicated 
        ///   <paramref name="indices"/> are set to one and the others are zero.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] KHot(int[][] indices, int columns, double[][] result)
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
#if NET45
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
#if NET45
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
        ///   Creates a jagged magic square matrix.
        /// </summary>
        /// 
        public static double[][] Magic(int size)
        {
            return Matrix.Magic(size).ToArray();
        }

        /// <summary>
        ///   Returns a square jagged diagonal matrix of the given size.
        /// </summary>
        /// 
        public static T[][] Diagonal<T>(int size, T value)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                "Square matrix's size must be a positive integer.");
            }

            var matrix = new T[size][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new T[size];
                matrix[i][i] = value;
            }

            return matrix;
        }

        /// <summary>
        ///   Returns a square jagged diagonal matrix of the given size.
        /// </summary>
        /// 
        public static T[][] Diagonal<T>(T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T[][] matrix = new T[values.Length][];
            for (int i = 0; i < values.Length; i++)
            {
                matrix[i] = new T[values.Length];
                matrix[i][i] = values[i];
            }

            return matrix;
        }

        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
#if NET45
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOutput[][] CreateAs<TInput, TOutput>(TInput[][] matrix)
        {
            var r = new TOutput[matrix.Length][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new TOutput[matrix[i].Length];
            return r;
        }


    }
}
