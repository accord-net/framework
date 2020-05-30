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
    using Accord.Tests.Imaging.Properties;
    using AForge;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Drawing;

    [TestFixture]
    public class SusanCornersDetectorTests
    {
        [Test]
        public void Can_Process_8bpp()
        {
            // test for #1974

            UnmanagedImage image = UnmanagedImage.FromManagedImage(Accord.Imaging.Image.Clone(Resources.image1));

            Assert.AreEqual(System.Drawing.Imaging.PixelFormat.Format8bppIndexed, image.PixelFormat);

            SusanCornersDetector detector = new SusanCornersDetector(25, 18);

            // this shouldn't throw
            var points = detector.ProcessImage(image);
        }
    }
}