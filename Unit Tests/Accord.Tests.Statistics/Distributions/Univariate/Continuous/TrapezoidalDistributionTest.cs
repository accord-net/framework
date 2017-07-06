// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Ashley Messer, 2014
// glyphard at gmail.com
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

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System.Globalization;
    using Accord.Math;

    [TestFixture]
    public class TrapezoidalDistributionTest
    {
        [Test]
        public void TrapezoidalDistributionConstructorTest()
        {
            double x = 0.75d;

            double a = 0;
            double b = (1.0d / 3.0d);
            double c = (2.0d / 3.0d);
            double d = 1.0d;
            double n1 = 2.0d;
            double n3 = 2.0d;

            var trapDist = new TrapezoidalDistribution(a, b, c, d, n1, n3);
            double mean = trapDist.Mean; //0.62499999999999989
            double variance = trapDist.Variance; //0.37103174603174593
            double pdf = trapDist.ProbabilityDensityFunction(x); //1.1249999999999998
            double cdf = trapDist.DistributionFunction(x); //1.28125
            string tostr = trapDist.ToString("N2", CultureInfo.InvariantCulture);

            Assert.AreEqual(mean, 0.625d, 1e-6);
            Assert.AreEqual(variance, 0.37103175d, 1e-6);
            Assert.AreEqual(pdf, 1.125, 1e-6, "should match output from dtrapezoid in R");
            Assert.AreEqual(cdf, 0.859375, 1e-6);
            Assert.AreEqual(tostr, "Trapezoidal(x; a = 0.00, b = 0.33, c = 0.67, d = 1.00, n1 = 2.00, n3 = 2.00, α = 1.00)");
            //Verified using R package 'trapezoid'
        }

        [Test]
        public void DocumentationTest1()
        {
            #region doc_ctor
            // Create a new trapezoidal distribution with linear growth between
            // 0 and 2, stability between 2 and 8, and decrease between 8 and 10.
            //
            //
            //            +-----------+
            //           /|           |\
            //          / |           | \
            //         /  |           |  \
            //  -------+---+-----------+---+-------
            //   ...   0   2   4   6   8   10  ...
            //
            var trapz = new TrapezoidalDistribution(a: 0, b: 2, c: 8, d: 10, n1: 1, n3: 1);

            double mean = trapz.Mean;     // 2.25
            double median = trapz.Median; // 5.0
            double mode = trapz.Mode;     // 4.7706917621394611
            double var = trapz.Variance;  // 17.986666666666665

            double cdf = trapz.DistributionFunction(x: 1.4);       // 0.13999999999999999
            double pdf = trapz.ProbabilityDensityFunction(x: 1.4); // 0.10000000000000001
            double lpdf = trapz.LogProbabilityDensityFunction(x: 1.4); // -2.3025850929940455

            double ccdf = trapz.ComplementaryDistributionFunction(x: 1.4); // 0.85999999999999999
            double icdf = trapz.InverseDistributionFunction(p: cdf); // 1.3999999999999997

            double hf = trapz.HazardFunction(x: 1.4); // 0.11627906976744187
            double chf = trapz.CumulativeHazardFunction(x: 1.4); // 0.15082288973458366

            string str = trapz.ToString(CultureInfo.InvariantCulture); // Trapezoidal(x; a=0, b=2, c=8, d=10, n1=1, n3=1, α = 1)
            #endregion

            Assert.AreEqual(2.25, mean);
            Assert.AreEqual(5.0, median);
            Assert.AreEqual(4.7706917621394611, mode);
            Assert.AreEqual(17.986666666666665, var);
            Assert.AreEqual(0.15082288973458366, chf);
            Assert.AreEqual(0.13999999999999999, cdf);
            Assert.AreEqual(0.10000000000000001, pdf);
            Assert.AreEqual(-2.3025850929940455, lpdf);
            Assert.AreEqual(0.11627906976744187, hf);
            Assert.AreEqual(0.85999999999999999, ccdf);
            Assert.AreEqual(1.3999999999999997, icdf);
            Assert.AreEqual("Trapezoidal(x; a = 0, b = 2, c = 8, d = 10, n1 = 1, n3 = 1, α = 1)", str);

            var range1 = trapz.GetRange(0.95);
            var range2 = trapz.GetRange(0.99);
            var range3 = trapz.GetRange(0.01);

            Assert.AreEqual(0.50000000000000044, range1.Min);
            Assert.AreEqual(9.5, range1.Max);
            Assert.AreEqual(0.10000000000000009, range2.Min);
            Assert.AreEqual(9.9, range2.Max);
            Assert.AreEqual(0.10000000000000001, range3.Min);
            Assert.AreEqual(9.9, range3.Max);
        }

        [Test]
        public void rtest()
        {
            /*
             install.packages('trapezoid')
             library('trapezoid')

             ptrapezoid((0:20)/2, min = 0.6, mode1 = 1.2, mode2 = 4.7, max = 6.4)
             dtrapezoid((0:20)/2, min = 0.6, mode1 = 1.2, mode2 = 4.7, max = 6.4)
             */

            var dist = new TrapezoidalDistribution(0.6, 1.2, 4.7, 6.4);

            Assert.AreEqual(0.00000000, dist.DistributionFunction(0.0));
            Assert.AreEqual(0.00000000, dist.DistributionFunction(0.5));
            Assert.AreEqual(0.02867384, dist.DistributionFunction(1.0), 1e-6);
            Assert.AreEqual(0.12903226, dist.DistributionFunction(1.5), 1e-6);
            Assert.AreEqual(0.23655914, dist.DistributionFunction(2.0), 1e-6);
            Assert.AreEqual(0.34408602, dist.DistributionFunction(2.5), 1e-6);
            Assert.AreEqual(0.45161290, dist.DistributionFunction(3.0), 1e-6);
            Assert.AreEqual(0.55913978, dist.DistributionFunction(3.5), 1e-6);
            Assert.AreEqual(0.66666667, dist.DistributionFunction(4.0), 1e-6);
            Assert.AreEqual(0.77419355, dist.DistributionFunction(4.5), 1e-6);     
            Assert.AreEqual(0.87602783, dist.DistributionFunction(5.0), 1e-6);
            Assert.AreEqual(0.94876660, dist.DistributionFunction(5.5), 1e-6);
            Assert.AreEqual(0.98987982, dist.DistributionFunction(6.0), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(6.5), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(7.0), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(7.5), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(8.0), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(8.5), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(9.0), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(9.5), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(10.0), 1e-6);
            Assert.AreEqual(1.00000000, dist.DistributionFunction(10.5));

            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(0.0));
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(0.5));
            Assert.AreEqual(0.14336918, dist.ProbabilityDensityFunction(1.0), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(1.5), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(2.0), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(2.5), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(3.0), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(3.5), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(4.0), 1e-6);
            Assert.AreEqual(0.21505376, dist.ProbabilityDensityFunction(4.5), 1e-6);
            Assert.AreEqual(0.17710310, dist.ProbabilityDensityFunction(5.0), 1e-6);
            Assert.AreEqual(0.11385199, dist.ProbabilityDensityFunction(5.5), 1e-6);
            Assert.AreEqual(0.05060089, dist.ProbabilityDensityFunction(6.0), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(6.5), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(7.0), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(7.5), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(8.0), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(8.5), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(9.0), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(9.5), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(10.0), 1e-6);
            Assert.AreEqual(0.00000000, dist.ProbabilityDensityFunction(10.5));
        }

        [Test]
        public void icdf()
        {
            var dist = new TrapezoidalDistribution(0, 1, 2, 3, 1, 1);

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            for (int i = 0; i < percentiles.Length; i++)
            {
                double x = percentiles[i];
                double icdf = dist.InverseDistributionFunction(x);
                double cdf = dist.DistributionFunction(icdf);
                Assert.AreEqual(x, cdf, 1e-5);
            }

        }

    }
}
