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
    using System.Collections;
    using System.Collections.Generic;
#if !NETSTANDARD1_4
    using System.Data;
#endif
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;

    public static partial class Matrix
    {

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        /// 
        public static Array DeepToMatrix(this Array array)
        {
            int[] shape = array.GetLength();
            int totalLength = array.GetTotalLength();
            Type elementType = array.GetInnerMostType();

            Array flat = array.DeepFlatten();

            Array result = Array.CreateInstance(elementType, shape);
            Buffer.BlockCopy(flat, 0, result, 0, totalLength);
            return result;
        }

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[][] array, bool transpose = false)
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
        public static T[][] ToJagged<T>(this T[] array, bool asColumnVector = true)
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
        ///   Obsolete. Please use <see cref="ToJagged{T}(T[], bool)"/> instead.
        /// </summary>
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this T[] array, bool asColumnVector = true)
        {
            return ToJagged(array, asColumnVector: asColumnVector);
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[] array, bool asColumnVector = false)
        {
            if (asColumnVector)
            {
                T[,] m = new T[array.Length, 1];
                for (int i = 0; i < array.Length; i++)
                    m[i, 0] = array[i];
                return m;
            }
            else
            {
                T[,] m = new T[1, array.Length];
                for (int i = 0; i < array.Length; i++)
                    m[0, i] = array[i];
                return m;
            }
        }

        /// <summary>
        ///   Converts a multidimensional array into a jagged array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this T[,] matrix, bool transpose = false)
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

        /// <summary>
        ///   Converts a multidimensional array into a jagged array.
        /// </summary>
        /// 
        public static T[][][] ToJagged<T>(this T[,,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int depth = matrix.GetLength(2);

            var array = new T[rows][][];
            for (int i = 0; i < rows; i++)
            {
                var row = array[i] = new T[cols][];
                for (int j = 0; j < row.Length; j++)
                {
                    var plane = row[j] = new T[depth];
                    for (int k = 0; k < plane.Length; k++)
                        plane[k] = matrix[i, j, k];
                }
            }

            return array;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this T[,] matrix, bool transpose = false)
        {
            return ToJagged(matrix, transpose);
        }



        #region Type conversions
        /// <summary>
        ///   Converts the values of a vector using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="vector">The vector to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector)
        {
            return Convert(vector, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
        }

        /// <summary>
        ///   Converts the values of a matrix using the default converter.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The matrix to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput[,] Convert<TInput, TOutput>(this TInput[,] matrix)
        {
            return Convert(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
        }

        /// <summary>
        ///   Converts the values of a matrix using the default converter.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The matrix to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput[,] Convert<TInput, TOutput>(TInput[][] matrix)
        {
            return Convert<TInput, TOutput>(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
        }

        /// <summary>
        ///   Converts the values of a vector using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="vector">The vector to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector,
#if !NETSTANDARD1_4
            Converter<TInput, TOutput>
#else
            Func<TInput, TOutput>
#endif 
            converter)
        {
#if !NETSTANDARD1_4
            return Array.ConvertAll(vector, converter);
#else
            return vector.Apply(converter);
#endif
        }

        /// <summary>
        ///   Converts the values of a matrix using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The matrix to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput[,] Convert<TInput, TOutput>(this TInput[][] matrix,
#if !NETSTANDARD1_4
            Converter<TInput, TOutput>
#else
            Func<TInput, TOutput>
#endif 
            converter)
        {
            var result = Matrix.CreateAs<TInput, TOutput>(matrix);

            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i, j] = converter(matrix[i][j]);

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
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput[,] Convert<TInput, TOutput>(this TInput[,] matrix,
#if !NETSTANDARD1_4
            Converter<TInput, TOutput>
#else
            Func<TInput, TOutput>
#endif 
            converter)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var result = new TOutput[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = converter(matrix[i, j]);

            return result;
        }

        /// <summary>
        ///   Converts an object into another type, irrespective of whether
        ///   the conversion can be done at compile time or not. This can be
        ///   used to convert generic types to numeric types during runtime.
        /// </summary>
        /// 
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// 
        /// <param name="array">The vector or array to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static TOutput To<TOutput>(this Array array)
        {
            return To(array, typeof(TOutput)).To<TOutput>();
        }

        /// <summary>
        ///   Converts an object into another type, irrespective of whether
        ///   the conversion can be done at compile time or not. This can be
        ///   used to convert generic types to numeric types during runtime.
        /// </summary>
        /// 
        /// <param name="array">The vector or array to be converted.</param>
        /// <param name="outputType">The type of the output.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static object To(this Array array, Type outputType)
        {
            if (array.GetType() == outputType)
                return array;

            if (!outputType.IsArray && array.Length == 1)
            {
                foreach (var obj in array)
                    return obj.To(outputType);
            }

            Type outputElementType = outputType.GetInnerMostType();

            Array result;

            if (outputType.IsJagged())
            {
                // multidimensional or jagged -> jagged
                result = Jagged.CreateAs(array, outputType);
            }
            else
            {
                // multidimensional or jagged -> multidimensional
                result = Matrix.CreateAs(array, outputType);
            }

            Copy(array, result);

            return result;
        }

        /// <summary>
        ///   Copies elements from an array to another array even if one
        ///   is a jagged array and the other a multidimensional array.
        /// </summary>
        /// 
        /// <param name="source">The array whose elements should be copied from.</param>
        /// <param name="destination">The array where elements will be written to.</param>
        /// 
        public static void Copy(this Array source, Array destination)
        {
            Type outputElementType = destination.GetInnerMostType();

#if !NETSTANDARD1_4
            if (source.GetType() == destination.GetType() && source.IsMatrix() && destination.IsMatrix())
            {
                if (outputElementType.IsPrimitive)
                {
                    Buffer.BlockCopy(source, 0, destination, 0, source.Length * Marshal.SizeOf(outputElementType));
                }
                else
                {
                    Array.Copy(source, destination, source.Length);
                }
            }
            else
#endif
            {

                bool deep = true;

                if (destination.GetLength().Contains(-1))
                {
                    // Result is a jagged array where not all dimensions have been specified
                    IEnumerator<int[]> a = source.GetIndices(deep).GetEnumerator();

                    while (a.MoveNext())
                    {
                        object inputValue = source.GetValue(deep, a.Current);
                        object outputValue = convertValue(outputElementType, inputValue);
                        destination.SetValue(outputValue, deep, a.Current);
                    }
                }
                else
                {
                    // Both matrices have been completely specified
                    IEnumerator<int[]> a = source.GetIndices(deep).GetEnumerator();
                    IEnumerator<int[]> b = destination.GetIndices(deep).GetEnumerator();

                    while (a.MoveNext() && b.MoveNext())
                    {
                        object inputValue = source.GetValue(deep, a.Current);
                        object outputValue = convertValue(outputElementType, inputValue);
                        destination.SetValue(outputValue, deep, b.Current);
                    }
                }
            }
        }


        /// <summary>
        ///  Gets the value at the specified position in the multidimensional System.Array.
        ///  The indexes are specified as an array of 32-bit integers.
        /// </summary>
        /// 
        /// <param name="array">A jagged or multidimensional array.</param>
        /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
        /// <param name="indices">A one-dimensional array of 32-bit integers that represent the
        ///   indexes specifying the position of the System.Array element to get.</param>
        ///   
        public static object GetValue(this Array array, bool deep, int[] indices)
        {
            if (array.IsVector())
                return array.GetValue(indices);

            if (deep && array.IsJagged())
            {
                Array current = array.GetValue(indices[0]) as Array;
                if (indices.Length == 1)
                    return current;
                int[] last = indices.Get(1, 0);
                return GetValue(current, true, last);
            }
            else
            {
                return array.GetValue(indices);
            }
        }

        /// <summary>
        ///  Gets the value at the specified position in the multidimensional System.Array.
        ///  The indexes are specified as an array of 32-bit integers.
        /// </summary>
        /// 
        /// <param name="array">A jagged or multidimensional array.</param>
        /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
        /// <param name="indices">A one-dimensional array of 32-bit integers that represent the
        ///   indexes specifying the position of the System.Array element to get.</param>
        /// <param name="value">The value retrieved from the array.</param>
        ///   
        public static bool TryGetValue(this Array array, bool deep, int[] indices, out object value)
        {
            value = null;

            if (array.IsVector())
            {
                if (indices.Length > array.Rank)
                    return false;

                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] > array.GetUpperBound(i))
                        return false;
                }

                value = array.GetValue(indices);
                return true;
            }

            if (deep && array.IsJagged())
            {
                Array current = array.GetValue(indices[0]) as Array;
                if (indices.Length == 1)
                {
                    value = current;
                    return true;
                }
                int[] last = indices.Get(1, 0);
                return TryGetValue(current, true, last, out value);
            }
            else
            {
                value = array.GetValue(indices);
                return true;
            }
        }

        /// <summary>
        ///   Sets a value to the element at the specified position in the multidimensional
        ///   or jagged System.Array. The indexes are specified as an array of 32-bit integers.
        /// </summary>
        /// 
        /// <param name="array">A jagged or multidimensional array.</param>
        /// <param name="value">The new value for the specified element.</param>
        /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
        /// <param name="indices">A one-dimensional array of 32-bit integers that represent
        ///   the indexes specifying the position of the element to set.</param>
        ///   
        public static void SetValue(this Array array, object value, bool deep, int[] indices)
        {
            if (deep && array.IsJagged())
            {
                Array current = array.GetValue(indices[0]) as Array;
                int[] last = indices.Get(1, 0);
                int length = last.Length == 0 ? 1 : last.Max() + 1;
                if (current == null || current.Length < length)
                {
                    if (current == null)
                    {
                        current = Array.CreateInstance(array.GetType().GetElementType(), length);
                    }
                    else
                    {
                        var r = Array.CreateInstance(array.GetType().GetElementType(), length);
                        current.CopyTo(r, 0);
                        current = r;
                    }
                }

                SetValue(current, value, true, last);
            }
            else
            {
                array.SetValue(value, indices);
            }
        }

        private static object convertValue(Type outputElementType, object inputValue)
        {
            Array inputArray = inputValue as Array;
            if (inputArray != null)
                return To(inputArray, outputElementType);
            return inputValue.To(outputElementType);
        }
        #endregion

        /// <summary>
        ///   Creates a vector containing every index that can be used to
        ///   address a given <paramref name="array"/>, in order.
        /// </summary>
        /// 
        /// <param name="array">The array whose indices will be returned.</param>
        /// <param name="deep">Pass true to retrieve all dimensions of the array,
        ///   even if it contains nested arrays (as in jagged matrices).</param>
        /// <param name="max">Bases computations on the maximum length possible for 
        ///   each dimension (in case the jagged matrices has different lengths).</param>
        /// <param name="order">The direction to access the matrix. Pass 1 to read the 
        ///   matrix in row-major order. Pass 0 to read in column-major order. Default is 
        ///   1 (row-major, c-style order).</param>
        /// 
        /// <returns>
        ///   An enumerable object that can be used to iterate over all
        ///   positions of the given <paramref name="array">System.Array</paramref>.
        /// </returns>
        /// 
        /// <example>
        /// <code>
        ///   double[,] a = 
        ///   { 
        ///      { 5.3, 2.3 },
        ///      { 4.2, 9.2 }
        ///   };
        ///   
        ///   foreach (int[] idx in a.GetIndices())
        ///   {
        ///      // Get the current element
        ///      double e = (double)a.GetValue(idx);
        ///   }
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Accord.Math.Vector.GetIndices{T}(T[])"/>
        /// 
        public static IEnumerable<int[]> GetIndices(this Array array, bool deep = false, bool max = false, MatrixOrder order = MatrixOrder.Default)
        {
            return Combinatorics.Sequences(array.GetLength(deep, max), firstColumnChangesFaster: order == MatrixOrder.FortranColumnMajor);
        }


        #region DataTable Conversions
#if !NETSTANDARD1_4
        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table)
        {
            return ToMatrix<double>(table);
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
        public static double[,] ToMatrix(this DataTable table, IFormatProvider provider)
        {
            return ToMatrix<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table, params string[] columnNames)
        {
            return ToMatrix<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToMatrix<T>(table, out names, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table)
        {
            String[] names;
            return ToMatrix<T>(table, out names);
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
                {
                    object obj = table.Rows[i][j];
                    m[i, j] = (T)System.Convert.ChangeType(obj, typeof(T));
                }

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, out string[] columnNames, IFormatProvider provider)
        {
            T[,] m = new T[table.Rows.Count, table.Columns.Count];
            columnNames = new string[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    object obj = table.Rows[i][j];
                    m[i, j] = (T)System.Convert.ChangeType(obj, typeof(T), provider);
                }

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, params string[] columnNames)
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
        public static T[,] ToMatrix<T>(this DataTable table, IFormatProvider provider, params string[] columnNames)
        {
            T[,] m = new T[table.Rows.Count, columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][columnNames[j]], typeof(T), provider);
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
        public static DataTable ToTable(this double[,] matrix, params string[] columnNames)
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
            int cols = matrix[0].Length;

            String[] columnNames = new String[cols];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(matrix, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[][] matrix, params string[] columnNames)
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
        ///   Converts an array of values into a <see cref="DataTable"/>,
        ///   attempting to guess column types by inspecting the data.
        /// </summary>
        /// 
        /// <param name="values">The values to be converted.</param>
        /// 
        /// <returns>A <see cref="DataTable"/> containing the given values.</returns>
        /// 
        /// <example>
        /// <code>
        /// // Specify some data in a table format
        /// //
        /// object[,] data = 
        /// {
        ///     { "Id", "IsSmoker", "Age" },
        ///     {   0,       1,        10  },
        ///     {   1,       1,        15  },
        ///     {   2,       0,        40  },
        ///     {   3,       1,        20  },
        ///     {   4,       0,        70  },
        ///     {   5,       0,        55  },
        /// };
        /// 
        /// // Create a new table with the data
        /// DataTable dataTable = data.ToTable();
        /// </code>
        /// </example>
        /// 
        public static DataTable ToTable(this object[,] values)
        {
            var columnNames = new string[values.Columns()];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(values, columnNames);
        }

        /// <summary>
        ///   Converts an array of values into a <see cref="DataTable"/>,
        ///   attempting to guess column types by inspecting the data.
        /// </summary>
        /// 
        /// <param name="matrix">The values to be converted.</param>
        /// <param name="columnNames">The column names to use in the data table.</param>
        /// 
        /// <returns>A <see cref="DataTable"/> containing the given values.</returns>
        /// 
        /// <example>
        /// <code>
        /// // Specify some data in a table format
        /// //
        /// object[,] data = 
        /// {
        ///     { "Id", "IsSmoker", "Age" },
        ///     {   0,       1,        10  },
        ///     {   1,       1,        15  },
        ///     {   2,       0,        40  },
        ///     {   3,       1,        20  },
        ///     {   4,       0,        70  },
        ///     {   5,       0,        55  },
        /// };
        /// 
        /// // Create a new table with the data
        /// DataTable dataTable = data.ToTable();
        /// </code>
        /// </example>
        /// 
        public static DataTable ToTable(this object[,] matrix, string[] columnNames)
        {
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            object[] headers = matrix.GetRow(0);

            if (headers.All(x => x is String))
            {
                // Get first data row to set types
                object[] first = matrix.GetRow(1);

                // Assume first row is header row
                for (int i = 0; i < first.Length; i++)
                    table.Columns.Add(headers[i] as String, first[i].GetType());

                // Parse all the other rows
                int rows = matrix.GetLength(0);
                for (int i = 1; i < rows; i++)
                {
                    var row = matrix.GetRow(i);
                    table.Rows.Add(row);
                }
            }
            else
            {
                for (int i = 0; i < matrix.Columns(); i++)
                {
                    Type columnType = GetHighestEnclosingType(matrix.GetColumn(i));
                    table.Columns.Add(columnNames[i], columnType);
                }

                int rows = matrix.GetLength(0);
                for (int i = 0; i < rows; i++)
                {
                    var row = matrix.GetRow(i);
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        private static Type GetHighestEnclosingType(object[] values)
        {
            var types = values.Select(x => x != null ? x.GetType() : null);
            if (types.Any(x => x == typeof(object)))
                return typeof(object);
            if (types.Any(x => x == typeof(string)))
                return typeof(string);
            if (types.Any(x => x == typeof(decimal)))
                return typeof(decimal);
            if (types.Any(x => x == typeof(double)))
                return typeof(double);
            if (types.Any(x => x == typeof(float)))
                return typeof(float);
            if (types.Any(x => x == typeof(int)))
                return typeof(int);
            if (types.Any(x => x == typeof(uint)))
                return typeof(uint);
            if (types.Any(x => x == typeof(short)))
                return typeof(int);
            if (types.Any(x => x == typeof(byte)))
                return typeof(int);
            if (types.Any(x => x == typeof(sbyte)))
                return typeof(int);

            return typeof(object);
        }



        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static double[][] ToJagged(this DataTable table)
        {
            return ToJagged<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static double[][] ToJagged(this DataTable table, IFormatProvider provider)
        {
            return ToJagged<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static double[][] ToJagged(this DataTable table, out string[] columnNames)
        {
            return ToJagged<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static double[][] ToJagged(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            return ToJagged<double>(table, provider, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static double[][] ToJagged(this DataTable table, params string[] columnNames)
        {
            return ToJagged<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a T[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static T[][] ToJagged<T>(this DataTable table)
        {
            String[] names;
            return ToJagged<T>(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a T[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToJagged<T>(table, provider, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a T[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, out string[] columnNames)
        {
            T[][] m = new T[table.Rows.Count][];
            columnNames = new string[table.Columns.Count];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var value = table.Rows[i][j];
                    m[i][j] = (T)System.Convert.ChangeType(value, typeof(T));
                }

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a T[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, IFormatProvider provider, out string[] columnNames)
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
        ///   Converts a DataTable to a T[][] array.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_table_tojagged" />
        /// </example>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, params string[] columnNames)
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
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table)
        {
            return ToVector<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, IFormatProvider provider)
        {
            return ToVector<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, out string columnName)
        {
            return ToVector<double>(table, out columnName);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, IFormatProvider provider, out string columnName)
        {
            return ToVector<double>(table, provider, out columnName);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, string columnName)
        {
            return ToVector<double>(table, columnName);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table)
        {
            String name;
            return ToVector<T>(table, out name);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, IFormatProvider provider)
        {
            String name;
            return ToVector<T>(table, provider, out name);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, out string columnName)
        {
            if (table.Columns.Count > 1)
                throw new ArgumentException("The given table has more than one column. Please specify which column should be converted.");

            columnName = table.Columns[0].ColumnName;
            return table.Columns[0].ToArray<T>();
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, IFormatProvider provider, out string columnName)
        {
            if (table.Columns.Count > 1)
                throw new ArgumentException("The given table has more than one column. Please specify which column should be converted.");

            columnName = table.Columns[0].ColumnName;
            return table.Columns[0].ToArray<T>(provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, string columnName)
        {
            return table.Columns[columnName].ToArray<T>();
        }






        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table)
        {
            return ToJagged<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, IFormatProvider provider)
        {
            return ToJagged<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, out string[] columnNames)
        {
            return ToJagged<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            return ToJagged<double>(table, provider, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, params string[] columnNames)
        {
            return ToJagged<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table)
        {
            String[] names;
            return ToJagged<T>(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToJagged<T>(table, provider, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, out string[] columnNames)
        {
            return ToJagged<T>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            return ToJagged<T>(table, provider, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, params string[] columnNames)
        {
            return ToJagged<T>(table, columnNames);
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
        public static double[] ToArray(this DataColumn column, IFormatProvider provider)
        {
            return ToArray<double>(column, provider);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataRow row, IFormatProvider provider, params string[] colNames)
        {
            return ToArray<double>(row, provider, colNames);
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
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataColumn column, IFormatProvider provider)
        {
            T[] m = new T[column.Table.Rows.Count];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(column.Table.Rows[i][column], typeof(T), provider);

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
        ///   Converts a DataColumn to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataRow row, IFormatProvider provider, params string[] colNames)
        {
            T[] m = new T[colNames.Length];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(row[colNames[i]], typeof(T), provider);

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataTable table, string columnName)
        {
            T[] m = new T[table.Rows.Count];

            DataColumn col = table.Columns[columnName];
            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataTable table, IFormatProvider provider, string columnName)
        {
            T[] m = new T[table.Rows.Count];

            DataColumn col = table.Columns[columnName];
            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T), provider);

            return m;
        }
#endif
        #endregion




        #region Obsolete
#if !NETSTANDARD1_4
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
#endif
        #endregion


    }
}
