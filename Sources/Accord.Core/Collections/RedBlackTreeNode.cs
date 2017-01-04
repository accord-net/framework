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

    /// <summary>
    ///   Possible node colors for <see cref="RedBlackTree{T}"/>s.
    /// </summary>
    /// 
    public enum RedBlackTreeNodeType 
    {
        /// <summary>
        ///   Red node.
        /// </summary>
        /// 
        Red,
 
        /// <summary>
        ///   Black node.
        /// </summary>
        /// 
        Black 
    }

    /// <summary>
    ///   <see cref="RedBlackTree{T}"/> node.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value to be stored.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTreeNode<T> : BinaryNode<RedBlackTreeNode<T>>
    {
        RedBlackTreeNode<T> parent;

        RedBlackTreeNodeType color;

        T value;

        /// <summary>
        ///   Constructs a new empty node.
        /// </summary>
        /// 
        public RedBlackTreeNode()
        {

        }

        /// <summary>
        ///   Constructs a node containing the given <param name="value"/>.
        /// </summary>
        /// 
        public RedBlackTreeNode(T value)
        {
            this.value = value;
        }

        /// <summary>
        ///   Gets or sets a reference to this node's parent node.
        /// </summary>
        /// 
        public RedBlackTreeNode<T> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        ///   Gets or sets this node's color.
        /// </summary>
        /// 
        public RedBlackTreeNodeType Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        ///   Gets or sets the value associated with this node.
        /// </summary>
        /// 
        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (color == RedBlackTreeNodeType.Black)
                return "Black: {0)".Format(Value);
            return "Red: {0)".Format(Value);
        }
    }

    /// <summary>
    ///   <see cref="RedBlackTree{T}"/> node.
    /// </summary>
    /// 
    /// <typeparam name="TKey">The type of the key that identifies the value.</typeparam>
    /// <typeparam name="TValue">The type of the values stored in this node.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTreeNode<TKey, TValue> : RedBlackTreeNode<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        ///   Constructs a new empty node.
        /// </summary>
        /// 
        public RedBlackTreeNode()
        {

        }

        /// <summary>
        ///   Constructs a new node containing the given <param name="key">
        ///   key</param> and <param name="value">value</param> pair.
        /// </summary>
        /// 
        public RedBlackTreeNode(TKey key, TValue value)
            : base(new KeyValuePair<TKey, TValue>(key, value))
        {
        }

        /// <summary>
        ///   Constructs a new node containing the given
        ///   <param name="item">key and value pair</param>.
        /// </summary>
        /// 
        public RedBlackTreeNode(KeyValuePair<TKey, TValue> item)
            : base(item)
        {
        }
    }

}
