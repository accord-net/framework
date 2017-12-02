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
    ///   Levenshtein distance.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In information theory and computer science, the Levenshtein distance is a
    ///   string metric for measuring the difference between two sequences. Informally,
    ///   the Levenshtein distance between two words is the minimum number of single-character 
    ///   edits (i.e. insertions, deletions or substitutions) required to change one 
    ///   word into the other. It is named after Vladimir Levenshtein, who considered 
    ///   this distance in 1965.</para>
    ///   
    /// <para>
    ///   Levenshtein distance may also be referred to as edit distance, although that 
    ///   may also denote a larger family of distance metrics. It is closely related to
    ///   pairwise string alignments.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Levenshtein_distance">
    ///       https://en.wikipedia.org/wiki/Levenshtein_distance </a></description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    /// <typeparam name="T">The type of elements in the string. Default is char.</typeparam>
    /// 
    [Serializable]
    public struct Levenshtein<T> : IMetric<T[]>, ICloneable
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
        public double Distance(T[] x, T[] y)
        {
            if (x == null || x.Length == 0)
            {
                if (y == null || y.Length == 0)
                    return 0;
                return y.Length;
            }
            else
            {
                if (y == null || y.Length == 0)
                    return x.Length;
            }

            int[,] d = new int[x.Length + 1, y.Length + 1];

            for (int i = 0; i <= x.Length; i++)
                d[i, 0] = i;

            for (int i = 0; i <= y.Length; i++)
                d[0, i] = i;

            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < y.Length; j++)
                {
                    int cost = (x[i].Equals(y[j])) ? 0 : 1;

                    int a = d[i, j + 1] + 1;
                    int b = d[i + 1, j] + 1;
                    int c = d[i, j] + cost;

                    d[i + 1, j + 1] = Math.Min(Math.Min(a, b), c);
                }
            }

            return d[x.Length, y.Length];
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Levenshtein<T>();
        }
    }
}
