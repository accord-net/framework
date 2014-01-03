// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;

    /// <summary>
    ///   Split-Set Validation.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is the non-generic version of the <see cref="SplitSetValidation"/>. For
    ///   greater flexibility, consider using <see cref="SplitSetValidation{T}"/>.
    /// </remarks>
    /// 
    /// <seealso cref="Bootstrap"/>
    /// <seealso cref="CrossValidation{T}"/>
    /// <seealso cref="SplitSetValidation{T}"/>
    /// 
    [Serializable]
    public class SplitSetValidation : SplitSetValidation<object>
    {

        /// <summary>
        ///   Creates a new split-set validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// <param name="proportion">The desired proportion of samples in the training
        /// set in comparison with the testing set.</param>
        /// 
        public SplitSetValidation(int size, double proportion)
            : base(size, proportion)
        {
        }

        /// <summary>
        ///   Creates a new split-set validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// <param name="proportion">The desired proportion of samples in the training
        /// set in comparison with the testing set.</param>
        /// <param name="outputs">The output labels to be balanced between the sets.</param>
        /// 
        public SplitSetValidation(int size, double proportion, int[] outputs)
            : base(size, proportion, outputs)
        {
        }

    }
}
