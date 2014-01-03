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

    public static partial class Matrix
    {

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="startRow">Start row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static T[,] Submatrix<T>(this T[,] source,
            int startRow, int endRow, int startColumn, int endColumn)
        {
            return submatrix(source, null, startRow, endRow, startColumn, endColumn);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="destination">The matrix where results should be stored.</param>
        /// <param name="startRow">Start row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static void Submatrix<T>(this T[,] source, T[,] destination, 
            int startRow, int endRow, int startColumn, int endColumn)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            if (destination.GetLength(0) < endRow - startRow + 1)
                throw new DimensionMismatchException("destination", 
                    "The destination matrix must be big enough to accommodate the results.");

            if (destination.GetLength(1) < endColumn - startColumn + 1)
                throw new DimensionMismatchException("destination",
                    "The destination matrix must be big enough to accommodate the results.");

            submatrix(source, destination, startRow, endRow, startColumn, endColumn);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// 
        public static T[,] Submatrix<T>(this T[,] source, int[] rowIndexes, int[] columnIndexes)
        {
            return submatrix(source, null, rowIndexes, columnIndexes);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="destination">The matrix where results should be stored.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// 
        public static void Submatrix<T>(this T[,] source, T[,] destination, int[] rowIndexes, int[] columnIndexes)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            submatrix(source, destination, rowIndexes, columnIndexes);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// 
        public static T[,] Submatrix<T>(this T[,] source, int[] rowIndexes)
        {
            return submatrix(source, null, rowIndexes, null);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="startRow">Starting row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="columnIndexes">Array of column indices</param>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        public static T[,] Submatrix<T>(this T[,] source, int startRow, int endRow, int[] columnIndexes)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            int newRows = endRow - startRow + 1;
            int newCols = cols;

            if ((startRow > endRow) || (startRow < 0) || (startRow >= rows) ||
                (endRow < 0) || (endRow >= rows))
            {
                throw new ArgumentException("Argument out of range.");
            }

            T[,] destination;

            if (columnIndexes != null)
            {
                newCols = columnIndexes.Length;
                for (int j = 0; j < columnIndexes.Length; j++)
                    if ((columnIndexes[j] < 0) || (columnIndexes[j] >= cols))
                        throw new ArgumentException("Argument out of range.");

                destination = new T[newRows, newCols];

                for (int i = startRow; i <= endRow; i++)
                    for (int j = 0; j < columnIndexes.Length; j++)
                        destination[i - startRow, j] = source[i, columnIndexes[j]];
            }
            else
            {
                if (startRow == 0 && endRow == rows - 1)
                    return source;

                destination = new T[newRows, newCols];

                for (int i = startRow; i <= endRow; i++)
                    for (int j = 0; j < newCols; j++)
                        destination[i - startRow, j] = source[i, j];
            }

            return destination;
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        public static T[,] Submatrix<T>(this T[,] source, int[] rowIndexes, int startColumn, int endColumn)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            int newRows = rows;
            int newCols = endColumn - startColumn + 1;

            if ((startColumn > endColumn) || (startColumn < 0) || (startColumn >= cols) ||
                (endColumn < 0) || (endColumn >= cols))
            {
                throw new ArgumentException("Argument out of range.");
            }

            T[,] destination;

            if (rowIndexes != null)
            {
                newRows = rowIndexes.Length;
                for (int j = 0; j < rowIndexes.Length; j++)
                    if ((rowIndexes[j] < 0) || (rowIndexes[j] >= rows))
                        throw new ArgumentException("Argument out of range.");

                destination = new T[newRows, newCols];

                for (int i = 0; i < rowIndexes.Length; i++)
                    for (int j = startColumn; j <= endColumn; j++)
                        destination[i, j - startColumn] = source[rowIndexes[i], j];
            }
            else
            {
                if (startColumn == 0 && endColumn == cols - 1)
                    return source;

                destination = new T[newRows, newCols];

                for (int i = 0; i < newRows; i++)
                    for (int j = startColumn; j <= endColumn; j++)
                        destination[i, j - startColumn] = source[i, j];
            }

            return destination;
        }




        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="startRow">Start row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static T[][] Submatrix<T>(this T[][] source,
            int startRow, int endRow, int startColumn, int endColumn)
        {
            return submatrix(source, null, startRow, endRow, startColumn, endColumn, false);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// <param name="reuseMemory">Set to true to avoid memory allocations 
        ///   when possible. This might result on the shallow copies of some
        ///   elements. Default is false (default is to always provide a true,
        ///   deep copy of every element in the matrices, using more memory).</param>
        /// 
        public static T[][] Submatrix<T>(this T[][] source, 
            int[] rowIndexes, int[] columnIndexes, bool reuseMemory = false)
        {
            return submatrix(source, null, rowIndexes, columnIndexes, reuseMemory);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// <param name="transpose">True to return a transposed matrix; false otherwise.</param>
        /// 
        public static T[][] Submatrix<T>(this T[][] source, int[] indexes, bool transpose = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (indexes == null)
                throw new ArgumentNullException("indexes");

            int rows = source.Length;
            if (rows == 0) return new T[0][];
            int cols = source[0].Length;

            T[][] destination;

            if (transpose)
            {
                destination = new T[cols][];

                for (int j = 0; j < destination.Length; j++)
                {
                    destination[j] = new T[indexes.Length];
                    for (int i = 0; i < indexes.Length; i++)
                        destination[j][i] = source[indexes[i]][j];
                }
            }
            else
            {
                destination = new T[indexes.Length][];

                for (int i = 0; i < indexes.Length; i++)
                    destination[i] = source[indexes[i]];
            }

            return destination;
        }


        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// <param name="reuseMemory">Set to true to avoid memory allocations 
        ///   when possible. This might result on the shallow copies of some
        ///   elements. Default is false (default is to always provide a true,
        ///   deep copy of every element in the matrices, using more memory).</param>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        public static T[][] Submatrix<T>(this T[][] source, int[] rowIndexes, 
            int startColumn, int endColumn, bool reuseMemory = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int rows = source.Length;
            if (rows == 0) return new T[0][];
            int cols = source[0].Length;

            int newRows = rows;
            int newCols = endColumn - startColumn + 1;

            if ((startColumn > endColumn) || (startColumn < 0) || (startColumn >= cols) ||
                (endColumn < 0) || (endColumn >= cols))
            {
                throw new ArgumentException("Argument out of range.");
            }

            T[][] destination;

            bool canReuseMemory = startColumn == 0 && endColumn == cols - 1;

            if (rowIndexes != null)
            {
                newRows = rowIndexes.Length;
                for (int j = 0; j < rowIndexes.Length; j++)
                    if ((rowIndexes[j] < 0) || (rowIndexes[j] >= rows))
                        throw new ArgumentException("Argument out of range.");

                destination = new T[newRows][];

                if (canReuseMemory && reuseMemory)
                {
                    for (int i = 0; i < rowIndexes.Length; i++)
                        destination[i] = source[rowIndexes[i]];
                }
                else
                {
                    for (int i = 0; i < rowIndexes.Length; i++)
                    {
                        var row = destination[i] = new T[newCols];
                        for (int j = startColumn; j <= endColumn; j++)
                            row[j - startColumn] = source[rowIndexes[i]][j];
                    }
                }
            }
            else
            {
                if (startColumn == 0 && endColumn == cols - 1)
                    return source;

                destination = new T[newRows][];

                for (int i = 0; i < destination.Length; i++)
                {
                    var row = destination[i] = new T[newCols];
                    for (int j = startColumn; j <= endColumn; j++)
                        row[j - startColumn] = source[i][j];
                }
            }

            return destination;
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="startRow">Starting row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="columnIndexes">Array of column indices</param>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        public static T[][] Submatrix<T>(this T[][] source, int startRow, int endRow, int[] columnIndexes)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int rows = source.Length;
            if (rows == 0) return new T[0][];
            int cols = source[0].Length;

            int newRows = endRow - startRow + 1;
            int newCols = cols;

            if ((startRow > endRow) || (startRow < 0) || (startRow >= rows) || (endRow < 0) || (endRow >= rows))
            {
                throw new ArgumentException("Argument out of range");
            }


            T[][] destination;

            if (columnIndexes != null)
            {
                newCols = columnIndexes.Length;
                for (int j = 0; j < columnIndexes.Length; j++)
                    if ((columnIndexes[j] < 0) || (columnIndexes[j] >= cols))
                        throw new ArgumentException("Argument out of range.");

                destination = new T[newRows][];
                for (int i = 0; i < destination.Length; i++)
                    destination[i] = new T[newCols];

                for (int i = startRow; i <= endRow; i++)
                {
                    for (int j = 0; j < columnIndexes.Length; j++)
                        destination[i - startRow][j] = source[i][columnIndexes[j]];
                }
            }
            else
            {
                if (startRow == 0 && endRow == rows - 1)
                    return source;

                destination = new T[newRows][];
                for (int i = 0; i < destination.Length; i++)
                    destination[i] = new T[newCols];

                for (int i = startRow; i <= endRow; i++)
                    for (int j = 0; j < newCols; j++)
                        destination[i - startRow][j] = source[i][j];
            }

            return destination;
        }




        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <param name="source">The vector to return the subvector from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// 
        public static T[] Submatrix<T>(this T[] source, int[] indexes)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (indexes == null)
                throw new ArgumentNullException("indexes");

            var destination = new T[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
                destination[i] = source[indexes[i]];

            return destination;
        }

        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <param name="source">The vector to return the subvector from.</param>
        /// <param name="startRow">Starting index.</param>
        /// <param name="endRow">End index.</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        public static T[] Submatrix<T>(this T[] source, int startRow, int endRow)
        {
            if (startRow < 0)
                throw new ArgumentOutOfRangeException("startRow");
            if (endRow >= source.Length)
                throw new ArgumentOutOfRangeException("endRow");

            var destination = new T[endRow - startRow + 1];
            for (int i = startRow; i <= endRow; i++)
                destination[i - startRow] = source[i];

            return destination;
        }

        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        public static T[] Submatrix<T>(this T[] source, int first)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (first < 0 || first > source.Length)
                throw new ArgumentOutOfRangeException("first");

            if (first == 0)
                return new T[0];

            return Submatrix<T>(source, 0, first - 1);
        }

        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <param name="source">The vector to return the subvector from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// 
        public static List<T> Submatrix<T>(this List<T> source, int[] indexes)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (indexes == null)
                throw new ArgumentNullException("indexes");

            var destination = new List<T>();
            for (int i = 0; i < indexes.Length; i++)
                destination.Add(source[indexes[i]]);

            return destination;
        }
      



        /// <summary>
        ///   Returns subgroups extracted from the given vector.
        /// </summary>
        /// 
        /// <param name="values">The vector to extract the groups from.</param>
        /// <param name="groups">The vector of indices for the groups.</param>
        /// 
        public static T[][] Subgroups<T>(this T[] values, int[] groups)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (groups == null)
                throw new ArgumentNullException("groups");

            if (values.Length != groups.Length)
                throw new DimensionMismatchException("groups",
                    "The vector of group labels should have the same length as the values vector.");

            int[] distinct = groups.Distinct();

            T[][] result = new T[distinct.Length][];

            for (int i = 0; i < result.Length; i++)
            {
                int[] idx = groups.Find(x => x == distinct[i]);
                result[i] = values.Submatrix(idx);
            }

            return result;
        }

        /// <summary>
        ///   Returns subgroups extracted from the given vector, assuming that
        ///   the groups should have been labels from 0 until the given number
        ///   of <paramref name="classes"/>.
        /// </summary>
        /// 
        /// <param name="values">The vector to extract the groups from.</param>
        /// <param name="groups">The vector of indices for the groups.</param>
        /// <param name="classes">The number of classes in the groups. Specifying this
        ///   parameter will make the method assume the groups should be containing
        ///   integer labels ranging from 0 until the number of classes.</param>
        /// 
        public static T[][] Subgroups<T>(this T[] values, int[] groups, int classes)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (groups == null)
                throw new ArgumentNullException("groups");

            if (values.Length != groups.Length)
                throw new DimensionMismatchException("groups", 
                    "The vector of group labels should have the same length as the values vector.");

            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes", 
                    "The number of classes must be a positive number.");

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] < 0 || groups[i] >= classes)
                    throw new ArgumentException("The group labels should be between"
                        + " 0 and the total number of classes", "groups");
            }

            T[][] result = new T[classes][];

            for (int i = 0; i < result.Length; i++)
            {
                int[] idx = groups.Find(x => x == i);
                result[i] = values.Submatrix(idx);
            }

            return result;
        }





        /// <summary>
        ///   Extracts a selected area from a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        private static T[,] submatrix<T>(this T[,] source, T[,] destination,
            int startRow, int endRow, int startColumn, int endColumn)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            if ((startRow > endRow) || (startColumn > endColumn) || (startRow < 0) ||
                (startRow >= rows) || (endRow < 0) || (endRow >= rows) ||
                (startColumn < 0) || (startColumn >= cols) || (endColumn < 0) ||
                (endColumn >= cols))
            {
                throw new ArgumentException("Argument out of range.");
            }

            if (destination == null)
                destination = new T[endRow - startRow + 1, endColumn - startColumn + 1];

            for (int i = startRow; i <= endRow; i++)
                for (int j = startColumn; j <= endColumn; j++)
                    destination[i - startRow, j - startColumn] = source[i, j];

            return destination;
        }

        /// <summary>
        ///   Extracts a selected area from a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        private static T[,] submatrix<T>(this T[,] source, T[,] destination, int[] rowIndexes, int[] columnIndexes)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            int newRows = rows;
            int newCols = cols;

            if (rowIndexes == null && columnIndexes == null)
            {
                return source;
            }

            if (rowIndexes != null)
            {
                newRows = rowIndexes.Length;
                for (int i = 0; i < rowIndexes.Length; i++)
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= rows))
                        throw new ArgumentException("Argument out of range.");
            }
            if (columnIndexes != null)
            {
                newCols = columnIndexes.Length;
                for (int i = 0; i < columnIndexes.Length; i++)
                    if ((columnIndexes[i] < 0) || (columnIndexes[i] >= cols))
                        throw new ArgumentException("Argument out of range.");
            }


            if (destination != null)
            {
                if (destination.GetLength(0) < newRows || destination.GetLength(1) < newCols)
                    throw new DimensionMismatchException("destination", 
                    "The destination matrix must be big enough to accommodate the results.");
            }
            else
            {
                destination = new T[newRows, newCols];
            }

            if (columnIndexes == null)
            {
                for (int i = 0; i < rowIndexes.Length; i++)
                    for (int j = 0; j < cols; j++)
                        destination[i, j] = source[rowIndexes[i], j];
            }
            else if (rowIndexes == null)
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columnIndexes.Length; j++)
                        destination[i, j] = source[i, columnIndexes[j]];
            }
            else
            {
                for (int i = 0; i < rowIndexes.Length; i++)
                    for (int j = 0; j < columnIndexes.Length; j++)
                        destination[i, j] = source[rowIndexes[i], columnIndexes[j]];
            }

            return destination;
        }

        /// <summary>
        ///   Extracts a selected area from a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        private static T[][] submatrix<T>(this T[][] source, T[][] destination, 
            int[] rowIndexes, int[] columnIndexes, bool reuseMemory)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Length == 0)
                return new T[0][];

            int rows = source.Length;
            int cols = source[0].Length;

            int newRows = rows;
            int newCols = cols;

            if (rowIndexes == null && columnIndexes == null)
            {
                return source;
            }

            if (rowIndexes != null)
            {
                newRows = rowIndexes.Length;
                for (int i = 0; i < rowIndexes.Length; i++)
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= rows))
                        throw new ArgumentException("Argument out of range.");
            }

            if (columnIndexes != null)
            {
                newCols = columnIndexes.Length;
                for (int i = 0; i < columnIndexes.Length; i++)
                    if ((columnIndexes[i] < 0) || (columnIndexes[i] >= cols))
                        throw new ArgumentException("Argument out of range.");
            }
          

            if (destination != null)
            {
                if (destination.Length < newRows)
                    throw new DimensionMismatchException("destination",
                    "The destination matrix must be big enough to accommodate the results.");
            }
            else
            {
                destination = new T[newRows][];
                if (columnIndexes != null && !reuseMemory)
                {
                    for (int i = 0; i < destination.Length; i++)
                        destination[i] = new T[newCols];
                }
            }


            if (columnIndexes == null)
            {
                if (reuseMemory)
                {
                    for (int i = 0; i < rowIndexes.Length; i++)
                        destination[i] = source[rowIndexes[i]];
                }
                else
                {
                    for (int i = 0; i < rowIndexes.Length; i++)
                        destination[i] = (T[])source[rowIndexes[i]].Clone();
                }
            }
            else if (rowIndexes == null)
            {
                for (int i = 0; i < source.Length; i++)
                    for (int j = 0; j < columnIndexes.Length; j++)
                        destination[i][j] = source[i][columnIndexes[j]];
            }
            else
            {
                for (int i = 0; i < rowIndexes.Length; i++)
                    for (int j = 0; j < columnIndexes.Length; j++)
                        destination[i][j] = source[rowIndexes[i]][columnIndexes[j]];
            }

            return destination;
        }

        /// <summary>
        ///   Extracts a selected area from a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        /// 
        private static T[][] submatrix<T>(this T[][] source, T[][] destination,
            int startRow, int endRow, int startColumn, int endColumn, bool reuseMemory)
        {
            int rows = source.Length;
            int cols = source[0].Length;

            if ((startRow > endRow) || (startColumn > endColumn) || (startRow < 0) ||
                (startRow >= rows) || (endRow < 0) || (endRow >= rows) ||
                (startColumn < 0) || (startColumn >= cols) || (endColumn < 0) ||
                (endColumn >= cols))
            {
                throw new ArgumentException("Argument out of range.");
            }

            bool canAvoidAllocation = startColumn == 0 && endColumn == rows - 1;

            int newCols = endColumn - startColumn + 1;

            if (destination == null)
            {
                destination = new T[endRow - startRow + 1][];

                if (!canAvoidAllocation || !reuseMemory)
                {
                    for (int i = 0; i < destination.Length; i++)
                        destination[i] = new T[newCols];
                }
            }

            if (reuseMemory && canAvoidAllocation)
            {
                for (int i = startRow; i <= endRow; i++)
                    destination[i - startRow] = source[i];
            }
            else
            {
                for (int i = startRow; i <= endRow; i++)
                    for (int j = startColumn; j <= endColumn; j++)
                        destination[i - startRow][j - startColumn] = source[i][j];
            }

            return destination;
        }

    }
}
