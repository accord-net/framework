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

using Accord.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AForge.Math;

namespace Accord.Tests.Math
{


    [TestClass()]
    public class HilbertTransformTest
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
        public void FHTTest()
        {
            Complex[] original = { (Complex)1, (Complex)2, (Complex)3, (Complex)4 };

            Complex[] actual = (Complex[])original.Clone();
            HilbertTransform.FHT(actual, FourierTransform.Direction.Forward);

            Assert.AreEqual(actual[0].Re, 1);
            Assert.AreEqual(actual[1].Re, 2);
            Assert.AreEqual(actual[2].Re, 3);
            Assert.AreEqual(actual[3].Re, 4);

            Assert.AreEqual(actual[0].Im, +1, 0.000000001);
            Assert.AreEqual(actual[1].Im, -1, 0.000000001);
            Assert.AreEqual(actual[2].Im, -1, 0.000000001);
            Assert.AreEqual(actual[3].Im, +1, 0.000000001);

            HilbertTransform.FHT(actual, FourierTransform.Direction.Backward);

            Assert.AreEqual(actual[0], original[0]);
            Assert.AreEqual(actual[1], original[1]);
            Assert.AreEqual(actual[2], original[2]);
            Assert.AreEqual(actual[3], original[3]);
        }

        [TestMethod()]
        public void FHTTest2()
        {
            double[] original = { -1.0, -0.8, -0.2, -0.1, 0.1, 0.2, 0.8, 1.0  };

            double[] actual = (double[])original.Clone();

            HilbertTransform.FHT(actual, FourierTransform.Direction.Forward);

            HilbertTransform.FHT(actual, FourierTransform.Direction.Backward);

            Assert.AreEqual(actual[0], original[0], 0.08);
            Assert.AreEqual(actual[1], original[1], 0.08);
            Assert.AreEqual(actual[2], original[2], 0.08);
            Assert.AreEqual(actual[3], original[3], 0.08);
        }
    }
}
