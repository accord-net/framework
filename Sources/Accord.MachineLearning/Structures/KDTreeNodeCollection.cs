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
    using AForge;

    /// <summary>
    ///   Collection of k-dimensional tree nodes.
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
    public class KDTreeNodeCollection<T> : ICollection<KDTreeNodeDistance<T>>
    {
        SortedSet<double> distances;
        Dictionary<double, List<KDTreeNode<T>>> positions;

        DoubleRange range;
        int count;


        /// <summary>
        ///   Gets or sets the maximum number of elements on this 
        ///   collection, if specified. A value of zero indicates
        ///   this instance has no upper limit of elements.
        /// </summary>
        /// 
        public int Capacity { get; private set; }

        /// <summary>
        ///   Gets the current distance limits for nodes contained
        ///   at this collection, such as the maximum and minimum
        ///   distances.
        /// </summary>
        /// 
        public DoubleRange Distance
        {
            get { return range; }
        }

        /// <summary>
        ///   Gets the farthest node in the collection (with greatest distance).
        /// </summary>
        /// 
        public KDTreeNode<T> Farthest
        {
            get { return positions[range.Max][0]; }
        }

        /// <summary>
        ///   Gets the nearest node in the collection (with smallest distance).
        /// </summary>
        /// 
        public KDTreeNode<T> Nearest
        {
            get { return positions[range.Min][0]; }
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
            distances = new SortedSet<double>();
            positions = new Dictionary<double, List<KDTreeNode<T>>>();
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
        public bool Add(KDTreeNode<T> value, double distance)
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

            if (distance < range.Max)
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
        public bool AddFarthest(KDTreeNode<T> value, double distance)
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

            if (distance > range.Min)
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
        private void add(double distance, KDTreeNode<T> item)
        {
            List<KDTreeNode<T>> position;

            if (!positions.TryGetValue(distance, out position))
                positions.Add(distance, position = new List<KDTreeNode<T>>());

            position.Add(item);
            distances.Add(distance);

            if (count == 0)
                range.Max = range.Min = distance;

            else
            {
                if (distance > range.Max)
                    range.Max = distance;
                if (distance < range.Min)
                    range.Min = distance;
            }


            count++;
        }


        /// <summary>
        ///   Removes all elements from this collection.
        /// </summary>
        /// 
        public void Clear()
        {
            distances.Clear();
            positions.Clear();

            count = 0;
            range.Max = 0;
            range.Min = 0;
        }


        /// <summary>
        ///   Gets the list of <see cref="KDTreeNode{T}"/> that holds the
        ///   objects laying out at the specified distance, if there is any.
        /// </summary>
        /// 
        public List<KDTreeNode<T>> this[double distance]
        {
            get
            {
                List<KDTreeNode<T>> position;
                if (!positions.TryGetValue(distance, out position))
                    return null;

                return position;
            }
        }

        /// <summary>
        ///   Gets the <see cref="Accord.MachineLearning.Structures.KDTreeNodeDistance{T}"/>
        ///   at the specified index. Note: this method will iterate over the entire collection
        ///   until the given position is found.
        /// </summary>
        /// 
        public KDTreeNodeDistance<T> this[int index]
        {
            get { return this.ElementAt(index); }
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
        public IEnumerator<KDTreeNodeDistance<T>> GetEnumerator()
        {
            foreach (var position in positions)
            {
                double distance = position.Key;
                foreach (var node in position.Value)
                    yield return new KDTreeNodeDistance<T>(node, distance);
            }

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
            return positions.GetEnumerator();
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
        public bool Contains(KDTreeNodeDistance<T> item)
        {
            List<KDTreeNode<T>> position;
            if (positions.TryGetValue(item.Distance, out position))
                return position.Contains(item.Node);

            return false;
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
        public void CopyTo(KDTreeNodeDistance<T>[] array, int arrayIndex)
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
        public void Add(KDTreeNodeDistance<T> item)
        {
            Add(item.Node, item.Distance);
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// 
        /// <param name="item">The object to remove from the collection. 
        /// The value can be null for reference types.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if item is successfully removed; otherwise, <c>false</c>. 
        /// </returns>
        /// 
        public bool Remove(KDTreeNodeDistance<T> item)
        {
            List<KDTreeNode<T>> position;
            if (!positions.TryGetValue(item.Distance, out position))
                return false;

            if (!position.Remove(item.Node))
                return false;

            range.Max = distances.Max;
            range.Min = distances.Min;
            count--;

            return true;
        }

        /// <summary>
        ///   Removes the farthest tree node from this collection.
        /// </summary>
        /// 
        public void RemoveFarthest()
        {
            List<KDTreeNode<T>> position = positions[range.Max];

            position.RemoveAt(0);

            if (position.Count() == 0)
            {
                distances.Remove(range.Max);
                range.Max = distances.Max;
            }

            count--;
        }

        /// <summary>
        ///   Removes the nearest tree node from this collection.
        /// </summary>
        /// 
        public void RemoveNearest()
        {
            List<KDTreeNode<T>> position = positions[range.Min];

            position.RemoveAt(0);

            if (position.Count() == 0)
            {
                distances.Remove(range.Min);
                range.Min = distances.Min;
            }

            count--;
        }

    }
}
