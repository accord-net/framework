// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.MachineLearning.Structures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Tree enumeration method delegate.
    /// </summary>
    /// 
    /// <typeparam name="T">The data type stored in the tree.</typeparam>
    /// 
    /// <param name="tree">The k-d tree to be traversed.</param>
    /// 
    /// <returns>An enumerator traversing the tree.</returns>
    /// 
    public delegate IEnumerator<KDTreeNode<T>> KDTreeTraversalMethod<T>(KDTree<T> tree);

    /// <summary>
    ///   Static class with tree traversal methods.
    /// </summary>
    /// 
    public static class KDTreeTraversal
    {
        /// <summary>
        ///   Breadth-first tree traversal method.
        /// </summary>
        /// 
        public static IEnumerator<KDTreeNode<T>> BreadthFirst<T>(KDTree<T> tree)
        {
            if (tree.Root == null)
                yield break;

            var queue = new Queue<KDTreeNode<T>>(new[] { tree.Root });

            while (queue.Count != 0)
            {
                KDTreeNode<T> current = queue.Dequeue();

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
        public static IEnumerator<KDTreeNode<T>> PreOrder<T>(KDTree<T> tree)
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<KDTreeNode<T>>();

            KDTreeNode<T> current = tree.Root;

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
        public static IEnumerator<KDTreeNode<T>> InOrder<T>(KDTree<T> tree)
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<KDTreeNode<T>>();

            KDTreeNode<T> current = tree.Root;

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
        public static IEnumerator<KDTreeNode<T>> PostOrder<T>(KDTree<T> tree)
        {
            if (tree.Root == null)
                yield break;

            var stack = new Stack<KDTreeNode<T>>(new[] { tree.Root });

            KDTreeNode<T> previous = tree.Root;

            while (stack.Count != 0)
            {
                KDTreeNode<T> current = stack.Peek();

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
    }
}
