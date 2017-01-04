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

    public static partial class Matrix
    {

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static int[] Abs(int[] value)
        {
            return Elementwise.Abs(value);
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] Abs(double[] value)
        {
            return Elementwise.Abs(value);
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] Sign(double[] value)
        {
            return Elementwise.Sign(value);
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] Abs(double[,] value)
        {
            return Elementwise.Abs(value);
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static int[,] Abs(int[,] value)
        {
            return Elementwise.Abs(value);
        }

        /// <summary>
        ///   Elementwise Square root.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] Sqrt(double[] value)
        {
            return Elementwise.Sqrt(value);
        }

        /// <summary>
        ///   Elementwise Square root.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] Sqrt(double[,] value)
        {
            return Elementwise.Sqrt(value);
        }

        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] Log(double[,] value)
        {
            return Elementwise.Log(value);
        }

        /// <summary>
        ///   Elementwise Exp operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] Exp(double[,] value)
        {
            return Elementwise.Exp(value);
        }

        /// <summary>
        ///   Elementwise Exp operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] Exp(double[] value)
        {
            return Elementwise.Exp(value);
        }


        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] Log(double[] value)
        {
            return Elementwise.Log(value);
        }


        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] Log(int[] value)
        {
            return Elementwise.Log(value);
        }


        /// <summary>
        ///   Elementwise power operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] ElementwisePower(double[,] x, double y)
        {
            return Elementwise.Pow(x, y);
        }

        /// <summary>
        ///   Elementwise power operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] ElementwisePower(double[] x, double y)
        {
            return Elementwise.Pow(x, y);
        }


        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] ElementwiseDivide(double[] a, double[] b)
        {
            return Elementwise.Divide(a, b);
        }

        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] ElementwiseDivide(this double[,] a, double[,] b)
        {
            return Elementwise.Divide(a, b);
        }

        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static float[,] ElementwiseDivide(float[,] a, float[,] b)
        {
            return Elementwise.Divide(a, b);
        }

        /// <summary>
        ///   Elementwise division.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] ElementwiseDivide(double[,] a, double[] b, int dimension = 0, bool inPlace = false)
        {
            if (inPlace)
                return Elementwise.Divide(a, b, dimension, a);
            return Elementwise.Divide(a, b, dimension);
        }



        /// <summary>
        ///   Elementwise division.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] ElementwiseDivide(int[,] a, int[] b, int dimension)
        {
            return Elementwise.Divide(a, b, dimension);
        }


        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] ElementwiseMultiply(double[] a, double[] b)
        {
            return Elementwise.Multiply(a, b);
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[] ElementwiseMultiply(double[] a, int[] b)
        {
            return Elementwise.Multiply(a, b);
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        /// 
        public static double[,] ElementwiseMultiply(double[,] a, double[,] b)
        {
            return Elementwise.Multiply(a, b);
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static int[] ElementwiseMultiply(int[] a, int[] b)
        {
            return Elementwise.Multiply(a, b);
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static int[,] ElementwiseMultiply(int[,] a, int[,] b)
        {
            return Elementwise.Multiply(a, b);
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] ElementwiseMultiply(double[,] a, double[] b, int dimension)
        {
            return Elementwise.Multiply(a, b, dimension);
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        [Obsolete("Please use the functions in the Elementwise class instead.")]
        public static double[,] ElementwiseMultiply(double[,] a, double[] b, double[,] r, int dimension)
        {
            return Elementwise.Multiply(a, b, dimension, r);
        }

    }
}
