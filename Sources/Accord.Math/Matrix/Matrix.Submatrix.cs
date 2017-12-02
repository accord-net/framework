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
    using Accord.Statistics;
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
        [Obsolete("Please use Get instead.")]
        public static T[,] Submatrix<T>(this T[,] source,
            int startRow, int endRow, int startColumn, int endColumn)
        {
            return Get(source, startRow, endRow + 1, startColumn, endColumn + 1);
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
        [Obsolete("Please use Get instead.")]
        public static T[,] Submatrix<T>(this T[,] source, T[,] destination,
            int startRow, int endRow, int startColumn, int endColumn)
        {
            return Get(source, destination, startRow, endRow + 1, startColumn, endColumn + 1);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// 
        [Obsolete("Please use Get instead.")]
        public static T[,] Submatrix<T>(this T[,] source, int[] rowIndexes, int[] columnIndexes)
        {
            return get(source, null, rowIndexes, columnIndexes);
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
        [Obsolete("Please use Get instead.")]
        public static void Submatrix<T>(this T[,] source, T[,] destination, int[] rowIndexes, int[] columnIndexes)
        {
            get(source, destination, rowIndexes, columnIndexes);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// 
        [Obsolete("Please use Get instead.")]
        public static T[,] Submatrix<T>(this T[,] source, int[] rowIndexes)
        {
            return get(source, null, rowIndexes, null);
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
        [Obsolete("Please use Get instead.")]
        public static T[,] Submatrix<T>(this T[,] source, int startRow, int endRow, int[] columnIndexes)
        {
            return Get(source, startRow, endRow + 1, columnIndexes);
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
        [Obsolete("Please use Get instead.")]
        public static T[,] Submatrix<T>(this T[,] source, int[] rowIndexes, int startColumn, int endColumn)
        {
            return Get(source, rowIndexes, startColumn, endColumn + 1);
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
        [Obsolete("Please use Get instead.")]
        public static T[][] Submatrix<T>(this T[][] source,
            int startRow, int endRow, int startColumn, int endColumn)
        {
            return get(source, null, startRow, endRow + 1, startColumn, endColumn + 1, false);
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
            return get(source, null, rowIndexes, columnIndexes, reuseMemory);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// <param name="transpose">True to return a transposed matrix; false otherwise.</param>
        /// 
        [Obsolete("Please use Get instead.")]
        public static T[][] Submatrix<T>(this T[][] source, int[] indexes, bool transpose = false)
        {
            return Get(source, indexes, transpose);
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
        [Obsolete("Please use Get instead.")]
        public static T[][] Submatrix<T>(this T[][] source, int[] rowIndexes,
            int startColumn, int endColumn, bool reuseMemory = false)
        {
            return Get(source, rowIndexes, startColumn, endColumn + 1, reuseMemory);
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
        [Obsolete("Please use Get instead.")]
        public static T[][] Submatrix<T>(this T[][] source, int startRow, int endRow, int[] columnIndexes)
        {
            return Get(source, startRow, endRow + 1, columnIndexes);
        }




        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <param name="source">The vector to return the subvector from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// <param name="inPlace">True to return the results in place, changing the
        ///   original <paramref name="source"/> vector; false otherwise.</param>
        /// 
        [Obsolete("Please use Get instead.")]
        public static T[] Submatrix<T>(this T[] source, int[] indexes, bool inPlace = false)
        {
            return Get(source, indexes, inPlace);
        }

        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <param name="source">The vector to return the subvector from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// 
        [Obsolete("Please use Get instead.")]
        public static T[] Submatrix<T>(this T[] source, IList<int> indexes)
        {
            return Get(source, indexes);
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
        [Obsolete("Please use Get instead.")]
        public static T[] Submatrix<T>(this T[] source, int startRow, int endRow)
        {
            return Get(source, startRow, endRow + 1);
        }

        /// <summary>
        ///   Returns a value extracted from the current vector.
        /// </summary>
        /// 
        [Obsolete("Please use Get instead.")]
        public static T[] Submatrix<T>(this T[] source, int first)
        {
            return Get(source, 0, first);
        }

        /// <summary>
        ///   Returns a subvector extracted from the current vector.
        /// </summary>
        /// 
        /// <param name="source">The vector to return the subvector from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// 
        [Obsolete("Please use Get instead.")]
        public static List<T> Submatrix<T>(this List<T> source, int[] indexes)
        {
            return Get(source, indexes);
        }



        /// <summary>
        ///   Obsolete. Please use <see cref="Classes.Separate{T}(T[], int[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Classes.Separate(T[], int[]) instead.")]
        public static T[][] Subgroups<T>(this T[] values, int[] groups)
        {
            int[] distinct = groups.Distinct().Sorted();
            int[] regroups = groups.Apply(x => Array.IndexOf(distinct, x));

            return Classes.Separate(values, regroups);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Classes.Separate{T}(T[], int[], int)"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Classes.Separate(T[], int[], int) instead.")]
        public static T[][] Subgroups<T>(this T[] values, int[] groups, int classes)
        {
            return Classes.Separate(values, groups, classes);
        }

    }
}
