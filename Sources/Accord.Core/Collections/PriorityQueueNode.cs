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
// The modifications contained in this class, are, however, available only under the
// LGPL license as stated in the beginning of this file.
//

namespace Accord.Collections
{
    using System;

    /// <summary>
    ///   Represents the node of a priority queue.
    /// </summary>
    /// 
    /// <typeparam name="T">The type for the values stored in the queue.</typeparam>
    /// 
    [Serializable]
    public struct PriorityQueueNode<T> : IComparable<PriorityQueueNode<T>>, IEquatable<PriorityQueueNode<T>>
    {
        [Serializable]
        class Reference
        {
            public int Value;
        }

        double priority;
        T value;
        long insertionIndex;
        Reference queueIndex;


        /// <summary>
        ///   Initializes a new instance of the <see cref="PriorityQueueNode{T}"/> struct.
        /// </summary>
        /// 
        /// <param name="value">The value to store in the node.</param>
        /// <param name="priority">A double value representing the priority for the node.</param>
        /// <param name="index">The index of the node in the priority queue.</param>
        /// <param name="insertionIndex">The original index of the node in the priority queue, at time of first insertion.</param>
        /// 
        public PriorityQueueNode(T value, double priority, int index, long insertionIndex)
        {
            this.priority = priority;
            this.value = value;
            this.insertionIndex = insertionIndex;
            this.queueIndex = new Reference() { Value = index };
        }


        /// <summary>
        ///   Gets a double-value representing the 
        ///   current priority for the node.
        /// </summary>
        /// 
        public double Priority
        {
            get { return priority; }
            internal set { priority = value; }
        }

        /// <summary>
        ///   Gets or sets the current value associated with this node.
        /// </summary>
        /// 
        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        ///   Gets the original position at which this node was inserted.
        /// </summary>
        /// 
        public long InsertionIndex
        {
            get { return insertionIndex; }
        }

        /// <summary>
        ///   Gets the current position of this node in its containing queue.
        /// </summary>
        /// 
        public int QueueIndex
        {
            get { return queueIndex.Value; }
            internal set { queueIndex.Value = value; }
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            if (obj is PriorityQueueNode<T>)
                return Equals((PriorityQueueNode<T>)obj);
            return false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                // Maybe nullity checks, if these are objects not primitives!
                hash = hash * 23 + priority.GetHashCode();
                hash = hash * 23 + value.GetHashCode();
                hash = hash * 23 + insertionIndex.GetHashCode();
                hash = hash * 23 + queueIndex.Value.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this node is empty (does not belong to any queue).
        /// </summary>
        /// 
        public bool IsEmpty
        {
            get { return queueIndex == null; }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(PriorityQueueNode<T> other)
        {
            return Value.Equals(other.value);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(PriorityQueueNode<T> other)
        {
            int order = this.Priority.CompareTo(other.Priority);
            if (order == 0)
                order = this.InsertionIndex.CompareTo(other.InsertionIndex);
            return order;
        }

        /// <summary>
        ///   Implements the equals operator.
        /// </summary>
        /// 
        public static bool operator ==(PriorityQueueNode<T> a, PriorityQueueNode<T> b)
        {
            return a.priority == b.priority && a.InsertionIndex == b.InsertionIndex && a.Value.Equals(b.Value);
        }

        /// <summary>
        ///   Implements the not equals operator.
        /// </summary>
        /// 
        public static bool operator !=(PriorityQueueNode<T> a, PriorityQueueNode<T> b)
        {
            return a.priority != b.priority || a.InsertionIndex != b.InsertionIndex || !a.Value.Equals(b.Value);
        }

        /// <summary>
        ///   Implements the greater than operator.
        /// </summary>
        /// 
        public static bool operator >(PriorityQueueNode<T> a, PriorityQueueNode<T> b)
        {
            if (a.Priority == b.Priority)
                return a.InsertionIndex > b.InsertionIndex;
            return a.Priority > b.Priority;
        }

        /// <summary>
        ///   Implements the less than operator.
        /// </summary>
        /// 
        public static bool operator <(PriorityQueueNode<T> a, PriorityQueueNode<T> b)
        {
            if (a.Priority == b.Priority)
                return a.InsertionIndex < b.InsertionIndex;
            return a.Priority < b.Priority;
        }

        /// <summary>
        ///   Gets an instance representing an empty node.
        /// </summary>
        /// 
        public static readonly PriorityQueueNode<T> Empty = new PriorityQueueNode<T>();

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            if (this == Empty)
                return "None";

            return String.Format("Priority: {0}, Value: {1}, Order: {2}, Index: {3}",
                priority,
                value,
                insertionIndex,
                queueIndex.Value);
        }
    }
}
