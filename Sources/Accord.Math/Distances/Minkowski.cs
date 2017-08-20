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

namespace Accord.Math.Distances
{
    using System;
    using System.Runtime.CompilerServices;
    using Accord.Compat;

    /// <summary>
    ///   The Minkowski distance is a metric in a normed vector space which can be 
    ///   considered as a generalization of both the <see cref="Euclidean">Euclidean 
    ///   distance</see> and the <see cref="Manhattan">Manhattan distance</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The framework distinguishes between metrics and distances by using different
    ///   types for them. This makes it possible to let the compiler figure out logic
    ///   problems such as the specification of a non-metric for a method that requires
    ///   a proper metric (i.e. that respects the triangle inequality).</para>
    ///   
    /// <para>
    ///   The objective of this technique is to make it harder to make some mistakes.
    ///   However, it is possible to bypass this mechanism by using the named constructors
    ///   such as <see cref="Nonmetric"/> to create distances implementing the <see cref="IMetric{T}"/>
    ///   interface that are not really metrics. Use at your own risk.</para>
    /// </remarks>
    /// 
    [Serializable]
    public struct Minkowski : IMetric<double[]>, IMetric<int[]>
    {
        private double p;

        /// <summary>
        ///   Gets the order <c>p</c> of this Minkowski distance.
        /// </summary>
        /// 
        public double Order { get { return p; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Minkowski"/> class.
        /// </summary>
        /// 
        /// <param name="p">The Minkowski order <c>p</c>.</param>
        /// 
        /// <exception cref="System.ArgumentOutOfRangeException">The Minkowski distance is not a metric for p &lt; 1.</exception>
        /// 
        public Minkowski(double p)
        {
            if (p < 1)
                throw new ArgumentOutOfRangeException("The Minkowski distance is not a metric for p < 1.");

            this.p = p;
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
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(int[] x, int[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += Math.Pow(Math.Abs(x[i] - y[i]), p);
            return Math.Pow(sum, 1 / p);
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
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] x, double[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += Math.Pow(Math.Abs(x[i] - y[i]), p);
            return Math.Pow(sum, 1 / p);
        }

        /// <summary>
        ///   Creates a non-metric Minkowski distance, bypassing
        ///   argument checking. Use at your own risk.
        /// </summary>
        /// 
        /// <param name="p">The Minkowski order <c>p</c>.</param>
        /// 
        /// <returns>A Minkowski object implementing a Minkowski distance
        ///   that is not necessarily a metric. Use at your own risk.</returns>
        /// 
        public static Minkowski Nonmetric(double p)
        {
            var minkowski = new Minkowski(1);
            minkowski.p = p;
            return minkowski;
        }

        /// <summary>
        ///   Gets the <see cref="Manhattan"/> distance as a special 
        ///   case of the <see cref="Minkowski"/> distance.
        /// </summary>
        /// 
        public static readonly Minkowski Manhattan = new Minkowski(1);

        /// <summary>
        ///   Gets the <see cref="Euclidean"/> distance as a special 
        ///   case of the <see cref="Minkowski"/> distance.
        /// </summary>
        /// 
        public static readonly Minkowski Euclidean = new Minkowski(2);

    }
}
