﻿// Accord Unit Tests
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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestClass()]
    public class PowerLognormalDistributionTest
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
        public void ConstructorTest1()
        {
            var plog = new PowerLognormalDistribution(power: 4.2, shape: 1.2);

            double cdf = plog.DistributionFunction(x: 1.4); // 0.98092157745191766
            double pdf = plog.ProbabilityDensityFunction(x: 1.4); // 0.046958580233533977
            double lpdf = plog.LogProbabilityDensityFunction(x: 1.4); // -3.0584893374471496

            double ccdf = plog.ComplementaryDistributionFunction(x: 1.4); // 0.019078422548082351
            double icdf = plog.InverseDistributionFunction(p: cdf); // 1.4

            double hf = plog.HazardFunction(x: 1.4); // 10.337649063164642
            double chf = plog.CumulativeHazardFunction(x: 1.4); // 3.9591972920568446

            string str = plog.ToString(CultureInfo.InvariantCulture); // PLD(x; p = 4.2, σ = 1.2)

            Assert.AreEqual(3.9591972920568446, chf);
            Assert.AreEqual(0.98092157745191766, cdf);
            Assert.AreEqual(0.046958580233533977, pdf);
            Assert.AreEqual(-3.0584893374471496, lpdf);
            Assert.AreEqual(10.337649063164642, hf);
            Assert.AreEqual(0.019078422548082351, ccdf);
            Assert.AreEqual(1.4000000000000001, icdf);
            Assert.AreEqual("PLD(x; p = 4.2, σ = 1.2)", str);
        }

    }
}
