// Accord Wavelet Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Gregoire Pau, 2006
// gregoire.pau at ebi.ac.uk
// 
// Based on the original C implementation by Gregoire Pau,
//   redistributed with modifications under the LGPL with
//   authorization of original author.
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

namespace Accord.Math.Wavelets
{
    using System;

    /// <summary>
    ///   Cohen-Daubechies-Feauveau Wavelet Transform
    /// </summary>
    /// 
    public class CDF97 : IWavelet
    {

        // Constants as used by Gregoire P.
        const double alpha = -1.586134342;
        const double beta = -0.05298011854;
        const double gamma = 0.8829110762;
        const double delta = 0.4435068522;
        const double zeta = 1.149604398;

        private int levels;

        /// <summary>
        ///   Constructs a new Cohen-Daubechies-Feauveau Wavelet Transform.
        /// </summary>
        /// 
        /// <param name="levels">The number of iterations for the 2D transform.</param>
        /// 
        public CDF97(int levels)
        {
            this.levels = levels;
        }

        /// <summary>
        ///   1-D Forward Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Forward(double[] data)
        {
            FWT97(data);
        }

        /// <summary>
        ///   2-D Forward Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Forward(double[,] data)
        {
            FWT97(data, levels);
        }

        /// <summary>
        ///   1-D Backward (Inverse) Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Backward(double[] data)
        {
            IWT97(data);
        }

        /// <summary>
        ///   2-D Backward (Inverse) Discrete Wavelet Transform.
        /// </summary>
        public void Backward(double[,] data)
        {
            IWT97(data, levels);
        }



        /// <summary>
        ///   Forward biorthogonal 9/7 wavelet transform
        /// </summary>
        public static void FWT97(double[] x)
        {
            int n = x.Length;

            // Predict 1
            for (int i = 1; i < n - 2; i += 2)
                x[i] += alpha * (x[i - 1] + x[i + 1]);
            x[n - 1] += 2 * alpha * x[n - 2];

            // Update 1
            for (int i = 2; i < n; i += 2)
                x[i] += beta * (x[i - 1] + x[i + 1]);
            x[0] += 2 * beta * x[1];

            // Predict 2
            for (int i = 1; i < n - 2; i += 2)
                x[i] += gamma * (x[i - 1] + x[i + 1]);
            x[n - 1] += 2 * gamma * x[n - 2];

            // Update 2
            for (int i = 2; i < n; i += 2)
                x[i] += delta * (x[i - 1] + x[i + 1]);
            x[0] += 2.0 * delta * x[1];

            // Scale
            for (int i = 0; i < n; i++)
            {
                if ((i % 2) != 0)
                    x[i] *= (1 / zeta);
                else x[i] /= (1 / zeta);
            }

            // Pack
            var tempbank = new double[n];
            for (int i = 0; i < n; i++)
            {
                if ((i % 2) == 0)
                    tempbank[i / 2] = x[i];
                else tempbank[n / 2 + i / 2] = x[i];
            }

            for (int i = 0; i < n; i++)
                x[i] = tempbank[i];
        }

        /// <summary>
        ///   Inverse biorthogonal 9/7 wavelet transform
        /// </summary>
        /// 
        public static void IWT97(double[] x)
        {
            int n = x.Length;

            // Unpack
            var tempbank = new double[n];
            for (int i = 0; i < n / 2; i++)
            {
                tempbank[i * 2] = x[i];
                tempbank[i * 2 + 1] = x[i + n / 2];
            }

            for (int i = 0; i < n; i++)
                x[i] = tempbank[i];

            // Undo scale
            for (int i = 0; i < n; i++)
            {
                if ((i % 2) != 0)
                    x[i] *= zeta;
                else x[i] /= zeta;
            }

            // Undo update 2
            for (int i = 2; i < n; i += 2)
                x[i] -= delta * (x[i - 1] + x[i + 1]);
            x[0] -= 2.0 * delta * x[1];

            // Undo predict 2
            for (int i = 1; i < n - 2; i += 2)
                x[i] -= gamma * (x[i - 1] + x[i + 1]);
            x[n - 1] -= 2.0 * gamma * x[n - 2];

            // Undo update 1
            for (int i = 2; i < n; i += 2)
                x[i] -= beta * (x[i - 1] + x[i + 1]);
            x[0] -= 2.0 * beta * x[1];

            // Undo predict 1
            for (int i = 1; i < n - 2; i += 2)
                x[i] -= alpha * (x[i - 1] + x[i + 1]);
            x[n - 1] -= 2.0 * alpha * x[n - 2];

        }

        /// <summary>
        ///   Forward biorthogonal 9/7 2D wavelet transform
        /// </summary>
        /// 
        public static double[,] FWT97(double[,] data, int levels)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            for (int i = 0; i < levels; i++)
            {
                fwt2d(data, w, h);
                fwt2d(data, w, h);
                w >>= 1;
                h >>= 1;
            }

            return data;
        }

        /// <summary>
        ///   Inverse biorthogonal 9/7 2D wavelet transform
        /// </summary>
        /// 
        public static double[,] IWT97(double[,] data, int levels)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            for (int i = 0; i < levels - 1; i++)
            {
                h >>= 1;
                w >>= 1;
            }

            for (int i = 0; i < levels; i++)
            {
                data = iwt2d(data, w, h);
                data = iwt2d(data, w, h);
                h <<= 1;
                w <<= 1;
            }

            return data;
        }



        private static double[,] fwt2d(double[,] x, int width, int height)
        {
            for (int j = 0; j < width; j++)
            {
                // Predict 1
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] += alpha * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] += 2 * alpha * x[height - 2, j];

                // Update 1
                for (int i = 2; i < height; i += 2)
                    x[i, j] += beta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] += 2 * beta * x[1, j];

                // Predict 2
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] += gamma * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] += 2 * gamma * x[height - 2, j];

                // Update 2
                for (int i = 2; i < height; i += 2)
                    x[i, j] += delta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] += 2 * delta * x[1, j];
            }

            // Pack
            var tempbank = new double[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if ((i % 2) == 0)
                        tempbank[j, i / 2] = (1 / zeta) * x[i, j];
                    else
                        tempbank[j, i / 2 + height / 2] = (zeta / 2) * x[i, j];
                }
            }

            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++)
                    x[i, j] = tempbank[i, j];

            return x;
        }

        private static double[,] iwt2d(double[,] x, int width, int height)
        {
            // Unpack
            var tempbank = new double[width, height];
            for (int j = 0; j < width / 2; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    tempbank[j * 2, i] = zeta * x[i, j];
                    tempbank[j * 2 + 1, i] = (2 / zeta) * x[i, j + width / 2];
                }
            }

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    x[i, j] = tempbank[i, j];


            for (int j = 0; j < width; j++)
            {
                // Undo update 2
                for (int i = 2; i < height; i += 2)
                    x[i, j] -= delta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] -= 2 * delta * x[1, j];

                // Undo predict 2
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] -= gamma * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] -= 2 * gamma * x[height - 2, j];

                // Undo update 1
                for (int i = 2; i < height; i += 2)
                    x[i, j] -= beta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] -= 2 * beta * x[1, j];

                // Undo predict 1
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] -= alpha * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] -= 2 * alpha * x[height - 2, j];
            }

            return x;
        }

    }
}
