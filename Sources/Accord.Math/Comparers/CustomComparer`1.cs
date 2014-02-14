// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Math.Comparers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Custom comparer which accepts any delegate or
    ///   anonymous function to perform value comparisons.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// 
    /// <example>
    /// <code>
    ///   // Assume we have values to sort
    ///   double[] values = { 0, 5, 3, 1, 8 };
    ///   
    ///   // We can create an ad-hoc sorting rule using
    ///   Array.Sort(values, new CustomComparer&lt;double>((a, b) => -a.CompareTo(b)));
    ///   
    ///   // Result will be { 8, 5, 3, 1, 0 }.
    /// </code>
    /// </example>
    /// 
    public class CustomComparer<T> : IComparer<T>, IEqualityComparer<T>
    {

        private Func<T, T, int> comparer;

        /// <summary>
        ///   Constructs a new <see cref="CustomComparer&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="comparer">The comparer function.</param>
        /// 
        public CustomComparer(Func<T, T, int> comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating
        ///   whether one is less than, equal to, or greater than
        ///   the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        /// <returns>A signed integer that indicates the relative values of x and y.</returns>
        /// 
        public int Compare(T x, T y)
        {
            return comparer(x, y);
        }

        /// <summary>
        ///   Determines whether the specified objects are equal.
        /// </summary>
        /// 
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// 
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// 
        public bool Equals(T x, T y)
        {
            return comparer(x, y) == 0;
        }

        /// <summary>
        ///   Returns a hash code for the given object.
        /// </summary>
        /// 
        /// <param name="obj">The object.</param>
        /// 
        /// <returns>
        ///   A hash code for the given object, suitable for use in
        ///   hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

}
