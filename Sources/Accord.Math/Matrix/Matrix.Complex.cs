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

namespace Accord.Math.ComplexExtensions
{
    using AForge;
    using AForge.Math;
    using System;

    /// <summary>
    ///  Static class ComplexMatrix. Defines a set of extension methods
    ///  that operates mainly on multidimensional arrays and vectors of
    ///  AForge.NET's <seealso cref="Complex"/> data type.
    /// </summary>
    /// 
    public static class ComplexMatrix
    {

        /// <summary>
        ///   Computes the absolute value of an array of complex numbers.
        /// </summary>
        public static Complex[] Abs(this Complex[] x)
        {
            if (x == null) throw new ArgumentNullException("x");

            Complex[] r = new Complex[x.Length];
            for (int i = 0; i < x.Length; i++)
                r[i] = new Complex(x[i].Magnitude, 0);
            return r;
        }

        /// <summary>
        ///   Computes the sum of an array of complex numbers.
        /// </summary>
        public static Complex Sum(this Complex[] x)
        {
            if (x == null) throw new ArgumentNullException("x");

            Complex r = Complex.Zero;
            for (int i = 0; i < x.Length; i++)
                r += x[i];
            return r;
        }

        /// <summary>
        ///   Elementwise multiplication of two complex vectors.
        /// </summary>
        public static Complex[] Multiply(this Complex[] a, Complex[] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            Complex[] r = new Complex[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Complex.Multiply(a[i], b[i]);
            }
            return r;
        }

        /// <summary>
        ///   Gets the magnitude of every complex number in an array.
        /// </summary>
        /// 
        public static double[] Magnitude(this Complex[] c)
        {
            if (c == null) throw new ArgumentNullException("c");

            double[] magnitudes = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                magnitudes[i] = c[i].Magnitude;

            return magnitudes;
        }

        /// <summary>
        ///   Gets the magnitude of every complex number in a matrix.
        /// </summary>
        /// 
        public static double[,] Magnitude(this Complex[,] c)
        {
            if (c == null) throw new ArgumentNullException("c");

            int rows = c.GetLength(0);
            int cols = c.GetLength(1);

            double[,] magnitudes = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    magnitudes[i, j] = c[i, j].Magnitude;

            return magnitudes;
        }

        /// <summary>
        ///   Gets the phase of every complex number in an array.
        /// </summary>
        /// 
        public static double[] Phase(this Complex[] c)
        {
            if (c == null) throw new ArgumentNullException("c");

            double[] phases = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                phases[i] = c[i].Phase;

            return phases;
        }

        /// <summary>
        ///   Returns the real vector part of the complex vector c.
        /// </summary>
        /// 
        /// <param name="c">A vector of complex numbers.</param>
        /// 
        /// <returns>A vector of scalars with the real part of the complex numbers.</returns>
        /// 
        public static double[] Re(this Complex[] c)
        {
            if (c == null) throw new ArgumentNullException("c");

            double[] re = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                re[i] = c[i].Re;

            return re;
        }

        /// <summary>
        ///   Returns the imaginary vector part of the complex vector c.
        /// </summary>
        /// 
        /// <param name="c">A vector of complex numbers.</param>
        /// 
        /// <returns>A vector of scalars with the imaginary part of the complex numbers.</returns>
        /// 
        public static double[] Im(this Complex[] c)
        {
            if (c == null) throw new ArgumentNullException("c");

            double[] im = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                im[i] = c[i].Im;

            return im;
        }

        /// <summary>
        ///   Converts a complex number to a matrix of scalar values
        ///   in which the first column contains the real values and 
        ///   the second column contains the imaginary values.
        /// </summary>
        /// <param name="c">An array of complex numbers.</param>
        public static double[,] ToArray(this Complex[] c)
        {
            if (c == null) throw new ArgumentNullException("c");

            double[,] arr = new double[c.Length, 2];
            for (int i = 0; i < c.GetLength(0); i++)
            {
                arr[i, 0] = c[i].Re;
                arr[i, 1] = c[i].Im;
            }

            return arr;
        }

        /// <summary>
        ///   Gets the range of the magnitude values in a complex number vector.
        /// </summary>
        /// 
        /// <param name="array">A complex number vector.</param>
        /// <returns>The range of magnitude values in the complex vector.</returns>
        /// 
        public static DoubleRange Range(this Complex[] array)
        {
            if (array == null) throw new ArgumentNullException("array");

            double min = array[0].SquaredMagnitude;
            double max = array[0].SquaredMagnitude;

            for (int i = 1; i < array.Length; i++)
            {
                double sqMagnitude = array[i].SquaredMagnitude;
                if (min > sqMagnitude)
                    min = sqMagnitude;
                if (max < sqMagnitude)
                    max = sqMagnitude;
            }

            return new DoubleRange(
                System.Math.Sqrt(min),
                System.Math.Sqrt(max));
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this Complex[][] objA, Complex[][] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                for (int j = 0; j < objA[i].Length; j++)
                {
                    double xr = objA[i][j].Re, yr = objB[i][j].Re;
                    double xi = objA[i][j].Im, yi = objB[i][j].Im;

                    if (Math.Abs(xr - yr) > threshold || (Double.IsNaN(xr) ^ Double.IsNaN(yr)))
                        return false;

                    if (Math.Abs(xi - yi) > threshold || (Double.IsNaN(xr) ^ Double.IsNaN(yr)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two vectors for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this Complex[] objA, Complex[] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                double xr = objA[i].Re, yr = objB[i].Re;
                double xi = objA[i].Im, yi = objB[i].Im;

                if (Math.Abs(xr - yr) > threshold || (Double.IsNaN(xr) ^ Double.IsNaN(yr)))
                    return false;

                if (Math.Abs(xi - yi) > threshold || (Double.IsNaN(xi) ^ Double.IsNaN(yi)))
                    return false;
            }
            return true;
        }
    }
}
