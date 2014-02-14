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
    using System.Collections.Generic;
    using System;

    /// <summary>
    ///   Directions for the General Comparer.
    /// </summary>
    /// 
    public enum ComparerDirection
    {
        /// <summary>
        ///   Sorting will be performed in ascending order.
        /// </summary>
        /// 
        Ascending = +1,

        /// <summary>
        ///   Sorting will be performed in descending order.
        /// </summary>
        /// 
        Descending = -1
    };

    /// <summary>
    ///   General comparer which supports multiple 
    ///   directions and comparison of absolute values.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    ///   // Assume we have values to sort
    ///   double[] values = { 0, -5, 3, 1, 8 };
    ///   
    ///   // We can create an ad-hoc sorting rule considering only absolute values
    ///   Array.Sort(values, new GeneralComparer(ComparerDirection.Ascending, Math.Abs));
    ///   
    ///   // Result will be { 0, 1, 3, 5, 8 }.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="ElementComparer{T}"/>
    /// <seealso cref="ArrayComparer{T}"/>
    /// <seealso cref="GeneralComparer"/>
    /// <seealso cref="CustomComparer{T}"/>
    /// 
    public class GeneralComparer : IComparer<double>, IComparer<int>
    {
        Func<double, double> map;
        private int direction = 1;

        /// <summary>
        ///   Gets or sets the sorting direction
        ///   used by this comparer.
        /// </summary>
        /// 
        public ComparerDirection Direction
        {
            get { return (ComparerDirection)direction; }
            set { direction = (int)value; }
        }

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// 
        /// <param name="direction">The direction to compare.</param>
        /// 
        public GeneralComparer(ComparerDirection direction)
            : this(direction, false) { }

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// 
        /// <param name="direction">The direction to compare.</param>
        /// <param name="useAbsoluteValues">True to compare absolute values, false otherwise. Default is false.</param>
        /// 
        public GeneralComparer(ComparerDirection direction, bool useAbsoluteValues)
        {
            if (useAbsoluteValues)
                this.map = Math.Abs;
            else this.map = (a) => a;

            this.direction = (int)direction;
        }

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// 
        /// <param name="direction">The direction to compare.</param>
        /// <param name="map">The mapping function which will be applied to
        ///   each vector element prior to any comparisons.</param>
        /// 
        public GeneralComparer(ComparerDirection direction, Func<double, double> map)
        {
            this.map = map;
            this.direction = (int)direction;
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than,
        ///    equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public int Compare(double x, double y)
        {
            return direction * (map(x).CompareTo(map(y)));
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than,
        ///    equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public int Compare(int x, int y)
        {
            return direction * (map(x).CompareTo(map(y)));
        }

    }
}
