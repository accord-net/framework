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
    using Accord.Math.Random;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static partial class Jagged
    {
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
        public static TOutput[][] Convert<TInput, TOutput>(TInput[,] matrix)
        {
            return Jagged.Convert(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
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
        public static TOutput[][] Convert<TInput, TOutput>(this TInput[][] matrix)
        {
            return Jagged.Convert(matrix, x => (TOutput)System.Convert.ChangeType(x, typeof(TOutput)));
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
        public static TOutput[][] Convert<TInput, TOutput>(this TInput[,] matrix,
#if !NETSTANDARD1_4
            Converter<TInput, TOutput> converter
#else
            Func<TInput, TOutput> converter
#endif
            )
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();

            var result = Jagged.Zeros<TOutput>(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i][j] = converter(matrix[i, j]);

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
        public static TOutput[][] Convert<TInput, TOutput>(this TInput[][] matrix,
#if !NETSTANDARD1_4
            Converter<TInput, TOutput> converter
#else
            Func<TInput, TOutput> converter
#endif
            )
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();

            var result = Jagged.Zeros<TOutput>(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i][j] = converter(matrix[i][j]);

            return result;
        }

        /// <summary>
        ///   Converts the values of a tensor.
        /// </summary>
        /// 
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="array">The tensor to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static Array Convert<TOutput>(this Array array)
        {
            return Convert(array, typeof(TOutput));
        }

        /// <summary>
        ///   Converts the values of a tensor.
        /// </summary>
        /// 
        /// <param name="type">The type of the output.</param>
        /// <param name="array">The tensor to be converted.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_matrix" />
        /// <code source="Unit Tests\Accord.Tests.Math\Matrix\Matrix.Conversion.cs" region="doc_convert_jagged" />
        /// </example>
        /// 
        public static Array Convert(this Array array, Type type)
        {
            Array r = Jagged.Zeros(type, array.GetLength(deep: true));

            foreach (int[] idx in r.GetIndices(deep: true))
            {
                var value = ExtensionMethods.To(array.GetValue(deep: true, indices: idx), type);
                r.SetValue(value: value, deep: true, indices: idx);
            }
            return r;
        }
    }
}
