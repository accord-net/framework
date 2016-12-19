﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

using Accord.Imaging.Filters;
using NUnit.Framework;
using System.Drawing;

namespace Accord.Tests.Imaging
{


    /// <summary>
    ///This is a test class for ConcatenateTest and is intended
    ///to contain all ConcatenateTest Unit Tests
    ///</summary>
    [TestFixture]
    public class ConcatenateTest
    {
        

        [Test]
        public void ConcatenateConstructorTest()
        {
            Bitmap img1 = Accord.Imaging.Image.Clone(Properties.Resources.image1);
            Bitmap img2 = Accord.Imaging.Image.Clone(Properties.Resources.image2);

            Concatenate target = new Concatenate(img1);
            var img3 = target.Apply(img2);

            Assert.AreEqual(System.Math.Max(img1.Height, img2.Height), img3.Height);
            Assert.AreEqual(img1.Width + img2.Width, img3.Width);



            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                    Assert.AreEqual(img1.GetPixel(i, j), img3.GetPixel(i, j));

            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                    Assert.AreEqual(img2.GetPixel(i, j), img3.GetPixel(i + 16, j));

        }
    }
}
