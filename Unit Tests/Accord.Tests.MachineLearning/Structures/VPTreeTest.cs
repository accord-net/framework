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

namespace Accord.Tests.MachineLearning
{
#if !MONO
    using Accord.Collections;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Tests.MachineLearning.Structures;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class VPTreeTest
    {


        [Test]
        public void FromDataTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[] points =
            {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
            };

            var tree = VPTree.FromData(points);

            List<VPTreeNode<double>> nodes = tree.ToList();

            for (int i = 1; i <= 7; i++)
                Assert.IsTrue(nodes.Select(x => x.Position).Contains(i));

            points = Vector.Shuffled(Vector.Range(1.0, 8.0));

            tree = VPTree.FromData(points);

            nodes = tree.ToList();

            for (int i = 1; i <= 7; i++)
                Assert.IsTrue(nodes.Select(x => x.Position).Contains(i));
        }

        [Test]
        public void FromDataTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            #region doc_create
            // Let's say we would like to create a VP tree
            // from a set of multidimensional data points:
            double[][] points =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
                new double[] { 7, 2 },
            };

            // We will create it using the Manhattan distance:
            var tree = VPTree.FromData(points, new Manhattan());

            // Now, we can query whether a point belongs to the tree
            double[] query = new double[] { 5, 3 };

            // Find the top-3 closest points within the tree:
            var neighbors = tree.Nearest(query, neighbors: 3);

            // Results will be:
            NodeDistance<VPTreeNode<double[]>>[] result = neighbors.ToArray();
            double d1 = result[0].Distance;
            double[] p1 = result[0].Node.Position;

            double d2 = result[0].Distance;
            double[] p2 = result[0].Node.Position;

            double d3 = result[0].Distance;
            double[] p3 = result[0].Node.Position;


            // We can also navigate the tree using:
            foreach (VPTreeNode<double[]> n in tree)
            {
                // We can extract information from its nodes:
                double[] location = n.Position; // should always have length 2
                double threshold = n.Threshold; // the node's threshold radius
                bool isLeaf = n.IsLeaf; // whether the node is a leaf or not
            }
            #endregion

            Assert.AreEqual(0, d1);
            Assert.AreEqual(0, d2);
            Assert.AreEqual(0, d3);

            Assert.AreEqual(0, p1);
            Assert.AreEqual(0, p2);
            Assert.AreEqual(0, p3);

            foreach (VPTreeNode<double[]> n in tree)
            {
                double[] location = n.Position;
                Assert.AreEqual(2, location.Length);
            }

            List<VPTreeNode<double[]>> nodes = tree.ToList();
            foreach (var p in points)
            {
                Assert.IsTrue(nodes.Select(x => x.Position).Contains(p, new ArrayComparer<double>()));
            }

            Assert.IsFalse(tree.Root.IsLeaf);

            Assert.AreEqual(3, neighbors.Count);
            Assert.AreEqual("[(5,4), 4]", neighbors[0].Node.ToString());
            Assert.AreEqual("[(2,3), 0]", neighbors[1].Node.ToString());
            Assert.AreEqual("[(7,2), 0]", neighbors[2].Node.ToString());

            Assert.AreEqual(8, tree.Root.Position[0]);
            Assert.AreEqual(1, tree.Root.Position[1]);

            Assert.AreEqual("[(5,4), 4]", tree.Root.Left.ToString());
            Assert.AreEqual("[(4,7), 6]", tree.Root.Right.ToString());

            Assert.AreEqual(4, tree.Root.Right.Position[0]);
            Assert.AreEqual(7, tree.Root.Right.Position[1]);

            Assert.AreEqual(9, tree.Root.Right.Right.Position[0]);
            Assert.AreEqual(6, tree.Root.Right.Right.Position[1]);
        }
    }
#endif
}
