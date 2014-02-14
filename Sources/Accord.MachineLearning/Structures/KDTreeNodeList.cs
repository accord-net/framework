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

namespace Accord.MachineLearning.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   List of k-dimensional tree nodes.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    /// 
    /// <remarks>
    ///   This class is used to store neighbor nodes when running one of the
    ///   search algorithms for <see cref="KDTree{T}">k-dimensional trees</see>.
    /// </remarks>
    /// 
    /// <seealso cref="KDTree{T}"/>
    /// <seealso cref="KDTreeNodeDistance{T}"/>
    /// 
    [Serializable]
    public class KDTreeNodeList<T> : List<KDTreeNodeDistance<T>>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="KDTreeNodeList&lt;T&gt;"/>
        ///   class that is empty.
        /// </summary>
        /// 
        public KDTreeNodeList()
        {

        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KDTreeNodeList&lt;T&gt;"/>
        ///   class that is empty and has the specified capacity.
        /// </summary>
        /// 
        public KDTreeNodeList(int capacity)
            : base(capacity)
        {

        }
    }
}
