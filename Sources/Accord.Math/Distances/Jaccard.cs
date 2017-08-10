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
    ///   Jaccard (Index) distance.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Jaccard index, also known as the Jaccard similarity coefficient (originally
    ///   coined coefficient de communauté by Paul Jaccard), is a statistic used for comparing
    ///   the similarity and diversity of sample sets. The Jaccard coefficient measures 
    ///   similarity between finite sample sets, and is defined as the size of the intersection
    ///   divided by the size of the union of the sample sets.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Jaccard_index">
    ///       https://en.wikipedia.org/wiki/Jaccard_index </a></description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    /// <seealso cref="Jaccard{T}"/>
    /// 
    [Serializable]
    public struct Jaccard : ISimilarity<double[]>, IDistance<double[]>
    {
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
            int inter = 0;
            int union = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != 0 || y[i] != 0)
                {
                    if (x[i] == y[i])
                        inter++;
                    union++;
                }
            }

            return (union == 0) ? 0 : 1.0 - (inter / (double)union);
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
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Similarity(double[] x, double[] y)
        {
            int inter = 0;
            int union = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != 0 || y[i] !=0 )
                {
                    if (x[i] == y[i])
                        inter++;
                    union++;
                }
            }

            return (inter == 0) ? 0 : inter / (double)union;
        }

    }
}
