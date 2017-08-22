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
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using System.Globalization;

    [TestFixture]
    public class UniformDistributionTest
    {


        [Test]
        public void ConstructorTest()
        {
            var uniform = new UniformContinuousDistribution(a: 0.42, b: 1.1);

            double mean = uniform.Mean;     // 0.76
            double median = uniform.Median; // 0.76
            double var = uniform.Variance;  // 0.03853333333333335
            double mode = uniform.Mode;     // 0.76000000000000001

            double cdf = uniform.DistributionFunction(x: 0.9); // 0.70588235294117641
            double pdf = uniform.ProbabilityDensityFunction(x: 0.9); // 1.4705882352941173
            double lpdf = uniform.LogProbabilityDensityFunction(x: 0.9); // 0.38566248081198445

            double ccdf = uniform.ComplementaryDistributionFunction(x: 0.9); // 0.29411764705882359
            double icdf = uniform.InverseDistributionFunction(p: cdf); // 0.9

            double hf = uniform.HazardFunction(x: 0.9); // 4.9999999999999973
            double chf = uniform.CumulativeHazardFunction(x: 0.9); // 1.2237754316221154

            string str = uniform.ToString(CultureInfo.InvariantCulture); // "U(x; a = 0.42, b = 1.1)"

            Assert.AreEqual(0.76, mean);
            Assert.AreEqual(0.76, median);
            Assert.AreEqual(0.76000000000000001, mode);
            Assert.AreEqual(0.03853333333333335, var);
            Assert.AreEqual(1.2237754316221154, chf);
            Assert.AreEqual(0.70588235294117641, cdf);
            Assert.AreEqual(1.4705882352941173, pdf);
            Assert.AreEqual(0.38566248081198445, lpdf);
            Assert.AreEqual(4.9999999999999973, hf);
            Assert.AreEqual(0.29411764705882359, ccdf);
            Assert.AreEqual(0.9, icdf);
            Assert.AreEqual("U(x; a = 0.42, b = 1.1)", str);

            var range1 = uniform.GetRange(0.95);
            var range2 = uniform.GetRange(0.99);
            var range3 = uniform.GetRange(0.01);

            Assert.AreEqual(0.45400000000000001, range1.Min);
            Assert.AreEqual(1.0660000000000001, range1.Max);
            Assert.AreEqual(0.42680000000000001, range2.Min);
            Assert.AreEqual(1.0932000000000002, range2.Max);
            Assert.AreEqual(0.42680000000000001, range3.Min);
            Assert.AreEqual(1.0932000000000002, range3.Max);

            Assert.AreEqual(0.42, uniform.Support.Min);
            Assert.AreEqual(1.1, uniform.Support.Max);

            Assert.AreEqual(uniform.InverseDistributionFunction(0), uniform.Support.Min);
            Assert.AreEqual(uniform.InverseDistributionFunction(1), uniform.Support.Max);
        }

        [Test]
        public void RTest()
        {
            var target = new UniformContinuousDistribution(3, 6);

            Assert.AreEqual(0, target.ProbabilityDensityFunction(2));
            Assert.AreEqual(1 / 3.0, target.ProbabilityDensityFunction(3), 1e-10);
            Assert.AreEqual(1 / 3.0, target.ProbabilityDensityFunction(4), 1e-10);
            Assert.AreEqual(1 / 3.0, target.ProbabilityDensityFunction(5), 1e-10);
            Assert.AreEqual(1 / 3.0, target.ProbabilityDensityFunction(6), 1e-10);
            Assert.AreEqual(0, target.ProbabilityDensityFunction(7));

            Assert.AreEqual(0, target.DistributionFunction(2));
            Assert.AreEqual(0, target.DistributionFunction(3));
            Assert.AreEqual(1 / 3.0, target.DistributionFunction(4), 1e-10);
            Assert.AreEqual(2 / 3.0, target.DistributionFunction(5), 1e-10);
            Assert.AreEqual(1, target.DistributionFunction(6), 1e-10);
            Assert.AreEqual(1, target.DistributionFunction(7));
        }

        [Test]
        public void IntervalTest()
        {
            var target = new UniformContinuousDistribution(-10, 10);

            for (int k = -15; k < 15; k++)
            {
                double a = target.DistributionFunction(k);
                double b = target.DistributionFunction(k + 1);
                double c = b - a; 

                double actual = target.DistributionFunction(k, k + 1);
                Assert.AreEqual(c, actual, 1e-15);
            }
        }

        [Test]
        public void UniformDistributionConstructorTest()
        {
            double a = 1;
            double b = 5;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);
            Assert.AreEqual(target.Minimum, a);
            Assert.AreEqual(target.Maximum, b);
        }

        [Test]
        public void UniformDistributionConstructorTest1()
        {
            double a = 6;
            double b = 5;

            bool thrown = false;
            try { UniformContinuousDistribution target = new UniformContinuousDistribution(a, b); }
            catch (ArgumentOutOfRangeException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void VarianceTest()
        {
            double a = 5;
            double b = 10;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);
            double actual = target.Variance;
            double expected = System.Math.Pow(b - a, 2) / 12.0;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest()
        {
            double a = -1;
            double b = 5;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);
            double expected = (a + b) / 2.0;
            double actual = target.Mean;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EntropyTest()
        {
            double a = 1;
            double b = 6;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);
            double expected = System.Math.Log(b - a);
            double actual = target.Entropy;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MedianTest()
        {
            double a = 1;
            double b = 6;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            double a = -5;
            double b = 11;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);
            double x = 4.2;
            double expected = 0.0625;
            double actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = -5;
            expected = 0.0625;
            actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = -6;
            expected = 0.0;
            actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = 11;
            expected = 0.0625;
            actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = 12;
            expected = 0.0;
            actual = target.ProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LogProbabilityDensityFunctionTest()
        {
            double a = -5;
            double b = 11;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);
            double x = 4.2;
            double expected = System.Math.Log(0.0625);
            double actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = -5;
            expected = System.Math.Log(0.0625);
            actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = -6;
            expected = System.Math.Log(0.0);
            actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = 11;
            expected = System.Math.Log(0.0625);
            actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);

            x = 12;
            expected = System.Math.Log(0.0);
            actual = target.LogProbabilityDensityFunction(x);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest()
        {
            UniformContinuousDistribution target = new UniformContinuousDistribution();
            double[] observations = { -1, 2, 5, 2, 3, 1, 4 };
            double[] weights = null;
            target.Fit(observations, weights);
            Assert.AreEqual(-1.0, target.Minimum);
            Assert.AreEqual(5.0, target.Maximum);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            double a = -2;
            double b = 2;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);

            double actual;

            actual = target.DistributionFunction(-2);
            Assert.AreEqual(0, actual);

            actual = target.DistributionFunction(-1);
            Assert.AreEqual(0.25, actual);

            actual = target.DistributionFunction(0);
            Assert.AreEqual(0.5, actual);

            actual = target.DistributionFunction(1);
            Assert.AreEqual(0.75, actual);

            actual = target.DistributionFunction(2);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void CloneTest()
        {
            double a = 12;
            double b = 72;
            UniformContinuousDistribution target = new UniformContinuousDistribution(a, b);

            UniformContinuousDistribution clone = (UniformContinuousDistribution)target.Clone();

            Assert.AreNotSame(target, clone);
            Assert.AreEqual(target.Entropy, clone.Entropy);
            Assert.AreEqual(target.Maximum, clone.Maximum);
            Assert.AreEqual(target.Minimum, clone.Minimum);
            Assert.AreEqual(target.StandardDeviation, clone.StandardDeviation);
            Assert.AreEqual(target.Variance, clone.Variance);
        }


        [Test]
        public void GenerateTest()
        {
            UniformContinuousDistribution target = new UniformContinuousDistribution(0, 2);

            double[] samples = target.Generate(1000000);

            for (int i = 0; i < samples.Length; i++)
            {
                Assert.IsTrue(samples[i] >= 0);
                Assert.IsTrue(samples[i] <= 2);
            }

            UniformContinuousDistribution newTarget = new UniformContinuousDistribution();
            newTarget.Fit(samples);

            Assert.AreEqual(0, newTarget.Minimum, 1e-5);
            Assert.AreEqual(2, newTarget.Maximum, 1e-5);
        }

        [Test]
        public void GenerateTest2()
        {
            UniformContinuousDistribution target = new UniformContinuousDistribution(0, 2);

            double[] samples = new double[1000000];

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = target.Generate();
                Assert.IsTrue(samples[i] >= 0);
                Assert.IsTrue(samples[i] <= 2);
            }

            UniformContinuousDistribution newTarget = new UniformContinuousDistribution();
            newTarget.Fit(samples);

            Assert.AreEqual(0, newTarget.Minimum, 1e-5);
            Assert.AreEqual(2, newTarget.Maximum, 1e-5);
        }

    }
}
