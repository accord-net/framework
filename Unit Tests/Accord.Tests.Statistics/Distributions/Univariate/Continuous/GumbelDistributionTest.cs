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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestFixture]
    public class GumbelDistributionTest
    {


        [Test]
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
            Assert.AreEqual(4.7949999999999999, mode);
            Assert.AreEqual(10.704745853604138, var);
            Assert.AreEqual(0.022988793482259906, chf);
            Assert.AreEqual(0.17767760424788051, cdf);
            Assert.AreEqual(0.12033954114322486, pdf);
            Assert.AreEqual(-2.1174380222001519, lpdf);
            Assert.AreEqual(0.034496912764029573, hf);
            Assert.AreEqual(0.82232239575211952, ccdf);
            Assert.AreEqual(3.3999999999999999, icdf);
            Assert.AreEqual("Gumbel(x; μ = 4.795, β = 2.55102040816327)", str);

            var range1 = gumbel.GetRange(0.95);
            var range2 = gumbel.GetRange(0.99);
            var range3 = gumbel.GetRange(0.01);

            Assert.AreEqual(1.9960492163695556, range1.Min, 1e-6);
            Assert.AreEqual(12.37202869894511, range1.Max, 1e-6);
            Assert.AreEqual(0.89913343799046785, range2.Min, 1e-6);
            Assert.AreEqual(16.530074551002095, range2.Max, 1e-6);
            Assert.AreEqual(0.8991334379904673, range3.Min, 1e-6);
            Assert.AreEqual(16.530074551002095, range3.Max, 1e-6);


            /* Tested against R's package QRM:

                > pGumbel(3.4,  4.795, 1 / 0.392)
                [1] 0.1776776
             
                > dGumbel(3.4,  4.795, 1 / 0.392)
                [1] 0.1203395
             
             */

            Assert.AreEqual(double.NegativeInfinity, gumbel.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, gumbel.Support.Max);

            Assert.AreEqual(gumbel.InverseDistributionFunction(0), gumbel.Support.Min);
            Assert.AreEqual(gumbel.InverseDistributionFunction(1), gumbel.Support.Max);
        }

    }
}
