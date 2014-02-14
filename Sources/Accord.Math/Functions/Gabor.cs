// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using AForge.Math;

    /// <summary>
    ///   Gabor kernel types.
    /// </summary>
    /// 
    public enum GaborKernelKind
    {

        /// <summary>
        ///   Creates kernel based on the real part of the Gabor function.
        /// </summary>
        /// 
        Real,

        /// <summary>
        ///   Creates a kernel based on the imaginary part of the Gabor function.
        /// </summary>
        /// 
        Imaginary,

        /// <summary>
        ///   Creates a kernel based on the Magnitude of the Gabor function.
        /// </summary>
        /// 
        Magnitude,

        /// <summary>
        ///   Creates a kernel based on the Squared Magnitude of the Gabor function.
        /// </summary>
        /// 
        SquaredMagnitude
    };

    /// <summary>
    ///   Gabor functions.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class has been contributed by Diego Catalano, author of the Catalano 
    ///   Framework, a native port of AForge.NET and Accord.NET for Java and Android.
    /// </remarks>
    /// 
    public static class Gabor
    {

        /// <summary>
        ///   1-D Gabor function.
        /// </summary>
        /// 
        public static double Function1D(double x, double mean, double amplitude,
            double position, double width, double phase, double frequency)
        {
            double a = (x - position) * (x - position);
            double b = (2 * width) * (2 * width);

            double envelope = mean + amplitude * Math.Exp(-a / b);
            double carry = Math.Cos(2 * Math.PI * frequency * (x - position) + phase);

            return envelope * carry;
        }

        /// <summary>
        ///   2-D Gabor function.
        /// </summary>
        /// 
        public static Complex Function2D(int x, int y, double lambda, double theta,
            double psi, double sigma, double gamma)
        {
            double X = +x * Math.Cos(theta) + y * Math.Sin(theta);
            double Y = -x * Math.Sin(theta) + y * Math.Cos(theta);

            double envelope = Math.Exp(-((X * X + gamma * gamma * Y * Y) / (2 * sigma * sigma)));
            double real = Math.Cos(2 * Math.PI * (X / lambda) + psi);
            double imaginary = Math.Sin(2 * Math.PI * (X / lambda) + psi);

            return new Complex(envelope * real, envelope * imaginary);
        }

        /// <summary>
        ///   Real part of the 2-D Gabor function.
        /// </summary>
        /// 
        public static double RealFunction2D(int x, int y, double lambda, double theta,
            double psi, double sigma, double gamma)
        {
            double X = +x * Math.Cos(theta) + y * Math.Sin(theta);
            double Y = -x * Math.Sin(theta) + y * Math.Cos(theta);

            double envelope = Math.Exp(-((X * X + gamma * gamma * Y * Y) / (2 * sigma * sigma)));
            double carrier = Math.Cos(2 * Math.PI * (X / lambda) + psi);

            return envelope * carrier;
        }

        /// <summary>
        ///   Imaginary part of the 2-D Gabor function.
        /// </summary>
        /// 
        public static double ImaginaryFunction2D(int x, int y, double lambda, double theta,
            double psi, double sigma, double gamma)
        {
            double X = +x * Math.Cos(theta) + y * Math.Sin(theta);
            double Y = -x * Math.Sin(theta) + y * Math.Cos(theta);

            double envelope = Math.Exp(-((X * X + gamma * gamma * Y * Y) / (2 * sigma * sigma)));
            double carrier = Math.Sin(2 * Math.PI * (X / lambda) + psi);

            return envelope * carrier;
        }

        /// <summary>
        ///   Computes the 2-D Gabor kernel.
        /// </summary>
        /// 
        public static double[,] Kernel2D(double lambda, double theta, double psi,
            double sigma, double gamma)
        {
            return Kernel2D(3, lambda, theta, psi, sigma, gamma, false, GaborKernelKind.Real);
        }

        /// <summary>
        ///   Computes the 2-D Gabor kernel.
        /// </summary>
        /// 
        public static double[,] Kernel2D(double lambda, double theta, double psi,
            double sigma, double gamma, bool normalized)
        {
            return Kernel2D(3, lambda, theta, psi, sigma, gamma, normalized, GaborKernelKind.Imaginary);
        }

        /// <summary>
        ///   Computes the 2-D Gabor kernel.
        /// </summary>
        /// 
        public static double[,] Kernel2D(int size, double lambda, double theta, double psi,
            double sigma, double gamma, bool normalized)
        {
            return Kernel2D(size, lambda, theta, psi, sigma,
                gamma, normalized, GaborKernelKind.Imaginary);
        }


        /// <summary>
        ///   Computes the 2-D Gabor kernel.
        /// </summary>
        /// 
        public static double[,] Kernel2D(int size, double lambda, double theta,
            double psi, double sigma, double gamma, bool normalized, GaborKernelKind function)
        {
            double sigmaX = sigma;
            double sigmaY = sigma / gamma;

            double a = Math.Max(
                Math.Abs(size * sigmaX * Math.Cos(theta)),
                Math.Abs(size * sigmaY * Math.Sin(theta)));
            int xMax = (int)Math.Ceiling(Math.Max(1, a));

            double b = Math.Max(
                Math.Abs(size * sigmaX * Math.Sin(theta)),
                Math.Abs(size * sigmaY * Math.Cos(theta)));
            int yMax = (int)Math.Ceiling(Math.Max(1, b));

            int[] xValues = Matrix.Vector(-xMax, xMax, increment: 1);
            int[] yValues = Matrix.Vector(-yMax, yMax, increment: 1);

            System.Diagnostics.Debug.Assert(xValues.Length == (2 * xMax + 1));
            System.Diagnostics.Debug.Assert(yValues.Length == (2 * yMax + 1));

            double[,] kernel = new double[xValues.Length, yValues.Length];

            double sum = 0;

            switch (function)
            {
                case GaborKernelKind.Real:
                    for (int i = 0; i < xValues.Length; i++)
                        for (int j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = Gabor.RealFunction2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma);
                    break;

                case GaborKernelKind.Imaginary:
                    for (int i = 0; i < xValues.Length; i++)
                        for (int j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = Gabor.ImaginaryFunction2D(
                                    xValues[i], yValues[j], lambda, theta, psi, sigma, gamma);
                    break;

                case GaborKernelKind.Magnitude:
                    for (int i = 0; i < xValues.Length; i++)
                        for (int j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = Gabor.Function2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma).Magnitude;
                    break;

                case GaborKernelKind.SquaredMagnitude:
                    for (int i = 0; i < xValues.Length; i++)
                        for (int j = 0; j < yValues.Length; j++)
                            sum += kernel[i, j] = Gabor.Function2D(
                                xValues[i], yValues[j], lambda, theta, psi, sigma, gamma).SquaredMagnitude;
                    break;
            }

            if (normalized)
                kernel.Divide(sum, inPlace: true);

            return kernel;
        }
    }
}
