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
    using System;
    using System.Globalization;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;

    [TestFixture]
    public class KumaraswamyDistributionTest
    {
        [Test]
        public void Constructor_KumaraswamyDistribution_PDF_given_0d2_AND_1d2_Parameters_Moments_matches_R_output()
        {
            var kumaraswamyDistribution = new KumaraswamyDistribution(0.2d, 1.2d);
            double mean = kumaraswamyDistribution.Mean; //0.1258821823337952
            double variance = kumaraswamyDistribution.Variance; //0.045500725605275683
            double median = kumaraswamyDistribution.Median; //0.016262209853672775
            double mode = kumaraswamyDistribution.Mode; //NaN  

            double pdf = kumaraswamyDistribution.ProbabilityDensityFunction(0.3); //0.46195081771596241
            double cdf = kumaraswamyDistribution.CumulativeHazardFunction(0.3); //1.8501524192880519
            string tostr = kumaraswamyDistribution.ToString(CultureInfo.InvariantCulture); // Kumaraswamy(x; a = 0.2, b = 1.2)

            Assert.AreEqual(tostr, "Kumaraswamy(x; a = 0.2, b = 1.2)");
            Assert.AreEqual(mean, 0.1258821823337952);
            Assert.AreEqual(variance, 0.045500725605275683);
            Assert.AreEqual(median, 0.016262209853672775);
            Assert.AreEqual(mode, double.NaN);
            Assert.AreEqual(pdf, 0.46195081771596241);
            Assert.AreEqual(cdf, 1.8501524192880519);
            
            // values verified in R package = ActuDistns
        }

        [Test]
        public void Constructor_ExtensiveTestForDocumentation()
        {
            // Create a new Kumaraswamy distribution with shape (4,2)
            var kumaraswamy = new KumaraswamyDistribution(a: 4, b: 2);

            double mean = kumaraswamy.Mean;     // 0.71111111111111114
            double median = kumaraswamy.Median; // 0.73566031573423674
            double mode = kumaraswamy.Mode;     // 0.80910671157022118
            double var = kumaraswamy.Variance;  // 0.027654320987654302

            double cdf = kumaraswamy.DistributionFunction(x: 0.4);           // 0.050544639999999919
            double pdf = kumaraswamy.ProbabilityDensityFunction(x: 0.4);     // 0.49889280000000014
            double lpdf = kumaraswamy.LogProbabilityDensityFunction(x: 0.4); // -0.69536403596913343

            double ccdf = kumaraswamy.ComplementaryDistributionFunction(x: 0.4); // 0.94945536000000008
            double icdf = kumaraswamy.InverseDistributionFunction(p: cdf);       // 0.40000011480618253

            double hf = kumaraswamy.HazardFunction(x: 0.4);            // 0.52545155993431869
            double chf = kumaraswamy.CumulativeHazardFunction(x: 0.4); // 0.051866764053008864

            string str = kumaraswamy.ToString(CultureInfo.InvariantCulture); // Kumaraswamy(x; a = 4, b = 2)

            Assert.AreEqual(0.71111111111111114, mean);
            Assert.AreEqual(0.73566031573423674, median);
            Assert.AreEqual(0.80910671157022118, mode);
            Assert.AreEqual(0.027654320987654302, var);
            Assert.AreEqual(0.051866764053008864, chf);
            Assert.AreEqual(0.050544639999999919, cdf);
            Assert.AreEqual(0.49889280000000014, pdf);
            Assert.AreEqual(-0.69536403596913343, lpdf);
            Assert.AreEqual(0.52545155993431869, hf);
            Assert.AreEqual(0.94945536000000008, ccdf);
            Assert.AreEqual(0.40000011480618253, icdf);
            Assert.AreEqual("Kumaraswamy(x; a = 4, b = 2)", str);

            var range1 = kumaraswamy.GetRange(0.95);
            var range2 = kumaraswamy.GetRange(0.99);
            var range3 = kumaraswamy.GetRange(0.01);

            Assert.AreEqual(0.39890408640473385, range1.Min);
            Assert.AreEqual(0.93868623055832956, range1.Max);
            Assert.AreEqual(0.26608174016394209, range2.Min);
            Assert.AreEqual(0.97400390609144138, range2.Max);
            Assert.AreEqual(0.26608174016394187, range3.Min);
            Assert.AreEqual(0.97400390609144138, range3.Max);
        }
    }
}
