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
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    ///   Ordered dictionary.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class provides a ordered dictionary implementation for C#/.NET. Unlike the rest
    ///   of the framework, this class is available under a MIT license, so please feel free to
    ///   re-use its source code in your own projects.
    /// </remarks>
    /// 
    /// <typeparam name="TKey">The types of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// 
    [Serializable]
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private List<TKey> list;
        private Dictionary<TKey, TValue> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        public OrderedDictionary()
        {
            this.dictionary = new Dictionary<TKey, TValue>();
            this.list = new List<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedDictionary{TKey, TValue}"/> can contain.</param>
        /// 
        public OrderedDictionary(int capacity)
        {
            this.dictionary = new Dictionary<TKey, TValue>(capacity);
            this.list = new List<TKey>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="comparer">The IEqualityComparer implementation to use when comparing keys, or null to use 
        ///     the default EqualityComparer for the type of the key.</param>
        /// 
        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(comparer);
            this.list = new List<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedDictionary{TKey, TValue}"/> can contain.</param>
        /// <param name="comparer">The IEqualityComparer implementation to use when comparing keys, or null to use 
        ///     the default EqualityComparer for the type of the key.</param>
        /// 
        public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            this.list = new List<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="dictionary">The System.Collections.Generic.IDictionary`2 whose elements are copied to the
        ///     new <see cref="OrderedDictionary{TKey, TValue}"/>.</param>
        /// <param name="comparer">The IEqualityComparer implementation to use when comparing keys, or null to use 
        ///     the default EqualityComparer for the type of the key.</param>
        /// 

        public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
            this.list = new List<TKey>();
        }

        /// <summary>
        ///   Gets the <typeparam ref="TValue"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="index">The index.</param>
        /// 
        public TKey GetKeyByIndex(int index)
        {
            return list[index];
        }

        /// <summary>
        ///   Gets the <typeparam ref="TValue"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="index">The index.</param>
        /// 
        public TValue GetValueByIndex(int index)
        {
            return this[GetKeyByIndex(index)];
        }

        /// <summary>
        ///   Gets or sets the <typeparam ref="TValue"/> with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// 
        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set
            {
                dictionary[key] = value;
                if (!list.Contains(key))
                    list.Add(key);
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <value>The keys.</value>
        /// 
        public ICollection<TKey> Keys
        {
            get { return list; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        public ICollection<TValue> Values
        {
            get { return list.Select(x => dictionary[x]).ToList(); }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        public int Count
        {
            get { return dictionary.Count;}
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        /// 
        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>)dictionary).IsReadOnly; }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// 
        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            if (!list.Contains(key))
                list.Add(key);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)dictionary).Add(item);
            if (!list.Contains(item.Key))
                list.Add(item.Key);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        public void Clear()
        {
            dictionary.Clear();
            list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// 
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// 
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)dictionary).Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// 
        /// <returns>true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.</returns>
        /// 
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// 
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// 
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> pair in this)
                array[arrayIndex++] = pair;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// 
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (TKey key in list)
                yield return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The key of the element to remove.</param>
        /// 
        /// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        /// 
        public bool Remove(TKey key)
        {
            if (dictionary.Remove(key))
            {
                list.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// 
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((IDictionary<TKey, TValue>)dictionary).Remove(item))
            {
                list.Remove(item.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// 
        /// <returns>true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.</returns>
        /// 
        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>)dictionary).TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
