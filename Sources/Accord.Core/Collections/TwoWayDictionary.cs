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
// Based on the BiDictionary implementation by Alexander Prokhorov. Available
// under the public domain at GitHub, under the project name Alba.Framework.
// https://github.com/Athari/Alba.Framework/blob/master/Alba.Framework/Collections/Collections/BiDictionary(TFirst%2CTSecond).cs
//

namespace Accord.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;

    /// <summary>
    ///   Two-way dictionary for efficient lookups by both key and value. This
    ///   can be used to represent a one-to-one relation among two object types.
    /// </summary>
    /// 
    /// <typeparam name="TFirst">The type of right keys in the dictionary.</typeparam>
    /// <typeparam name="TSecond">The type of left keys in the dictionary.</typeparam>
    /// 
    [Serializable]
    public sealed class TwoWayDictionary<TFirst, TSecond> : IDictionary<TFirst, TSecond>,
        IReadOnlyDictionary<TFirst, TSecond>, IDictionary
    {
        private IDictionary<TFirst, TSecond> firstToSecond;

        [NonSerialized]
        private IDictionary<TSecond, TFirst> secondToFirst;

        [NonSerialized]
        private ReverseDictionary reverse;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TwoWayDictionary{TFirst, TSecond}"/> class
        ///   that is empty, has the default initial capacity, and uses the default equality comparer
        ///   for the key type.
        /// </summary>
        /// 
        public TwoWayDictionary()
        {
            this.firstToSecond = new Dictionary<TFirst, TSecond>();
            this.secondToFirst = new Dictionary<TSecond, TFirst>();
            this.reverse = new ReverseDictionary(this);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TwoWayDictionary{TFirst, TSecond}"/> class
        ///   that is empty, has the specified initial capacity, and uses the default equality comparer
        ///   for the key type.
        /// </summary>
        /// 
        /// <param name="capacity">The initial number of elements that this dictionary can contain.</param>
        /// 
        public TwoWayDictionary(int capacity)
        {
            this.firstToSecond = new Dictionary<TFirst, TSecond>(capacity);
            this.secondToFirst = new Dictionary<TSecond, TFirst>(capacity);
            this.reverse = new ReverseDictionary(this);
        }

        /// <summary> 
        ///   Initializes a new instance of the <see cref="TwoWayDictionary{TFirst, TSecond}"/> class
        ///   that contains elements copied from the specified dictionary and uses the default equality
        ///   comparer for the key type.
        /// </summary>
        /// 
        /// <param name="dictionary">The dictionary whose elements are copied to the new <see cref="TwoWayDictionary{TFirst, TSecond}"/>.</param>
        /// 
        public TwoWayDictionary(IDictionary<TFirst, TSecond> dictionary)
        {
            this.firstToSecond = new Dictionary<TFirst, TSecond>(dictionary);
            this.secondToFirst = new Dictionary<TSecond, TFirst>();

            foreach (var value in dictionary)
                this.secondToFirst.Add(value.Value, value.Key);

            this.reverse = new ReverseDictionary(this);
        }

        /// <summary>
        ///   Gets the reverse dictionary that maps values back to keys.
        /// </summary>
        /// 
        public IDictionary<TSecond, TFirst> Reverse
        {
            get { return reverse; }
        }

        /// <summary>
        ///   Gets the number of elements contained in this <see cref="TwoWayDictionary{TFirst, TSecond}"/>.
        /// </summary>
        /// 
        public int Count
        {
            get { return firstToSecond.Count; }
        }

        /// <summary>
        ///   Gets an object that can be used to synchronize access to the <see cref="TwoWayDictionary{TFirst, TSecond}"/>.
        /// </summary>
        /// 
        object ICollection.SyncRoot
        {
            get { return ((ICollection)firstToSecond).SyncRoot; }
        }

        /// <summary>
        ///   Gets a value indicating whether access to the <see cref="TwoWayDictionary{TFirst, TSecond}"/> is synchronized (thread safe).
        /// </summary>
        /// 
        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)firstToSecond).IsSynchronized; }
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.
        /// </summary>
        /// 
        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)firstToSecond).IsFixedSize; }
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// 
        public bool IsReadOnly
        {
            get { return firstToSecond.IsReadOnly || secondToFirst.IsReadOnly; }
        }

        /// <summary>
        ///   Gets or sets the element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The left key.</param>
        /// 
        public TSecond this[TFirst key]
        {
            get { return firstToSecond[key]; }
            set
            {
                firstToSecond[key] = value;
                secondToFirst[value] = key;
            }
        }

        /// <summary>
        ///   Gets or sets the element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The left key.</param>
        /// 
        object IDictionary.this[object key]
        {
            get { return ((IDictionary)firstToSecond)[key]; }
            set
            {
                ((IDictionary)firstToSecond)[key] = value;
                ((IDictionary)secondToFirst)[value] = key;
            }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        public ICollection<TFirst> Keys
        {
            get { return firstToSecond.Keys; }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)firstToSecond).Keys; }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        IEnumerable<TFirst> IReadOnlyDictionary<TFirst, TSecond>.Keys
        {
            get { return ((IReadOnlyDictionary<TFirst, TSecond>)firstToSecond).Keys; }
        }

        /// <summary>
        ///  Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        public ICollection<TSecond> Values
        {
            get { return firstToSecond.Values; }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        ICollection IDictionary.Values
        {
            get { return ((IDictionary)firstToSecond).Values; }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        IEnumerable<TSecond> IReadOnlyDictionary<TFirst, TSecond>.Values
        {
            get { return ((IReadOnlyDictionary<TFirst, TSecond>)firstToSecond).Values; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            return firstToSecond.GetEnumerator();
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
            return GetEnumerator();
        }

        /// <summary>
        ///   Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.
        /// </returns>
        /// 
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)firstToSecond).GetEnumerator();
        }

        /// <summary>
        ///   Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// 
        public void Add(TFirst key, TSecond value)
        {
            firstToSecond.Add(key, value);
            secondToFirst.Add(value, key);
        }

        /// <summary>
        ///   Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// 
        /// <param name="key">The <see cref="T:System.Object" /> to use as the key of the element to add.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to use as the value of the element to add.</param>
        /// 
        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)firstToSecond).Add(key, value);
            ((IDictionary)secondToFirst).Add(value, key);
        }

        /// <summary>
        ///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        void ICollection<KeyValuePair<TFirst, TSecond>>.Add(KeyValuePair<TFirst, TSecond> item)
        {
            firstToSecond.Add(item);
            secondToFirst.Add(item.Value, item.Key);
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
        public bool ContainsKey(TFirst key)
        {
            return firstToSecond.ContainsKey(key);
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
        bool ICollection<KeyValuePair<TFirst, TSecond>>.Contains(KeyValuePair<TFirst, TSecond> item)
        {
            return firstToSecond.Contains(item);
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
        public bool TryGetValue(TFirst key, out TSecond value)
        {
            return firstToSecond.TryGetValue(key, out value);
        }

        /// <summary>
        ///   Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The key of the element to remove.</param>
        /// 
        /// <returns>
        ///   true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        /// 
        public bool Remove(TFirst key)
        {
            TSecond value;
            if (firstToSecond.TryGetValue(key, out value))
            {
                firstToSecond.Remove(key);
                secondToFirst.Remove(value);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        ///   Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// 
        /// <param name="key">The key of the element to remove.</param>
        /// 
        void IDictionary.Remove(object key)
        {
            var firstToSecond = (IDictionary)this.firstToSecond;
            if (!firstToSecond.Contains(key))
                return;
            var value = firstToSecond[key];
            firstToSecond.Remove(key);
            ((IDictionary)secondToFirst).Remove(value);
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        /// <returns>
        ///   true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// 
        bool ICollection<KeyValuePair<TFirst, TSecond>>.Remove(KeyValuePair<TFirst, TSecond> item)
        {
            return firstToSecond.Remove(item);
        }

        /// <summary>
        ///   Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.</param>
        /// 
        /// <returns>
        ///  true if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.
        /// </returns>
        /// 
        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)firstToSecond).Contains(key);
        }

        /// <summary>
        ///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        public void Clear()
        {
            firstToSecond.Clear();
            secondToFirst.Clear();
        }

        void ICollection<KeyValuePair<TFirst, TSecond>>.CopyTo(KeyValuePair<TFirst, TSecond>[] array, int arrayIndex)
        {
            firstToSecond.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)firstToSecond).CopyTo(array, index);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Force complete deserialization of the first-to-second dictionary
            (this.firstToSecond as IDeserializationCallback).OnDeserialization(this);

            this.secondToFirst = new Dictionary<TSecond, TFirst>(firstToSecond.Count);
            foreach (var item in firstToSecond)
                secondToFirst.Add(item.Value, item.Key);
            reverse = new ReverseDictionary(this);
        }

        private class ReverseDictionary : IDictionary<TSecond, TFirst>, IReadOnlyDictionary<TSecond, TFirst>, IDictionary
        {
            private readonly TwoWayDictionary<TFirst, TSecond> owner;

            public ReverseDictionary(TwoWayDictionary<TFirst, TSecond> owner)
            {
                this.owner = owner;
            }

            public int Count
            {
                get { return owner.secondToFirst.Count; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)owner.secondToFirst).SyncRoot; }
            }

            bool ICollection.IsSynchronized
            {
                get { return ((ICollection)owner.secondToFirst).IsSynchronized; }
            }

            bool IDictionary.IsFixedSize
            {
                get { return ((IDictionary)owner.secondToFirst).IsFixedSize; }
            }

            public bool IsReadOnly
            {
                get { return owner.secondToFirst.IsReadOnly || owner.firstToSecond.IsReadOnly; }
            }

            public TFirst this[TSecond key]
            {
                get { return owner.secondToFirst[key]; }
                set
                {
                    owner.secondToFirst[key] = value;
                    owner.firstToSecond[value] = key;
                }
            }

            object IDictionary.this[object key]
            {
                get { return ((IDictionary)owner.secondToFirst)[key]; }
                set
                {
                    ((IDictionary)owner.secondToFirst)[key] = value;
                    ((IDictionary)owner.firstToSecond)[value] = key;
                }
            }

            public ICollection<TSecond> Keys
            {
                get { return owner.secondToFirst.Keys; }
            }

            ICollection IDictionary.Keys
            {
                get { return ((IDictionary)owner.secondToFirst).Keys; }
            }

            IEnumerable<TSecond> IReadOnlyDictionary<TSecond, TFirst>.Keys
            {
                get { return ((IReadOnlyDictionary<TSecond, TFirst>)owner.secondToFirst).Keys; }
            }

            public ICollection<TFirst> Values
            {
                get { return owner.secondToFirst.Values; }
            }

            ICollection IDictionary.Values
            {
                get { return ((IDictionary)owner.secondToFirst).Values; }
            }

            IEnumerable<TFirst> IReadOnlyDictionary<TSecond, TFirst>.Values
            {
                get { return ((IReadOnlyDictionary<TSecond, TFirst>)owner.secondToFirst).Values; }
            }

            public IEnumerator<KeyValuePair<TSecond, TFirst>> GetEnumerator()
            {
                return owner.secondToFirst.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return ((IDictionary)owner.secondToFirst).GetEnumerator();
            }

            public void Add(TSecond key, TFirst value)
            {
                owner.secondToFirst.Add(key, value);
                owner.firstToSecond.Add(value, key);
            }

            void IDictionary.Add(object key, object value)
            {
                ((IDictionary)owner.secondToFirst).Add(key, value);
                ((IDictionary)owner.firstToSecond).Add(value, key);
            }

            void ICollection<KeyValuePair<TSecond, TFirst>>.Add(KeyValuePair<TSecond, TFirst> item)
            {
                owner.secondToFirst.Add(item);
                owner.firstToSecond.Add(item.Value, item.Key);
            }

            public bool ContainsKey(TSecond key)
            {
                return owner.secondToFirst.ContainsKey(key);
            }

            bool ICollection<KeyValuePair<TSecond, TFirst>>.Contains(KeyValuePair<TSecond, TFirst> item)
            {
                return owner.secondToFirst.Contains(item);
            }

            public bool TryGetValue(TSecond key, out TFirst value)
            {
                return owner.secondToFirst.TryGetValue(key, out value);
            }

            public bool Remove(TSecond key)
            {
                TFirst value;
                if (owner.secondToFirst.TryGetValue(key, out value))
                {
                    owner.secondToFirst.Remove(key);
                    owner.firstToSecond.Remove(value);
                    return true;
                }
                else
                    return false;
            }

            void IDictionary.Remove(object key)
            {
                var firstToSecond = (IDictionary)owner.secondToFirst;
                if (!firstToSecond.Contains(key))
                    return;
                var value = firstToSecond[key];
                firstToSecond.Remove(key);
                ((IDictionary)owner.firstToSecond).Remove(value);
            }

            bool ICollection<KeyValuePair<TSecond, TFirst>>.Remove(KeyValuePair<TSecond, TFirst> item)
            {
                return owner.secondToFirst.Remove(item);
            }

            bool IDictionary.Contains(object key)
            {
                return ((IDictionary)owner.secondToFirst).Contains(key);
            }

            public void Clear()
            {
                owner.secondToFirst.Clear();
                owner.firstToSecond.Clear();
            }

            void ICollection<KeyValuePair<TSecond, TFirst>>.CopyTo(KeyValuePair<TSecond, TFirst>[] array, int arrayIndex)
            {
                owner.secondToFirst.CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((IDictionary)owner.secondToFirst).CopyTo(array, index);
            }
        }
    }
}
