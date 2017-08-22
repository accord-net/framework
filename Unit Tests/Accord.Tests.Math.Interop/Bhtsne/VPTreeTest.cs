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

#if !MONO
namespace Accord.Tests.Interop.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Accord.Collections;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Distances;
    using NUnit.Framework;
    using AccordTestsMathCpp2;

    [TestFixture]
    public class VPTreeTest
    {
# if DEBUG
        [Test, Ignore("Random")]
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

            var pointsw = points.Apply((x, i) => new DataPointWrapper(2, i, x));

            VPTreeWrapper wrapper = new VPTreeWrapper();
            wrapper.create(pointsw);
            var tree = VPTree.FromData(points, Vector.Range(points.Length),
                new Euclidean(), inPlace: false);

            var nodes = tree.ToList();

            for (int k = 1; k < points.Length; k++)
            {
                var actual = new Dictionary<double, SortedSet<int>>();
                foreach (var p in points)
                {
                    var result = tree.Nearest(p, k);
                    foreach (var r in result)
                    {
                        SortedSet<int> bucket;
                        if (actual.TryGetValue(r.Distance, out bucket))
                            bucket.Add(r.Node.Value);
                        else
                            actual[r.Distance] = new SortedSet<int> { r.Node.Value };
                    }
                }

                var expected = new Dictionary<double, SortedSet<int>>();
                foreach (var p in pointsw)
                {
                    var result = new List<DataPointWrapper>();
                    var distance = new List<double>();
                    wrapper.search(p, k, result, distance);
                    foreach (var r in result.Zip(distance, (a, b) => new { Distance = b, Index = a.index() }))
                    {
                        SortedSet<int> bucket;
                        if (expected.TryGetValue(r.Distance, out bucket))
                            bucket.Add(r.Index);
                        else
                            expected[r.Distance] = new SortedSet<int> { r.Index };
                    }
                }

                Assert.IsTrue(actual.IsEqual(expected));
            }
        }
#endif

        [Test]
        public void specific_test()
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

            var pointsw = points.Apply((x, i) => new DataPointWrapper(2, i, x));

            VPTreeWrapper wrapper = new VPTreeWrapper();
            wrapper.create(pointsw);
            var tree = VPTree.FromData(points, Vector.Range(points.Length),
                new Euclidean(), inPlace: false);

            int k = 2;
            var expected = new List<int>();
            {
                var p = pointsw[0];
                var result = new List<DataPointWrapper>();
                var distance = new List<double>();
                wrapper.search(p, k, result, distance);
                foreach (var r in result)
                    expected.Add(r.index());
            }

            var actual = new List<int>();
            {
                double[] p = points[0];
                var result = tree.Nearest(p, k);
                foreach (var r in result)
                    actual.Add(r.Node.Value);
            }



            Assert.IsTrue(actual.ToArray().IsEqual(expected.ToArray()));
        }

        [Test, Ignore("Random")]
        public void larger_test1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var points = TSNETest.yinyang.Submatrix(null, 0, 1).ToJagged();
            var pointsw = points.Apply((x, i) => new DataPointWrapper(2, i, x));

            VPTreeWrapper wrapper = new VPTreeWrapper();
            wrapper.create(pointsw);
            var tree = VPTree.FromData(points, Vector.Range(points.Length),
                new Euclidean(), inPlace: false);

            var nodes = tree.ToList();

            for (int k = 1; k < points.Length; k++)
            {
                var actual = new Dictionary<double, SortedSet<int>>();
                foreach (var p in points)
                {
                    var result = tree.Nearest(p, k);
                    foreach (var r in result)
                    {
                        SortedSet<int> bucket;
                        if (actual.TryGetValue(r.Distance, out bucket))
                            bucket.Add(r.Node.Value);
                        else
                            actual[r.Distance] = new SortedSet<int> { r.Node.Value };
                    }
                }

                var expected = new Dictionary<double, SortedSet<int>>();
                foreach (var p in pointsw)
                {
                    var result = new List<DataPointWrapper>();
                    var distance = new List<double>();
                    wrapper.search(p, k, result, distance);
                    foreach (var r in result.Zip(distance, (a, b) => new { Distance = b, Index = a.index() }))
                    {
                        SortedSet<int> bucket;
                        if (expected.TryGetValue(r.Distance, out bucket))
                            bucket.Add(r.Index);
                        else
                            expected[r.Distance] = new SortedSet<int> { r.Index };
                    }
                }

                Assert.IsTrue(actual.IsEqual(expected));
            }
        }


        [Test]
        public void specific_test_1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var points = TSNETest.yinyang.Submatrix(null, 0, 1).ToJagged();
            var pointsw = points.Apply((x, i) => new DataPointWrapper(2, i, x));

            VPTreeWrapper wrapper = new VPTreeWrapper();
            wrapper.create(pointsw);
            var tree = VPTree.FromData(points, Vector.Range(points.Length),
                new Euclidean(), inPlace: false);

            var nodes = tree.ToList();

            int k = 1;
            var p = 2;

            var actual = new List<int>();
            {
                var result = tree.Nearest(points[p], k);
                foreach (var r in result)
                {
                    actual.Add(r.Node.Value);
                }
            }

            var expected = new List<int>();
            {
                var result = new List<DataPointWrapper>();
                var distance = new List<double>();
                wrapper.search(pointsw[p], k, result, distance);
                foreach (var r in result.Zip(distance, (a, b) => new { Distance = b, Index = a.index() }))
                {
                    expected.Add(r.Index);
                }
            }

            Assert.IsTrue(actual.ToArray().IsEqual(expected.ToArray()));
        }

    }
}
#endif
