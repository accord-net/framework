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


    public static partial class Matrix
    {

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        /// 
        public static int[] Abs(this int[] value)
        {
            int[] r = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Abs(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        /// 
        public static double[] Abs(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Abs(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        /// 
        public static double[] Sign(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Sign(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        /// 
        public static double[,] Abs(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            double[,] r = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Abs(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        /// 
        public static int[,] Abs(this int[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            int[,] r = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Abs(value[i, j]);
            return r;
        }


        /// <summary>
        ///   Elementwise Square root.
        /// </summary>
        /// 
        public static double[] Sqrt(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Sqrt(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise Square root.
        /// </summary>
        /// 
        public static double[,] Sqrt(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            double[,] r = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Sqrt(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        /// 
        public static double[,] Log(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            double[,] r = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Log(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise Exp operation.
        /// </summary>
        /// 
        public static double[,] Exp(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            double[,] r = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Exp(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise Exp operation.
        /// </summary>
        /// 
        public static double[] Exp(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Exp(value[i]);
            return r;
        }


        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        /// 
        public static double[] Log(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Log(value[i]);
            return r;
        }


        /// <summary>
        ///   Elementwise power operation.
        /// </summary>
        /// 
        /// <param name="x">A matrix.</param>
        /// <param name="y">A power.</param>
        /// 
        /// <returns>Returns x elevated to the power of y.</returns>
        /// 
        public static double[,] ElementwisePower(this double[,] x, double y)
        {
            double[,] r = new double[x.GetLength(0), x.GetLength(1)];

            for (int i = 0; i < x.GetLength(0); i++)
                for (int j = 0; j < x.GetLength(1); j++)
                    r[i, j] = System.Math.Pow(x[i, j], y);

            return r;
        }

        /// <summary>
        ///   Elementwise power operation.
        /// </summary>
        /// 
        /// <param name="x">A matrix.</param>
        /// <param name="y">A power.</param>
        /// 
        /// <returns>Returns x elevated to the power of y.</returns>
        /// 
        public static double[] ElementwisePower(this double[] x, double y)
        {
            double[] r = new double[x.Length];

            for (int i = 0; i < r.Length; i++)
                r[i] = System.Math.Pow(x[i], y);

            return r;
        }


        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        /// 
        public static double[] ElementwiseDivide(this double[] a, double[] b)
        {
            double[] r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] / b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        /// 
        public static double[,] ElementwiseDivide(this double[,] a, double[,] b)
        {
            int rows = a.GetLength(0);
            int cols = b.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j] / b[i, j];

            return r;
        }


        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        /// 
        public static float[,] ElementwiseDivide(this float[,] a, float[,] b)
        {
            int rows = a.GetLength(0);
            int cols = b.GetLength(1);

            float[,] r = new float[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j] / b[i, j];

            return r;
        }


        /// <summary>
        ///   Elementwise division.
        /// </summary>
        /// 
        public static double[,] ElementwiseDivide(this double[,] a, double[] b, int dimension = 0, bool inPlace = false)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double[,] r = (inPlace) ? a : new double[rows, cols];

            if (dimension == 1)
            {
                if (cols != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of columns in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = a[i, j] / b[j];
            }
            else
            {
                if (rows != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = a[i, j] / b[i];
            }

            return r;
        }

        /// <summary>
        ///   Elementwise division.
        /// </summary>
        /// 
        public static double[][] ElementwiseDivide(this double[][] a, double[] b, int dimension = 0, bool inPlace = false)
        {
            int rows = a.Length;
            int cols = a[0].Length;

            double[][] r = a;

            if (!inPlace)
            {
                r = new double[rows][];
                for (int i = 0; i < r.Length; i++)
                    r[i] = new double[cols];
            }

            if (dimension == 0)
            {
                if (cols != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of columns in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i][j] = a[i][j] / b[j];
            }
            else
            {
                if (rows != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i][j] = a[i][j] / b[i];
            }

            return r;
        }

        /// <summary>
        ///   Elementwise division.
        /// </summary>
        /// 
        public static double[,] ElementwiseDivide(this int[,] a, int[] b, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double[,] r = new double[rows, cols];

            if (dimension == 1)
            {
                if (cols != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of columns in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = a[i, j] / (double)b[j];
            }
            else
            {
                if (rows != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = a[i, j] / (double)b[i];
            }

            return r;
        }


        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        public static double[] ElementwiseMultiply(this double[] a, double[] b)
        {
            double[] r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] * b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        public static double[] ElementwiseMultiply(this double[] a, int[] b)
        {
            double[] r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] * b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        /// 
        public static double[,] ElementwiseMultiply(this double[,] a, double[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must agree.", "b");

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double[,] r = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j] * b[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        /// 
        public static int[] ElementwiseMultiply(this int[] a, int[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector dimensions must agree.", "b");

            int[] r = new int[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] * b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        /// 
        public static int[,] ElementwiseMultiply(this int[,] a, int[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must agree.", "b");

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            int[,] r = new int[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j] * b[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        /// 
        /// <param name="a">The left matrix a.</param>
        /// <param name="b">The right vector b.</param>
        /// <param name="dimension">
        ///   If set to 0, b will be multiplied with every row vector in a. 
        ///   If set to 1, b will be multiplied with every column vector.
        /// </param>
        /// 
        public static double[,] ElementwiseMultiply(this double[,] a, double[] b, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double[,] r = new double[rows, cols];

            return ElementwiseMultiply(a, b, r, dimension);
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        /// 
        /// <param name="a">The left matrix a.</param>
        /// <param name="b">The right vector b.</param>
        /// <param name="r">The result vector r.</param>
        /// <param name="dimension">
        ///   If set to 0, b will be multiplied with every row vector in a. 
        ///   If set to 1, b will be multiplied with every column vector.
        /// </param>
        /// 
        public static double[,] ElementwiseMultiply(this double[,] a, double[] b, double[,] r, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            if (dimension == 1)
            {
                if (cols != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of columns in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = a[i, j] * b[j];
            }
            else
            {
                if (rows != b.Length) throw new ArgumentException(
                        "Length of B should be equals to the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = a[i, j] * b[i];
            }

            return r;
        }



    }
}
