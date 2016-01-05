// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.Math.Distances
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Euclidean distance metric.
    /// </summary>
    /// 
    /// <seealso cref="SquareEuclidean"/>
    /// 
    [Serializable]
    public sealed class Euclidean : IMetric<double[]>, ISimilarity<double[]>,
        IMetric<Tuple<double, double>>, ISimilarity<Tuple<double, double>>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Euclidean"/> class.
        /// </summary>
        /// 
        public Euclidean()
        {
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] - y[i];
                sum += u * u;
            }
            return Math.Sqrt(sum);
        }

 
        /// <summary>
        ///   Gets the Euclidean distance between two points. Note: this function 
        ///   is dangerous as it is too easy to invert its arguments by mistake. 
        ///   Please consider using the Tuple&lt;double, double> overload instead.
        /// </summary>
        /// 
        /// <param name="x1">The first coordinate of first point in space.</param>
        /// <param name="y1">The second coordinate of first point in space.</param>
        /// <param name="x2">The first coordinate of second point in space.</param>
        /// <param name="y2">The second coordinate of second point in space.</param>
        /// 
        /// <returns>The Euclidean distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        ///   Gets the Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="x">The first point in space.</param>
        /// <param name="y">The second point in space.</param>
        /// 
        /// <returns>The Euclidean distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(Tuple<double, double> x, Tuple<double, double> y)
        {
            double dx = x.Item1 - y.Item1;
            double dy = y.Item1 - y.Item2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        ///   Gets a similarity measure between two points.
        /// </summary>
        /// 
        /// <param name="x">The first point to be compared.</param>
        /// <param name="y">The second point to be compared.</param>
        /// 
        /// <returns>A similarity measure between x and y.</returns>
        /// 
        public double Similarity(double[] x, double[] y)
        {
            double sum = 0.0;

            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] - y[i];
                sum += u * u;
            }

            return 1.0 / (1.0 + Math.Sqrt(sum));
        }

        /// <summary>
        ///   Gets a similarity measure between two points.
        /// </summary>
        /// 
        /// <param name="x">The first point to be compared.</param>
        /// <param name="y">The second point to be compared.</param>
        /// 
        /// <returns>A similarity measure between x and y.</returns>
        /// 
        public double Similarity(Tuple<double, double> x, Tuple<double, double> y)
        {
            double dx = x.Item1 - y.Item1;
            double dy = y.Item1 - y.Item2;

            return 1.0 / (1.0 + Math.Sqrt(dx * dx + dy * dy));
        }
    }
}
