// Accord Core Library
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

#if NET35
namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Minimum SortedSet implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    [Serializable, System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    internal class SortedSet<T> : IEnumerable<T>
    {

        private SortedList<T, int> list;


        public T Max { get; set; }

        public T Min { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SortedSet&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public SortedSet()
        {
            list = new SortedList<T, int>();
        }

        /// <summary>
        ///   Determines whether the set contains the specified value.
        /// </summary>
        /// 
        /// <param name="value">The value.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if this object contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Contains(T value)
        {
            return list.ContainsKey(value);
        }

        /// <summary>
        ///   Adds the specified value.
        /// </summary>
        /// 
        /// <param name="value">The value.</param>
        /// 
        public void Add(T value)
        {
            if (!list.ContainsKey(value))
                list.Add(value, 0);

            Min = list.Keys[0];
            Max = list.Keys[list.Count - 1];
        }

        public void Remove(T value)
        {
            if (list.Remove(value))
            {
                if (list.Count > 0)
                {
                    Min = list.Keys[0];
                    Max = list.Keys[list.Count - 1];
                }
                else
                {
                    Min = default(T);
                    Max = default(T);
                }
            }
        }

        public void Clear()
        {
            list.Clear();
            Min = default(T);
            Max = default(T);
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in list)
                yield return item.Key;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif
