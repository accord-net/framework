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
    using Accord.Statistics.Moving;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;
    using System.Globalization;

    [TestClass()]
    public class NakagamiDistributionTest
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
            NakagamiDistribution n = new NakagamiDistribution(0.807602, 12.5);
            Assert.AreEqual(3.0510602824983368, n.Mean);
            Assert.AreEqual(3.1910311525611705, n.Variance);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            var nakagami = new NakagamiDistribution(shape: 2.4, spread: 4.2);

            double mean = nakagami.Mean;     // 1.946082119049118
            double median = nakagami.Median; // 1.9061151110206338
            double var = nakagami.Variance;  // 0.41276438591729486

            double cdf = nakagami.DistributionFunction(x: 1.4); // 0.20603416752368109
            double pdf = nakagami.ProbabilityDensityFunction(x: 1.4); // 0.49253215371343023
            double lpdf = nakagami.LogProbabilityDensityFunction(x: 1.4); // -0.708195533773302

            double ccdf = nakagami.ComplementaryDistributionFunction(x: 1.4); // 0.79396583247631891
            double icdf = nakagami.InverseDistributionFunction(p: cdf); // 1.400000000131993

            double hf = nakagami.HazardFunction(x: 1.4); // 0.62034426869133652
            double chf = nakagami.CumulativeHazardFunction(x: 1.4); // 0.23071485080660473

            string str = nakagami.ToString(CultureInfo.InvariantCulture); // Nakagami(x; μ = 2,4, ω = 4,2)"

            Assert.AreEqual(1.946082119049118, mean);
            Assert.AreEqual(1.9061151110206338, median, 1e-6);
            Assert.AreEqual(0.41276438591729486, var);
            Assert.AreEqual(0.23071485080660473, chf);
            Assert.AreEqual(0.20603416752368109, cdf);
            Assert.AreEqual(0.49253215371343023, pdf);
            Assert.AreEqual(-0.708195533773302, lpdf);
            Assert.AreEqual(0.62034426869133652, hf);
            Assert.AreEqual(0.79396583247631891, ccdf);
            Assert.AreEqual(1.40, icdf, 1e-7);
            Assert.AreEqual("Nakagami(x; μ = 2.4, ω = 4.2)", str);
        }

        [TestMethod()]
        public void ProbabilityDistributionTest()
        {
            NakagamiDistribution n = new NakagamiDistribution(0.807602, 12.5);

            double[] expected = { 0, 0.1775314, 0.224023, 0.2081279, 0.158044, 0.101360 };
            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.ProbabilityDensityFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [TestMethod()]
        public void CumulativeDistributionTest()
        {
            NakagamiDistribution n = new NakagamiDistribution(0.807602, 12.5);

            double[] expected = { 0, 0.1139332, 0.3209643, 0.541133, 0.7258253, 0.8551455 };
            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.DistributionFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-6);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }


        [TestMethod()]
        public void GenerateTest()
        {
            NakagamiDistribution target = new NakagamiDistribution(2, 5);

            double[] samples = target.Generate(1000000);

            var actual = NakagamiDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Shape, 0.01);
            Assert.AreEqual(5, actual.Spread, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            NakagamiDistribution target = new NakagamiDistribution(4, 2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = NakagamiDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Shape, 0.01);
            Assert.AreEqual(2, actual.Spread, 0.01);
        }

        [TestMethod()]
        public void MedianTest()
        {
            NakagamiDistribution target = new NakagamiDistribution(5.42, 1.37);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}