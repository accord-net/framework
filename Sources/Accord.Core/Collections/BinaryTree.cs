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
    using System.Collections.Generic;
    using Accord.Compat;

    /// <summary>
    ///   Base class for binary trees.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    [Serializable]
    public class BinaryTree<TNode> : IEnumerable<TNode>
        where TNode : BinaryNode<TNode>, new()
    {

        /// <summary>
        ///   Gets the root node of this tree.
        /// </summary>
        /// 
        public TNode Root { get; protected set; }

        /// <summary>
        ///   Returns an enumerator that iterates through the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object 
        ///   that can be used to iterate through the collection.
        /// </returns>
        /// 
        public virtual IEnumerator<TNode> GetEnumerator()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<TNode>(new[] { Root });

            while (stack.Count != 0)
            {
                TNode current = stack.Pop();

                yield return current;

                if (current.Left != null)
                    stack.Push(current.Left);

                if (current.Right != null)
                    stack.Push(current.Right);
            }
        }

        /// <summary>
        ///   Traverse the tree using a <see cref="TreeTraversal">tree traversal
        ///   method</see>. Can be iterated with a foreach loop.
        /// </summary>
        /// 
        /// <param name="method">The tree traversal method. Common methods are
        /// available in the <see cref="TreeTraversal"/>static class.</param>
        /// 
        /// <returns>An <see cref="IEnumerable{T}"/> object which can be used to
        /// traverse the tree using the chosen traversal method.</returns>
        /// 
        public IEnumerable<TNode> Traverse(BinaryTraversalMethod<TNode> method)
        {
            return new BinaryTreeTraversal(this, method);
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        private class BinaryTreeTraversal : IEnumerable<TNode>
        {
            private BinaryTree<TNode> tree;
            private BinaryTraversalMethod<TNode> method;

            public BinaryTreeTraversal(BinaryTree<TNode> tree, BinaryTraversalMethod<TNode> method)
            {
                this.tree = tree;
                this.method = method;
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                return method(tree);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return method(tree);
            }
        }
    }
}