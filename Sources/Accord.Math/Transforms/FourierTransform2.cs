// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2014
// diego.catalano at live.com
//
// Copyright © Nayuki Minase, 2014
// nayuki at eigenstate.org
// http://nayuki.eigenstate.org/page/free-small-fft-in-multiple-languages
//
// Copyright © Guney Ozsan, 2019
// guneyozsan at gmail.com
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
// Contains code distributed by Project Nayuki, available under a MIT license 
// at http://nayuki.eigenstate.org/page/free-small-fft-in-multiple-languages
//
// The original license is listed below:
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of
//    this software and associated documentation files (the "Software"), to deal in
//    the Software without restriction, including without limitation the rights to
//    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//    the Software, and to permit persons to whom the Software is furnished to do so,
//    subject to the following conditions:
//
//     - The above copyright notice and this permission notice shall be included in
//       all copies or substantial portions of the Software.
//
//   The Software is provided "as is", without warranty of any kind, express or
//   implied, including but not limited to the warranties of merchantability,
//   fitness for a particular purpose and noninfringement. In no event shall the
//   authors or copyright holders be liable for any claim, damages or other
//   liability, whether in an action of contract, tort or otherwise, arising from,
//   out of or in connection with the Software or the use or other dealings in the
//   Software.
//

namespace Accord.Math.Transforms
{
    using System;
    using System.Runtime.CompilerServices;
    using Accord.Compat;
    using System.Numerics;

    /// <summary>
    ///   Fourier Transform (for arbitrary size matrices).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The transforms in this class accept arbitrary-length matrices and are not restricted to 
    ///   only matrices that have dimensions which are powers of two. It also provides results which 
    ///   are more equivalent with other mathematical packages, such as MATLAB and Octave.</para>
    /// <para>
    ///   This class had been created as an alternative to <see cref="FourierTransform">AForge.NET's 
    ///   original FourierTransform class</see> that would provide more expected results.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following examples show how to compute 1-D Discrete Fourier Transform and 
    ///   1-D Fast Fourier Transforms, respectively:</para>
    /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_dft" />
    /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_fft" />
    /// 
    /// <para>
    ///   The next examples show how to compute 2-D Discrete Fourier Transform and 
    ///   2-D Fast Fourier Transforms, respectively:</para>
    /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_dft2" />
    /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_fft2" />
    /// </example>
    /// 
    /// <seealso cref="FourierTransform"/>
    /// 
    public static class FourierTransform2
    {
        // Trigonometric tables cached.
        private static double[] cosTable;
        private static double[] sinTable;
        private static double[] expCosTable;
        private static double[] expSinTable;

        /// <summary>
        ///   1-D Discrete Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">The data to transform.</param>
        /// <param name="direction">The transformation direction.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_dft" />
        /// </example>
        /// 
        public static void DFT(Complex[] data, FourierTransform.Direction direction)
        {
            int n = data.Length;
            var c = new Complex[n];

            // for each destination element
            for (int i = 0; i < c.Length; i++)
            {
                double sumRe = 0;
                double sumIm = 0;
                double phim = 2 * Math.PI * i / (double)n;

                // sum source elements
                for (int j = 0; j < n; j++)
                {
                    double re = data[j].Real;
                    double im = data[j].Imaginary;
                    double cosw = Math.Cos(phim * j);
                    double sinw = Math.Sin(phim * j);

                    if (direction == FourierTransform.Direction.Backward)
                        sinw = -sinw;

                    sumRe += (re * cosw + im * sinw);
                    sumIm += (im * cosw - re * sinw);
                }

                c[i] = new Complex(sumRe, sumIm);
            }

            if (direction == FourierTransform.Direction.Backward)
            {
                for (int i = 0; i < c.Length; i++)
                    data[i] = c[i] / n;
            }
            else
            {
                for (int i = 0; i < c.Length; i++)
                    data[i] = c[i];
            }
        }

        /// <summary>
        ///   2-D Discrete Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">The data to transform.</param>
        /// <param name="direction">The transformation direction.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_dft2" />
        /// </example>
        /// 
        public static void DFT2(Complex[][] data, FourierTransform.Direction direction)
        {
            int m = data.Columns();

            if (direction == FourierTransform.Direction.Forward)
            {
                // process rows
                for (int i = 0; i < data.Length; i++)
                {
                    // transform it
                    DFT(data[i], FourierTransform.Direction.Forward);
                }

                // process columns
                var col = new Complex[data.Length];
                for (int j = 0; j < m; j++)
                {
                    // copy column
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i][j];

                    // transform it
                    DFT(col, FourierTransform.Direction.Forward);

                    // copy back
                    for (int i = 0; i < col.Length; i++)
                        data[i][j] = col[i];
                }
            }
            else
            {
                // process columns
                var col = new Complex[data.Length];
                for (int j = 0; j < m; j++)
                {
                    // copy column
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i][j];

                    // transform it
                    DFT(col, FourierTransform.Direction.Backward);

                    // copy back
                    for (int i = 0; i < col.Length; i++)
                        data[i][j] = col[i];
                }

                // process rows
                for (int i = 0; i < data.Length; i++)
                {
                    // transform it
                    DFT(data[i], FourierTransform.Direction.Backward);
                }
            }
        }

        /// <summary>
        ///   1-D Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">The data to transform..</param>
        /// <param name="direction">The transformation direction.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_fft" />
        /// </example>
        /// 
        public static void FFT(Complex[] data, FourierTransform.Direction direction)
        {
            int n = data.Length;

            if (n == 0)
                return;

            if (direction == FourierTransform.Direction.Backward)
            {
                for (int i = 0; i < data.Length; i++)
                    data[i] = new Complex(data[i].Imaginary, data[i].Real);
            }

            if ((n & (n - 1)) == 0)
            {
                // Is power of 2
                TransformRadix2(data);
            }
            else
            {
                // More complicated algorithm for arbitrary sizes
                TransformBluestein(data);
            }

            if (direction == FourierTransform.Direction.Backward)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    double im = data[i].Imaginary;
                    double re = data[i].Real;
                    data[i] = new Complex(im / n, re / n);
                }
            }
        }

        /// <summary>
        ///   1-D Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="real">The real part of the complex numbers to transform.</param>
        /// <param name="imag">The imaginary part of the complex numbers to transform.</param>
        /// <param name="direction">The transformation direction.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_fft" />
        /// </example>
        /// 
        public static void FFT(double[] real, double[] imag, FourierTransform.Direction direction)
        {
            if (direction == FourierTransform.Direction.Forward)
            {
                FFT(real, imag);
            }
            else
            {
                FFT(imag, real);
            }

            if (direction == FourierTransform.Direction.Backward)
            {
                for (int i = 0; i < real.Length; i++)
                {
                    real[i] /= real.Length;
                    imag[i] /= real.Length;
                }
            }
        }

        /// <summary>
        ///   2-D Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">The data to transform.</param>
        /// <param name="direction">The Transformation direction.</param>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Math\FourierTransformTest.cs" region="doc_fft2" />
        /// </example>
        /// 
        public static void FFT2(Complex[][] data, FourierTransform.Direction direction)
        {
            int n = data.Length;
            int m = data[0].Length;

            // process rows
            for (int i = 0; i < data.Length; i++)
            {
                // transform it
                FFT(data[i], direction);
            }

            // process columns
            var col = new Complex[n];
            for (int j = 0; j < m; j++)
            {
                // copy column
                for (int i = 0; i < col.Length; i++)
                    col[i] = data[i][j];

                // transform it
                FFT(col, direction);

                // copy back
                for (int i = 0; i < col.Length; i++)
                    data[i][j] = col[i];
            }
        }

        /// <summary>
        ///   Computes the discrete Fourier transform (DFT) of the given complex vector, 
        ///   storing the result back into the vector. The vector can have any length. 
        ///   This is a wrapper function.
        /// </summary>
        /// 
        /// <param name="real">The real.</param>
        /// <param name="imag">The imag.</param>
        /// 
        private static void FFT(double[] real, double[] imag)
        {
            int n = real.Length;

            if (n == 0)
                return;

            if ((n & (n - 1)) == 0)
            {
                // Is power of 2
                TransformRadix2(real, imag);
            }
            else
            {
                // More complicated algorithm for arbitrary sizes
                TransformBluestein(real, imag);
            }
        }

        /// <summary>
        ///   Computes the inverse discrete Fourier transform (IDFT) of the given complex 
        ///   vector, storing the result back into the vector. The vector can have any length.
        ///   This is a wrapper function. This transform does not perform scaling, so the 
        ///   inverse is not a true inverse.
        /// </summary>
        /// 
        private static void IDFT(Complex[] data)
        {
            int n = data.Length;

            if (n == 0)
                return;

            for (int i = 0; i < data.Length; i++)
                data[i] = new Complex(data[i].Imaginary, data[i].Real);

            if ((n & (n - 1)) == 0)
            {
                // Is power of 2
                TransformRadix2(data);
            }
            else
            {
                // More complicated algorithm for arbitrary sizes
                TransformBluestein(data);
            }

            for (int i = 0; i < data.Length; i++)
            {
                double im = data[i].Imaginary;
                double re = data[i].Real;
                data[i] = new Complex(im, re);
            }
        }


        /// <summary>
        ///   Computes the inverse discrete Fourier transform (IDFT) of the given complex 
        ///   vector, storing the result back into the vector. The vector can have any length.
        ///   This is a wrapper function. This transform does not perform scaling, so the 
        ///   inverse is not a true inverse.
        /// </summary>
        /// 
        private static void IDFT(double[] real, double[] imag)
        {
            FFT(imag, real);
        }

        /// <summary>
        ///   Computes the discrete Fourier transform (DFT) of the given complex vector, storing 
        ///   the result back into the vector. The vector's length must be a power of 2. Uses the 
        ///   Cooley-Tukey decimation-in-time radix-2 algorithm.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentException">Length is not a power of 2.</exception>
        /// 
        private static void TransformRadix2(double[] real, double[] imag)
        {
            int n = real.Length;

            int levels = (int)Math.Floor(Math.Log(n, 2));

            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            // Trigonometric tables.
            double[] cosTable = CosTable(n / 2);
            double[] sinTable = SinTable(n / 2);

            // Bit-reversed addressing permutation
            for (int i = 0; i < real.Length; i++)
            {
                int j = unchecked((int)((uint)Reverse(i) >> (32 - levels)));

                if (j > i)
                {
                    var temp = real[i];
                    real[i] = real[j];
                    real[j] = temp;

                    temp = imag[i];
                    imag[i] = imag[j];
                    imag[j] = temp;
                }
            }

            // Cooley-Tukey decimation-in-time radix-2 FFT
            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;

                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        int h = j + halfsize;
                        double re = real[h];
                        double im = imag[h];

                        double tpre = +re * cosTable[k] + im * sinTable[k];
                        double tpim = -re * sinTable[k] + im * cosTable[k];

                        real[h] = real[j] - tpre;
                        imag[h] = imag[j] - tpim;

                        real[j] += tpre;
                        imag[j] += tpim;
                    }
                }

                // Prevent overflow in 'size *= 2'
                if (size == n)
                    break;
            }
        }

        /// <summary>
        ///   Computes the discrete Fourier transform (DFT) of the given complex vector, storing 
        ///   the result back into the vector. The vector's length must be a power of 2. Uses the 
        ///   Cooley-Tukey decimation-in-time radix-2 algorithm.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentException">Length is not a power of 2.</exception>
        /// 
        private static void TransformRadix2(Complex[] complex)
        {
            int n = complex.Length;

            int levels = (int)Math.Floor(Math.Log(n, 2));

            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");
            
            // Trigonometric tables.
            double[] cosTable = CosTable(n / 2);
            double[] sinTable = SinTable(n / 2);

            // Bit-reversed addressing permutation
            for (int i = 0; i < complex.Length; i++)
            {
                int j = unchecked((int)((uint)Reverse(i) >> (32 - levels)));

                if (j > i)
                {
                    var temp = complex[i];
                    complex[i] = complex[j];
                    complex[j] = temp;
                }
            }


            // Cooley-Tukey decimation-in-time radix-2 FFT
            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;

                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        int h = j + halfsize;
                        double re = complex[h].Real;
                        double im = complex[h].Imaginary;

                        double tpre = +re * cosTable[k] + im * sinTable[k];
                        double tpim = -re * sinTable[k] + im * cosTable[k];

                        double rej = complex[j].Real;
                        double imj = complex[j].Imaginary;

                        complex[h] = new Complex(rej - tpre, imj - tpim);
                        complex[j] = new Complex(rej + tpre, imj + tpim);
                    }
                }

                // Prevent overflow in 'size *= 2'
                if (size == n)
                    break;
            }
        }


        /// <summary>
        ///   Computes the discrete Fourier transform (DFT) of the given complex vector, storing 
        ///   the result back into the vector. The vector can have any length. This requires the 
        ///   convolution function, which in turn requires the radix-2 FFT function. Uses 
        ///   Bluestein's chirp z-transform algorithm.
        /// </summary>
        /// 
        private static void TransformBluestein(double[] real, double[] imag)
        {
            int n = real.Length;
            int m = HighestOneBit(n * 2 + 1) << 1;

            // Trigonometric tables.
            double[] cosTable = ExpCosTable(n);
            double[] sinTable = ExpSinTable(n);
            
            // Temporary vectors and preprocessing
            var areal = new double[m];
            var aimag = new double[m];
            for (int i = 0; i < real.Length; i++)
            {
                areal[i] = +real[i] * cosTable[i] + imag[i] * sinTable[i];
                aimag[i] = -real[i] * sinTable[i] + imag[i] * cosTable[i];
            }

            var breal = new double[m];
            var bimag = new double[m];
            breal[0] = cosTable[0];
            bimag[0] = sinTable[0];

            for (int i = 1; i < cosTable.Length; i++)
            {
                breal[i] = breal[m - i] = cosTable[i];
                bimag[i] = bimag[m - i] = sinTable[i];
            }

            // Convolution
            var creal = new double[m];
            var cimag = new double[m];
            Convolve(areal, aimag, breal, bimag, creal, cimag);

            // Postprocessing
            for (int i = 0; i < n; i++)
            {
                real[i] = +creal[i] * cosTable[i] + cimag[i] * sinTable[i];
                imag[i] = -creal[i] * sinTable[i] + cimag[i] * cosTable[i];
            }
        }

        private static void TransformBluestein(Complex[] data)
        {
            int n = data.Length;
            int m = HighestOneBit(n * 2 + 1) << 1;

            // Trigonometric tables.
            double[] cosTable = ExpCosTable(n);
            double[] sinTable = ExpSinTable(n);
            
            // Temporary vectors and preprocessing
            var areal = new double[m];
            var aimag = new double[m];

            for (int i = 0; i < data.Length; i++)
            {
                double re = data[i].Real;
                double im = data[i].Imaginary;

                areal[i] = +re * cosTable[i] + im * sinTable[i];
                aimag[i] = -re * sinTable[i] + im * cosTable[i];
            }

            var breal = new double[m];
            var bimag = new double[m];
            breal[0] = cosTable[0];
            bimag[0] = sinTable[0];

            for (int i = 1; i < cosTable.Length; i++)
            {
                breal[i] = breal[m - i] = cosTable[i];
                bimag[i] = bimag[m - i] = sinTable[i];
            }

            // Convolution
            var creal = new double[m];
            var cimag = new double[m];
            Convolve(areal, aimag, breal, bimag, creal, cimag);

            // Postprocessing
            for (int i = 0; i < data.Length; i++)
            {
                double re = +creal[i] * cosTable[i] + cimag[i] * sinTable[i];
                double im = -creal[i] * sinTable[i] + cimag[i] * cosTable[i];
                data[i] = new Complex(re, im);
            }
        }

        /// <summary>
        ///   Computes the circular convolution of the given real 
        ///   vectors. All vectors must have the same length.
        /// </summary>
        /// 
        public static void Convolve(double[] x, double[] y, double[] result)
        {
            int n = x.Length;
            Convolve(x, new double[n], y, new double[n], result, new double[n]);
        }

        /// <summary>
        ///   Computes the circular convolution of the given complex 
        ///   vectors. All vectors must have the same length.
        /// </summary>
        /// 
        public static void Convolve(Complex[] x, Complex[] y, Complex[] result)
        {
            FFT(x, FourierTransform.Direction.Forward);
            FFT(y, FourierTransform.Direction.Forward);

            for (int i = 0; i < x.Length; i++)
            {
                double xreal = x[i].Real;
                double ximag = x[i].Imaginary;
                double yreal = y[i].Real;
                double yimag = y[i].Imaginary;

                double re = xreal * yreal - ximag * yimag;
                double im = ximag * yreal + xreal * yimag;

                x[i] = new Complex(re, im);
            }

            IDFT(x);

            // Scaling (because this FFT implementation omits it)
            for (int i = 0; i < x.Length; i++)
            {
                result[i] = x[i] / x.Length;
            }
        }

        /// <summary>
        ///   Computes the circular convolution of the given complex 
        ///   vectors. All vectors must have the same length.
        /// </summary>
        /// 
        public static void Convolve(double[] xreal, double[] ximag, double[] yreal, double[] yimag, double[] outreal, double[] outimag)
        {
            int n = xreal.Length;

            FFT(xreal, ximag);
            FFT(yreal, yimag);

            for (int i = 0; i < xreal.Length; i++)
            {
                var temp = xreal[i] * yreal[i] - ximag[i] * yimag[i];
                ximag[i] = ximag[i] * yreal[i] + xreal[i] * yimag[i];
                xreal[i] = temp;
            }

            IDFT(xreal, ximag);

            // Scaling (because this FFT implementation omits it)
            for (int i = 0; i < n; i++)
            {
                outreal[i] = xreal[i] / n;
                outimag[i] = ximag[i] / n;
            }
        }

        private static int HighestOneBit(int i)
        {
            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);
            i |= (i >> 16);
            return i - (int)((uint)i >> 1);
        }

        private static int Reverse(int i)
        {
            i = (i & 0x55555555) << 1 | (int)((uint)i >> 1) & 0x55555555;
            i = (i & 0x33333333) << 2 | (int)((uint)i >> 2) & 0x33333333;
            i = (i & 0x0f0f0f0f) << 4 | (int)((uint)i >> 4) & 0x0f0f0f0f;
            i = (i << 24) | ((i & 0xff00) << 8) |
                ((int)((uint)i >> 8) & 0xff00) | (int)((uint)i >> 24);
            return i;
        }

        /// <summary>
        ///   Computes the Magnitude spectrum of a complex signal.
        /// </summary>
        /// 
        public static double[] GetMagnitudeSpectrum(Complex[] fft)
        {
            if (fft == null)
                throw new ArgumentNullException("fft");

            // assumes fft is symmetric

            // In a two-sided spectrum, half the energy is displayed at the positive frequency,
            // and half the energy is displayed at the negative frequency. Therefore, to convert
            // from a two-sided spectrum to a single-sided spectrum, discard the second half of
            // the array and multiply every point except for DC by two.

            int numUniquePts = (int)System.Math.Ceiling((fft.Length + 1) / 2.0);
            double[] mx = new double[numUniquePts];

            mx[0] = fft[0].Magnitude / fft.Length;
            for (int i = 0; i < numUniquePts; i++)
                mx[i] = fft[i].Magnitude * 2 / fft.Length;

            return mx;
        }

        /// <summary>
        ///   Computes the Power spectrum of a complex signal.
        /// </summary>
        /// 
        public static double[] GetPowerSpectrum(Complex[] fft)
        {
            if (fft == null)
                throw new ArgumentNullException("fft");

            int n = (int)System.Math.Ceiling((fft.Length + 1) / 2.0);

            double[] mx = new double[n];

            mx[0] = fft[0].SquaredMagnitude() / fft.Length;

            for (int i = 1; i < n; i++)
                mx[i] = fft[i].SquaredMagnitude() * 2.0 / fft.Length;

            return mx;
        }

        /// <summary>
        ///   Computes the Phase spectrum of a complex signal.
        /// </summary>
        /// 
        public static double[] GetPhaseSpectrum(Complex[] fft)
        {
            if (fft == null) throw new ArgumentNullException("fft");

            int n = (int)System.Math.Ceiling((fft.Length + 1) / 2.0);

            double[] mx = new double[n];

            for (int i = 0; i < n; i++)
                mx[i] = fft[i].Phase;

            return mx;
        }

        /// <summary>
        ///   Creates an evenly spaced frequency vector (assuming a symmetric FFT)
        /// </summary>
        /// 
        public static double[] GetFrequencyVector(int length, int sampleRate)
        {
            int numUniquePts = (int)System.Math.Ceiling((length + 1) / 2.0);

            double[] freq = new double[numUniquePts];
            for (int i = 0; i < numUniquePts; i++)
                freq[i] = i * sampleRate / (double)length;

            return freq;
        }

        /// <summary>
        ///   Gets the spectral resolution for a signal of given sampling rate and number of samples.
        /// </summary>
        /// 
        public static double GetSpectralResolution(int samplingRate, int samples)
        {
            return samplingRate / (double)samples;
        }

        /// <summary>
        ///   Gets the power Cepstrum for a complex signal.
        /// </summary>
        /// 
        public static double[] GetPowerCepstrum(Complex[] signal)
        {
            if (signal == null)
                throw new ArgumentNullException("signal");

            FourierTransform.FFT(signal, FourierTransform.Direction.Backward);

            Complex[] logabs = new Complex[signal.Length];
            for (int i = 0; i < logabs.Length; i++)
                logabs[i] = new Complex(System.Math.Log(signal[i].Magnitude), 0);

            FourierTransform.FFT(logabs, FourierTransform.Direction.Forward);

            return logabs.Re();
        }

        /// <summary>
        ///   Gets a half period cosine table.
        ///   Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        /// 
        private static double[] CosTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (cosTable != null && sampleCount == cosTable.Length)
                return cosTable;

            // Create a new table and keep in memory.
            cosTable = new double[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                cosTable[i] = Math.Cos(Math.PI * i / sampleCount);
            }
            return cosTable;
        }

        /// <summary>
        ///   Gets a half period sinus table.
        ///   Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        ///
        private static double[] SinTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (sinTable != null && sampleCount == sinTable.Length)
                return sinTable;

            // Create a new table and keep in memory.
            sinTable = new double[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                sinTable[i] = Math.Sin(Math.PI * i / sampleCount);
            }
            return sinTable;
        }
        
        /// <summary>
        ///   Gets a cosine table with exponentially increasing frequency by i * i.
        ///   Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        ///
        private static double[] ExpCosTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (expCosTable != null && sampleCount == expCosTable.Length)
                return expCosTable;

            // Create a new table and keep in memory.
            expCosTable = new double[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                int j = (int)((long)i * i % (sampleCount * 2));  // This is more accurate than j = i * i
                expCosTable[i] = Math.Cos(Math.PI * j / sampleCount);
            }
            return expCosTable;
        }

        /// <summary>
        ///   Gets a sinus table with exponentially increasing frequency by i * i.
        ///   Keeps the results in memory and reuses if parameters are the same.
        /// </summary>
        ///
        private static double[] ExpSinTable(int sampleCount)
        {
            // Return table from memory if period matches.
            if (expSinTable != null && sampleCount == expSinTable.Length)
                return expSinTable;

            // Create a new table and keep in memory.
            expSinTable = new double[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                int j = (int)((long)i * i % (sampleCount * 2));  // This is more accurate than j = i * i
                expSinTable[i] = Math.Sin(Math.PI * j / sampleCount);
            }
            return expSinTable;
        }
    }
}
