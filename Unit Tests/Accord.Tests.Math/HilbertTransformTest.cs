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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using NUnit.Framework;
    using Accord.Compat;
    using System.Numerics;

    [TestFixture]
    public class HilbertTransformTest
    {

        [Test]
        public void FHTTest()
        {
            Complex[] original = { (Complex)1, (Complex)2, (Complex)3, (Complex)4 };

            Complex[] actual = (Complex[])original.Clone();
            HilbertTransform.FHT(actual, FourierTransform.Direction.Forward);

            Assert.AreEqual(actual[0].Real, 1);
            Assert.AreEqual(actual[1].Real, 2);
            Assert.AreEqual(actual[2].Real, 3);
            Assert.AreEqual(actual[3].Real, 4);

            Assert.AreEqual(actual[0].Imaginary, +1, 0.000000001);
            Assert.AreEqual(actual[1].Imaginary, -1, 0.000000001);
            Assert.AreEqual(actual[2].Imaginary, -1, 0.000000001);
            Assert.AreEqual(actual[3].Imaginary, +1, 0.000000001);

            HilbertTransform.FHT(actual, FourierTransform.Direction.Backward);

            Assert.AreEqual(actual[0], original[0]);
            Assert.AreEqual(actual[1], original[1]);
            Assert.AreEqual(actual[2], original[2]);
            Assert.AreEqual(actual[3], original[3]);
        }

        [Test]
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
