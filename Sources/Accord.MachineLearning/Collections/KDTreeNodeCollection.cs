// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Rednaxela, 2009
// http://robowiki.net/wiki/User:Rednaxela
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
// The internal implementation for this class have been rewritten during v2.15.
// Before, this class used to work based on standard .NET collections. However,
// those have been replaced based on an Interval Heap implementation by Rednaxela.
// Please note that the entire KD-tree implementation haven't changed, but only
// the internal workings of this single class (KDTreeNodeCollection). The original
// author has chosen to distribute his code under a permissive license, reproduced
// below.
// 
//    Copyright 2009 Rednaxela
// 
//    This software is provided 'as-is', without any express or implied
//    warranty. In no event will the authors be held liable for any damages
//    arising from the use of this software.
//     
//    Permission is granted to anyone to use this software for any purpose,
//    including commercial applications, and to alter it and redistribute it
//    freely, subject to the following restrictions:
// 
//    1. The origin of this software must not be misrepresented; you must not
//       claim that you wrote the original software. If you use this software
//       in a product, an acknowledgment in the product documentation would be
//       appreciated but is not required.
// 
//    2. This notice may not be removed or altered from any source
//       distribution.
// 

namespace Accord.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Compat;

    /// <summary>
    ///   Collection of k-dimensional tree nodes.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    /// <remarks>
    ///   This class is used to store neighbor nodes when running one of the
    ///   search algorithms for <see cref="KDTree{T}">k-dimensional trees</see>.
    /// </remarks>
    /// 
    /// <seealso cref="KDTree{T}"/>
    /// <seealso cref="NodeDistance{T}"/>
    /// 
    [Serializable]
    public class KDTreeNodeCollection<TNode> : ICollection<NodeDistance<TNode>>
        where TNode : KDTreeNodeBase<TNode>, IComparable<TNode>, IEquatable<TNode>
    {
        double[] distances;
        TNode[] positions;

        int count;


        /// <summary>
        ///   Gets or sets the maximum number of elements on this 
        ///   collection, if specified. A value of zero indicates
        ///   this instance has no upper limit of elements.
        /// </summary>
        /// 
        public int Capacity { get; private set; }

        /// <summary>
        ///   Gets the minimum distance between a node
        ///   in this collection and the query point.
        /// </summary>
        /// 
        public double Minimum
        {
            get
            {
                if (count == 0)
                    throw new InvalidOperationException();
                return distances[0];
            }
        }

        /// <summary>
        ///   Gets the maximum distance between a node
        ///   in this collection and the query point.
        /// </summary>
        /// 
        public double Maximum
        {
            get
            {
                if (count == 0)
                    throw new InvalidOperationException();

                if (count == 1)
                    return distances[0];
                return distances[1];
            }
        }

        /// <summary>
        ///   Gets the farthest node in the collection (with greatest distance).
        /// </summary>
        /// 
        public TNode Farthest
        {
            get
            {
                if (count == 0)
                    return null;

                if (count == 1)
                    return positions[0];
                return positions[1];
            }
        }

        /// <summary>
        ///   Gets the nearest node in the collection (with smallest distance).
        /// </summary>
        /// 
        public TNode Nearest
        {
            get
            {
                if (count == 0)
                    return null;

                return positions[0];
            }
        }


        /// <summary>
        ///   Creates a new <see cref="KDTreeNodeCollection&lt;T&gt;"/> with a maximum size.
        /// </summary>
        /// 
        /// <param name="size">The maximum number of elements allowed in this collection.</param>
        /// 
        public KDTreeNodeCollection(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size");

            Capacity = size;
            distances = new double[size];
            positions = new TNode[size];
        }

        /// <summary>
        ///   Attempts to add a value to the collection. If the list is full
        ///   and the value is more distant than the farthest node in the
        ///   collection, the value will not be added.
        /// </summary>
        /// 
        /// <param name="value">The node to be added.</param>
        /// <param name="distance">The node distance.</param>
        /// 
        /// <returns>Returns true if the node has been added; false otherwise.</returns>
        /// 
        public bool Add(TNode value, double distance)
        {
            // The list does have a limit. We have to check if the list
            // is already full or not, to see if we can discard or keep
            // the point

            if (count < Capacity)
            {
                // The list still has room for new elements. 
                // Just add the value at the right position.

                add(distance, value);

                return true; // a value has been added
            }

            // The list is at its maximum capacity. Check if the value
            // to be added is closer than the current farthest point.

            if (distance < Maximum)
            {
                // Yes, it is closer. Remove the previous farthest point
                // and insert this new one at an appropriate position to
                // keep the list ordered.

                RemoveFarthest();

                add(distance, value);

                return true; // a value has been added
            }

            // The value is even farther
            return false; // discard it
        }

        /// <summary>
        ///   Attempts to add a value to the collection. If the list is full
        ///   and the value is more distant than the farthest node in the
        ///   collection, the value will not be added.
        /// </summary>
        /// 
        /// <param name="value">The node to be added.</param>
        /// <param name="distance">The node distance.</param>
        /// 
        /// <returns>Returns true if the node has been added; false otherwise.</returns>
        /// 
        public bool AddFarthest(TNode value, double distance)
        {
            // The list does have a limit. We have to check if the list
            // is already full or not, to see if we can discard or keep
            // the point

            if (count < Capacity)
            {
                // The list still has room for new elements. 
                // Just add the value at the right position.

                add(distance, value);

                return true; // a value has been added
            }

            // The list is at its maximum capacity. Check if the value
            // to be added is farther than the current nearest point.

            if (distance > Minimum)
            {
                // Yes, it is farther. Remove the previous nearest point
                // and insert this new one at an appropriate position to
                // keep the list ordered.

                RemoveNearest();

                add(distance, value);

                return true; // a value has been added
            }

            // The value is even closer
            return false; // discard it
        }

        /// <summary>
        ///   Adds the specified item to the collection.
        /// </summary>
        /// 
        /// <param name="distance">The distance from the node to the query point.</param>
        /// <param name="item">The item to be added.</param>
        /// 
        private void add(double distance, TNode item)
        {
            positions[count] = item;
            distances[count] = distance;
            count++;

            // Ensure it is in the right place.
            siftUpLast();
        }


        /// <summary>
        ///   Removes all elements from this collection.
        /// </summary>
        /// 
        public void Clear()
        {
            for (int i = 0; i < positions.Length; i++)
                positions[i] = null;

            count = 0;
        }

        /// <summary>
        ///   Gets the <see cref="Accord.Collections.NodeDistance{T}"/>
        ///   at the specified index. Note: this method will iterate over the entire collection
        ///   until the given position is found.
        /// </summary>
        /// 
        public NodeDistance<TNode> this[int index]
        {
            get { return new NodeDistance<TNode>(positions[index], distances[index]); }
        }

        /// <summary>
        ///   Gets the number of elements in this collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is read only.
        ///   For this collection, always returns false.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through this collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object 
        ///   that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<NodeDistance<TNode>> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return new NodeDistance<TNode>(positions[i], distances[i]);

            yield break;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through this collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that
        ///   can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }




        /// <summary>
        ///   Determines whether this instance contains the specified item.
        /// </summary>
        /// 
        /// <param name="item">The object to locate in the collection. 
        ///   The value can be null for reference types.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the item is found in the collection; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Contains(NodeDistance<TNode> item)
        {
            int i = Array.IndexOf(positions, item.Node);

            if (i == -1)
                return false;

            return distances[i] == item.Distance;
        }

        /// <summary>
        ///   Copies the entire collection to a compatible one-dimensional <see cref="System.Array"/>, starting
        ///   at the specified <paramref name="arrayIndex">index</paramref> of the <paramref name="array">target
        ///   array</paramref>.
        /// </summary>
        /// 
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the
        ///    elements copied from tree. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// 
        public void CopyTo(NodeDistance<TNode>[] array, int arrayIndex)
        {
            int index = arrayIndex;

            foreach (var pair in this)
                array[index++] = pair;
        }

        /// <summary>
        ///   Adds the specified item to this collection.
        /// </summary>
        /// 
        /// <param name="item">The item.</param>
        /// 
        public void Add(NodeDistance<TNode> item)
        {
            Add(item.Node, item.Distance);
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public bool Remove(NodeDistance<TNode> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Removes the farthest tree node from this collection.
        /// </summary>
        /// 
        public void RemoveFarthest()
        {
            // If we have no items in the queue.
            if (count == 0)
                throw new InvalidOperationException("The collection is empty.");

            // If we have one item, remove the min.
            if (count == 1)
            {
                RemoveNearest();
                return;
            }

            // Remove the max.
            count--;
            positions[1] = positions[count];
            distances[1] = distances[count];
            positions[count] = null;
            siftDownMax(1);
        }

        /// <summary>
        ///   Removes the nearest tree node from this collection.
        /// </summary>
        /// 
        public void RemoveNearest()
        {
            // Check for errors.
            if (count == 0)
                throw new InvalidOperationException("The collection is empty.");

            // Remove the min
            count--;
            positions[0] = positions[count];
            distances[0] = distances[count];
            positions[count] = null;
            siftDownMin(0);
        }


        private void siftUpLast()
        {
            // Work out where the element was inserted.
            int u = count - 1;

            // If it is the only element, nothing to do.
            if (u == 0)
                return;

            // If it is the second element, sort with it's pair.
            if (u == 1)
            {
                // Swap if less than paired item.
                if (distances[u] < distances[u - 1])
                    swap(u, u - 1);
            }

            // If it is on the max side, 
            else if (u % 2 == 1)
            {
                // Already paired. Ensure pair is ordered right
                int p = (u / 2 - 1) | 1; // The larger value of the parent pair

                // If less than it's pair
                if (distances[u] < distances[u - 1])
                {
                    // Swap with it's pair
                    u = swap(u, u - 1); 

                    // If smaller than smaller parent pair
                    if (distances[u] < distances[p - 1])
                    {
                        // Swap into min-heap side
                        u = swap(u, p - 1);
                        siftUpMin(u);
                    }
                }
                else
                {
                    // If larger that larger parent pair
                    if (distances[u] > distances[p])
                    {
                        // Swap into max-heap side
                        u = swap(u, p);
                        siftUpMax(u);
                    }
                }
            }
            else
            {
                // Inserted in the lower-value slot without a partner
                int p = (u / 2 - 1) | 1; // The larger value of the parent pair

                // If larger that larger parent pair
                if (distances[u] > distances[p])
                {
                    // Swap into max-heap side
                    u = swap(u, p);
                    siftUpMax(u);
                }

                // else if smaller than smaller parent pair
                else if (distances[u] < distances[p - 1])
                {
                    // Swap into min-heap side
                    u = swap(u, p - 1);
                    siftUpMin(u);
                }
            }
        }

        private void siftUpMin(int c)
        {
            // Min-side parent: (x/2-1)&~1
            for (int p = (c / 2 - 1) & ~1; p >= 0 && distances[c] < distances[p]; c = p, p = (c / 2 - 1) & ~1)
            {
                swap(c, p);
            }
        }

        private void siftUpMax(int c)
        {
            // Max-side parent: (x/2-1)|1
            for (int p = (c / 2 - 1) | 1; p >= 0 && distances[c] > distances[p]; c = p, p = (c / 2 - 1) | 1)
            {
                swap(c, p);
            }
        }

        private void siftDownMin(int p)
        {
            // For each child of the parent.
            for (int c = p * 2 + 2; c < count; p = c, c = p * 2 + 2)
            {
                // If the next child is less than the current child, select the next one.
                if (c + 2 < count && distances[c + 2] < distances[c])
                {
                    c += 2;
                }

                // If it is less than our parent swap.
                if (distances[c] < distances[p])
                {
                    swap(p, c);

                    // Swap the pair if necessary.
                    if (c + 1 < count && distances[c + 1] < distances[c])
                    {
                        swap(c, c + 1);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void siftDownMax(int p)
        {
            // For each child on the max side of the tree.
            for (int c = p * 2 + 1; c <= count; p = c, c = p * 2 + 1)
            {
                // If the child is the last one (and only has half a pair).
                if (c == count)
                {
                    // Check if we need to swap with th parent.
                    if (distances[c - 1] > distances[p])
                        swap(p, c - 1);
                    break;
                }

                // If there is only room for a right child lower pair.
                else if (c + 2 == count)
                {
                    // Swap the children.
                    if (distances[c + 1] > distances[c])
                    {
                        // Swap with the parent.
                        if (distances[c + 1] > distances[p])
                            swap(p, c + 1);
                        break;
                    }
                }

                else if (c + 2 < count)
                {
                    // If there is room for a right child upper pair
                    if (distances[c + 2] > distances[c])
                    {
                        c += 2;
                    }
                }

                if (distances[c] > distances[p])
                {
                    swap(p, c);
                    // Swap with pair if necessary
                    if (distances[c - 1] > distances[c])
                    {
                        swap(c, c - 1);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private int swap(int x, int y)
        {
            // Store temp.
            var node = positions[y];
            var dist = distances[y];

            // Swap
            positions[y] = positions[x];
            distances[y] = distances[x];
            positions[x] = node;
            distances[x] = dist;

            // Return.
            return y;
        }

    }
}
