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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Training-Test split.
    /// </summary>
    /// 
    /// <typeparam name="T">The type being separated in training and test splits.</typeparam>
    /// 
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    /// 
    public class TrainTestSplit<T> : IEnumerable<T>
    {
        /// <summary>
        ///   Gets or sets the training split.
        /// </summary>
        /// 
        public T Training { get; set; }

        /// <summary>
        ///   Gets or sets the testing split.
        /// </summary>
        /// 
        public T Testing { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            yield return Training;
            yield return Testing;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return Training;
            yield return Testing;
        }
    }
}
