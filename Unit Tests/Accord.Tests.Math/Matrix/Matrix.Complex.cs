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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math.ComplexExtensions;
    using AForge.Math;
    using AForge;

    [TestClass()]
    public class ComplexMatrixTest
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
        public void AbsTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            Complex[] expected = { new Complex(Math.Sqrt(26), 0), new Complex(Math.Sqrt(5), 0), new Complex(Math.Sqrt(26), 0) };
            Complex[] actual = ComplexMatrix.Abs(x);
            Assert.IsTrue(expected.IsEqual(actual, 1e-5));
        }

        [TestMethod()]
        public void ImTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[] expected = { 5, -1, 1 };
            double[] actual = ComplexMatrix.Im(x);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void MagnitudeTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[] expected = { Math.Sqrt(26), Math.Sqrt(5), Math.Sqrt(26) };
            double[] actual = ComplexMatrix.Magnitude(x);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void MultiplyTest()
        {
            Complex[] a = { new Complex(7, 5), new Complex(2, -3), new Complex(-5, 1) };
            Complex[] b = { new Complex(1, 5), new Complex(8, -1), new Complex(-4, 8) };
            Complex[] expected = { new Complex(-18, 40), new Complex(13, -26), new Complex(12, -44) };
            Complex[] actual = ComplexMatrix.Multiply(a, b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void PhaseTest()
        {
            Complex[] x = { new Complex(0, 5), new Complex(2, 0), new Complex(-5, 1) };
            double[] expected = { 1, Math.Sqrt(5), Math.Sqrt(26) };
            double[] actual = ComplexMatrix.Phase(x);

            for (int i = 0; i < x.Length; i++)
                Assert.AreEqual(x[i].Phase, Math.Atan2(x[i].Im, x[i].Re));
        }


        [TestMethod()]
        public void ReTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[] expected = { 1, 2, -5 };
            double[] actual = ComplexMatrix.Re(x);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void SumTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            Complex expected = new Complex(-2, 5);
            Complex actual = ComplexMatrix.Sum(x);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToArrayTest()
        {
            Complex[] c = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[,] expected = 
            {
                {  1, 5  },
                {  2, -1 },
                { -5, 1  },
            };

            double[,] actual = ComplexMatrix.ToArray(c);

            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
}
