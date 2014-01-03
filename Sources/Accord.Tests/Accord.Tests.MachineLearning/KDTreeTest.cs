// Accord Unit Tests
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

namespace Accord.Tests.Math
{
    using System.Linq;
    using Accord.MachineLearning.Structures;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System;

    [TestClass()]
    public class KDTreeTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void FromDataTest()
        {
            // This is the same example found in Wikipedia page on
            // k-d trees: http://en.wikipedia.org/wiki/K-d_tree

            // Suppose we have the following set of points:

            double[][] points =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
                new double[] { 7, 2 },
            };


            // To create a tree from a set of points, we use
            KDTree<int> tree = KDTree.FromData<int>(points);

            // Now we can manually navigate the tree
            KDTreeNode<int> node = tree.Root.Left.Right;

            // Or traverse it automatically
            foreach (KDTreeNode<int> n in tree)
            {
                double[] location = n.Position;
                Assert.AreEqual(2, location.Length);
            }

            // Given a query point, we can also query for other
            // points which are near this point within a radius

            double[] query = new double[] { 5, 3 };

            // Locate all nearby points within an Euclidean distance of 1.5
            // (answer should be a single point located at position (5,4))
            List<KDTreeNodeDistance<int>> result = tree.Nearest(query, radius: 1.5); 
            
            // We can also use alternate distance functions
            tree.Distance = Accord.Math.Distance.Manhattan;

            // And also query for a fixed number of neighbor points
            // (answer should be the points at (5,4), (7,2), (2,3))
            KDTreeNodeCollection<int> neighbors = tree.Nearest(query, neighbors: 3);


            Assert.IsTrue(node.IsLeaf);
            Assert.IsFalse(tree.Root.IsLeaf);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("(5,4)", result[0].Node.ToString());

            Assert.AreEqual(3, neighbors.Count);
            Assert.AreEqual("(5,4)", neighbors[2].Node.ToString());
            Assert.AreEqual("(7,2)", neighbors[1].Node.ToString());
            Assert.AreEqual("(2,3)", neighbors[0].Node.ToString());

            Assert.AreEqual(7, tree.Root.Position[0]);
            Assert.AreEqual(2, tree.Root.Position[1]);
            Assert.AreEqual(0, tree.Root.Axis);

            Assert.AreEqual(5, tree.Root.Left.Position[0]);
            Assert.AreEqual(4, tree.Root.Left.Position[1]);
            Assert.AreEqual(1, tree.Root.Left.Axis);

            Assert.AreEqual(9, tree.Root.Right.Position[0]);
            Assert.AreEqual(6, tree.Root.Right.Position[1]);
            Assert.AreEqual(1, tree.Root.Right.Axis);

            Assert.AreEqual(2, tree.Root.Left.Left.Position[0]);
            Assert.AreEqual(3, tree.Root.Left.Left.Position[1]);
            Assert.AreEqual(0, tree.Root.Left.Left.Axis);

            Assert.AreEqual(4, tree.Root.Left.Right.Position[0]);
            Assert.AreEqual(7, tree.Root.Left.Right.Position[1]);
            Assert.AreEqual(0, tree.Root.Left.Right.Axis);

            Assert.AreEqual(8, tree.Root.Right.Left.Position[0]);
            Assert.AreEqual(1, tree.Root.Right.Left.Position[1]);
            Assert.AreEqual(0, tree.Root.Right.Left.Axis);

            Assert.IsNull(tree.Root.Right.Right);

        }


        [TestMethod()]
        public void FromDataTest2()
        {
            double[][] points =
            {
              new double[]  { 2, 3 },
              new double[]  { 2, 4 },
              new double[]  { 4, 3 }
            };

            var tree = KDTree.FromData<int>(points);


            Assert.AreEqual(2, tree.Root.Position[0]);
            Assert.AreEqual(3, tree.Root.Position[1]);

            Assert.AreEqual(2, tree.Root.Left.Position[0]);
            Assert.AreEqual(4, tree.Root.Left.Position[1]);

            Assert.AreEqual(4, tree.Root.Right.Position[0]);
            Assert.AreEqual(3, tree.Root.Right.Position[1]);

            Assert.IsNull(tree.Root.Left.Left);
            Assert.IsNull(tree.Root.Left.Right);
            Assert.IsNull(tree.Root.Right.Left);
            Assert.IsNull(tree.Root.Right.Right);
        }


        [TestMethod()]
        public void NearestTest()
        {
            double[][] points =
            {
                new double[] { 1, 1 }, new double[] { 1, 2 }, new double[] { 1, 3 }, new double[] { 1, 4 }, new double[] { 1, 5 }, 
                new double[] { 2, 1 }, new double[] { 2, 2 }, new double[] { 2, 3 }, new double[] { 2, 4 }, new double[] { 2, 5 }, 
                new double[] { 3, 1 }, new double[] { 3, 2 }, new double[] { 3, 3 }, new double[] { 3, 4 }, new double[] { 3, 5 }, 
                new double[] { 4, 1 }, new double[] { 4, 2 }, new double[] { 4, 3 }, new double[] { 4, 4 }, new double[] { 4, 5 }, 
                new double[] { 5, 1 }, new double[] { 5, 2 }, new double[] { 5, 3 }, new double[] { 5, 4 }, new double[] { 5, 5 }, 
            };

            var tree = KDTree.FromData<int>(points);

            tree.Distance = Accord.Math.Distance.Manhattan;

            
            for (int i = 0; i < points.Length; i++)
            {
                var retrieval = tree.Nearest(points[i], 0.0);

                Assert.AreEqual(1, retrieval.Count);
                Assert.AreEqual(points[i][0], retrieval[0].Node.Position[0]);
                Assert.AreEqual(points[i][1], retrieval[0].Node.Position[1]);
            }

            var result = tree.Nearest(new double[] { 3, 3 }, 1.0);

            double[][] expected =
            {
                                        new double[] { 2, 3 },
                new double[] { 3, 2 },  new double[] { 3, 3 }, new double[] { 3, 4 }, 
                                        new double[] { 4, 3 }, 
            };

            Assert.AreEqual(expected.Length, result.Count);

            double[][] actual = (from node in result select node.Node.Position).ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(actual.Contains(expected[i], new CustomComparer<double[]>((a, b) => a.IsEqual(b) ? 0 : 1)));
            }
        }

        [TestMethod()]
        public void NearestTest3()
        {
            double[][] points =
            {
                new double[] { 1, 1 }, new double[] { 1, 2 }, new double[] { 1, 3 }, new double[] { 1, 4 }, new double[] { 1, 5 }, 
                new double[] { 2, 1 }, new double[] { 2, 2 }, new double[] { 2, 3 }, new double[] { 2, 4 }, new double[] { 2, 5 }, 
                new double[] { 3, 1 }, new double[] { 3, 2 }, new double[] { 3, 3 }, new double[] { 3, 4 }, new double[] { 3, 5 }, 
                new double[] { 4, 1 }, new double[] { 4, 2 }, new double[] { 4, 3 }, new double[] { 4, 4 }, new double[] { 4, 5 }, 
                new double[] { 5, 1 }, new double[] { 5, 2 }, new double[] { 5, 3 }, new double[] { 5, 4 }, new double[] { 5, 5 }, 
            };

            var tree = KDTree.FromData<int>(points);

            tree.Distance = Accord.Math.Distance.Manhattan;


            for (int i = 0; i < points.Length; i++)
            {
                var retrieval = tree.Nearest(points[i], 1);

                Assert.AreEqual(1, retrieval.Count);
                Assert.AreEqual(points[i][0], retrieval[0].Node.Position[0]);
                Assert.AreEqual(points[i][1], retrieval[0].Node.Position[1]);
            }

            KDTreeNodeCollection<int> result = tree.Nearest(new double[] { 3, 3 }, 5);

            double[][] expected =
            {
                                        new double[] { 2, 3 },
                new double[] { 3, 2 },  new double[] { 3, 3 }, new double[] { 3, 4 }, 
                                        new double[] { 4, 3 }, 
            };

            Assert.AreEqual(expected.Length, result.Count);

            double[][] actual = (from node in result select node.Node.Position).ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(actual.Contains(expected[i], new CustomComparer<double[]>((a, b) => a.IsEqual(b) ? 0 : 1)));
            }
        }

        [TestMethod()]
        public void NearestTest2()
        {
            double[][] points =
            {
                                        new double[] { 2, 3 }, 
                 new double[] { 3, 2 }, new double[] { 3, 3 }, new double[] { 3, 4 }, 
                                        new double[] { 4, 3 }, 
            };

            var tree = KDTree.FromData<int>(points);
            tree.Distance = Accord.Math.Distance.Manhattan;

            Assert.AreEqual(3, tree.Root.Position[0]);
            Assert.AreEqual(3, tree.Root.Position[1]);

            Assert.AreEqual(3, tree.Root.Left.Position[0]);
            Assert.AreEqual(4, tree.Root.Left.Position[1]);

            Assert.AreEqual(4, tree.Root.Right.Position[0]);
            Assert.AreEqual(3, tree.Root.Right.Position[1]);

            Assert.AreEqual(2, tree.Root.Left.Left.Position[0]);
            Assert.AreEqual(3, tree.Root.Left.Left.Position[1]);

            Assert.AreEqual(3, tree.Root.Right.Left.Position[0]);
            Assert.AreEqual(2, tree.Root.Right.Left.Position[1]);

            var result = tree.Nearest(new double[] { 3, 3 }, 5);

            double[][] expected =
            {
                                        new double[] { 2, 3 },
                new double[] { 3, 2 },  new double[] { 3, 3 }, new double[] { 3, 4 }, 
                                        new double[] { 4, 3 }, 
            };

            Assert.AreEqual(expected.Length, result.Count);

            double[][] actual = (from node in result select node.Node.Position).ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(actual.Contains(expected[i], new CustomComparer<double[]>((a, b) => a.IsEqual(b) ? 0 : 1)));
            }
        }

        [TestMethod()]
        public void TraverseTest0()
        {
            double[][] points =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
                new double[] { 7, 2 },
            };


            // To create a tree from a set of points, we use
            KDTree<int> tree = KDTree.FromData<int>(points);

            double[][] inOrder =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 4, 7 },
                new double[] { 7, 2 },
                new double[] { 8, 1 },
                new double[] { 9, 6 },
            };

            int i = 0;
            foreach (var node in tree.Traverse(KDTreeTraversal.InOrder))
            {
                Assert.AreEqual(node.Position[0], inOrder[i][0]);
                Assert.AreEqual(node.Position[1], inOrder[i][1]);
                i++;
            }
        }

        [TestMethod()]
        public void TraverseTest1()
        {
            double[][] points =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
                new double[] { 7, 2 },
            };


            // To create a tree from a set of points, we use
            KDTree<int> tree = KDTree.FromData<int>(points);

            double[][] breadth =
            {
                new double[] { 7, 2 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 2, 3 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
            };

            double[][] inOrder =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 4, 7 },
                new double[] { 7, 2 },
                new double[] { 8, 1 },
                new double[] { 9, 6 },
            };

            double[][] postOrder =
            {
                new double[] { 2, 3 },
                new double[] { 4, 7 },
                new double[] { 5, 4 },
                new double[] { 8, 1 },
                new double[] { 9, 6 },
                new double[] { 7, 2 },
            };

            double[][] preOrder =
            {
                new double[] { 7, 2 },
                new double[] { 5, 4 },
                new double[] { 2, 3 },
                new double[] { 4, 7 },
                new double[] { 9, 6 },
                new double[] { 8, 1 },
            };


            AreEqual(tree, breadth, KDTreeTraversal.BreadthFirst);
            AreEqual(tree, preOrder, KDTreeTraversal.PreOrder);
            AreEqual(tree, inOrder, KDTreeTraversal.InOrder);
            AreEqual(tree, postOrder, KDTreeTraversal.PostOrder);
        }

        private static void AreEqual(KDTree<int> tree, double[][] expected, Func<KDTree<int>, IEnumerator<KDTreeNode<int>>> method)
        {
            double[][] actual = tree.Traverse(method.Invoke).Select(p => p.Position).ToArray();

            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i][0], actual[i][0]);
                Assert.AreEqual(expected[i][1], actual[i][1]);
            }
        }
    }
}
