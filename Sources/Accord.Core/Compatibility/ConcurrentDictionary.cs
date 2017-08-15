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

    internal class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dict;

        public ConcurrentDictionary()
        {
            dict = new Dictionary<TKey, TValue>();
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (dict)
                    return ((IDictionary<TKey, TValue>)dict)[key];
            }
            set
            {
                lock (dict)
                    ((IDictionary<TKey, TValue>)dict)[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (dict)
                    return ((IDictionary<TKey, TValue>)dict).Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (dict)
                    return ((IDictionary<TKey, TValue>)dict).Values;
            }
        }

        public int Count
        {
            get
            {
                lock (dict)
                    return ((IDictionary<TKey, TValue>)dict).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (dict)
                    return ((IDictionary<TKey, TValue>)dict).IsReadOnly;
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (dict)
                ((IDictionary<TKey, TValue>)dict).Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (dict)
                ((IDictionary<TKey, TValue>)dict).Add(item);
        }

        public void Clear()
        {
            lock (dict)
                ((IDictionary<TKey, TValue>)dict).Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (dict)
                ((IDictionary<TKey, TValue>)dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).Remove(item);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (dict)
                return ((IDictionary<TKey, TValue>)dict).GetEnumerator();
        }
    }
}
#endif
