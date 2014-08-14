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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.Geometry;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math.Geometry;
    using Accord.Math;

    [TestClass()]
    public class RansacPlaneTest
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
        public void RansacPlaneConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            {
                Point3[] points = new Point3[300];

                int c = 0;
                for (int i = 0; i < points.Length / 3; i++)
                {
                    points[c++] = new Point3((float)i, (float)0, (float)0);
                    points[c++] = new Point3((float)0, (float)i, (float)0);
                    points[c++] = new Point3((float)i, (float)i, (float)0);
                }

                RansacPlane target = new RansacPlane(0.80, 0.9);

                Plane expected = Plane.FromPoints(points[3], points[4], points[5]);
                Plane actual = target.Estimate(points);

                Assert.AreEqual(actual.A, 0);
                Assert.AreEqual(actual.B, 0);
                Assert.AreEqual(actual.C, -1);
                Assert.AreEqual(actual.Offset, 0);

                Assert.IsTrue(expected.Equals(actual, 1e-3));
            }

            {
                Point3[] points = new Point3[300];

                int c = 0;
                for (int i = 0; i < points.Length / 3; i++)
                {
                    points[c++] = new Point3((float)i, (float)0, (float)50);
                    points[c++] = new Point3((float)0, (float)i, (float)50);
                    points[c++] = new Point3((float)i, (float)i, (float)50);
                }

                RansacPlane target = new RansacPlane(0.80, 0.9);

                Plane expected = Plane.FromPoints(points[6], points[7], points[8]);
                expected.Normalize();

                Plane actual = target.Estimate(points);

                Assert.AreEqual(actual.A, 0);
                Assert.AreEqual(actual.B, 0, 1e-5);
                Assert.AreEqual(actual.C, -1, 1e-5);
                Assert.AreEqual(actual.Offset, 50, 1e-4);

                Assert.IsTrue(expected.Equals(actual, 1e-3));
            }

            {
                Point3[] points = new Point3[10 * 10];

                int c = 0;
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        double x = i + 5 * Accord.Math.Tools.Random.NextDouble();
                        double y = j + 5 * Accord.Math.Tools.Random.NextDouble();
                        double z = x + y - 1;

                        points[c++] = new Point3((float)x, (float)z, (float)y);
                    }
                }

                RansacPlane target = new RansacPlane(0.80, 0.9);

                Plane actual = target.Estimate(points);
                var normal = actual.Normal / actual.Normal.Max;

                Assert.AreEqual(normal.X, +1, 1e-5);
                Assert.AreEqual(normal.Y, -1, 1e-5);
                Assert.AreEqual(normal.Z, +1, 1e-5);
            }
        }

        [TestMethod()]
        public void RansacPlaneConstructorTest2()
        {
            Point3[] points = 
            {
                new Point3(1,1,1),
                new Point3(2,3,4),
                new Point3(4,6,9),
                new Point3(2,2,3)
            };

            RansacPlane target = new RansacPlane(0.80, 0.9);

            Plane plane = target.Estimate(points);
            var normal = plane.Normal / plane.Normal.Max;
            double d = plane.DistanceToPoint(Point3.Origin);

            Assert.AreEqual(normal.X, 1, 1e-4);
            Assert.AreEqual(normal.Y, 1, 1e-4);
            Assert.AreEqual(normal.Z, -1, 1e-4);
            Assert.AreEqual(plane.Offset, -d, 1e-4);
        }
    }
}
