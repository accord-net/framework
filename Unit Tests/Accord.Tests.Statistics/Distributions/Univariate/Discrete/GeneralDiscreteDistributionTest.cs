﻿// Accord Unit Tests
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
    using Accord.Math;
    using System.Globalization;
    using System;

    [TestFixture]
    public class DiscreteDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            // Create a Categorical distribution for 3 symbols, in which
            // the first and second symbol have 25% chance of appearing,
            // and the third symbol has 50% chance of appearing.

            //                         1st   2nd   3rd
            double[] probabilities = { 0.25, 0.25, 0.50 };

            var dist = new GeneralDiscreteDistribution(probabilities);

            double mean = dist.Mean;     // 1.25
            double median = dist.Median; // 1.00
            double var = dist.Variance;  // 0.6875

            double cdf = dist.DistributionFunction(k: 2);    // 1

            double pdf1 = dist.ProbabilityMassFunction(k: 0); // 0.25
            double pdf2 = dist.ProbabilityMassFunction(k: 1); // 0.25
            double pdf3 = dist.ProbabilityMassFunction(k: 2); // 0.50

            double lpdf = dist.LogProbabilityMassFunction(k: 2); // -0.69314718055994529

            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.0

            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 0
            int icdf2 = dist.InverseDistributionFunction(p: 0.39); // 1
            int icdf3 = dist.InverseDistributionFunction(p: 0.56); // 2

            double hf = dist.HazardFunction(x: 0); // 0.33333333333333331
            double chf = dist.CumulativeHazardFunction(x: 0); // 0.2876820724517809

            string str = dist.ToString(CultureInfo.InvariantCulture); // "Categorical(x; p = { 0.25, 0.25, 0.5 })"

            Assert.AreEqual(1.25, mean);
            Assert.AreEqual(1.00, median);
            Assert.AreEqual(0.6875, var);
            Assert.AreEqual(0.2876820724517809, chf, 1e-10);
            Assert.AreEqual(1.0, cdf);
            Assert.AreEqual(0.25, pdf1);
            Assert.AreEqual(0.25, pdf2);
            Assert.AreEqual(0.5, pdf3);
            Assert.AreEqual(-0.69314718055994529, lpdf);
            Assert.AreEqual(0.33333333333333331, hf, 1e-10);
            Assert.AreEqual(0.0, ccdf);
            Assert.AreEqual(0, icdf1);
            Assert.AreEqual(1, icdf2);
            Assert.AreEqual(2, icdf3);
            Assert.AreEqual("Categorical(x; p = { 0.25, 0.25, 0.5 })", str);

            var range1 = dist.GetRange(0.95);
            var range2 = dist.GetRange(0.99);
            var range3 = dist.GetRange(0.01);

            Assert.AreEqual(0, range1.Min);
            Assert.AreEqual(2, range1.Max);
            Assert.AreEqual(0, range2.Min);
            Assert.AreEqual(2, range2.Max);
            Assert.AreEqual(0, range3.Min);
            Assert.AreEqual(2, range3.Max);
        }

        [Test]
        public void IntervalTest()
        {
            var target = GeneralDiscreteDistribution.Uniform(-10, 10);

            for (int k = -15; k < 15; k++)
            {
                double expected = target.ProbabilityMassFunction(k);

                double a = target.DistributionFunction(k);
                double b = target.DistributionFunction(k - 1);
                double c = a - b;

                Assert.AreEqual(expected, c, 1e-15);
                Assert.AreEqual(c, target.DistributionFunction(k - 1, k), 1e-15);
            }
        }

        [Test]
        public void ConstructorTest2()
        {
            double[] probabilities = { 0.25, 0.25, 0.50 };

            var dist = new GeneralDiscreteDistribution(probabilities);

            double var = dist.Variance;  // 0.6875
            double median = dist.Median; // 1.00
            double mean = dist.Mean;     // 1.25

            Assert.AreEqual(1.25, mean);
            Assert.AreEqual(1.00, median);
            Assert.AreEqual(0.6875, var);
        }

        [Test]
        public void FitTest()
        {
            GeneralDiscreteDistribution target = new GeneralDiscreteDistribution(4);
            double[] values = { 0.00, 1.00, 2.00, 3.00 };
            double[] weights = { 0.25, 0.25, 0.25, 0.25 };

            target.Fit(values, weights);

            double[] expected = { 0.25, 0.25, 0.25, 0.25 };
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void FitTest2()
        {
            double[] expected = { 0.50, 0.00, 0.25, 0.25 };

            GeneralDiscreteDistribution target;

            double[] values = { 0.00, 2.00, 3.00 };
            double[] weights = { 0.50, 0.25, 0.25 };
            target = new GeneralDiscreteDistribution(4);
            target.Fit(values, weights);
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            // --

            double[] values2 = { 0.00, 0.00, 2.00, 3.00 };
            double[] weights2 = { 0.25, 0.25, 0.25, 0.25 };
            target = new GeneralDiscreteDistribution(4);
            target.Fit(values2, weights2);
            double[] actual2 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual2));
        }

        [Test]
        public void FitTest3()
        {
            GeneralDiscreteDistribution target = new GeneralDiscreteDistribution(-1, 4);
            double[] values = { 0.00, 1.00, 2.00, 3.00 };
            double[] weights = { 0.25, 0.25, 0.25, 0.25 };

            target.Fit(values.Subtract(1), weights);

            double[] expected = { 0.25, 0.25, 0.25, 0.25 };
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void FitTest4()
        {
            double[] expected = { 0.50, 0.00, 0.25, 0.25 };

            GeneralDiscreteDistribution target;

            double[] values = { 0.00, 2.00, 3.00 };
            double[] weights = { 0.50, 0.25, 0.25 };
            target = new GeneralDiscreteDistribution(-1, 4);
            target.Fit(values.Subtract(1), weights);
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            // --

            double[] values2 = { 0.00, 0.00, 2.00, 3.00 };
            double[] weights2 = { 0.25, 0.25, 0.25, 0.25 };
            target = new GeneralDiscreteDistribution(-1, 4);
            target.Fit(values2.Subtract(1), weights2);
            double[] actual2 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual2));
        }

        [Test]
        public void FitTest_vector_inputs()
        {
            double[] expected = { 0.50, 0.00, 0.25, 0.25 };

            GeneralDiscreteDistribution target;

            double[] values = { 0.00, 2.00, 3.00 };
            double[] weights = { 0.50, 0.25, 0.25 };
            target = new GeneralDiscreteDistribution(4);
            target.Fit(values, weights);
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            // --

            double[][] values2 =
            {
                new[] { 1.00, 0.00, 0.00, 0.00 },
                new[] { 0.00, 0.00, 0.00, 0.00 },
                new[] { 0.00, 0.00, 1.00, 0.00 },
                new[] { 0.00, 0.00, 0.00, 1.00 },
            };

            double[] weights2 = { 0.50, 0.00, 0.25, 0.25 };

            target = new GeneralDiscreteDistribution(4);
            target.Fit(values2, weights2);
            double[] actual2 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual2));


            double[][] values3 =
            {
                new[] { 1.00, 0.00, 0.00, 0.00 },
                new[] { 0.00, 1.00, 0.00, 0.00 },
                new[] { 0.00, 0.00, 1.00, 0.00 },
                new[] { 0.00, 0.00, 0.00, 1.00 },
            };

            double[] weights3 = { 0.50, 0.00, 0.25, 0.25 };

            target = new GeneralDiscreteDistribution(4);
            target.Fit(values3, weights3);
            double[] actual3 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual3));


            double[][] values4 =
            {
                new[] { 0.50, 0.00, 0.00, 0.00 },
                new[] { 0.00, 0.00, 0.00, 0.00 },
                new[] { 0.00, 0.00, 0.25, 0.00 },
                new[] { 0.00, 0.00, 0.00, 0.25 },
            };

            target = new GeneralDiscreteDistribution(4);
            target.Fit(values4);
            double[] actual4 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual4));
        }

        [Test]
        public void DistributionFunctionTest()
        {
            var target = new GeneralDiscreteDistribution(0.1, 0.4, 0.5);

            double actual;

            actual = target.DistributionFunction(0);
            Assert.AreEqual(0.1, actual, 1e-6);

            actual = target.DistributionFunction(1);
            Assert.AreEqual(0.5, actual, 1e-6);

            actual = target.DistributionFunction(2);
            Assert.AreEqual(1.0, actual, 1e-6);

            actual = target.DistributionFunction(3);
            Assert.AreEqual(1.0, actual, 1e-6);


            Assert.AreEqual(1.3999999, target.Mean, 1e-6);
        }

        [Test]
        public void MeanTest()
        {
            var target = new GeneralDiscreteDistribution(0.1, 0.4, 0.5);
            double expected = 0 * 0.1 + 1 * 0.4 + 2 * 0.5;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest2()
        {
            var target = new GeneralDiscreteDistribution(42, 0.1, 0.4, 0.5);
            double expected = 42 * 0.1 + 43 * 0.4 + 44 * 0.5;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest3()
        {
            var target = new GeneralDiscreteDistribution(2, 0.5, 0.5);
            double expected = (2.0 + 3.0) / 2.0;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VarianceTest()
        {
            var target = new GeneralDiscreteDistribution(42, 0.1, 0.4, 0.5);
            double mean = target.Mean;
            double expected = ((42 - mean) * (42 - mean) * 0.1
                             + (43 - mean) * (43 - mean) * 0.4
                             + (44 - mean) * (44 - mean) * 0.5);

            Assert.AreEqual(expected, target.Variance);
        }

        [Test]
        public void EntropyTest()
        {
            var target = new GeneralDiscreteDistribution(42, 0.1, 0.4, 0.5);
            double expected = -0.1 * System.Math.Log(0.1) +
                              -0.4 * System.Math.Log(0.4) +
                              -0.5 * System.Math.Log(0.5);

            Assert.AreEqual(expected, target.Entropy);
        }

        [Test]
        public void MedianTest()
        {
            var target = new GeneralDiscreteDistribution(0.1, 0.4, 0.5);
            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void UniformTest()
        {
            int a = 2;
            int b = 5;
            int n = b - a + 1;

            // Wikipedia definitions
            double expectedMean = (a + b) / 2.0;
            double expectedVar = (System.Math.Pow(b - a + 1, 2) - 1) / 12.0;
            double p = 1.0 / n;


            GeneralDiscreteDistribution dist = GeneralDiscreteDistribution.Uniform(a, b);

            Assert.AreEqual(expectedMean, dist.Mean); ;
            Assert.AreEqual(expectedVar, dist.Variance);
            Assert.AreEqual(n, dist.Frequencies.Length);


            Assert.AreEqual(0, dist.ProbabilityMassFunction(0));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(1));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(2));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(3));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(4));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(5));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(6));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(7));
        }

        [Test]
        public void ProbabilityMassFunctionTest()
        {
            GeneralDiscreteDistribution dist = GeneralDiscreteDistribution.Uniform(2, 5);
            double p = 0.25;
            Assert.AreEqual(0, dist.ProbabilityMassFunction(0));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(1));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(2));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(3));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(4));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(5));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(6));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(7));
        }

        [Test]
        public void LogProbabilityMassFunctionTest()
        {
            GeneralDiscreteDistribution dist = GeneralDiscreteDistribution.Uniform(2, 5);

            double p = System.Math.Log(0.25);
            double l = System.Math.Log(0);

            Assert.AreEqual(l, dist.LogProbabilityMassFunction(0));
            Assert.AreEqual(l, dist.LogProbabilityMassFunction(1));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(2));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(3));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(4));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(5));
            Assert.AreEqual(l, dist.LogProbabilityMassFunction(6));
            Assert.AreEqual(l, dist.LogProbabilityMassFunction(7));
        }


        [Test]
        public void GenerateTest()
        {
            double[] transProbRow = { 0.2, 0.5, 0.3 };
            var gdd = new GeneralDiscreteDistribution(transProbRow);
            int n = 100000;
            {
                int[] samples = gdd.Generate(n);
                var target = new GeneralDiscreteDistribution(3);
                target.Fit(samples);
                Assert.AreEqual(0.2, target.Frequencies[0], 0.01);
                Assert.AreEqual(0.5, target.Frequencies[1], 0.01);
                Assert.AreEqual(0.3, target.Frequencies[2], 0.01);
            }

            {
                int[] samples = new int[n].Apply((x) => gdd.Generate());
                var target = new GeneralDiscreteDistribution(3);
                target.Fit(samples);
                Assert.AreEqual(0.2, target.Frequencies[0], 0.01);
                Assert.AreEqual(0.5, target.Frequencies[1], 0.01);
                Assert.AreEqual(0.3, target.Frequencies[2], 0.01);
            }

            {
                double[] samples = gdd.Generate(n).ToDouble();
                var target = new GeneralDiscreteDistribution(3);
                target.Fit(samples);
                Assert.AreEqual(0.2, target.Frequencies[0], 0.01);
                Assert.AreEqual(0.5, target.Frequencies[1], 0.01);
                Assert.AreEqual(0.3, target.Frequencies[2], 0.01);
            }

            {
                double[] samples = new int[n].Apply((x) => (double)gdd.Generate());
                var target = new GeneralDiscreteDistribution(3);
                target.Fit(samples);
                Assert.AreEqual(0.2, target.Frequencies[0], 0.01);
                Assert.AreEqual(0.5, target.Frequencies[1], 0.01);
                Assert.AreEqual(0.3, target.Frequencies[2], 0.01);
            }
        }


        [Test]
        public void RandomTest()
        {
            Assert.Throws<InvalidOperationException>(() => GeneralDiscreteDistribution.Random(new[] { 1e-14, 1e-15 }));
            Assert.Throws<ArgumentException>(() => GeneralDiscreteDistribution.Random(new[] { 0.0, 0.0 }));
        }
    }
}
