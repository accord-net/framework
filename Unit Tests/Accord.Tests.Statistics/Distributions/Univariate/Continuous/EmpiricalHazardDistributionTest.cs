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
    public class EmpiricalHazardDistributionTest
    {

        [Test]
        public void DocumentationExample_Aalen()
        {
            #region doc_create
            // Consider the following hazard rates, occurring at the given time steps
            double[] times = { 0.5, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 17, 20, 21 };

            double[] hazards =
            {
                0, 0.111111111111111, 0.0625, 0.0714285714285714, 0.0769230769230769,
                0, 0.0909090909090909, 0, 0.111111111111111, 0.125, 0,
                0.166666666666667, 0.2, 0, 0.5, 0
            };

            // Create a new distribution given the observations and event times
            var distribution = new EmpiricalHazardDistribution(times, hazards);

            // Common measures
            double mean = distribution.Mean;     // 6.1658527179584119
            double median = distribution.Median; // 11.999999704601453
            double var = distribution.Variance;  // 44.101147497430993

            // Cumulative distribution functions
            double cdf = distribution.DistributionFunction(x: 4);               //  0.275274821017619
            double ccdf = distribution.ComplementaryDistributionFunction(x: 4); //  0.724725178982381
            double icdf = distribution.InverseDistributionFunction(p: cdf);     //  4.4588994137113307

            // Probability density functions
            double pdf = distribution.ProbabilityDensityFunction(x: 4);         //  0.055748090690952365
            double lpdf = distribution.LogProbabilityDensityFunction(x: 4);     // -2.8869121169242962

            // Hazard (failure rate) functions
            double hf = distribution.HazardFunction(x: 4);                      //  0.0769230769230769
            double chf = distribution.CumulativeHazardFunction(x: 4);           //  0.32196275946275932

            string str = distribution.ToString(); // H(x; v, t)
            #endregion

            try { double mode = distribution.Mode; Assert.Fail(); }
            catch { }

            Assert.AreEqual(SurvivalEstimator.FlemingHarrington, distribution.Estimator);
            Assert.AreEqual(1, distribution.ComplementaryDistributionFunction(0));
            Assert.AreEqual(0, distribution.ComplementaryDistributionFunction(Double.PositiveInfinity));

            Assert.AreEqual(6.1658527179584119, mean);
            Assert.AreEqual(11.999999704601453, median, 1e-6);
            Assert.AreEqual(44.101147497430993, var);
            Assert.AreEqual(0.32196275946275932, chf);
            Assert.AreEqual(0.275274821017619, cdf);
            Assert.AreEqual(0.055748090690952365, pdf);
            Assert.AreEqual(-2.8869121169242962, lpdf);
            Assert.AreEqual(0.0769230769230769, hf);
            Assert.AreEqual(0.724725178982381, ccdf);
            Assert.AreEqual(4.9284925495354344, icdf, 1e-8);
            Assert.AreEqual("H(x; v, t)", str);

            var range1 = distribution.GetRange(0.95);
            var range2 = distribution.GetRange(0.99);
            var range3 = distribution.GetRange(0.01);

            Assert.AreEqual(1, range1.Min, 1e-3);
            Assert.AreEqual(22, range1.Max, 1e-3);
            Assert.AreEqual(1, range2.Min, 1e-3);
            Assert.AreEqual(22, range2.Max, 1e-3);
            Assert.AreEqual(1, range3.Min, 1e-3);
            Assert.AreEqual(22, range3.Max, 1e-3);

            for (int i = 0; i < hazards.Length; i++)
                Assert.AreEqual(hazards[i], distribution.HazardFunction(times[i]));

            Assert.AreEqual(0, distribution.Support.Min);
            Assert.AreEqual(22, distribution.Support.Max);

            Assert.AreEqual(distribution.InverseDistributionFunction(0), distribution.Support.Min);
            Assert.AreEqual(distribution.InverseDistributionFunction(1), distribution.Support.Max);
        }

        [Test]
        public void DocumentationExample_KaplanMeier()
        {
            // Consider the following hazard rates, occurring at the given time steps
            double[] times = { 0.5, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 17, 20, 21 };

            double[] hazards =
            {
                0, 0.111111111111111, 0.0625, 0.0714285714285714, 0.0769230769230769,
                0, 0.0909090909090909, 0, 0.111111111111111, 0.125, 0,
                0.166666666666667, 0.2, 0, 0.5, 0
            };


            // Create a new distribution given the observations and event times
            var distribution = new EmpiricalHazardDistribution(times, hazards, SurvivalEstimator.KaplanMeier);

            // Common measures
            double mean = distribution.Mean;     // 5.49198237428757
            double median = distribution.Median; // 11.999999704601453
            double var = distribution.Variance;  // 39.83481657555663

            // Cumulative distribution functions
            double cdf = distribution.DistributionFunction(x: 4);               //  0.275274821017619
            double ccdf = distribution.ComplementaryDistributionFunction(x: 4); //  0.018754904264376961
            double icdf = distribution.InverseDistributionFunction(p: cdf);     //  5.1216931216931183

            // Probability density functions
            double pdf = distribution.ProbabilityDensityFunction(x: 4);         //  0.055748090690952365
            double lpdf = distribution.LogProbabilityDensityFunction(x: 4);     // -2.8869121169242962

            // Hazard (failure rate) functions
            double hf = distribution.HazardFunction(x: 4);                      //  0.0769230769230769
            double chf = distribution.CumulativeHazardFunction(x: 4);           //  0.32196275946275932

            string str = distribution.ToString(); // H(x; v, t)

            try { double mode = distribution.Mode; Assert.Fail(); }
            catch { }

            Assert.AreEqual(SurvivalEstimator.KaplanMeier, distribution.Estimator);
            Assert.AreEqual(1, distribution.ComplementaryDistributionFunction(0));
            Assert.AreEqual(0, distribution.ComplementaryDistributionFunction(Double.PositiveInfinity));

            Assert.AreEqual(5.49198237428757, mean);
            Assert.AreEqual(11.999999704601453, median, 1e-6);
            Assert.AreEqual(39.83481657555663, var);
            Assert.AreEqual(0.33647223662121273, chf);
            Assert.AreEqual(0.28571428571428559, cdf);
            Assert.AreEqual(0.054945054945054937, pdf);
            Assert.AreEqual(-2.9014215940827497, lpdf);
            Assert.AreEqual(0.0769230769230769, hf);
            Assert.AreEqual(0.71428571428571441, ccdf);

            double iicdf = distribution.DistributionFunction(icdf);
            Assert.AreEqual(iicdf, cdf);
            Assert.AreEqual(5.1216931216931183, icdf, 1e-8);
            Assert.AreEqual("H(x; v, t)", str);

            var range1 = distribution.GetRange(0.95);
            var range2 = distribution.GetRange(0.99);
            var range3 = distribution.GetRange(0.01);

            Assert.AreEqual(1, range1.Min, 1e-3);
            Assert.AreEqual(22, range1.Max, 1e-3);
            Assert.AreEqual(1, range2.Min, 1e-3);
            Assert.AreEqual(22, range2.Max, 1e-3);
            Assert.AreEqual(1, range3.Min, 1e-3);
            Assert.AreEqual(22, range3.Max, 1e-3);

            for (int i = 0; i < hazards.Length; i++)
                Assert.AreEqual(hazards[i], distribution.HazardFunction(times[i]));


            Assert.AreEqual(0, distribution.Support.Min);
            Assert.AreEqual(22, distribution.Support.Max);

            Assert.AreEqual(distribution.InverseDistributionFunction(0), distribution.Support.Min);
            Assert.AreEqual(distribution.InverseDistributionFunction(1), distribution.Support.Max);
        }

        [Test]
        public void MeasuresTest()
        {
            double[] values =
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.9000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 1, 9, 8, 7, 3, 6, 5, 4, 2, 10
            };

            var target = new EmpiricalHazardDistribution(times, values);
            var general = new GeneralContinuousDistribution(target);

            //Assert.AreEqual(general.Mean, target.Mean);
            //Assert.AreEqual(general.Variance, target.Variance);
            Assert.AreEqual(general.Median, target.Median);

            for (int i = -10; i < 10; i++)
            {
                double x = i;
                double expected = general.CumulativeHazardFunction(x);
                double actual = target.CumulativeHazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-4);
            }

            for (int i = -10; i < 10; i++)
            {
                double x = i;
                double expected = general.HazardFunction(x);
                double actual = target.HazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-5);
            }
        }

        [Test]
        public void MedianTest()
        {
            double[] values =
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            var target = new EmpiricalHazardDistribution(times, values);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));

            Assert.AreEqual(1, target.ComplementaryDistributionFunction(0));
            Assert.AreEqual(0, target.ComplementaryDistributionFunction(Double.PositiveInfinity));
        }

        [Test]
        public void DistributionFunctionTest2()
        {

            double[] values =
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            var target = new EmpiricalHazardDistribution(times, values);

            double[] expected =
            {
                1.000000000000000,
                0.999238023657475,
                0.999238023657475,
                0.999238023657475,
                0.999238023657475,
                0.999238023657475,
                0.98893509519066469,
                0.98893509519066469,
                0.96284543081744489,
                0.92957227114936058,
                0.92957227114936058,
            };


            double[] hazardFunction = new double[expected.Length];
            double[] survivalFunction = new double[expected.Length];

            for (int i = 0; i < 11; i++)
                hazardFunction[i] = target.CumulativeHazardFunction(i + 1);

            for (int i = 0; i < 11; i++)
                survivalFunction[i] = target.ComplementaryDistributionFunction(i + 1);


            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], survivalFunction[i], 1e-5);

                // Ho = -log(So)
                Assert.AreEqual(hazardFunction[i], -Math.Log(survivalFunction[i]), 1e-5);

                // So = exp(-Ho)
                Assert.AreEqual(survivalFunction[i], Math.Exp(-hazardFunction[i]), 1e-5);
            }

            Assert.AreEqual(1, target.ComplementaryDistributionFunction(0));
            Assert.AreEqual(0, target.ComplementaryDistributionFunction(Double.PositiveInfinity));
        }


        [Test]
        public void LeukemiaExample_KaplanMeier()
        {
            // The following are times of remission (weeks) for 21 leukemia
            // patients receiving control treatment (Table 1.1 of Cox & Oakes):
            // http://www-personal.umich.edu/~yili/lect2notes.pdf

            double[] t = { 1, 1, 2, 2, 3, 4, 4, 5, 5, 8, 8, 8, 8, 11, 11, 12, 12, 15, 17, 22, 23 };

            var distribution = EmpiricalHazardDistribution.Estimate(t, 
                survival: SurvivalEstimator.KaplanMeier, hazard: HazardEstimator.KaplanMeier);

            Assert.AreEqual(1, distribution.Survivals[0]);
            Assert.AreEqual(0.905, distribution.Survivals[1], 1e-3);
            Assert.AreEqual(0.809, distribution.Survivals[2], 1e-3);
            Assert.AreEqual(0.762, distribution.Survivals[3], 1e-3);

            /*
             http://statpages.org/prophaz2.html
                1, 1 
                1, 1 
                2, 1 
                2, 1
                3, 1
                4, 1
                4, 1
                5, 1
                5, 1 
                8, 1 
                8, 1 
                8, 1 
                8, 1 
                11, 1
                11, 1 
                12, 1 
                12, 1 
                15, 1 
                17, 1 
                22, 1 
                23, 1
             */
        }

        [Test]
        public void LeukemiaExampleCensoring_KaplanMeier_KaplanMeier()
        {
            // The following are times of remission (weeks) for 21 leukemia
            // patients receiving control treatment (Table 1.1 of Cox & Oakes):

            double[] t = { 6, 6, 6, 6, 7, 9, 10, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 32, 34, 35 };
            int[] c = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 };

            var distribution = EmpiricalHazardDistribution.Estimate(t, c,
                survival: SurvivalEstimator.KaplanMeier, hazard: HazardEstimator.KaplanMeier);

            int[] intervals = { 6, 7, 9, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 34, 35 };

            double[] expected =
            {
                0.8571 , 0.8067, 0.8067, 0.7529, 0.7529, 0.6902,
                0.6275, 0.6275, 0.6275, 0.6275, 0.5378, 0.4482,
                0.4482, 0.4482, 0.4482, 0.4482
            };

            for (int i = 0; i < intervals.Length; i++)
            {
                double x = intervals[i];
                double actual = distribution.ComplementaryDistributionFunction(x);

                double e = expected[i];
                Assert.AreEqual(e, actual, 1e-4);
            }
        }

        [Test]
        public void LeukemiaExampleCensoring_KaplanMeier_FlemingHarrington()
        {
            // The following are times of remission (weeks) for 21 leukemia
            // patients receiving control treatment (Table 1.1 of Cox & Oakes):

            double[] t = { 6, 6, 6, 6, 7, 9, 10, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 32, 34, 35 };
            int[] c = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 };

            var distribution = EmpiricalHazardDistribution.Estimate(t, c, survival: SurvivalEstimator.FlemingHarrington);

            int[] intervals = { 6, 7, 9, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 34, 35 };

            double[] expected =
            {
                0.8571 , 0.8067, 0.8067, 0.7529, 0.7529, 0.6902,
                0.6275, 0.6275, 0.6275, 0.6275, 0.5378, 0.4482,
                0.4482, 0.4482, 0.4482, 0.4482
            };

            for (int i = 0; i < intervals.Length; i++)
            {
                double x = intervals[i];
                double actual = distribution.ComplementaryDistributionFunction(x);

                double e = expected[i];
                Assert.AreEqual(e, actual, 0.1);
            }
        }

        [Test]
        public void LeukemiaExampleCensoring_KaplanMeier_NelsonAalen()
        {
            // http://www-personal.umich.edu/~yili/lect2notes.pdf
            // The following are times of remission (weeks) for 21 leukemia
            // patients receiving control treatment (Table 1.1 of Cox & Oakes):

            double[] t = { 6, 6, 6, 6, 7, 9, 10, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 32, 34, 35 };
            int[] c = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 };

            var distribution = EmpiricalHazardDistribution.Estimate(t, c,
                survival: SurvivalEstimator.KaplanMeier, hazard: HazardEstimator.BreslowNelsonAalen);

            int[] intervals = { 6, 7, 9, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 34, 35 };

            double[] expected =
            {
                0.8571 , 0.8067, 0.8067, 0.7529, 0.7529, 0.6902,
                0.6275, 0.6275, 0.6275, 0.6275, 0.5378, 0.4482,
                0.4482, 0.4482, 0.4482, 0.4482
            };

            for (int i = 0; i < intervals.Length; i++)
            {
                double x = intervals[i];
                double actual = distribution.ComplementaryDistributionFunction(x);

                double e = expected[i];
                Assert.AreEqual(e, actual, 0.02);
            }
        }

        [Test]
        public void LeukemiaExampleCensoring_FlemingHarrington_NelsonAalen()
        {
            // http://www-personal.umich.edu/~yili/lect2notes.pdf
            // The following are times of remission (weeks) for 21 leukemia
            // patients receiving control treatment (Table 1.1 of Cox & Oakes):

            double[] t = { 6, 6, 6, 6, 7, 9, 10, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 32, 34, 35 };
            int[] c = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 };

            var distribution = EmpiricalHazardDistribution.Estimate(t, outcome: c,
                survival: SurvivalEstimator.FlemingHarrington, hazard: HazardEstimator.BreslowNelsonAalen);

            int[] intervals = { 6, 7, 9, 10, 11, 13, 16, 17, 19, 20, 22, 23, 25, 32, 34, 35 };

            double[] expected =
            {
                0.8571 , 0.8067, 0.8067, 0.7529, 0.7529, 0.6902,
                0.6275, 0.6275, 0.6275, 0.6275, 0.5378, 0.4482,
                0.4482, 0.4482, 0.4482, 0.4482
            };

            for (int i = 0; i < intervals.Length; i++)
            {
                double x = intervals[i];
                double actual = distribution.ComplementaryDistributionFunction(x);

                double e = expected[i];
                Assert.AreEqual(e, actual, 0.1);
            }
        }

        [Test]
        public void KaplanMeierTest1()
        {
            // Example from
            // http://sas-and-r.blogspot.fr/2010/05/example-738-kaplan-meier-survival.html

            double[] times;
            SurvivalOutcome[] censor;
            CreateExample1(out times, out censor);

            var distribution = EmpiricalHazardDistribution.Estimate(times, outcome: censor,
                survival: SurvivalEstimator.KaplanMeier, hazard: HazardEstimator.KaplanMeier);

            Assert.AreEqual(SurvivalEstimator.KaplanMeier, distribution.Estimator);

            int[] t = { 1, 2, 3, 4, 6, 8, 9, 12, 14, 20 };
            double[] e = { 0.889, 0.833, 0.774, 0.714, 0.649, 0.577, 0.505, 0.421, 0.337, 0.168 };

            double[] actual = t.ToDouble().Apply(distribution.ComplementaryDistributionFunction);

            for (int i = 0; i < e.Length; i++)
                Assert.AreEqual(e[i], actual[i], 1e-3);

            // Assert.AreEqual(11.177, distribution.Mean);
            Assert.AreEqual(12, distribution.Median, 1e-5);
        }

        [Test]
        public void NelsonAalenTest1()
        {
            // Example from
            // http://sas-and-r.blogspot.fr/2010/05/example-738-kaplan-meier-survival.html
            // http://sas-and-r.blogspot.fr/2010/05/example-739-nelson-aalen-estimate-of.html

            double[] times;
            SurvivalOutcome[] censor;
            CreateExample1(out times, out censor);

            // Test with Breslow method

            {
                var distribution = EmpiricalHazardDistribution.Estimate(times, censor, ties: HazardTiesMethod.Breslow);

                double[] expectedCHF =
                {
                    0.0000000, 0.1111111, 0.1111111, 0.1736111, 0.1736111, 0.2450397, 0.3219628,
                    0.3219628, 0.4128719, 0.4128719, 0.5239830, 0.6489830, 0.6489830, 0.8156496,
                    1.0156496, 1.0156496, 1.0156496, 1.5156496, 1.5156496
                };

                double[] actualCHF = times.Apply(distribution.CumulativeHazardFunction);

                for (int i = 0; i < actualCHF.Length; i++)
                    Assert.AreEqual(expectedCHF[i], actualCHF[i], 1e-6);


                //Assert.AreEqual(11.177, distribution.Mean);
                Assert.AreEqual(12, distribution.Median, 1e-5);
            }

            // Test with Effron method
            {
                var distribution = EmpiricalHazardDistribution.Estimate(times, censor);

                double[] expectedCHF =
                {
                    0.0000000, 0.1111111, 0.1111111, 0.1756496, 0.1756496, 0.2497576, 0.3298003,
                    0.3298003, 0.4251104, 0.4251104, 0.5428935, 0.6764249, 0.6764249, 0.8587464,
                    1.0818900, 1.0818900, 1.0818900, 1.7750372, 1.7750372
                };

                double[] actualCHF = times.Apply(distribution.CumulativeHazardFunction);

                for (int i = 0; i < actualCHF.Length; i++)
                    Assert.AreEqual(expectedCHF[i], actualCHF[i], 1e-6);


                //Assert.AreEqual(11.177, distribution.Mean);
                Assert.AreEqual(12, distribution.Median, 1e-5);
            }
        }

        [Test]
        public void ConstructorTest1()
        {
            double[] times;
            SurvivalOutcome[] censor;
            CreateExample1(out times, out censor);

            var distribution = EmpiricalHazardDistribution.Estimate(times, censor,
                survival: SurvivalEstimator.FlemingHarrington, hazard: HazardEstimator.BreslowNelsonAalen);

            double[] t = distribution.Times;
            double[] s = distribution.Survivals;
            double[] h = distribution.Hazards;

            double[] nt = distribution.Times.Distinct();
            double[] nh = nt.Apply(distribution.HazardFunction);

            var target = new EmpiricalHazardDistribution(nt, nh, SurvivalEstimator.FlemingHarrington);

            for (int i = 0; i < times.Length; i++)
            {
                double expected = distribution.HazardFunction(times[i]);
                double actual = target.HazardFunction(times[i]);
                Assert.AreEqual(expected, actual);
            }

            for (int i = 0; i < times.Length; i++)
            {
                double expected = distribution.CumulativeHazardFunction(times[i]);
                double actual = target.CumulativeHazardFunction(times[i]);
                Assert.AreEqual(expected, actual, 1e-5);
            }

            for (int i = 0; i < times.Length; i++)
            {
                double expected = distribution.ProbabilityDensityFunction(times[i]);
                double actual = target.ProbabilityDensityFunction(times[i]);
                Assert.AreEqual(expected, actual, 1e-5);
            }
        }

        private static void CreateExample1(out double[] times, out SurvivalOutcome[] censor)
        {
            // Example from http://sas-and-r.blogspot.fr/2010/05/example-738-kaplan-meier-survival.html

            object[,] data =
            {
                // time  event
                { 0.5,   false },
                { 1,     true  },
                { 1,     true  },
                { 2,     true  },
                { 2,     false },
                { 3,     true  },
                { 4,     true  },
                { 5,     false },
                { 6,     true  },
                { 7,     false },
                { 8,     true  },
                { 9,     true  },
                { 10,    false },
                { 12,    true  },
                { 14,    false },
                { 14,    true  },
                { 17,    false },
                { 20,    true  },
                { 21,    false },
            };

            times = data.GetColumn(0).To<double[]>();
            censor = data.GetColumn(1).To<SurvivalOutcome[]>();
        }

        [Test]
        public void MeasuresTest_KaplanMeier()
        {
            double[] values =
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.9000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 1, 9, 8, 7, 3, 6, 5, 4, 2, 10
            };

            var target = new EmpiricalHazardDistribution(times, values, SurvivalEstimator.KaplanMeier);
            var general = new GeneralContinuousDistribution(target);

            //Assert.AreEqual(general.Mean, target.Mean);
            //Assert.AreEqual(general.Variance, target.Variance);
            Assert.AreEqual(general.Median, target.Median);

            for (int i = -10; i < 10; i++)
            {
                double x = i;
                double expected = general.CumulativeHazardFunction(x);
                double actual = target.CumulativeHazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-4);
            }

            for (int i = -10; i < 10; i++)
            {
                double x = i;
                double expected = general.HazardFunction(x);
                double actual = target.HazardFunction(x);
                Assert.AreEqual(expected, actual, 1e-5);
            }
        }

        [Test]
        public void MedianTest_KaplanMeier()
        {
            double[] values =
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            var target = new EmpiricalHazardDistribution(times, values, SurvivalEstimator.KaplanMeier);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));


            Assert.AreEqual(1, target.ComplementaryDistributionFunction(0));
            Assert.AreEqual(0, target.ComplementaryDistributionFunction(Double.PositiveInfinity));
        }

        [Test]
        public void DistributionFunctionTest2_KaplanMeier()
        {

            double[] values =
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            var target = new EmpiricalHazardDistribution(times, values, SurvivalEstimator.KaplanMeier);

            double[] expected =
            {
                1.000000000000000,
                0.999238023657475,
                0.999238023657475,
                0.999238023657475,
                0.999238023657475,
                0.999238023657475,
                0.98893509519066469,
                0.98893509519066469,
                0.96284543081744489,
                0.92957227114936058,
                0.92957227114936058,
            };


            double[] hazardFunction = new double[expected.Length];
            double[] survivalFunction = new double[expected.Length];

            for (int i = 0; i < 11; i++)
                hazardFunction[i] = target.CumulativeHazardFunction(i + 1);

            for (int i = 0; i < 11; i++)
                survivalFunction[i] = target.ComplementaryDistributionFunction(i + 1);


            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], survivalFunction[i], 1e-3);

                // Ho = -log(So)
                Assert.AreEqual(hazardFunction[i], -Math.Log(survivalFunction[i]), 1e-5);

                // So = exp(-Ho)
                Assert.AreEqual(survivalFunction[i], Math.Exp(-hazardFunction[i]), 1e-5);
            }


            Assert.AreEqual(1, target.ComplementaryDistributionFunction(0));
            Assert.AreEqual(0, target.ComplementaryDistributionFunction(Double.PositiveInfinity));
        }

        [Test]
        public void inverse_cdf()
        {
            // Consider the following hazard rates, occurring at the given time steps
            double[] times = { 0.5, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 17, 20, 21 };

            double[] hazards =
            {
                0, 0.111111111111111, 0.0625, 0.0714285714285714, 0.0769230769230769,
                0, 0.0909090909090909, 0, 0.111111111111111, 0.125, 0,
                0.166666666666667, 0.2, 0, 0.5, 0
            };

            var distribution = new EmpiricalHazardDistribution(times, hazards);

            Assert.AreEqual(0, distribution.Support.Min);
            Assert.AreEqual(22, distribution.Support.Max);

            Assert.AreEqual(0, distribution.InverseDistributionFunction(0));
            Assert.AreEqual(22, distribution.InverseDistributionFunction(1));
            Assert.AreEqual(22, distribution.InverseDistributionFunction(0.999));

            Assert.AreEqual(0, distribution.DistributionFunction(0));
            Assert.AreEqual(0.1051606831856301d, distribution.DistributionFunction(1));
            Assert.AreEqual(0.1593762566654946d, distribution.DistributionFunction(2));
            Assert.AreEqual(0.78033456236530996d, distribution.DistributionFunction(20));

            Assert.AreEqual(0.78033456236530996d, distribution.DistributionFunction(21));
            Assert.AreEqual(0.78033456236530996d, distribution.InnerDistributionFunction(21));

            Assert.AreEqual(1.0, distribution.DistributionFunction(22));
            Assert.AreEqual(1.0, distribution.InnerDistributionFunction(22));

            Assert.AreEqual(1.0, distribution.InnerDistributionFunction(23));
            Assert.AreEqual(1.0, distribution.InnerDistributionFunction(24));
            Assert.AreEqual(1.0, distribution.DistributionFunction(22));

            double[] percentiles = Vector.Interval(0.0, 1.0, stepSize: 0.1);

            for (int i = 0; i < percentiles.Length; i++)
            {
                double p = percentiles[i];
                double icdf = distribution.InverseDistributionFunction(p);
                double cdf = distribution.DistributionFunction(icdf);
                Assert.AreEqual(cdf, p, 0.1);
            }
        }
    }
}
