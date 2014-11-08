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

namespace Accord.Tests.Statistics
{

    ﻿using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ShapiroWilkDistributionTest
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
        public void ConstructorTest()
        {
            // Example from http://www.nag.com/numeric/cl/nagdoc_cl23/pdf/G01/g01ddc.pdf

            double[] a = 
            { 
                0.11, 7.87, 4.61, 10.14, 7.95, 3.14, 0.46, 4.43, 
                0.21, 4.75, 0.71, 1.52, 3.24, 0.93, 0.42, 4.97,
                9.53, 4.55, 0.47, 6.66 
            };

            var sw = new ShapiroWilkDistribution(a.Length);

            double expected = 0.0421;
            double actual = sw.ComplementaryDistributionFunction(0.9005);
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            // Example from http://www.nag.com/numeric/cl/nagdoc_cl23/pdf/G01/g01ddc.pdf

            double[] b =
            {
                1.36, 1.14, 2.92, 2.55, 1.46, 1.06, 5.27, 1.11, 3.48,
                1.10, 0.88, 0.51, 1.46, 0.52, 6.20, 1.69, 0.08, 3.67,
                2.81, 3.49
            };

            var sw = new ShapiroWilkDistribution(b.Length);

            double expected = 0.5246;
            double actual = sw.ComplementaryDistributionFunction(0.9590);
            Assert.AreEqual(expected, actual, 1e-3);
        }

    }
}