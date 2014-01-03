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
using System;

namespace Accord.Tests.Imaging
{


    [TestClass()]
    public class KNearestNeighborMatchingTest
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
            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector();

            var keyPoints1 = freak.ProcessImage(Properties.Resources.image1).ToArray();
            var keyPoints2 = freak.ProcessImage(Properties.Resources.image2).ToArray();

            bool thrown = false;

            try
            {
                var matcher = new KNearestNeighborMatching<byte[]>(5, Distance.BitwiseHamming);
                IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);
            }
            catch (ArgumentException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void MatchTest2()
        {
            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector();

            var keyPoints1 = freak.ProcessImage(Properties.Resources.old).ToArray();
            var keyPoints2 = freak.ProcessImage(Properties.Resources._new).ToArray();

            var matcher = new KNearestNeighborMatching<byte[]>(5, Distance.BitwiseHamming);

            { // direct
                IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(1, matches[0].Length);
                Assert.AreEqual(1, matches[1].Length);
            }

            { // reverse
                IntPoint[][] matches = matcher.Match(keyPoints2, keyPoints1);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(1, matches[0].Length);
                Assert.AreEqual(1, matches[1].Length);
            }

        }

        [TestMethod()]
        public void MatchTest3()
        {
            FastCornersDetector fast = new FastCornersDetector(threshold: 10);

            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(fast);

            var keyPoints1 = freak.ProcessImage(Properties.Resources.old).ToArray();
            var keyPoints2 = freak.ProcessImage(Properties.Resources.flower01).ToArray();

            var matcher = new KNearestNeighborMatching<byte[]>(5, Distance.BitwiseHamming);

            { // direct
                IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(138, matches[0].Length);
                Assert.AreEqual(138, matches[1].Length);
                Assert.AreEqual(532, matches[0][0].X);
                Assert.AreEqual(159, matches[0][0].Y);
                Assert.AreEqual(keyPoints2[0].ToIntPoint(), matches[1][0]);
            }

            { // reverse
                IntPoint[][] matches = matcher.Match(keyPoints2, keyPoints1);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(138, matches[0].Length);
                Assert.AreEqual(138, matches[1].Length);
                Assert.AreEqual(keyPoints2[0].ToIntPoint(), matches[0][0]);
                Assert.AreEqual(532, matches[1][0].X);
                Assert.AreEqual(159, matches[1][0].Y);
            }

        }

    }
}
