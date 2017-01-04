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
        ///   Creates a vector containing every index that can be used to
        ///   address a given <paramref name="array"/>, in order. 
        /// </summary>
        /// 
        /// <param name="array">The array whose indices will be returned.</param>
        /// 
        /// <returns>
        ///   A vector of the same size as the given <paramref name="array"/> 
        ///   containing all vector indices from 0 up to the length of 
        ///   <paramref name="array"/>.
        /// </returns>
        /// 
        /// <example>
        /// <code>
        ///   double[] a = { 5.3, 2.3, 4.2 };
        ///   int[] idx = a.GetIndices(); // output will be { 0, 1, 2 }
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Matrix.GetIndices"/>
        /// 
        public static int[] GetIndices<T>(this T[] array)
        {
            return Vector.Range(0, array.Length);
        }


    }
}
