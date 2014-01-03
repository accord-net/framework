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

    [TestClass()]
    public class SpecialTest
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
        public void BinomialTest()
        {
            int n = 63;
            int k = 6;

            double expected = 67945521;
            double actual;

            actual = Special.Binomial(n, k);
            Assert.AreEqual(expected, actual);

            n = 42;
            k = 12;
            expected = 11058116888;

            actual = Special.Binomial(n, k);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void Log1pTest()
        {
            double precision = 1e-16;
            Assert.IsTrue(double.IsNaN(Special.Log1p(double.NaN)));
            Assert.IsTrue(double.IsNaN(Special.Log1p(-32.0482175)));
            Assert.AreEqual(double.PositiveInfinity, Special.Log1p(double.PositiveInfinity));

            double b = System.Math.Log(1 - 7e-32);
            double c = Special.Log1p(7e-32);
            Assert.AreEqual(c, 7e-32);


            Assert.AreEqual(-0.2941782295312541, Special.Log1p(-0.254856327), precision);
            Assert.AreEqual(7.368050685564151, Special.Log1p(1583.542), precision);
            Assert.AreEqual(0.4633708685409921, Special.Log1p(0.5894227), precision);
            Assert.AreEqual(709.782712893384, Special.Log1p(double.MaxValue), precision);
            Assert.AreEqual(double.MinValue, Special.Log1p(double.MinValue), precision);
        }


        [TestMethod()]
        public void FactorialTest()
        {
            int n = 3;
            double expected = 6;
            double actual = Special.Factorial(n);
            Assert.AreEqual(expected, actual);

            n = 35;
            expected = 1.033314796638614e+40;
            actual = Special.Factorial(n);
            Assert.AreEqual(expected, actual, 1e27);
        }

        [TestMethod()]
        public void LnFactorialTest()
        {
            int n = 4;
            double expected = 3.178053830347946;
            double actual = Special.LogFactorial(n);
            Assert.AreEqual(expected, actual, 0.0000000001);

            n = 170;
            expected = 7.065730622457874e+02;
            actual = Special.LogFactorial(n);
            Assert.AreEqual(expected, actual, 0.0000000001);
        }

        [TestMethod()]
        public void EpsilonTest()
        {
            double x = 0.5;
            double expected = 0.00000000000000011102230246251565;
            double actual = Special.Epslon(x);
            Assert.AreEqual(expected, actual);

            x = 0.0;
            expected = 0.0;
            actual = Special.Epslon(x);
            Assert.AreEqual(expected, actual);

            x = 1.0;
            expected = 0.00000000000000022204460492503131;
            actual = Special.Epslon(x);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BinomialTest2()
        {
            int n = 6;
            int k = 4;
            double expected = 15;
            double actual = Special.Binomial(n, k);
            Assert.AreEqual(expected, actual);

            n = 100;
            k = 47;
            expected = 8.441348728306404e+28;
            actual = Special.Binomial(n, k);
            Assert.AreEqual(expected, actual, 1e+16);
        }


    }
}
