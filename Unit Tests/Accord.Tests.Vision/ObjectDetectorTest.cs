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

namespace Accord.Tests.Vision
{
    using Accord.Vision;
    using NUnit.Framework;
    using System.Drawing;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;
    using Accord.Tests.Vision.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class ObjectDetectorTest
    {

        [Test]
        public void ProcessFrame()
        {
            #region doc_example
            // In order to use a HaarObjectDetector, first we have to tell it
            // which type of objects we would like to detect. And in a Haar detector,
            // different object classifiers are specified in terms of a HaarCascade.

            // The framework comes with some built-in cascades for common body
            // parts, such as Face and Nose. However, it is also possible to
            // load a cascade from cascade XML definitions in OpenCV 2.0 format.

            // In this example, we will be creating a cascade for a Face detector:
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();

            // Note: In the case we would like to load it from XML, we could use:
            // var cascade = HaarCascade.FromXml("filename.xml");

            // Now, create a new Haar object detector with the cascade:
            var detector = new HaarObjectDetector(cascade, minSize: 50, 
                searchMode: ObjectDetectorSearchMode.NoOverlap);

            // Note that we have specified that we do not want overlapping objects,
            // and that the minimum object an object can have is 50 pixels. Now, we
            // can use the detector to classify a new image. For instance, consider
            // the famous Lena picture:

            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.lena_color);

            // We have to call ProcessFrame to detect all rectangles containing the 
            // object we are interested in (which in this case, is the face of Lena):
            Rectangle[] rectangles = detector.ProcessFrame(bmp);

            // The answer will be a single rectangle of dimensions
            //
            //   {X = 126 Y = 112 Width = 59 Height = 59}
            //
            // which indeed contains the only face in the picture.
            #endregion

            Assert.AreEqual(1, detector.DetectedObjects.Length);
            Assert.AreEqual(126, detector.DetectedObjects[0].X);
            Assert.AreEqual(112, detector.DetectedObjects[0].Y);
            Assert.AreEqual(59, detector.DetectedObjects[0].Width);
            Assert.AreEqual(59, detector.DetectedObjects[0].Height);
        }

        [Test]
        public void ProcessFrame2()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                30, ObjectDetectorSearchMode.NoOverlap);

            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.lena_gray);

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

        [Test]
        public void ProcessFrame3()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                15, ObjectDetectorSearchMode.NoOverlap);

            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.three);

            target.ProcessFrame(bmp);

            Assert.AreEqual(2, target.DetectedObjects.Length);

            int i = 0;
           /* Assert.AreEqual(180, target.DetectedObjects[0].X);
            Assert.AreEqual(275, target.DetectedObjects[0].Y);
            Assert.AreEqual(41, target.DetectedObjects[0].Width);
            Assert.AreEqual(41, target.DetectedObjects[0].Height);
            */
            Assert.AreEqual(168, target.DetectedObjects[i].X);
            Assert.AreEqual(144, target.DetectedObjects[i].Y);
            Assert.AreEqual(49, target.DetectedObjects[i].Width);
            Assert.AreEqual(49, target.DetectedObjects[i].Height);

            i++;
            Assert.AreEqual(392, target.DetectedObjects[i].X);
            Assert.AreEqual(133, target.DetectedObjects[i].Y);
            Assert.AreEqual(59, target.DetectedObjects[i].Width);
            Assert.AreEqual(59, target.DetectedObjects[i].Height);


            target = new HaarObjectDetector(cascade,
                15, ObjectDetectorSearchMode.Single);

            target.ProcessFrame(bmp);

            Assert.AreEqual(1, target.DetectedObjects.Length);
        }

        [Test, Category("Random")]
        public void MinSizeTest()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                50, ObjectDetectorSearchMode.Default);

            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.lena_color);
            Rectangle[] result;

            target.MinSize = new Size(10, 60);
            result = target.ProcessFrame(bmp);
            Assert.AreEqual(3, result.Length); // Mono outputs 6 instead of 3
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

        [Test]
        public void MaxSizeTest()
        {
            HaarCascade cascade = new FaceHaarCascade();
            HaarObjectDetector target = new HaarObjectDetector(cascade,
                50, ObjectDetectorSearchMode.Default);

            Bitmap bmp = Accord.Imaging.Image.Clone(Resources.lena_color);
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
