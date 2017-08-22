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

namespace Accord.MachineLearning
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Common interface for Bag of Words objects.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the element to be 
    /// converted to a fixed-length vector representation.</typeparam>
    /// 
    /// <seealso cref="BagOfWords"/> 
    /// 
    public interface IBagOfWords<T> : ITransform<T, double[]>, ITransform<T, int[]>
    {
        /// <summary>
        ///   Gets the number of words in this codebook.
        /// </summary>
        /// 
        int NumberOfWords { get; }

        /// <summary>
        ///   Gets the codeword representation of a given value.
        /// </summary>
        /// 
        /// <param name="value">The value to be processed.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        [Obsolete("Please use the Transform(value) method instead.")]
        double[] GetFeatureVector(T value);
    }
  
}
