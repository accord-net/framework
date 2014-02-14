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

using Accord.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using AForge;
using Accord.Math;
using System.Collections.Generic;

namespace Accord.Tests.Imaging
{

    [TestClass()]
    public class CorrelationMatchingTest
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
        public void MatchTest()
        {
            int windowSize = 3;
            CorrelationMatching target = new CorrelationMatching(windowSize);
            Bitmap image1 = Properties.Resources.image1;
            Bitmap image2 = Properties.Resources.image1;

            IntPoint[] points1 = 
            {
                new IntPoint( 3,  3),
                new IntPoint(14,  3),
                new IntPoint( 3, 14),
                new IntPoint(14, 14),
            };

            IntPoint[] points2 = 
            {
                new IntPoint( 3,  3),
                new IntPoint(14,  3),
                new IntPoint( 3, 14),
                new IntPoint(14, 14),
            };

            IntPoint[][] expected = 
            {
                new IntPoint[]
                { 
                    new IntPoint( 3,  3),
                    new IntPoint(14,  3),
                    new IntPoint( 3, 14),
                    new IntPoint(14, 14)
                },

                new IntPoint[]
                { 
                    new IntPoint( 3,  3),
                    new IntPoint(14,  3),
                    new IntPoint( 3, 14),
                    new IntPoint(14, 14)
                },

            };

            IntPoint[][] actual = target.Match(image1, image2, points1, points2);

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [TestMethod()]
        public void MatchTest2()
        {
            for (int windowSize = 1; windowSize <= 15; windowSize += 2)
            {
                CorrelationMatching target = new CorrelationMatching(windowSize);
                Bitmap image1 = Properties.Resources.image1;
                Bitmap image2 = Properties.Resources.image1;

                Assert.AreEqual(16, image1.Height);
                Assert.AreEqual(16, image2.Height);

                Assert.AreEqual(16, image1.Width);
                Assert.AreEqual(16, image2.Width);

                // will test every possible point in the image
                // (and also some points outside just to make sure)
                List<IntPoint> points = new List<IntPoint>();
                for (int i = -5; i < 20; i++)
                    for (int j = -5; j < 20; j++)
                        points.Add(new IntPoint(i, j));


                // Assert that no exception if thrown
                IntPoint[][] actual = target.Match(image1, image2, points.ToArray(), points.ToArray());

                Assert.IsNotNull(actual);
                Assert.AreEqual(2, actual.Length);

                var p1 = actual[0]; var p2 = actual[1];
                Assert.AreEqual(p1.Length, p2.Length);

                
                for (int i = 0; i < p1.Length; i++)
                {
                    // As the images are the same, assert that
                    // each point correlates with itself.
                    Assert.AreEqual(p1[i], p2[i]);

                    // Also assert we have no bogus values
                    Assert.IsTrue(p1[i].X >= 0 && p1[i].X < 16);
                    Assert.IsTrue(p1[i].Y >= 0 && p1[i].Y < 16);
                }

            }
        }
    }
}
