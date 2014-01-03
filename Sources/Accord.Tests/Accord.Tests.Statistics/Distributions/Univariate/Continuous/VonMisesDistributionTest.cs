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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;

    [TestClass()]
    public class VonMisesDistributionTest
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
            var vonMises = new VonMisesDistribution(mean: 0.42, concentration: 1.2);

            double mean = vonMises.Mean;     // 0.42
            double median = vonMises.Median; // 0.42
            double var = vonMises.Variance;  // 0.48721760532782921

            double cdf = vonMises.DistributionFunction(x: 1.4); // 0.81326928491589345
            double pdf = vonMises.ProbabilityDensityFunction(x: 1.4); // 0.2228112141141676
            double lpdf = vonMises.LogProbabilityDensityFunction(x: 1.4); // -1.5014304395467863

            double ccdf = vonMises.ComplementaryDistributionFunction(x: 1.4); // 0.18673071508410655
            double icdf = vonMises.InverseDistributionFunction(p: cdf); // 1.3999999637927665

            double hf = vonMises.HazardFunction(x: 1.4); // 1.1932220899695576
            double chf = vonMises.CumulativeHazardFunction(x: 1.4); // 1.6780877262500649

            string str = vonMises.ToString(CultureInfo.InvariantCulture); // VonMises(x; μ = 0.42, κ = 1.2)

            double imedian = vonMises.InverseDistributionFunction(p: 0.5);

            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.42, median);
            Assert.AreEqual(0.42000000260613551, imedian, 1e-8);
            Assert.AreEqual(0.48721760532782921, var);
            Assert.AreEqual(1.6780877262500649, chf);
            Assert.AreEqual(0.81326928491589345, cdf);
            Assert.AreEqual(0.2228112141141676, pdf);
            Assert.AreEqual(-1.5014304395467863, lpdf);
            Assert.AreEqual(1.1932220899695576, hf);
            Assert.AreEqual(0.18673071508410655, ccdf);
            Assert.AreEqual(1.39999999999, icdf, 1e-8);
            Assert.AreEqual("VonMises(x; μ = 0.42, κ = 1.2)", str);
        }


        [TestMethod()]
        public void FitTest()
        {
            double[] angles = 
            {
               2.537498, 0.780449, 3.246623, 1.835845, 1.525273,
               2.821987, 1.783134, 1.165753, 3.298262, 2.941366,
               2.485515, 2.090029, 2.460631, 2.804243, 1.626327,
            };


            var distribution = VonMisesDistribution.Estimate(angles);

            Assert.AreEqual(2.411822, distribution.Concentration, 1e-6);
            Assert.AreEqual(2.249981, distribution.Mean, 1e-6);

            Assert.AreEqual(0.2441525, distribution.Variance, 1e-3);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            VonMisesDistribution dist = new VonMisesDistribution(2.249981, 2.411822);

            double actual = dist.ProbabilityDensityFunction(2.14);
            double expected = 0.5686769438969197;

            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            VonMisesDistribution dist = new VonMisesDistribution(2.249981, 2.411822);
            double x = 2.14;

            double actual = dist.LogProbabilityDensityFunction(x);
            double expected = System.Math.Log(dist.ProbabilityDensityFunction(x));

            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void MedianTest()
        {
            VonMisesDistribution target = new VonMisesDistribution(1.621, 4.52);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5), 1e-6);
        }
    }
}
