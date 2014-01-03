// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using AForge.Math;

    /// <summary>
    ///   Discrete Sine Transform
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, the discrete sine transform (DST) is a Fourier-related transform
    ///   similar to the discrete Fourier transform (DFT), but using a purely real matrix. It
    ///   is equivalent to the imaginary parts of a DFT of roughly twice the length, operating
    ///   on real data with odd symmetry (since the Fourier transform of a real and odd function
    ///   is imaginary and odd), where in some variants the input and/or output data are shifted
    ///   by half a sample.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors, "Discrete sine transform," Wikipedia, The Free Encyclopedia,
    ///       available at: http://en.wikipedia.org/w/index.php?title=Discrete_sine_transform </description></item>
    ///     <item><description>
    ///       K. R. Castleman, Digital Image Processing. Chapter 13, p.288.
    ///       Prentice. Hall, 1998.</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public static class SineTransform
    {

        /// <summary>
        ///   Forward Discrete Sine Transform.
        /// </summary>
        /// 
        public static void DST(double[] data)
        {
            double[] result = new double[data.Length];

            for (int k = 1; k < result.Length + 1; k++)
            {
                double sum = 0;
                for (int i = 1; i < data.Length + 1; i++)
                    sum += data[i - 1] * Math.Sin(Math.PI * ((k * i) / (data.Length + 1.0)));
                result[k - 1] = sum;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = result[i];
        }

        /// <summary>
        ///   Inverse Discrete Sine Transform.
        /// </summary>
        /// 
        public static void IDST(double[] data)
        {
            double[] result = new double[data.Length];

            double inverse = 2.0 / (data.Length + 1);

            for (int k = 1; k < result.Length + 1; k++)
            {
                double sum = 0;
                for (int i = 1; i < data.Length + 1; i++)
                    sum += data[i - 1] * Math.Sin(Math.PI * ((k * i) / (data.Length + 1.0)));
                result[k - 1] = sum * inverse;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = result[i];
        }

        /// <summary>
        ///   Forward Discrete Sine Transform.
        /// </summary>
        /// 
        public static void DST(double[][] data)
        {
            int rows = data.Length;
            int cols = data[0].Length;

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                    row[j] = data[i][j];

                DST(row);

                for (int j = 0; j < row.Length; j++)
                    data[i][j] = row[j];
            }

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < col.Length; i++)
                    col[i] = data[i][j];

                DST(col);

                for (int i = 0; i < col.Length; i++)
                    data[i][j] = col[i];
            }
        }

        /// <summary>
        ///   Inverse Discrete Sine Transform.
        /// </summary>
        /// 
        public static void IDST(double[][] data)
        {
            int rows = data.Length;
            int cols = data[0].Length;

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < row.Length; i++)
                    col[i] = data[i][j];

                IDST(col);

                for (int i = 0; i < col.Length; i++)
                    data[i][j] = col[i];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                    row[j] = data[i][j];

                IDST(row);

                for (int j = 0; j < row.Length; j++)
                    data[i][j] = row[j];
            }
        }
    }
}
