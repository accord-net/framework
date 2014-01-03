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
    using System.Data;
    using System.Globalization;

    public static partial class Matrix
    {

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[][] array)
        {
            return ToMatrix(array, false);
        }

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[][] array, bool transpose)
        {
            int rows = array.Length;
            if (rows == 0) return new T[0, rows];
            int cols = array[0].Length;

            T[,] m;

            if (transpose)
            {
                m = new T[cols, rows];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        m[j, i] = array[i][j];
            }
            else
            {
                m = new T[rows, cols];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        m[i, j] = array[i][j];
            }

            return m;
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[] array)
        {
            T[,] m = new T[1, array.Length];
            for (int i = 0; i < array.Length; i++)
                m[0, i] = array[i];

            return m;
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        /// 
        public static T[][] ToArray<T>(this T[] array, bool asColumnVector = true)
        {
            if (asColumnVector)
            {
                T[][] m = new T[array.Length][];
                for (int i = 0; i < array.Length; i++)
                    m[i] = new[] { array[i] };
                return m;
            }
            else
            {
                return new T[][] { array };
            }
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[] array, bool asColumnVector = false)
        {
            if (asColumnVector)
            {
                T[,] m = new T[1, array.Length];
                for (int i = 0; i < array.Length; i++)
                    m[0, i] = array[i];
                return m;
            }
            else
            {
                T[,] m = new T[array.Length, 1];
                for (int i = 0; i < array.Length; i++)
                    m[i, 0] = array[i];
                return m;
            }
        }

        /// <summary>
        ///   Converts a multidimensional array into a jagged array.
        /// </summary>
        /// 
        public static T[][] ToArray<T>(this T[,] matrix)
        {
            return ToArray(matrix, false);
        }

        /// <summary>
        ///   Converts a multidimensional array into a jagged array.
        /// </summary>
        /// 
        public static T[][] ToArray<T>(this T[,] matrix, bool transpose)
        {
            T[][] array;

            if (transpose)
            {
                int cols = matrix.GetLength(1);

                array = new T[cols][];
                for (int i = 0; i < cols; i++)
                    array[i] = matrix.GetColumn(i);
            }
            else
            {
                int rows = matrix.GetLength(0);

                array = new T[rows][];
                for (int i = 0; i < rows; i++)
                    array[i] = matrix.GetRow(i);
            }

            return array;
        }



        #region Type conversions

        /// <summary>
        ///   Converts a double-precision floating point multidimensional
        ///   array into a single-precision floating point multidimensional
        ///   array.
        /// </summary>
        /// 
        public unsafe static double[,] ToDouble(this float[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;

            double[,] result = new double[rows, cols];

            fixed (float* srcPtr = matrix)
            fixed (double* dstPtr = result)
            {
                float* src = srcPtr;
                double* dst = dstPtr;

                for (int i = 0; i < length; i++, src++, dst++)
                    *dst = (double)*src;
            }

            return result;
        }

        /// <summary>
        ///   Converts a double-precision floating point multidimensional
        ///   array into a single-precision floating point multidimensional
        ///   array.
        /// </summary>
        /// 
        public unsafe static double[,] ToDouble(this int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;

            double[,] result = new double[rows, cols];

            fixed (int* srcPtr = matrix)
            fixed (double* dstPtr = result)
            {
                int* src = srcPtr;
                double* dst = dstPtr;

                for (int i = 0; i < length; i++, src++, dst++)
                    *dst = (double)*src;
            }

            return result;
        }

        /// <summary>
        ///   Converts a single-precision floating point multidimensional
        ///   array into a double-precision floating point multidimensional
        ///   array.
        /// </summary>
        /// 
        public unsafe static float[,] ToSingle(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;

            float[,] result = new float[rows, cols];

            fixed (double* srcPtr = matrix)
            fixed (float* dstPtr = result)
            {
                double* src = srcPtr;
                float* dst = dstPtr;

                for (int i = 0; i < length; i++, src++, dst++)
                    *dst = (float)*src;
            }

            return result;
        }

        /// <summary>
        ///   Truncates a double matrix to integer values.
        /// </summary>
        /// <param name="matrix">The matrix to be truncated.</param>
        /// 
        public unsafe static int[,] ToInt32(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;

            int[,] result = new int[rows, cols];

            fixed (double* srcPtr = matrix)
            fixed (int* dstPtr = result)
            {
                double* src = srcPtr;
                int* dst = dstPtr;

                for (int i = 0; i < length; i++, src++, dst++)
                    *dst = (int)*src;
            }

            return result;
        }

        /// <summary>
        ///   Truncates a double matrix to integer values.
        /// </summary>
        /// <param name="matrix">The matrix to be truncated.</param>
        /// 
        public static int[][] ToInt32(this double[][] matrix)
        {
            int[][] result = new int[matrix.Length][];

            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = new int[matrix[i].Length];
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i][j] = (int)matrix[i][j];
            }

            return result;
        }

        /// <summary>
        ///   Truncates an integer matrix to double values.
        /// </summary>
        /// <param name="matrix">The matrix to be converted.</param>
        /// 
        public static double[][] ToDouble(this int[][] matrix)
        {
            double[][] result = new double[matrix.Length][];

            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = new double[matrix[i].Length];
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i][j] = (double)matrix[i][j];
            }

            return result;
        }

        /// <summary>
        ///   Converts a double-precision floating point multidimensional
        ///   array into a single-precision floating point multidimensional
        ///   array.
        /// </summary>
        /// 
        public static double[] ToDouble(this float[] vector)
        {
            double[] result = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                result[i] = (double)vector[i];
            return result;
        }

        /// <summary>
        ///   Converts a double-precision floating point multidimensional
        ///   array into a single-precision floating point multidimensional
        ///   array.
        /// </summary>
        /// 
        public static double[] ToDouble(this short[] vector)
        {
            double[] result = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                result[i] = (double)vector[i];
            return result;
        }

        /// <summary>
        ///   Truncates a double vector to integer values.
        /// </summary>
        /// <param name="vector">The vector to be truncated.</param>
        /// 
        public static int[] ToInt32(this double[] vector)
        {
            int[] result = new int[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                result[i] = (int)vector[i];
            return result;
        }

        /// <summary>
        ///   Converts a integer vector into a double vector.
        /// </summary>
        /// <param name="vector">The vector to be converted.</param>
        /// 
        public static double[] ToDouble(this int[] vector)
        {
            double[] result = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                result[i] = (double)vector[i];
            return result;
        }

        /// <summary>
        ///   Converts a double vector into a single vector.
        /// </summary>
        /// <param name="vector">The vector to be converted.</param>
        /// 
        public static float[] ToSingle(this double[] vector)
        {
            float[] result = new float[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                result[i] = (float)vector[i];
            return result;
        }

        /// <summary>
        ///   Converts the values of a vector using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="vector">The vector to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector, Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(vector, converter);
        }

        /// <summary>
        ///   Converts the values of a matrix using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The matrix to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        public static TOutput[][] Convert<TInput, TOutput>(this TInput[][] matrix, Converter<TInput, TOutput> converter)
        {
            TOutput[][] result = new TOutput[matrix.Length][];

            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = new TOutput[matrix[i].Length];
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i][j] = converter(matrix[i][j]);
            }

            return result;
        }

        /// <summary>
        ///   Converts the values of a matrix using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The vector to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        public static TOutput[,] Convert<TInput, TOutput>(this TInput[,] matrix, Converter<TInput, TOutput> converter)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            TOutput[,] result = new TOutput[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = converter(matrix[i, j]);

            return result;
        }
        #endregion





        #region DataTable Conversions

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table)
        {
            String[] names;
            return ToMatrix(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table, out string[] columnNames)
        {
            return ToMatrix<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, out string[] columnNames)
        {
            T[,] m = new T[table.Rows.Count, table.Columns.Count];
            columnNames = new string[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][j], typeof(T));

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table, string[] columnNames)
        {
            return ToMatrix<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, string[] columnNames)
        {
            T[,] m = new T[table.Rows.Count, columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][columnNames[j]], typeof(T));
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[,] matrix)
        {
            int cols = matrix.GetLength(1);

            String[] columnNames = new String[cols];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(matrix, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[,] matrix, string[] columnNames)
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.CurrentCulture;

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < cols; i++)
                table.Columns.Add(columnNames[i], typeof(double));

            for (int i = 0; i < rows; i++)
                table.Rows.Add(matrix.GetRow(i).Convert(x => (object)x));

            return table;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[][] matrix)
        {
            int cols = matrix.GetLength(1);

            String[] columnNames = new String[cols];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(matrix, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[][] matrix, string[] columnNames)
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.CurrentCulture;

            for (int i = 0; i < columnNames.Length; i++)
                table.Columns.Add(columnNames[i], typeof(double));

            for (int i = 0; i < matrix.Length; i++)
                table.Rows.Add(matrix[i].Convert(x => (object)x));

            return table;
        }



        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToArray(this DataTable table)
        {
            String[] names;
            return ToArray(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToArray(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToArray(table, out names, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToArray(this DataTable table, out string[] columnNames)
        {
            return ToArray<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToArray(this DataTable table, out string[] columnNames, IFormatProvider provider)
        {
            return ToArray<double>(table, out columnNames, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToArray<T>(this DataTable table, out string[] columnNames)
        {
            T[][] m = new T[table.Rows.Count][];
            columnNames = new string[table.Columns.Count];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][j], typeof(T));

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToArray<T>(this DataTable table, out string[] columnNames, IFormatProvider provider)
        {
            T[][] m = new T[table.Rows.Count][];
            columnNames = new string[table.Columns.Count];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][j], typeof(T), provider);

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToArray(this DataTable table, params string[] columnNames)
        {
            return ToArray<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToArray<T>(this DataTable table, params string[] columnNames)
        {
            T[][] m = new T[table.Rows.Count][];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                DataColumn col = table.Columns[columnNames[j]];

                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataColumn column)
        {
            return ToArray<double>(column);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataColumn column)
        {
            T[] m = new T[column.Table.Rows.Count];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(column.Table.Rows[i][column], typeof(T));

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataRow row, params string[] colNames)
        {
            T[] m = new T[colNames.Length];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(row[colNames[i]], typeof(T));

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataRow row, params string[] colNames)
        {
            return ToArray<double>(row, colNames);
        }

        /// <summary>
        ///   Converts a DataTable to a int[][] array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataTable table, string columnNames)
        {
            T[] m = new T[table.Rows.Count];

            DataColumn col = table.Columns[columnNames];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));

            return m;
        }
        #endregion




        #region Obsolete
        /// <summary>
        ///   Converts a DataColumn to a int[] array.
        /// </summary>
        /// 
        [Obsolete("Use ToArray<T> instead.")]
        public static int[] ToInt32Array(this DataColumn column)
        {
            return ToArray<int>(column);
        }

        /// <summary>
        ///   Converts a DataTable to a int[][] array.
        /// </summary>
        /// 
        [Obsolete("Use ToArray<T> instead.")]
        public static int[][] ToIntArray(this DataTable table, params string[] columnNames)
        {
            return ToArray<int>(table, columnNames);
        }
        #endregion


    }
}
