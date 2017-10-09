// Accord Core Library
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
// The code for this class has been based on the original "High-speed Priority Queue"
// project originally developed by Daniel "Blue Raja" Pflughoeft. It was originally
// shared under a free MIT license, as shown below:
//
//    The MIT License (MIT)
//      
//    Copyright (c) 2013 Daniel "BlueRaja" Pflughoeft
//    https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
//      
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//    THE SOFTWARE.
// 
// The modifications contained in this class, are, however, available under the
// free software LGPL license as stated in the beginning of this file.
//

namespace Accord.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Accord.Compat;

    /// <summary>
    ///   Priority order for <see cref="PriorityQueue{T}"/>.
    /// </summary>
    /// 
    public enum PriorityOrder
    {
        /// <summary>
        ///   Minimum order. In Minimum Priority Queues, items with smaller
        ///   priority numbers receive higher priority and are the ones that will
        ///   be dequeued first (i.e. similar to NICE number in UNIX systems).
        /// </summary>
        /// 
        Minimum,

        /// <summary>
        ///   MAximum order. In Maximum Priority Queues, items with higher
        ///   priority numbers receive higher priority and are the ones that will
        ///   be dequeued first.
        /// </summary>
        /// 
        Maximum
    }

    /// <summary>
    ///   Priority queue.
    /// </summary>
    /// 
    /// <typeparam name="T">The values in the queue.</typeparam>
    /// 
    /// <remarks>
    ///   The code for this class has been based on the original "High-speed Priority Queue"
    ///   project originally developed by Daniel "Blue Raja" Pflughoeft. It was originally
    ///   shared under a free MIT license, as shown below:
    ///   <code>
    ///      The MIT License (MIT)
    ///        
    ///      Copyright (c) 2013 Daniel "BlueRaja" Pflughoeft
    ///      https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    ///        
    ///      Permission is hereby granted, free of charge, to any person obtaining a copy
    ///      of this software and associated documentation files (the "Software"), to deal
    ///      in the Software without restriction, including without limitation the rights
    ///      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ///      copies of the Software, and to permit persons to whom the Software is
    ///      furnished to do so, subject to the following conditions:
    ///      
    ///      The above copyright notice and this permission notice shall be included in
    ///      all copies or substantial portions of the Software.
    ///      
    ///      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ///      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ///      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ///      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ///      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ///      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    ///      THE SOFTWARE.
    ///   </code>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class PriorityQueue<T> : IEnumerable<PriorityQueueNode<T>>
    {
        private int numberOfNodes;
        private PriorityQueueNode<T>[] nodes;
        private long counter;

        PriorityOrder order = PriorityOrder.Minimum;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PriorityQueue{T}"/> class.
        /// </summary>
        /// 
        /// <param name="capacity">The initial capacity for the queue.</param>
        /// <param name="order">The sort order for the queue. If set to <see cref="PriorityOrder.Minimum"/>, 
        ///   items that have a smaller priority number are the next to be dequeued. If set to 
        ///   <see cref="PriorityOrder.Maximum"/>, items with larger priority numbers are the ones
        ///   with higher priority and will be the next to be dequeued. Default is <see cref="PriorityOrder.Minimum"/>.</param>
        /// 
        /// <exception cref="System.InvalidOperationException">New queue size cannot be smaller than 1.</exception>
        /// 
        public PriorityQueue(int capacity = 10, PriorityOrder order = PriorityOrder.Minimum)
        {
#if DEBUG
            if (capacity <= 0)
                throw new InvalidOperationException("New queue size cannot be smaller than 1.");
#endif
            this.numberOfNodes = 0;
            this.nodes = new PriorityQueueNode<T>[capacity + 1];
            this.counter = 0;
            this.order = order;
        }

        /// <summary>
        ///   Gets the number of nodes in the queue. This is an O(1) operation.
        /// </summary>
        /// 
        public int Count
        {
            get { return numberOfNodes; }
        }

        /// <summary>
        ///   Gets the current capacity of this queue.
        /// </summary>
        /// 
        public int Capacity
        {
            get { return nodes.Length - 1; }
        }

        /// <summary>
        ///   Gets or sets the <see cref="PriorityOrder">ordering of this priority queue</see>. 
        /// </summary>
        /// 
        public PriorityOrder Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        ///   Removes every node from the queue. This is an O(1) operation.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            numberOfNodes = 0;
        }

        /// <summary>
        ///     Returns whether the given node is in the queue. This is an O(1) operation.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(PriorityQueueNode<T> node)
        {
#if DEBUG
            if (node.QueueIndex < 0 || node.QueueIndex >= nodes.Length)
                throw new InvalidOperationException("node.QueueIndex has been corrupted. Did you change it manually? Or add this node to another queue?");
#endif
            return nodes[node.QueueIndex] == node;
        }

        /// <summary>
        ///   Enqueue a node to the priority queue. Lower values are placed in front.
        ///   Ties are broken by first-in-first-out. This is an O(log n) operation.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PriorityQueueNode<T> Enqueue(T value, double priority)
        {
            CheckQueue();

            numberOfNodes++;
            if (numberOfNodes >= nodes.Length)
                Resize(nodes.Length + (int)(0.1 * nodes.Length));

            var node = new PriorityQueueNode<T>(value, priority, numberOfNodes, counter++);
            nodes[numberOfNodes] = node;
            cascadeUp(ref nodes[numberOfNodes]);

            CheckQueue();
            return node;
        }

        [Conditional("DEBUG")]
        private void CheckQueue()
        {
            if (!IsValidQueue())
            {
                throw new InvalidOperationException("Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same node to two different queues?)");
            }
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void swap(int i, int j)
        {
            // Swap the nodes
            var aux = nodes[i];
            nodes[i] = nodes[j];
            nodes[j] = aux;

            // Re-assign the indices
            nodes[i].QueueIndex = i;
            nodes[j].QueueIndex = j;
        }

        private void cascadeUp(ref PriorityQueueNode<T> node)
        {
            // aka Heapify-up
            int current = node.QueueIndex;
            int parent = node.QueueIndex / 2;
            while (parent >= 1)
            {
                if (HasHigherPriority(parent, current))
                    break;

                // Node has lower priority value, so move it up the heap
                swap(current, parent);
                current = nodes[parent].QueueIndex;
                parent = current / 2;
            }
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void cascadeDown(ref PriorityQueueNode<T> node)
        {
            // aka Heapify-down
            int newParent;
            int finalQueueIndex = node.QueueIndex;

            while (true)
            {
                newParent = nodes[finalQueueIndex].QueueIndex;
                int childLeftIndex = 2 * finalQueueIndex;

                // Check if the left-child is higher-priority than the current node
                if (childLeftIndex > numberOfNodes)
                    break;

                if (HasHigherPriority(childLeftIndex, newParent))
                    newParent = childLeftIndex;

                // Check if the right-child is higher-priority than either the current node or the left child
                int childRightIndex = childLeftIndex + 1;
                if (childRightIndex <= numberOfNodes)
                {
                    if (HasHigherPriority(childRightIndex, newParent))
                        newParent = childRightIndex;
                }

                // If either of the children has higher (smaller) priority, swap and continue cascading
                if (newParent != finalQueueIndex)
                {
                    swap(finalQueueIndex, newParent);
                    finalQueueIndex = newParent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        ///   Returns true if 'higher' has higher priority than 'lower', false otherwise. Note that
        ///   calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherPriority(int i, int j)
        {
            var a = nodes[i];
            var b = nodes[j];

            if (order == PriorityOrder.Minimum)
                return a.Priority < b.Priority || (a.Priority == b.Priority && a.InsertionIndex < b.InsertionIndex);
            return a.Priority > b.Priority || (a.Priority == b.Priority && a.InsertionIndex < b.InsertionIndex);
        }


        /// <summary>
        ///   Removes the head of the queue (node with minimum priority; ties are broken by order
        ///   of insertion), and returns it. This is an O(log n) operation.
        /// </summary>
        /// 
        public PriorityQueueNode<T> Dequeue()
        {
#if DEBUG
            if (numberOfNodes <= 0)
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
#endif

            CheckQueue();
            var returnMe = nodes[1];
            Remove(returnMe);
            CheckQueue();
            return returnMe;
        }

        /// <summary>
        ///   Resize the queue so it can accept more nodes.  All currently enqueued nodes are kept.
        ///   Attempting to decrease the queue size to a size too small to hold the existing nodes
        ///   results in undefined behavior. This is an O(n) operation.
        /// </summary>
        /// 
        public void Resize(int capacity)
        {
#if DEBUG
            if (capacity <= 0)
                throw new InvalidOperationException("Queue size cannot be smaller than 1");

            if (capacity < numberOfNodes)
                throw new InvalidOperationException("Called Resize(" + capacity + "), but current queue contains " + numberOfNodes + " nodes");
#endif

            var newArray = new PriorityQueueNode<T>[capacity + 1];
            int highestIndexToCopy = Math.Min(capacity, numberOfNodes);
            for (int i = 1; i <= highestIndexToCopy; i++)
                newArray[i] = nodes[i];

            nodes = newArray;
        }

        /// <summary>
        ///   Returns the head of the queue, without removing it (use Dequeue() for that).
        ///   If the queue is empty, behavior is undefined. This is an O(1) operation.
        /// </summary>
        /// 
        public PriorityQueueNode<T> First
        {
            get
            {
#if DEBUG
                if (numberOfNodes <= 0)
                    throw new InvalidOperationException("Cannot call .First on an empty queue");
#endif

                return nodes[1];
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is read only (returns false).
        /// </summary>
        /// 
        /// <value>
        ///   Returns <c>false</c>, as instances of this class are not read only.
        /// </value>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   This method must be called on a node every time its priority changes while it is in the queue.  
        ///   <b>Forgetting to call this method will result in a corrupted queue!</b>. This is an O(log n) operation.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void UpdatePriority(ref PriorityQueueNode<T> node, double priority)
        {
#if DEBUG
            if (!Contains(node))
                throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + node);
#endif
            node.Priority = priority;
            OnNodeUpdated(ref node);
        }

        private void OnNodeUpdated(ref PriorityQueueNode<T> node)
        {
            // Bubble the updated node up or down as appropriate
            int parentIndex = node.QueueIndex / 2;

            if (parentIndex > 0 && HasHigherPriority(node.QueueIndex, parentIndex))
            {
                cascadeUp(ref node);
            }
            else
            {
                cascadeDown(ref node);
            }
        }

        /// <summary>
        ///   Removes a node from the queue. The node does not need to be the head of the
        ///   queue. This is an O(log n) operation.
        /// </summary>
        /// 
        public void Remove(PriorityQueueNode<T> node)
        {
#if DEBUG
            if (!Contains(node))
                throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + node);
#endif
            CheckQueue();

            // If the node is already the last node, we can remove it immediately
            if (node.QueueIndex == numberOfNodes)
            {
                nodes[numberOfNodes] = PriorityQueueNode<T>.Empty;
                numberOfNodes--;
                return;
            }

            int index = node.QueueIndex;
            // Swap the node with the last node
            swap(node.QueueIndex, numberOfNodes);
            nodes[numberOfNodes] = PriorityQueueNode<T>.Empty;
            numberOfNodes--;

            // Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
            OnNodeUpdated(ref nodes[index]);

            CheckQueue();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<PriorityQueueNode<T>> GetEnumerator()
        {
            for (int i = 1; i <= numberOfNodes; i++)
                yield return nodes[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Returns an array containing the items in this list, 
        ///   optionally in in priority order.
        /// </summary>
        /// 
        /// <param name="sorted">Whether to return the items in priority order.</param>
        /// 
        public PriorityQueueNode<T>[] ToArray(bool sorted = true)
        {
            var result = new PriorityQueueNode<T>[numberOfNodes];
            for (int i = 0; i < result.Length; i++)
                result[i] = nodes[i + 1];
            if (sorted)
                Array.Sort(result);
            return result;
        }


        /// <summary>
        ///   Checks to make sure the queue is still in a valid state.
        /// </summary>
        /// 
        public bool IsValidQueue()
        {
            for (int i = 1; i < nodes.Length; i++)
            {
                if (!nodes[i].IsEmpty)
                {
                    if (checkChildren(i, 2 * i))
                        return false;

                    if (checkChildren(i, 2 * i + 1))
                        return false;
                }
            }

            return true;
        }

        private bool checkChildren(int currentIndex, int childIndex)
        {
            if (childIndex >= nodes.Length)
                return false;

            if (nodes[childIndex].IsEmpty)
                return false;

            if (!HasHigherPriority(childIndex, currentIndex))
                return false;

            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var values = this.ToArray();

            var sb = new StringBuilder();

            sb.Append("{");
            for (int i = 0; i < values.Length; i++)
            {
                sb.Append(values[i]);
                if (i < values.Length - 1)
                    sb.AppendLine(", ");
            }
            sb.Append("}");

            return sb.ToString();
        }
    }
}