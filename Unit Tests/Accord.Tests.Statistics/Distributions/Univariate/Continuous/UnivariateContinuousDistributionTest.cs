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
    using Accord.Statistics.Distributions;
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Math;

    [TestFixture]
    public class UnivariateContinuousDistributionTest
    {

        internal virtual UnivariateContinuousDistribution CreateUnivariateContinuousDistribution()
        {
            double mean = 23;
            double var = 4.2;
            double dev = System.Math.Sqrt(var);
            return new NormalDistribution(mean, dev);
        }



        [Test]
        public void VarianceTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.Variance;
            double expected = 4.2;

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void StandardDeviationTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.StandardDeviation;
            double expected = System.Math.Sqrt(4.2);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.Mean;
            double expected = 23;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EntropyTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.Entropy;
            double expected = 0.5 * System.Math.Log(2 * System.Math.PI * System.Math.E * 4.2);

            Assert.AreEqual(expected, actual);
        }



        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.011697993604177;
            double actual = target.ProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void LogProbabilityDensityFunctionTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = System.Math.Log(0.011697993604177);
            double actual = target.LogProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void ProbabilityFunctionTest()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.011697993604177;
            double actual = target.ProbabilityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-10);
        }



        [Test]
        public void DistributionFunctionTest1()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.008859529329290905;
            double actual = target.DistributionFunction(x);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.008859529329290905;
            double actual = target.DistributionFunction(x);

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void fit_example()
        {
            #region doc_fit
            // Let's say we have a UnivariateContinuousDistribution that we have
            // built somehow, either using a constructor for a common distribution
            // or that we have received as the output of a method we have called:
            UnivariateContinuousDistribution dist = new NormalDistribution();

            // Let's say we have a set of observations, and some optional weights:
            double[] observations = { 0.12, 2, 0.52 };

            // Note: the weights are optional. You do not need to have different weights 
            // for the different observations you would like to fit, but we will use them 
            // as an example to show that it is also possible to specify them if we would 
            // like to, but we could also set them to null in case we do not need them:
            double[] weights = { 0.25, 0.25, 0.50 }; // could also be null

            // Now, we can finally fit the distribution to the observations that we have:
            dist.Fit(observations, weights); // changes 'dist' to become the dist we need

            // Now we can verify that the distribution has been updated:
            double mean = dist.Mean;                // should be 0.79
            double var = dist.Variance;             // should be 0.82352
            double stdDev = dist.StandardDeviation; // should be 0.90748002732842559
            #endregion

            Assert.AreEqual(0.79, mean);
            Assert.AreEqual(0.82352, var);
            Assert.AreEqual(0.90748002732842559, stdDev);
        }

        [Test]
        public void FitTest6()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };

            target.Fit(observations);

            double expected = 0.88;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest5()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };
            double[] weights = { 0.25, 0.25, 0.50 };
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            double expected = 0.79;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest4()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };
            IFittingOptions options = null;

            target.Fit(observations, options);

            double expected = 0.88;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest3()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };
            double[] weights = { 0.25, 0.25, 0.50 };

            target.Fit(observations, weights);

            double expected = 0.79;
            double actual = (target as NormalDistribution).Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest2()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };

            target.Fit(observations);

            double expected = 0.88;
            double actual = (target as NormalDistribution).Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest1()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };
            double[] weights = { 0.25, 0.25, 0.50 };
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            double expected = 0.79;
            double actual = (target as NormalDistribution).Mean;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };
            IFittingOptions options = null;

            target.Fit(observations, options);

            double expected = 0.88;
            double actual = (target as NormalDistribution).Mean;

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void GenerateTest()
        {
            UniformContinuousDistribution target = new UniformContinuousDistribution(2, 5);

            double[] samples = target.Generate(1000000);

            var actual = UniformContinuousDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Minimum, 1e-4);
            Assert.AreEqual(5, actual.Maximum, 1e-4);
        }

        [Test]
        public void GenerateTest2()
        {
            UniformContinuousDistribution target = new UniformContinuousDistribution(-1, 4);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = UniformContinuousDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(-1, actual.Minimum, 1e-4);
            Assert.AreEqual(4, actual.Maximum, 1e-4);
        }

        [Test]
        public void GetRangeTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            var range1 = target.GetRange(0.95);
            var range2 = target.GetRange(0.99);
            var range3 = target.GetRange(0.01);

            Assert.AreEqual(19.629053173483637, range1.Min);
            Assert.AreEqual(26.370946826516363, range1.Max);
            Assert.AreEqual(18.232405574041742, range2.Min);
            Assert.AreEqual(27.767594425958258, range2.Max);
            Assert.AreEqual(18.232405574041742, range3.Min);
            Assert.AreEqual(27.767594425958258, range3.Max);
        }

        [Test]
        public void GetRangeTest_RangeException()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            bool thrown = false;
            try { target.GetRange(0 - Double.Epsilon); }
            catch (ArgumentOutOfRangeException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { target.GetRange(1.0 + 2 * Constants.DoubleEpsilon); }
            catch (ArgumentOutOfRangeException) { thrown = true; }
            Assert.IsTrue(thrown);
        }

        [Test]
        public void GetRangeTest2()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            var range2 = target.GetRange(0.0 + Double.Epsilon);
            Assert.AreEqual(-55.834722290615161, range2.Min);
            Assert.AreEqual(Double.PositiveInfinity, range2.Max);

            var range3 = target.GetRange(1.0 - Constants.DoubleEpsilon);
            Assert.AreEqual(-55.834722290615161, range2.Min);
            Assert.AreEqual(Double.PositiveInfinity, range2.Max);
        }
    }
}
