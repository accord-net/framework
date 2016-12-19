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
    using System;
    using Accord.Math.Comparers;

    [TestFixture]
    public class GeneralComparerTest
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
        public void GeneralComparerConstructorTest()
        {
            double[] actual, expected;

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.NegativeInfinity, -1, 0, 2, Double.PositiveInfinity };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Ascending, useAbsoluteValues: false));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, 2, 0, -1, Double.NegativeInfinity };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, false));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, Double.NegativeInfinity, 2, -1, 0 };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, true));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, Double.NegativeInfinity, 2, -1, 0 };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, Math.Abs));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));
        }
    }
}
