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
    using Accord.Imaging.Moments;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;
    using Accord.Imaging.Converters;
    using Accord.Math;
    using System.Drawing;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class HuMomentsTest
    {

        [Test]
        public void ComputeTest()
        {
            var hu0 = new HuMoments(Accord.Imaging.Image.Clone(Resources.hu0));
            var hu1 = new HuMoments(Accord.Imaging.Image.Clone(Resources.hu1));

            Assert.AreEqual(hu0.I1 / hu1.I1, 1, 0.12);
            Assert.AreEqual(hu0.I2 / hu1.I2, 1, 0.5);
            Assert.AreEqual(hu0.I3 / hu1.I3, 1, 0.7);
        }

        [Test]
        public void ComputeTest2()
        {
            float[,] img1; new ImageToMatrix().Convert(Accord.Imaging.Image.Clone(Resources.tt1), out img1);
            float[,] img2; new ImageToMatrix().Convert(Accord.Imaging.Image.Clone(Resources.tt2), out img2);
            float[,] img3; new ImageToMatrix().Convert(Accord.Imaging.Image.Clone(Resources.tt3), out img3);
            float[,] img4; new ImageToMatrix().Convert(Accord.Imaging.Image.Clone(Resources.tt4), out img4);

            var hu1 = new HuMoments(img1);
            var hu2 = new HuMoments(img2);
            var hu3 = new HuMoments(img3);
            var hu4 = new HuMoments(img4);

            Assert.AreEqual(hu1.I1, hu2.I1, 0.015);
            Assert.AreEqual(hu1.I2, hu2.I2, 0.015);
            Assert.AreEqual(hu1.I3, hu2.I3, 1e-5);
            Assert.AreEqual(hu1.I4, hu2.I4, 1e-4);
            Assert.AreEqual(hu1.I5, hu2.I5, 1e-5);
            Assert.AreEqual(hu1.I6, hu2.I6, 1e-5);

            Assert.AreEqual(hu3.I1, hu4.I1, 1e-3);
            Assert.AreEqual(hu3.I2, hu4.I2, 1e-4);
            Assert.AreEqual(hu3.I3, hu4.I3, 1e-5);
            Assert.AreEqual(hu3.I4, hu4.I4, 1e-4);
            Assert.AreEqual(hu3.I5, hu4.I5, 1e-5);
            Assert.AreEqual(hu3.I6, hu4.I6, 1e-5);
        }
    }
}
