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
    using Accord.Statistics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestClass()]
    [Ignore]
    public class GeneralContinuousDistributionTest
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
        public void ConstructorTest1()
        {
            var original = new NormalDistribution(mean: 4, stdDev: 4.2);

            var normal = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testNormal(normal);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            var original =  new NormalDistribution(mean: 4, stdDev: 4.2);

            var normal = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testNormal(normal);
        }

        private static void testNormal(GeneralContinuousDistribution normal)
        {
            double mean = normal.Mean;     // 4.0
            double median = normal.Median; // 4.0
            double var = normal.Variance;  // 17.64

            double cdf = normal.DistributionFunction(x: 1.4); // 0.26794249453351904
            double pdf = normal.ProbabilityDensityFunction(x: 1.4); // 0.078423391448155175
            double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -2.5456330358182586

            double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.732057505466481
            double icdf = normal.InverseDistributionFunction(p: cdf); // 1.4

            double hf = normal.HazardFunction(x: 1.4); // 0.10712736480747137
            double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.31189620872601354

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(17.64, var);
            Assert.AreEqual(0.31189620872601354, chf);
            Assert.AreEqual(0.26794249453351904, cdf);
            Assert.AreEqual(0.078423391448155175, pdf);
            Assert.AreEqual(-2.5456330358182586, lpdf);
            Assert.AreEqual(0.10712736480747137, hf);
            Assert.AreEqual(0.732057505466481, ccdf);
            Assert.AreEqual(1.4, icdf);
        }

        [TestMethod()]
        public void ConstructorTest3()
        {
            var original = new InverseGaussianDistribution(mean: 0.42, shape: 1.2);

            var invGaussian = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testInvGaussian(invGaussian);
        }

        [TestMethod()]
        public void ConstructorTest4()
        {
            var original = new InverseGaussianDistribution(mean: 0.42, shape: 1.2);

            var invGaussian = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testInvGaussian(invGaussian);
        }

        private static void testInvGaussian(GeneralContinuousDistribution invGaussian)
        {
            double mean = invGaussian.Mean;     // 0.42
            double median = invGaussian.Median;   // 0.35856861093990083
            double var = invGaussian.Variance; // 0.061739999999999989

            double cdf = invGaussian.DistributionFunction(x: 0.27); // 0.30658791274125458
            double pdf = invGaussian.ProbabilityDensityFunction(x: 0.27); // 2.3461495925760354
            double lpdf = invGaussian.LogProbabilityDensityFunction(x: 0.27); // 0.85277551314980737

            double ccdf = invGaussian.ComplementaryDistributionFunction(x: 0.27); // 0.69341208725874548
            double icdf = invGaussian.InverseDistributionFunction(p: cdf); // 0.26999999957543408

            double hf = invGaussian.HazardFunction(x: 0.27); // 3.383485283406336
            double chf = invGaussian.CumulativeHazardFunction(x: 0.27); // 0.36613081401302111


            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.35856861093990083, median, 1e-7);
            Assert.AreEqual(0.061739999999999989, var);
            Assert.AreEqual(0.36613081401302111, chf);
            Assert.AreEqual(0.30658791274125458, cdf);
            Assert.AreEqual(2.3461495925760354, pdf);
            Assert.AreEqual(0.85277551314980737, lpdf);
            Assert.AreEqual(3.383485283406336, hf);
            Assert.AreEqual(0.69341208725874548, ccdf);
            Assert.AreEqual(0.26999999957543408, icdf, 1e-8);
        }

        [TestMethod()]
        public void ConstructorTest6()
        {
            var original = new LaplaceDistribution(location: 4, scale: 2);

            var laplace = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testLaplace(laplace);
        }

        [TestMethod()]
        public void ConstructorTest7()
        {
            var original = new LaplaceDistribution(location: 4, scale: 2);

            var laplace = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testLaplace(laplace);
        }

        private static void testLaplace(GeneralContinuousDistribution laplace)
        {
            double mean = laplace.Mean;     // 4.0
            double median = laplace.Median; // 4.0
            double var = laplace.Variance;  // 8.0

            double cdf = laplace.DistributionFunction(x: 0.27); // 0.077448104942453522
            double pdf = laplace.ProbabilityDensityFunction(x: 0.27); // 0.038724052471226761
            double lpdf = laplace.LogProbabilityDensityFunction(x: 0.27); // -3.2512943611198906

            double ccdf = laplace.ComplementaryDistributionFunction(x: 0.27); // 0.92255189505754642
            double icdf = laplace.InverseDistributionFunction(p: cdf); // 0.27

            double hf = laplace.HazardFunction(x: 0.27); // 0.041974931360160776
            double chf = laplace.CumulativeHazardFunction(x: 0.27); // 0.080611649844768624

            Assert.AreEqual(4.0, mean);
            Assert.AreEqual(4.0, median);
            Assert.AreEqual(8.0, var);
            Assert.AreEqual(0.080611649844768624, chf);
            Assert.AreEqual(0.077448104942453522, cdf);
            Assert.AreEqual(0.038724052471226761, pdf);
            Assert.AreEqual(-3.2512943611198906, lpdf);
            Assert.AreEqual(0.041974931360160776, hf);
            Assert.AreEqual(0.92255189505754642, ccdf);
            Assert.AreEqual(0.26999999840794775, icdf);
        }

        [TestMethod()]
        public void MedianTest()
        {
            var laplace = new LaplaceDistribution(location: 2, scale: 0.42);

            var target = GeneralContinuousDistribution.FromDensityFunction(
                laplace.Support, laplace.ProbabilityDensityFunction);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            Assert.AreEqual(laplace.Median, target.Median);

            target = GeneralContinuousDistribution.FromDistributionFunction(
                laplace.Support, laplace.DistributionFunction);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            Assert.AreEqual(laplace.Median, target.Median);
        }

        [TestMethod()]
        public void ConstructorTest8()
        {
            var original = new LognormalDistribution(location: 0.42, shape: 1.1);

            var log = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testLognormal(log);
        }

        [TestMethod()]
        public void ConstructorTest9()
        {
            var original = new LognormalDistribution(location: 0.42, shape: 1.1);

            var log = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testLognormal(log);
        }

        private static void testLognormal(GeneralContinuousDistribution log)
        {
            double mean = log.Mean;     // 2.7870954605658511
            double median = log.Median; // 1.5219615583481305
            double var = log.Variance;  // 18.28163603621158

            double cdf = log.DistributionFunction(x: 0.27); // 0.057961222885664958
            double pdf = log.ProbabilityDensityFunction(x: 0.27); // 0.39035530085982068
            double lpdf = log.LogProbabilityDensityFunction(x: 0.27); // -0.94069792674674835

            double ccdf = log.ComplementaryDistributionFunction(x: 0.27); // 0.942038777114335
            double icdf = log.InverseDistributionFunction(p: cdf); // 0.26999997937815973

            double hf = log.HazardFunction(x: 0.27); // 0.41437285846720867
            double chf = log.CumulativeHazardFunction(x: 0.27); // 0.059708840588116374


            Assert.AreEqual(2.7870954605658511, mean);
            Assert.AreEqual(1.5219615583481305, median, 1e-7);
            Assert.AreEqual(18.28163603621158, var);
            Assert.AreEqual(0.059708840588116374, chf);
            Assert.AreEqual(0.057961222885664958, cdf);
            Assert.AreEqual(0.39035530085982068, pdf);
            Assert.AreEqual(-0.94069792674674835, lpdf);
            Assert.AreEqual(0.41437285846720867, hf);
            Assert.AreEqual(0.942038777114335, ccdf);
            Assert.AreEqual(0.26999997937815973, icdf, 1e-7);
        }

        [TestMethod()]
        public void ConstructorTest10()
        {
            var original = new ChiSquareDistribution(degreesOfFreedom: 7);

            var chisq = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testChiSquare(chisq);
        }

        [TestMethod()]
        public void ConstructorTest11()
        {
            var original = new ChiSquareDistribution(degreesOfFreedom: 7);

            var chisq = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testChiSquare(chisq);
        }

        private static void testChiSquare(GeneralContinuousDistribution chisq)
        {
            double mean = chisq.Mean;     // 7
            double median = chisq.Median; // 6.345811195595612
            double var = chisq.Variance;  // 14

            double cdf = chisq.DistributionFunction(x: 6.27); // 0.49139966433823956
            double pdf = chisq.ProbabilityDensityFunction(x: 6.27); // 0.11388708001184455
            double lpdf = chisq.LogProbabilityDensityFunction(x: 6.27); // -2.1725478476948092

            double ccdf = chisq.ComplementaryDistributionFunction(x: 6.27); // 0.50860033566176044
            double icdf = chisq.InverseDistributionFunction(p: cdf); // 6.2700000000852318

            double hf = chisq.HazardFunction(x: 6.27); // 0.22392254197721179
            double chf = chisq.CumulativeHazardFunction(x: 6.27); // 0.67609276602233315

            Assert.AreEqual(7, mean);
            Assert.AreEqual(6.345811195595612, median, 1e-6);
            Assert.AreEqual(14, var);
            Assert.AreEqual(0.67609276602233315, chf);
            Assert.AreEqual(0.49139966433823956, cdf);
            Assert.AreEqual(0.11388708001184455, pdf);
            Assert.AreEqual(-2.1725478476948092, lpdf);
            Assert.AreEqual(0.22392254197721179, hf);
            Assert.AreEqual(0.50860033566176044, ccdf);
            Assert.AreEqual(6.2700000000852318, icdf, 1e-6);
        }

        [TestMethod()]
        public void ConstructorTest12()
        {
            var original = new GompertzDistribution(eta: 4.2, b: 1.1);

            var gompertz = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testGompertz(gompertz);
        }

        [TestMethod()]
        public void ConstructorTest13()
        {
            var original = new GompertzDistribution(eta: 4.2, b: 1.1);

            var gompertz = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testGompertz(gompertz);
        }

        private static void testGompertz(GeneralContinuousDistribution gompertz)
        {
            double median = gompertz.Median; // 0.13886469671401389

            double cdf = gompertz.DistributionFunction(x: 0.27); // 0.76599768199799145
            double pdf = gompertz.ProbabilityDensityFunction(x: 0.27); // 1.4549484164912097
            double lpdf = gompertz.LogProbabilityDensityFunction(x: 0.27); // 0.37497044741163688

            double ccdf = gompertz.ComplementaryDistributionFunction(x: 0.27); // 0.23400231800200855
            double icdf = gompertz.InverseDistributionFunction(p: cdf); // 0.26999999999766749

            double hf = gompertz.HazardFunction(x: 0.27); // 6.2176666834502088
            double chf = gompertz.CumulativeHazardFunction(x: 0.27); // 1.4524242576820101

            Assert.AreEqual(0.13886469671401389, median);
            Assert.AreEqual(1.4524242576820101, chf);
            Assert.AreEqual(0.76599768199799145, cdf);
            Assert.AreEqual(1.4549484164912097, pdf);
            Assert.AreEqual(0.37497044741163688, lpdf);
            Assert.AreEqual(6.2176666834502088, hf);
            Assert.AreEqual(0.23400231800200855, ccdf);
            Assert.AreEqual(0.26999999999766749, icdf);
        }

        [TestMethod()]
        public void ConstructorTest14()
        {
            var original = new NakagamiDistribution(shape: 2.4, spread: 4.2);

            var nakagami = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testNakagami(nakagami);
        }

        [TestMethod()]
        public void ConstructorTest15()
        {
            var original = new NakagamiDistribution(shape: 2.4, spread: 4.2);

            var nakagami = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testNakagami(nakagami);
        }

        private static void testNakagami(GeneralContinuousDistribution nakagami)
        {
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
        }

        [TestMethod()]
        public void ConstructorTest16()
        {
            var original = new VonMisesDistribution(mean: 0.42, concentration: 1.2);

            var vonMises = GeneralContinuousDistribution.FromDensityFunction(
                original.Support, original.ProbabilityDensityFunction);

            testVonMises(vonMises);
        }

        [TestMethod()]
        public void ConstructorTest17()
        {
            var original = new VonMisesDistribution(mean: 0.42, concentration: 1.2);

            var vonMises = GeneralContinuousDistribution.FromDistributionFunction(
                original.Support, original.DistributionFunction);

            testVonMises(vonMises);
        }

        private static void testVonMises(GeneralContinuousDistribution vonMises)
        {
            double mean = vonMises.Mean;     // 0.42
            double median = vonMises.Median; // 0.42
            double var = vonMises.Variance;  // 0.48721760532782921

            double cdf = vonMises.DistributionFunction(x: 1.4); // 0.81326928491589345
            double pdf = vonMises.ProbabilityDensityFunction(x: 1.4); // 0.2228112141141676
            double lpdf = vonMises.LogProbabilityDensityFunction(x: 1.4); // -1.5014304395467863

            double ccdf = vonMises.ComplementaryDistributionFunction(x: 1.4); // 0.18673071508410655
            double icdf = vonMises.InverseDistributionFunction(p: cdf); // 1.3999999637927665

            double hf = vonMises.HazardFunction(x: 1.4); // 1.1932220899695576
            double chf = vonMises.CumulativeHazardFunction(x: 1.4); // 1.6780877262500649

            double imedian = vonMises.InverseDistributionFunction(p: 0.5);

            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.42, median);
            Assert.AreEqual(0.42000000260613551, imedian, 1e-8);
            Assert.AreEqual(0.48721760532782921, var);
            Assert.AreEqual(1.6780877262500649, chf);
            Assert.AreEqual(0.81326928491589345, cdf);
            Assert.AreEqual(0.2228112141141676, pdf);
            Assert.AreEqual(-1.5014304395467863, lpdf);
            Assert.AreEqual(1.1932220899695576, hf);
            Assert.AreEqual(0.18673071508410655, ccdf);
            Assert.AreEqual(1.39999999999, icdf, 1e-8);
        }

        [TestMethod()]
        public void InverseDistributionFunctionTest()
        {
            double[] expected =
            {
                Double.NegativeInfinity, -4.38252, -2.53481, -1.20248,
                -0.0640578, 1.0, 2.06406, 3.20248, 4.53481, 6.38252,
                Double.PositiveInfinity
            };

            NormalDistribution original = new NormalDistribution(1.0, 4.2);
            var target = GeneralContinuousDistribution.FromDistributionFunction(
               original.Support, original.DistributionFunction);

            for (int i = 0; i < expected.Length; i++)
            {
                double x = i / 10.0;
                double actual = target.InverseDistributionFunction(x);
                Assert.AreEqual(expected[i], actual, 1e-5);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }


        [TestMethod()]
        public void MedianTest2()
        {
            NormalDistribution original = new NormalDistribution(0.4, 2.2);

            var target = GeneralContinuousDistribution.FromDistributionFunction(
               original.Support, original.DistributionFunction);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            Assert.AreEqual(target.Median, original.Median);

            target = GeneralContinuousDistribution.FromDensityFunction(
               original.Support, original.ProbabilityDensityFunction);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
            Assert.AreEqual(target.Median, original.Median);
        }
    }
}
