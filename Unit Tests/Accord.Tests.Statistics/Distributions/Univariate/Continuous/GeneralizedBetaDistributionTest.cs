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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class GeneralizedBetaDistributionTest
    {


        [Test]
        public void BetaDistributionConstructorTest()
        {
            double expected, actual;

            {
                var target = new GeneralizedBetaDistribution(1.73, 4.2);
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

        [Test]
        public void MedianTest()
        {
            double alpha = 0.42;
            double beta = 1.57;

            var target = new GeneralizedBetaDistribution(alpha, beta);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void BetaMeanTest()
        {
            #region doc_create2
            double alpha = 0.42;
            double beta = 1.57;

            var betaDistribution = new GeneralizedBetaDistribution(alpha, beta);

            double mean = betaDistribution.Mean; // 0.21105527638190955
            double median = betaDistribution.Median; // 0.11577706212908731
            double var = betaDistribution.Variance; // 0.055689279830523512
            double mode = betaDistribution.Mode;    // 57.999999999999957

            double chf = betaDistribution.CumulativeHazardFunction(x: 0.27); // 1.1828193992944409
            double cdf = betaDistribution.DistributionFunction(x: 0.27); // 0.69358638272337991
            double pdf = betaDistribution.ProbabilityDensityFunction(x: 0.27); // 0.94644031936694828
            double lpdf = betaDistribution.LogProbabilityDensityFunction(x: 0.27); // -0.055047364344046057
            double hf = betaDistribution.HazardFunction(x: 0.27); // 3.0887671630877072
            double ccdf = betaDistribution.ComplementaryDistributionFunction(x: 0.27); // 0.30641361727662009
            double icdf = betaDistribution.InverseDistributionFunction(p: cdf); // 0.26999999068687469

            string str = betaDistribution.ToString(System.Globalization.CultureInfo.InvariantCulture); // "B(x; α = 0.42, β = 1.57)
            #endregion

            Assert.AreEqual(0.21105527638190955, mean);
            Assert.AreEqual(0.11577706212908731, median);
            Assert.AreEqual(57.999999999999957, mode);
            Assert.AreEqual(0.055689279830523512, var);
            Assert.AreEqual(1.1828193992944409, chf);
            Assert.AreEqual(0.69358638272337991, cdf);
            Assert.AreEqual(0.94644031936694828, pdf);
            Assert.AreEqual(-0.055047364344046057, lpdf);
            Assert.AreEqual(3.0887671630877072, hf);
            Assert.AreEqual(0.30641361727662009, ccdf);
            Assert.AreEqual(0.27, icdf, 1e-10);
            Assert.AreEqual("B(x; α = 0.42, β = 1.57, min = 0, max = 1)", str);

            Assert.IsFalse(Double.IsNaN(median));

            var range1 = betaDistribution.GetRange(0.95);
            var range2 = betaDistribution.GetRange(0.99);
            var range3 = betaDistribution.GetRange(0.01);

            Assert.AreEqual(0.00045925525776717733, range1.Min);
            Assert.AreEqual(0.72381020663218609, range1.Max);
            Assert.AreEqual(0.0000099485893745082635, range2.Min);
            Assert.AreEqual(0.89625688707910811, range2.Max);
            Assert.AreEqual(0.0000099485893745082432, range3.Min);
            Assert.AreEqual(0.89625688707910811, range3.Max);


            Assert.AreEqual(0, betaDistribution.Support.Min);
            Assert.AreEqual(1, betaDistribution.Support.Max);

            Assert.AreEqual(betaDistribution.InverseDistributionFunction(0), betaDistribution.Support.Min);
            Assert.AreEqual(betaDistribution.InverseDistributionFunction(1), betaDistribution.Support.Max);
        }

        [Test]
        public void small_alpha_beta()
        {
            double alpha = 1e-150;
            double beta = 1e-150;

            var betaDistribution = new GeneralizedBetaDistribution(alpha, beta);

            double mean = betaDistribution.Mean;
            Assert.Throws<ArithmeticException>(() => { double median = betaDistribution.Median; });
            double var = betaDistribution.Variance;
            double mode = betaDistribution.Mode;

            double cdf = betaDistribution.DistributionFunction(x: 0.27); 
            double pdf = betaDistribution.ProbabilityDensityFunction(x: 0.27); 
            double lpdf = betaDistribution.LogProbabilityDensityFunction(x: 0.27); 
            double ccdf = betaDistribution.ComplementaryDistributionFunction(x: 0.27); 
            double icdf = betaDistribution.InverseDistributionFunction(p: cdf); 

            Assert.AreEqual(0.5, mean);
            Assert.AreEqual(0.5, mode);
            Assert.AreEqual(0.25, var);
            Assert.AreEqual(0.5, cdf, 1e-5);
            Assert.AreEqual(2.5367833587011923E-150, pdf);
            Assert.AreEqual(-344.45686706484332, lpdf);
            Assert.AreEqual(0.49999999999999989, ccdf);
            Assert.AreEqual(0, icdf, 1e-10);

            Assert.Throws<ArithmeticException>(() => betaDistribution.GetRange(0.95));

            Assert.AreEqual(1, betaDistribution.Support.Length);
            Assert.AreEqual(0, betaDistribution.Support.Min);
            Assert.AreEqual(1, betaDistribution.Support.Max);

            Assert.AreEqual(betaDistribution.InverseDistributionFunction(0), betaDistribution.Support.Min);
            Assert.AreEqual(betaDistribution.InverseDistributionFunction(1), betaDistribution.Support.Max);
        }


        [Test]
        public void NoncentralBetaMeanTest()
        {
            #region doc_create
            // Create a 4-parameter Beta distribution with the following parameters (α, β, a, b):
            var beta = new GeneralizedBetaDistribution(alpha: 1.42, beta: 1.57, min: 1, max: 4.2);

            double mean = beta.Mean;     // 2.5197324414715716
            double median = beta.Median; // 2.4997705845160225
            double var = beta.Variance;  // 0.19999664152943961
            double mode = beta.Mode;     // 2.3575757575757574
            double h = beta.Entropy;     // -0.050654548091478513

            double cdf = beta.DistributionFunction(x: 2.27);           // 0.40828630817664596
            double pdf = beta.ProbabilityDensityFunction(x: 2.27);     // 1.2766172921464953
            double lpdf = beta.LogProbabilityDensityFunction(x: 2.27); // 0.2442138392176838

            double chf = beta.CumulativeHazardFunction(x: 2.27);       // 0.5247323897609667
            double hf = beta.HazardFunction(x: 2.27);                  // 2.1574915534109484

            double ccdf = beta.ComplementaryDistributionFunction(x: 2.27); // 0.59171369182335409
            double icdf = beta.InverseDistributionFunction(p: cdf);        // 2.27

            string str = beta.ToString(System.Globalization.CultureInfo.InvariantCulture); // "B(x; α = 1.42, β = 1.57, min = 1, max = 4.2)"
            #endregion

            Assert.AreEqual(2.5197324414715716, mean);
            Assert.AreEqual(2.4997705845160225, median);
            Assert.AreEqual(2.3575757575757574, mode);
            Assert.AreEqual(-0.050654548091478513, h);
            Assert.AreEqual(0.19999664152943961, var);
            Assert.AreEqual(0.5247323897609667, chf);
            Assert.AreEqual(0.40828630817664596, cdf);
            Assert.AreEqual(0.39894290379577979, pdf);
            Assert.AreEqual(-0.91893697058799706, lpdf);
            Assert.AreEqual(0.67421611044092133, hf);
            Assert.AreEqual(0.59171369182335409, ccdf);
            Assert.AreEqual(2.27, icdf, 1e-10);
            Assert.AreEqual("B(x; α = 1.42, β = 1.57, min = 1, max = 4.2)", str);

            Assert.IsFalse(Double.IsNaN(median));

            var range1 = beta.GetRange(0.95);
            var range2 = beta.GetRange(0.99);
            var range3 = beta.GetRange(0.01);

            Assert.AreEqual(1.2650560572620337, range1.Min);
            Assert.AreEqual(3.8411806104750186, range1.Max);
            Assert.AreEqual(1.0841613594568846, range2.Min);
            Assert.AreEqual(4.0728683052205534, range2.Max);
            Assert.AreEqual(1.0841613594568846, range3.Min);
            Assert.AreEqual(4.0728683052205534, range3.Max);
        }

        [Test]
        public void BetaPERTTest()
        {
            #region doc_pert
            // Create a Beta from a minimum, maximum and most likely value
            var b = GeneralizedBetaDistribution.Pert(min: 1, max: 3, mode: 2);

            double mean = b.Mean;     // 2
            double median = b.Median; // 2
            double mode = b.Mode;     // 2
            double var = b.Variance;  // 0.071428571428571425
            #endregion

            double min = b.Min;
            double max = b.Max;
            double alpha = b.Alpha;
            double beta = b.Beta;

            Assert.AreEqual(mean, median);
            Assert.AreEqual(mean, mode);

            Assert.AreEqual(2, mean);
            Assert.AreEqual(2, median);
            Assert.AreEqual(2.0, mode);

            Assert.AreEqual(0.071428571428571425, var);
            Assert.AreEqual(1.0, min);
            Assert.AreEqual(3, max);
            Assert.AreEqual(3, alpha);
            Assert.AreEqual(3, beta);
        }

        [Test]
        public void BetaPERTTest2()
        {
            // Create a Beta from a minimum, maximum and most likely value
            var b = GeneralizedBetaDistribution.Pert(min: 1, max: 3, mode: 1.5);

            double mean = b.Mean;     // 1.6666666666666667
            double median = b.Median; // 1.6276203409113952
            double var = b.Variance;  // 0.063492063492063516
            double mode = b.Mode;     // 1.5

            double min = b.Min;       // 1.0
            double max = b.Max;       // 3.0
            double alpha = b.Alpha;   // 1.9999999999999993
            double beta = b.Beta;     // 3.9999999999999982

            Assert.AreEqual(1.6666666666666667, mean);
            Assert.AreEqual(1.6276203409113952, median);
            Assert.AreEqual(0.063492063492063516, var);
            Assert.AreEqual(1.5, mode);
            Assert.AreEqual(1.0, min);
            Assert.AreEqual(3.0, max);
            Assert.AreEqual(1.9999999999999993, alpha);
            Assert.AreEqual(3.9999999999999982, beta);
        }

        [Test]
        public void BetaVosePERTTest()
        {
            #region doc_pert2
            // Create a Beta from a minimum, maximum and most likely value
            var b = GeneralizedBetaDistribution.Vose(min: 1, max: 3, mode: 1.42);

            double mean = b.Mean;     // 1.6133333333333333
            double median = b.Median; // 1.5727889200146494
            double mode = b.Mode;     // 1.4471823077804513
            double var = b.Variance;  // 0.055555555555555546
            #endregion

            double min = b.Min;
            double max = b.Max;
            double alpha = b.Alpha;
            double beta = b.Beta;

            Assert.AreEqual(1.6133333333333333, mean);
            Assert.AreEqual(1.5727889200146494, median);
            Assert.AreEqual(0.055555555555555546, var);
            Assert.AreEqual(1.4471823077804513, mode);
            Assert.AreEqual(1, min);
            Assert.AreEqual(3, max);
            Assert.AreEqual(2.0406826666666666, alpha);
            Assert.AreEqual(4.6137173333333337, beta);
        }

        [Test]
        public void BetaVosePERTTest_ModeEqualMean()
        {
            // Create a Beta from a minimum, maximum and most likely value
            var b = GeneralizedBetaDistribution.Vose(min: 1, max: 3, mode: 2);

            double mean = b.Mean;
            double median = b.Median;
            double var = b.Variance;
            double mode = b.Mode;

            double min = b.Min;
            double max = b.Max;
            double alpha = b.Alpha;
            double beta = b.Beta;

            Assert.AreEqual(mean, median);
            Assert.AreEqual(mean, mode);

            Assert.AreEqual(2, mean);
            Assert.AreEqual(2, median);
            Assert.AreEqual(2, mode);
            Assert.AreEqual(0.055555555555555552, var);
            Assert.AreEqual(1, min);
            Assert.AreEqual(3, max);
            Assert.AreEqual(4, alpha);
            Assert.AreEqual(4, beta);
        }

        [Test]
        public void BetaMeanTest2()
        {
            int trials = 161750;
            int successes = 10007;

            var betaDistribution = GeneralizedBetaDistribution.Standard(successes, trials);

            double mean = betaDistribution.Mean; // 0.06187249616697166
            double median = betaDistribution.Median; // 0.06187069085946604
            double p025 = betaDistribution.InverseDistributionFunction(p: 0.025); // 0.06070354581334864
            double p975 = betaDistribution.InverseDistributionFunction(p: 0.975); // 0.0630517079399996

            string str = betaDistribution.ToString();

            Assert.AreEqual(0.06187249616697166, mean);
            Assert.AreEqual(0.06187069085946604, median, 1e-6);
            Assert.AreEqual(0.06070354581334864, p025, 1e-6);
            Assert.AreEqual(0.0630517079399996, p975, 1e-6);
            Assert.AreEqual("B(x; α = 10008, β = 151744, min = 0, max = 1)", str);
        }

        [Test]
        public void BetaMeanTest3()
        {
            int trials = 100;
            int successes = 78;

            var betaDistribution = GeneralizedBetaDistribution.Standard(successes, trials);

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

        [Test]
        public void BetaFitTest1()
        {
            double[] x = { 0.1, 0.5, 0.3, 0.8, 0.6, 0.7, 0.9, 0.9, 0.9, 0.9 };

            var target = GeneralizedBetaDistribution.Standard(0, 1);

            target.Fit(x);

            Assert.AreEqual(1.1810718232044195, target.Alpha, 1e-10);
            Assert.AreEqual(0.60843093922651903, target.Beta, 1e-10);
        }

        [Test]
        public void BetaFit_RealWeights()
        {
            double[] x = { 1.0, 0.1, 0.5, 0.3, 0.5, 0.8, 0.6, 0.7, 0.9, 0.9, 0.9 };
            int[] w = { 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2 };

            var target = new GeneralizedBetaDistribution(1, 1);

            target.Fit(x, w.ToDouble());

            Assert.AreEqual(1.1401591160220996, target.Alpha);
            Assert.AreEqual(0.58735469613259694, target.Beta);
        }

        [Test]
        public void BetaFit_IntWeights()
        {
            double[] x = { 1.0, 0.1, 0.5, 0.3, 0.5, 0.8, 0.6, 0.7, 0.9, 0.9, 0.9 };
            int[] w = { 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2 };

            var target = new GeneralizedBetaDistribution(1, 1);

            target.Fit(x, w);

            Assert.AreEqual(1.1810718232044195, target.Alpha);
            Assert.AreEqual(0.60843093922651903, target.Beta, 1e-8);
        }

        [Test]
        public void BetaMLEFitTest1()
        {
            double[] x = samples;

            {
                var target = GeneralizedBetaDistribution.Standard(1, 1);
                var options = new BetaOptions() { Method = BetaEstimationMethod.Moments };
                target.Fit(x, options);

                Assert.AreEqual(1, target.Alpha, 0.04);
                Assert.AreEqual(3, target.Beta, 0.5);
            }

            {
                var target = GeneralizedBetaDistribution.Standard(0, 1);
                var options = new BetaOptions() { Method = BetaEstimationMethod.MaximumLikelihood };
                target.Fit(x, options);

                Assert.AreEqual(1, target.Alpha, 0.04);
                Assert.AreEqual(3, target.Beta, 0.55);
            }
        }

        [Test]
        public void BetaMLEFit_RealWeights()
        {
            double[] x = { 1.0, 0.1, 0.5, 0.3, 0.5, 0.8, 0.6, 0.7, 0.9, 0.9, 0.9 };
            int[] w = { 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2 };

            var target = new GeneralizedBetaDistribution(1, 1);


            var options = new BetaOptions()
            {
                Method = BetaEstimationMethod.MaximumLikelihood
            };

            target.Fit(x, w.ToDouble(), options);

            Assert.AreEqual(1.1401591160220996, target.Alpha);
            Assert.AreEqual(0.58735469613259694, target.Beta);
        }

        [Test]
        public void BetaMLEFit_IntWeights()
        {
            double[] x = { 1.0, 0.1, 0.5, 0.3, 0.5, 0.8, 0.6, 0.7, 0.9, 0.9, 0.9 };
            int[] w = { 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2 };

            var target = GeneralizedBetaDistribution.Standard(0, 1);


            var options = new BetaOptions()
            {
                Method = BetaEstimationMethod.MaximumLikelihood
            };

            target.Fit(x, w, options);

            Assert.AreEqual(1.1810718232044195, target.Alpha);
            Assert.AreEqual(0.60843093922651903, target.Beta, 1e-10);
        }


        [Test]
        public void RangeTest()
        {
            Assert.DoesNotThrow(() => new GeneralizedBetaDistribution(1e+300, 1e+300));
            //Assert.Throws<ArgumentOutOfRangeException>(() => new GeneralizedBetaDistribution(1e+308, 1e+308));
        }

        [Test]
        public void BetaGenerateTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int n = 100000;

            var beta = new GeneralizedBetaDistribution(alpha: 2, beta: 3, min: 0, max: 1);
            double[] samples = beta.Generate(samples: n);

            Assert.AreEqual(n, samples.Length);

            var actual = GeneralizedBetaDistribution.Estimate(samples, 0, 1);

            Assert.AreEqual(2, actual.Alpha, 1e-2);
            Assert.AreEqual(3, actual.Beta, 1e-2);
        }

        [Test]
        public void BetaGenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int n = 100000;

            var beta = new GeneralizedBetaDistribution(alpha: 2, beta: 3, min: 0, max: 1);
            double[] samples = new double[n];

            for (int i = 0; i < n; i++)
                samples[i] = beta.Generate();

            var actual = GeneralizedBetaDistribution.Estimate(samples, 0, 1);

            Assert.AreEqual(2, actual.Alpha, 3e-2);
            Assert.AreEqual(3, actual.Beta, 3e-2);
        }

        [Test]
        public void BetaRandomTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int n = 100000;

            double[] samples = GeneralizedBetaDistribution
                .Random(alpha: 2, beta: 3, min: 0, max: 1, samples: n);

            Assert.AreEqual(n, samples.Length);

            var actual = GeneralizedBetaDistribution.Estimate(samples, 0, 1);

            Assert.AreEqual(2, actual.Alpha, 1e-2);
            Assert.AreEqual(3, actual.Beta, 1e-2);
        }

        [Test]
        public void BetaRandomTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int n = 100000;

            double[] samples = GeneralizedBetaDistribution
                .Random(alpha: 2, beta: 3, min: 10, max: 15, samples: n);

            for (int i = 0; i < samples.Length; i++)
            {
                Assert.IsTrue(samples[i] > 10);
                Assert.IsTrue(samples[i] < 15);
            }

            Assert.AreEqual(n, samples.Length);

            var actual = GeneralizedBetaDistribution.Estimate(samples, 10, 15,
                new GeneralizedBetaOptions { Method = BetaEstimationMethod.MaximumLikelihood });

            Assert.AreEqual(2, actual.Alpha, 0.015);
            Assert.AreEqual(3, actual.Beta, 0.03);
        }


        private static double[] samples =
        {
            0.0576536, 0.0259565, 0.00823091, 0.0734909, 0.73978, 0.461741, 0.0379376, 0.388558,
            0.229866, 0.11422, 0.175104, 0.392522, 0.477605, 0.14909, 0.0669694, 0.345076, 0.123438,
            0.390018, 0.35776, 0.221581, 0.306762, 0.00845882, 0.215674, 0.00725579, 0.0269225,
            0.257018, 0.698815, 0.123448, 0.128692, 0.112612, 0.0181288, 0.237952, 0.33275, 0.0382219,
            0.219962, 0.0805055, 0.194096, 0.0689114, 0.15014, 0.0785429, 0.204803, 0.132801, 0.0780098,
            0.285554, 0.00239364, 0.300125, 0.0100048, 0.101187, 0.457942, 0.329837, 0.182756, 0.222086,
            0.24584, 0.190944, 0.108643, 0.293735, 0.0837905, 0.408274, 0.599756, 0.16522, 0.402054,
            0.383576, 0.599936, 0.0363877, 0.709853, 0.0564157, 0.0381653, 0.00269904, 0.0677391,
            0.403635, 0.214474, 0.214504, 0.247671, 0.0727752, 0.488096, 0.0107926, 0.479685, 0.21034,
            0.122196, 0.69273, 0.277719, 0.388118, 0.0662547, 0.626829, 0.221732, 0.176252, 0.209033,
            0.0218196, 0.326473, 0.156983, 0.0465249, 0.21851, 0.344093, 0.0325541, 0.104152, 0.350898,
            0.657449, 0.175062, 0.0426272, 0.0186954, 0.111174, 0.0526368, 0.0122465, 0.352131, 0.113912,
            0.520488, 0.291171, 0.116819, 0.185977, 0.0157473, 0.0164909, 0.151552, 0.24276, 0.175418,
            0.307812, 0.420045, 0.318959, 0.590715, 0.420124, 0.0429698, 0.270997, 0.203933, 0.111277,
            0.0303802, 0.242686, 0.622897, 0.386008, 0.110773, 0.00839994, 0.0229025, 0.00373459,
            0.086345, 0.177629, 0.285038, 0.10536, 0.209442, 0.252373, 0.312422, 0.290403, 0.558124,
            0.0408083, 0.204488, 0.0199158, 0.328801, 0.0253332, 0.137478, 0.0268162, 0.223154,
            0.328269, 0.22767, 0.213346, 0.345765, 0.343224, 0.496009, 0.269042, 0.345304, 0.152658,
            0.316284, 0.038899, 0.399112, 0.18071, 0.058962, 0.132478, 0.513784, 0.127359, 0.331945,
            0.0885819, 0.258951, 0.0195058, 0.0934522, 0.176096, 0.41691, 0.43996, 0.184367, 0.229717,
            0.198429, 0.237468, 0.145022, 0.194586, 0.0599747, 0.0848648, 0.254936, 0.0703234, 0.169601,
            0.0293011, 0.46589, 0.870883, 0.229739, 0.0685973, 0.079173, 0.209093, 0.254463, 0.193677,
            0.243653, 0.259542, 0.166267, 0.106444, 0.250221, 0.0251355, 0.718235, 0.0675517, 0.0803322,
            0.194324, 0.481255, 0.00658332, 0.246207, 0.0219087, 0.158527, 0.018173, 0.281022, 0.508714,
            0.0287672, 0.677162, 0.242504, 0.495153, 0.216918, 0.245249, 0.455043, 0.204309, 0.0475366,
            0.263423, 0.511431, 0.268065, 0.175027, 0.112739, 0.0564441, 0.373102, 0.0976276, 0.0652941,
            0.247803, 0.423739, 0.209293, 0.116794, 0.0202292, 0.0663067, 0.447308, 0.0326603, 0.329269,
            0.149665, 0.0657793, 0.0920978, 0.592771, 0.145452, 0.0434141, 0.383149, 0.686177, 0.306567,
            0.34562, 0.527187, 0.282707, 0.162794, 0.0949111, 0.236317, 0.155303, 0.337799, 0.362701,
            0.047339, 0.307746, 0.156433, 0.310438, 0.162522, 0.0737563, 0.383657, 0.766691, 0.0462097,
            0.0367943, 0.442429, 0.161974, 0.0899691, 0.179522, 0.601936, 0.207823, 0.187397, 0.178963,
            0.0300015
        };
    }
}
