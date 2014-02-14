// Accord Wavelet Library
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

namespace Accord.Math.Wavelets
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Haar Wavelet Transform.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Musawir Ali, An Introduction to Wavelets and the Haar Transform.
    ///       Available on: http://www.cs.ucf.edu/~mali/haar/ </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    public class Haar : IWavelet
    {
        private const double SQRT2 = Accord.Math.Constants.Sqrt2;

        /*
        private const double w0 = 1.0 / SQRT2;
        private const double w1 = -1.0 / SQRT2;
        private const double s0 = 1.0 / SQRT2;
        private const double s1 = 1.0 / SQRT2;
        //*/

        /*
        private const double w0 = 1.0;
        private const double w1 = -1.0;
        private const double s0 = 1.0;
        private const double s1 = 1.0;
        //*/

        //*
        private const double w0 = 0.5;
        private const double w1 = -0.5;
        private const double s0 = 0.5;
        private const double s1 = 0.5;
        //*/

        private int levels;

        /// <summary>
        ///   Constructs a new Haar Wavelet Transform.
        /// </summary>
        /// <param name="levels">The number of iterations for the 2D transform.</param>
        /// 
        public Haar(int levels)
        {
            this.levels = levels;
        }

        /// <summary>
        ///   1-D Forward Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Forward(double[] data)
        {
            FWT(data);
        }

        /// <summary>
        ///   1-D Backward (Inverse) Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Backward(double[] data)
        {
            IWT(data);
        }

        /// <summary>
        ///   2-D Forward Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Forward(double[,] data)
        {
            FWT(data, levels);
        }

        /// <summary>
        ///   2-D Backward (Inverse) Discrete Wavelet Transform.
        /// </summary>
        /// 
        public void Backward(double[,] data)
        {
            IWT(data, levels);
        }




        /// <summary>
        ///   Discrete Haar Wavelet Transform
        /// </summary>
        /// 
        public static void FWT(double[] data)
        {
            double[] temp = new double[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                temp[i] = data[k] * s0 + data[k + 1] * s1;
                temp[i + h] = data[k] * w0 + data[k + 1] * w1;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }

        /// <summary>
        ///   Inverse Haar Wavelet Transform
        /// </summary>
        /// 
        public static void IWT(double[] data)
        {
            double[] temp = new double[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                temp[k] = (data[i] * s0 + data[i + h] * w0) / w0;
                temp[k + 1] = (data[i] * s1 + data[i + h] * w1) / s0;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }


        /// <summary>
        ///   Discrete Haar Wavelet 2D Transform
        /// </summary>
        /// 
        public static void FWT(double[,] data, int iterations)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int k = 0; k < iterations; k++)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < row.Length; j++)
                        row[j] = data[i, j];

                    FWT(row);

                    for (int j = 0; j < row.Length; j++)
                        data[i, j] = row[j];
                }

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i, j];

                    FWT(col);

                    for (int i = 0; i < col.Length; i++)
                        data[i, j] = col[i];
                }
            }
        }

        /// <summary>
        ///   Inverse Haar Wavelet 2D Transform
        /// </summary>
        /// 
        public static void IWT(double[,] data, int iterations)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] col = new double[rows];
            double[] row = new double[cols];

            for (int l = 0; l < iterations; l++)
            {
                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < row.Length; i++)
                        col[i] = data[i, j];

                    IWT(col);

                    for (int i = 0; i < col.Length; i++)
                        data[i, j] = col[i];
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < row.Length; j++)
                        row[j] = data[i, j];

                    IWT(row);

                    for (int j = 0; j < row.Length; j++)
                        data[i, j] = row[j];
                }
            }
        }


    }
}
