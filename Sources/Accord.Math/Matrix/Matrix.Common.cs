// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Accord.Math.Decompositions;
    using Accord.Math.Comparers;
    using System.Collections.Generic;

    public static partial class Matrix
    {

        #region Comparison

        /// <summary>
        ///   Compares two values for equality, considering a relative acceptance threshold.
        /// </summary>
        /// 
        public static bool IsRelativelyEqual(this double a, double b, double threshold)
        {
            return Math.Abs(a - b) <= Math.Abs(a) * threshold;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        /// 
        public static bool IsEqual(this double[,] objA, double[,] objB, double threshold)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.GetLength(0) != objB.GetLength(0) ||
                objA.GetLength(1) != objB.GetLength(1))
                return false;

            for (int i = 0; i < objA.GetLength(0); i++)
            {
                for (int j = 0; j < objB.GetLength(1); j++)
                {
                    double x = objA[i, j], y = objB[i, j];

                    if (Math.Abs(x - y) > threshold || (Double.IsNaN(x) ^ Double.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        /// 
        public static bool IsEqual(this float[,] objA, float[,] objB, double threshold)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.GetLength(0); i++)
            {
                for (int j = 0; j < objB.GetLength(1); j++)
                {
                    float x = objA[i, j], y = objB[i, j];

                    if (Math.Abs(x - y) > threshold || (Single.IsNaN(x) ^ Single.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this double[][] objA, double[][] objB, double threshold)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                for (int j = 0; j < objA[i].Length; j++)
                {
                    double x = objA[i][j], y = objB[i][j];

                    if (Math.Abs(x - y) > threshold || (Double.IsNaN(x) ^ Double.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this float[][] objA, float[][] objB, double threshold)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                for (int j = 0; j < objA[i].Length; j++)
                {
                    float x = objA[i][j], y = objB[i][j];

                    if (Math.Abs(x - y) > threshold || (Single.IsNaN(x) ^ Single.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two vectors for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this double[] objA, double[] objB, double threshold)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                double x = objA[i], y = objB[i];

                if (Math.Abs(x - y) > threshold || (Double.IsNaN(x) ^ Double.IsNaN(y)))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   Compares two vectors for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this float[] objA, float[] objB, double threshold)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                float x = objA[i], y = objB[i];

                if (Math.Abs(x - y) > threshold || (Single.IsNaN(x) ^ Single.IsNaN(y)))
                    return false;
            }
            return true;
        }


        /// <summary>
        ///   Compares each member of a vector for equality with a scalar value x.
        /// </summary>
        public static bool IsEqual(this double[] vector, double scalar)
        {
            if (vector == null) throw new ArgumentNullException("vector");
            if (vector.Length == 0) return false;

            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i] != scalar)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   Compares each member of a matrix for equality with a scalar value x.
        /// </summary>
        public static bool IsEqual(this double[,] matrix, double scalar)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            if (matrix.Length == 0) return false;

            unsafe
            {
                fixed (double* ptr = matrix)
                {
                    double* p = ptr;
                    for (int i = 0; i < matrix.Length; i++, p++)
                        if (*p != scalar) return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality.
        /// </summary>
        public static bool IsEqual<T>(this T[][] objA, T[][] objB)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.Length != objB.Length)
                return false;

            for (int i = 0; i < objA.Length; i++)
            {
                if (objA[i] == objB[i])
                    continue;

                if (objA[i] == null || objB[i] == null)
                    return false;

                if (objA[i].Length != objB[i].Length)
                    return false;

                for (int j = 0; j < objA[i].Length; j++)
                {
                    T elemA = objA[i][j];
                    T elemB = objB[i][j];
                    if (!elemA.Equals(elemB))
                        return false;
                }
            }
            return true;
        }

        /// <summary>Compares two matrices for equality.</summary>
        public static bool IsEqual<T>(this T[,] objA, T[,] objB)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.GetLength(0) != objB.GetLength(0) ||
                objA.GetLength(1) != objB.GetLength(1))
                return false;

            int rows = objA.GetLength(0);
            int cols = objA.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!objA[i, j].Equals(objB[i, j]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>Compares two vectors for equality.</summary>
        public static bool IsEqual<T>(this T[] objA, params T[] objB)
        {
            if (objA == objB) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.Length != objB.Length)
                return false;

            for (int i = 0; i < objA.Length; i++)
            {
                if (!objA[i].Equals(objB[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   This method should not be called. Use Matrix.IsEqual instead.
        /// </summary>
        /// 
        public static new bool Equals(object value)
        {
            throw new NotSupportedException("Use Matrix.IsEqual instead.");
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN(this double[,] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsNaN(e)) return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN(this double[] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsNaN(e)) return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN(this double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (Double.IsNaN(matrix[i][j]))
                        return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a infinity value.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains infinity values, false otherwise.</returns>
        /// 
        public static bool HasInfinity(this double[,] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsInfinity(e)) return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a infinity value.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a infinity value, false otherwise.</returns>
        /// 
        public static bool HasInfinity(this double[] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsInfinity(e)) return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a infinity value.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a infinity value, false otherwise.</returns>
        /// 
        public static bool HasInfinity(this double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (Double.IsInfinity(matrix[i][j]))
                        return true;
            return false;
        }

        #endregion


        #region Transpose

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[,] Transpose<T>(this T[,] matrix)
        {
            return Transpose(matrix, false);
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[][] Transpose<T>(this T[][] matrix)
        {
            return Transpose(matrix, false);
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="inPlace">True to store the transpose over the same input
        ///   <paramref name="matrix"/>, false otherwise. Default is false.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[,] Transpose<T>(this T[,] matrix, bool inPlace)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.", "matrix");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        T element = matrix[j, i];
                        matrix[j, i] = matrix[i, j];
                        matrix[i, j] = element;
                    }
                }

                return matrix;
            }
            else
            {
                T[,] result = new T[cols, rows];

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        result[j, i] = matrix[i, j];

                return result;
            }
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="inPlace">True to store the transpose over the same input
        ///   <paramref name="matrix"/>, false otherwise. Default is false.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[][] Transpose<T>(this T[][] matrix, bool inPlace)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            if (rows == 0) return new T[rows][];
            int cols = matrix[0].Length;

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
        /// <param name="rowVector">A row vector.</param>
        /// <returns>The transpose of the given vector.</returns>
        public static T[,] Transpose<T>(this T[] rowVector)
        {
            if (rowVector == null) throw new ArgumentNullException("rowVector");

            T[,] trans = new T[rowVector.Length, 1];
            for (int i = 0; i < rowVector.Length; i++)
                trans[i, 0] = rowVector[i];

            return trans;
        }
        #endregion


        #region Matrix Characteristics

        /// <summary>
        ///   Returns true if a vector of real-valued observations
        ///   is ordered in ascending or descending order.
        /// </summary>
        /// 
        /// <param name="values">An array of values.</param>
        /// <param name="direction">The sort order direction.</param>
        /// 
        public static bool IsSorted<T>(this T[] values, ComparerDirection direction) where T : IComparable<T>
        {
            if (direction == ComparerDirection.Descending)
            {
                for (int i = 1; i < values.Length; i++)
                    if (values[i - 1].CompareTo(values[i]) >= 0)
                        return false;
            }
            else
            {
                for (int i = 1; i < values.Length; i++)
                    if (values[i - 1].CompareTo(values[i]) <= 0)
                        return false;
            }

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is square.
        /// </summary>
        public static bool IsSquare<T>(this T[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return matrix.GetLength(0) == matrix.GetLength(1);
        }

        /// <summary>
        ///   Returns true if a matrix is symmetric.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsSymmetric<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (matrix[i, j].CompareTo(matrix[j, i]) != 0)
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Returns true if a matrix is upper triangular.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsUpperTriangular<T>(this T[][] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

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
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsLowerTriangular<T>(this T[][] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

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
        ///   Returns true if a matrix is upper triangular.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsUpperTriangular<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < i; j++)
                    if (matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is lower triangular.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsLowerTriangular<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = i + 1; j < matrix.GetLength(1); j++)
                    if (matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is lower triangular.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsDiagonal<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (i != j && matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is lower triangular.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsDiagonal<T>(this T[][] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.Length != matrix[0].Length)
                return false;

            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (i != j && matrix[i][j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is symmetric.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsSymmetric<T>(this T[][] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            if (matrix.Length == matrix[0].Length)
            {
                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (matrix[i][j].CompareTo(matrix[j][i]) != 0)
                            return false;
                    }
                }
                return true;
            }
            return false;
        }



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
        public static double Trace(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);

            double trace = 0.0;
            for (int i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///   Gets the trace of a matrix product.
        /// </summary>
        /// 
        public static unsafe double Trace(double[,] matrixA, double[,] matrixB)
        {
            if (matrixA.Length != matrixB.Length)
                throw new DimensionMismatchException("matrixB", "Matrices must have the same length.");

            int length = matrixA.Length;

            fixed (double* ptrA = matrixA)
            fixed (double* ptrB = matrixB)
            {
                double* a = ptrA;
                double* b = ptrB;

                double trace = 0.0;
                for (int i = 0; i < length; i++)
                    trace += (*a++) * (*b++);
                return trace;
            }
        }

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
        public static int Trace(this int[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);

            int trace = 0;
            for (int i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

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
        public static float Trace(this float[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);

            float trace = 0.0f;
            for (int i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

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
            if (matrix == null)
                throw new ArgumentNullException("matrix");

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
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            T[] r = new T[matrix.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = matrix[i][i];

            return r;
        }

        /// <summary>
        ///   Gets the diagonal vector from a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The diagonal vector from the given matrix.</returns>
        /// 
        public static T[] Diagonal<T>(this T[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            var r = new T[matrix.GetLength(0)];
            for (int i = 0; i < r.Length; i++)
                r[i] = matrix[i, i];

            return r;
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static double Determinant(this double[,] matrix)
        {
            // Assume the most general case
            return Determinant(matrix, false);
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static double Determinant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust Cholesky decomposition
            {
                var chol = new CholeskyDecomposition(matrix, robust: true, lowerTriangular: true);

                if (!chol.PositiveDefinite)
                {
                    throw new ArgumentException("The matrix could not be decomposed using " +
                        " a robust Cholesky decomposition. Please specify symmetric as false " +
                        " and provide a full matrix to be decomposed.", "matrix");
                }

                return chol.Determinant;
            }

            return new LuDecomposition(matrix).Determinant;
        }

        /// <summary>
        ///   Gets the log-determinant of a matrix.
        /// </summary>
        /// 
        public static double LogDeterminant(this double[,] matrix)
        {
            // Assume the most general case
            return LogDeterminant(matrix, false);
        }

        /// <summary>
        ///   Gets the log-determinant of a matrix.
        /// </summary>
        /// 
        public static double LogDeterminant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust Cholesky decomposition
            {
                var chol = new CholeskyDecomposition(matrix, robust: true, lowerTriangular: true);

                if (!chol.PositiveDefinite)
                {
                    throw new ArgumentException("The matrix could not be decomposed using " +
                        " a robust Cholesky decomposition. Please specify symmetric as false " +
                        " and provide a full matrix to be decomposed.", "matrix");
                }

                return chol.LogDeterminant;
            }

            return new LuDecomposition(matrix).LogDeterminant;
        }

        /// <summary>
        ///   Gets the pseudo-determinant of a matrix.
        /// </summary>
        /// 
        public static double PseudoDeterminant(this double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).PseudoDeterminant;
        }

        /// <summary>
        ///   Gets the log of the pseudo-determinant of a matrix.
        /// </summary>
        /// 
        public static double LogPseudoDeterminant(this double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).LogPseudoDeterminant;
        }



        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static int Rank(this double[,] matrix)
        {
            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).Rank;
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static int Rank(this float[,] matrix)
        {
            return new SingularValueDecompositionF(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).Rank;
        }

        /// <summary>
        ///    Gets whether a matrix is singular.
        /// </summary>
        /// 
        public static bool IsSingular(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            return new SingularValueDecomposition(matrix).IsSingular;
        }

        /// <summary>
        ///    Gets whether a matrix is positive definite.
        /// </summary>
        /// 
        public static bool IsPositiveDefinite(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return new CholeskyDecomposition(matrix).PositiveDefinite;
        }

        /// <summary>
        ///    Gets whether a matrix is positive definite.
        /// </summary>
        /// 
        public static bool IsPositiveDefinite(this double[][] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return new JaggedCholeskyDecomposition(matrix).PositiveDefinite;
        }
        #endregion


        #region Summation
        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static float[] Sum(this float[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[] sum;

            if (dimension == -1)
            {
                sum = new double[1];
                foreach (double a in matrix)
                    sum[0] += a;
            }
            else if (dimension == 0)
            {
                sum = new double[cols];

                for (int j = 0; j < cols; j++)
                {
                    double s = 0.0;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new double[rows];

                for (int j = 0; j < rows; j++)
                {
                    double s = 0.0;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static float[] Sum(this float[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            float[] sum;

            if (dimension == -1)
            {
                sum = new float[1];
                foreach (float a in matrix)
                    sum[0] += a;
            }
            else if (dimension == 0)
            {
                sum = new float[cols];

                for (int j = 0; j < cols; j++)
                {
                    float s = 0.0f;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new float[rows];

                for (int j = 0; j < rows; j++)
                {
                    float s = 0.0f;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[][] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[][] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            double[] sum;

            if (dimension == 0)
            {
                sum = new double[cols];

                for (int i = 0; i < matrix.Length; i++)
                {
                    double[] row = matrix[i];
                    for (int j = 0; j < row.Length; j++)
                        sum[j] += row[j];
                }
            }
            else if (dimension == 1)
            {
                sum = new double[rows];

                for (int j = 0; j < matrix.Length; j++)
                {
                    double[] row = matrix[j];
                    double s = 0.0;
                    for (int i = 0; i < row.Length; i++)
                        s += row[i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static int[] Sum(this int[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated. Default is 0.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static int[] Sum(this int[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int[] sum;

            if (dimension == 0)
            {
                sum = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    int s = 0;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new int[rows];
                for (int j = 0; j < rows; j++)
                {
                    int s = 0;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        /// 
        public static double Sum(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double sum = 0.0;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        /// 
        public static float Sum(this float[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            float sum = 0.0f;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        public static int Sum(this int[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            int sum = 0;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>Calculates a vector cumulative sum.</summary>
        public static double[] CumulativeSum(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            if (vector.Length == 0)
                return new double[0];

            double[] sum = new double[vector.Length];

            sum[0] = vector[0];
            for (int i = 1; i < vector.Length; i++)
                sum[i] += sum[i - 1] + vector[i];
            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[][] CumulativeSum(this double[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            double[][] sum;

            if (dimension == 1)
            {
                sum = new double[matrix.GetLength(0)][];
                sum[0] = matrix.GetRow(0);

                // for each row
                for (int i = 1; i < matrix.GetLength(0); i++)
                {
                    sum[i] = new double[matrix.GetLength(1)];

                    // for each column
                    for (int j = 0; j < matrix.GetLength(1); j++)
                        sum[i][j] += sum[i - 1][j] + matrix[i, j];
                }
            }
            else if (dimension == 0)
            {
                sum = new double[matrix.GetLength(1)][];
                sum[0] = matrix.GetColumn(0);

                // for each column
                for (int i = 1; i < matrix.GetLength(1); i++)
                {
                    sum[i] = new double[matrix.GetLength(0)];

                    // for each row
                    for (int j = 0; j < matrix.GetLength(0); j++)
                        sum[i][j] += sum[i - 1][j] + matrix[j, i];
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }
        #endregion

        #region Product
        /// <summary>
        ///   Gets the product of all elements in a vector.
        /// </summary>
        public static double Product(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double product = 1.0;
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }

        /// <summary>
        ///   Gets the product of all elements in a vector.
        /// </summary>
        public static int Product(this int[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            int product = 1;
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }
        #endregion


        #region Operation Mapping (Apply)

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static void ApplyInPlace<T>(this T[] vector, Func<T, T> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            for (int i = 0; i < vector.Length; i++)
                vector[i] = func(vector[i]);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static void ApplyInPlace<T>(this T[] vector, Func<T, int, T> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            for (int i = 0; i < vector.Length; i++)
                vector[i] = func(vector[i], i);
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static void ApplyInPlace<T>(this T[,] matrix, Func<T, T> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = func(matrix[i, j]);
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static void ApplyInPlace<T>(this T[,] matrix, Func<T, int, int, T> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = func(matrix[i, j], i, j);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TData, TResult>(this TData[] vector, Func<TData, TResult> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            TResult[] result = new TResult[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                result[i] = func(vector[i]);

            return result;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static TResult[] ApplyWithIndex<TData, TResult>(this TData[] vector, Func<TData, int, TResult> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            TResult[] result = new TResult[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                result[i] = func(vector[i], i);

            return result;
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static TResult[,] Apply<TData, TResult>(this TData[,] matrix, Func<TData, TResult> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            TResult[,] r = new TResult[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = func(matrix[i, j]);

            return r;
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static TResult[,] ApplyWithIndex<TData, TResult>(this TData[,] matrix, Func<TData, int, int, TResult> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            TResult[,] result = new TResult[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = func(matrix[i, j], i, j);

            return result;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TData, TResult>(this IList<TData> vector, Func<TData, TResult> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            TResult[] result = new TResult[vector.Count];

            for (int i = 0; i < vector.Count; i++)
                result[i] = func(vector[i]);

            return result;
        }
        #endregion


        #region Rounding and discretization
        /// <summary>
        ///   Rounds a double-precision floating-point matrix to a specified number of fractional digits.
        /// </summary>
        /// 
        public static double[,] Round(this double[,] matrix, int decimals = 0)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = System.Math.Round(matrix[i, j], decimals);

            return result;
        }

        /// <summary>
        ///   Returns the largest integer less than or equal than to the specified 
        ///   double-precision floating-point number for each element of the matrix.
        /// </summary>
        /// 
        public static double[,] Floor(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = System.Math.Floor(matrix[i, j]);

            return result;
        }

        /// <summary>
        ///   Returns the largest integer greater than or equal than to the specified 
        ///   double-precision floating-point number for each element of the matrix.
        /// </summary>
        public static double[,] Ceiling(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = System.Math.Ceiling(matrix[i, j]);

            return result;
        }

        /// <summary>
        ///   Rounds a double-precision floating-point number array to a specified number of fractional digits.
        /// </summary>
        public static double[] Round(double[] vector, int decimals = 0)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double[] result = new double[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Round(vector[i], decimals);
            return result;
        }

        /// <summary>
        ///   Returns the largest integer less than or equal than to the specified 
        ///   double-precision floating-point number for each element of the array.
        /// </summary>
        public static double[] Floor(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double[] result = new double[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Floor(vector[i]);
            return result;
        }

        /// <summary>
        ///   Returns the largest integer greater than or equal than to the specified 
        ///   double-precision floating-point number for each element of the array.
        /// </summary>
        public static double[] Ceiling(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double[] result = new double[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Ceiling(vector[i]);
            return result;
        }

        #endregion


        #region Morphological operations
        /// <summary>
        ///   Transforms a vector into a matrix of given dimensions.
        /// </summary>
        public static T[,] Reshape<T>(this T[] array, int rows, int cols)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (rows < 0)
                throw new ArgumentOutOfRangeException("rows",
                    rows, "Number of rows must be a positive integer.");

            if (cols < 0)
                throw new ArgumentOutOfRangeException("cols",
                cols, "Number of columns must be a positive integer.");

            if (array.Length != rows * cols)
                throw new ArgumentOutOfRangeException("array",
                    "The length of the array should equal the product of "
                    + "the parameter \"rows\" times parameter \"cols\".");


            T[,] result = new T[rows, cols];

            for (int j = 0, k = 0; j < cols; j++)
                for (int i = 0; i < rows; i++)
                    result[i, j] = array[k++];

            return result;
        }

        /// <summary>
        ///   Transforms a matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        public static T[] Reshape<T>(this T[,] matrix)
        {
            return Reshape(matrix, 0);
        }

        /// <summary>
        ///   Transforms a matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="dimension">The direction to perform copying. Pass
        /// 0 to perform a row-wise copy. Pass 1 to perform a column-wise
        /// copy. Default is 0.</param>
        /// 
        public static T[] Reshape<T>(this T[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            if (dimension < 0) throw new ArgumentOutOfRangeException("dimension", dimension,
                "Vector's dimension must be a positive integer.");

            if (matrix.Rank > 2)
                throw new RankException("The method only works with matrices of rank 2.");

            if (dimension != 0 && dimension != 1)
                throw new ArgumentOutOfRangeException("dimension");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[] result = new T[rows * cols];

            if (dimension == 1)
            {
                for (int j = 0, k = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        result[k++] = matrix[j, i];
            }
            else
            {
                for (int i = 0, k = 0; i < cols; i++)
                    for (int j = 0; j < rows; j++)
                        result[k++] = matrix[j, i];
            }

            return result;
        }

        /// <summary>
        ///   Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// <param name="array">A jagged array.</param>
        /// 
        public static T[] Reshape<T>(this T[][] array)
        {
            return Reshape(array, 0);
        }

        /// <summary>
        ///   Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="array">A jagged array.</param>
        /// <param name="dimension">The direction to perform copying. Pass
        /// 0 to perform a row-wise copy. Pass 1 to perform a column-wise
        /// copy. Default is 0.</param>
        /// 
        public static T[] Reshape<T>(this T[][] array, int dimension)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (dimension < 0)
                throw new ArgumentOutOfRangeException("dimension", dimension,
                "Vector's dimension must be a positive integer.");

            if (dimension != 0 && dimension != 1)
                throw new ArgumentOutOfRangeException("dimension");

            int count = 0;
            for (int i = 0; i < array.Length; i++)
                count += array[i].Length;

            T[] result = new T[count];

            if (dimension == 1)
            {
                for (int j = 0, k = 0; j < array.Length; j++)
                    for (int i = 0; i < array[j].Length; i++)
                        result[k++] = array[j][i];
            }
            else
            {
                int maxCols = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Length > maxCols)
                        maxCols = array[i].Length;
                }

                for (int i = 0, k = 0; i < maxCols; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (i < array[j].Length)
                            result[k++] = array[j][i];
                    }
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        ///   Convolves an array with the given kernel.
        /// </summary>
        /// 
        /// <param name="a">A floating number array.</param>
        /// <param name="kernel">A convolution kernel.</param>
        /// 
        public static double[] Convolve(this double[] a, double[] kernel)
        {
            return Convolve(a, kernel, false);
        }

        /// <summary>
        /// Convolves an array with the given kernel.
        /// </summary>
        /// 
        /// <param name="a">A floating number array.</param>
        /// <param name="kernel">A convolution kernel.</param>
        /// <param name="trim">
        ///   If <c>true</c> the resulting array will be trimmed to
        ///   have the same length as the input array. Default is false.</param>
        ///   
        public static double[] Convolve(this double[] a, double[] kernel, bool trim)
        {
            double[] result;
            int m = (int)System.Math.Ceiling(kernel.Length / 2.0);

            if (trim)
            {
                result = new double[a.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (int j = 0; j < kernel.Length; j++)
                    {
                        int k = i - j + m - 1;
                        if (k >= 0 && k < a.Length)
                            result[i] += a[k] * kernel[j];
                    }
                }
            }
            else
            {
                result = new double[a.Length + m];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (int j = 0; j < kernel.Length; j++)
                    {
                        int k = i - j;
                        if (k >= 0 && k < a.Length)
                            result[i] += a[k] * kernel[j];
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///   Creates a memberwise copy of a jagged matrix.
        /// </summary>
        /// 
        public static T[][] MemberwiseClone<T>(this T[][] a)
        {
            T[][] clone = new T[a.Length][];
            for (int i = 0; i < a.Length; i++)
                clone[i] = (T[])a[i].Clone();
            return clone;
        }

    }
}

