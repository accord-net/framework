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
    using System;

    [TestClass()]
    public class BetaDistributionTest
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
        public void BetaDistributionConstructorTest()
        {
            double expected, actual;

            {
                BetaDistribution target = new BetaDistribution(1.73, 4.2);
                actual = target.ProbabilityDensityFunction(-1);
                expected = 0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0);
                expected = 0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(1);
                expected = 0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0.2);
                expected = 2.27095841;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0.4);
                expected = 1.50022749;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void MedianTest()
        {
            double alpha = 0.42;
            double beta = 1.57;

            BetaDistribution target = new BetaDistribution(alpha, beta);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void BetaMeanTest()
        {
            double alpha = 0.42;
            double beta = 1.57;

            BetaDistribution betaDistribution = new BetaDistribution(alpha, beta);

            double mean = betaDistribution.Mean; // 0.21105527638190955
            double median = betaDistribution.Median; // 0.11577706212908731
            double var = betaDistribution.Variance; // 0.055689279830523512
            double chf = betaDistribution.CumulativeHazardFunction(x: 0.27); // 1.1828193992944409
            double cdf = betaDistribution.DistributionFunction(x: 0.27); // 0.69358638272337991
            double pdf = betaDistribution.ProbabilityDensityFunction(x: 0.27); // 0.94644031936694828
            double lpdf = betaDistribution.LogProbabilityDensityFunction(x: 0.27); // -0.055047364344046057
            double hf = betaDistribution.HazardFunction(x: 0.27); // 3.0887671630877072
            double ccdf = betaDistribution.ComplementaryDistributionFunction(x: 0.27); // 0.30641361727662009
            double icdf = betaDistribution.InverseDistributionFunction(p: cdf); // 0.26999999068687469

            string str = betaDistribution.ToString(System.Globalization.CultureInfo.InvariantCulture); // "B(x; α = 0.42, β = 1.57)

            Assert.AreEqual(0.21105527638190955, mean);
            Assert.AreEqual(0.11577706212908731, median);
            Assert.AreEqual(0.055689279830523512, var);
            Assert.AreEqual(1.1828193992944409, chf);
            Assert.AreEqual(0.69358638272337991, cdf);
            Assert.AreEqual(0.94644031936694828, pdf);
            Assert.AreEqual(-0.055047364344046057, lpdf);
            Assert.AreEqual(3.0887671630877072, hf);
            Assert.AreEqual(0.30641361727662009, ccdf);
            Assert.AreEqual(0.27, icdf, 1e-10);
            Assert.AreEqual("B(x; α = 0.42, β = 1.57)", str);
        }

        [TestMethod()]
        public void BetaMeanTest2()
        {
            int trials = 161750;
            int successes = 10007;

            BetaDistribution betaDistribution = new BetaDistribution(successes, trials);

            double mean = betaDistribution.Mean; // 0.06187249616697166
            double median = betaDistribution.Median; // 0.06187069085946604
            double p025 = betaDistribution.InverseDistributionFunction(p: 0.025); // 0.06070354581334864
            double p975 = betaDistribution.InverseDistributionFunction(p: 0.975); // 0.0630517079399996

            string str = betaDistribution.ToString();

            Assert.AreEqual(0.06187249616697166, mean);
            Assert.AreEqual(0.06187069085946604, median, 1e-6);
            Assert.AreEqual(0.06070354581334864, p025, 1e-6);
            Assert.AreEqual(0.0630517079399996, p975, 1e-6);
            Assert.AreEqual("B(x; α = 10008, β = 151744)", str);
        }

        [TestMethod()]
        public void BetaMeanTest3()
        {
            int trials = 100;
            int successes = 78;

            BetaDistribution betaDistribution = new BetaDistribution(successes, trials);

            double mean = betaDistribution.Mean; // 0.77450980392156865
            double median = betaDistribution.Median; // 0.77630912598534851
            double p025 = betaDistribution.InverseDistributionFunction(p: 0.025); // 0.68899653915764347
            double p975 = betaDistribution.InverseDistributionFunction(p: 0.975); // 0.84983461640764513

            double orig025 = betaDistribution.DistributionFunction(p025);
            double orig975 = betaDistribution.DistributionFunction(p975);

            Assert.AreEqual(0.025, orig025, 1e-8);
            Assert.AreEqual(0.975, orig975, 1e-8);
            Assert.AreEqual(0.77450980392156865, mean, 1e-9);
            Assert.AreEqual(0.7763091275412235, median, 1e-9);
            Assert.AreEqual(0.68899667463246894, p025, 1e-6);
            Assert.AreEqual(0.84983461640764513, p975, 1e-6);
        }

    }
}
