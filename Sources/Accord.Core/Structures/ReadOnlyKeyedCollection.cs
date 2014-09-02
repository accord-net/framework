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

namespace Accord
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;


    /// <summary>
    ///   Read-only keyed dictionary wrapper.
    /// </summary>
    /// 
    /// <remarks>
    ///   This collection implements a read-only keyed collection. Read-only collections
    ///   can not be changed once they are created and are useful for presenting information
    ///  to the user without allowing alteration.
    /// </remarks>
    /// 
    /// <typeparam name="TKey">The types of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// 
    [Serializable]
    public abstract class ReadOnlyKeyedCollection<TKey, TValue> :
        IReadOnlyCollection<TValue>, IDictionary<TKey, TValue>, IList<TValue>
    {

        List<TValue> list;
        Dictionary<TKey, TValue> dictionary;


        /// <summary>
        ///   Initializes a new instance of the 
        ///   <see cref="ReadOnlyKeyedCollection&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        protected ReadOnlyKeyedCollection()
        {
            list = new List<TValue>();
            dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        ///   Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// 
        /// <param name="collection">
        ///   The collection whose elements should be added to the end of this list. 
        ///   The collection itself cannot be null, but it can contain elements that 
        ///   are null, if type T is a reference type.
        /// </param>
        /// 
        protected void AddRange(IList<TValue> collection)
        {
            list.AddRange(collection);
            foreach (var e in collection)
                dictionary.Add(GetKeyForItem(e), e);
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
        ///   Returns true.
        /// </summary>
        /// 
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        ///   Gets the number of elements in the collection.
        /// </summary>
        /// 
        /// <returns>The number of elements in the collection. </returns>
        /// 
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<TValue> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

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
        ///   Gets or sets the element at the specified index.
        /// </summary>
        /// 
        /// <param name="index">The index.</param>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public TValue this[int index]
        {
            get { return list[index]; }
            set
            {
                throw new NotSupportedException("This collection is read-only");
            }
        }


        /// <summary>
        ///   Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// 
        /// <returns>
        ///   The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        /// 
        public int IndexOf(TValue item)
        {
            return list.IndexOf(item);
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
        public bool Contains(TValue item)
        {
            return dictionary.ContainsValue(item);
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
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
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
            foreach (TValue value in list)
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(GetKeyForItem(value), value);
        }





        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Clear()
        {
            throw new NotSupportedException("This collection is read-only");
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
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public void Add(TValue item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        /// <summary>
        ///   This method is not supported, as this is a read-only collection.
        /// </summary>
        /// 
        /// <exception cref="System.NotSupportedException">This collection is read-only</exception>
        /// 
        public bool Remove(TValue item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

    }
}
