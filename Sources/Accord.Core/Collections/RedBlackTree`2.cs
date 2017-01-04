// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Steven G. Johnson, 2008
// stevenj@alum.mit.edu
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
//
// The source code presented in this file has been adapted from the
// Red Black tree implementation presented in the NLopt Numerical 
// Optimization Library. Original license details are given below.
//
//    Copyright (c) 2007-2011 Massachusetts Institute of Technology
//
//    Permission is hereby granted, free of charge, to any person obtaining
//    a copy of this software and associated documentation files (the
//    "Software"), to deal in the Software without restriction, including
//    without limitation the rights to use, copy, modify, merge, publish,
//    distribute, sublicense, and/or sell copies of the Software, and to
//    permit persons to whom the Software is furnished to do so, subject to
//    the following conditions:
// 
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
// 
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//

namespace Accord.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Red-black tree specialized for key-based value retrieval.
    /// </summary>
    /// 
    /// <remarks>
    ///   See <see cref="RedBlackTree{T}"/>.
    /// </remarks>
    /// 
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTree<TKey, TValue> : RedBlackTree<KeyValuePair<TKey, TValue>>
    {

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using the default
        ///   <see cref="IComparer{T}"/> for the key type <typeparamref name="TKey"/>.
        /// </summary>
        /// 
        public RedBlackTree()
            : base(KeyValuePairComparer<TKey, TValue>.Default)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using 
        ///   the provided <see cref="IComparer{T}"/> implementation.
        /// </summary>
        /// 
        /// <param name="comparer">
        ///   The element comparer to be used to order elements in the tree.</param>
        /// 
        public RedBlackTree(IComparer<KeyValuePair<TKey, TValue>> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using the default
        ///   <see cref="IComparer{T}"/> for the key type <typeparamref name="TKey"/>.
        /// </summary>
        /// 
        /// <param name="allowDuplicates">
        ///   Pass <c>true</c> to allow duplicate elements 
        ///   in the tree; <c>false</c> otherwise.</param>
        /// 
        public RedBlackTree(bool allowDuplicates)
            : base(KeyValuePairComparer<TKey, TValue>.Default, allowDuplicates)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using 
        ///   the provided <see cref="IComparer{T}"/> implementation.
        /// </summary>
        /// 
        /// <param name="comparer">
        ///   The element comparer to be used to order elements in the tree.</param>
        /// <param name="allowDuplicates">
        ///   Pass <c>true</c> to allow duplicate elements 
        ///   in the tree; <c>false</c> otherwise.</param>
        /// 
        public RedBlackTree(IComparer<KeyValuePair<TKey, TValue>> comparer, bool allowDuplicates) 
            : base(comparer, allowDuplicates)
        {
        }

    }
}
