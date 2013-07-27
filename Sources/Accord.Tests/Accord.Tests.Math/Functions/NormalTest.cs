// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion



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

    }
}
