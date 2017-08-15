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
    using Accord.Math.Geometry;
    using AForge;
    using Accord.Imaging.Filters;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Tests.Imaging.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class KCurvatureTest
    {


        [Test]
        public void FindPeaksTest()
        {
            Bitmap hand = Accord.Imaging.Image.Clone(Resources.rhand);

            GaussianBlur median = new GaussianBlur(1.1);
            median.ApplyInPlace(hand);

            // Extract contour
            BorderFollowing bf = new BorderFollowing(1);
            List<IntPoint> contour = bf.FindContour(hand);

            hand = hand.Clone(new Rectangle(0, 0, hand.Width, hand.Height), PixelFormat.Format24bppRgb);

            // Find peaks
            KCurvature kcurv = new KCurvature(30, new DoubleRange(0, 45));
            // kcurv.Suppression = 30;
            var peaks = kcurv.FindPeaks(contour);

            List<IntPoint> supports = new List<IntPoint>();
            for (int i = 0; i < peaks.Count; i++)
            {
                int j = contour.IndexOf(peaks[i]);
                supports.Add(contour[(j + kcurv.K) % contour.Count]);
                supports.Add(contour[Accord.Math.Tools.Mod(j - kcurv.K, contour.Count)]);
            }

            // show(hand, contour, peaks, supports);

            Assert.AreEqual(2, peaks.Count);
            Assert.AreEqual(46, peaks[0].X);
            Assert.AreEqual(0, peaks[0].Y);
            Assert.AreEqual(2, peaks[1].X);
            Assert.AreEqual(11, peaks[1].Y);
        }

/*
        private static void show(Bitmap hand, List<IntPoint> contour, List<IntPoint> peaks, List<IntPoint> supports)
        {
            PointsMarker cmarker = new PointsMarker(contour, Color.White, 1);
            cmarker.ApplyInPlace(hand);

            PointsMarker pmarker = new PointsMarker(peaks, Color.Green, 5);
            pmarker.ApplyInPlace(hand);

            PointsMarker hmarker = new PointsMarker(supports, Color.Yellow, 5);
            hmarker.ApplyInPlace(hand);

            ImageBox.Show(hand, PictureBoxSizeMode.Zoom);
        }
*/

        [Test]
        public void FindPeaksTest2()
        {
            Bitmap hand = Accord.Imaging.Image.Clone(Resources.rhand0);

            //  ImageBox.Show(hand);

            GaussianBlur median = new GaussianBlur(1.1);
            median.ApplyInPlace(hand);

            // Extract contour
            BorderFollowing bf = new BorderFollowing(20);
            List<IntPoint> contour = bf.FindContour(hand);

            hand = hand.Clone(new Rectangle(0, 0, hand.Width, hand.Height), PixelFormat.Format24bppRgb);

            // Find peaks
            KCurvature kcurv = new KCurvature(30, new DoubleRange(0, 45));
            var peaks = kcurv.FindPeaks(contour);

            List<IntPoint> supports = new List<IntPoint>();
            for (int i = 0; i < peaks.Count; i++)
            {
                int j = contour.IndexOf(peaks[i]);
                supports.Add(contour[(j + kcurv.K) % contour.Count]);
                supports.Add(contour[Accord.Math.Tools.Mod(j - kcurv.K, contour.Count)]);
            }

            // show(hand, contour, peaks, supports);

            Assert.AreEqual(5, peaks.Count);
            Assert.AreEqual(0, peaks[0].X);
            Assert.AreEqual(80, peaks[0].Y);
            Assert.AreEqual(113, peaks[1].X);
            Assert.AreEqual(26, peaks[1].Y);
            Assert.AreEqual(98, peaks[2].X);
            Assert.AreEqual(9, peaks[2].Y);
            Assert.AreEqual(73, peaks[3].X);
            Assert.AreEqual(2, peaks[3].Y);
            Assert.AreEqual(38, peaks[4].X);
            Assert.AreEqual(14, peaks[4].Y);
        }

   
        [Test]
        public void FindPeaksTest3()
        {
            Bitmap hand = Accord.Imaging.Image.Clone(Resources.rhand1);

            GaussianBlur median = new GaussianBlur(1.1);
            median.ApplyInPlace(hand);

            // Extract contour
            BorderFollowing bf = new BorderFollowing(20);
            List<IntPoint> contour = bf.FindContour(hand);

            hand = hand.Clone(new Rectangle(0, 0, hand.Width, hand.Height), PixelFormat.Format24bppRgb);

            // Find peaks
            KCurvature kcurv = new KCurvature(30, new DoubleRange(0, 45));
            var peaks = kcurv.FindPeaks(contour);

            List<IntPoint> supports = new List<IntPoint>();
            for (int i = 0; i < peaks.Count; i++)
            {
                int j = contour.IndexOf(peaks[i]);
                supports.Add(contour[(j + kcurv.K) % contour.Count]);
                supports.Add(contour[Accord.Math.Tools.Mod(j - kcurv.K, contour.Count)]);
            }

            // show(hand, contour, peaks, supports);

            Assert.AreEqual(2, peaks.Count);
            Assert.AreEqual(38, peaks[0].X);
            Assert.AreEqual(2, peaks[0].Y);
            Assert.AreEqual(1, peaks[1].X);
            Assert.AreEqual(15, peaks[1].Y);
        }

        [Test]
        public void FindPeaksTest4()
        {
            Bitmap hand = Accord.Imaging.Image.Clone(Resources.rhand2);

            GaussianBlur median = new GaussianBlur(1.1);
            median.ApplyInPlace(hand);

            // Extract contour
            BorderFollowing bf = new BorderFollowing(20);
            List<IntPoint> contour = bf.FindContour(hand);

            hand = hand.Clone(new Rectangle(0, 0, hand.Width, hand.Height), PixelFormat.Format24bppRgb);

            // Find peaks
            KCurvature kcurv = new KCurvature(30, new DoubleRange(0, 45));
            var peaks = kcurv.FindPeaks(contour);

            List<IntPoint> supports = new List<IntPoint>();
            for (int i = 0; i < peaks.Count; i++)
            {
                int j = contour.IndexOf(peaks[i]);
                supports.Add(contour[(j + kcurv.K) % contour.Count]);
                supports.Add(contour[Accord.Math.Tools.Mod(j - kcurv.K, contour.Count)]);
            }

            // show(hand, contour, peaks, supports);

            Assert.AreEqual(1, peaks.Count);
            Assert.AreEqual(18, peaks[0].X);
            Assert.AreEqual(0, peaks[0].Y);
        }


        [Test]
        public void FindPeaksTest6()
        {
            Bitmap hand = Accord.Imaging.Image.Clone(Resources.rhand3);

            GaussianBlur median = new GaussianBlur(1.1);
            median.ApplyInPlace(hand);

            // Extract contour
            BorderFollowing bf = new BorderFollowing(1);
            List<IntPoint> contour = bf.FindContour(hand);

            hand = hand.Clone(new Rectangle(0, 0, hand.Width, hand.Height), PixelFormat.Format24bppRgb);

            // Find peaks
            KCurvature kcurv = new KCurvature(30, new DoubleRange(0, 45));
            var peaks = kcurv.FindPeaks(contour);

            List<IntPoint> supports = new List<IntPoint>();
            for (int i = 0; i < peaks.Count; i++)
            {
                int j = contour.IndexOf(peaks[i]);
                supports.Add(contour[(j + kcurv.K) % contour.Count]);
                supports.Add(contour[Accord.Math.Tools.Mod(j - kcurv.K, contour.Count)]);
            }

            // show(hand, contour, peaks, supports);

            Assert.AreEqual(5, peaks.Count);
            Assert.AreEqual(0, peaks[0].X);
            Assert.AreEqual(95, peaks[0].Y);
            Assert.AreEqual(98, peaks[1].X);
            Assert.AreEqual(13, peaks[1].Y);
            Assert.AreEqual(69, peaks[2].X);
            Assert.AreEqual(0, peaks[2].Y);
            Assert.AreEqual(39, peaks[3].X);
            Assert.AreEqual(0, peaks[3].Y);
            Assert.AreEqual(17, peaks[4].X);
            Assert.AreEqual(17, peaks[4].Y);
        }
    }
}
