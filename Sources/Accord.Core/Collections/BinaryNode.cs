// Accord Machine Learning Library
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
    using System.Text;
    using Accord.Compat;

    /// <summary>
    ///   Tree node for binary trees.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    [Serializable]
    public class BinaryNode<TNode> : IEquatable<TNode>, ITreeNode<TNode> // TODO: Try to remove IEquatable
        where TNode : BinaryNode<TNode>
    {
        /// <summary>
        ///   Gets or sets the left subtree of this node.
        /// </summary>
        /// 
        public TNode Left { get; set; }

        /// <summary>
        ///   Gets or sets the right subtree of this node.
        /// </summary>
        /// 
        public TNode Right { get; set; }

        /// <summary>
        ///   Gets whether this node is a leaf (has no children).
        /// </summary>
        /// 
        public bool IsLeaf
        {
            get { return Left == default(TNode) && Right == default(TNode); }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// 
        public bool Equals(TNode other)
        {
            return this == other;
        }

        /// <summary>
        ///   Gets or sets the collection of child nodes
        ///   under this node.
        /// </summary>
        /// 
        public TNode[] Children
        {
            get { return new[] { Left, Right }; }
            set
            {
                if (value.Length != 2)
                    throw new ArgumentException("The array must have length 2.", "value");
                Left = value[0];
                Right = value[1];
            }
        }

    }
}
