// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Philipp Berens, 2011
// berens at tuebingen.mpg.de
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
// This file contains implementations based on the original code by Philipp
// Berens, shared under a 2-clause BSD license. The original license text 
// is reproduced below. 
//
// Copyright © Philipp Berens, 2011
// All rights reserved.
//
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are
//    met:
//
//        * Redistributions of source code must retain the above copyright
//          notice, this list of conditions and the following disclaimer.
//        * Redistributions in binary form must reproduce the above copyright
//          notice, this list of conditions and the following disclaimer in
//          the documentation and/or other materials provided with the distribution
//    
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//   ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//   LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//   CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
//   SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//   INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//   POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Statistics
{
    using System;
    using Accord.Math;
    using Accord.Compat;
    using System.Numerics;

    /// <summary>
    ///   Set of statistics functions operating over a circular space.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents collection of common functions used in
    ///   statistics. The values are handled as belonging to a distribution
    ///   defined over a circle, such as the <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution"/>.
    /// </remarks>
    /// 
    public static class Circular
    {

        /// <summary>
        ///   Transforms circular data into angles (normalizes the data to be between -PI and PI).
        /// </summary>
        /// 
        /// <param name="samples">The samples to be transformed.</param>
        /// <param name="length">The maximum possible sample value (such as 24 for hour data).</param>
        /// <param name="inPlace">Whether to perform the transformation in place.</param>
        /// 
        /// <returns>A double array containing the same data in <paramref name="samples"/>,
        ///   but normalized between -PI and PI.</returns>
        /// 
        public static double[] ToRadians(this double[] samples, double length, bool inPlace = false)
        {
            double[] result = inPlace ? samples : new double[samples.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = samples[i] * ((2 * Math.PI) / length) - Math.PI;

            return result;
        }

        /// <summary>
        ///   Transforms circular data into angles (normalizes the data to be between -PI and PI).
        /// </summary>
        /// 
        /// <param name="sample">The sample to be transformed.</param>
        /// <param name="length">The maximum possible sample value (such as 24 for hour data).</param>
        /// 
        /// <returns>The <paramref name="sample"/> normalized to be between -PI and PI.</returns>
        /// 
        public static double ToRadians(this double sample, double length)
        {
            double m = Accord.Math.Tools.Mod(sample, length);
            return (m / length) * (2.0 * Math.PI) - Math.PI;
        }

        /// <summary>
        ///   Transforms angular data back into circular data (reverts the
        ///   <see cref="ToRadians(double[], double, bool)" /> transformation.
        /// </summary>
        /// 
        /// <param name="angle">The angle to be reconverted into the original unit.</param>
        /// <param name="length">The maximum possible sample value (such as 24 for hour data).</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// 
        /// <returns>The original before being converted.</returns>
        /// 
        public static double ToCircular(this double angle, double length, bool wrap = true)
        {
            double m = ((angle + Math.PI) / (2 * Math.PI)) * length;

            if (wrap)
                m = Accord.Math.Tools.Mod(m, length);

            return m;
        }

        #region Array Measures

        /// <summary>
        ///   Computes the sum of cosines and sines for the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <param name="cos">The sum of cosines, returned as an out parameter.</param>
        /// <param name="sin">The sum of sines, returned as an out parameter.</param>
        /// 
        public static void Sum(double[] angles, out double cos, out double sin)
        {
            cos = 0;
            sin = 0;

            for (int i = 0; i < angles.Length; i++)
            {
                cos += Math.Cos(angles[i]);
                sin += Math.Sin(angles[i]);
            }
        }

        /// <summary>
        ///   Computes the Mean direction of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The mean direction of the given angles.</returns>
        /// 
        public static double Mean(double[] angles)
        {
            double cos, sin;
            Sum(angles, out cos, out sin);
            return Mean(angles.Length, cos, sin);
        }

        /// <summary>
        ///   Computes the circular Mean direction of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The circular Mean direction of the given samples.</returns>
        /// 
        public static double Mean(double[] samples, double length)
        {
            return ToCircular(Mean(ToRadians(samples, length)), length);
        }

        /// <summary>
        ///   Computes the Mean direction of the given angles.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// <param name="cos">The sum of the cosines of the samples.</param>
        /// <param name="sin">The sum of the sines of the samples.</param>
        /// 
        /// <returns>The mean direction of the given angles.</returns>
        /// 
        public static double Mean(int samples, double cos, double sin)
        {
            return Math.Atan2(sin / samples, cos / samples);
        }

        /// <summary>
        ///   Computes the mean resultant vector length (r) of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The mean resultant vector length of the given angles.</returns>
        /// 
        public static double Resultant(double[] angles)
        {
            double cos, sin;
            Sum(angles, out cos, out sin);
            return Resultant(angles.Length, cos, sin);
        }

        /// <summary>
        ///   Computes the resultant vector length (r) of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The mean resultant vector length of the given samples.</returns>
        /// 
        public static double Resultant(double[] samples, double length)
        {
            return ToCircular(Resultant(ToRadians(samples, length)), length);
        }

        /// <summary>
        ///   Computes the mean resultant vector length (r) of the given angles.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// <param name="cos">The sum of the cosines of the samples.</param>
        /// <param name="sin">The sum of the sines of the samples.</param>
        /// 
        /// <returns>The mean resultant vector length of the given angles.</returns>
        /// 
        public static double Resultant(int samples, double cos, double sin)
        {
            double x = sin / samples;
            double y = cos / samples;
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        ///   Computes the circular variance of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The circular variance of the given samples.</returns>
        /// 
        public static double Variance(double[] samples, double length)
        {
            double scale = length / (2 * Math.PI);
            return Variance(ToRadians(samples, length)) * scale * scale;
        }

        /// <summary>
        ///   Computes the Variance of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The variance of the given angles.</returns>
        /// 
        public static double Variance(double[] angles)
        {
            double cos, sin;
            Sum(angles, out cos, out sin);
            return Variance(angles.Length, cos, sin);
        }

        /// <summary>
        ///   Computes the Variance of the given angles.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// <param name="cos">The sum of the cosines of the samples.</param>
        /// <param name="sin">The sum of the sines of the samples.</param>
        /// 
        /// <returns>The variance of the angles.</returns>
        /// 
        public static double Variance(int samples, double cos, double sin)
        {
            double rho = Math.Sqrt(sin * sin + cos * cos);
            double var = 1.0 - rho / samples;
            return var;
        }

        /// <summary>
        ///   Computes the circular standard deviation of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The circular standard deviation of the given samples.</returns>
        /// 
        public static double StandardDeviation(double[] samples, double length)
        {
            double scale = length / (2 * Math.PI);
            return StandardDeviation(ToRadians(samples, length)) * scale;
        }

        /// <summary>
        ///   Computes the Standard Deviation of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The standard deviation of the given angles.</returns>
        /// 
        public static double StandardDeviation(double[] angles)
        {
            double cos, sin;
            Sum(angles, out cos, out sin);
            return StandardDeviation(angles.Length, cos, sin);
        }


        /// <summary>
        ///   Computes the Standard Deviation of the given angles.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// <param name="cos">The sum of the cosines of the samples.</param>
        /// <param name="sin">The sum of the sines of the samples.</param>
        /// 
        /// <returns>The standard deviation of the angles.</returns>
        /// 
        public static double StandardDeviation(int samples, double cos, double sin)
        {
            double rho = Math.Sqrt(sin * sin + cos * cos);
            return Math.Sqrt(-2.0 * Math.Log(rho / samples));
        }

        /// <summary>
        ///   Computes the circular angular deviation of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The circular angular deviation of the given samples.</returns>
        /// 
        public static double AngularDeviation(double[] samples, double length)
        {
            double scale = length / (2 * Math.PI);
            return AngularDeviation(ToRadians(samples, length)) * scale;
        }

        /// <summary>
        ///   Computes the Angular Deviation of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The angular deviation of the given angles.</returns>
        /// 
        public static double AngularDeviation(double[] angles)
        {
            double cos, sin;
            Sum(angles, out cos, out sin);
            return AngularDeviation(angles.Length, cos, sin);
        }

        /// <summary>
        ///   Computes the Angular Deviation of the given angles.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// <param name="cos">The sum of the cosines of the samples.</param>
        /// <param name="sin">The sum of the sines of the samples.</param>
        /// 
        /// <returns>The angular deviation of the angles.</returns>
        /// 
        public static double AngularDeviation(int samples, double cos, double sin)
        {
            double rho = Math.Sqrt(sin * sin + cos * cos);
            double var = 1.0 - rho / samples;
            return Math.Sqrt(2 * var);
        }

        /// <summary>
        ///   Computes the circular standard error of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// <param name="alpha">The confidence level. Default is 0.05.</param>
        /// 
        /// <returns>The circular standard error of the given samples.</returns>
        /// 
        public static double StandardError(double[] samples, double length, double alpha)
        {
            double scale = length / (2 * Math.PI);
            return StandardError(ToRadians(samples, length), alpha) * scale;
        }

        /// <summary>
        ///   Computes the standard error of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="alpha">The confidence level. Default is 0.05.</param>
        /// 
        /// <returns>The standard error of the given angles.</returns>
        /// 
        public static double StandardError(double[] angles, double alpha)
        {
            double cos, sin;
            Sum(angles, out cos, out sin);
            return StandardError(angles.Length, cos, sin, alpha);
        }

        /// <summary>
        ///   Computes the standard error of the given angles.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// <param name="cos">The sum of the cosines of the samples.</param>
        /// <param name="sin">The sum of the sines of the samples.</param>
        /// <param name="alpha">The confidence level. Default is 0.05.</param>
        /// 
        /// <returns>The standard error of the angles.</returns>
        /// 
        public static double StandardError(int samples, double cos, double sin, double alpha)
        {
            int n = samples;

            double x = sin / n;
            double y = cos / n;
            double r = Math.Sqrt(x * x + y * y);

            double R = n * r;

            // double c2 = ChiSquareDistribution.Inverse(1 - alpha, 1);
            double c2 = 2 * Gamma.InverseUpperIncomplete(0.5, alpha);

            // check for resultant vector length and select appropriate formula
            double t = 0;

            if (r < 0.9 && r > Math.Sqrt((c2 / 2) / n))
                t = Math.Sqrt((2 * n * (2 * R * R - n * c2)) / (4 * n - c2));  // equ. 26.24
            else if (r >= 0.9)
                t = Math.Sqrt(n * n - (n * n - R * R) * Math.Exp(c2 / n));     // equ. 26.25
            else
                t = Double.NaN;

            // apply final transform
            t = Math.Acos(t / R);

            return t;
        }

        /// <summary>
        ///   Computes the angular distance between two angles.
        /// </summary>
        /// 
        /// <param name="x">The first angle.</param>
        /// <param name="y">The second angle.</param>
        /// 
        /// <returns>The distance between the two angles.</returns>
        /// 
        public static double Distance(double x, double y)
        {
            return Distance(Math.Cos(x), Math.Sin(x), Math.Cos(y), Math.Sin(y));
        }

        /// <summary>
        ///   Computes the distance between two circular samples.
        /// </summary>
        /// 
        /// <param name="x">The first sample.</param>
        /// <param name="y">The second sample.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The distance between the two angles.</returns>
        /// 
        public static double Distance(double x, double y, double length)
        {
            double ax = ToRadians(x, length);
            double ay = ToRadians(y, length);
            double dxy = Distance(ax, ay);
            return (dxy * length) / (2 * Math.PI);
        }

        /// <summary>
        ///   Computes the angular distance between two angles.
        /// </summary>
        /// 
        /// <param name="cosx">The cosine of the first sample.</param>
        /// <param name="sinx">The sin of the first sample.</param>
        /// <param name="cosy">The cosine of the second sample.</param>
        /// <param name="siny">The sin of the second sample.</param>
        /// 
        /// <returns>The distance between the two angles.</returns>
        /// 
        public static double Distance(double cosx, double sinx, double cosy, double siny)
        {
            double den = (cosy * cosy + siny * siny);
            double e = (cosx * cosy + sinx * siny) / den;
            double f = (sinx * cosy - cosx * siny) / den;

            return Math.Atan2(f, e);
        }

        /// <summary>
        ///   Computes the circular Median of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// 
        /// <returns>The circular Median of the given samples.</returns>
        /// 
        public static double Median(double[] samples, double length)
        {
            return ToCircular(Median(ToRadians(samples, length)), length);
        }

        /// <summary>
        ///   Computes the circular Median direction of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The circular Median of the given angles.</returns>
        /// 
        public static double Median(double[] angles)
        {
            double[] dm = new double[angles.Length];
            for (int i = 0; i < angles.Length; i++)
            {
                double cosx = Math.Cos(angles[i]);
                double sinx = Math.Sin(angles[i]);

                for (int j = i + 1; j < angles.Length; j++)
                {
                    double cosy = Math.Cos(angles[j]);
                    double siny = Math.Sin(angles[j]);

                    double den = (cosy * cosy + siny * siny);
                    double e = (cosx * cosy + sinx * siny) / den;
                    double f = (sinx * cosy - cosx * siny) / den;
                    double d = Math.Atan2(f, e);

                    if (d > 0)
                    {
                        dm[j] += 1;
                        dm[i] -= 1;
                    }
                    else if (d < 0)
                    {
                        dm[j] -= 1;
                        dm[i] += 1;
                    }
                }
            }

            for (int i = 0; i < dm.Length; i++)
                dm[i] = Math.Abs(dm[i]);


            int imin = 0;
            double dmin = dm[0];
            for (int i = 1; i < dm.Length; i++)
            {
                if (dm[i] < dmin)
                {
                    dmin = dm[i];
                    imin = i;
                }
            }


            double md;

            if (dm.Length % 2 != 0)
            {
                // is odd
                md = angles[imin];
            }
            else
            {
                // is even
                int count = 0;
                double cos = 0, sin = 0;
                for (int i = 0; i < dm.Length; i++)
                {
                    if (dm[i] == dmin)
                    {
                        cos += Math.Cos(angles[i]);
                        sin += Math.Sin(angles[i]);
                        count++;
                    }
                }

                md = Math.Atan2(sin / count, cos / count);
            }

            double mean = Mean(angles);

            double d1 = Distance(mean, md);
            double d2 = Distance(mean, md + Math.PI);

            if (Math.Abs(d1) > Math.Abs(d2))
                md = Accord.Math.Tools.Mod(md + Math.PI, 2 * Math.PI);

            return md;
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// <param name="q1">The first quartile, as an out parameter.</param>
        /// <param name="q3">The third quartile, as an out parameter.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given samples.</returns>
        /// 
        public static double Quartiles(double[] samples, double length, out double q1, out double q3, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            double q2 = Quartiles(ToRadians(samples, length), out q1, out q3, wrap, type: type);
            q1 = ToCircular(q1, length, wrap);
            q3 = ToCircular(q3, length, wrap);
            return ToCircular(q2, length);
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// <param name="q1">The first quartile, as an out parameter.</param>
        /// <param name="q3">The third quartile, as an out parameter.</param>
        /// <param name="median">The median value of the <paramref name="samples"/>, if already known.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given samples.</returns>
        /// 
        public static double Quartiles(double[] samples, double length, out double q1, out double q3, double median, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            double angleMedian = Circular.ToRadians(median, length);
            double q2 = Quartiles(ToRadians(samples, length), out q1, out q3, angleMedian, wrap, type: type);
            q1 = ToCircular(q1, length, wrap);
            q3 = ToCircular(q3, length, wrap);
            return ToCircular(q2, length);
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="q1">The first quartile, as an out parameter.</param>
        /// <param name="q3">The third quartile, as an out parameter.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given angles.</returns>
        /// 
        public static double Quartiles(double[] angles, out double q1, out double q3, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            return Quartiles(angles, out q1, out q3, Median(angles), wrap, type: type);
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// <param name="range">The sample quartiles, as an out parameter.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given samples.</returns>
        /// 
        public static double Quartiles(double[] samples, double length, out DoubleRange range, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            double q2 = Quartiles(ToRadians(samples, length), out range, wrap, type: type);
            range.Min = ToCircular(range.Min, length, wrap);
            range.Max = ToCircular(range.Max, length, wrap);
            return ToCircular(q2, length);
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular samples.
        ///   The minimum possible value for a sample must be zero and the maximum must
        ///   be indicated in the parameter <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="samples">A double array containing the circular samples.</param>
        /// <param name="length">The maximum possible value of the samples.</param>
        /// <param name="range">The sample quartiles, as an out parameter.</param>
        /// <param name="median">The median value of the <paramref name="samples"/>, if already known.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given samples.</returns>
        /// 
        public static double Quartiles(double[] samples, double length, out DoubleRange range, double median, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            double angleMedian = Circular.ToRadians(median, length);
            double q2 = Quartiles(ToRadians(samples, length), out range, angleMedian, wrap, type: type);
            range.Min = ToCircular(range.Min, length, wrap);
            range.Max = ToCircular(range.Max, length, wrap);
            return ToCircular(q2, length);
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="range">The sample quartiles, as an out parameter.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given angles.</returns>
        /// 
        public static double Quartiles(double[] angles, out DoubleRange range, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            return Quartiles(angles, out range, Median(angles), wrap, type: type);
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="range">The sample quartiles, as an out parameter.</param>
        /// <param name="median">The angular median, if already known.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given angles.</returns>
        /// 
        public static double Quartiles(double[] angles, out DoubleRange range, double median, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            double q1, q3;
            double q2 = Quartiles(angles, out q1, out q3, median, wrap, type: type);

            Accord.Diagnostics.Debug.Assert(q2 == median);

            range = new DoubleRange(q1, q3);
            return median;
        }

        /// <summary>
        ///   Computes the circular quartiles of the given circular angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="q1">The first quartile, as an out parameter.</param>
        /// <param name="q3">The third quartile, as an out parameter.</param>
        /// <param name="median">The angular median, if already known.</param>
        /// <param name="wrap">
        ///   Whether range values should be wrapped to be contained in the circle. If 
        ///   set to false, range values could be returned outside the [+pi;-pi] range.
        /// </param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// 
        /// <returns>The median of the given angles.</returns>
        /// 
        public static double Quartiles(double[] angles, out double q1, out double q3, double median, bool wrap = true, QuantileMethod type = QuantileMethod.Default)
        {
            double[] x = new double[angles.Length];
            for (int i = 0; i < angles.Length; i++)
                x[i] = Accord.Math.Tools.Mod(angles[i] - median, 2 * Math.PI);

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = (x[i] < -Math.PI) ? (x[i] + (2 * Math.PI)) : (x[i]);
                x[i] = (x[i] > +Math.PI) ? (x[i] - (2 * Math.PI)) : (x[i]);
            }

            x.Quartiles(out q1, out q3, alreadySorted: false, type: type, inPlace: true);

            q1 = q1 + median;
            q3 = q3 + median;

            if (wrap)
            {
                q1 = Accord.Math.Tools.Mod(q1, 2 * Math.PI);
                q3 = Accord.Math.Tools.Mod(q3, 2 * Math.PI);
            }

            return median;
        }


        /// <summary>
        ///   Computes the concentration (kappa) of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <returns>
        ///   The concentration (kappa) parameter of the <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution"/>
        ///   for the given data.
        /// </returns>
        /// 
        public static double Concentration(double[] angles)
        {
            return Concentration(angles, Mean(angles));
        }

        /// <summary>
        ///   Computes the concentration (kappa) of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="mean">The mean of the angles, if already known.</param>
        /// <returns>
        ///   The concentration (kappa) parameter of the <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution"/>
        ///   for the given data.
        /// </returns>
        /// 
        public static double Concentration(double[] angles, double mean)
        {
            double cos = 0;
            for (int i = 0; i < angles.Length; i++)
                cos += Math.Cos(angles[i] - mean);

            return estimateKappa(cos / angles.Length);
        }
        #endregion

        /// <summary>
        ///   Computes the Weighted Mean of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="weights">An unit vector containing the importance of each angle
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double WeightedMean(double[] angles, double[] weights)
        {
            double cos = 0, sin = 0;
            for (int i = 0; i < angles.Length; i++)
            {
                cos += Math.Cos(angles[i]) * weights[i];
                sin += Math.Sin(angles[i]) * weights[i];
            }

            return Math.Atan2(sin, cos);
        }

        /// <summary>
        ///   Computes the Weighted Concentration of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="weights">An unit vector containing the importance of each angle
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double WeightedConcentration(double[] angles, double[] weights)
        {
            return WeightedConcentration(angles, weights, WeightedMean(angles, weights));
        }

        /// <summary>
        ///   Computes the Weighted Concentration of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="weights">An unit vector containing the importance of each angle
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="mean">The mean of the angles, if already known.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double WeightedConcentration(double[] angles, double[] weights, double mean)
        {
            double cos = 0;

            for (int i = 0; i < angles.Length; i++)
                cos += Math.Cos(angles[i] - mean) * weights[i];

            return estimateKappa(cos);
        }

        /// <summary>
        ///   Computes the maximum likelihood estimate
        ///   of kappa given by Best and Fisher (1981).
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This method implements the approximation to the Maximum Likelihood
        ///   Estimative of the kappa concentration parameter as suggested by Best
        ///   and Fisher (1981), cited by Zheng Sun (2006) and Hussin and Mohamed
        ///   (2008). Other useful approximations are given by Suvrit Sra (2009).</para>
        ///   
        /// <para>    
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       A.G. Hussin and I.B. Mohamed, 2008. Efficient Approximation for the von Mises Concentration Parameter.
        ///       Asian Journal of Mathematics &amp; Statistics, 1: 165-169. </description></item>
        ///     <item><description><a href="http://www.kyb.mpg.de/publications/attachments/vmfnote_7045%5B0%5D.pdf">
        ///       Suvrit Sra, "A short note on parameter approximation for von Mises-Fisher distributions:
        ///       and a fast implementation of $I_s(x)$". (revision of Apr. 2009). Computational Statistics (2011).
        ///       Available on: http://www.kyb.mpg.de/publications/attachments/vmfnote_7045%5B0%5D.pdf </a></description></item>
        ///     <item><description>
        ///       Zheng Sun. M.Sc. Comparing measures of fit for circular distributions. Master thesis, 2006.
        ///       Available on: https://dspace.library.uvic.ca:8443/bitstream/handle/1828/2698/zhengsun_master_thesis.pdf </description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        private static double estimateKappa(double r)
        {
            // Best and Fisher (1981) gave a simple
            //   approximation to the MLE of kappa:

            double r3 = r * r * r;

            if (r < 0.53)
            {
                double r5 = r3 * r * r;
                return (2.0 * r) + (r3) + (5.0 * r5 / 6.0);
            }

            if (r < 0.85)
            {
                return -0.4 + 1.39 * r + 0.43 / (1.0 - r);
            }

            return 1.0 / (r3 - 4 * r * r + 3 * r);

            // However, Sun (2006) mentions this estimate of k
            // is not reliable when r is small, such as when r < 0.7.
        }

        /// <summary>
        ///   Computes the circular skewness of the given circular angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The circular skewness for the given <paramref name="angles"/>.</returns>
        /// 
        public static double Skewness(double[] angles)
        {
            // compute necessary values
            double theta = Circular.Mean(angles);

            Complex m = CentralMoments(angles, 2);
            double rho2 = m.Magnitude;
            double mu2 = m.Phase;

            // compute skewness 
            double b = 0; // Pewsey, Metrika, 2004
            for (int i = 0; i < angles.Length; i++)
                b += Math.Sin(2 * Distance(angles[i], theta));
            b /= angles.Length;

            /*
            // alternative skewness measure from Fisher
            // Statistical Analysis of Circular Data, p. 34
            double b0 = 0; // (formula 2.29)
            double omR = Math.Pow(1 - R, 3 / 2.0);

            for (int i = 0; i < angles.Length; i++)
                b0 += rho2 * Math.Sin(Distance(mu2, 2 * theta)) / omR;
             */

            return b;
        }

        /// <summary>
        ///   Computes the circular kurtosis of the given circular angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// 
        /// <returns>The circular kurtosis for the given <paramref name="angles"/>.</returns>
        /// 
        public static double Kurtosis(double[] angles)
        {
            // Compute mean direction
            double theta = Circular.Mean(angles);

            // Compute central moments
            double rho2 = CentralMoments(angles, 2).Magnitude;
            double mu2 = NoncentralMoments(angles, 2).Phase;

            // compute skewness 
            double k = 0;
            for (int i = 0; i < angles.Length; i++) // Pewsey, Metrika, 2004
                k += Math.Cos(2 * Circular.Distance(angles[i], theta));
            k /= angles.Length;

            /*
            double k0 = 0;
            double R4 = (R * R * R * R);
            double omR2 = (1 - R) * (1 - R);
            for (int i = 0; i < angles.Length; i++) // Fisher, Circular Statistics, p. 34
                k0 += (rho2 * Math.Cos(Circular.Distance(mu2, 2 * theta)) - R4) / omR2; // (formula 2.30)
            */

            return k;
        }

        /// <summary>
        ///   Computes the complex circular central 
        ///   moments of the given circular angles.
        /// </summary>
        /// 
        public static Complex CentralMoments(double[] angles, int order)
        {
            double theta = Mean(angles);
            double v = angles.Length / theta;
            double[] alpha = new double[angles.Length];

            for (int i = 0; i < alpha.Length; i++)
                alpha[i] = Distance(angles[i], v);

            return NoncentralMoments(angles, order);
        }

        /// <summary>
        ///   Computes the complex circular non-central
        ///   moments of the given circular angles.
        /// </summary>
        /// 
        public static Complex NoncentralMoments(double[] angles, int order)
        {
            double cbar = 0;
            double sbar = 0;

            for (int i = 0; i < angles.Length; i++)
            {
                cbar += Math.Cos(angles[i] * order);
                sbar += Math.Sin(angles[i] * order);
            }

            return new Complex(cbar, sbar);
        }
    }
}
