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

#if NET35
namespace Accord.Compat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Minimum Lazy implementation for .NET 3.5 to make
    ///   Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    public class ConcurrentBag<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection
    {
        private LinkedList<T> list;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConcurrentBag{T}"/> class.
        /// </summary>
        /// 
        public ConcurrentBag()
        {
            this.list = new LinkedList<T>();
        }

        /// <summary>
        ///   Adds the specified item.
        /// </summary>
        /// 
        public void Add(T item)
        {
            lock (SyncRoot)
                list.AddLast(item);
        }

        /// <summary>
        ///   Counts this instance.
        /// </summary>
        /// 
        public int Count
        {
            get
            {
                lock (SyncRoot)
                    return list.Count;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// 
        public object SyncRoot
        {
            get { return ((ICollection)list).SyncRoot; }
        }

        /// <summary>
        ///   Returns true.
        /// </summary>
        /// 
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        public IEnumerator<T> GetEnumerator()
        {
            lock (SyncRoot)
                return list.GetEnumerator();
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (SyncRoot)
                return list.GetEnumerator();
        }

        /// <summary>
        ///   Copies the elements in the list to an array.
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            lock (SyncRoot)
                ((ICollection)list).CopyTo(array, index);
        }

        /// <summary>
        ///   Copies all elements to an array.
        /// </summary>
        /// 
        public T[] ToArray()
        {
            lock (SyncRoot)
            {
                var array = new T[list.Count];
                int i = 0;
                foreach (T item in list)
                    array[i++] = item;
                return array;
            }
        }
    }
}

namespace System.Collections.Concurrent
{
    internal class Dummy
    {

    }
}

#endif
