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

    public static partial class Matrix
    {

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The value to which matrix elements will be set.</param>
        /// 
        public static void Set<T>(this T[][] destination, T value)
        {
            Set(destination, Jagged.Create(destination.Rows(), destination.Columns(), value));
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The value to which matrix elements will be set.</param>
        /// 
        public static void Set<T>(this T[,] destination, T value)
        {
            Set(destination, Matrix.Create(destination.Rows(), destination.Columns(), value));
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// 
        public static void Set<T>(this T[][] destination, T[][] value)
        {
            set(destination, null, null, value, null, null);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// 
        public static void Set<T>(this T[,] destination, T[,] value)
        {
            set(destination, null, null, value, null, null);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static void Set<T>(this T[][] destination, T[][] value, int[] rowIndices, int startColumn, int endColumn)
        {
            int endIndex = end(endColumn, destination.Columns());
            int[] columnIndices = Accord.Math.Vector.Range(startColumn, endIndex);
            set(destination, rowIndices, columnIndices, value, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static void Set<T>(this T[,] destination, T[,] value, int[] rowIndices, int startColumn, int endColumn)
        {
            int endIndex = end(endColumn, destination.Columns());
            int[] columnIndices = Accord.Math.Vector.Range(startColumn, endIndex);
            set(destination, rowIndices, columnIndices, value, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static void Set<T>(this T[][] destination, T value, int[] rowIndices, int startColumn, int endColumn)
        {
            Set(destination, Jagged.Create(destination.Rows(), destination.Columns(), value), rowIndices, startColumn, endColumn);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="columnIndices">Array of column indices.</param>
        /// <param name="startRow">Start row index</param>
        /// <param name="endRow">End row index</param>
        /// 
        public static void Set<T>(this T[,] destination, T[,] value, int startRow, int endRow, int[] columnIndices)
        {
            int endIndex = end(endRow, destination.Rows());
            int[] rowIndices = Accord.Math.Vector.Range(startRow, endIndex);
            set(destination, rowIndices, columnIndices, value, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="columnIndices">Array of column indices.</param>
        /// <param name="startRow">Start row index</param>
        /// <param name="endRow">End row index</param>
        /// 
        public static void Set<T>(this T[][] destination, T[][] value, int startRow, int endRow, int[] columnIndices)
        {
            int endIndex = end(endRow, destination.Rows());
            int[] rowIndices = Accord.Math.Vector.Range(startRow, endIndex);
            set(destination, rowIndices, columnIndices, value, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// 
        public static void Set<T>(this T[,] destination, T value, int[] rowIndices, int startColumn, int endColumn)
        {
            T[,] source = Matrix.Create(destination.Rows(), destination.Columns(), value);
            Set(destination, source, rowIndices, startColumn, endColumn);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="startRow">Starting row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="columnIndices">Array of column indices</param>
        /// 
        public static void Set<T>(this T[][] destination, T value, int startRow, int endRow, int[] columnIndices)
        {
            T[][] source = Jagged.Create(destination.Rows(), destination.Columns(), value);
            Set(destination, source, startRow, endRow, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="startRow">Starting row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="columnIndices">Array of column indices</param>
        /// 
        public static void Set<T>(this T[,] destination, T value, int startRow, int endRow, int[] columnIndices)
        {
            T[,] source = Matrix.Create(destination.Rows(), destination.Columns(), value);
            Set(destination, source, startRow, endRow, columnIndices);
        }


        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="startRow">Starting row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="startCol">Starting column index</param>
        /// <param name="endCol">End column index</param>
        /// 
        public static void Set<T>(this T[][] destination, T value, int startRow, int endRow, int startCol, int endCol)
        {
            T[][] values = Jagged.Create<T>(destination.Rows(), destination.Columns(), value);
            int rowIndex = end(endRow, destination.Rows());
            int[] rowIndices = Accord.Math.Vector.Range(startRow, rowIndex);
            int colIndex = end(endCol, destination.Columns());
            int[] columnIndices = Accord.Math.Vector.Range(startCol, colIndex);
            set(destination, rowIndices, columnIndices, values, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="startRow">Starting row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="startCol">Starting column index</param>
        /// <param name="endCol">End column index</param>
        /// 
        public static void Set<T>(this T[,] destination, T value, int startRow, int endRow, int startCol, int endCol)
        {
            T[,] values = Matrix.Create<T>(destination.Rows(), destination.Columns(), value);
            int rowIndex = end(endRow, destination.Rows());
            int[] rowIndices = Accord.Math.Vector.Range(startRow, rowIndex);
            int colIndex = end(endCol, destination.Columns());
            int[] columnIndices = Accord.Math.Vector.Range(startCol, colIndex);
            set(destination, rowIndices, columnIndices, values, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="columnIndices">Array of column indices</param>
        /// 
        public static void Set<T>(this T[][] destination, T value, int[] rowIndices, int[] columnIndices)
        {
            var values = Jagged.Create<T>(destination.Rows(), destination.Columns(), value);
            set(destination, rowIndices, columnIndices, values, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="columnIndices">Array of column indices</param>
        /// 
        public static void Set<T>(this T[,] destination, T value, int[] rowIndices, int[] columnIndices)
        {
            var values = Matrix.Create<T>(destination.Rows(), destination.Columns(), value);
            set(destination, rowIndices, columnIndices, values, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="columnIndices">Array of column indices</param>
        /// 
        public static void Set<T>(this T[][] destination, T[][] value, int[] rowIndices, int[] columnIndices)
        {
            set(destination, rowIndices, columnIndices, value, rowIndices, columnIndices);
        }

        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="rowIndices">Array of row indices</param>
        /// <param name="columnIndices">Array of column indices</param>
        /// 
        public static void Set<T>(this T[,] destination, T[,] value, int[] rowIndices, int[] columnIndices)
        {
            set(destination, rowIndices, columnIndices, value, rowIndices, columnIndices);
        }




        /// <summary>
        ///   Sets a region of a matrix to the given values.
        /// </summary>
        /// 
        /// <param name="destination">The matrix where elements will be set.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="indices">Array of indices.</param>
        /// 
        public static void Set<T>(this T[] destination, T value, int[] indices)
        {
            if (destination == null)
                throw new ArgumentNullException("source");

            if (indices == null)
                throw new ArgumentNullException("Indices");

            for (int i = 0; i < indices.Length; i++)
                destination[Matrix.index(indices[i], destination.Length)] = value;
        }

        /// <summary>
        ///   Sets a subvector to the given value.
        /// </summary>
        /// 
        /// <param name="destination">The vector to return the subvector from.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="startRow">Starting index.</param>
        /// <param name="endRow">End index.</param>
        /// 
        public static void Set<T>(this T[] destination, T value, int startRow, int endRow)
        {
            endRow = end(endRow, destination.Length);
            for (int i = startRow; i < endRow; i++)
                destination[i] = value;
        }

        /// <summary>
        ///   Sets a subvector to the given value.
        /// </summary>
        /// 
        /// <param name="destination">The vector to return the subvector from.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="index">The index of the element to be set.</param>
        /// 
        public static void Set<T>(this T[] destination, T value, int index)
        {
            if (destination == null)
                throw new ArgumentNullException("source");

            if (index >= destination.Length)
                throw new ArgumentOutOfRangeException("index");

            index = Matrix.index(index, destination.Length);

            destination[index] = value;
        }

        /// <summary>
        ///   Sets a subvector to the given value.
        /// </summary>
        /// 
        /// <param name="destination">The vector to return the subvector from.</param>
        /// <param name="value">The matrix of values to which matrix elements will be set.</param>
        /// <param name="indices">Array of indices.</param>
        /// 
        public static void Set<T>(this List<T> destination, T value, int[] indices)
        {
            if (destination == null)
                throw new ArgumentNullException("source");

            if (indices == null)
                throw new ArgumentNullException("Indices");

            for (int i = 0; i < indices.Length; i++)
                destination[indices[i]] = value;
        }



        private static void set<T>(this
            T[,] dst, int[] dstRowIndices, int[] dstColumnIndices,
            T[,] src, int[] srcRowIndices, int[] srcColumnIndices)
        {
            if (src == null)
                throw new ArgumentNullException("source");

            if (dst == null)
                throw new ArgumentNullException("destination");

            if (srcRowIndices == null)
                srcRowIndices = Accord.Math.Vector.Range(0, src.Rows());

            if (srcColumnIndices == null)
                srcColumnIndices = Accord.Math.Vector.Range(0, src.Columns());

            if (dstRowIndices == null)
                dstRowIndices = Accord.Math.Vector.Range(0, dst.Rows());
            if (dstColumnIndices == null)
                dstColumnIndices = Accord.Math.Vector.Range(0, dst.Columns());

            for (int i = 0; i < srcRowIndices.Length; i++)
            {
                int si = Matrix.index(srcRowIndices[i], src.Rows());
                int di = Matrix.index(srcRowIndices[i], dst.Rows());

                for (int j = 0; j < srcColumnIndices.Length; j++)
                {
                    int sj = Matrix.index(srcColumnIndices[j], src.Columns());
                    int dj = Matrix.index(dstColumnIndices[j], dst.Columns());
                    dst[di, dj] = src[si, sj];
                }
            }
        }

        private static void set<T>(this
            T[][] dst, int[] dstRowIndices, int[] dstColumnIndices,
            T[][] src, int[] srcRowIndices, int[] srcColumnIndices)
        {
            if (src == null)
                throw new ArgumentNullException("source");

            if (dst == null)
                throw new ArgumentNullException("destination");

            if (srcRowIndices == null)
                srcRowIndices = Accord.Math.Vector.Range(0, src.Rows());
            if (srcColumnIndices == null)
                srcColumnIndices = Accord.Math.Vector.Range(0, src.Columns());

            if (dstRowIndices == null)
                dstRowIndices = Accord.Math.Vector.Range(0, dst.Rows());
            if (dstColumnIndices == null)
                dstColumnIndices = Accord.Math.Vector.Range(0, dst.Columns());

            for (int i = 0; i < srcRowIndices.Length; i++)
            {
                int si = Matrix.index(srcRowIndices[i], src.Rows());
                int di = Matrix.index(srcRowIndices[i], dst.Rows());

                for (int j = 0; j < srcColumnIndices.Length; j++)
                {
                    int sj = Matrix.index(srcColumnIndices[j], src.Columns());
                    int dj = Matrix.index(dstColumnIndices[j], dst.Columns());
                    dst[di][dj] = src[si][sj];
                }
            }
        }

    }
}
