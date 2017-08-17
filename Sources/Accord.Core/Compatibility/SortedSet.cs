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
    ///   Minimum SortedSet implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    [Serializable, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class SortedSet<T> : ISet<T>, IEnumerable<T>
    {

        private SortedList<T, int> list;


        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public T Max { get; private set; }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public T Min { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SortedSet&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public SortedSet()
        {
            list = new SortedList<T, int>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SortedSet&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public SortedSet(IEnumerable<T> collection)
        {
            list = new SortedList<T, int>();
            foreach (var v in collection)
                this.Add(v);
        }

        /// <summary>
        ///   Adds the specified value.
        /// </summary>
        /// 
        /// <param name="value">The value.</param>
        /// 
        public override void Add(T value)
        {
            if (!list.ContainsKey(value))
            {
                list.Add(value, 0);
                base.Add(value);
            }

            Min = list.Keys[0];
            Max = list.Keys[list.Count - 1];
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override bool Remove(T value)
        {
            if (list.Remove(value))
            {
                base.Remove(value);
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

                return true;
            }

            return false;
        }


        /// <summary>
        ///   Modifies this set to contain all elements both sets.
        /// </summary>
        public void UnionWith(ISet<T> set)
        {
            base.set.UnionWith(set);
            list.Clear();
            Min = default(T);
            Max = default(T);
            foreach (var v in base.set)
                Add(v);
        }

        /// <summary>
        ///   Removes the given elements from this set.
        /// </summary>
        public void ExceptWith(ISet<T> set)
        {
            base.set.ExceptWith(set);
            list.Clear();
            Min = default(T);
            Max = default(T);
            foreach (var v in base.set)
                Add(v);
        }


        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            list.Clear();
            Min = default(T);
            Max = default(T);
            base.Clear();
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        public override IEnumerator<T> GetEnumerator()
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
