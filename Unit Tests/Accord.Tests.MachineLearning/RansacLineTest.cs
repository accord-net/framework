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
    using Accord.MachineLearning.Geometry;
    using Accord.Math.Geometry;
    using NUnit.Framework;
    using Point = Accord.Point;

    [TestFixture]
    public class RansacLineTest
    {

        [Test]
        public void RansacLineConstructorTest()
        {
            Point[] points = new Point[500];

            {
                for (int i = 0; i < points.Length; i++)
                {
                    double x = i;
                    double y = i; //+ 5 * Accord.Math.Tools.Random.NextDouble();
                    points[i] = new Point((float)x, (float)y);
                }

                RansacLine target = new RansacLine(0.80, 0.9);

                Line actual = target.Estimate(points);
                Line expected = Line.FromPoints(points[0], points[499]);

                Assert.AreEqual(expected.Slope, actual.Slope, 1e-3);
                Assert.AreEqual(expected.Intercept, actual.Intercept, 1e-2);
            }

            {
                for (int i = 0; i < points.Length; i++)
                {
                    double x = i;
                    double y = i + 5 * Accord.Math.Tools.Random.NextDouble();
                    points[i] = new Point((float)x, (float)y);
                }

                RansacLine target = new RansacLine(0.80, 0.9);

                Line actual = target.Estimate(points);

                Assert.AreEqual(1.0, actual.Slope, 1e-3);
                Assert.AreEqual(0.0, actual.Intercept, 1e-2);
            }

            {
                for (int i = 0; i < points.Length; i++)
                {
                    double x = i + 50;
                    double y = i + 50 + 5 * Accord.Math.Tools.Random.NextDouble();
                    points[i] = new Point((float)x, (float)y);
                }

                RansacLine target = new RansacLine(0.80, 0.9);

                Line actual = target.Estimate(points);

                Assert.AreEqual(1.0, actual.Slope, 1e-3);
                Assert.AreEqual(0.0, actual.Intercept, 1e-2);
            }
        }

    }
}
