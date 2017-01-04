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
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class EmpiricalDistributionTest
    {

        [Test]
        public void EmpiricalDistributionConstructorTest1()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            double smoothing = 0.5;

            EmpiricalDistribution target = new EmpiricalDistribution(samples, smoothing);
            Assert.AreEqual(samples, target.Samples);
            Assert.AreEqual(smoothing, target.Smoothing);

            Assert.AreEqual(3, target.Mean);
            Assert.AreEqual(1.1375929179890421, target.StandardDeviation);
            Assert.AreEqual(target.Variance, target.Variance);
        }

        [Test]
        public void EmpiricalDistributionConstructorTest2()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);
            Assert.AreEqual(samples, target.Samples);
            Assert.AreEqual(0.67595864392399474, target.Smoothing);
        }

        [Test]
        public void EmpiricalDistributionConstructorTest3()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            EmpiricalDistribution distribution = new EmpiricalDistribution(samples);

            double mean = distribution.Mean;     // 3
            double median = distribution.Median; // 2.9999993064186787
            double var = distribution.Variance;  // 1.2941176470588236
            double mode = distribution.Mode;     // 3

            double chf = distribution.CumulativeHazardFunction(x: 4.2); // 2.1972245773362191
            double cdf = distribution.DistributionFunction(x: 4.2); // 0.88888888888888884
            double pdf = distribution.ProbabilityDensityFunction(x: 4.2); // 0.181456280142802
            double lpdf = distribution.LogProbabilityDensityFunction(x: 4.2); // -1.7067405350495708
            double hf = distribution.HazardFunction(x: 4.2); // 1.6331065212852196
            double ccdf = distribution.ComplementaryDistributionFunction(x: 4.2); //0.11111111111111116
            double icdf = distribution.InverseDistributionFunction(p: cdf); // 4.1999999999999993
            double smoothing = distribution.Smoothing; // 0.67595864392399474

            string str = distribution.ToString(); // Fn(x; S)

            Assert.AreEqual(samples, distribution.Samples);
            Assert.AreEqual(0.67595864392399474, smoothing);
            Assert.AreEqual(3.0, mode);
            Assert.AreEqual(3.0, mean);
            Assert.AreEqual(2.9999993064186787, median);
            Assert.AreEqual(1.2941176470588236, var);
            Assert.AreEqual(2.1972245773362191, chf);
            Assert.AreEqual(0.88888888888888884, cdf);
            Assert.AreEqual(0.18145628014280227, pdf, 1e-15);
            Assert.AreEqual(-1.7067405350495708, lpdf);
            Assert.AreEqual(1.6331065212852196, hf, 1e-15);
            Assert.AreEqual(0.11111111111111116, ccdf);
            Assert.AreEqual(4.1999999999999993, icdf);
            Assert.AreEqual("Fn(x; S)", str);

            var range1 = distribution.GetRange(0.95);
            var range2 = distribution.GetRange(0.99);
            var range3 = distribution.GetRange(0.01);

            Assert.AreEqual(0.99999947547912593, range1.Min);
            Assert.AreEqual(5.0000002464240794, range1.Max);
            Assert.AreEqual(0.99999913215637204, range2.Min);
            Assert.AreEqual(5.0000004605903117, range2.Max);
            Assert.AreEqual(0.99999913215637204, range3.Min);
            Assert.AreEqual(5.0000004605903117, range3.Max);
        }

        [Test]
        public void WeightedEmpiricalDistributionConstructorTest()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            var distribution = new EmpiricalDistribution(original);

            int[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] samples = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };
            var target = new EmpiricalDistribution(samples, weights);

            Assert.AreEqual(distribution.Entropy, target.Entropy, 1e-10);
            Assert.AreEqual(distribution.Mean, target.Mean);
            Assert.AreEqual(distribution.Median, target.Median);
            Assert.AreEqual(distribution.Mode, target.Mode);
            Assert.AreEqual(distribution.Quartiles.Min, target.Quartiles.Min);
            Assert.AreEqual(distribution.Quartiles.Max, target.Quartiles.Max);
            Assert.AreEqual(distribution.Smoothing, target.Smoothing);
            Assert.AreEqual(distribution.StandardDeviation, target.StandardDeviation);
            Assert.AreEqual(distribution.Support.Min, target.Support.Min);
            Assert.AreEqual(distribution.Support.Max, target.Support.Max);
            Assert.AreEqual(distribution.Variance, target.Variance);
            Assert.IsTrue(target.Weights.IsEqual(weights.Divide(weights.Sum())));
            Assert.AreEqual(target.Samples, samples);

            for (double x = 0; x < 6; x += 0.1)
            {
                double actual, expected;
                expected = distribution.ComplementaryDistributionFunction(x);
                actual = target.ComplementaryDistributionFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.CumulativeHazardFunction(x);
                actual = target.CumulativeHazardFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.DistributionFunction(x);
                actual = target.DistributionFunction(x);
                Assert.AreEqual(expected, actual);

                expected = distribution.HazardFunction(x);
                actual = target.HazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.InverseDistributionFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                actual = target.InverseDistributionFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                Assert.AreEqual(expected, actual);

                expected = distribution.LogProbabilityDensityFunction(x);
                actual = target.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.ProbabilityDensityFunction(x);
                actual = target.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.QuantileDensityFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                actual = target.QuantileDensityFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                Assert.AreEqual(expected, actual, 1e-10);
            }
        }

        [Test]
        public void WeightedEmpiricalDistributionConstructorTest2()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            var distribution = new EmpiricalDistribution(original);

            double[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] samples = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };

            weights = weights.Divide(weights.Sum());

            var target = new EmpiricalDistribution(samples, weights, distribution.Smoothing);

            Assert.AreEqual(distribution.Mean, target.Mean);
            Assert.AreEqual(distribution.Median, target.Median);
            Assert.AreEqual(distribution.Mode, target.Mode);
            Assert.AreEqual(distribution.Quartiles.Min, target.Quartiles.Min);
            Assert.AreEqual(distribution.Quartiles.Max, target.Quartiles.Max);
            Assert.AreEqual(distribution.Smoothing, target.Smoothing);
            Assert.AreEqual(1.1685534824642432, target.StandardDeviation);
            Assert.AreEqual(distribution.Support.Min, target.Support.Min);
            Assert.AreEqual(distribution.Support.Max, target.Support.Max);
            Assert.AreEqual(1.3655172413793104, target.Variance);
            Assert.AreEqual(target.Weights, weights);
            Assert.AreEqual(target.Samples, samples);

            for (double x = 0; x < 6; x += 0.1)
            {
                double actual, expected;
                expected = distribution.ComplementaryDistributionFunction(x);
                actual = target.ComplementaryDistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.CumulativeHazardFunction(x);
                actual = target.CumulativeHazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.DistributionFunction(x);
                actual = target.DistributionFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.HazardFunction(x);
                actual = target.HazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-14);

                expected = distribution.InverseDistributionFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                actual = target.InverseDistributionFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                Assert.AreEqual(expected, actual, 1e-14);

                expected = distribution.LogProbabilityDensityFunction(x);
                actual = target.LogProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.ProbabilityDensityFunction(x);
                actual = target.ProbabilityDensityFunction(x);
                Assert.AreEqual(expected, actual, 1e-15);

                expected = distribution.QuantileDensityFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                actual = target.QuantileDensityFunction(Accord.Math.Tools.Scale(0, 6, 0, 1, x));
                Assert.AreEqual(expected, actual, 1e-10);
            }
        }

        [Test]
        public void WeightedEmpiricalDistributionConstructorTest3()
        {
            double[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] samples = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };

            weights = weights.Divide(weights.Sum());

            var target = new EmpiricalDistribution(samples, weights);

            Assert.AreEqual(1.2377597081667415, target.Smoothing);
        }

        [Test]
        public void EmpiricalDistributionConstructorTest4()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples, FaultySmoothingRule(samples));
            Assert.AreEqual(samples, target.Samples);
            Assert.AreEqual(1.9144923416414432, target.Smoothing);
        }

        [Test]
        public void EmpiricalDistributionConstructorTest5()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            EmpiricalDistribution distribution = new EmpiricalDistribution(samples, FaultySmoothingRule(samples));


            double mean = distribution.Mean; // 3
            double median = distribution.Median; // 2.9999993064186787
            double var = distribution.Variance; // 1.2941176470588236
            double chf = distribution.CumulativeHazardFunction(x: 4.2); // 2.1972245773362191
            double cdf = distribution.DistributionFunction(x: 4.2); // 0.88888888888888884
            double pdf = distribution.ProbabilityDensityFunction(x: 4.2); // 0.15552784414141974
            double lpdf = distribution.LogProbabilityDensityFunction(x: 4.2); // -1.8609305013898356
            double hf = distribution.HazardFunction(x: 4.2); // 1.3997505972727771
            double ccdf = distribution.ComplementaryDistributionFunction(x: 4.2); //0.11111111111111116
            double icdf = distribution.InverseDistributionFunction(p: cdf); // 4.1999999999999993
            double smoothing = distribution.Smoothing; // 1.9144923416414432

            string str = distribution.ToString(); // Fn(x; S)

            Assert.AreEqual(samples, distribution.Samples);
            Assert.AreEqual(1.9144923416414432, smoothing, 1.0e-15);
            Assert.AreEqual(3.0, mean);
            Assert.AreEqual(2.9999993064186787, median);
            Assert.AreEqual(1.2941176470588236, var);
            Assert.AreEqual(2.1972245773362191, chf);
            Assert.AreEqual(0.88888888888888884, cdf);
            Assert.AreEqual(0.15552784414141974, pdf, 1e-15);
            Assert.AreEqual(-1.8609305013898356, lpdf);
            Assert.AreEqual(1.3997505972727771, hf, 1e-15);
            Assert.AreEqual(0.11111111111111116, ccdf);
            Assert.AreEqual(4.1999999999999993, icdf);
            Assert.AreEqual("Fn(x; S)", str);
        }

        [Test]
        public void MedianTest()
        {
            double[] samples = { 1, 5, 2, 5, 1, 7, 1, 9 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void CloneTest()
        {
            double[] samples = { 4, 2 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);

            EmpiricalDistribution clone = (EmpiricalDistribution)target.Clone();

            Assert.AreNotSame(target, clone);
            Assert.AreEqual(target.Entropy, clone.Entropy);
            Assert.AreEqual(target.Mean, clone.Mean);
            Assert.AreNotSame(target.Samples, clone.Samples);
            Assert.AreEqual(target.StandardDeviation, clone.StandardDeviation);
            Assert.AreEqual(target.Variance, clone.Variance);

            for (int i = 0; i < clone.Samples.Length; i++)
                Assert.AreEqual(target.Samples[i], clone.Samples[i]);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            double[] samples = { 1, 5, 2, 5, 1, 7, 1, 9 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);

            Assert.AreEqual(0.000, target.DistributionFunction(0));
            Assert.AreEqual(0.375, target.DistributionFunction(1));
            Assert.AreEqual(0.500, target.DistributionFunction(2));
            Assert.AreEqual(0.750, target.DistributionFunction(5));
            Assert.AreEqual(0.875, target.DistributionFunction(7));
            Assert.AreEqual(1.000, target.DistributionFunction(9));
        }

        [Test]
        public void FitTest1()
        {
            EmpiricalDistribution target = new EmpiricalDistribution(new double[] { 0 });

            double[] observations = { 1, 5, 2, 5, 1, 7, 1, 9, 4, 2 };
            double[] weights = null;
            IFittingOptions options = null;

            target.Fit(observations, weights, options);
            Assert.AreEqual(1.8652004071576875, target.Smoothing);
            Assert.AreNotSame(observations, target.Samples);
            CollectionAssert.AreEqual(observations, target.Samples);
        }

        [Test]
        public void FitTest2()
        {
            EmpiricalDistribution target = new EmpiricalDistribution(new double[] { 0 });

            double[] observations = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            double[] weights = null;
            IFittingOptions options = new EmpiricalOptions { SmoothingRule = FaultySmoothingRule };

            target.Fit(observations, weights, options);
            Assert.AreEqual(1.9144923416414432, target.Smoothing);
            Assert.AreNotSame(observations, target.Samples);
            CollectionAssert.AreEqual(observations, target.Samples);
        }

        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            double[] samples = { 1, 5, 2, 5, 1, 7, 1, 9, 4, 2 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples, 1);

            Assert.AreEqual(1.0, target.Smoothing);

            double actual;

            actual = target.ProbabilityDensityFunction(1);
            Assert.AreEqual(0.16854678051819402, actual);

            actual = target.ProbabilityDensityFunction(2);
            Assert.AreEqual(0.15866528844260089, actual, 1e-15);

            actual = target.ProbabilityDensityFunction(3);
            Assert.AreEqual(0.0996000842425018, actual, 1e-15);

            actual = target.ProbabilityDensityFunction(4);
            Assert.AreEqual(0.1008594542833362, actual);

            actual = target.ProbabilityDensityFunction(6);
            Assert.AreEqual(0.078460710909263, actual, 1e-15);

            actual = target.ProbabilityDensityFunction(8);
            Assert.AreEqual(0.049293898826709738, actual, 1e-15);
        }

        [Test]
        public void LogProbabilityDensityFunctionTest()
        {
            double[] samples = { 1, 5, 2, 5, 1, 7, 1, 9, 4, 2 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples, 1);

            Assert.AreEqual(1.0, target.Smoothing);

            double actual;
            double expected;

            actual = target.LogProbabilityDensityFunction(1);
            expected = System.Math.Log(0.16854678051819402);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = target.LogProbabilityDensityFunction(2);
            expected = System.Math.Log(0.15866528844260089);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = target.LogProbabilityDensityFunction(3);
            expected = System.Math.Log(0.0996000842425018);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = target.LogProbabilityDensityFunction(4);
            expected = System.Math.Log(0.1008594542833362);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = target.LogProbabilityDensityFunction(6);
            expected = System.Math.Log(0.078460710909263);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = target.LogProbabilityDensityFunction(8);
            expected = System.Math.Log(0.049293898826709738);
            Assert.AreEqual(expected, actual, 1e-6);
        }


        private static double FaultySmoothingRule(double[] observations)
        {
            double sigma = Measures.StandardDeviation(observations);
            return sigma * Math.Pow(4.0 / (3.0 * observations.Length), -1 / 5.0);
        }

        private static double FaultySmoothingRule(double[] observations, double[] weights, int[] repeats)
        {
            return FaultySmoothingRule(observations);
        }
    }
}
