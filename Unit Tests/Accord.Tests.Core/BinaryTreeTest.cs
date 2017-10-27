// Accord Unit Tests
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

using Accord.Collections;
using Accord.Math;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Tests
{
    [TestFixture]
    public class BinaryTreeTest
    {
        #region doc_ctor_1
        /// <summary>
        ///   Implement this class as you need. There are no fundamental properties
        ///   or methods that should be overriden by your class, except for any data
        ///   value that you might want your node to carry.
        /// </summary>
        /// 
        class MyTreeNode : BinaryNode<MyTreeNode>
        {
            /// <summary>
            ///   Gets or sets a custom value that you would like your nodes to have.
            /// </summary>
            /// 
            public string Value { get; set; }
        }
        #endregion

        [Test]
        public void tree_test()
        {
            #region doc_ctor_2
            // Let's start by creating an empty tree 
            var tree = new BinaryTree<MyTreeNode>();

            // Now, we can proceed by placing elements on different positions of the tree. Note that this class 
            // does not  implement a search  tree, so it is not possible to place elements automatically using 
            // an ".Add()" method. Instead, this class offers functionality that is common to all Binary Trees,
            // such as traversing the tree breadth-first, depth-first, in order, in pre-order, and in post-order.

            // Let's start populating the tree:
            tree.Root = new MyTreeNode()
            {
                Left = new MyTreeNode()
                {
                    Left = new MyTreeNode()
                    {
                        Value = "a"
                    },

                    Value = "b",

                    Right = new MyTreeNode()
                    {
                        Value = "c"
                    },
                },

                Value = "d",

                Right = new MyTreeNode()
                {
                    Left = new MyTreeNode()
                    {
                        Value = "e"
                    },

                    Value = "f",

                    Right = new MyTreeNode()
                    {
                        Value = "g"
                    },
                }
            };

            // Now, let's traverse the tree in order:
            List<string> breadthFirst = tree.Traverse(TreeTraversal.BreadthFirst).Select(x => x.Value).ToList();
            // should return: "d", "b", "f", "a", "c", "e", "g"

            List<string> depthFirst = tree.Traverse(TreeTraversal.DepthFirst).Select(x => x.Value).ToList();
            // should return: "d", "f", "g", "e", "b", "c", "a"

            List<string> inOrder = tree.Traverse(TreeTraversal.InOrder).Select(x => x.Value).ToList();
            // should return: "a", "b", "c", "d", "e", "f", "g"

            List<string> postOrder = tree.Traverse(TreeTraversal.PostOrder).Select(x => x.Value).ToList();
            // should return: "a", "c", "b", "e", "g", "f", "d"

            List<string> preOrder = tree.Traverse(TreeTraversal.PreOrder).Select(x => x.Value).ToList();
            // should return: "d", "b", "a", "c", "f", "e", "g"
            #endregion

            Assert.AreEqual(new[] { "d", "b", "f", "a", "c", "e", "g" }, breadthFirst);
            Assert.AreEqual(new[] { "d", "f", "g", "e", "b", "c", "a" }, depthFirst);
            Assert.AreEqual(new[] { "a", "b", "c", "d", "e", "f", "g" }, inOrder);
            Assert.AreEqual(new[] { "a", "c", "b", "e", "g", "f", "d" }, postOrder);
            Assert.AreEqual(new[] { "d", "b", "a", "c", "f", "e", "g" }, preOrder);
        }

    }
}
