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
    using AForge.Math.Geometry;
    using AForge;
    using System.Drawing;
    using Accord.Math;
    using Accord.Imaging;
    using Point = AForge.Point;
    using System.Collections.Generic;
    using AForge.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Controls.Imaging;
    using Accord.Controls;

    [TestClass()]
    public class RansacLineTest
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

        [Ignore]
        [TestMethod]
        public void RansacLineConstructorTest2()
        {
            Bitmap image = Properties.Resources.noise_line;

            ImageBox.Show(image); 

            var detector = new SusanCornersDetector();

            List<IntPoint> cloud = detector.ProcessImage(image);

            Bitmap marks = new PointsMarker(cloud, Color.Pink).Apply(image);
            ImageBox.Show(marks);

            RansacLine ransac = new RansacLine(5, 1e-10);
            Line line = ransac.Estimate(cloud);

            Bitmap result = new LineMarker(line).Apply(image);
            ImageBox.Show(result);

            Assert.Fail();
        }

    }
}
