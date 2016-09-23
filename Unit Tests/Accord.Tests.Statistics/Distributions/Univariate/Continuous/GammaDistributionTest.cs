// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using NUnit.Framework;
    using System;
    using System.Globalization;

    [TestFixture]
    public class GammaDistributionTest
    {


        [Test]
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

        [Test]
        public void GammaDistributionConstructorTest2()
        {
            var gamma = new GammaDistribution(theta: 4, k: 2);

            double mean = gamma.Mean;     // 8.0
            double median = gamma.Median; // 6.7133878418421506
            double var = gamma.Variance;  // 32.0
            double mode = gamma.Mode;     // 4.0

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
            Assert.AreEqual(4.0, mode);
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

            var range1 = gamma.GetRange(0.95);
            var range2 = gamma.GetRange(0.99);
            var range3 = gamma.GetRange(0.01);

            Assert.AreEqual(1.4214460427946485, range1.Min);
            Assert.AreEqual(18.975458073562308, range1.Max);
            Assert.AreEqual(0.59421896101306348, range2.Min);
            Assert.AreEqual(26.553408271975243, range2.Max);
            Assert.AreEqual(0.59421896101306348, range3.Min);
            Assert.AreEqual(26.553408271975243, range3.Max);
        }

        [Test]
        public void MedianTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            GammaDistribution target = new GammaDistribution(scale, shape);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
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

        [Test]
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

        [Test]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            GammaDistribution target = new GammaDistribution(5, 2);

            double[] samples = target.Generate(10000000);

            var actual = GammaDistribution.Estimate(samples);

            Assert.AreEqual(5, actual.Scale, 1e-3);
            Assert.AreEqual(2, actual.Shape, 1e-3);
        }

        [Test]
        public void GenerateTest3()
        {
            Accord.Math.Tools.SetupGenerator(1);

            GammaDistribution target = new GammaDistribution(4, 2);

            double[] samples = new double[10000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = GammaDistribution.Estimate(samples);

            Assert.AreEqual(4, actual.Scale, 5e-3);
            Assert.AreEqual(2, actual.Shape, 5e-3);
        }

        [Test]
        public void GenerateTest4()
        {
            Accord.Math.Tools.SetupGenerator(1);

            GammaDistribution target = new GammaDistribution(0.4, 0.2);

            double[] samples = new double[10000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = GammaDistribution.Estimate(samples);

            Assert.AreEqual(0.4, actual.Scale, 1e-3);
            Assert.AreEqual(0.2, actual.Shape, 1e-3);
        }

        [Test]
        public void GenerateTest5()
        {
            Accord.Math.Tools.SetupGenerator(1);

            var target = new GammaDistribution(42, 0.1337);
            var samples = target.Generate(10000000);

            var actual = GammaDistribution.Estimate(samples);
            Assert.AreEqual(42, actual.Scale, 5e-2);
            Assert.AreEqual(0.1337, actual.Shape, 5e-4);
        }

        [Test]
        public void GenerateTest6()
        {
            // https://github.com/accord-net/framework/issues/281
            Accord.Math.Random.Generator.Seed = 1;

            var target = new GammaDistribution(1.5, 0.9);
            var samples = target.Generate(10000000);

            var actual = GammaDistribution.Estimate(samples);
            Assert.AreEqual(1.5, actual.Scale, 5e-2);
            Assert.AreEqual(0.9, actual.Shape, 5e-4);
        }


        [Test]
        public void NegativeValueTest()
        {
            double[] samples = NormalDistribution.Standard.Generate(100);

            try
            {
                GammaDistribution.Estimate(samples);
                Assert.Fail();
            }
            catch (ArgumentException)
            {

            }
        }


        [Test]
        public void FitTest()
        {
            // Gamma Distribution Fit stalls for some arrays #159
            // https://github.com/accord-net/framework/issues/159

            double[] x = { 1275.56, 1239.44, 1237.92, 1237.22, 1237.1, 1238.41, 1238.62, 1237.05, 1237.19, 1236.51, 1264.6, 1238.19, 1237.39, 1235.79, 1236.53, 1236.8, 1238.06, 1236.5, 1235.32, 1236.44, 1236.58, 1236.3, 1237.91, 1238.6, 1238.49, 1239.21, 1238.57, 1244.63, 1236.06, 1236.4, 1237.88, 1237.56, 1236.66, 1236.59, 1236.53, 1236.32, 1238.29, 1237.79, 1237.86, 1236.42, 1236.23, 1236.37, 1237.18, 1237.63, 1245.8, 1238.04, 1238.55, 1238.39, 1236.75, 1237.07, 1250.78, 1238.6, 1238.36, 1236.58, 1236.82, 1238.4, 1257.68, 1237.78, 1236.52, 1234.9, 1237.9, 1238.58, 1238.12, 1237.89, 1236.54, 1236.55, 1238.37, 1237.29, 1237.64, 1236.8, 1237.73, 1236.71, 1238.23, 1237.84, 1236.26, 1237.58, 1238.31, 1238.4, 1237.08, 1236.61, 1235.92, 1236.41, 1237.89, 1237.98, 1246.75, 1237.92, 1237.1, 1237.97, 1238.69, 1237.05, 1236.96, 1239.44, 1238.49, 1237.88, 1236.01, 1236.57, 1236.44, 1235.76, 1237.62, 1238, 1263.14, 1237.66, 1237, 1236, 1261.96, 1238.58, 1237.77, 1237.06, 1236.31, 1238.63, 1237.23, 1236.85, 1236.23, 1236.46, 1236.9, 1237.85, 1238, 1237.02, 1236.19, 1236.05, 1235.73, 1258.3, 1235.98, 1237.76, 1246.93, 1239.1, 1237.72, 1237.67, 1236.79, 1237.61, 1238.41, 1238.29, 1238.11, 1237, 1236.52, 1236.6, 1236.31, 1237.77, 1238.58, 1237.88, 1247.35, 1236.14, 1236.83, 1236.15, 1237.93, 1238.16, 1237.34, 1236.78, 1238.66, 1237.76, 1237.19, 1236.7, 1236.04, 1236.66, 1237.86, 1238.54, 1238.05, 1238.41, 1236.94, 1240.95, 1261.01, 1237.72, 1237.91, 1238.2, 1235.68, 1236.89, 1235.12, 1271.31, 1236.97, 1270.76, 1238.52, 1238.19, 1238.6, 1237.16, 1236.72, 1236.71, 1237.14, 1238.48, 1237.95, 1237.42, 1235.86, 1236.39, 1236.13, 1236.58, 1237.95, 1237.76, 1237.39, 1238.16, 1236.31, 1236.41, 1236.12, 1238.7, 1236.48, 1237.84, 1236.38, 1237.95, 1238.48, 1236.51, 1236.56 };

            var gamma = GammaDistribution.Estimate(x);

            Assert.AreEqual(1238.8734170854279, gamma.Mean, 1e-10);
            Assert.AreEqual(41566.439533445438, gamma.Shape, 1e-10);
            Assert.AreEqual(0.029804655654680219, gamma.Scale, 1e-10);
        }

        [Test]
        public void FitTest2()
        {
            // Gamma Distribution Fit stalls for some arrays #301
            // https://github.com/accord-net/framework/issues/301

            double[] x = { 1.003, 1.012, 1.011, 1.057, 1.033, 1.051, 1.045, 1.045, 1.037, 1.059, 1.028, 1.032, 1.029, 1.031, 1.029, 1.023, 1.035 };

            var gamma = GammaDistribution.Estimate(x);

            Assert.AreEqual(1.0329411764705885, gamma.Mean, 1e-10);
            Assert.AreEqual(4679.4730379075245, gamma.Shape, 1e-10);
            Assert.AreEqual(0.00022073878150444029, gamma.Scale, 1e-10);
        }

        [Test]
        public void FitTestOptions()
        {
            // Gamma Distribution Fit stalls for some arrays #301
            // https://github.com/accord-net/framework/issues/301

            double[] x = { 1.003, 1.012, 1.011, 1.057, 1.033, 1.051, 1.045, 1.045, 1.037, 1.059, 1.028, 1.032, 1.029, 1.031, 1.029, 1.023, 1.035 };

            var gamma = GammaDistribution.Estimate(x, tol: 1e-2, iterations: 0);

            Assert.AreEqual(1.0329411764705885, gamma.Mean, 1e-10);
            Assert.AreEqual(4679.4730319532555, gamma.Shape, 1e-10);
            Assert.AreEqual(0.00022073878178531338, gamma.Scale, 1e-10);
        }

    }
}