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
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Collections;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Compat;

#if !MONO
    /// <summary>
    ///   Base class for <see cref="VPTree">Vantage-Point Trees</see>.
    /// </summary>
    /// 
    /// <typeparam name="TPoint">The type for the position vector of each node.</typeparam>
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    /// <seealso cref="VPTree"/>
    /// <seealso cref="VPTree{TPoint, TData}"/>
    /// <seealso cref="VPTree{TPoint}"/>
    /// 
    [Serializable]
    public class VPTreeBase<TPoint, TNode> : BinaryTree<TNode>, IEnumerable<TNode>
        where TNode : VPTreeNodeBase<TPoint, TNode>, new()
    {
        double tau;
        IDistance<TPoint> distance;

        /// <summary>
        ///   Gets or set the distance function used to
        ///   measure distances amongst points on this tree
        /// </summary>
        ///
        public IDistance<TPoint> Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Gets or sets the radius of the nodes in the tree.
        /// </summary>
        /// 
        public double Radius
        {
            get { return tau; }
            set { tau = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="VPTreeBase{TPoint, TNode}"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance to use when comparing points.</param>
        /// 
        public VPTreeBase(IDistance<TPoint> distance)
        {
            this.Distance = distance;
        }



        /// <summary>
        ///   Retrieves a fixed point of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="neighbors">The number of neighbors to retrieve.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public List<NodeDistance<TNode>> Nearest(TPoint position, int neighbors)
        {
            return Nearest(position, neighbors, new List<NodeDistance<TNode>>());
        }

        /// <summary>
        ///   Retrieves a fixed point of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="neighbors">The number of neighbors to retrieve.</param>
        /// <param name="results">The list where to store results.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public List<NodeDistance<TNode>> Nearest(TPoint position, int neighbors, List<NodeDistance<TNode>> results)
        {
            // Use a priority queue to store intermediate results on
            var heap = new PriorityQueue<TNode>(neighbors, order: PriorityOrder.Maximum);

            // Variable that tracks the distance to the farthest point in our results
            tau = Double.MaxValue;

            // Perform the search
            search(Root, position, neighbors, heap);

            string str = heap.ToString();

            // Gather final results
            while (heap.Count != 0)
            {
                var node = heap.Dequeue();
                results.Add(new NodeDistance<TNode>(node.Value, node.Priority));
            }

            // Results are in reverse order
            results.Reverse();
            return results;
        }




        // Function that (recursively) fills the tree
        internal TNode buildFromPoints(TPoint[] items, int lower, int upper, bool inPlace)
        {
            if (inPlace)
            {
                items = (TPoint[])items.Clone();
            }

            if (upper == lower)
                return null;

            var rand = Accord.Math.Random.Generator.Random;

            // Lower index is center of current node
            var node = new TNode();
            node.Position = items[lower];

            // if we did not arrive at leaf yet
            if (upper - lower > 1)
            {
                // Choose an arbitrary point and move it to the start
                int i = rand.Next(lower, upper);
                items.Swap(lower, i);

                // Partition around the median distance
                int median = (upper + lower) / 2;
                TPoint e = items[lower];

                Func<TPoint, TPoint, int> comparer = (TPoint x, TPoint y) =>
                    Distance.Distance(e, x).CompareTo(Distance.Distance(e, y));

                Sort.NthElement(items,
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
                node.Position = e;
                node.Left = buildFromPoints(items, lower + 1, median, inPlace: true);
                node.Right = buildFromPoints(items, median, upper, inPlace: true);
            }

            // Return result
            return node;
        }

        // Helper function that searches the tree
        internal void search(TNode node, TPoint target, int k, PriorityQueue<TNode> heap)
        {
            if (node == null)
                return;     // indicates that we're done here

            // Compute distance between target and current node
            double dist = Distance.Distance(node.Position, target);

            // If current node within radius tau
            if (dist < tau)
            {
                if (heap.Count == k)
                    heap.Dequeue();                 // remove furthest node from result list (if we already have k results)
                heap.Enqueue(node, dist);           // add current node to result list
                if (heap.Count == k)
                    tau = heap.First.Priority;     // update value of tau (farthest point in result list)
            }

            // Return if we arrived at a leaf
            if (node.Left == null && node.Right == null)
                return;

            // If the target lies within the radius of ball
            if (dist < node.Threshold)
            {
                if (dist - tau <= node.Threshold)
                {
                    // if there can still be neighbors inside the ball, recursively search left child first
                    search(node.Left, target, k, heap);
                }

                if (dist + tau >= node.Threshold)
                {
                    // if there can still be neighbors outside the ball, recursively search right child
                    search(node.Right, target, k, heap);
                }

                // If the target lies outsize the radius of the ball
            }
            else
            {
                if (dist + tau >= node.Threshold)
                {
                    // if there can still be neighbors outside the ball, recursively search right child first
                    search(node.Right, target, k, heap);
                }

                if (dist - tau <= node.Threshold)
                {
                    // if there can still be neighbors inside the ball, recursively search left child
                    search(node.Left, target, k, heap);
                }
            }
        }
    }
#endif
}