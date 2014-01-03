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

namespace Accord.MachineLearning.DecisionTrees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.MachineLearning.DecisionTrees;

    /// <summary>
    ///   Tree enumeration method delegate.
    /// </summary>
    /// 
    /// <returns>An enumerator traversing the tree.</returns>
    /// 
    public delegate IEnumerator<DecisionNode> DecisionTreeTraversalMethod(DecisionNode node);


    /// <summary>
    ///   Common traversal methods for n-ary trees.
    /// </summary>
    /// 
    public static class DecisionTreeTraversal
    {
        /// <summary>
        ///   Breadth-first traversal method.
        /// </summary>
        /// 
        public static IEnumerator<DecisionNode> BreadthFirst(DecisionNode tree)
        {
            if (tree == null)
                yield break;

            var queue = new Queue<DecisionNode>(new[] { tree });

            while (queue.Count != 0)
            {
                DecisionNode current = queue.Dequeue();

                yield return current;

                if (current.Branches != null)
                {
                    foreach (var child in current.Branches)
                        queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        ///   Depth-first traversal method.
        /// </summary>
        /// 
        public static IEnumerator<DecisionNode> DepthFirst(DecisionNode tree)
        {
            if (tree == null)
                yield break;

            var stack = new Stack<DecisionNode>(new[] { tree });

            while (stack.Count != 0)
            {
                DecisionNode current = stack.Pop();

                yield return current;

                if (current.Branches != null)
                    for (int i = current.Branches.Count - 1; i >= 0; i--)
                        stack.Push(current.Branches[i]);
            }
        }

        internal static DecisionNode GetNextSibling(Dictionary<DecisionNode, int> cursors, DecisionNode node)
        {
            var parent = node.Parent;

            if (parent == null) return null;

            // Get current node index
            int index;
            if (!cursors.TryGetValue(node, out index))
                cursors[node] = index = 0;

            int nextIndex = index + 1;
            if (nextIndex < parent.Branches.Count)
            {
                var sibling = parent.Branches[nextIndex];
                cursors[sibling] = nextIndex;
                return sibling;
            }

            return null;
        }

        /// <summary>
        ///   Post-order tree traversal method.
        /// </summary>
        /// 
        /// <remarks>
        ///   Adapted from John Cowan (1998) recommendation.
        /// </remarks>
        /// 
        public static IEnumerator<DecisionNode> PostOrder(DecisionNode tree)
        {
            var cursor = new Dictionary<DecisionNode, int>();

            DecisionNode currentNode = tree;

            while (currentNode != null)
            {
                // Move down to first child
                DecisionNode nextNode = null;
                if (currentNode.Branches != null)
                    nextNode = currentNode.Branches.FirstOrDefault();

                if (nextNode != null)
                {
                    currentNode = nextNode;
                    continue;
                }

                // No child nodes, so walk tree
                while (currentNode != null)
                {
                    yield return currentNode; // post order

                    // Move to sibling if possible.
                    nextNode = GetNextSibling(cursor, currentNode);

                    if (nextNode != null)
                    {
                        currentNode = nextNode;
                        break;
                    }

                    // Move up
                    if (currentNode == nextNode)
                        currentNode = null;
                    else
                        currentNode = currentNode.Parent;
                }
            }
        }
    }
}
