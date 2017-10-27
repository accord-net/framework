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
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   Vector types.
    /// </summary>
    /// 
    public enum VectorType : int
    {
        /// <summary>
        ///   The vector is a row vector, meaning it should have a size equivalent 
        ///   to [1 x N] where N is the number of elements in the vector.
        /// </summary>
        /// 
        RowVector = 0,

        /// <summary>
        ///   The vector is a column vector, meaning it should have a size equivalent
        ///   to [N x 1] where N is the number of elements in the vector.
        /// </summary>
        /// 
        ColumnVector = 1
    }

    /// <summary>
    ///   Elementwise matrix and vector operations.
    /// </summary>
    /// 
    /// <seealso cref="VectorType"/>
    ///
    [GeneratedCode("Accord.NET T4 Templates", "3.7")]
    public static partial class Elementwise
    {
        private static int rows<U>(U[] b)
        {
            return b.Length;
        }

        private static int cols<U>(U[][] b)
        {
            return b[0].Length;
        }

        private static int rows<U>(U[,] b)
        {
            return b.GetLength(0);
        }

        private static int cols<U>(U[,] b)
        {
            return b.GetLength(1);
        }


        [Conditional("DEBUG")]
        static void check<T, U>(T[] a, U[] b)
        {
            if (a.Length != b.Length)
                throw new DimensionMismatchException("b");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U[] b, V[] result)
        {
            if (a.Length != b.Length)
                throw new DimensionMismatchException("b");
            if (a.Length != result.Length)
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U b, V[] result)
        {
            if (a.Length != result.Length)
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T a, U[] b, V[] result)
        {
            if (b.Length != result.Length)
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U>(T[,] a, U[,] b)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
        }

        [Conditional("DEBUG")]
        static void check<T, U>(T[][] a, U[][] b)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[,] a, U[,] b, V[,] result)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[][] a, U[][] b, V[][] result)
        {
            if (rows(a) != rows(b) || cols(a) != cols(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }




        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[,] a, U[] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[][] a, U[] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }



        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[] a, U[,] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T[] a, U[][] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }








        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[,] a, U[] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[][] a, U[] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(a) != rows(b))
                    throw new DimensionMismatchException("b");
            }
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }



        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[] a, U[,] b, V[,] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(VectorType d, T[] a, U[][] b, V[][] result)
        {
            if (d == 0)
            {
                if (cols(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            else
            {
                if (rows(b) != rows(a))
                    throw new DimensionMismatchException("b");
            }
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }








        [Conditional("DEBUG")]
        static void check<T, U, V>(T[,] a, U[] b, V[,] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[][] a, U[] b, V[][] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U[,] b, V[,] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[] a, U[][] b, V[][] result)
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
            if (rows(a) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }





        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T a, U[,] b, V[,] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(int d, T a, U[][] b, V[][] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T a, U[,] b, V[,] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T a, U[][] b, V[][] result)
        {
            if (rows(b) != rows(result) || cols(b) != cols(result))
                throw new DimensionMismatchException("result");
        }








        [Conditional("DEBUG")]
        static void check<T, U, V>(T[][] a, U b, V[][] result)
        {
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V>(T[,] a, U b, V[,] result)
        {
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V, W>(T[][] a, U b, V[,] c, W[,] result)
        {
            if (rows(a) != rows(c) || cols(a) != cols(c))
                throw new DimensionMismatchException("c");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V, W>(T[][] a, U b, V[][] c, W[][] result)
        {
            if (rows(a) != rows(c) || cols(a) != cols(c))
                throw new DimensionMismatchException("c");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

        [Conditional("DEBUG")]
        static void check<T, U, V, W>(T[,] a, U b, V[,] c, W[,] result)
        {
            if (rows(a) != rows(c) || cols(a) != cols(c))
                throw new DimensionMismatchException("c");
            if (rows(a) != rows(result) || cols(a) != cols(result))
                throw new DimensionMismatchException("result");
        }

    }
}
