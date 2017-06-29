// Accord Machine Learning Library
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

namespace Accord.MachineLearning.Performance
{

    /// <summary>
    ///   Range of parameters to be tested in a grid search.
    /// </summary>
    /// 
    public class GridSearchRange<T> : IGridSearchRange
    {
        /// <summary>
        ///   Gets or sets the range of values that should be tested for this parameter.
        /// </summary>
        /// 
        public T[] Values { get; set; }

        /// <summary>
        ///   Gets or sets the index of the current value in the search,
        ///   whose value will be shown in the <see cref="Value"/> property.
        /// </summary>
        /// 
        public int Index { get; set; }

        /// <summary>
        ///   Gets the current value being considered during the grid-search.
        /// </summary>
        /// 
        public T Value { get { return Values[Index]; } }

        /// <summary>
        ///   Gets the number of values that this parameter can assume (the length of the parameter range).
        /// </summary>
        /// 
        public int Length { get { return Values.Length; } }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>A new object that is a copy of this instance.</returns>
        /// 
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="GridSearchRange{T}"/> to <typeparam ref="T"/>.
        /// </summary>
        /// 
        /// <param name="range">The range to be converted.</param>
        /// 
        /// <returns>The value of the parameter's <see cref="GridSearchRange{T}.Value"/>.</returns>
        /// 
        public static implicit operator T(GridSearchRange<T> range)
        {
            return range.Value;
        }

    }
}
