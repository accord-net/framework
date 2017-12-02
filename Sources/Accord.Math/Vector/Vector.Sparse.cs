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

namespace Accord.Math
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    public static partial class Vector
    {

        /// <summary>
        ///   Gets the inner product (scalar product) between two vectors (a'*b).
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// 
        /// <returns>The inner product of the multiplication of the vectors.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Dot(this Sparse<double> a, Sparse<double> b)
        {
            double sum = 0;

            int i = 0, j = 0;

            while (i < a.Indices.Length && j < b.Indices.Length)
            {
                int posx = a.Indices[i];
                int posy = b.Indices[j];

                if (posx == posy)
                {
                    sum += a.Values[i] * b.Values[j];
                    i++;
                    j++;
                }
                else if (posx < posy)
                {
                    i++;
                }
                else //if (posx > posy)
                {
                    j++;
                }
            }

            return sum;
        }

        /// <summary>
        ///   Gets the inner product (scalar product) between two vectors (a'*b).
        /// </summary>
        /// 
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// 
        /// <returns>The inner product of the multiplication of the vectors.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Dot(this Sparse<double> a, double[] b)
        {
            double sum = 0;

            int i = 0, j = 0;

            while (i < a.Indices.Length && j < b.Length)
            {
                int posx = a.Indices[i];
                int posy = j;

                if (posx == posy)
                {
                    sum += a.Values[i] * b[j];
                    i++;
                    j++;
                }
                else if (posx < posy)
                {
                    i++;
                }
                else // if (posx > posy)
                {
                    j++;
                }
            }

            return sum;
        }

        /// <summary>
        ///   Adds a sparse vector to a dense vector.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] Add(this Sparse<double> a, double[] b, double[] result)
        {
            for (int j = 0; j < b.Length; j++)
                result[j] = b[j];

            for (int j = 0; j < a.Indices.Length; j++)
                result[a.Indices[j]] += a.Values[j];

            return result;
        }

        /// <summary>
        /// Divides an array of sparse vectors by the associated scalars in a dense vector.
        /// </summary>
        /// 
        public static Sparse<double>[] Divide(this Sparse<double>[] a, double[] b, Sparse<double>[] result)
        {
            for (int i = 0; i < a.Length; i++)
                Elementwise.Divide(a[i].Values, b[i], result[i].Values);
            return result;
        }

        /// <summary>
        /// Divides a sparse vector by a scalar.
        /// </summary>
        /// 
        public static Sparse<double> Divide(this Sparse<double> a, double b, Sparse<double> result)
        {
            Elementwise.Divide(a.Values, b, result.Values);
            return result;
        }

    }
}
