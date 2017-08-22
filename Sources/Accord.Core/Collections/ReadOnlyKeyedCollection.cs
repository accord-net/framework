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

namespace Accord.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Compat;

    /// <summary>
    ///   Read-only keyed collection wrapper.
    /// </summary>
    /// 
    /// <remarks>
    ///   This collection implements a read-only keyed collection. Read-only collections
    ///   can not be changed once they are created and are useful for presenting information
    ///   to the user without allowing alteration. A keyed collection is a collection whose 
    ///   elements can be retrieved by key or by index.
    /// </remarks>
    /// 
    /// <typeparam name="TKey">The types of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// 
    [Serializable]
    public abstract class ReadOnlyKeyedCollection<TKey, TValue> 
        : ReadOnlyCollection<TValue>, IDictionary<TKey, TValue>, IList<TValue>
#if !MONO && !NET35
        , IReadOnlyCollection<TValue>
#endif
    {

        Dictionary<TKey, TValue> dictionary;


        /// <summary>
        ///   Initializes a new instance of the 
        ///   <see cref="ReadOnlyKeyedCollection&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        protected ReadOnlyKeyedCollection(IList<TValue> components)
            : base(components)
        {
            dictionary = new Dictionary<TKey, TValue>();
            foreach (var value in components)
                dictionary.Add(GetKeyForItem(value), value);
        }

        /// <summary>
        ///   When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// 
        /// <param name="item">The element from which to extract the key.</param>
        /// 
        /// <returns>The key for the specified element.</returns>
        /// 
        protected abstract TKey GetKeyForItem(TValue item);


        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// 
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }


        /// <summary>
        ///   Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// 
        /// <returns>
        ///   true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
        /// </returns>
        /// 
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        /// 
        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        /// 
        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        /// <summary>
        ///   Gets the value associated with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// 
        /// <returns>
        ///   true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// 
        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        ///   Gets or sets the element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set
            {
                throw new NotSupportedException("This collection is read-only");
            }
        }



        /// <summary>
        ///   Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// 
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        /// <returns>
        ///   true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        /// 
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return (dictionary as IDictionary<TKey, TValue>).Contains(item);
        }


        /// <summary>
        ///   Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// 
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// 
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }


        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Add(KeyValuePair<TValue, TKey> item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public bool Remove(TKey key)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Insert(int index, TValue item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Clear()
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   Returns true.
        /// </summary>
        /// 
        public bool IsReadOnly
        {
            get { return true; }
        }
    }
}
