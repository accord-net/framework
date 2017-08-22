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
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Non-generic interface for <see cref="GridSearchRange{T}"/>.
    /// </summary>
    /// 
    /// <seealso cref="GridSearch"/>
    /// <seealso cref="GridSearchRange{T}"/>
    /// 
    public interface IGridSearchRange : ICloneable
    {
        /// <summary>
        ///   Gets or sets the index of the current value in the search.
        /// </summary>
        /// 
        int Index { get; set; }

        /// <summary>
        ///   Gets the number of values that this parameter can assume (the length of the parameter range).
        /// </summary>
        /// 
        int Length { get; }
    }
}
