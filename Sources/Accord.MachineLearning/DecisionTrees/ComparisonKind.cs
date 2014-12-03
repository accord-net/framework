// Accord Machine Learning Library
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

namespace Accord.MachineLearning.DecisionTrees
{
    using System;


    /// <summary>
    ///   Numeric comparison category.
    /// </summary>
    /// 
    public enum ComparisonKind
    {
        /// <summary>
        ///   The node does no comparison.
        /// </summary>
        /// 
        None,

        /// <summary>
        ///   The node compares for equality.
        /// </summary>
        /// 
        Equal,

        /// <summary>
        ///   The node compares for non-equality.
        /// </summary>
        /// 
        NotEqual,

        /// <summary>
        ///   The node compares for greater-than or equality.
        /// </summary>
        /// 
        GreaterThanOrEqual,

        /// <summary>
        ///   The node compares for greater-than.
        /// </summary>
        /// 
        GreaterThan,

        /// <summary>
        ///   The node compares for less-than.
        /// </summary>
        /// 
        LessThan,

        /// <summary>
        ///   The node compares for less-than or equality.
        /// </summary>
        LessThanOrEqual
    }

    /// <summary>
    ///   Extension methods for <see cref="ComparisonKind"/> enumeration values.
    /// </summary>
    /// 
    public static class ComparisonExtensions
    {

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <param name="comparison">The comparison type.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this ComparisonKind comparison)
        {
            switch (comparison)
            {
                case ComparisonKind.Equal:
                    return "==";

                case ComparisonKind.GreaterThan:
                    return ">";

                case ComparisonKind.GreaterThanOrEqual:
                    return ">=";

                case ComparisonKind.LessThan:
                    return "<";

                case ComparisonKind.LessThanOrEqual:
                    return "<=";

                case ComparisonKind.NotEqual:
                    return "!=";

                default:
                    throw new InvalidOperationException("Unexpected node comparison type.");
            }
        }
    }
}
