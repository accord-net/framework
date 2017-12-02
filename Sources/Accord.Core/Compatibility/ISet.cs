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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;
    using System.Collections;

    /// <summary>
    ///   Minimum ISet implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [Serializable]
    public class ISet<T> : ICollection<T>
    {
        internal HashSet<T> set;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ISet&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public ISet()
        {
            this.set = new HashSet<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISet&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="set">The set.</param>
        /// 
        public ISet(HashSet<T> set)
        {
            this.set = set;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> to ISet.
        /// </summary>
        /// 
        /// <param name="set">The set.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
        public static implicit operator ISet<T>(HashSet<T> set)
        {
            return new ISet<T>(set);
        }

        /// <summary>
        ///   Performs an implicit conversion from ISet to <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="set">The set.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
        public static implicit operator HashSet<T>(ISet<T> set)
        {
            return set.set;
        }

        /// <summary>
        ///   Adds the specified item.
        /// </summary>
        /// 
        /// <param name="item">The item.</param>
        /// 
        public virtual void Add(T item)
        {
            set.Add(item);
        }

        /// <summary>
        ///   Modifies the current <see cref="ISet{T}"/> object to contain all elements 
        ///   that are present in itself, the specified collection, or both.
        /// </summary>
        /// 
        public virtual void UnionWith(IEnumerable<T> items)
        {
            foreach (T item in items)
                set.Add(item);
        }

        /// <summary>
        ///   Clears this instance.
        /// </summary>
        /// 
        public virtual void Clear()
        {
            set.Clear();
        }

        /// <summary>
        ///   Determines whether this instance contains the specified item.
        /// </summary>
        /// 
        /// <param name="item">The item.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the set contains the specified item; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        /// <summary>
        /// Copies the elements of this set to an array.
        /// </summary>
        /// 
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// 
        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Gets the number of elements in this set.
        /// </summary>
        /// 
        public int Count
        {
            get { return set.Count; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is read only.
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
        ///   Removes the specified item.
        /// </summary>
        /// 
        /// <param name="item">The item.</param>
        /// 
        public virtual bool Remove(T item)
        {
            return set.Remove(item);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public virtual IEnumerator<T> GetEnumerator()
        {
            return set.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return set.GetEnumerator();
        }

        /// <summary>
        ///   Determines whether this set contains the 
        ///   exact same elements as another set.
        /// </summary>
        /// 
        /// <param name="set">The other set.</param>
        /// 
        public bool SetEquals(ISet<T> set)
        {
            foreach (var e in set)
                if (!this.Contains(e))
                    return false;

            foreach (var e in this)
                if (!set.Contains(e))
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether this instance is a subset of the specified set.
        /// </summary>
        public bool IsSubsetOf(IEnumerable<T> set)
        {
            return this.set.IsSubsetOf(set);
        }

    }
}
#endif
