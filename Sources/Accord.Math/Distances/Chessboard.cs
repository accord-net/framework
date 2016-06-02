// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    ///   Chessboard metric. 
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The chessboard metric is defined as the maximum absolute 
    ///   difference between elements at the same position of two
    ///   vectors.</para>
    ///   
    /// <para>
    ///   For two dimensional spaces, the metric could be given as:</para>
    /// <code>
    ///   d((x1, y1), (x2,y2)) = max(abs(x1 - x2), abs(y1 - y2))
    /// </code>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Chessboard : IMetric<double[]>, IMetric<int[]>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Chessboard"/> class.
        /// </summary>
        /// 
        public Chessboard()
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
        public double Distance(int[] x, int[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum = Math.Max(sum, x[i] - y[i]);
            return sum;
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
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum = Math.Max(sum, x[i] - y[i]);
            return sum;
        }

    }
}
