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
    public class SkewNormalDistributionTest
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
        public void ConstructorTest1()
        {
            // Create a Skew normal distribution with location 2, scale 3 and shape 4.2
            var skewNormal = new SkewNormalDistribution(location: 2, scale: 3, shape: 4.2);

            double mean = skewNormal.Mean;     // 4.3285611780515953
            double median = skewNormal.Median; // 4.0230040653062265
            double var = skewNormal.Variance;  // 3.5778028400709641
            double mode = skewNormal.Mode;     // 3.220622226764422

            double cdf = skewNormal.DistributionFunction(x: 1.4); // 0.020166854942526125
            double pdf = skewNormal.ProbabilityDensityFunction(x: 1.4); // 0.052257431834162059
            double lpdf = skewNormal.LogProbabilityDensityFunction(x: 1.4); // -2.9515731621912877

            double ccdf = skewNormal.ComplementaryDistributionFunction(x: 1.4); // 0.97983314505747388
            double icdf = skewNormal.InverseDistributionFunction(p: cdf); // 1.3999998597203041

            double hf = skewNormal.HazardFunction(x: 1.4); // 0.053332990517581239
            double chf = skewNormal.CumulativeHazardFunction(x: 1.4); // 0.020372981958858238

            string str = skewNormal.ToString(CultureInfo.InvariantCulture); // Sn(x; ξ = 2, ω = 3, α = 4.2)

            Assert.AreEqual(4.3285611780515953, mean);
            Assert.AreEqual(4.0230040653062265, median);
            Assert.AreEqual(3.2206222267273086, mode);
            Assert.AreEqual(3.5778028400709641, var);
            Assert.AreEqual(0.020372981958858238, chf);
            Assert.AreEqual(0.020166854942526125, cdf);
            Assert.AreEqual(0.052257431834161927, pdf);
            Assert.AreEqual(-2.9515731621912908, lpdf);
            Assert.AreEqual(0.053332990517581107, hf);
            Assert.AreEqual(0.97983314505747388, ccdf);
            Assert.AreEqual(1.3999998597203041, icdf);
            Assert.AreEqual("Sn(x; ξ = 2, ω = 3, α = 4.2)", str);

            var range1 = skewNormal.GetRange(0.95);
            var range2 = skewNormal.GetRange(0.99);
            var range3 = skewNormal.GetRange(0.01);

            Assert.AreEqual(1.7924702650334006, range1.Min);
            Assert.AreEqual(7.879891951905198, range1.Max);
            Assert.AreEqual(1.1477106790399794, range2.Min);
            Assert.AreEqual(9.7274879203457694, range2.Max);
            Assert.AreEqual(1.1477106790399785, range3.Min);
            Assert.AreEqual(9.7274879203457694, range3.Max);
        }

        [Test]
        public void ConstructorTest2()
        {
            var normal = SkewNormalDistribution.Normal(mean: 4, stdDev: 4.2);

            double mean = normal.Mean;     // 4.0
            double median = normal.Median; // 4.0
            double var = normal.Variance;  // 17.64
            double mode = normal.Mode;     // 4.0

            double cdf = normal.DistributionFunction(x: 1.4); // 0.26794249453351904
            double pdf = normal.ProbabilityDensityFunction(x: 1.4); // 0.078423391448155175
            double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -2.5456330358182586

            double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.732057505466481
            double icdf = normal.InverseDistributionFunction(p: cdf); // 1.4

            double hf = normal.HazardFunction(x: 1.4); // 0.10712736480747137
            double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.31189620872601354

            string str = normal.ToString(CultureInfo.InvariantCulture); // Sn(x; ξ = 4, ω = 4.2, α = 0)

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(4.0, median, 1e-5);
            Assert.AreEqual(17.64, var);
            Assert.AreEqual(4, mode, 1e-6);
            Assert.AreEqual(0.31189620872601354, chf);
            Assert.AreEqual(0.26794249453351904, cdf);
            Assert.AreEqual(0.078423391448155175, pdf, 1e-8);
            Assert.AreEqual(-2.5456330358182586, lpdf, 1e-8);
            Assert.AreEqual(0.10712736480747137, hf, 1e-8);
            Assert.AreEqual(0.732057505466481, ccdf, 1e-8);
            Assert.AreEqual(1.4, icdf, 1e-8);
            Assert.AreEqual("Sn(x; ξ = 4, ω = 4.2, α = 0)", str);
        }

    }
}
