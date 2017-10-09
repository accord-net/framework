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
    using System.Collections.Generic;


    public static partial class Vector
    {
        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static double[] Random(int size)
        {
            return Random(size, 0.0, 1.0);
        }

        /// <summary>
        ///   Draws a random sample from a group of observations, without repetitions.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the observations.</typeparam>
        /// 
        /// <param name="values">The observation vector.</param>
        /// <param name="size">The size of the sample to be drawn (how many samples to get).</param>
        /// 
        /// <returns>A vector containing the samples drawn from <paramref name="values"/>.</returns>
        /// 
        public static T[] Sample<T>(this T[] values, int size)
        {
            int[] idx = Vector.Sample(size, values.Length);
            System.Diagnostics.Debug.Assert(idx.Length == size);
            return values.Get(idx);
        }
    }
}
