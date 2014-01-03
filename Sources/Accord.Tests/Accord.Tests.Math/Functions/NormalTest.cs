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

    [TestClass()]
    public class NormalTest
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
        public void FunctionTest()
        {
            // p = 0.5 * erfc(-z ./ sqrt(2))
            double value = 0.42;
            double expected = 0.662757273151751;
            double actual = Normal.Function(value);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void InverseTest()
        {
            double value = 0.662757273151751;
            double expected = 0.42;
            double actual = Normal.Inverse(value);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void BatchTest()
        {
            double x = 0.42;
            double phi = Normal.Function(x);
            double phic = Normal.Complemented(x);
            double inv = Normal.Inverse(x);

            double hphic = Normal.Complemented(x);
            double hphi = Normal.HighAccuracyFunction(x);


            Assert.AreEqual(0.66275727315175048, phi);
            Assert.AreEqual(0.33724272684824952, phic);
            Assert.AreEqual(-0.20189347914185085, inv);

            Assert.AreEqual(0.66275727315175048, hphi);
            Assert.AreEqual(0.33724272684824946, hphic, 1e-10);
        }

        [TestMethod()]
        public void BatchTest2()
        {
            double x = 16.6;
            double phi = Normal.Function(x);
            double phic = Normal.Complemented(x);
            double hphic = Normal.HighAccuracyComplemented(x);
            double hphi = Normal.HighAccuracyFunction(x);

            Assert.AreEqual(1.0, phi);
            Assert.AreEqual(3.4845465199504055E-62, phic);
            Assert.AreEqual(0.99999999999999556, hphi);
            Assert.AreEqual(3.48454651995264E-62, hphic);
        }


        [TestMethod()]
        public void BivariateFunctionTest()
        {
            {
                double a = Normal.Bivariate(+0, +0, 0.0);
                double b = Normal.Bivariate(-1, +0, 0.0);
                double c = Normal.Bivariate(+0, -1, 0.0);
                double d = Normal.Bivariate(-1, -1, 0.0);

                Assert.AreEqual(a, 0.25);
                Assert.AreEqual(b, 0.07932762696572859, 1e-10);
                Assert.AreEqual(c, 0.07932762696572859, 1e-10);
                Assert.AreEqual(d, 0.02517148960005514, 1e-10);
            }

            {
                double a = Normal.Bivariate(0, 0, 0.0);
                double b = Normal.Bivariate(1, 0, 0.0);
                double c = Normal.Bivariate(0, 1, 0.0);
                double d = Normal.Bivariate(1, 1, 0.0);

                Assert.AreEqual(a, 0.25);
                Assert.AreEqual(b, 0.4206723730342714, 1e-10);
                Assert.AreEqual(c, 0.4206723730342714, 1e-10);
                Assert.AreEqual(d, 0.707860981737141, 1e-10);
            }

            {
                double a = Normal.Bivariate(+0, +0, 0.99);
                double b = Normal.Bivariate(-1, +0, 0.99);
                double c = Normal.Bivariate(+0, -1, 0.99);
                double d = Normal.Bivariate(-1, -1, 0.99);

                Assert.AreEqual(a, 0.477473293177794, 1e-6);
                Assert.AreEqual(b, 0.1586552539314527, 1e-10);
                Assert.AreEqual(c, 0.1586552539314527, 1e-10);
                Assert.AreEqual(d, 0.1450035348479944, 1e-8);
            }

            {
                double a = Normal.Bivariate(0, 0, 0.99);
                double b = Normal.Bivariate(1, 0, 0.99);
                double c = Normal.Bivariate(0, 1, 0.99);
                double d = Normal.Bivariate(1, 1, 0.99);

                Assert.AreEqual(a, 0.477473293177794, 1e-6);
                Assert.AreEqual(b, 0.4999999999999956, 1e-10);
                Assert.AreEqual(c, 0.4999999999999956, 1e-10);
                Assert.AreEqual(d, 0.8276930269850803, 1e-8);
            }

            {
                double a = Normal.Bivariate(0, 0, 0.42);
                double b = Normal.Bivariate(0, 1, 0.42);
                double c = Normal.Bivariate(1, 0, 0.42);
                double d = Normal.Bivariate(1, 1, 0.42);

                double e = Normal.Bivariate(-0, -1, 0.42);
                double f = Normal.Bivariate(-1, -0, 0.42);
                double g = Normal.Bivariate(-1, -1, 0.42);


                Assert.AreEqual(a, 0.3189849652491711, 1e-10);
                Assert.AreEqual(b, 0.4611405565191831, 1e-10);
                Assert.AreEqual(c, 0.4611405565191831, 1e-10);
                Assert.AreEqual(d, 0.7379699765574815, 1e-10);
                Assert.AreEqual(e, 0.1197958104506402, 1e-10);
                Assert.AreEqual(f, 0.1197958104506402, 1e-10);
                Assert.AreEqual(g, 0.05528048442039563, 1e-10);
            }

        }

    }
}
