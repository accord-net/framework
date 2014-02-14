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
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;

    [TestClass()]
    public class GammaDistributionTest
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
        public void GammaDistributionConstructorTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            double[] expected = 
            {
                double.NegativeInfinity, 0.987114, 0.635929, 0.486871, 0.400046,
                0.341683, 0.299071, 0.266236, 0.239956, 0.218323, 0.200126
            };

            GammaDistribution target = new GammaDistribution(scale, shape);

            Assert.AreEqual(shape, target.Shape);
            Assert.AreEqual(scale, target.Scale);

            Assert.AreEqual(shape * scale, target.Mean);
            Assert.AreEqual(shape * scale * scale, target.Variance);
        }

        [TestMethod()]
        public void GammaDistributionConstructorTest2()
        {
            var gamma = new GammaDistribution(theta: 4, k: 2);

            double mean = gamma.Mean;     // 8.0
            double median = gamma.Median; // 6.7133878418421506
            double var = gamma.Variance;  // 32.0

            double cdf = gamma.DistributionFunction(x: 0.27); // 0.002178158242390601
            double pdf = gamma.ProbabilityDensityFunction(x: 0.27); // 0.015773530285395465
            double lpdf = gamma.LogProbabilityDensityFunction(x: 0.27); // -4.1494220422235433
            double ccdf = gamma.ComplementaryDistributionFunction(x: 0.27); // 0.99782184175760935
            double icdf = gamma.InverseDistributionFunction(p: cdf); // 0.26999998689819171

            double hf = gamma.HazardFunction(x: 0.27); // 0.015807962529274005
            double chf = gamma.CumulativeHazardFunction(x: 0.27); // 0.0021805338793574793

            string str = gamma.ToString(CultureInfo.InvariantCulture); // "Γ(x; k = 2, θ = 4)"

            Assert.AreEqual(8.0, mean);
            Assert.AreEqual(6.7133878418421506, median, 1e-6);
            Assert.AreEqual(32.0, var);
            Assert.AreEqual(0.0021805338793574793, chf);
            Assert.AreEqual(0.002178158242390601, cdf);
            Assert.AreEqual(0.015773530285395465, pdf);
            Assert.AreEqual(-4.1494220422235433, lpdf);
            Assert.AreEqual(0.015807962529274005, hf);
            Assert.AreEqual(0.99782184175760935, ccdf);
            Assert.AreEqual(0.26999998689819171, icdf, 1e-6);
            Assert.AreEqual("Γ(x; k = 2, θ = 4)", str);

            double p05 = gamma.DistributionFunction(median);
            Assert.AreEqual(0.5, p05, 1e-6);
        }

        [TestMethod()]
        public void MedianTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            GammaDistribution target = new GammaDistribution(scale, shape);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void DensityFunctionTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            double[] pdf = 
            {
                double.PositiveInfinity, 0.987114, 0.635929, 0.486871, 0.400046,
                0.341683, 0.299071, 0.266236, 0.239956, 0.218323, 0.200126
            };

            GammaDistribution target = new GammaDistribution(scale, shape);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double actual = target.ProbabilityDensityFunction(x);
                double expected = pdf[i];

                Assert.AreEqual(expected, actual, 1e-6);
                Assert.IsFalse(double.IsNaN(actual));

                double logActual = target.LogProbabilityDensityFunction(x);
                double logExpected = Math.Log(pdf[i]);

                Assert.AreEqual(logExpected, logActual, 1e-5);
                Assert.IsFalse(double.IsNaN(logActual));
            }
        }

        [TestMethod()]
        public void CumulativeFunctionTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            double[] cdf = 
            {
                0, 0.251017, 0.328997, 0.38435, 0.428371, 0.465289,
                0.497226, 0.525426, 0.55069, 0.573571, 0.594469
            };

            GammaDistribution target = new GammaDistribution(scale, shape);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double actual = target.DistributionFunction(x);
                double expected = cdf[i];

                Assert.AreEqual(expected, actual, 1e-5);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            GammaDistribution target = new GammaDistribution(5, 2);

            double[] samples = target.Generate(1000000);

            var actual = GammaDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(5, actual.Scale, 0.02);
            Assert.AreEqual(2, actual.Shape, 0.01);
        }

        [TestMethod()]
        public void GenerateTest3()
        {
            Accord.Math.Tools.SetupGenerator(1);

            GammaDistribution target = new GammaDistribution(4, 2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = GammaDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Scale, 0.01);
            Assert.AreEqual(2, actual.Shape, 0.01);
        }
    }
}