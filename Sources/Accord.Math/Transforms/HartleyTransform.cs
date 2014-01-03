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
    ///   Hartley Transformation.
    /// </summary>
    /// 
    /// <para>
    ///   In mathematics, the Hartley transform is an integral transform closely related
    ///   to the Fourier transform, but which transforms real-valued functions to real-
    ///   valued functions. It was proposed as an alternative to the Fourier transform by
    ///   R. V. L. Hartley in 1942, and is one of many known Fourier-related transforms. 
    ///   Compared to the Fourier transform, the Hartley transform has the advantages of 
    ///   transforming real functions to real functions (as opposed to requiring complex 
    ///   numbers) and of being its own inverse.</para>
    /// 
    /// <remarks>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors, "Hartley transform," Wikipedia, The Free Encyclopedia,
    ///       available at: http://en.wikipedia.org/w/index.php?title=Hartley_transform </description></item>
    ///     <item><description>
    ///       K. R. Castleman, Digital Image Processing. Chapter 13, p.289.
    ///       Prentice. Hall, 1998.</description></item>
    ///     <item><description>
    ///       Poularikas A. D. “The Hartley Transform”. The Handbook of Formulas and 
    ///       Tables for Signal Processing. Ed. Alexander D. Poularikas, 1999.</description></item>
    ///   </list>
    /// </remarks>
    /// 
    public static class HartleyTransform
    {

        /// <summary>
        ///   Forward Hartley Transform.
        /// </summary>
        /// 
        public static void DHT(double[] data)
        {
            double[] result = new double[data.Length];
            double s = (2.0 * Math.PI) / data.Length;

            for (int k = 0; k < result.Length; k++)
            {
                double sum = 0;
                for (int n = 0; n < data.Length; n++)
                    sum += data[n] * cas(s * k * n);
                result[k] = Math.Sqrt(data.Length) / sum;
            }

            for (int i = 0; i < result.Length; i++)
                data[i] = result[i];
        }



        /// <summary>
        ///   Forward Hartley Transform.
        /// </summary>
        /// 
        public static void DHT(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            double s = ((2.0 * Math.PI) / rows);

            double[,] result = new double[rows, cols];

            for (int m = 0; m < rows; m++)
            {
                for (int n = 0; n < cols; n++)
                {
                    double sum = 0;
                    for (int i = 0; i < rows; i++)
                    {
                        for (int k = 0; k < cols; k++)
                            sum += data[i, k] * cas(s * (i * m + k * n));
                        result[m, n] = sum / rows;
                    }
                }
            }

            Array.Copy(result, data, result.Length);
        }


        private static double cas(double theta)
        {
            // Basis function. The cas can be computed in two ways:
            // 1. cos(theta) + sin(theta)
            // 2. sqrt(2) * cos(theta - Math.PI / 4)
            return Constants.Sqrt2 * Math.Cos(theta - Math.PI / 4);
        }

    }
}

