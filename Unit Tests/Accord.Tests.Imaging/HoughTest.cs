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

namespace Accord.Imaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using Accord.Imaging;
    using NUnit.Framework;
    using Accord.DataSets;
    using Accord.Tests.Imaging.Properties;
    using Accord.Imaging.Filters;
    using Accord.Math;

    [TestFixture]
    public class HoughTest
    {
        [Test]
        public void hough_line()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;

            #region doc_apply_part1
            // Let's use a standard test image to show how to apply
            // the Hough image transform. For this, we can load an
            // example image using the TestImages class:
            var testImages = new Accord.DataSets.TestImages(basePath);

            // We'll use the sudoku test image from OpenCV:
            Bitmap image = testImages.GetImage("sudoku.png");

            // Convert it to binary and mark the possible lines 
            // in white so it can be processed by the transform
            var sequence = new FiltersSequence(
                Grayscale.CommonAlgorithms.BT709,
                new NiblackThreshold(),
                new Invert()
            );

            // Apply the sequence of filters above:
            Bitmap binaryImage = sequence.Apply(image);

            // Finally, we can create a Line Hough Transform:
            var lineTransform = new HoughLineTransformation();

            // and then apply it to the binary image:
            lineTransform.ProcessImage(binaryImage);

            // the output of the transform can be seen using
            Bitmap houghLineImage = lineTransform.ToBitmap();

            // For visualization purposes, we can either display the result of the
            // transform on the screen or save it to disk to be visualized later:
            // ImageBox.Show(houghLineImage);
            // houghLineImage.Save("hough-output.png");
            #endregion

            #region doc_apply_part2
            // Now, let's say we would like to retrieve the lines and use them
            // for further processing. First, the lines can be ordered by their
            // relative intensity using
            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.9);

            // Then, let's plot them on top of the input image. Since we will
            // apply many operations to a single image, it is better to first
            // convert it to an UnmanagedImage object to avoid having to lock
            // the image into memory multiple times.

            UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(image);

            // Finally, plot them in order:
            foreach (HoughLine line in lines)
            {
                line.Draw(unmanagedImage, color: Color.Red);
            }

            // ImageBox.Show(unmanagedImage);
            // unmanagedImage.ToManagedImage().Save("hough-lines.png");
            #endregion

            Assert.AreEqual(18, lines.Length);

            lines = lineTransform.GetLinesByRelativeIntensity(0.95);
            Assert.AreEqual(6, lines.Length);
            Assert.AreEqual(89, lines[0].Theta);
            Assert.AreEqual(-236, lines[0].Radius);
            Assert.AreEqual(531, lines[0].Intensity);
            Assert.AreEqual(1, lines[0].RelativeIntensity, 1e-8);

            Assert.AreEqual(4, lines[1].Theta);
            Assert.AreEqual(224, lines[1].Radius);
            Assert.AreEqual(516, lines[1].Intensity);
            Assert.AreEqual(0.97175141242937857, lines[1].RelativeIntensity, 1e-8);

            Assert.AreEqual(90, lines[2].Theta);
            Assert.AreEqual(-129, lines[2].Radius);
            Assert.AreEqual(508, lines[2].Intensity);
            Assert.AreEqual(0.95668549905838041, lines[2].RelativeIntensity, 1e-8);

            Assert.AreEqual(90, lines[3].Theta);
            Assert.AreEqual(-77, lines[3].Radius);
            Assert.AreEqual(507, lines[3].Intensity);
            Assert.AreEqual(0.95480225988700562, lines[3].RelativeIntensity, 1e-8);

            Assert.AreEqual(2, lines[4].Theta);
            Assert.AreEqual(123, lines[4].Radius);
            Assert.AreEqual(505, lines[4].Intensity);
            Assert.AreEqual(0.95103578154425616, lines[4].RelativeIntensity, 1e-8);

            Assert.AreEqual(179, lines[5].Theta);
            Assert.AreEqual(26, lines[5].Radius);
            Assert.AreEqual(505, lines[5].Intensity);
            Assert.AreEqual(0.95103578154425616, lines[5].RelativeIntensity, 1e-8);
        }

    }
}
