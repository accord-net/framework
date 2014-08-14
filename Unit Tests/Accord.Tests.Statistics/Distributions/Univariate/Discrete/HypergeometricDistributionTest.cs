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
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;

    [TestClass()]
    public class HypergeometricDistributionTest
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

            int populationSize = 15; // population size N
            int success = 7;         // number of successes in the sample  
            int samples = 8;         // number of samples drawn from N

            // Create a new Hypergeometric distribution with N = 15, n = 8, and s = 7
            var dist = new HypergeometricDistribution(populationSize, success, samples);

            double mean = dist.Mean;     // 1.3809523809523812
            double median = dist.Median; // 4.0
            double var = dist.Variance;  // 3.2879818594104315
            double mode = dist.Mode;     // 4.0

            double cdf = dist.DistributionFunction(k: 2);               // 0.80488799999999994
            double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.19511200000000006

            double pdf1 = dist.ProbabilityMassFunction(k: 4); // 0.38073038073038074
            double pdf2 = dist.ProbabilityMassFunction(k: 5); // 0.18275058275058276
            double pdf3 = dist.ProbabilityMassFunction(k: 6); // 0.030458430458430458

            double lpdf = dist.LogProbabilityMassFunction(k: 2); // -2.3927801721315989

            int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 4
            int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
            int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 5
            int icdf4 = dist.InverseDistributionFunction(p: 0.50);

            double hf = dist.HazardFunction(x: 4); // 1.7753623188405792
            double chf = dist.CumulativeHazardFunction(x: 4); // 1.5396683418789763

            string str = dist.ToString(CultureInfo.InvariantCulture); // "HyperGeometric(x; N = 15, m = 7, n = 8)"

            Assert.AreEqual(3.7333333333333334, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(0.99555555555555553, var);
            Assert.AreEqual(4, mode);
            Assert.AreEqual(1.5396683418789763, chf, 1e-10);
            Assert.AreEqual(0.10023310023310024, cdf);
            Assert.AreEqual(0.38073038073038074, pdf1);
            Assert.AreEqual(0.18275058275058276, pdf2);
            Assert.AreEqual(0.030458430458430458, pdf3);
            Assert.AreEqual(-2.3927801721315989, lpdf);
            Assert.AreEqual(1.7753623188405792, hf);
            Assert.AreEqual(0.89976689976689972, ccdf);
            Assert.AreEqual(3, icdf1);
            Assert.AreEqual(4, icdf2);
            Assert.AreEqual(5, icdf3);
            Assert.AreEqual("HyperGeometric(x; N = 15, m = 7, n = 8)", str);
        }

        [TestMethod()]
        public void HypergeometricDistributionConstructorTest()
        {
            bool thrown;

            thrown = false;
            try { new HypergeometricDistribution(10, 0, 11); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(10, 11, 9); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(0, 0, 1); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(1, 0, 0); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(1, -1, 1); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            int N = 10;
            int n = 8;
            int m = 9;

            var target = new HypergeometricDistribution(N, m, n);
            Assert.AreEqual(N, target.PopulationSize);
            Assert.AreEqual(n, target.SampleSize);
            Assert.AreEqual(m, target.PopulationSuccess);

            double dN = N;
            double dn = n;
            double dm = m;

            Assert.AreEqual(dn * (dm / dN), target.Mean);
            Assert.AreEqual(dn * dm * (dN - dm) * (dN - dn) / (dN * dN * (dN - 1.0)), target.Variance);
        }

        [TestMethod()]
        public void CloneTest()
        {
            int populationSize = 12;
            int draws = 5;
            int success = 4;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            var actual = (HypergeometricDistribution)target.Clone();

            Assert.AreNotSame(target, actual);
            Assert.AreNotEqual(target, actual);

            Assert.AreEqual(target.PopulationSize, actual.PopulationSize);
            Assert.AreEqual(target.SampleSize, actual.SampleSize);
            Assert.AreEqual(target.PopulationSuccess, actual.PopulationSuccess);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            int k = 5;
            double expected = 0.96829836829836835;
            double actual = target.DistributionFunction(k);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void MedianTest()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void LogProbabilityMassFunctionTest()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            int k = 5;
            double expected = Math.Log(0.182750582750583);
            double actual = target.LogProbabilityMassFunction(k);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest()
        {
            int N = 50;
            int n = 10;
            int m = 5;
            var target = new HypergeometricDistribution(N, m, n);

            int k = 4;
            double expected = 0.0039645830580150657;
            double actual = target.ProbabilityMassFunction(k);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest2()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            int k = 5;
            double expected = 0.182750582750583;
            double actual = target.ProbabilityMassFunction(k);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void DistributionFunctionTest2()
        {
            // Verified against http://stattrek.com/online-calculator/hypergeometric.aspx

            int population = 20;
            int populationSuccess = 8;
            int sample = 6;

            double[] pmf = { 0.0238390092879257, 0.163467492260062, 0.357585139318886, 0.317853457172343, 0.119195046439628, 0.0173374613003096, 0.000722394220846233 };
            double[] less = { 0.0000000000000000, 0.023839009287926, 0.187306501547988, 0.544891640866874, 0.862745098039217, 0.981940144478844, 0.999277605779154 };
            double[] lessEqual = { 0.0238390092879257, 0.187306501547988, 0.544891640866874, 0.862745098039217, 0.981940144478845, 0.999277605779154, 1 };
            double[] greater = { 0.976160990712074, 0.812693498452012, 0.455108359133126, 0.137254901960783, 0.018059855521155, 0.000722394220845968, 0 };
            double[] greaterEqual = { 1, 0.976160990712074, 0.812693498452012, 0.455108359133126, 0.137254901960783, 0.0180598555211555, 0.00072239422084619 };

            var target = new HypergeometricDistribution(population, populationSuccess, sample);

            for (int i = 0; i < pmf.Length; i++)
            {
                {   // P(X = i)
                    double actual = target.ProbabilityMassFunction(i);
                    Assert.AreEqual(pmf[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X <= i)
                    double actual = target.DistributionFunction(i);
                    Assert.AreEqual(lessEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X < i)
                    double actual = target.DistributionFunction(i, inclusive: false);
                    Assert.AreEqual(less[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X > i)
                    double actual = target.ComplementaryDistributionFunction(i);
                    Assert.AreEqual(greater[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X >= i)
                    double actual = target.ComplementaryDistributionFunction(i, inclusive: true);
                    Assert.AreEqual(greaterEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }
            }

        }

    }
}
