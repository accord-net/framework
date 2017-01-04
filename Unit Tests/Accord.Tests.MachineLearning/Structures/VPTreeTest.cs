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

            double[][] points =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
                new double[] { 7, 2 },
            };


            var tree = VPTree.FromData(points, new Manhattan());

            foreach (var n in tree)
            {
                double[] location = n.Position;
                Assert.AreEqual(2, location.Length);
            }

            List<VPTreeNode<double[]>> nodes = tree.ToList();
            foreach (var p in points)
            {
                Assert.IsTrue(nodes.Select(x => x.Position).Contains(p, new ArrayComparer<double>()));
            }

            var query = new double[] { 5, 3 };

            var neighbors = tree.Nearest(query, neighbors: 3);

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
