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
    using System.Collections.Generic;
    using AForge;
    using System.Linq;
    using Accord.Math.Comparers;

    public static partial class Matrix
    {

        #region Remove
        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// 
        public static T[,] Remove<T>(this T[,] data, int[] rowIndexes, int[] columnIndexes)
        {
            if (rowIndexes == null)
                rowIndexes = new int[0];

            if (columnIndexes == null)
                columnIndexes = new int[0];


            int srcRows = data.GetLength(0);
            int srcCols = data.GetLength(1);

            int dstRows = srcRows - rowIndexes.Length;
            int dstCols = srcCols - columnIndexes.Length;


            T[,] X = new T[dstRows, dstCols];

            for (int i = 0, y = 0; i < srcRows; i++)
            {
                if (!rowIndexes.Contains(i))
                {
                    for (int j = 0, x = 0; j < srcCols; j++)
                    {
                        if (!columnIndexes.Contains(j))
                        {
                            X[y, x] = data[i, j];
                            x++;
                        }
                    }
                    y++;
                }
            }

            return X;
        }

        #endregion



        #region Row and column getters and setters

        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[] GetColumn<T>(this T[,] m, int index)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);

            if (index >= cols)
                throw new ArgumentOutOfRangeException("index");

            T[] column = new T[rows];

            for (int i = 0; i < column.Length; i++)
                column[i] = m[i, index];

            return column;
        }

        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[] GetColumn<T>(this T[][] m, int index)
        {
            T[] column = new T[m.Length];

            for (int i = 0; i < column.Length; i++)
                column[i] = m[i][index];

            return column;
        }

        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[][] GetColumns<T>(this T[][] m, params int[] index)
        {
            T[][] columns = new T[m.Length][];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new T[index.Length];
                for (int j = 0; j < index.Length; j++)
                    columns[i][j] = m[i][index[j]];
            }

            return columns;
        }

        /// <summary>
        ///   Gets a row vector from a matrix.
        /// </summary>
        /// 
        public static T[] GetRow<T>(this T[][] m, int index)
        {
            return (T[])m[index].Clone();
        }

        /// <summary>
        ///   Gets a row vector from a matrix.
        /// </summary>
        /// 
        public static T[][] GetRows<T>(this T[][] m, params int[] index)
        {
            T[][] rows = new T[index.Length][];

            for (int i = 0; i < index.Length; i++)
                rows[i] = (T[])m[index[i]].Clone();

            return rows;
        }


        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[,] GetColumns<T>(this T[,] m, params int[] index)
        {
            int rows = m.GetLength(0);

            T[,] columns = new T[rows, index.Length];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index.Length; j++)
                    columns[i, j] = m[i, index[j]];
            }

            return columns;
        }

        /// <summary>
        ///   Stores a column vector into the given column position of the matrix.
        /// </summary>
        public static T[,] SetColumn<T>(this T[,] m, int index, T[] column)
        {
            for (int i = 0; i < column.Length; i++)
                m[i, index] = column[i];

            return m;
        }

        /// <summary>
        ///   Gets a row vector from a matrix.
        /// </summary>
        public static T[] GetRow<T>(this T[,] m, int index)
        {
            T[] row = new T[m.GetLength(1)];

            for (int i = 0; i < row.Length; i++)
                row[i] = m[index, i];

            return row;
        }

        /// <summary>
        ///   Stores a row vector into the given row position of the matrix.
        /// </summary>
        public static T[,] SetRow<T>(this T[,] m, int index, T[] row)
        {
            for (int i = 0; i < row.Length; i++)
                m[index, i] = row[i];

            return m;
        }
        #endregion


        #region Row and column insertion and removal

        /// <summary>
        ///   Returns a new matrix without one of its columns.
        /// </summary>
        /// 
        public static T[][] RemoveColumn<T>(this T[][] matrix, int index)
        {
            T[][] X = new T[matrix.Length][];

            for (int i = 0; i < matrix.Length; i++)
            {
                X[i] = new T[matrix[i].Length - 1];
                for (int j = 0; j < index; j++)
                {
                    X[i][j] = matrix[i][j];
                }
                for (int j = index + 1; j < matrix[i].Length; j++)
                {
                    X[i][j - 1] = matrix[i][j];
                }
            }
            return X;
        }

        /// <summary>
        ///   Returns a new matrix without one of its columns.
        /// </summary>
        /// 
        public static T[,] RemoveColumn<T>(this T[,] matrix, int index)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int newCols = cols - 1;

            T[,] X = new T[rows, newCols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                {
                    X[i, j] = matrix[i, j];
                }
                for (int j = index + 1; j < cols; j++)
                {
                    X[i, j - 1] = matrix[i, j];
                }
            }
            return X;
        }


        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[,] InsertColumn<T>(this T[,] matrix, T[] column)
        {
            return InsertColumn(matrix, column, matrix.GetLength(1));
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T>(this T[][] matrix, T[] column)
        {
            return InsertColumn(matrix, column, matrix[0].Length);
        }

        /// <summary>
        ///   Returns a new matrix with a given row vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[,] InsertRow<T>(this T[,] matrix, T[] row)
        {
            return InsertRow(matrix, row, matrix.GetLength(0));
        }

        /// <summary>
        ///   Returns a new matrix with a given row vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[][] InsertRow<T>(this T[][] matrix, T[] row)
        {
            return InsertRow(matrix, row, matrix.Length);
        }



        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at a given index.
        /// </summary>
        /// 
        public static T[,] InsertColumn<T>(this T[,] matrix, T[] column, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (column == null)
                throw new ArgumentNullException("column");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int maxRows = System.Math.Max(rows, column.Length);
            int minRows = System.Math.Min(rows, column.Length);

            T[,] X = new T[maxRows, cols + 1];

            // Copy original matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                    X[i, j] = matrix[i, j];
                for (int j = index; j < cols; j++)
                    X[i, j + 1] = matrix[i, j];
            }

            // Copy additional column
            for (int i = 0; i < column.Length; i++)
                X[i, index] = column[i];

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at a given index.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T>(this T[][] matrix, T[] column, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (column == null)
                throw new ArgumentNullException("column");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            int maxRows = System.Math.Max(rows, column.Length);
            int minRows = System.Math.Min(rows, column.Length);

            T[][] X = new T[maxRows][];
            for (int i = 0; i < X.Length; i++)
                X[i] = new T[cols + 1];

            // Copy original matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                    X[i][j] = matrix[i][j];

                for (int j = index; j < cols; j++)
                    X[i][j + 1] = matrix[i][j];
            }

            // Copy additional column
            for (int i = 0; i < column.Length; i++)
                X[i][index] = column[i];

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given row vector inserted at a given index.
        /// </summary>
        /// 
        public static T[,] InsertRow<T>(this T[,] matrix, T[] row, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (row == null)
                throw new ArgumentNullException("row");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int maxCols = System.Math.Max(cols, row.Length);
            int minCols = System.Math.Min(cols, row.Length);

            var X = new T[rows + 1, maxCols];

            // Copy original matrix
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                    X[j, i] = matrix[j, i];

                for (int j = index; j < rows; j++)
                    X[j + 1, i] = matrix[j, i];
            }

            // Copy additional column
            for (int i = 0; i < row.Length; i++)
                X[index, i] = row[i];

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given row vector inserted at a given index.
        /// </summary>
        /// 
        public static T[][] InsertRow<T>(this T[][] matrix, T[] row, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (row == null)
                throw new ArgumentNullException("row");

            int rows = matrix.Length;
            int cols = matrix[0].Length;
            int maxCols = System.Math.Max(cols, row.Length);
            int minCols = System.Math.Min(cols, row.Length);

            var X = new T[rows + 1][];
            for (int i = 0; i < X.Length; i++)
                X[i] = new T[maxCols];

            // Copy original matrix
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                    X[j][i] = matrix[j][i];

                for (int j = index; j < rows; j++)
                    X[j + 1][i] = matrix[j][i];
            }

            // Copy additional column
            for (int i = 0; i < row.Length; i++)
                X[index][i] = row[i];

            return X;
        }

        /// <summary>
        ///   Returns a new matrix without one of its rows.
        /// </summary>
        /// 
        public static T[,] RemoveRow<T>(this T[,] matrix, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int newRows = rows - 1;

            T[,] X = new T[newRows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < index; j++)
                    X[j, i] = matrix[j, i];

                for (int j = index + 1; j < rows; j++)
                    X[j - 1, i] = matrix[j, i];
            }

            return X;
        }

        /// <summary>
        ///   Removes an element from a vector.
        /// </summary>
        /// 
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            T[] r = new T[array.Length - 1];
            for (int i = 0; i < index; i++)
                r[i] = array[i];
            for (int i = index; i < r.Length; i++)
                r[i] = array[i + 1];

            return r;
        }
        #endregion


        #region Element search

        /// <summary>
        ///   Gets the number of elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        public static int Count<T>(this T[] data, Func<T, bool> func)
        {
            int count = 0;
            for (int i = 0; i < data.Length; i++)
                if (func(data[i])) count++;
            return count;
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        public static int[] Find<T>(this T[] data, Func<T, bool> func)
        {
            return Find(data, func, false);
        }

        /// <summary>
        ///   Gets the indices of the first element matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        public static int First<T>(this T[] data, Func<T, bool> func)
        {
            return Find(data, func, true)[0];
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        /// <param name="firstOnly">
        ///    Set to true to stop when the first element is
        ///    found, set to false to get all elements.
        /// </param>
        public static int[] Find<T>(this T[] data, Func<T, bool> func, bool firstOnly)
        {
            List<int> idx = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (func(data[i]))
                {
                    if (firstOnly)
                        return new int[] { i };
                    else idx.Add(i);
                }
            }
            return idx.ToArray();
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        public static int[][] Find<T>(this T[,] data, Func<T, bool> func)
        {
            return Find(data, func, false);
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        /// <param name="firstOnly">
        ///    Set to true to stop when the first element is
        ///    found, set to false to get all elements.
        /// </param>
        public static int[][] Find<T>(this T[,] data, Func<T, bool> func, bool firstOnly)
        {
            List<int[]> idx = new List<int[]>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (func(data[i, j]))
                    {
                        if (firstOnly)
                            return new int[][] { new int[] { i, j } };
                        else idx.Add(new int[] { i, j });
                    }
                }
            }
            return idx.ToArray();
        }
        #endregion


        #region Element ranges (maximum and minimum)



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
        public static T Max<T>(this T[] values) where T : IComparable
        {
            int imax;
            return Max(values, out imax);
        }


        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static T Min<T>(this T[] values, out int imax) where T : IComparable
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
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
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static T Min<T>(this T[] values) where T : IComparable
        {
            int imin;
            return Min(values, out imin);
        }


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



        /// <summary>
        ///   Gets the range of the values in a vector.
        /// </summary>
        public static DoubleRange Range(this double[] array)
        {
            if (array.Length == 0)
                return new DoubleRange(0, 0);

            double min = array[0];
            double max = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                if (min > array[i])
                    min = array[i];
                if (max < array[i])
                    max = array[i];
            }

            return new DoubleRange(min, max);
        }

        /// <summary>
        ///   Gets the range of the values in a vector.
        /// </summary>
        /// 
        public static IntRange Range(this int[] array)
        {
            if (array.Length == 0)
                return new IntRange(0, 0);

            int min = array[0];
            int max = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                if (min > array[i])
                    min = array[i];
                if (max < array[i])
                    max = array[i];
            }

            return new IntRange(min, max);
        }

        /// <summary>
        ///   Gets the range of the values across a matrix.
        /// </summary>
        /// 
        public static IntRange Range(this int[,] value)
        {
            if (value.Length == 0)
                return new IntRange(0, 0);

            int min = value[0, 0];
            int max = value[0, 0];

            foreach (int v in value)
            {
                if (v < min) min = v;
                if (v > max) max = v;
            }

            return new IntRange(min, max);
        }

        /// <summary>
        ///   Gets the range of the values across a matrix.
        /// </summary>
        /// 
        public static DoubleRange Range(this double[,] value)
        {
            if (value.Length == 0)
                return new DoubleRange(0, 0);

            double min = value[0, 0];
            double max = value[0, 0];

            foreach (double v in value)
            {
                if (v < min) min = v;
                if (v > max) max = v;
            }

            return new DoubleRange(min, max);
        }

        /// <summary>
        ///   Gets the range of the values across the columns of a matrix.
        /// </summary>
        /// 
        public static DoubleRange[] Range(this double[,] value, int dimension)
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
        public static DoubleRange[] Range(this double[][] value, int dimension)
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

        #endregion


        /// <summary>
        ///   Performs an in-place re-ordering of elements in 
        ///   a given array using the given vector of indices.
        /// </summary>
        /// 
        /// <param name="values">The values to be ordered.</param>
        /// <param name="indices">The new index positions.</param>
        /// 
        public static void Swap<T>(this T[] values, int[] indices)
        {
            T[] newValues = values.Submatrix(indices);

            for (int i = 0; i < values.Length; i++)
                values[i] = newValues[i];
        }

        /// <summary>
        ///   Retrieves a list of the distinct values for each matrix column.
        /// </summary>
        /// 
        /// <param name="values">The matrix.</param>
        /// 
        /// <returns>An array containing arrays of distinct values for
        /// each column in the <paramref name="values"/>.</returns>
        /// 
        public static T[][] Distinct<T>(this T[,] values)
        {
            int rows = values.GetLength(0);
            int cols = values.GetLength(1);

            var sets = new HashSet<T>[cols];

            for (int i = 0; i < sets.Length; i++)
                sets[i] = new HashSet<T>();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    sets[j].Add(values[i, j]);

            T[][] result = new T[cols][];
            for (int i = 0; i < result.Length; i++)
                result[i] = sets[i].ToArray();

            return result;
        }

        /// <summary>
        ///   Retrieves a list of the distinct values for each matrix column.
        /// </summary>
        /// 
        /// <param name="values">The matrix.</param>
        /// 
        /// <returns>An array containing arrays of distinct values for
        /// each column in the <paramref name="values"/>.</returns>
        /// 
        public static T[][] Distinct<T>(this T[][] values)
        {
            int rows = values.Length;
            int cols = values[0].Length;

            var sets = new HashSet<T>[cols];

            for (int i = 0; i < sets.Length; i++)
                sets[i] = new HashSet<T>();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    sets[j].Add(values[i][j]);

            T[][] result = new T[cols][];
            for (int i = 0; i < result.Length; i++)
                result[i] = sets[i].ToArray();

            return result;
        }

        /// <summary>
        ///   Retrieves only distinct values contained in an array.
        /// </summary>
        /// 
        /// <param name="values">The array.</param>
        /// 
        /// <returns>An array containing only the distinct values in <paramref name="values"/>.</returns>
        /// 
        public static T[] Distinct<T>(this T[] values)
        {
            var set = new HashSet<T>(values);

            return set.ToArray();
        }

        /// <summary>
        ///   Retrieves only distinct values contained in an array.
        /// </summary>
        /// 
        /// <param name="values">The array.</param>
        /// <param name="allowNulls">Whether to allow null values in 
        ///   the method's output. Default is true.</param>
        /// 
        /// <returns>An array containing only the distinct values in <paramref name="values"/>.</returns>
        /// 
        public static T[] Distinct<T>(this T[] values, bool allowNulls)
            where T : class
        {
            var set = new HashSet<T>(values);

            if (!allowNulls)
                set.Remove(null);

            return set.ToArray();
        }

        /// <summary>
        ///   Retrieves only distinct values contained in an array.
        /// </summary>
        /// 
        /// <param name="values">The array.</param>
        /// <param name="property">The property of the object used to determine distinct instances.</param>
        /// 
        /// <returns>An array containing only the distinct values in <paramref name="values"/>.</returns>
        /// 
        public static T[] Distinct<T, TProperty>(this T[] values, Func<T, TProperty> property)
            where TProperty : IComparable<TProperty>
        {
            CustomComparer<T> comparer = new CustomComparer<T>(
                (a, b) => property(a).CompareTo(property(b)));

            var set = new HashSet<T>(values, comparer);

            return set.ToArray();
        }

        /// <summary>
        ///   Sorts the columns of a matrix by sorting keys.
        /// </summary>
        /// 
        /// <param name="keys">The key value for each column.</param>
        /// <param name="values">The matrix to be sorted.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// 
        public static TValue[,] Sort<TKey, TValue>(TKey[] keys, TValue[,] values, IComparer<TKey> comparer)
        {
            int[] indices = new int[keys.Length];
            for (int i = 0; i < keys.Length; i++) indices[i] = i;

            Array.Sort<TKey, int>(keys, indices, comparer);

            return values.Submatrix(0, values.GetLength(0) - 1, indices);
        }

        /// <summary>
        ///   Retrieves the top <c>count</c> values of an array.
        /// </summary>
        /// 
        public static int[] Top<T>(this T[] values, int count, bool inPlace = false)
            where T : IComparable
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count",
                "The number of elements to be selected must be positive.");
            }

            if (count == 0)
                return new int[0];

            if (count > values.Length)
                return Matrix.Indices(0, values.Length);

            T[] work = (inPlace) ? values : (T[])values.Clone();

            int[] idx = new int[values.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;

            int pivot = select(work, idx, 0, values.Length - 1, count, true);

            int[] result = new int[count];
            for (int i = 0; i < result.Length; i++)
                result[i] = idx[i];

            return result;
        }

        /// <summary>
        ///   Retrieves the bottom <c>count</c> values of an array.
        /// </summary>
        /// 
        public static int[] Bottom<T>(this T[] values, int count, bool inPlace = false)
            where T : IComparable
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count",
                "The number of elements to be selected must be positive.");
            }

            if (count == 0)
                return new int[0];

            if (count > values.Length)
                return Matrix.Indices(0, values.Length);

            T[] work = (inPlace) ? values : (T[])values.Clone();

            int[] idx = new int[values.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;

            int pivot = select(work, idx, 0, values.Length - 1, count, false);

            int[] result = new int[count];
            for (int i = 0; i < result.Length; i++)
                result[i] = idx[i];

            return result;
        }

        private static int select<T>(T[] list, int[] idx, int left, int right, int k, bool asc)
            where T : IComparable
        {
            while (left != right)
            {
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;

                int pivotNewIndex = partition(list, idx, left, right, pivotIndex, asc);
                int pivotDist = pivotNewIndex - left + 1;

                if (pivotDist == k)
                    return pivotNewIndex;

                else if (k < pivotDist)
                    right = pivotNewIndex - 1;
                else
                {
                    k = k - pivotDist;
                    left = pivotNewIndex + 1;
                }
            }

            return -1;
        }

        private static int partition<T>(T[] list, int[] idx, int left, int right, int pivotIndex, bool asc)
            where T : IComparable
        {
            T pivotValue = list[pivotIndex];

            // Move pivot to end
            swap(ref list[pivotIndex], ref list[right]);
            swap(ref idx[pivotIndex], ref idx[right]);

            int storeIndex = left;

            if (asc)
            {
                for (int i = left; i < right; i++)
                {
                    if (list[i].CompareTo(pivotValue) > 0)
                    {
                        swap(ref list[storeIndex], ref list[i]);
                        swap(ref idx[storeIndex], ref idx[i]);
                        storeIndex++;
                    }
                }
            }
            else
            {
                for (int i = left; i < right; i++)
                {
                    if (list[i].CompareTo(pivotValue) < 0)
                    {
                        swap(ref list[storeIndex], ref list[i]);
                        swap(ref idx[storeIndex], ref idx[i]);
                        storeIndex++;
                    }
                }
            }

            // Move pivot to its final place
            swap(ref list[right], ref list[storeIndex]);
            swap(ref idx[right], ref idx[storeIndex]);
            return storeIndex;
        }

        private static void swap<T>(ref T a, ref T b)
        {
            T aux = a; a = b; b = aux;
        }
    }
}
