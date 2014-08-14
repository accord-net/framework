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

namespace Accord.Tests.Imaging
{
    using System.Drawing;
    using Accord.Imaging.Moments;
    using Accord.Tests.Imaging.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Imaging.Converters;
    using Accord.Math;


    [TestClass()]
    public class RawMomentsTest
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
        public void RawMomentsConstructorTest()
        {
            Bitmap image = Resources.hu;

            RawMoments target = new RawMoments(image, order: 3);

            Assert.AreEqual(86424.0 / target.M00, 1);
            Assert.AreEqual(1.177254E+7 / target.M01, 1, 1e-10);
            Assert.AreEqual(2.19032653E+9 / target.M02, 1, 1e-8);

            Assert.AreEqual(1.5264756E7 / target.M10, 1, 1e-10);
            Assert.AreEqual(2.08566813E9 / target.M11, 1, 1e-6);
            Assert.AreEqual(3.8643911413E11 / target.M12, 1, 1e-5);
            Assert.AreEqual(3.604560164E9 / target.M20, 1, 1e-3);
            Assert.AreEqual(4.9401212329E11 / target.M21, 1, 1e-3);

            Assert.AreEqual(9.451364E+11 / target.M30, 1, 1e-8);
            Assert.AreEqual(4.599169E+11 / target.M03, 1, 1e-8);
        }

        [TestMethod()]
        public void RawMomentsConstructorTest2()
        {
            float[,] image; new ImageToMatrix()
                {
                    Min = 0,
                    Max = 255
                }.Convert(Resources.hu, out image);

            RawMoments target = new RawMoments(image, order: 3);

            Assert.AreEqual(86424.0 / target.M00, 1);
            Assert.AreEqual(1.177254E+7 / target.M01, 1, 1e-10);
            Assert.AreEqual(2.19032653E+9 / target.M02, 1, 1e-8);

            Assert.AreEqual(1.5264756E7 / target.M10, 1, 1e-10);
            Assert.AreEqual(2.08566813E9 / target.M11, 1, 1e-6);
            Assert.AreEqual(3.8643911413E11 / target.M12, 1, 1e-5);
            Assert.AreEqual(3.604560164E9 / target.M20, 1, 1e-3);
            Assert.AreEqual(4.9401212329E11 / target.M21, 1, 1e-3);

            Assert.AreEqual(9.451364E+11 / target.M30, 1, 1e-8);
            Assert.AreEqual(4.599169E+11 / target.M03, 1, 1e-8);
        }

        [TestMethod()]
        public void RawMomentsConstructorTest3()
        {
            float[,] img = 
            {
                { 1, 2, 3, 4, 5 },
                { 6, 7, 8, 9, 10 },
                { 11, 12, 13, 14, 15 },
                { 16, 17, 18, 19, 20 },
            };

            double sum = img.Sum().Sum();

            RawMoments raw = new RawMoments(img);

            Assert.AreEqual(sum, raw.M00);
        }
    }
}
