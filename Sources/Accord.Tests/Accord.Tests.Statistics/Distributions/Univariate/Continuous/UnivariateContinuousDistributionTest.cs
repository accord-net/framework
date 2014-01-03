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
    using Accord.Statistics.Distributions;
    using System;
    using Accord.Statistics.Distributions.Fitting;

    [TestClass()]
    public class UnivariateContinuousDistributionTest
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


        internal virtual UnivariateContinuousDistribution CreateUnivariateContinuousDistribution()
        {
            double mean = 23;
            double var = 4.2;
            double dev = System.Math.Sqrt(var);
            return new NormalDistribution(mean, dev);
        }



        [TestMethod()]
        public void VarianceTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.Variance;
            double expected = 4.2;

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }

        [TestMethod()]
        public void StandardDeviationTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.StandardDeviation;
            double expected = System.Math.Sqrt(4.2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void MeanTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.Mean;
            double expected = 23;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EntropyTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double actual = target.Entropy;
            double expected = 0.5 * System.Math.Log(2 * System.Math.PI * System.Math.E * 4.2);

            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.011697993604177;
            double actual = target.ProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = System.Math.Log(0.011697993604177);
            double actual = target.LogProbabilityDensityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void ProbabilityFunctionTest()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.011697993604177;
            double actual = target.ProbabilityFunction(x);

            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-10);
        }



        [TestMethod()]
        public void DistributionFunctionTest1()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.00885952932929092;
            double actual = target.DistributionFunction(x);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void DistributionFunctionTest()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();
            double x = 18.14;
            double expected = 0.00885952932929092;
            double actual = target.DistributionFunction(x);

            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void FitTest7()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };
            double[] weights = { 0.25, 0.25, 0.50 };

            target.Fit(observations, weights);

            double expected = 0.79;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FitTest6()
        {
            UnivariateContinuousDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };

            target.Fit(observations);

            double expected = 0.88;
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void FitTest2()
        {
            IDistribution target = CreateUnivariateContinuousDistribution();

            double[] observations = { 0.12, 2, 0.52 };

            target.Fit(observations);

            double expected = 0.88;
            double actual = (target as NormalDistribution).Mean;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
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

        [TestMethod()]
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


        [TestMethod()]
        public void GenerateTest()
        {
            UniformContinuousDistribution target = new UniformContinuousDistribution(2, 5);

            double[] samples = target.Generate(1000000);

            var actual = UniformContinuousDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Minimum, 1e-4);
            Assert.AreEqual(5, actual.Maximum, 1e-4);
        }

        [TestMethod()]
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
    }
}
