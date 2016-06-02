// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
namespace Accord
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Minimum Lazy implementation for .NET 3.5 to make
    ///   Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    public class ConcurrentBag<T> : IEnumerable<T>
    {
        private LinkedList<T> list;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConcurrentBag{T}"/> class.
        /// </summary>
        /// 
        public ConcurrentBag()
        {
            list = new LinkedList<T>();
        }

        /// <summary>
        ///   Adds the specified item.
        /// </summary>
        /// 
        public void Add(T item)
        {
            lock (list)
                list.AddLast(item);
        }

        /// <summary>
        ///   Counts this instance.
        /// </summary>
        /// 
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
#endif
