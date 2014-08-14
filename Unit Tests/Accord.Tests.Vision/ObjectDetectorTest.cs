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

namespace Accord.Tests.Vision
{
    using Accord.Vision;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;
    using Accord.Controls;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;

    [TestClass()]
    public class ObjectDetectorTest
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
        public void ProcessFrame()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                50, ObjectDetectorSearchMode.NoOverlap);

            Bitmap bmp = Properties.Resources.lena_color;

            target.ProcessFrame(bmp);

            Assert.AreEqual(1, target.DetectedObjects.Length);
            Assert.AreEqual(126, target.DetectedObjects[0].X);
            Assert.AreEqual(112, target.DetectedObjects[0].Y);
            Assert.AreEqual(59, target.DetectedObjects[0].Width);
            Assert.AreEqual(59, target.DetectedObjects[0].Height);
        }

        [TestMethod()]
        public void ProcessFrame2()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                30, ObjectDetectorSearchMode.NoOverlap);

            Bitmap bmp = Properties.Resources.lena_gray;

            target.ProcessFrame(bmp);

            Assert.AreEqual(1, target.DetectedObjects.Length);
            Assert.AreEqual(255, target.DetectedObjects[0].X);
            Assert.AreEqual(225, target.DetectedObjects[0].Y);
            Assert.AreEqual(123, target.DetectedObjects[0].Width);
            Assert.AreEqual(123, target.DetectedObjects[0].Height);


            target = new HaarObjectDetector(cascade,
                30, ObjectDetectorSearchMode.Default);

            target.ProcessFrame(bmp);

            Assert.AreEqual(6, target.DetectedObjects.Length);
            Assert.AreEqual(255, target.DetectedObjects[0].X);
            Assert.AreEqual(225, target.DetectedObjects[0].Y);
            Assert.AreEqual(123, target.DetectedObjects[0].Width);
            Assert.AreEqual(123, target.DetectedObjects[0].Height);
        }

        [TestMethod()]
        public void ProcessFrame3()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                15, ObjectDetectorSearchMode.NoOverlap);

            Bitmap bmp = Properties.Resources.three;

            target.ProcessFrame(bmp);

            Assert.AreEqual(3, target.DetectedObjects.Length);
            Assert.AreEqual(180, target.DetectedObjects[0].X);
            Assert.AreEqual(275, target.DetectedObjects[0].Y);
            Assert.AreEqual(41, target.DetectedObjects[0].Width);
            Assert.AreEqual(41, target.DetectedObjects[0].Height);

            Assert.AreEqual(168, target.DetectedObjects[1].X);
            Assert.AreEqual(144, target.DetectedObjects[1].Y);
            Assert.AreEqual(49, target.DetectedObjects[1].Width);
            Assert.AreEqual(49, target.DetectedObjects[1].Height);

            Assert.AreEqual(392, target.DetectedObjects[2].X);
            Assert.AreEqual(133, target.DetectedObjects[2].Y);
            Assert.AreEqual(59, target.DetectedObjects[2].Width);
            Assert.AreEqual(59, target.DetectedObjects[2].Height);


            target = new HaarObjectDetector(cascade,
                15, ObjectDetectorSearchMode.Single);

            target.ProcessFrame(bmp);

            Assert.AreEqual(1, target.DetectedObjects.Length);
        }

        [TestMethod()]
        public void MinSizeTest()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                50, ObjectDetectorSearchMode.Default);

            Bitmap bmp = Properties.Resources.lena_color;
            Rectangle[] result;

            target.MinSize = new Size(10, 60);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(3, result.Length);
            foreach (var r in result)
            {
                Assert.IsTrue(r.Width >= target.MinSize.Width);
                Assert.IsTrue(r.Height >= target.MinSize.Height);
            }


            target.MinSize = new Size(85, 85);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(2, result.Length);
            foreach (var r in result)
            {
                Assert.IsTrue(r.Width >= target.MinSize.Width);
                Assert.IsTrue(r.Height >= target.MinSize.Height);
            }

            target.MinSize = new Size(1, 1);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(4, result.Length);
            foreach (var r in result)
            {
                Assert.IsTrue(r.Width >= target.MinSize.Width);
                Assert.IsTrue(r.Height >= target.MinSize.Height);
            }
        }

        [TestMethod()]
        public void MaxSizeTest()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                50, ObjectDetectorSearchMode.Default);

            Bitmap bmp = Properties.Resources.lena_color;
            Rectangle[] result;

            target.MaxSize = new Size(10, 60);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(0, result.Length);
           
            target.MaxSize = new Size(60, 60);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(1, result.Length);
            foreach (var r in result)
            {
                Assert.IsTrue(r.Width <= target.MaxSize.Width);
                Assert.IsTrue(r.Height <= target.MaxSize.Height);
            }

            target.MaxSize = new Size(80, 80);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(2, result.Length);
            foreach (var r in result)
            {
                Assert.IsTrue(r.Width <= target.MaxSize.Width);
                Assert.IsTrue(r.Height <= target.MaxSize.Height);
            }

        }
    }
}
