﻿// Accord Math Library
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
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Matrix
    {

        #region Vector ArgMin/ArgMax
        
        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
        public static int ArgMax<T>(this T[] values) where T : IComparable
        {
            int imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }

            return imax;
        }

        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
        public static int ArgMax<T>(this T[] values, out T max) where T : IComparable
        {
            int imax = 0;
            max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }

            return imax;
        }

            /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static int ArgMin<T>(this T[] values) where T : IComparable
        {
            int imin = 0;
            T min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }
            }

            return imin;
        }

        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static int ArgMin<T>(this T[] values, out T min) where T : IComparable
        {
            int imin = 0;
            min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }
            }

            return imin;
        }
        #endregion


        #region Vector Min/Max
        /// <summary>
        ///   Gets the maximum non-null element in a vector.
        /// </summary>
        /// 
        public static Nullable<T> Max<T>(this Nullable<T>[] values, out int imax)
            where T : struct, IComparable
        {
            imax = -1;
            Nullable<T> max = null;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].HasValue)
                {
                    if (max == null || values[i].Value.CompareTo(max.Value) > 0)
                    {
                        max = values[i];
                        imax = i;
                    }
                }
            }

            return max;
        }



        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
        public static T Max<T>(this T[] values, out int imax) where T : IComparable
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
        public static T Max<T>(this T[] values, out int imax, bool alreadySorted) where T : IComparable
        {
            if (alreadySorted)
            {
                imax = values.Length - 1;
                return values[values.Length - 1];
            }

            return Max(values, out imax);
        }

        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
        public static T Max<T>(this T[] values) where T : IComparable
        {
            int imax;
            return Max(values, out imax);
        }

    

        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static T Min<T>(this T[] values, out int imin) where T : IComparable
        {
            imin = 0;
            T min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }
            }
            return min;
        }

        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static T Min<T>(this T[] values) where T : IComparable
        {
            int imin;
            return Min(values, out imin);
        }
        #endregion



        #region limited length
        /// <summary>
        ///   Gets the maximum element in a vector up to a fixed length.
        /// </summary>
        /// 
        public static T Max<T>(this T[] values, int length, out int imax) where T : IComparable
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        /// <summary>
        ///   Gets the maximum element in a vector up to a fixed length.
        /// </summary>
        /// 
        public static T Max<T>(this T[] values, int length) where T : IComparable
        {
            int imax;
            return Max(values, length, out imax);
        }


        /// <summary>
        ///   Gets the minimum element in a vector up to a fixed length.
        /// </summary>
        /// 
        public static T Min<T>(this T[] values, int length, out int imax) where T : IComparable
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < length; i++)
            {
                if (values[i].CompareTo(max) < 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        /// <summary>
        ///   Gets the minimum element in a vector up to a fixed length.
        /// </summary>
        /// 
        public static T Min<T>(this T[] values, int length) where T : IComparable
        {
            int imin;
            return Min(values, length, out imin);
        }
        #endregion





        #region GetRange

        /// <summary>
        ///   Gets the maximum and minimum values in a vector.
        /// </summary>
        /// 
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// <param name="min">The minimum value in the vector.</param>
        /// <param name="max">The maximum value in the vector.</param>
        /// 
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        /// 
        public static void GetRange<T>(this T[] values, out T min, out T max) where T : IComparable
        {
            if (values.Length == 0)
            {
                min = max = default(T);
                return;
            }

            min = max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                    min = values[i];
                if (values[i].CompareTo(max) > 0)
                    max = values[i];
            }
        }

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
        public static void GetRange<T>(this T[,] values, out T min, out T max) where T : IComparable
        {
            if (values.Length == 0)
            {
                min = max = default(T);
                return;
            }

            min = max = values[0, 0];
            foreach (var v in values)
            {
                if (v.CompareTo(min) < 0)
                    min = v;
                if (v.CompareTo(max) > 0)
                    max = v;
            }
        }

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
        public static void GetRange<T>(this T[][] values, out T min, out T max) where T : IComparable
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
        ///   Gets the maximum and minimum values in a vector.
        /// </summary>
        /// 
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// 
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        /// 
        public static IntRange GetRange(this int[] values)
        {
            int min, max;
            GetRange(values, out min, out max);
            return new IntRange(min, max);
        }
        /// <summary>
        ///   Gets the maximum and minimum values in a vector.
        /// </summary>
        /// 
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// 
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        /// 
        public static DoubleRange GetRange(this double[] values)
        {
            double min, max;
            GetRange(values, out min, out max);
            return new DoubleRange(min, max);
        }

        /// <summary>
        ///   Gets the maximum and minimum values in a vector.
        /// </summary>
        /// 
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// 
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        /// 
        public static IntRange GetRange(this int[,] values)
        {
            int min, max;
            GetRange(values, out min, out max);
            return new IntRange(min, max);
        }

        /// <summary>
        ///   Gets the maximum and minimum values in a vector.
        /// </summary>
        /// 
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// 
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        /// 
        public static DoubleRange GetRange(this double[,] values)
        {
            double min, max;
            GetRange(values, out min, out max);
            return new DoubleRange(min, max);
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
        public static DoubleRange[] GetRange(this double[,] value, int dimension)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);
            DoubleRange[] ranges;

            if (dimension == 0)
            {
                ranges = new DoubleRange[cols];

                for (int j = 0; j < ranges.Length; j++)
                {
                    double max = value[0, j];
                    double min = value[0, j];

                    for (int i = 0; i < rows; i++)
                    {
                        if (value[i, j] > max)
                            max = value[i, j];
                        if (value[i, j] < min)
                            min = value[i, j];
                    }

                    ranges[j] = new DoubleRange(min, max);
                }
            }
            else
            {
                ranges = new DoubleRange[rows];

                for (int j = 0; j < ranges.Length; j++)
                {
                    double max = value[j, 0];
                    double min = value[j, 0];

                    for (int i = 0; i < cols; i++)
                    {
                        if (value[j, i] > max)
                            max = value[j, i];
                        if (value[j, i] < min)
                            min = value[j, i];
                    }

                    ranges[j] = new DoubleRange(min, max);
                }
            }

            return ranges;
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
        ///   Deprecated
        /// </summary>
        /// 
        [Obsolete("Please use GetRange instead.")]
        public static IntRange Range(this int[] values)
        {
            return GetRange(values);
        }

        /// <summary>
        ///   Deprecated
        /// </summary>
        /// 
        [Obsolete("Please use GetRange instead.")]
        public static DoubleRange Range(this double[] values)
        {
            return GetRange(values);
        }

        /// <summary>
        ///   Deprecated
        /// </summary>
        /// 
        [Obsolete("Please use GetRange instead.")]
        public static IntRange Range(this int[,] values)
        {
            return GetRange(values);
        }

        /// <summary>
        ///   Deprecated
        /// </summary>
        /// 
        [Obsolete("Please use GetRange instead.")]
        public static DoubleRange Range(this double[,] values)
        {
            return GetRange(values);
        }

        /// <summary>
        ///   Deprecated.
        /// </summary>
        /// 
        [Obsolete("Please use GetRange instead.")]
        public static DoubleRange[] Range(this double[,] value, int dimension)
        {
            return GetRange(value, dimension);
        }

        /// <summary>
        ///   Deprecated.
        /// </summary>
        ///
        public static DoubleRange[] Range(this double[][] value, int dimension)
        {
            return GetRange(value, dimension);
        }
        #endregion




        /// <summary>
        ///   Gets the maximum value of a matrix.
        /// </summary>
        /// 
        public static T Max<T>(this T[,] matrix) where T : IComparable
        {
            Tuple<int, int> imax;
            return Max(matrix, out imax);
        }

        /// <summary>
        ///   Gets the maximum value of a matrix.
        /// </summary>
        /// 
        public static T Max<T>(this T[,] matrix, out Tuple<int, int> imax) where T : IComparable
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T max = matrix[0, 0];
            imax = Tuple.Create(0, 0);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j].CompareTo(max) > 0)
                    {
                        max = matrix[i, j];
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
        public static T Min<T>(this T[,] matrix) where T : IComparable
        {
            Tuple<int, int> imin;
            return Min(matrix, out imin);
        }

        /// <summary>
        ///   Gets the minimum value of a matrix.
        /// </summary>
        /// 
        public static T Min<T>(this T[,] matrix, out Tuple<int, int> imin) where T : IComparable
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T min = matrix[0, 0];
            imin = Tuple.Create(0, 0);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j].CompareTo(min) < 0)
                    {
                        min = matrix[i, j];
                        imin = Tuple.Create(i, j);
                    }
                }
            }

            return min;
        }


        /// <summary>
        ///   Gets the maximum value of a matrix.
        /// </summary>
        /// 
        public static T Max<T>(this T[][] matrix) where T : IComparable
        {
            Tuple<int, int> imax;
            return Max(matrix, out imax);
        }

        /// <summary>
        ///   Gets the maximum value of a matrix.
        /// </summary>
        /// 
        public static T Max<T>(this T[][] matrix, out Tuple<int, int> imax) where T : IComparable
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
        public static T Min<T>(this T[][] matrix) where T : IComparable
        {
            Tuple<int, int> imin;
            return Min(matrix, out imin);
        }

        /// <summary>
        ///   Gets the minimum value of a matrix.
        /// </summary>
        /// 
        public static T Min<T>(this T[][] matrix, out Tuple<int, int> imin) where T : IComparable
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
        public static T[] Max<T>(this T[,] matrix, int dimension) where T : IComparable
        {
            int[] imax;
            return Max(matrix, dimension, out imax);
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[,] matrix, int dimension, out int[] imax) where T : IComparable
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[] max;

            if (dimension == 1) // Search down columns
            {
                imax = new int[rows];
                max = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j, i].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[j, i];
                            imax[j] = i;
                        }
                    }
                }
            }
            else
            {
                imax = new int[cols];
                max = matrix.GetRow(0);

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i, j].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[i, j];
                            imax[j] = i;
                        }
                    }
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[,] matrix, int dimension) where T : IComparable
        {
            int[] imin;
            return Min(matrix, dimension, out imin);
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[,] matrix, int dimension, out int[] imin) where T : IComparable
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[] min;

            if (dimension == 1) // Search down columns
            {
                imin = new int[rows];
                min = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j, i].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[j, i];
                            imin[j] = i;
                        }
                    }
                }
            }
            else
            {
                imin = new int[cols];
                min = matrix.GetRow(0);

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i, j].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[i, j];
                            imin[j] = i;
                        }
                    }
                }
            }

            return min;
        }



        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension) where T : IComparable
        {
            int[] imax;
            return Max(matrix, dimension, out imax);
        }

        /// <summary>
        ///   Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension, out int[] imax) where T : IComparable
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            T[] max;

            if (dimension == 1) // Search down columns
            {
                imax = new int[rows];
                max = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j][i].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[j][i];
                            imax[j] = i;
                        }
                    }
                }
            }
            else
            {
                imax = new int[cols];
                max = (T[])matrix[0].Clone();

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i][j].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[i][j];
                            imax[j] = i;
                        }
                    }
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension) where T : IComparable
        {
            int[] imin;
            return Min(matrix, dimension, out imin);
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension, out int[] imin) where T : IComparable
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            T[] min;

            if (dimension == 1) // Search down columns
            {
                imin = new int[rows];
                min = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j][i].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[j][i];
                            imin[j] = i;
                        }
                    }
                }
            }
            else
            {
                imin = new int[cols];
                min = (T[])matrix[0].Clone();

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i][j].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[i][j];
                            imin[j] = i;
                        }
                    }
                }
            }

            return min;
        }



    }
}
