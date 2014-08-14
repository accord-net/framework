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
    using System.Globalization;

    [TestClass()]
    public class UnivariateDiscreteDistributionTest
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
            // Create an uniform (discrete) distribution in [2, 6] 
            var dist = new UniformDiscreteDistribution(a: 2, b: 6);

            // Common measures
            double mean = dist.Mean;     // 4.0
            double median = dist.Median; // 4.0
            double var = dist.Variance;  // 1.3333333333333333

            // Cumulative distribution functions
            double cdf = dist.DistributionFunction(k: 2);               // 0.2
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.8

            // Probability mass functions
            double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.2
            double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.2
            double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.2
            double lpmf = dist.LogProbabilityMassFunction(k: 2); // -1.6094379124341003

            // Quantile function
            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 2
            int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
            int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 6

            // Hazard (failure rate) functions
            double hf = dist.HazardFunction(x: 4); // 0.5
            double chf = dist.CumulativeHazardFunction(x: 4); // 0.916290731874155

            // String representation
            string str = dist.ToString(CultureInfo.InvariantCulture); // "U(x; a = 2, b = 6)"

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(1.3333333333333333, var);
            Assert.AreEqual(0.916290731874155, chf, 1e-10);
            Assert.AreEqual(0.2, cdf);
            Assert.AreEqual(0.2, pmf1);
            Assert.AreEqual(0.2, pmf2);
            Assert.AreEqual(0.2, pmf3);
            Assert.AreEqual(-1.6094379124341003, lpmf);
            Assert.AreEqual(0.5, hf);
            Assert.AreEqual(0.8, ccdf);
            Assert.AreEqual(2, icdf1);
            Assert.AreEqual(4, icdf2);
            Assert.AreEqual(6, icdf3);
            Assert.AreEqual("U(x; a = 2, b = 6)", str);
        }

        internal virtual UnivariateDiscreteDistribution CreateUnivariateDiscreteDistribution()
        {
            double mean = 0.42;
            return new BernoulliDistribution(mean);
        }

        [TestMethod()]
        public void VarianceTest()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double actual = target.Variance;
            double expected = 0.42 * (1.0 - 0.42);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void StandardDeviationTest()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double actual = target.StandardDeviation;
            double expected = System.Math.Sqrt(0.42 * (1.0 - 0.42));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void MeanTest()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double actual = target.Mean;
            double expected = 0.42;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EntropyTest()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();

            double q = 0.42;
            double p = 1 - q;

            double actual = target.Entropy;
            double expected = -q * System.Math.Log(q) - p * System.Math.Log(p);

            Assert.AreEqual(expected, actual);


            target.Fit(new double[] { 0, 1, 0, 0, 1, 0 });

            q = target.Mean;
            p = 1 - q;

            actual = target.Entropy;
            expected = -q * System.Math.Log(q) - p * System.Math.Log(p);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();

            double p = 0.42;
            double q = 1 - p;

            Assert.AreEqual(q, target.ProbabilityMassFunction(0));
            Assert.AreEqual(p, target.ProbabilityMassFunction(1));

            double[] observations = { 0, 1, 0, 0, 1, 0 };

            target.Fit(observations);

            p = target.Mean;
            q = 1 - p;

            Assert.AreEqual(q, target.ProbabilityMassFunction(0));
            Assert.AreEqual(p, target.ProbabilityMassFunction(1));
        }

        [TestMethod()]
        public void LogProbabilityMassFunctionTest()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();

            double p = 0.42;
            double q = 1 - p;

            double lnp = System.Math.Log(p);
            double lnq = System.Math.Log(q);

            Assert.AreEqual(lnq, target.LogProbabilityMassFunction(0));
            Assert.AreEqual(lnp, target.LogProbabilityMassFunction(1));
        }

        [TestMethod()]
        public void FitTest7()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            double[] weights = { 0.125, 0.125, 0.25, 0.25, 0.25 };
            target.Fit(observations, weights);

            double mean = Accord.Statistics.Tools.WeightedMean(observations, weights);

            Assert.AreEqual(mean, target.Mean);
        }

        [TestMethod()]
        public void FitTest6()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            target.Fit(observations);

            double mean = Accord.Statistics.Tools.Mean(observations);

            Assert.AreEqual(mean, target.Mean);
        }

        [TestMethod()]
        public void FitTest5()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            double[] weights = { 0.125, 0.125, 0.25, 0.25, 0.25 };
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            double mean = Accord.Statistics.Tools.WeightedMean(observations, weights);

            Assert.AreEqual(mean, target.Mean);
        }

        [TestMethod()]
        public void FitTest4()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            IFittingOptions options = null;

            target.Fit(observations, options);

            double mean = Accord.Statistics.Tools.Mean(observations);

            Assert.AreEqual(mean, target.Mean);
        }

        [TestMethod()]
        public void DistributionFunctionTest1()
        {
            UnivariateDiscreteDistribution target = CreateUnivariateDiscreteDistribution();

            double q = 1.0 - 0.42;

            Assert.AreEqual(0, target.DistributionFunction(-1));
            Assert.AreEqual(q, target.DistributionFunction(+0));
            Assert.AreEqual(1, target.DistributionFunction(+1));
            Assert.AreEqual(1, target.DistributionFunction(+2));
        }



        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void ProbabilityFunctionTest()
        {
            IDistribution target = CreateUnivariateDiscreteDistribution();

            double p = 0.42;
            double q = 1 - p;

            Assert.AreEqual(q, target.ProbabilityFunction(0));
            Assert.AreEqual(p, target.ProbabilityFunction(1));


            double[] observations = { 0, 1, 0, 0, 1, 0 };

            target.Fit(observations);

            p = Accord.Statistics.Tools.Mean(observations);
            q = 1 - p;

            Assert.AreEqual(q, target.ProbabilityFunction(0));
            Assert.AreEqual(p, target.ProbabilityFunction(1));
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest3()
        {
            IDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            target.Fit(observations);

            double mean = Accord.Statistics.Tools.Mean(observations);

            Assert.AreEqual(mean, (target as BernoulliDistribution).Mean);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest2()
        {
            IDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            target.Fit(observations);

            double mean = Accord.Statistics.Tools.Mean(observations);

            Assert.AreEqual(mean, (target as BernoulliDistribution).Mean);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest1()
        {
            IDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            double[] weights = { 0.125, 0.125, 0.25, 0.25, 0.25 };
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            double mean = Accord.Statistics.Tools.WeightedMean(observations, weights);

            Assert.AreEqual(mean, (target as BernoulliDistribution).Mean);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void FitTest()
        {
            IDistribution target = CreateUnivariateDiscreteDistribution();
            double[] observations = { 0, 1, 1, 1, 1 };
            IFittingOptions options = null;

            target.Fit(observations, options);

            double mean = Accord.Statistics.Tools.Mean(observations);

            Assert.AreEqual(mean, (target as BernoulliDistribution).Mean);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void DistributionFunctionTest()
        {
            IDistribution target = CreateUnivariateDiscreteDistribution();

            double q = 1.0 - 0.42;

            Assert.AreEqual(0, target.DistributionFunction(-1.0));
            Assert.AreEqual(q, target.DistributionFunction(+0.0));
            Assert.AreEqual(q, target.DistributionFunction(+0.5));
            Assert.AreEqual(1, target.DistributionFunction(+1.0));
            Assert.AreEqual(1, target.DistributionFunction(+1.1));
        }
    }
}
