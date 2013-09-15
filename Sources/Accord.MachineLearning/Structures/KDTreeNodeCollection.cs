// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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
        private LinkedList<KDTreeNodeDistance<T>> list;

        [NonSerialized]
        private LinkedListNode<KDTreeNodeDistance<T>> farthest;
        [NonSerialized]
        private LinkedListNode<KDTreeNodeDistance<T>> nearest;

        /// <summary>
        ///   Gets or sets the maximum number of elements on this 
        ///   collection, if specified. A value of zero indicates
        ///   this instance has no upper limit of elements.
        /// </summary>
        /// 
        public int Maximum { get; private set; }

        /// <summary>
        ///   Gets the farthest node in the collection (with greatest distance).
        /// </summary>
        /// 
        public KDTreeNodeDistance<T> Farthest
        {
            get { return (farthest == null) ? (KDTreeNodeDistance<T>)null : farthest.Value; }
        }

        /// <summary>
        ///   Gets the nearest node in the collection (with smallest distance).
        /// </summary>
        /// 
        public KDTreeNodeDistance<T> Nearest
        {
            get { return (nearest == null) ? (KDTreeNodeDistance<T>)null : nearest.Value; }
        }

        /// <summary>
        ///   Gets whether this collection has any upper limit
        ///   on the number of elements it can contain.
        /// </summary>
        /// 
        public bool HasMaximumSize { get { return Maximum > 0; } }

        /// <summary>
        ///   Creates a new <see cref="KDTreeNodeCollection&lt;T&gt;"/>.
        /// </summary>
        /// 
        public KDTreeNodeCollection()
        {
            list = new LinkedList<KDTreeNodeDistance<T>>();
        }

        /// <summary>
        ///   Creates a new <see cref="KDTreeNodeCollection&lt;T&gt;"/> with a maximum size.
        /// </summary>
        /// 
        /// <param name="size">The maximum number of elements allowed in this collection.</param>
        /// 
        public KDTreeNodeCollection(int size)
            : this()
        {
            Maximum = size;
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
        public bool TryAdd(KDTreeNode<T> value, double distance)
        {
            // Check if the list has a maximum number of
            // neighbors, and if it has, if it is full
            if (HasMaximumSize && list.Count == Maximum)
            {
                // List is already full. Check if the value
                // to be added is closer than the current
                // farthest point.

                if (distance < farthest.Value.Distance)
                {
                    // Yes, it is closer. Remove the previous
                    // farthest point and replace with this
                    list.Remove(farthest);

                    // Insert at the right place
                    var node = list.AddLast(new KDTreeNodeDistance<T>(value, distance));

                    farthest = node;

                    if (distance < nearest.Value.Distance)
                        nearest = farthest;

                    return true; // a value has been added
                }
                else
                {
                    // The value is even farther
                    return false; // discard it
                }
            }
            else
            {
                // The list still has room for new elements. 
                // Just add the value at the right position.

                var node = list.AddLast(new KDTreeNodeDistance<T>(value, distance));

                // Update node information
                if (farthest == null || distance > farthest.Value.Distance) farthest = node;
                if (nearest == null || distance < nearest.Value.Distance) nearest = node;

                return true; // a value has been added
            }
        }

        /// <summary>
        ///   Gets the <see cref="Accord.MachineLearning.Structures.KDTreeNodeDistance&lt;T&gt;"/> at the specified index.
        /// </summary>
        /// 
        public KDTreeNodeDistance<T> this[int index]
        {
            get { return list.ElementAt(index); }
        }
        /// <summary>
        ///   Removes all elements from this collection.
        /// </summary>
        /// 
        public void Clear()
        {
            list.Clear();
            farthest = null;
            nearest = null;
        }

        /// <summary>
        ///   Gets the number of elements in this collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return list.Count; }
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
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<KDTreeNodeDistance<T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through this collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        ///   Adds the specified item to the collection.
        /// </summary>
        /// 
        /// <param name="item">The item to be added.</param>
        /// 
        public void Add(KDTreeNodeDistance<T> item)
        {
            var node = list.AddLast(item);

            // Update node information
            if (farthest == null || item.Distance > farthest.Value.Distance)
                farthest = node;
            if (nearest == null || item.Distance < nearest.Value.Distance)
                nearest = node;
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
            return list.Contains(item);
        }

        /// <summary>
        ///   Copies the entire collection to a compatible one-dimensional <see cref="System.Array"/>, starting
        ///   at the specified <paramref name="arrayIndex">index</paramref> of the <paramref name="array">target array</paramref>.
        /// </summary>
        /// 
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the
        ///    elements copied from tree. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// 
        public void CopyTo(KDTreeNodeDistance<T>[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// 
        /// <param name="item">The object to remove from the collection. The value can be null for reference types.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if item is successfully removed; otherwise, <c>false</c>. 
        /// </returns>
        /// 
        public bool Remove(KDTreeNodeDistance<T> item)
        {
            var node = list.Find(item);

            if (node != null)
            {
                list.Remove(node);

                // Update node information
                if (farthest == node) farthest = null;
                if (nearest == node) nearest = null;
                return true;
            }

            return false;
        }
    }
}
