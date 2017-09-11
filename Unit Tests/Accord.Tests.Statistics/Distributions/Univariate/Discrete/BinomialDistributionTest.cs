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
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System.Globalization;

    [TestFixture]
    public class BinomialDistributionTest
    {


        [Test]
        public void ConstructorTest()
        {
            var bin = new BinomialDistribution(trials: 16, probability: 0.12);

            double mean = bin.Mean;     // 1.92
            double median = bin.Median; // 2
            double var = bin.Variance;  // 1.6896
            double mode = bin.Mode;     // 2

            double cdf = bin.DistributionFunction(k: 0);    // 0.12933699143209909
            double pdf = bin.ProbabilityMassFunction(k: 1); // 0.28218979948821621
            double lpdf = bin.LogProbabilityMassFunction(k: 0); // -2.0453339441581582

            double ccdf = bin.ComplementaryDistributionFunction(k: 0); // 0.87066300856790091
            int icdf0 = bin.InverseDistributionFunction(p: 0.37); // 1
            int icdf1 = bin.InverseDistributionFunction(p: 0.50); // 2
            int icdf2 = bin.InverseDistributionFunction(p: 0.99); // 5
            int icdf3 = bin.InverseDistributionFunction(p: 0.999); // 7

            double hf = bin.HazardFunction(x: 0); // 1.3809523809523814
            double chf = bin.CumulativeHazardFunction(x: 0); // 0.86750056770472328

            string str = bin.ToString(CultureInfo.InvariantCulture); // "Binomial(x; n = 16, p = 0.12)"

            double[] probabilities = new double[10];
            for (int i = 0; i < probabilities.Length; i++)
                probabilities[i] = bin.ProbabilityMassFunction(i);

            Assert.AreEqual(1.92, mean);
            Assert.AreEqual(2, median);
            Assert.AreEqual(2, mode);
            Assert.AreEqual(1.6896, var);
            Assert.AreEqual(0.13850027875444251, chf, 1e-10);
            Assert.AreEqual(0.12933699143209909, cdf, 1e-10);
            Assert.AreEqual(0.28218979948821621, pdf, 1e-10);
            Assert.AreEqual(-2.0453339441581582, lpdf);
            Assert.AreEqual(0.14855000173354949, hf, 1e-10);
            Assert.AreEqual(0.87066300856790091, ccdf, 1e-10);
            Assert.AreEqual(1, icdf0);
            Assert.AreEqual(2, icdf1);
            Assert.AreEqual(5, icdf2);
            Assert.AreEqual(7, icdf3);
            Assert.AreEqual("Binomial(x; n = 16, p = 0.12)", str);



            var range1 = bin.GetRange(0.95);
            var range2 = bin.GetRange(0.99);
            var range3 = bin.GetRange(0.01);

            Assert.AreEqual(0.0, range1.Min);
            Assert.AreEqual(4.0, range1.Max);
            Assert.AreEqual(0.0, range2.Min);
            Assert.AreEqual(5.0, range2.Max);
            Assert.AreEqual(0.0, range3.Min);
            Assert.AreEqual(5.0, range3.Max);
        }

        [Test]
        public void ProbabilityMassFunctionTest()
        {
            double[] expected =
            {
                0.0, 0.0260838446329553, 0.104335628936830,  0.198238170750635,
                0.237886375826922,  0.202203904741285,  0.129410809619744,
                0.0647055601029058, 0.0258822861585249, 0.00841176318971187,
                0.00224314223412042
            };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.ProbabilityMassFunction(i - 1);
                Assert.AreEqual(expected[i], actual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));

                double logActual = target.LogProbabilityMassFunction(i - 1);
                Assert.AreEqual(Math.Log(expected[i]), logActual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [Test]
        public void FitTest()
        {
            int trials = 5;
            BinomialDistribution target = new BinomialDistribution(trials);

            double[] observations = { 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 };
            double[] weights = null;
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            Assert.AreEqual(5, target.NumberOfTrials);
            Assert.AreEqual(0.066666666666666666, target.ProbabilityOfSuccess, 1e-10);
        }

        [Test]
        public void BinomialDistributionConstructorTest()
        {
            int trials = 5;
            double probability = 0.52;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            Assert.AreEqual(trials, target.NumberOfTrials);
            Assert.AreEqual(probability, target.ProbabilityOfSuccess);
            Assert.AreEqual(trials * probability, target.Mean);
            Assert.AreEqual(trials * probability * (1 - probability), target.Variance);
        }

        [Test]
        public void CloneTest()
        {
            int trials = 3;
            double probability = 0.52;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            BinomialDistribution actual = (BinomialDistribution)target.Clone();

            Assert.AreNotSame(target, actual);
            Assert.AreNotEqual(target, actual);

            Assert.AreEqual(target.NumberOfTrials, actual.NumberOfTrials);
            Assert.AreEqual(target.ProbabilityOfSuccess, actual.ProbabilityOfSuccess);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            // http://www.stat.yale.edu/Courses/1997-98/101/binom.htm
            // Verified in http://stattrek.com/online-calculator/binomial.aspx

            double[] pmf = { 0.0260838446329553, 0.10433562893683, 0.198238170750635, 0.237886375826923, 0.202203904741285 };
            double[] cdfLess = { 0.0000000000000000, 0.0260838446329553, 0.130419473569785, 0.32865764432042, 0.566544020147343 };
            double[] cdfLessEqual = { 0.0260838446329553, 0.130419473569785, 0.32865764432042, 0.566544020147343, 0.768747924888628 };
            double[] cdfGreater = { 0.973916155367045, 0.869580526430215, 0.67134235567958, 0.433455979852657, 0.231252075111372 };
            double[] cdfGreaterEqual = { 1.000000000000000, 0.973916155367045, 0.869580526430215, 0.67134235567958, 0.433455979852657 };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < pmf.Length; i++)
            {
                {   // P(X = i)
                    double actual = target.ProbabilityMassFunction(i);
                    Assert.AreEqual(pmf[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X <= i)
                    double actual = target.DistributionFunction(i);
                    Assert.AreEqual(cdfLessEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X < i)
                    double actual = target.DistributionFunction(i, inclusive: false);
                    Assert.AreEqual(cdfLess[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X > i)
                    double actual = target.ComplementaryDistributionFunction(i);
                    Assert.AreEqual(cdfGreater[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X >= i)
                    double actual = target.ComplementaryDistributionFunction(i, inclusive: true);
                    Assert.AreEqual(cdfGreaterEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }
            }
        }

        [Test]
        public void DistributionFunctionTest2()
        {
            // http://www.stat.yale.edu/Courses/1997-98/101/binom.htm
            // Verified in http://stattrek.com/online-calculator/binomial.aspx

            double[] expected = { 0.0260838, 0.130419, 0.3287, 0.5665, 0.7687, 0.8982, 0.9629, 0.9887, 0.9972, 0.9994 };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual, 1e-4);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [Test]
        public void InverseDistributionFunctionTest()
        {
            double[] pvalues = { 0.0260838, 0.130419, 0.3287, 0.5665, 0.7687, 0.8982, 0.9629, 0.9887, 0.9972, 0.9994 };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < pvalues.Length; i++)
            {
                double p = pvalues[i] + 1e-4;
                double actual = target.InverseDistributionFunction(p);

                Assert.AreEqual(i + 1, actual);
            }
        }

        [Test]
        public void DistributionFunctionTest3()
        {
            // http://www.danielsoper.com/statcalc3/calc.aspx?id=70

            double probability = 0.2;
            int successes = 2;
            int trials = 10;

            BinomialDistribution target = new BinomialDistribution(trials, probability);

            double actual = target.DistributionFunction(successes);
            double expected = 0.67779953;

            Assert.AreEqual(expected, actual, 1e-5);

        }


        [Test]
        public void MedianTest()
        {
            int trials = 5;

            {
                BinomialDistribution target = new BinomialDistribution(trials, 0.0);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BinomialDistribution target = new BinomialDistribution(trials, 0.2);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BinomialDistribution target = new BinomialDistribution(trials, 0.6);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BinomialDistribution target = new BinomialDistribution(trials, 0.8);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }

            {
                BinomialDistribution target = new BinomialDistribution(trials, 1.0);
                Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            }
        }

        [Test]
        public void icdf()
        {
            int trials = 1;

            BinomialDistribution dist = new BinomialDistribution(trials, 0.5);
            double icdf000 = dist.InverseDistributionFunction(0.0);
            Assert.AreEqual(0, icdf000);

            double icdf001 = dist.InverseDistributionFunction(0.1);
            Assert.AreEqual(0, icdf001);

            double icdf050 = dist.InverseDistributionFunction(0.5); // important
            Assert.AreEqual(0, icdf050);

            double icdf090 = dist.InverseDistributionFunction(0.9);
            Assert.AreEqual(1, icdf090);

            double icdf100 = dist.InverseDistributionFunction(0.9);
            Assert.AreEqual(1, icdf100);

            Assert.AreEqual(dist.Median, icdf050);
        }

        [Test]
        public void icdf2()
        {
            int trials = 1;

            BinomialDistribution dist = new BinomialDistribution(trials, 0.5);

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            for (int i = 0; i < percentiles.Length; i++)
            {
                double x = percentiles[i];
                int icdf = dist.InverseDistributionFunction(x);
                double cdf = dist.DistributionFunction(icdf);
                int iicdf = dist.InverseDistributionFunction(cdf);
                double iiicdf = dist.DistributionFunction(iicdf);
                Assert.AreEqual(iicdf, icdf, 1e-5);

                double rx = Math.Round(x);
                double rc = Math.Round(cdf);

                Assert.AreEqual(rx, rc, 1e-5);
                Assert.AreEqual(iiicdf, cdf, 1e-5);
            }
        }

        [Test]
        public void OverflowTest()
        {
            // http://stackoverflow.com/questions/23222097/accord-net-binomial-probability-mass-function-result-differs-from-excel-result
            var binom = new BinomialDistribution(3779, 0.0638);

            double expected = 0.021944019794458;
            double actual = binom.ProbabilityMassFunction(250);

            Assert.AreEqual(expected, actual, 1e-7);
        }

        [Test]
        [Category("Slow")]
        public void GenerateTest()
        {
            var target = new BinomialDistribution(4, 0.2);

            int[] samples = target.Generate(1000000);

            target.Fit(samples);

            Assert.AreEqual(4, target.NumberOfTrials, 0.01);
            Assert.AreEqual(0.2, target.ProbabilityOfSuccess, 1e-3);
        }

        [Test]
        public void GenerateTest2()
        {
            var target = new BinomialDistribution(4, 0.2);

            double[] samples = target.Generate(1000000).ToDouble();

            target.Fit(samples);

            Assert.AreEqual(4, target.NumberOfTrials, 0.01);
            Assert.AreEqual(0.2, target.ProbabilityOfSuccess, 1e-3);
        }

        [Test]
        public void GenerateTest3()
        {
            var target = new BinomialDistribution(4);

            int[] samples = { 1, 0, 3, 1, 2, 3 };

            target.Fit(samples);

            Assert.AreEqual(4, target.NumberOfTrials, 0.01);
            Assert.AreEqual(0.41666666666666669, target.ProbabilityOfSuccess, 1e-3);
        }
    }
}
