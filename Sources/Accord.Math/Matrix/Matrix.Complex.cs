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
    using Accord.Compat;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///  Static class ComplexExtensions. Defines a set of extension methods
    ///  that operates mainly on multidimensional arrays and vectors of
    ///  AForge.NET's <seealso cref="Complex"/> data type.
    /// </summary>
    /// 
    public static class ComplexMatrix
    {

        /// <summary>
        ///   Computes the absolute value of an array of complex numbers.
        /// </summary>
        /// 
        public static Complex[] Abs(this Complex[] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            Complex[] r = new Complex[x.Length];
            for (int i = 0; i < x.Length; i++)
                r[i] = new Complex(x[i].Magnitude, 0);
            return r;
        }

        /// <summary>
        ///   Computes the sum of an array of complex numbers.
        /// </summary>
        /// 
        public static Complex Sum(this Complex[] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            Complex r = Complex.Zero;
            for (int i = 0; i < x.Length; i++)
                r += x[i];
            return r;
        }

        /// <summary>
        ///   Elementwise multiplication of two complex vectors.
        /// </summary>
        /// 
        public static Complex[] Multiply(this Complex[] a, Complex[] b)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");

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
            return c.Apply((x, i) => x.Magnitude);
        }

        /// <summary>
        ///   Gets the magnitude of every complex number in a matrix.
        /// </summary>
        /// 
        public static double[,] Magnitude(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Magnitude);
        }

        /// <summary>
        ///   Gets the magnitude of every complex number in a matrix.
        /// </summary>
        /// 
        public static double[][] Magnitude(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Magnitude);
        }

        /// <summary>
        ///   Gets the phase of every complex number in an array.
        /// </summary>
        /// 
        public static double[] Phase(this Complex[] c)
        {
            return c.Apply((x, i) => x.Phase);
        }

        /// <summary>
        ///   Gets the phase of every complex number in a matrix.
        /// </summary>
        /// 
        public static double[,] Phase(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Phase);
        }

        /// <summary>
        ///   Gets the phase of every complex number in a matrix.
        /// </summary>
        /// 
        public static double[][] Phase(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Phase);
        }

        /// <summary>
        ///   Gets the conjugate of every complex number in an array.
        /// </summary>
        /// 
        public static Complex[] Conjugate(this Complex[] c)
        {
            return c.Apply((x, i) => Complex.Conjugate(x));
        }

        /// <summary>
        ///   Gets the conjugate of every complex number in a matrix.
        /// </summary>
        /// 
        public static Complex[,] Conjugate(this Complex[,] c)
        {
            return c.Apply((x, i, j) => Complex.Conjugate(x));
        }

        /// <summary>
        ///   Gets the conjugate of every complex number in a matrix.
        /// </summary>
        /// 
        public static Complex[][] Conjugate(this Complex[][] c)
        {
            return c.Apply((x, i, j) => Complex.Conjugate(x));
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
            return c.Apply((x, i) => x.Real);
        }

        /// <summary>
        ///   Returns the real matrix part of the complex matrix c.
        /// </summary>
        /// 
        /// <param name="c">A matrix of complex numbers.</param>
        /// 
        /// <returns>A matrix of scalars with the real part of the complex numbers.</returns>
        /// 
        public static double[,] Re(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Real);
        }

        /// <summary>
        ///   Returns the real matrix part of the complex matrix c.
        /// </summary>
        /// 
        /// <param name="c">A matrix of complex numbers.</param>
        /// 
        /// <returns>A matrix of scalars with the real part of the complex numbers.</returns>
        /// 
        public static double[][] Re(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Real);
        }

        /// <summary>
        ///   Returns the imaginary vector part of the complex vector c.
        /// </summary>
        /// 
        /// <param name="c">A vector of complex numbers.</param>
        /// 
        /// <returns>A vector of scalars with the imaginary part of the complex numbers.</returns>
        /// 
        // TODO: Rename to Imaginary
        public static double[] Im(this Complex[] c)
        {
            return c.Apply((x, i) => x.Imaginary);
        }

        /// <summary>
        /// Returns the imaginary matrix part of the complex matrix c.
        /// </summary>
        /// <param name="c">A matrix of complex numbers.</param>
        /// <returns>A matrix of scalars with the imaginary part of the complex numbers.</returns>
        public static double[,] Im(this Complex[,] c)
        {
            return c.Apply((x, i, j) => x.Imaginary);
        }

        /// <summary>
        /// Returns the imaginary matrix part of the complex matrix c.
        /// </summary>
        /// <param name="c">A matrix of complex numbers.</param>
        /// <returns>A matrix of scalars with the imaginary part of the complex numbers.</returns>
        public static double[][] Im(this Complex[][] c)
        {
            return c.Apply((x, i, j) => x.Imaginary);
        }

        /// <summary>
        ///   Converts a complex number to a matrix of scalar values
        ///   in which the first column contains the real values and 
        ///   the second column contains the imaginary values.
        /// </summary>
        /// <param name="c">An array of complex numbers.</param>
        public static double[,] ToArray(this Complex[] c)
        {
            if (c == null)
                throw new ArgumentNullException("c");

            double[,] arr = new double[c.Length, 2];
            for (int i = 0; i < c.GetLength(0); i++)
            {
                arr[i, 0] = c[i].Real;
                arr[i, 1] = c[i].Imaginary;
            }

            return arr;
        }

        /// <summary>
        ///   Converts a vector of real numbers to complex numbers.
        /// </summary>
        /// 
        /// <param name="real">The real numbers to be converted.</param>
        /// 
        /// <returns>
        ///   A vector of complex number with the given 
        ///   real numbers as their real components.
        /// </returns>
        /// 
        public static Complex[] ToComplex(this double[] real)
        {
            if (real == null)
                throw new ArgumentNullException("real");

            Complex[] arr = new Complex[real.Length];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = new Complex(real[i], 0);

            return arr;
        }

        /// <summary>
        ///   Combines a vector of real and a vector of
        ///   imaginary numbers to form complex numbers.
        /// </summary>
        /// 
        /// <param name="real">The real part of the complex numbers.</param>
        /// <param name="imag">The imaginary part of the complex numbers</param>
        /// 
        /// <returns>
        ///   A vector of complex number with the given 
        ///   real numbers as their real components and
        ///   imaginary numbers as their imaginary parts.
        /// </returns>
        /// 
        public static Complex[] ToComplex(this double[] real, double[] imag)
        {
            if (real == null)
                throw new ArgumentNullException("real");

            if (imag == null)
                throw new ArgumentNullException("imag");

            if (real.Length != imag.Length)
                throw new DimensionMismatchException("imag");

            Complex[] arr = new Complex[real.Length];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = new Complex(real[i], imag[i]);

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
            if (array == null)
                throw new ArgumentNullException("array");

            double min = array[0].Magnitude;
            double max = array[0].Magnitude;

            for (int i = 1; i < array.Length; i++)
            {
                double value = array[i].Magnitude;

                if (min > value)
                    min = value;
                if (max < value)
                    max = value;
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
                    double xr = objA[i][j].Real;
                    double yr = objB[i][j].Real;
                    double xi = objA[i][j].Imaginary;
                    double yi = objB[i][j].Imaginary;

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
                double xr = objA[i].Real;
                double yr = objB[i].Real;
                double xi = objA[i].Imaginary;
                double yi = objB[i].Imaginary;

                if (Math.Abs(xr - yr) > threshold || (Double.IsNaN(xr) ^ Double.IsNaN(yr)))
                    return false;

                if (Math.Abs(xi - yi) > threshold || (Double.IsNaN(xi) ^ Double.IsNaN(yi)))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   Gets the squared magnitude of a complex number.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double SquaredMagnitude(this Complex value)
        {
            return value.Magnitude * value.Magnitude;
        }

    }
}
