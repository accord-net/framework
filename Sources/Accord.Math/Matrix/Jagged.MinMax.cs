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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Compat;

    public static partial class Matrix
    {

        internal static int GetLength<T>(T[][] values, int dimension)
        {
            if (dimension == 1)
                return values.Length;
            return values[0].Length;
        }

        internal static int GetLength<T>(T[,] values, int dimension)
        {
            if (dimension == 1)
                return values.GetLength(0);
            return values.GetLength(1);
        }



        #region Matrix ArgMin/ArgMax

        /// <summary>
        ///   Gets the index of the maximum element in a matrix.
        /// </summary>
        /// 
        public static Tuple<int, int> ArgMax<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            Tuple<int, int> index;
            Max(matrix, out index);
            return index;
        }

        /// <summary>
        ///   Gets the index of the maximum element in a matrix across a given dimension.
        /// </summary>
        /// 
        public static int[] ArgMax<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var values = new T[s];
            var indices = new int[s];
            Max(matrix, dimension, indices, values);
            return indices;
        }

        /// <summary>
        ///   Gets the index of the maximum element in a matrix across a given dimension.
        /// </summary>
        /// 
        public static int[] ArgMax<T>(this T[][] matrix, int dimension, int[] result)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var values = new T[s];
            Max(matrix, dimension, result, values);
            return result;
        }


        /// <summary>
        ///   Gets the index of the minimum element in a matrix.
        /// </summary>
        /// 
        public static Tuple<int, int> ArgMin<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            Tuple<int, int> index;
            Min(matrix, out index);
            return index;
        }

        /// <summary>
        ///   Gets the index of the minimum element in a matrix across a given dimension.
        /// </summary>
        /// 
        public static int[] ArgMin<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var values = new T[s];
            var indices = new int[s];
            Min(matrix, dimension, indices, values);
            return indices;
        }

        /// <summary>
        ///   Gets the index of the minimum element in a matrix across a given dimension.
        /// </summary>
        /// 
        public static int[] ArgMin<T>(this T[][] matrix, int dimension, int[] result)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            T[] values = new T[s];
            Min(matrix, dimension, result, values);
            return result;
        }


        #endregion


        #region Matrix Min/Max

        /// <summary>
        ///   Gets the maximum value of a matrix.
        /// </summary>
        /// 
        public static T Max<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            T max = matrix[0][0];
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (matrix[i][j].CompareTo(max) > 0)
                        max = matrix[i][j];

            return max;
        }

        /// <summary>
        ///   Gets the minimum value of a matrix.
        /// </summary>
        /// 
        public static T Min<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            Tuple<int, int> index;
            return Min(matrix, out index);
        }




        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Min<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var result = new T[s];
            var indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Max<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var result = new T[s];
            var indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Max<T>(this T[][] matrix, int dimension, T[] result)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Min<T>(this T[][] matrix, int dimension, T[] result)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }




        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Min<T>(this T[][] matrix, int dimension, out int[] indices)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var result = new T[s];
            indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Max<T>(this T[][] matrix, int dimension, out int[] indices)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            var result = new T[s];
            indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Max<T>(this T[][] matrix, int dimension, out int[] indices, T[] result)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Min<T>(this T[][] matrix, int dimension, out int[] indices, T[] result)
            where T : IComparable<T>
        {
            int s = GetLength(matrix, dimension);
            indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }

        #endregion







        #region Core implementations
        /// <summary>
        ///   Gets the maximum value of a matrix.
        /// </summary>
        /// 
        public static T Max<T>(this T[][] matrix, out Tuple<int, int> imax)
            where T : IComparable<T>
        {
            T max = matrix[0][0];
            imax = Tuple.Create(0, 0);

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j].CompareTo(max) > 0)
                    {
                        max = matrix[i][j];
                        imax = Tuple.Create(i, j);
                    }
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum value of a matrix.
        /// </summary>
        /// 
        public static T Min<T>(this T[][] matrix, out Tuple<int, int> imin)
            where T : IComparable<T>
        {
            T min = matrix[0][0];
            imin = Tuple.Create(0, 0);

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j].CompareTo(min) < 0)
                    {
                        min = matrix[i][j];
                        imin = Tuple.Create(i, j);
                    }
                }
            }

            return min;
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Max<T>(this T[][] matrix, int dimension, int[] indices, T[] result)
            where T : IComparable<T>
        {
            if (dimension == 1) // Search down columns
            {
                matrix.GetColumn(0, result: result);
                for (int j = 0; j < matrix.Length; j++)
                {
                    for (int i = 0; i < matrix[j].Length; i++)
                    {
                        if (matrix[j][i].CompareTo(result[j]) > 0)
                        {
                            result[j] = matrix[j][i];
                            indices[j] = i;
                        }
                    }
                }
            }
            else
            {
                matrix.GetRow(0, result: result);
                for (int j = 0; j < result.Length; j++)
                {
                    for (int i = 0; i < matrix.Length; i++)
                    {
                        if (matrix[i][j].CompareTo(result[j]) > 0)
                        {
                            result[j] = matrix[i][j];
                            indices[j] = i;
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        /// 
        public static T[] Min<T>(this T[][] matrix, int dimension, int[] indices, T[] result)
            where T : IComparable<T>
        {
            if (dimension == 1) // Search down columns
            {
                matrix.GetColumn(0, result: result);
                for (int j = 0; j < matrix.Length; j++)
                {
                    for (int i = 0; i < matrix[j].Length; i++)
                    {
                        if (matrix[j][i].CompareTo(result[j]) < 0)
                        {
                            result[j] = matrix[j][i];
                            indices[j] = i;
                        }
                    }
                }
            }
            else
            {
                matrix.GetRow(0, result: result);
                for (int j = 0; j < result.Length; j++)
                {
                    for (int i = 0; i < matrix.Length; i++)
                    {
                        if (matrix[i][j].CompareTo(result[j]) < 0)
                        {
                            result[j] = matrix[i][j];
                            indices[j] = i;
                        }
                    }
                }
            }

            return result;
        }
        #endregion



        /// <summary>
        ///   Gets the maximum and minimum values in a matrix.
        /// </summary>
        /// 
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// <param name="min">The minimum value in the vector.</param>
        /// <param name="max">The maximum value in the vector.</param>
        /// 
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        /// 
        public static void GetRange<T>(this T[][] values, out T min, out T max)
            where T : IComparable<T>
        {
            if (values.Length == 0 || values[0].Length == 0)
            {
                min = max = default(T);
                return;
            }

            min = max = values[0][0];
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values[i].Length; j++)
                {
                    if (values[i][j].CompareTo(min) < 0)
                        min = values[i][j];
                    if (values[i][j].CompareTo(max) > 0)
                        max = values[i][j];
                }
            }
        }



        /// <summary>
        ///   Gets the range of the values across the columns of a matrix.
        /// </summary>
        /// 
        /// <param name="value">The matrix whose ranges should be computed.</param>
        /// <param name="dimension">
        ///   Pass 0 if the range should be computed for each of the columns. Pass 1
        ///   if the range should be computed for each row. Default is 0.
        /// </param>
        /// 
        public static DoubleRange[] GetRange(this double[][] value, int dimension)
        {
            int rows = value.Length;
            int cols = value[0].Length;
            DoubleRange[] ranges;

            if (dimension == 0)
            {
                ranges = new DoubleRange[cols];

                for (int j = 0; j < ranges.Length; j++)
                {
                    double max = value[0][j];
                    double min = value[0][j];

                    for (int i = 0; i < rows; i++)
                    {
                        if (value[i][j] > max)
                            max = value[i][j];
                        if (value[i][j] < min)
                            min = value[i][j];
                    }

                    ranges[j] = new DoubleRange(min, max);
                }
            }
            else
            {
                ranges = new DoubleRange[rows];

                for (int j = 0; j < ranges.Length; j++)
                {
                    double max = value[j][0];
                    double min = value[j][0];

                    for (int i = 0; i < cols; i++)
                    {
                        if (value[j][i] > max)
                            max = value[j][i];
                        if (value[j][i] < min)
                            min = value[j][i];
                    }

                    ranges[j] = new DoubleRange(min, max);
                }
            }

            return ranges;
        }


        /// <summary>
        ///   Deprecated.
        /// </summary>
        ///
        [Obsolete("Please use GetRange instead.")]

        public static DoubleRange[] Range(this double[][] value, int dimension)
        {
            return GetRange(value, dimension);
        }


    }
}
