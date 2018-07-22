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

#define CHECKS

namespace Accord.Math
{
    using System;
    using Accord.Math.Decompositions;
    using Accord.Math.Comparers;
    using System.Collections.Generic;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public static partial class Matrix
    {

        // TODO: Use T4 templates for the equality comparisons


        #region Transpose

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static T[][] Transpose<T>(this IList<T[]> matrix)
        {
            int rows = matrix.Count;
            if (rows == 0)
                return new T[rows][];
            int cols = matrix[0].Length;
#if CHECKS
            if (!IsRectangular(matrix))
                throw new ArgumentException("Only rectangular matrices can be transposed.");
#endif
            T[][] result = new T[cols][];
            for (int j = 0; j < cols; j++)
            {
                result[j] = new T[rows];
                for (int i = 0; i < rows; i++)
                    result[j][i] = matrix[i][j];
            }

            return result;
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static TItem[][] Transpose<TCollection, TItem>(this TCollection matrix)
            where TCollection : ICollection, IEnumerable<TItem[]>
        {
            int rows = matrix.Count;
            if (rows == 0)
                return new TItem[rows][];
            int cols = matrix.Columns();
#if CHECKS
            if (!IsRectangular(matrix))
                throw new ArgumentException("Only rectangular matrices can be transposed.");
#endif
            TItem[][] result = Jagged.Zeros<TItem>(cols, rows);

            int i = 0;
            foreach (TItem[] row in matrix)
            {
                for (int j = 0; j < result.Length; j++)
                    result[j][i] = row[j];
                i++;
            }

            return result;
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static T[][] Transpose<T>(this T[][] matrix)
        {
            return Transpose(matrix, false);
        }



        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <param name="inPlace">True to store the transpose over the same input
        ///   <paramref name="matrix"/>, false otherwise. Default is false.</param>
        ///   
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static T[][] Transpose<T>(this T[][] matrix, bool inPlace)
        {
            int rows = matrix.Length;
            if (rows == 0) return new T[rows][];
            int cols = matrix[0].Length;
#if CHECKS
            if (!IsRectangular(matrix))
                throw new ArgumentException("Only rectangular matrices can be transposed.");
#endif
            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.", "matrix");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        T element = matrix[j][i];
                        matrix[j][i] = matrix[i][j];
                        matrix[i][j] = element;
                    }
                }

                return matrix;
            }
            else
            {
                T[][] result = new T[cols][];
                for (int j = 0; j < cols; j++)
                {
                    result[j] = new T[rows];
                    for (int i = 0; i < rows; i++)
                        result[j][i] = matrix[i][j];
                }

                return result;
            }
        }

        /// <summary>
        ///   Gets the transpose of a row vector.
        /// </summary>
        /// 
        /// <param name="rowVector">A row vector.</param>
        /// <param name="result">The matrix where to store the transpose.</param>
        /// 
        /// <returns>The transpose of the given vector.</returns>
        /// 
        public static T[][] Transpose<T>(this T[] rowVector, out T[][] result)
        {
            result = new T[rowVector.Length][];
            for (int i = 0; i < rowVector.Length; i++)
                result[i] = new T[] { rowVector[i] };
            return result;
        }

        /// <summary>
        ///   Gets the transpose of a row vector.
        /// </summary>
        /// 
        /// <param name="rowVector">A row vector.</param>
        /// <param name="result">The matrix where to store the transpose.</param>
        /// 
        /// <returns>The transpose of the given vector.</returns>
        /// 
        public static T[][] Transpose<T>(this T[] rowVector, T[][] result)
        {
            for (int i = 0; i < rowVector.Length; i++)
                result[i] = new T[] { rowVector[i] };
            return result;
        }

        #endregion


        #region Matrix Characteristics

        /// <summary>
        ///   Gets the number of rows in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of rows must be computed.</param>
        /// 
        /// <returns>The number of rows in the matrix.</returns>
        /// 
        public static int Rows<T>(this T[][] matrix)
        {
            return matrix.Length;
        }

        /// <summary>
        ///   Gets the number of columns in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// 
        /// <returns>The number of columns in the matrix.</returns>
        /// 
        public static int Columns<T>(this T[][] matrix)
        {
            if (matrix.Length == 0)
                return 0;
            return matrix[0].Length;
        }

        /// <summary>
        ///   Gets the number of columns in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// 
        /// <returns>The number of columns in the matrix.</returns>
        /// 
        public static int Columns<T>(this IEnumerable<T[]> matrix)
        {
            foreach (var row in matrix)
                return row.Length;
            return 0;
        }

        /// <summary>
        ///   Gets the number of columns in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// <param name="max">Whether to compute the maximum length across all rows (because
        ///   rows can have different lengths in jagged matrices). Default is false.</param>
        /// 
        /// <returns>The number of columns in the matrix.</returns>
        /// 
        public static int Columns<T>(this T[][] matrix, bool max = false)
        {
            if (matrix.Length == 0)
                return 0;

            if (max)
            {
                int maxLength = matrix[0].Length;
                for (int i = 1; i < matrix.Length; i++)
                    if (matrix[i].Length > maxLength)
                        maxLength = matrix[i].Length;
                return maxLength;
            }

            return matrix[0].Length;
        }

        /// <summary>
        ///   Gets the number of channels (planes) in a jagged matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of channels must be computed.</param>
        /// <param name="max">Whether to compute the maximum length across all columns (because
        ///   columns can have different lengths in jagged matrices). Default is false.</param>
        /// 
        /// <returns>The number of channels (planes) in the matrix.</returns>
        /// 
        public static int Depth<T>(this T[][][] matrix, bool max = false)
        {
            if (matrix.Length == 0 || matrix[0].Length == 0)
                return 0;

            if (max)
            {
                int maxLength = matrix[0][0].Length;
                for (int i = 0; i < matrix.Length; i++)
                    for (int j = 0; j < matrix[i].Length; j++)
                        if (matrix[i][j].Length > maxLength)
                            maxLength = matrix[i][j].Length;
                return maxLength;
            }

            return matrix[0][0].Length;
        }

        /// <summary>
        ///   Returns true if a matrix is upper triangular.
        /// </summary>
        public static bool IsUpperTriangular<T>(this T[][] matrix) where T : IComparable
        {
            T zero = default(T);

            if (matrix.Length != matrix[0].Length)
                return false;

            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < i; j++)
                    if (matrix[i][j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is lower triangular.
        /// </summary>
        public static bool IsLowerTriangular<T>(this T[][] matrix) where T : IComparable
        {
            T zero = default(T);

            if (matrix.Length != matrix[0].Length)
                return false;

            for (int i = 0; i < matrix.Length; i++)
                for (int j = i + 1; j < matrix[i].Length; j++)
                    if (matrix[i][j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Gets the lower triangular part of a matrix.
        /// </summary>
        /// 
        public static T[][] GetLowerTriangle<T>(this T[][] matrix, bool includeDiagonal = true)
        {
            int s = includeDiagonal ? 1 : 0;
            var r = Jagged.CreateAs(matrix);
            for (int i = 0; i < matrix.Rows(); i++)
                for (int j = 0; j < i + s; j++)
                    r[i][j] = matrix[i][j]; ;
            return r;
        }

        /// <summary>
        ///   Gets the upper triangular part of a matrix.
        /// </summary>
        /// 
        public static T[][] GetUpperTriangle<T>(this T[][] matrix, bool includeDiagonal = false)
        {
            int s = includeDiagonal ? 0 : 1;
            var r = Jagged.CreateAs(matrix);
            for (int i = 0; i < matrix.Rows(); i++)
                for (int j = i + s; j < matrix.Columns(); j++)
                    r[i][j] = matrix[i][j]; ;
            return r;
        }

        /// <summary>
        ///   Transforms a triangular matrix in a symmetric matrix by copying
        ///   its elements to the other, unfilled part of the matrix.
        /// </summary>
        /// 
        public static T[][] GetSymmetric<T>(this T[][] matrix, MatrixType type, T[][] result = null)
        {
            if (result == null)
                result = Jagged.CreateAs(matrix);

            switch (type)
            {
                case MatrixType.LowerTriangular:
                    for (int i = 0; i < matrix.Rows(); i++)
                        for (int j = 0; j <= i; j++)
                            result[i][j] = result[j][i] = matrix[i][j];
                    break;
                case MatrixType.UpperTriangular:
                    for (int i = 0; i < matrix.Rows(); i++)
                        for (int j = i; j <= matrix.Columns(); j++)
                            result[i][j] = result[j][i] = matrix[i][j];
                    break;
                default:
                    throw new Exception("Matrix type can be either LowerTriangular or UpperTrianguler.");
            }

            return result;
        }

        /// <summary>
        ///   Returns true if a matrix is diagonal.
        /// </summary>
        /// 
        public static bool IsDiagonal<T>(this T[][] matrix) where T : IComparable
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.Length != matrix[0].Length)
                return false;

            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (i != j && matrix[i][j].CompareTo(zero) != 0)
                        return false;

            return true;
        }


        // TODO: Move to T4 template
        /// <summary>
        ///   Gets the trace of a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   The trace of an n-by-n square matrix A is defined to be the sum of the
        ///   elements on the main diagonal (the diagonal from the upper left to the
        ///   lower right) of A.
        /// </remarks>
        /// 
        public static float Trace(this float[][] matrix)
        {
            float trace = 0.0f;
            for (int i = 0; i < matrix.Length; i++)
                trace += matrix[i][i];
            return trace;
        }

        /// <summary>
        ///   Gets the diagonal vector from a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The diagonal vector from the given matrix.</returns>
        /// 
        public static T[] Diagonal<T>(this T[][] matrix)
        {
            T[] r = new T[matrix.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = matrix[i][i];

            return r;
        }

        /// <summary>
        ///    Gets whether a matrix is positive definite.
        /// </summary>
        /// 
        public static bool IsPositiveDefinite(this double[][] matrix)
        {
            return new JaggedCholeskyDecomposition(matrix).IsPositiveDefinite;
        }

        /// <summary>
        /// Determines whether the specified matrix is rectangular.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix.</param>
        /// 
        /// <returns><c>true</c> if the specified matrix is rectangular; otherwise, <c>false</c>.</returns>
        /// 
        public static bool IsRectangular<T>(T[][] matrix)
        {
            int numberOfCols = matrix[0].Length;

            for (int i = 1; i < matrix.Length; i++)
                if (matrix[i].Length != numberOfCols)
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified matrix is rectangular.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix.</param>
        /// 
        /// <returns><c>true</c> if the specified matrix is rectangular; otherwise, <c>false</c>.</returns>
        /// 
        public static bool IsRectangular<T>(this IEnumerable<T[]> matrix)
        {
            int numberOfCols = matrix.Columns();

            foreach (T[] row in matrix)
                if (row.Length != numberOfCols)
                    return false;

            return true;
        }

        ///// <summary>
        /////   Determines whether a jagged array is a rectangular
        /////   matrix (containing all inner arrays of same size).
        ///// </summary>
        ///// 
        //public static bool IsRectangular<T>(this T[][] array)
        //{
        //    if (array.Length == 0)
        //        return true;
        //    int cols = array[0].Length;
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        if (array[0].Length != cols)
        //            return false;
        //    }

        //    return true;
        //}
        #endregion




        #region Operation Mapping (Apply)

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[][] Apply<TInput, TResult>(TInput[][] matrix, Func<TInput, TResult> func)
        {
            return Apply(matrix, func, Jagged.CreateAs<TInput, TResult>(matrix));
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[][] Apply<TInput, TResult>(this TInput[][] matrix, Func<TInput, int, int, TResult> func)
        {
            return Apply(matrix, func, Jagged.CreateAs<TInput, TResult>(matrix));
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[][] Apply<TInput, TResult>(this TInput[][] matrix, Func<TInput, TResult> func, TResult[][] result)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i][j] = func(matrix[i][j]);
            return result;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[][] Apply<TInput, TResult>(this TInput[][] matrix, Func<TInput, int, int, TResult> func, TResult[][] result)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i][j] = func(matrix[i][j], i, j);
            return result;
        }
        #endregion


        /// <summary>
        ///   Creates a member-wise copy of a jagged matrix. Matrix elements
        ///   themselves are copied only in a shallowed manner (i.e. not cloned).
        /// </summary>
        /// 
        public static T[][] MemberwiseClone<T>(this T[][] a)
        {
            T[][] clone = new T[a.Length][];
            for (int i = 0; i < a.Length; i++)
                clone[i] = (T[])a[i].Clone();
            return clone;
        }

        /// <summary>
        ///   Creates a member-wise copy of a jagged matrix. Matrix elements
        ///   themselves are copied only in a shallowed manner (i.e. not cloned).
        /// </summary>
        /// 
        public static T[][] Copy<T>(this T[][] a)
        {
            T[][] clone = new T[a.Length][];
            for (int i = 0; i < a.Length; i++)
                clone[i] = (T[])a[i].Clone();
            return clone;
        }


        /// <summary>
        ///   Copies the content of an array to another array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// 
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// <param name="transpose">Whether to transpose the matrix when copying or not. Default is false.</param>
        /// 
        public static void CopyTo<T>(this T[][] matrix, T[][] destination, bool transpose = false)
        {
            if (matrix == destination)
            {
                if (transpose)
                    matrix.Transpose(inPlace: true);
            }
            else
            {
                if (transpose)
                {
                    int rows = Math.Min(matrix.Rows(), destination.Columns());
                    int cols = Math.Min(matrix.Columns(), destination.Rows());
                    for (int i = 0; i < rows; i++)
                        for (int j = 0; j < cols; j++)
                            destination[j][i] = matrix[i][j];
                }
                else
                {
                    int rows = Math.Min(matrix.Rows(), destination.Rows());
                    int cols = Math.Min(matrix.Columns(), destination.Columns());
                    for (int i = 0; i < rows; i++)
                        for (int j = 0; j < cols; j++)
                            destination[i][j] = matrix[i][j];
                }
            }
        }

        /// <summary>
        ///   Copies the content of an array to another array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// 
        /// <param name="vector">The source vector to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// 
        public static void CopyTo<T>(this T[] vector, T[] destination)
        {
            if (vector != destination)
                for (int i = 0; i < vector.Length; i++)
                    destination[i] = vector[i];
        }

        /// <summary>
        ///   Copies the content of an array to another array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// 
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// 
        public static void CopyTo<T>(this T[][] matrix, T[,] destination)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    destination[i, j] = matrix[i][j];
        }

    }
}

