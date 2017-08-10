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
    using System.Collections.Generic;
    using Accord.Compat;

    /// <summary>
    ///   Vanilla key-based comparer for <see cref="KeyValuePair{TKey, TValue}"/>.
    /// </summary>
    /// 
    /// <typeparam name="TKey">The key type in the key-value pair.</typeparam>
    /// <typeparam name="TValue">The value type in the key-value pair.</typeparam>
    /// 
    [Serializable]
    public class KeyValuePairComparer<TKey, TValue> 
        : Comparer<KeyValuePair<TKey, TValue>>, IComparer<TKey>
    {
        
        private readonly IComparer<TKey> keyComparer;


        /// <summary>
        ///   Initializes a new instance of the <see cref="KeyValuePairComparer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="keyComparer">The comparer to be used to compare keys.</param>
        /// 
        public KeyValuePairComparer(IComparer<TKey> keyComparer)
        {
            if (keyComparer == null)
                throw new ArgumentNullException("keyComparer");

            this.keyComparer = keyComparer;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KeyValuePairComparer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// 
        public KeyValuePairComparer()
        {
            this.keyComparer = Comparer<TKey>.Default;
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether 
        ///   one is less than, equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return keyComparer.Compare(x.Key, y.Key);
        }
      
        /// <summary>
        ///   Compares two objects and returns a value indicating whether 
        ///   one is less than, equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public int Compare(TKey x, TKey y)
        {
            return keyComparer.Compare(x, y);
        }

        /// <summary>
        ///    Returns a default sort order comparer for the
        ///    key-value pair specified by the generic argument.
        /// </summary>
        /// 
        public new static KeyValuePairComparer<TKey, TValue> Default
        {
            get { return new KeyValuePairComparer<TKey, TValue>(); }
        }

    }
}
