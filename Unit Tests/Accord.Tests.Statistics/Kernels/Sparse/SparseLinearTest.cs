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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Kernels.Sparse;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using Accord.Math.Distances;
    using Accord.Math;

    [TestFixture]
    public class SparseLinearTest
    {

        [Test]
        public void FunctionTest()
        {
            Linear dense = new Linear(1);
            SparseLinear target = new SparseLinear(1);

            double[] sx = { 1, -0.555556, 2, +0.250000, 3, -0.864407, 4, -0.916667 };
            double[] sy = { 1, -0.666667, 2, -0.166667, 3, -0.864407, 4, -0.916667 };
            double[] sz = { 1, -0.944444, 3, -0.898305, 4, -0.916667 };

            double[] dx = { -0.555556, +0.250000, -0.864407, -0.916667 };
            double[] dy = { -0.666667, -0.166667, -0.864407, -0.916667 };
            double[] dz = { -0.944444, +0.000000, -0.898305, -0.916667 };

            double expected, actual;

            expected = dense.Function(dx, dy);
            actual = target.Function(sx, sy);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = dense.Function(dx, dz);
            actual = target.Function(sx, sz);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = dense.Function(dy, dz);
            actual = target.Function(sy, sz);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void FunctionTest2()
        {
            SparseLinearTest.SparseTest(new Linear(), new SparseLinear());
            SparseLinearTest.SparseTest(new Linear(1), new SparseLinear(1));
        }

        public static void SparseTest<Ta, Tb>(Ta denseKernel, Tb sparseKernel)
            where Ta : IDistance, IKernel, IKernel<Sparse<double>>, IDistance<Sparse<double>>
            where Tb : IKernel, IDistance
        {
            double[][] sparse =
            {
                new double[] { },
                new double[] { 2, 1 },
                new double[] { 1, 1 },
                new double[] { 1, 1, 2, 1 }
            };

            double[][] dense =
            {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 },
            };

            for (int i = 0; i < sparse.Length; i++)
            {
                for (int j = 0; j < sparse.Length; j++)
                {
                    double expected = denseKernel.Function(dense[i], dense[j]);
                    double actual = sparseKernel.Function(sparse[i], sparse[j]);

                    Assert.AreEqual(expected, actual);
                }
            }

            for (int i = 0; i < sparse.Length; i++)
            {
                for (int j = 0; j < sparse.Length; j++)
                {
                    double expected = denseKernel.Distance(dense[i], dense[j]);
                    double actual = sparseKernel.Distance(sparse[i], sparse[j]);

                    Assert.AreEqual(expected, actual);
                }
            }

            SparseTest2(denseKernel, denseKernel);
        }

        public static void SparseTest2<Ta, Tb>(Ta sparseKernel, Tb denseKernel)
          where Ta : IDistance, IKernel, IKernel<Sparse<double>>, IDistance<Sparse<double>>
          where Tb : IDistance, IKernel
        {
            Sparse<double>[] sparse =
            {
                Sparse.Parse("109:1 389:1 429:1 488:1 538:1 566:1 598:2 659:1 741:1 1240:1 1302:1 1568:1 1962:1 2122:1 2362:1 2433:1 2474:2 2539:2 2638:1 2812:1 2871:1 2930:1 2960:1 3158:1 4468:1 5023:1 5520:1 6894:1 7076:1 7369:1 9062:2 9086:1 9422:1 10004:1 11187:2 13191:1 13384:1 14131:1 14196:1 14219:1 14608:1 15472:1 16018:2 17370:1 18603:1 18604:2 18605:2 18606:2"),
                Sparse.Parse("77:9 137:1 248:1 271:1 357:1 377:3 400:1 412:1 678:1 824:1 880:16 955:1 960:1 970:3 971:2 972:1 1007:2 1035:2 1166:1 1304:16 1354:1 1628:1 1686:2 1721:1 1877:1 2025:1 2219:1 2497:2 2539:4 2550:1 2874:1 2930:2 3378:1 3411:1 3572:17 3626:2 3688:1 3818:1 4039:1 4479:1 4526:2 4748:1 4822:1 4966:1 5479:1 5562:1 5583:2 5844:2 5848:1 6092:1 6096:1 6149:1 6268:1 6819:1 6894:18 7822:1 8139:1 8626:10 9824:17 10461:1 10609:2 10851:1 11463:1 11874:1 11875:2 12397:16 12412:1 13381:1 13384:1 13590:12 13755:2 14074:17 14166:1 14184:2 14517:1 14904:2 15400:1 15531:1 15579:17 15580:16 15936:16 16488:1 16579:1 16642:29 16793:2 17083:2 17458:1 20543:1 20544:2 21252:1 21358:2 22296:1 22479:3 23590:1 25024:17 25809:1 26235:1 26236:2 26237:1 26238:1 26239:1 26240:1 26241:2"),
            };

            double[][] dense =
            {
                sparse[0].ToDense(30000),
                sparse[1].ToDense(30000),
            };

            for (int i = 0; i < sparse.Length; i++)
            {
                for (int j = 0; j < sparse.Length; j++)
                {
                    double expected = denseKernel.Function(dense[i], dense[j]);
                    double actual = sparseKernel.Function(sparse[i], sparse[j]);

                    Assert.AreEqual(expected, actual);
                }
            }

            for (int i = 0; i < sparse.Length; i++)
            {
                for (int j = 0; j < sparse.Length; j++)
                {
                    double expected = denseKernel.Distance(dense[i], dense[j]);
                    double actual = sparseKernel.Distance(sparse[i], sparse[j]);

                    Assert.AreEqual(expected, actual);
                }
            }
        }


        [Test]
        public void DistanceTest()
        {
            Linear dense = new Linear(1);
            SparseLinear target = new SparseLinear(1);

            double[] sx = { 1, -0.555556, 2, +0.250000, 3, -0.864407, 4, -0.916667 };
            double[] sy = { 1, -0.666667, 2, -0.166667, 3, -0.864407, 4, -0.916667 };
            double[] sz = { 1, -0.944444, 3, -0.898305, 4, -0.916667 };

            double[] dx = { -0.555556, +0.250000, -0.864407, -0.916667 };
            double[] dy = { -0.666667, -0.166667, -0.864407, -0.916667 };
            double[] dz = { -0.944444, +0.000000, -0.898305, -0.916667 };

            double expected, actual;

            expected = dense.Distance(dx, dy);
            actual = target.Distance(sx, sy);
            Assert.AreEqual(expected, actual);

            expected = dense.Distance(dx, dz);
            actual = target.Distance(sx, sz);
            Assert.AreEqual(expected, actual);

            expected = dense.Distance(dy, dz);
            actual = target.Distance(sy, sz);
            Assert.AreEqual(expected, actual);
        }
    }
}
