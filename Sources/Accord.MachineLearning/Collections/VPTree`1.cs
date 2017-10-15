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

namespace Accord.Collections
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Math.Distances;
    using Accord.Compat;

#if !MONO
    /// <summary>
    ///   Vantage-Point Tree.
    /// </summary>
    /// 
    /// <typeparam name="TPoint">The type for the position vector of each node.</typeparam>
    /// 
    [Serializable]
    public class VPTree<TPoint> : VPTreeBase<TPoint, VPTreeNode<TPoint>>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="VPTree{TPoint}"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance to use when comparing points.</param>
        /// 
        public VPTree(IDistance<TPoint> distance)
            : base(distance)
        {
        }

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree{TPoint}"/> populated with the given data points.</returns>
        /// 
        public static VPTree<TPoint> FromData(TPoint[] points, IDistance<TPoint> distance, bool inPlace = false)
        {
            var tree = new VPTree<TPoint>(distance);
            tree.Root = tree.buildFromPoints(points, 0, points.Length, inPlace);
            return tree;
        }
    }
#endif
}