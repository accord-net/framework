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
    public class SPTreeTest
    {


        [Test]
        public void FromDataTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[][] points = Vector.Interval(1.0, 7.0).ToJagged();

            var tree = SPTree.FromData(points);
            Assert.IsTrue(tree.Root.IsCorrect());

            var nodes = tree.ToList();

            foreach (var p in points)
                Assert.IsTrue(nodes.Where(x=> x.IsLeaf)
                    .Select(x => x.Position)
                    .Contains(p, new ArrayComparer<double>()));

            points = Vector.Shuffled(Vector.Range(1.0, 8.0)).ToJagged();

            tree = SPTree.FromData(points);
            Assert.IsTrue(tree.Root.IsCorrect());

            nodes = tree.ToList();

            foreach (var p in points)
                Assert.IsTrue(nodes.Select(x => x.Position).Contains(p));
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


            var tree = SPTree.FromData(points);

            var nodes = tree.ToList();

            Assert.IsTrue(tree.Root.IsCorrect());


            foreach (var p in points)
            {
                Assert.IsTrue(nodes.Where(x => x.IsLeaf && !x.IsEmpty)
                    .Select(x => x.Position)
                    .Contains(p, new ArrayComparer<double>()));
            }


            Assert.IsTrue(tree.Root.IsCorrect());
            Assert.IsFalse(tree.Root.IsLeaf);

            Assert.IsNull(tree.Root.Position);
            Assert.AreEqual(5.8333333333333339, tree.Root.CenterOfMass[0]);
            Assert.AreEqual(3.8333333333333339, tree.Root.CenterOfMass[1]);

            Assert.AreEqual(4, tree.Root.Children.Length);
            Assert.AreEqual(9, tree.Root.Children[0].Position[0]);
            Assert.AreEqual(6, tree.Root.Children[0].Position[1]);

            Assert.IsNull(tree.Root.Children[1].Position);
            Assert.AreEqual(4.5, tree.Root.Children[1].CenterOfMass[0]);
            Assert.AreEqual(5.5, tree.Root.Children[1].CenterOfMass[1]);
        }

    }
}
