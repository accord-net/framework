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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Tree enumeration method delegate.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    /// <param name="tree">The binary tree to be traversed.</param>
    /// 
    /// <returns>An enumerator traversing the tree.</returns>
    /// 
    public delegate IEnumerator<TNode> BinaryTraversalMethod<TNode>(BinaryTree<TNode> tree)
        where TNode : BinaryNode<TNode>;

    /// <summary>
    ///   Tree enumeration method delegate.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    /// <returns>An enumerator traversing the tree.</returns>
    /// 
    public delegate IEnumerator<TNode> TraversalMethod<TNode>(Tree<TNode> tree)
        where TNode : TreeNode<TNode>;

    /// <summary>
    ///   Static class with tree traversal methods.
    /// </summary>
    /// 
    public static class TreeTraversal
    {
        /// <summary>
        ///   Breadth-first tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<TNode> BreadthFirst<TNode>(BinaryTree<TNode> tree)
            where TNode : BinaryNode<TNode>, new()
        {
            if (tree.Root == null)
                yield break;

            var queue = new Queue<TNode>(new[] { tree.Root });

            while (queue.Count != 0)
            {
                TNode current = queue.Dequeue();

                if (current != null)
                    yield return current;

                if (current.Left != null)
                    queue.Enqueue(current.Left);

                if (current.Right != null)
                    queue.Enqueue(current.Right);
            }
        }

        /// <summary>
        ///   Pre-order tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<TNode> PreOrder<TNode>(BinaryTree<TNode> tree)
            where TNode : BinaryNode<TNode>, new()
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<TNode>();

            TNode current = tree.Root;

            while (stack.Count != 0 || current != null)
            {
                if (current != null)
                {
                    stack.Push(current);
                    yield return current;
                    current = current.Left;
                }
                else
                {
                    current = stack.Pop();
                    current = current.Right;
                }
            }
        }

        /// <summary>
        ///   In-order tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<TNode> InOrder<TNode>(BinaryTree<TNode> tree)
            where TNode : BinaryNode<TNode>, new()
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<TNode>();

            TNode current = tree.Root;

            while (stack.Count != 0 || current != null)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    current = stack.Pop();
                    yield return current;
                    current = current.Right;
                }
            }
        }

        /// <summary>
        ///   Post-order tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<TNode> PostOrder<TNode>(BinaryTree<TNode> tree)
            where TNode : BinaryNode<TNode>, new()
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<TNode>(new[] { tree.Root });

            TNode previous = tree.Root;

            while (stack.Count != 0)
            {
                TNode current = stack.Peek();

                if (previous == current || previous.Left == current || previous.Right == current)
                {
                    if (current.Left != null)
                        stack.Push(current.Left);
                    else if (current.Right != null)
                        stack.Push(current.Right);
                    else
                    {
                        yield return stack.Pop();
                    }
                }
                else if (current.Left == previous)
                {
                    if (current.Right != null)
                        stack.Push(current.Right);
                    else
                    {
                        yield return stack.Pop();
                    }
                }
                else if (current.Right == previous)
                {
                    yield return stack.Pop();
                }
                else
                {
                    throw new InvalidOperationException();
                }

                previous = current;
            }
        }

        /// <summary>
        ///   Depth-first tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<TNode> DepthFirst<TNode>(BinaryTree<TNode> tree)
            where TNode : BinaryNode<TNode>
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<TNode>();
            stack.Push(tree.Root);

            while (stack.Count != 0)
            {
                TNode node = stack.Pop();

                if (node != null)
                    yield return node;

                if (node.Left != null)
                    stack.Push(node.Left);

                if (node.Right != null)
                    stack.Push(node.Right);
            }
        }

        /// <summary>
        ///   Depth-first tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<TNode> DepthFirst<TNode>(Tree<TNode> tree)
            where TNode : TreeNode<TNode>
        {
            if (tree.Root == null)
                yield break;

            TNode node = tree.Root;

            while (node != null)
            {
                yield return node;

                if (node.IsLeaf)
                {
                    while (node.Next == null && node.Parent != null)
                        node = node.Parent;
                    node = node.Next;
                }
                else
                {
                    node = node.Children[0];
                }
            }
        }

    }
}
