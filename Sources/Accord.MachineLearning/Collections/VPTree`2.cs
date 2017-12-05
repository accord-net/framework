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
// The code contained in this class was adapted from Laurens van der Maaten excellent 
// BH T-SNE code from https://github.com/lvdmaaten/bhtsne/blob/master/vptree.h. It was 
// originally adopted with minor modifications from Steve Hanov's great tutorial available 
// at http://stevehanov.ca/blog/index.php?id=130. The original license is listed below:
//  
//    Copyright (c) 2014, Laurens van der Maaten (Delft University of Technology)
//    All rights reserved.
//   
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//    3. All advertising materials mentioning features or use of this software
//       must display the following acknowledgement:
//       This product includes software developed by the Delft University of Technology.
//    4. Neither the name of the Delft University of Technology nor the names of 
//       its contributors may be used to endorse or promote products derived from 
//       this software without specific prior written permission.
//   
//    THIS SOFTWARE IS PROVIDED BY LAURENS VAN DER MAATEN ''AS IS'' AND ANY EXPRESS
//    OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
//    OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO 
//    EVENT SHALL LAURENS VAN DER MAATEN BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
//    BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
//    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
//    IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
//    OF SUCH DAMAGE.
//   

namespace Accord.Collections
{
    using Accord.Math;
    using Accord.Math.Distances;
    using System;
    using System.Linq;
    using System.Collections.Generic;

#if !MONO
    /// <summary>
    ///   Vantage-Point Tree.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A vantage-point tree (or VP tree) is a metric tree that segregates data in a metric space by choosing
    ///   a position in the space (the "vantage point") and partitioning the data points into two parts: those 
    ///   points that are nearer to the vantage point than a threshold, and those points that are not. By 
    ///   recursively applying this procedure to partition the data into smaller and smaller sets, a tree data
    ///   structure is created where neighbors in the tree are likely to be neighbors in the space.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Vantage-point tree. Available on:
    ///       https://en.wikipedia.org/wiki/Vantage-point_tree </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <typeparam name="TPoint">The type for the position vector of each node.</typeparam>
    /// <typeparam name="TData">The type for the value stored at each node.</typeparam>
    /// 
    /// <example>
    /// 
    /// </example>
    /// 
    [Serializable]
    public class VPTree<TPoint, TData> : VPTreeBase<TPoint, VPTreeNode<TPoint, TData>>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="VPTree{TPoint, TData}"/> class.
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
        /// <param name="values">The corresponding values at each data point.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree{TPoint, TData}"/> populated with the given data points.</returns>
        /// 
        public static VPTree<TPoint, TData> FromData(TPoint[] points, TData[] values, IDistance<TPoint> distance, bool inPlace)
        {
            var tree = new VPTree<TPoint, TData>(distance);
            tree.Root = tree.buildFromPoints(points, values, 0, points.Length - 1, inPlace);
            return tree;
        }




        /// <summary>
        ///  Function that (recursively) fills the tree 
        /// </summary>
        VPTreeNode<TPoint, TData> buildFromPoints(TPoint[] items, TData[] values, int lower, int upper, bool inPlace)
        {
            if (!inPlace)
            {
                items = (TPoint[])items.Clone();
                values = (TData[])values.Clone();
            }

            if (upper == lower)
                return null;

            var rand = Accord.Math.Random.Generator.Random;

            // Lower index is center of current node
            var node = new VPTreeNode<TPoint, TData>();
            node.Position = items[lower];
            node.Value = values[lower];

            // if we did not arrive at leaf yet
            if (upper - lower > 1)
            {
                // Choose an arbitrary point and move it to the start
#if DEBUG
                int i = upper - 1;
#else
                int i = rand.Next(lower, upper);
#endif
                items.Swap(lower, i);
                values.Swap(lower, i);

                // Partition around the median distance
                int median = (upper + lower) / 2;
                TPoint e = items[lower];

                Func<TPoint, TPoint, int> comparer = (TPoint x, TPoint y) =>
                    {
                        var d1 = Distance.Distance(e, x);
                        var d2 = Distance.Distance(e, y);
                        return d1.CompareTo(d2);
                    };

                Sort.NthElement(items, values,
                    first: lower + 1,
                    last: upper,
                    n: median,
                    compare: comparer);

                //std::nth_element(_items.begin() + lower + 1,
                //                 _items.begin() + median,
                //                 _items.begin() + upper,
                //                 distancecomparator(_items[lower]));

                // Threshold of the new node will be the distance to the median
                node.Threshold = Distance.Distance(items[lower], items[median]);

                // Recursively build tree
                node.Position = items[lower];
                node.Value = values[lower];
                node.Left = buildFromPoints(items, values, lower + 1, median, inPlace: true);
                node.Right = buildFromPoints(items, values, median, upper, inPlace: true);
            }

            // Return result
            return node;
        }
    }
#endif
}