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
    using Accord.Controls;
    using System.Globalization;

    [TestClass()]
    public class InverseGaussianTest
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
        public void ConstructorTest2()
        {
            var invGaussian = new InverseGaussianDistribution(mean: 0.42, shape: 1.2);

            double mean = invGaussian.Mean;     // 0.42
            double median = invGaussian.Median;   // 0.35856861093990083
            double var = invGaussian.Variance; // 0.061739999999999989

            double cdf = invGaussian.DistributionFunction(x: 0.27); // 0.30658791274125458
            double pdf = invGaussian.ProbabilityDensityFunction(x: 0.27); // 2.3461495925760354
            double lpdf = invGaussian.LogProbabilityDensityFunction(x: 0.27); // 0.85277551314980737

            double ccdf = invGaussian.ComplementaryDistributionFunction(x: 0.27); // 0.69341208725874548
            double icdf = invGaussian.InverseDistributionFunction(p: cdf); // 0.26999999957543408

            double hf = invGaussian.HazardFunction(x: 0.27); // 3.383485283406336
            double chf = invGaussian.CumulativeHazardFunction(x: 0.27); // 0.36613081401302111

            string str = invGaussian.ToString(CultureInfo.InvariantCulture); // "N^-1(x; μ = 0.42, λ = 1.2)"

            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.35856861093990083, median, 1e-7);
            Assert.AreEqual(0.061739999999999989, var);
            Assert.AreEqual(0.36613081401302111, chf);
            Assert.AreEqual(0.30658791274125458, cdf);
            Assert.AreEqual(2.3461495925760354, pdf);
            Assert.AreEqual(0.85277551314980737, lpdf);
            Assert.AreEqual(3.383485283406336, hf);
            Assert.AreEqual(0.69341208725874548, ccdf);
            Assert.AreEqual(0.26999999957543408, icdf, 1e-8);
            Assert.AreEqual("N^-1(x; μ = 0.42, λ = 1.2)", str);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            InverseGaussianDistribution g = new InverseGaussianDistribution(1.2, 4.2);
            Assert.AreEqual(1.2, g.Mean);
            Assert.AreEqual(4.2, g.Shape);

            Assert.AreEqual(0.41142857142857142, g.Variance);
            Assert.AreEqual(0.64142698058981851, g.StandardDeviation);
        }

        [TestMethod()]
        public void ProbabilityFunctionTest()
        {
            InverseGaussianDistribution g = new InverseGaussianDistribution(1.2, 4.2);

            double expected = 0.363257;
            double actual = g.ProbabilityDensityFunction(0.42);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void ProbabilityFunctionTest2()
        {
            InverseGaussianDistribution g = new InverseGaussianDistribution(4.1, 1.2);

            double[] expected =
            {
                0.0457398, 0.323655, 0.477189, 0.509189, 0.490063, 0.453721, 0.413867, 0.375711, 0.34101, 0.310123
            };

            for (int i = 0; i < expected.Length; i++)
            {
                double x = (i + 1) / 10.0;

                double actual = g.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected[i], actual, 1e-6);
                Assert.IsFalse(double.IsNaN(actual));
            }
            
        }

        [TestMethod()]
        public void CumulativeFunctionTest()
        {
            InverseGaussianDistribution g = new InverseGaussianDistribution(1.2, 4.2);

            double expected = 0.030679;
            double actual = g.DistributionFunction(0.42);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));
        }

        [TestMethod()]
        public void CumulativeFunctionTest2()
        {
            InverseGaussianDistribution g = new InverseGaussianDistribution(4.1, 1.2);

            double[] expected =
            {
                0.000710666, 0.0190607, 0.0604859, 0.110461, 0.160665, 0.207921, 0.2513, 0.290755, 0.326559, 0.359084
            };

            for (int i = 0; i < expected.Length; i++)
            {
                double x = (i + 1) / 10.0;
                double actual = g.DistributionFunction(x);

                Assert.AreEqual(expected[i], actual, 1e-6);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void MedianTest()
        {
            InverseGaussianDistribution target = new InverseGaussianDistribution(6, 2);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void GenerateTest()
        {
            InverseGaussianDistribution target = new InverseGaussianDistribution(1, 1);

            double[] samples = target.Generate(1000000);

            InverseGaussianDistribution actual = new InverseGaussianDistribution(5, 5);
            actual.Fit(samples);

            Assert.AreEqual(1, actual.Mean, 0.01);
            Assert.AreEqual(1, actual.Shape, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            InverseGaussianDistribution target = new InverseGaussianDistribution(5, 2);

            double[] samples = target.Generate(1000000);

            var actual = InverseGaussianDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(5, actual.Mean, 0.01);
            Assert.AreEqual(2, actual.Shape, 0.01);
        }

        [TestMethod()]
        public void GenerateTest3()
        {
            InverseGaussianDistribution target = new InverseGaussianDistribution(4, 2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = InverseGaussianDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Mean, 0.01);
            Assert.AreEqual(2, actual.Shape, 0.01);
        }

    }
}