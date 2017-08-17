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
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using Accord.Tests.Vision.Properties;
    using NUnit.Framework;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
#if NO_BITMAP
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif


    [TestFixture]
    public class BlendTest
    {

        [Test, Ignore("Random")]
        public void Panorama_Example1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Let's start with two pictures that have been
            // taken from slightly different points of view:
            //
            Bitmap img1 = Accord.Imaging.Image.Clone(Resources.dc_left);
            Bitmap img2 = Accord.Imaging.Image.Clone(Resources.dc_right);

            // Those pictures are shown below:
            // ImageBox.Show(img1, PictureBoxSizeMode.Zoom, 640, 480);
            // ImageBox.Show(img2, PictureBoxSizeMode.Zoom, 640, 480);


            // Step 1: Detect feature points using Surf Corners Detector
            var surf = new SpeededUpRobustFeaturesDetector();

            var points1 = surf.ProcessImage(img1);
            var points2 = surf.ProcessImage(img2);

            // Step 2: Match feature points using a k-NN
            var matcher = new KNearestNeighborMatching(5);
            var matches = matcher.Match(points1, points2);

            // Step 3: Create the matrix using a robust estimator
            var ransac = new RansacHomographyEstimator(0.001, 0.99);
            MatrixH homographyMatrix = ransac.Estimate(matches);

            Assert.AreEqual(1.15707409, homographyMatrix.Elements[0], 1e-5);
            Assert.AreEqual(-0.0233834628, homographyMatrix.Elements[1], 1e-5);
            Assert.AreEqual(-261.8217, homographyMatrix.Elements[2], 1e-2);
            Assert.AreEqual(0.08801343, homographyMatrix.Elements[3], 1e-5);
            Assert.AreEqual(1.12451434, homographyMatrix.Elements[4], 1e-5);
            Assert.AreEqual(-171.191208, homographyMatrix.Elements[5], 1e-2);
            Assert.AreEqual(0.000127789128, homographyMatrix.Elements[6], 1e-5);
            Assert.AreEqual(0.00006173445, homographyMatrix.Elements[7], 1e-5);
            Assert.AreEqual(8, homographyMatrix.Elements.Length);


            // Step 4: Project and blend using the homography
            Blend blend = new Blend(homographyMatrix, img1);


            // Compute the blending algorithm
            Bitmap result = blend.Apply(img2);

            // Show on screen
            // ImageBox.Show(result, PictureBoxSizeMode.Zoom, 640, 480);


            result = Accord.Imaging.Image.Clone(result);

#if NET35
            // result.Save(@"C:\Projects\Accord.NET\framework\Unit Tests\Accord.Tests.Imaging\Resources\blend_net35.png", ImageFormat.Png);
            Bitmap image = Accord.Imaging.Image.Clone(Resources.blend_net35);
#else
            // result.Save(@"C:\Projects\Accord.NET\framework\Unit Tests\Accord.Tests.Imaging\Resources\blend_net45.png", ImageFormat.Png);
            Bitmap image = Accord.Imaging.Image.Clone(Resources.blend_net45);
#endif

            double[,] expected; new ImageToMatrix().Convert(image, out expected);
            double[,] actual; new ImageToMatrix().Convert(result, out actual);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 0.1));
        }


        [Test]
        public void ApplyTest()
        {
            var img1 = Accord.Imaging.Image.Clone(Resources.image2);
            var img2 = Accord.Imaging.Image.Clone(Resources.image2);

            MatrixH homography = new MatrixH(1, 0, 32,
                                             0, 1, 0,
                                             0, 0);

            Blend blend = new Blend(homography, img1);
            var actual = blend.Apply(img2);

            Assert.AreEqual(64, actual.Size.Width);
            Assert.AreEqual(32, actual.Size.Height);



            homography = new MatrixH(1, 0, 32,
                                     0, 1, 32,
                                     0, 0);

            blend = new Blend(homography, img1);
            actual = blend.Apply(img2);


            Assert.AreEqual(64, actual.Size.Width);
            Assert.AreEqual(64, actual.Size.Height);

            // ImageBox.Show(img3, PictureBoxSizeMode.Zoom);
        }

        [Test]
        public void ApplyTest2()
        {
            var img1 = Accord.Imaging.Image.Clone(Resources.image2);
            var img2 = Accord.Imaging.Image.Clone(Resources.image2);

            var img3 = Accord.Imaging.Image.Clone(Resources.image2);
            var img4 = Accord.Imaging.Image.Clone(Resources.image2);


            MatrixH homography;
            Blend blend;

            homography = new MatrixH(1, 0, 32,
                                     0, 1, 0,
                                     0, 0);


            blend = new Blend(homography, img1);
            var img12 = blend.Apply(img2);

            //ImageBox.Show("Blend of 1 and 2", img12, PictureBoxSizeMode.Zoom);
            Assert.AreEqual(img12.PixelFormat, PixelFormat.Format32bppArgb);

            blend = new Blend(homography, img3);
            var img34 = blend.Apply(img4);

            //ImageBox.Show("Blend of 3 and 4", img34, PictureBoxSizeMode.Zoom);
            Assert.AreEqual(img34.PixelFormat, PixelFormat.Format32bppArgb);


            homography = new MatrixH(1, 0, 64,
                                     0, 1, 0,
                                     0, 0);


            blend = new Blend(homography, img12);
            var img1234 = blend.Apply(img34);

            //ImageBox.Show("Blend of 1, 2, 3, 4", img1234, PictureBoxSizeMode.Zoom);
            Assert.AreEqual(img1234.PixelFormat, PixelFormat.Format32bppArgb);



            // Blend of 1 and 5 (8bpp and 32bpp)
            homography = new MatrixH(1, 0, 0,
                                     0, 1, 32,
                                     0, 0);


            //ImageBox.Show("Image 1", img1, PictureBoxSizeMode.Zoom);
            blend = new Blend(homography, img1234);
            var img15 = blend.Apply(img1);
            //ImageBox.Show("Blend of 1 and 5", img15, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(img1234.PixelFormat, PixelFormat.Format32bppArgb);
            Assert.AreEqual(img1.PixelFormat, PixelFormat.Format8bppIndexed);
            Assert.AreEqual(img15.PixelFormat, PixelFormat.Format32bppArgb);
            Assert.AreEqual(128, img15.Width);
            Assert.AreEqual(64, img15.Height);

        }

    }
}
