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
// Contains functions from the Free FFT and convolution:
// Copyright © Nayuki Minase, 2014
// Original work: http://nayuki.eigenstate.org/page/free-small-fft-in-multiple-languages
//
// Original license is listed below:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// - The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.
// - The Software is provided "as is", without warranty of any kind, express or
//    implied, including but not limited to the warranties of merchantability,
//    fitness for a particular purpose and noninfringement. In no event shall the
//    authors or copyright holders be liable for any claim, damages or other
//    liability, whether in an action of contract, tort or otherwise, arising from,
//    out of or in connection with the Software or the use or other dealings in the
//    Software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Accord.Math.Transforms
{
    /// <summary>
    /// Fourier Transform.
    /// </summary>
    public class FourierTransform2
    {
        /**
         * Transformation direction.
         */
        public enum Direction
        {
            /// <summary>
            /// Forward direction of Fourier transformation.
            /// </summary>
            Forward,

            /// <summary>
            /// Backward direction of Fourier transformation.
            /// </summary>
            Backward
        };

        /**
        * 1-D Discrete Fourier Transform.
        * @param data Data to transform.
        * @param direction Transformation direction.
        */
        public static void DFT(Complex[] data, Direction direction)
        {
            int n = data.GetLength(0);
            Complex[] c = new Complex[n];

            // for each destination element
            for (int i = 0; i < n; i++)
            {
                c[i] = new Complex(0, 0);
                double sumRe = 0;
                double sumIm = 0;
                double phim = 2 * System.Math.PI * i / n;

                // sum source elements
                for (int j = 0; j < n; j++)
                {
                    double gRe = data[j].Real;
                    double gIm = data[j].Imaginary;
                    double cosw = System.Math.Cos(phim * j);
                    double sinw = System.Math.Sin(phim * j);
                    if (direction == Direction.Backward)
                        sinw = -sinw;

                    sumRe += (gRe * cosw + data[j].Imaginary * sinw);
                    sumIm += (gIm * cosw - data[j].Real * sinw);
                }

                c[i] = new Complex(sumRe, sumIm);
            }

            if (direction == Direction.Backward)
            {
                for (int i = 0; i < c.GetLength(0); i++)
                    data[i] = new Complex(c[i].Real / n, c[i].Imaginary / n);

            }
            else
            {
                for (int i = 0; i < c.GetLength(0); i++)
                    data[i] = new Complex(c[i].Real, c[i].Imaginary);

            }
        }

        /**
         * 2-D Discrete Fourier Transform.
         * @param data Data to transform.
         * @param direction Transformation direction.
         */
        public static void DFT2(Complex[][] data, Direction direction)
        {

            int n = data.GetLength(0);
            int m = data[0].GetLength(1);
            Complex[] row = new Complex[System.Math.Max(m, n)];

            for (int i = 0; i < n; i++)
            {
                // copy row
                for (int j = 0; j < n; j++)
                    row[j] = data[i][j];
                // transform it
                DFT(row, direction);
                // copy back
                for (int j = 0; j < n; j++)
                    data[i][j] = row[j];
            }

            // process columns
            Complex[] col = new Complex[n];

            for (int j = 0; j < n; j++)
            {
                // copy column
                for (int i = 0; i < n; i++)
                    col[i] = data[i][j];
                // transform it
                DFT(col, direction);
                // copy back
                for (int i = 0; i < n; i++)
                    data[i][j] = col[i];
            }
        }

        /**
         * 1-D Fast Fourier Transform.
         * @param data Data to transform.
         * @param direction Transformation direction.
         */
        public static void FFT(Complex[] data, Direction direction)
        {
            double[] real = Real(data);
            double[] img = Imaginary(data);
            if (direction == Direction.Forward)
                FFT(real, img);
            else
                FFT(img, real);
            if (direction == Direction.Forward)
            {
                for (int i = 0; i < real.GetLength(0); i++)
                {
                    data[i] = new Complex(real[i], img[i]);
                }
            }
            else
            {
                int n = real.GetLength(0);
                for (int i = 0; i < n; i++)
                {
                    data[i] = new Complex(real[i] / n, img[i] / n);
                }
            }
        }

        /**
         * 2-D Fast Fourier Transform.
         * @param data Data to transform.
         * @param direction Transformation direction.
         */
        public static void FFT2(Complex[][] data, Direction direction)
        {
            int n = data.GetLength(0);
            int m = data.GetLength(1);
            Complex[] row = new Complex[System.Math.Max(m, n)];

            for (int i = 0; i < n; i++)
            {
                // copy row
                for (int j = 0; j < n; j++)
                    row[j] = data[i][j];
                // transform it
                FFT(row, direction);
                // copy back
                for (int j = 0; j < n; j++)
                    data[i][j] = row[j];
            }

            // process columns
            Complex[] col = new Complex[n];

            for (int j = 0; j < n; j++)
            {
                // copy column
                for (int i = 0; i < n; i++)
                    col[i] = data[i][j];
                // transform it
                FFT(col, direction);
                // copy back
                for (int i = 0; i < n; i++)
                    data[i][j] = col[i];
            }
        }

        /* 
         * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
         * The vector can have any length. This is a wrapper function.
         */
        private static void FFT(double[] real, double[] imag)
        {
            int n = real.GetLength(0);
            if (n == 0)
            {
                return;
            }
            else if ((n & (n - 1)) == 0)  // Is power of 2
                TransformRadix2(real, imag);
            else  // More complicated algorithm for arbitrary sizes
                TransformBluestein(real, imag);
        }


        /* 
         * Computes the inverse discrete Fourier transform (IDFT) of the given complex vector, storing the result back into the vector.
         * The vector can have any length. This is a wrapper function. This transform does not perform scaling, so the inverse is not a true inverse.
         */
        private static void inverseTransform(double[] real, double[] imag)
        {
            FFT(imag, real);
        }

        /* 
         * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
         * The vector's length must be a power of 2. Uses the Cooley-Tukey decimation-in-time radix-2 algorithm.
         */
        private static void TransformRadix2(double[] real, double[] imag)
        {
            int n = real.GetLength(0);
            int levels = (int)System.Math.Floor(System.Math.Log(n, 2));
            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");
            double[] cosTable = new double[n / 2];
            double[] sinTable = new double[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                cosTable[i] = System.Math.Cos(2 * System.Math.PI * i / n);
                sinTable[i] = System.Math.Sin(2 * System.Math.PI * i / n);
            }

            // Bit-reversed addressing permutation
            for (int i = 0; i < n; i++)
            {
                unchecked
                {
                    int j = (int)((uint)Reverse(i) >> (32 - levels));
                    if (j > i)
                    {
                        double temp = real[i];
                        real[i] = real[j];
                        real[j] = temp;
                        temp = imag[i];
                        imag[i] = imag[j];
                        imag[j] = temp;
                    }
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
                        double tpre = real[j + halfsize] * cosTable[k] + imag[j + halfsize] * sinTable[k];
                        double tpim = -real[j + halfsize] * sinTable[k] + imag[j + halfsize] * cosTable[k];
                        real[j + halfsize] = real[j] - tpre;
                        imag[j + halfsize] = imag[j] - tpim;
                        real[j] += tpre;
                        imag[j] += tpim;
                    }
                }

                // Prevent overflow in 'size *= 2'
                if (size == n)
                    break;
            }
        }

        /* 
         * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
         * The vector can have any length. This requires the convolution function, which in turn requires the radix-2 FFT function.
         * Uses Bluestein's chirp z-transform algorithm.
         */
        private static void TransformBluestein(double[] real, double[] imag)
        {
            int n = real.GetLength(0);
            int m = HighestOneBit(n * 2 + 1) << 1;

            // Trignometric tables
            double[] cosTable = new double[n];
            double[] sinTable = new double[n];
            for (int i = 0; i < n; i++)
            {
                int j = (int)((long)i * i % (n * 2));  // This is more accurate than j = i * i
                cosTable[i] = System.Math.Cos(System.Math.PI * j / n);
                sinTable[i] = System.Math.Sin(System.Math.PI * j / n);
            }

            // Temporary vectors and preprocessing
            double[] areal = new double[m];
            double[] aimag = new double[m];
            for (int i = 0; i < n; i++)
            {
                areal[i] = real[i] * cosTable[i] + imag[i] * sinTable[i];
                aimag[i] = -real[i] * sinTable[i] + imag[i] * cosTable[i];
            }
            double[] breal = new double[m];
            double[] bimag = new double[m];
            breal[0] = cosTable[0];
            bimag[0] = sinTable[0];
            for (int i = 1; i < n; i++)
            {
                breal[i] = breal[m - i] = cosTable[i];
                bimag[i] = bimag[m - i] = sinTable[i];
            }

            // Convolution
            double[] creal = new double[m];
            double[] cimag = new double[m];
            convolve(areal, aimag, breal, bimag, creal, cimag);

            // Postprocessing
            for (int i = 0; i < n; i++)
            {
                real[i] = creal[i] * cosTable[i] + cimag[i] * sinTable[i];
                imag[i] = -creal[i] * sinTable[i] + cimag[i] * cosTable[i];
            }
        }

        /* 
         * Computes the circular convolution of the given real vectors. Each vector's length must be the same.
         */
        private static void convolve(double[] x, double[] y, double[] result)
        {
            int n = x.GetLength(0);
            convolve(x, new double[n], y, new double[n], result, new double[n]);
        }

        /* 
         * Computes the circular convolution of the given complex vectors. Each vector's length must be the same.
         */
        private static void convolve(double[] xreal, double[] ximag, double[] yreal, double[] yimag, double[] outreal, double[] outimag)
        {

            int n = xreal.GetLength(0);

            FFT(xreal, ximag);
            FFT(yreal, yimag);
            for (int i = 0; i < n; i++)
            {
                double temp = xreal[i] * yreal[i] - ximag[i] * yimag[i];
                ximag[i] = ximag[i] * yreal[i] + xreal[i] * yimag[i];
                xreal[i] = temp;
            }
            inverseTransform(xreal, ximag);

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

        public static double[] Real(Complex[] data)
        {
            double[] real = new double[data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                real[i] = data[i].Real;
            }
            return real;
        }

        public static double[] Imaginary(Complex[] data)
        {
            double[] im = new double[data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                im[i] = data[i].Imaginary;
            }
            return im;
        }

    }

}