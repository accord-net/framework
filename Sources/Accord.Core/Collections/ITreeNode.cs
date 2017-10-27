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
    /// <summary>
    ///   Common interface for tree nodes, such as <see cref="BinaryTree{TNode}"/> and <see cref="TreeNode{TNode}"/>.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The type of the tree node.</typeparam>
    /// 
    /// <seealso cref="BinaryTree{TNode}"/>
    /// <seealso cref="TreeNode{TNode}"/>
    /// <seealso cref="RedBlackTree{T}"/>
    /// 
    public interface ITreeNode<TNode> 
        where TNode : ITreeNode<TNode>
    {
        /// <summary>
        ///   Gets or sets the collection of child nodes
        ///   under this node.
        /// </summary>
        /// 
        TNode[] Children { get; set; }

        /// <summary>
        ///   Gets whether this node is a leaf (has no children).
        /// </summary>
        /// 
        bool IsLeaf { get; }
    }
}