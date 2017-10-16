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
    ///   Tree node with arbitrary number of children.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    [Serializable]
    public class TreeNode<TNode> : ITreeNode<TNode>
        where TNode : TreeNode<TNode>
    {
        /// <summary>
        ///   Gets or sets the parent of this node.
        /// </summary>
        /// 
        public TNode Parent { get; set; }

        /// <summary>
        ///   Gets or sets the index of this node in the
        ///   collection of children nodes of its parent.
        /// </summary>
        /// 
        public int Index { get; set; }

        /// <summary>
        ///   Gets or sets the collection of child nodes
        ///   under this node.
        /// </summary>
        /// 
        public TNode[] Children { get; set; }

        /// <summary>
        ///   Gets the next sibling of this node (the node
        ///   immediately next to it in its parent's collection).
        /// </summary>
        /// 
        public TNode Next
        {
            get
            {
                if (Parent == null)
                    return null;
                if (Index + 1 >= Parent.Children.Length)
                    return null;
                return Parent.Children[Index + 1];
            }
        }

        /// <summary>
        ///   Gets the previous sibling of this node.
        /// </summary>
        /// 
        public TNode Previous
        {
            get
            {
                if (Index == 0)
                    return null;
                return Parent.Children[Index - 1];
            }
        }

        /// <summary>
        ///   Gets whether this node is a leaf (has no children).
        /// </summary>
        /// 
        public bool IsLeaf
        {
            get { return Children == null || Children.Length == 0; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TreeNode{TNode}"/> class.
        /// </summary>
        /// 
        /// <param name="index">The index of this node in the children collection of its parent node.</param>
        /// 
        public TreeNode(int index)
        {
            this.Index = index;
        }

    }
}
