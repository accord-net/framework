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

namespace Accord.Tests.Imaging
{
    using Accord.Imaging;
    using NUnit.Framework;
    using System.Drawing;
    using AForge;
    using Accord.Math;
    using System.Collections.Generic;
    using Accord.Tests.Vision.Properties;
    using System;
    using Accord.Math.Distances;
#if NO_BITMAP
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class KNearestNeighborMatchingTest
    {

        [Test]
        public void MatchTest()
        {
            var image1 = Accord.Imaging.Image.Clone(Resources.image1);
            var image2 = Accord.Imaging.Image.Clone(Resources.image2);
            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector();

            var keyPoints1 = freak.ProcessImage(image1).ToArray();
            var keyPoints2 = freak.ProcessImage(image2).ToArray();

            bool thrown = false;

            try
            {
                var matcher = new KNearestNeighborMatching<byte[]>(5, new Hamming());
                IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);
            }
            catch (ArgumentException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void MatchTest2()
        {
            var imgOld = Accord.Imaging.Image.Clone(Resources.old);
            var imgNew = Accord.Imaging.Image.Clone(Resources._new);
            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector();

            var keyPoints1 = freak.ProcessImage(imgOld).ToArray();
            var keyPoints2 = freak.ProcessImage(imgNew).ToArray();

            var matcher = new KNearestNeighborMatching<byte[]>(5, new Hamming());

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

        [Test]
        [Category("Random")]
        public void MatchTest3()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var old = Accord.Imaging.Image.Clone(Resources.old);
            var flower01 = Accord.Imaging.Image.Clone(Resources.flower01);

            FastCornersDetector fast = new FastCornersDetector(threshold: 10);

            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(fast);

            var keyPoints1 = freak.ProcessImage(old).ToArray();
            var keyPoints2 = freak.ProcessImage(flower01).ToArray();

            var matcher = new KNearestNeighborMatching<byte[]>(5, new Hamming());

            { // direct
                IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(143, matches[0].Length);
                Assert.AreEqual(143, matches[1].Length);
                Assert.AreEqual(532, matches[0][0].X);
                Assert.AreEqual(159, matches[0][0].Y);
                Assert.AreEqual(keyPoints2[0].ToIntPoint(), matches[1][0]);
            }

            { // reverse
                IntPoint[][] matches = matcher.Match(keyPoints2, keyPoints1);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(143, matches[0].Length);
                Assert.AreEqual(143, matches[1].Length);
                Assert.AreEqual(keyPoints2[0].ToIntPoint(), matches[0][0]);
                Assert.AreEqual(532, matches[1][0].X);
                Assert.AreEqual(159, matches[1][0].Y);
            }

        }

        [Test]
        [Category("Random")]
        public void MatchTest3_Compatibility()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var old = Accord.Imaging.Image.Clone(Resources.old);
            var flower01 = Accord.Imaging.Image.Clone(Resources.flower01);

            FastCornersDetector fast = new FastCornersDetector(threshold: 10);

            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(fast);

            var keyPoints1 = freak.ProcessImage(old).ToArray();
            var keyPoints2 = freak.ProcessImage(flower01).ToArray();

            var matcher = new KNearestNeighborMatching<byte[]>(5, Distance.BitwiseHamming);

            { // direct
                IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(143, matches[0].Length);
                Assert.AreEqual(143, matches[1].Length);
                Assert.AreEqual(532, matches[0][0].X);
                Assert.AreEqual(159, matches[0][0].Y);
                Assert.AreEqual(keyPoints2[0].ToIntPoint(), matches[1][0]);
            }

            { // reverse
                IntPoint[][] matches = matcher.Match(keyPoints2, keyPoints1);
                Assert.AreEqual(2, matches.Length);
                Assert.AreEqual(143, matches[0].Length);
                Assert.AreEqual(143, matches[1].Length);
                Assert.AreEqual(keyPoints2[0].ToIntPoint(), matches[0][0]);
                Assert.AreEqual(532, matches[1][0].X);
                Assert.AreEqual(159, matches[1][0].Y);
            }

        }

    }
}
