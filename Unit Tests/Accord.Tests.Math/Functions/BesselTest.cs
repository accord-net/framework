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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using NUnit.Framework;

    [TestFixture]
    public class BesselTest
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



        [Test]
        public void BesselY0Test()
        {
            double actual;

            actual = Bessel.Y0(64);
            Assert.AreEqual(0.037067103232088, actual, 0.000001);
        }

        [Test]
        public void BesselJTest()
        {
            double actual;

            actual = Bessel.J(0, 1);
            Assert.AreEqual(0.765197686557967, actual, 0.000001);

            actual = Bessel.J(0, 5);
            Assert.AreEqual(-0.177596771314338, actual, 0.000001);

            actual = Bessel.J(2, 17.3);
            Assert.AreEqual(0.117351128521774, actual, 0.000001);
        }

        [Test]
        public void BesselYTest()
        {
            double actual;

            actual = Bessel.Y(2, 4);
            Assert.AreEqual(0.215903594603615, actual, 0.000001);

            actual = Bessel.Y(0, 64);
            Assert.AreEqual(0.037067103232088, actual, 0.000001);
        }

        [Test]
        public void BesselJ0Test()
        {
            double actual;

            actual = Bessel.J0(1);
            Assert.AreEqual(0.765197686557967, actual, 0.000001);

            actual = Bessel.J0(5);
            Assert.AreEqual(-0.177596771314338, actual, 0.000001);
        }

    }
}
