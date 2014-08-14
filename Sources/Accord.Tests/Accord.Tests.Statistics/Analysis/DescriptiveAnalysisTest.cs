﻿// Accord Unit Tests
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
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using AForge;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class DescriptiveAnalysisTest
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
        public void DescriptiveAnalysisConstructorTest1()
        {
            // Suppose we would like to compute descriptive
            // statistics from the following data samples:
            double[,] data =
            {
                { 1, 52, 5 },
                { 2, 12, 5 },
                { 1, 65, 5 },
                { 1, 25, 5 },
                { 2, 62, 5 },
            };

            // Create the analysis
            DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

            // Compute
            analysis.Compute();

            test(analysis);
        }

        [TestMethod()]
        public void DescriptiveAnalysisConstructorTest4()
        {
            double[][] data =
            {
                new double[] { 1, 52, 5 },
                new double[] { 2, 12, 5 },
                new double[] { 1, 65, 5 },
                new double[] { 1, 25, 5 },
                new double[] { 2, 62, 5 },
            };

            // Create the analysis
            DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

            analysis.Compute();

            test(analysis);
        }

        [TestMethod()]
        public void DescriptiveAnalysisConstructorTest3()
        {
            // Suppose we would like to compute descriptive
            // statistics from the following data samples:
            double[] data = { 52, 12, 65, 25, 62 };

            // Create the analysis
            DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

            // Compute
            analysis.Compute();

            var columnNames = analysis.ColumnNames;
            var correlation = analysis.CorrelationMatrix;
            var covariance = analysis.CovarianceMatrix;
            var deviationScores = analysis.DeviationScores;
            var distinct = analysis.Distinct;
            var kurtosis = analysis.Kurtosis;
            var means = analysis.Means;
            var medians = analysis.Medians;
            var modes = analysis.Modes;
            var ranges = analysis.Ranges;
            var samples = analysis.Samples;
            var skewness = analysis.Skewness;
            var source = analysis.Source;
            var standardDeviation = analysis.StandardDeviations;
            var standardErrors = analysis.StandardErrors;
            var standardScores = analysis.StandardScores;
            var sums = analysis.Sums;
            var variables = analysis.Variables;
            var variances = analysis.Variances;
            var confidence = analysis.Confidence;
            var quartiles = analysis.Quartiles;

            Assert.IsTrue(columnNames.IsEqual(new string[] 
            {
                "Column 0",
            }));

            Assert.IsTrue(correlation.IsEqual(new double[,] { { 1 } }));
            Assert.IsTrue(covariance.IsEqual(new double[,] { { 552.7 } }));

            Assert.IsTrue(deviationScores.IsEqual(new double[,] 
            {
                {   8.7999999999999972 },
                { -31.200000000000003  },
                {  21.799999999999997  },
                { -18.200000000000003  },
                {  18.799999999999997  }
            }));

            Assert.IsTrue(distinct.IsEqual(new int[] { 5 }));
            Assert.IsTrue(kurtosis.IsEqual(new double[] { -2.213664721197441 }));
            Assert.IsTrue(means.IsEqual(new double[] { 43.2 }));
            Assert.IsTrue(medians.IsEqual(new double[] { 52.0 }));
            Assert.IsTrue(modes.IsEqual(new double[] { 52.0 }));
            Assert.IsTrue(ranges.Apply(p => p.Min).IsEqual(new double[] { 12 }));
            Assert.IsTrue(ranges.Apply(p => p.Max).IsEqual(new double[] { 65 }));
            Assert.IsTrue(samples == 5);
            Assert.IsTrue(skewness.IsEqual(new double[] { -0.60008123614710385 }));
            Assert.IsTrue(source.IsEqual(new double[,] 
            {
                { 52 },
                { 12 },
                { 65 },
                { 25 },
                { 62 },
            }));

            Assert.IsTrue(standardDeviation.IsEqual(new double[] { 23.509572518444482 }));
            Assert.IsTrue(standardErrors.IsEqual(new double[] { 10.513800454640558 }));
            Assert.IsTrue(standardScores.IsEqual(new double[,] 
            { 
                {  0.37431561093235277 },
                { -1.3271189842147058  },
                {  0.92728185435514676 },
                { -0.77415274079191176 },
                {  0.79967425971911732 },
            }));

            Assert.IsTrue(sums.IsEqual(new double[] { 216.0 }));
            Assert.IsTrue(variables == 1);
            Assert.IsTrue(variances.IsEqual(new double[] { 552.7 }));

            Assert.AreEqual(1, confidence.Length);
            Assert.AreEqual(22.593329768263665, confidence[0].Min);
            Assert.AreEqual(63.806670231736341, confidence[0].Max);

            DoubleRange q;
            double q2 = Accord.Statistics.Tools.Quartiles(data, out q, alreadySorted: false);

            Assert.AreEqual(1, quartiles.Length);
            Assert.AreEqual(q.Min, quartiles[0].Min);
            Assert.AreEqual(q.Max, quartiles[0].Max);

            Assert.AreEqual(18.5, quartiles[0].Min);
            Assert.AreEqual(63.5, quartiles[0].Max);
        }

        [TestMethod()]
        public void DescriptiveAnalysisConstructorTest()
        {
            double[,] data = Matrix.Magic(3);
            string[] columnNames = { "x", "y", "z" };
            DescriptiveAnalysis target = new DescriptiveAnalysis(data, columnNames);
            target.Compute();

            Assert.AreEqual("x", target.ColumnNames[0]);
            Assert.AreEqual("y", target.ColumnNames[1]);
            Assert.AreEqual("z", target.ColumnNames[2]);


            Assert.IsTrue(target.CorrelationMatrix.IsEqual(new double[,]
            {
                {  1.0000,   -0.7559,    0.1429 },
                { -0.7559,    1.0000,   -0.7559 },
                {  0.1429,   -0.7559,    1.0000 },
            }, 0.0001));

            Assert.IsTrue(target.CovarianceMatrix.IsEqual(new double[,]
            {
                {  7,    -8,     1 },
                { -8,    16,    -8 },
                {  1,    -8,     7 },
            }, 0.00000001));

            Assert.IsTrue(target.StandardScores.IsEqual(new double[,]
            { 
                { 1.1339,   -1.0000,    0.3780 },
                { -0.7559,         0,    0.7559 },
                { -0.3780,    1.0000,   -1.1339 },
            }, 0.001));

            Assert.IsTrue(target.Means.IsEqual(new double[] { 5, 5, 5 }));

            Assert.IsTrue(target.StandardDeviations.IsEqual(new double[] { 2.6458, 4.0000, 2.6458 }, 0.001));

            Assert.IsTrue(target.Medians.IsEqual(new double[] { 4, 5, 6 }));


            Assert.AreEqual(3, target.Ranges[0].Min);
            Assert.AreEqual(8, target.Ranges[0].Max);
            Assert.AreEqual(1, target.Ranges[1].Min);
            Assert.AreEqual(9, target.Ranges[1].Max);
            Assert.AreEqual(2, target.Ranges[2].Min);
            Assert.AreEqual(7, target.Ranges[2].Max);

            Assert.AreEqual(3, target.Samples);
            Assert.AreEqual(3, target.Variables);

            Assert.IsTrue(target.Source.IsEqual(Matrix.Magic(3)));

            Assert.IsTrue(target.Sums.IsEqual(new double[] { 15, 15, 15 }));

            Assert.IsTrue(target.Variances.IsEqual(new double[] { 7, 16, 7 }));

            Assert.AreEqual(3, target.Quartiles[0].Min);
            Assert.AreEqual(8, target.Quartiles[0].Max);
            Assert.AreEqual(1, target.Quartiles[1].Min);
            Assert.AreEqual(9, target.Quartiles[1].Max);
            Assert.AreEqual(2, target.Quartiles[2].Min);
            Assert.AreEqual(7, target.Quartiles[2].Max);
        }


        private static void test(DescriptiveAnalysis analysis)
        {
            var columnNames = analysis.ColumnNames;
            var correlation = analysis.CorrelationMatrix;
            var covariance = analysis.CovarianceMatrix;
            var deviationScores = analysis.DeviationScores;
            var distinct = analysis.Distinct;
            var kurtosis = analysis.Kurtosis;
            var means = analysis.Means;
            var medians = analysis.Medians;
            var modes = analysis.Modes;
            var ranges = analysis.Ranges;
            var samples = analysis.Samples;
            var skewness = analysis.Skewness;
            var source = analysis.Source;
            var standardDeviation = analysis.StandardDeviations;
            var standardErrors = analysis.StandardErrors;
            var standardScores = analysis.StandardScores;
            var sums = analysis.Sums;
            var variables = analysis.Variables;
            var variances = analysis.Variances;
            var quartiles = analysis.Quartiles;
            var inner = analysis.InnerFences;
            var outer = analysis.OuterFences;

            Assert.IsTrue(columnNames.IsEqual(new string[] 
            {
                "Column 0",
                "Column 1",
                "Column 2",
            }));

            Assert.IsTrue(correlation.IsEqual(new double[,] 
            {
                {  1.00000000000000000, -0.24074447786944278, 0 },
                { -0.24074447786944278,  1.00000000000000000, 0 },
                {  0.00000000000000000,  0.00000000000000000, 0 }
            }));

            Assert.IsTrue(covariance.IsEqual(new double[,] 
            {
                {  0.3,  -3.1,  0.0 },
                { -3.1, 552.7,  0.0 },
                {  0.0,   0.0,  0.0 }
            }));

            Assert.IsTrue(deviationScores.IsEqual(new double[,] 
            {
                { -0.39999999999999991,   8.7999999999999972, 0.0 },
                {  0.60000000000000009, -31.200000000000003,  0.0 },
                { -0.39999999999999991,  21.799999999999997,  0.0 },
                { -0.39999999999999991, -18.200000000000003,  0.0 },
                {  0.60000000000000009,  18.799999999999997,  0.0 }
            }));

            Assert.IsTrue(distinct.IsEqual(new int[] { 2, 5, 1 }));
            Assert.IsTrue(kurtosis.IsEqual(new double[] { -3.3333333333333321, -2.213664721197441, double.NaN }));
            Assert.IsTrue(means.IsEqual(new double[] { 1.4, 43.2, 5.0 }));
            Assert.IsTrue(medians.IsEqual(new double[] { 1.0, 52.0, 5.0 }));
            Assert.IsTrue(modes.IsEqual(new double[] { 1.0, 52.0, 5.0 }));
            Assert.IsTrue(ranges.Apply(p => p.Min).IsEqual(new double[] { 1, 12, 5 }));
            Assert.IsTrue(ranges.Apply(p => p.Max).IsEqual(new double[] { 2, 65, 5 }));
            Assert.IsTrue(samples == 5);
            Assert.IsTrue(skewness.IsEqual(new double[] { 0.60858061945018527, -0.60008123614710385, double.NaN }));
            Assert.IsTrue(source.IsEqual(new double[,] 
            {
                { 1, 52, 5 },
                { 2, 12, 5 },
                { 1, 65, 5 },
                { 1, 25, 5 },
                { 2, 62, 5 },
            }));

            Assert.IsTrue(standardDeviation.IsEqual(new double[] { 0.54772255750516607, 23.509572518444482, 0.0 }));
            Assert.IsTrue(standardErrors.IsEqual(new double[] { 0.24494897427831777, 10.513800454640558, 0.0 }));
            Assert.IsTrue(standardScores.IsEqual(new double[,] 
            { 
                { -0.73029674334022132,  0.37431561093235277,  0.0 },
                {  1.0954451150103324,  -1.3271189842147058,   0.0 },
                { -0.73029674334022132,  0.92728185435514676,  0.0 },
                { -0.73029674334022132, -0.77415274079191176,  0.0 },
                {  1.0954451150103324,   0.79967425971911732,  0.0 },
            }));

            Assert.IsTrue(sums.IsEqual(new double[] { 7, 216.0, 25 }));
            Assert.IsTrue(variables == 3);
            Assert.IsTrue(variances.IsEqual(new double[] { 0.3, 552.7, 0.0 }));

            Assert.AreEqual(3, quartiles.Length);
            Assert.AreEqual(1, quartiles[0].Min);
            Assert.AreEqual(2, quartiles[0].Max);
            Assert.AreEqual(18.5, quartiles[1].Min);
            Assert.AreEqual(63.5, quartiles[1].Max);

            Assert.AreEqual(18.5 - 1.5 * (63.5 - 18.5), inner[1].Min);
            Assert.AreEqual(63.5 + 1.5 * (63.5 - 18.5), inner[1].Max);

            Assert.AreEqual(18.5 - 3 * (63.5 - 18.5), outer[1].Min);
            Assert.AreEqual(63.5 + 3 * (63.5 - 18.5), outer[1].Max);

            Assert.AreEqual(5, quartiles[2].Min);
            Assert.AreEqual(5, quartiles[2].Max);
        }

        [TestMethod()]
        public void DescriptiveAnalysisOneSampleTest()
        {
            double[] data = { 52 };

            // Create the analysis
            DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

            // Compute
            analysis.Compute();

            var columnNames = analysis.ColumnNames;
            var correlation = analysis.CorrelationMatrix;
            var covariance = analysis.CovarianceMatrix;
            var deviationScores = analysis.DeviationScores;
            var distinct = analysis.Distinct;
            var kurtosis = analysis.Kurtosis;
            var means = analysis.Means;
            var medians = analysis.Medians;
            var modes = analysis.Modes;
            var ranges = analysis.Ranges;
            var samples = analysis.Samples;
            var skewness = analysis.Skewness;
            var source = analysis.Source;
            var standardDeviation = analysis.StandardDeviations;
            var standardErrors = analysis.StandardErrors;
            var standardScores = analysis.StandardScores;
            var sums = analysis.Sums;
            var variables = analysis.Variables;
            var variances = analysis.Variances;
            var confidence = analysis.Confidence;
            var quartiles = analysis.Quartiles;
            var innerFence = analysis.InnerFences;
            var outerFence = analysis.OuterFences;

            Assert.IsTrue(columnNames.IsEqual(new string[] 
        {
            "Column 0",
        }));

            Assert.IsTrue(correlation.IsEqual(new double[,] { { Double.NaN } }));
            Assert.IsTrue(covariance.IsEqual(new double[,] { { Double.NaN } }));

            Assert.IsTrue(deviationScores.IsEqual(0));

            Assert.IsTrue(distinct.IsEqual(new int[] { 1 }));
            Assert.IsTrue(kurtosis.IsEqual(new double[] { Double.NaN }));
            Assert.IsTrue(means.IsEqual(new double[] { 52 }));
            Assert.IsTrue(medians.IsEqual(new double[] { 52.0 }));
            Assert.IsTrue(modes.IsEqual(new double[] { 52.0 }));
            Assert.IsTrue(samples == 1);
            Assert.IsTrue(skewness.IsEqual(new double[] { Double.NaN }));
            Assert.IsTrue(source.IsEqual(new double[,] 
        {
            { 52 },
        }));

            Assert.IsTrue(standardDeviation.IsEqual(new double[] { double.NaN }));
            Assert.IsTrue(standardErrors.IsEqual(new double[] { double.NaN }));
            Assert.IsTrue(standardScores.IsEqual(new double[,] 
        { 
            {  0.0 },
        }));

            Assert.IsTrue(sums.IsEqual(new double[] { 52 }));
            Assert.IsTrue(variables == 1);
            Assert.IsTrue(variances.IsEqual(new double[] { double.NaN }));

            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(52, ranges[0].Min);
            Assert.AreEqual(52, ranges[0].Max);

            Assert.AreEqual(1, confidence.Length);
            Assert.AreEqual(double.NaN, confidence[0].Min);
            Assert.AreEqual(double.NaN, confidence[0].Max);

            Assert.AreEqual(1, quartiles.Length);
            Assert.AreEqual(52, quartiles[0].Min);
            Assert.AreEqual(52, quartiles[0].Max);

            Assert.AreEqual(1, innerFence.Length);
            Assert.AreEqual(52, innerFence[0].Min);
            Assert.AreEqual(52, innerFence[0].Max);

            Assert.AreEqual(1, outerFence.Length);
            Assert.AreEqual(52, outerFence[0].Min);
            Assert.AreEqual(52, outerFence[0].Max);
        }

        [TestMethod()]
        public void DescriptiveAnalysisTwoSampleTest()
        {
            double[][] data = 
            { 
                new [] { 52.0 },
                new [] { 42.0 }
            };

            // Create the analysis
            DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

            // Compute
            analysis.Compute();

            var columnNames = analysis.ColumnNames;
            var correlation = analysis.CorrelationMatrix;
            var covariance = analysis.CovarianceMatrix;
            var deviationScores = analysis.DeviationScores;
            var distinct = analysis.Distinct;
            var kurtosis = analysis.Kurtosis;
            var means = analysis.Means;
            var medians = analysis.Medians;
            var modes = analysis.Modes;
            var ranges = analysis.Ranges;
            var samples = analysis.Samples;
            var skewness = analysis.Skewness;
            var source = analysis.Source;
            var standardDeviation = analysis.StandardDeviations;
            var standardErrors = analysis.StandardErrors;
            var standardScores = analysis.StandardScores;
            var sums = analysis.Sums;
            var variables = analysis.Variables;
            var variances = analysis.Variances;
            var confidence = analysis.Confidence;
            var quartiles = analysis.Quartiles;
            var innerFence = analysis.InnerFences;
            var outerFence = analysis.OuterFences;

            Assert.IsTrue(columnNames.IsEqual(new string[] 
            {
                "Column 0"
            }));

            Assert.IsTrue(correlation.IsEqual(Matrix.Create(1, 1, 0.99999999999999978)));
            Assert.IsTrue(covariance.IsEqual(Matrix.Create(1, 1, 50.0)));

            Assert.IsTrue(deviationScores.IsEqual(new double[,] { { 5 }, { -5 } }));

            Assert.IsTrue(distinct.IsEqual(new int[] { 2 }));
            Assert.IsTrue(kurtosis.IsEqual(new double[] { Double.NaN }));
            Assert.IsTrue(means.IsEqual(new double[] { 47 }));
            Assert.IsTrue(medians.IsEqual(new double[] { 94 }));
            Assert.IsTrue(modes.IsEqual(new double[] { 52.0 }));
            Assert.IsTrue(samples == 2);
            Assert.IsTrue(skewness.IsEqual(new double[] { Double.NaN }));
            Assert.IsTrue(source.IsEqual(new double[,] 
            {
                { 52 },
                { 42 }, 
            }));

            Assert.IsTrue(standardDeviation.IsEqual(new double[] { 7.0710678118654755 }));
            Assert.IsTrue(standardErrors.IsEqual(new double[] { 5.0 }));
            Assert.IsTrue(standardScores.IsEqual(new double[,] 
            { 
                {  0.70710678118654746 }, { -0.70710678118654746 },
            }));

            Assert.IsTrue(sums.IsEqual(new double[] { 94 }));
            Assert.IsTrue(variables == 1);
            Assert.IsTrue(variances.IsEqual(new double[] { 50 }));

            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(42, ranges[0].Min);
            Assert.AreEqual(52, ranges[0].Max);

            Assert.AreEqual(1, confidence.Length);
            Assert.AreEqual(37.200180077299734, confidence[0].Min);
            Assert.AreEqual(56.799819922700266, confidence[0].Max);

            Assert.AreEqual(1, quartiles.Length);
            Assert.AreEqual(73, quartiles[0].Min);
            Assert.AreEqual(68, quartiles[0].Max);

            Assert.AreEqual(1, innerFence.Length);
            Assert.AreEqual(80.5, innerFence[0].Min);
            Assert.AreEqual(60.5, innerFence[0].Max);

            Assert.AreEqual(1, outerFence.Length);
            Assert.AreEqual(88, outerFence[0].Min);
            Assert.AreEqual(53, outerFence[0].Max);
        }

        [TestMethod()]
        public void DescriptiveAnalysisNSampleTest()
        {
            for (int i = 1; i < 100; i++)
            {
                double[] data = Matrix.Random(i, 0.0, 1.0);

                // Create the analysis
                DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

                // Compute
                analysis.Compute();

                var columnNames = analysis.ColumnNames;
                var correlation = analysis.CorrelationMatrix;
                var covariance = analysis.CovarianceMatrix;
                var deviationScores = analysis.DeviationScores;
                var distinct = analysis.Distinct;
                var kurtosis = analysis.Kurtosis;
                var means = analysis.Means;
                var medians = analysis.Medians;
                var modes = analysis.Modes;
                var ranges = analysis.Ranges;
                var samples = analysis.Samples;
                var skewness = analysis.Skewness;
                var source = analysis.Source;
                var standardDeviation = analysis.StandardDeviations;
                var standardErrors = analysis.StandardErrors;
                var standardScores = analysis.StandardScores;
                var sums = analysis.Sums;
                var variables = analysis.Variables;
                var variances = analysis.Variances;
                var confidence = analysis.Confidence;
                var quartiles = analysis.Quartiles;
                var innerFence = analysis.InnerFences;
                var outerFence = analysis.OuterFences;

                Assert.AreEqual(1, columnNames.Length);
                Assert.AreEqual(1, correlation.Length);
                Assert.AreEqual(1, covariance.Length);
                Assert.AreEqual(i, deviationScores.Length);
                Assert.AreEqual(1, distinct.Length);
                Assert.AreEqual(1, kurtosis.Length);
                Assert.AreEqual(1, means.Length);
                Assert.AreEqual(1, medians.Length);
                Assert.AreEqual(1, modes.Length);
                Assert.AreEqual(1, ranges.Length);
                Assert.AreEqual(i, samples);
                Assert.AreEqual(1, skewness.Length);
                Assert.AreEqual(i, source.Length);
                Assert.AreEqual(1, standardDeviation.Length);
                Assert.AreEqual(1, standardErrors.Length);
                Assert.AreEqual(i, standardScores.Length);
                Assert.AreEqual(1, sums.Length);
                Assert.AreEqual(1, variables);
                Assert.AreEqual(1, variances.Length);
                Assert.AreEqual(1, confidence.Length);
                Assert.AreEqual(1, innerFence.Length);
                Assert.AreEqual(1, outerFence.Length);
                Assert.AreEqual(1, quartiles.Length);
            }

        }
    }
}
