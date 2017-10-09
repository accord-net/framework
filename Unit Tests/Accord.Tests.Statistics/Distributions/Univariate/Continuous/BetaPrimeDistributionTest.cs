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

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System.Globalization;

    [TestFixture]
    public class BetaPrimeDistributionTest
    {
        [Test]
        public void Confirm_BetPrimeDistribution_Relative_to_F_Distribution()
        {
            double alpha = 4.0d;
            double beta = 6.0d;

            FDistribution fdist = new FDistribution((int)alpha * 2, (int)beta * 2);
            double fMean = fdist.Mean;
            double fPdf = (beta / alpha) * fdist.ProbabilityDensityFunction(4.0d);
            double fCdf = fdist.DistributionFunction(4.0d);

            var betaPrimeDist = new BetaPrimeDistribution(alpha, beta);
            double bpMean = (beta / alpha) * betaPrimeDist.Mean;
            double bpPdf = betaPrimeDist.ProbabilityDensityFunction((alpha / beta) * 4.0d);
            double bpCdf = betaPrimeDist.DistributionFunction((alpha / beta) * 4.0d);

            Assert.AreEqual(fMean, bpMean, 0.00000001, "mean should be equal");
            Assert.AreEqual(fPdf, bpPdf, 0.00000001, "probability density should be equal");
            Assert.AreEqual(fCdf, bpCdf, 0.00000001, "cumulative distribution should be equal");

            //Beta Prime distribution is a scaled version of Pearson Type VI, which itself is scale of F distribution
        }


        [Test]
        public void Constructor_ExtensiveTestForDocumentation()
        {
            // Create a new Beta-Prime distribution with shape (4,2)
            var betaPrime = new BetaPrimeDistribution(alpha: 4, beta: 2);

            double mean = betaPrime.Mean;     // 4.0
            double median = betaPrime.Median; // 2.1866398762435981
            double mode = betaPrime.Mode;     // 1.0
            double var = betaPrime.Variance;  // +inf

            double cdf = betaPrime.DistributionFunction(x: 0.4);           // 0.02570357589099781
            double pdf = betaPrime.ProbabilityDensityFunction(x: 0.4);     // 0.16999719504628183
            double lpdf = betaPrime.LogProbabilityDensityFunction(x: 0.4); // -1.7719733417957513

            double ccdf = betaPrime.ComplementaryDistributionFunction(x: 0.4); // 0.97429642410900219
            double icdf = betaPrime.InverseDistributionFunction(p: cdf);       // 0.39999982363709291

            double hf = betaPrime.HazardFunction(x: 0.4);            // 0.17448200654307533
            double chf = betaPrime.CumulativeHazardFunction(x: 0.4); // 0.026039684773113869

            string str = betaPrime.ToString(CultureInfo.InvariantCulture); // BetaPrime(x; α = 4, β = 2)

            Assert.AreEqual(4, betaPrime.Alpha);
            Assert.AreEqual(2, betaPrime.Beta);

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(2.1866398674965075, median, 1e-6);
            Assert.AreEqual(1.0, mode);
            Assert.AreEqual(double.PositiveInfinity, var);
            Assert.AreEqual(0.026039684773113869, chf);
            Assert.AreEqual(0.02570357589099781, cdf);
            Assert.AreEqual(0.16999719504628183, pdf);
            Assert.AreEqual(-1.7719733417957513, lpdf);
            Assert.AreEqual(0.17448200654307533, hf);
            Assert.AreEqual(0.97429642410900219, ccdf);
            Assert.AreEqual(0.4, icdf, 1e-6);
            Assert.AreEqual("BetaPrime(x; α = 4, β = 2)", str);

            var range1 = betaPrime.GetRange(0.95);
            var range2 = betaPrime.GetRange(0.99);
            var range3 = betaPrime.GetRange(0.01);

            Assert.AreEqual(0.52112458565894904, range1.Min);
            Assert.AreEqual(12.082088955968088, range1.Max);
            Assert.AreEqual(0.28546647531299646, range2.Min);
            Assert.AreEqual(29.597777581273448, range2.Max);
            Assert.AreEqual(range2.Min, range3.Min, 1e-15);
            Assert.AreEqual(range2.Max, range3.Max, 1e-15);
        }

    }
}
