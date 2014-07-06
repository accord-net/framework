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
    public class GumbelDistributionTest
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
            var gumbel = new GumbelDistribution(location: 4.795, scale: 1 / 0.392);

            double mean = gumbel.Mean;     // 6.2674889410753387
            double median = gumbel.Median; // 5.7299819402593481
            double mode = gumbel.Mode;     // 4.7949999999999999
            double var = gumbel.Variance;  // 10.704745853604138

            double cdf = gumbel.DistributionFunction(x: 3.4); // 0.17767760424788051
            double pdf = gumbel.ProbabilityDensityFunction(x: 3.4); // 0.12033954114322486
            double lpdf = gumbel.LogProbabilityDensityFunction(x: 3.4); // -2.1174380222001519

            double ccdf = gumbel.ComplementaryDistributionFunction(x: 3.4); // 0.82232239575211952
            double icdf = gumbel.InverseDistributionFunction(p: cdf); // 3.3999999904866245

            double hf = gumbel.HazardFunction(x: 1.4); // 0.03449691276402958
            double chf = gumbel.CumulativeHazardFunction(x: 1.4); // 0.022988793482259906

            string str = gumbel.ToString(CultureInfo.InvariantCulture); // Gumbel(x; μ = 4.795, β = 2.55)

            Assert.AreEqual(6.2674889410753387, mean);
            Assert.AreEqual(5.7299819402593481, median);
            Assert.AreEqual(10.704745853604138, var);
            Assert.AreEqual(0.022988793482259906, chf);
            Assert.AreEqual(0.17767760424788051, cdf);
            Assert.AreEqual(0.12033954114322486, pdf);
            Assert.AreEqual(-2.1174380222001519, lpdf);
            Assert.AreEqual(0.03449691276402958, hf);
            Assert.AreEqual(0.82232239575211952, ccdf);
            Assert.AreEqual(3.3999999904866245, icdf);
            Assert.AreEqual("Gumbel(x; μ = 4.795, β = 2.55102040816327)", str);
        }

    }
}
