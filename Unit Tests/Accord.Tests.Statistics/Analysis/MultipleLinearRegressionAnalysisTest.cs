// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Statistics.Analysis;
    using NUnit.Framework;
    using System;
    using AForge;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Math;
    using Accord.Statistics.Testing;

    [TestFixture]
    public class MultipleLinearRegressionAnalysisTest
    {

        [Test]
        public void ComputeTest()
        {
            // Example 5.1 from 
            // http://www.weibull.com/DOEWeb/estimating_regression_models_using_least_squares.htm

            double[][] inputs = 
            {
                new double[] { 41.9, 29.1 }, // 1 
                new double[] { 43.4, 29.3 }, // 2
                new double[] { 43.9, 29.5 }, // 3
                new double[] { 44.5, 29.7 }, // 4
                new double[] { 47.3, 29.9 }, // 5
                new double[] { 47.5, 30.3 }, // 6
                new double[] { 47.9, 30.5 }, // 7
                new double[] { 50.2, 30.7 }, // 8
                new double[] { 52.8, 30.8 }, // 9
                new double[] { 53.2, 30.9 }, // 10
                new double[] { 56.7, 31.5 }, // 11
                new double[] { 57.0, 31.7 }, // 12
                new double[] { 63.5, 31.9 }, // 13
                new double[] { 65.3, 32.0 }, // 14
                new double[] { 71.1, 32.1 }, // 15
                new double[] { 77.0, 32.5 }, // 16
                new double[] { 77.8, 32.9 }, // 17
            };

            double[] outputs = 
            {
                251.3,
                251.3,
                248.3,
                267.5,
                273.0,
                276.5,
                270.3,
                274.9,
                285.0,
                290.0,
                297.0,
                302.5,
                304.5,
                309.3,
                321.7,
                330.7,
                349.0,
            };

            var target = new MultipleLinearRegressionAnalysis(inputs, outputs, intercept: true);

            target.Compute();

            Assert.AreEqual(0.968022, target.RSquared, 1e-5);
            Assert.AreEqual(0.963454, target.RSquareAdjusted, 1e-5);

            Assert.AreEqual(2, target.Table[0].DegreesOfFreedom);
            Assert.AreEqual(14, target.Table[1].DegreesOfFreedom);
            Assert.AreEqual(16, target.Table[2].DegreesOfFreedom);

            Assert.AreEqual(12816.345909673832, target.Table[0].SumOfSquares, 1e-10);
            Assert.AreEqual(423.37409032616614, target.Table[1].SumOfSquares, 1e-10);
            Assert.AreEqual(13239.719999999998, target.Table[2].SumOfSquares, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Table[0].SumOfSquares));
            Assert.IsFalse(Double.IsNaN(target.Table[1].SumOfSquares));
            Assert.IsFalse(Double.IsNaN(target.Table[2].SumOfSquares));

            Assert.AreEqual(6408.1729548369158, target.Table[0].MeanSquares, 1e-10);
            Assert.AreEqual(30.241006451869008, target.Table[1].MeanSquares, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Table[0].MeanSquares));
            Assert.IsFalse(Double.IsNaN(target.Table[1].MeanSquares));

            Assert.AreEqual(211.90342871147618, target.Table[0].Statistic.Value, 1e-10);
            Assert.AreEqual(0.000000000034191538489380946, target.Table[0].Significance.PValue, 1e-16);
            Assert.IsFalse(Double.IsNaN(target.Table[0].Significance.PValue));

            Assert.AreEqual(1.2387232694931045, target.Coefficients[0].Value, 1e-10);
            Assert.AreEqual(12.082353323342893, target.Coefficients[1].Value, 1e-10);
            Assert.AreEqual(-153.51169396147372, target.Coefficients[2].Value, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Coefficients[0].Value));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[1].Value));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[2].Value));

            Assert.IsFalse(target.Coefficients[0].IsIntercept);
            Assert.IsFalse(target.Coefficients[1].IsIntercept);
            Assert.IsTrue(target.Coefficients[2].IsIntercept);

            Assert.AreEqual(0.394590262021004, target.Coefficients[0].StandardError, 1e-10);
            Assert.AreEqual(3.9322914100115307, target.Coefficients[1].StandardError, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Coefficients[0].StandardError));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[1].StandardError));

            Assert.AreEqual(3.1392646720388844, target.Coefficients[0].TTest.Statistic, 1e-10);
            Assert.AreEqual(3.0725986615797285, target.Coefficients[1].TTest.Statistic, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Coefficients[0].TTest.Statistic));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[1].TTest.Statistic));

            DoubleRange range;

            range = target.Coefficients[0].TTest.GetConfidenceInterval(0.9);
            Assert.AreEqual(0.54372744151743968, range.Min, 1e-10);
            Assert.AreEqual(1.9337190974687695, range.Max, 1e-10);

            range = target.Coefficients[1].TTest.GetConfidenceInterval(0.9);
            Assert.AreEqual(5.1563686060690417, range.Min, 1e-10);
            Assert.AreEqual(19.008338040616746, range.Max, 1e-10);


            MultipleLinearRegression mlr = new MultipleLinearRegression(2, true);
            mlr.Regress(inputs, outputs);
            double r2 = mlr.CoefficientOfDetermination(inputs, outputs, false);
            double r2a = mlr.CoefficientOfDetermination(inputs, outputs, true);

            Assert.AreEqual(r2, target.RSquared);
            Assert.AreEqual(r2a, target.RSquareAdjusted);
        }

        [Test]
        public void ComputeTest2()
        {
            // Consider the following data. An experimenter would
            // like to infer a relationship between two variables
            // A and B and a corresponding outcome variable R.

            double[][] example = 
            {
                //                A    B      R
                new double[] {  6.41, 10.11, 26.1 },
                new double[] {  6.61, 22.61, 33.8 },
                new double[] {  8.45, 11.11, 52.7 },
                new double[] {  1.22, 18.11, 16.2 },
                new double[] {  7.42, 12.81, 87.3 },
                new double[] {  4.42, 10.21, 12.5 },
                new double[] {  8.61, 11.94, 77.5 },
                new double[] {  1.73, 13.13, 12.1 },
                new double[] {  7.47, 17.11, 86.5 },
                new double[] {  6.11, 15.13, 62.8 },
                new double[] {  1.42, 16.11, 17.5 },
            };

            // For this, we first extract the input and output
            // pairs. The first two columns have values for the
            // input variables, and the last for the output:

            double[][] inputs = example.GetColumns(new[] { 0, 1 });
            double[] output = example.GetColumn(2);

            // Next, we can create a new multiple linear regression for the variables
            var regression = new MultipleLinearRegressionAnalysis(inputs, output, intercept: true);

            regression.Compute(); // compute the analysis

            // Now we can show a summary of analysis
            // Accord.Controls.DataGridBox.Show(regression.Coefficients);

            // We can also show a summary ANOVA
            // Accord.Controls.DataGridBox.Show(regression.Table);


            // And also extract other useful information, such
            // as the linear coefficients' values and std errors:
            double[] coef = regression.CoefficientValues;
            double[] stde = regression.StandardErrors;

            // Coefficients of performance, such as r²
            double rsquared = regression.RSquared;

            // Hypothesis tests for the whole model
            ZTest ztest = regression.ZTest;
            FTest ftest = regression.FTest;

            // and for individual coefficients
            TTest ttest0 = regression.Coefficients[0].TTest;
            TTest ttest1 = regression.Coefficients[1].TTest;

            // and also extract confidence intervals
            DoubleRange ci = regression.Coefficients[0].Confidence;

            Assert.AreEqual(3, coef.Length);
            Assert.AreEqual(8.7405051051757816, coef[0]);
            Assert.AreEqual(1.1198079243314365, coef[1], 1e-10);
            Assert.AreEqual(-19.604474518407862, coef[2], 1e-10);
            Assert.IsFalse(coef.HasNaN());

            Assert.AreEqual(2.375916659234715, stde[0], 1e-10);
            Assert.AreEqual(1.7268508921418664, stde[1], 1e-10);
            Assert.AreEqual(30.989640986710953, stde[2], 1e-10);
            Assert.IsFalse(coef.HasNaN());

            Assert.AreEqual(0.62879941171298936, rsquared, 1e-10);

            Assert.AreEqual(0.99999999999999822, ztest.PValue, 1e-10);
            Assert.AreEqual(0.018986050133298293, ftest.PValue, 1e-10);

            Assert.AreEqual(0.0062299844256985537, ttest0.PValue, 1e-10);
            Assert.AreEqual(0.53484850318449118, ttest1.PValue, 1e-14);
            Assert.IsFalse(Double.IsNaN(ttest1.PValue));

            Assert.AreEqual(3.2616314640800566, ci.Min, 1e-10);
            Assert.AreEqual(14.219378746271506, ci.Max, 1e-10);
        }

        [Test]
        public void learn_Test()
        {
            #region doc_learn_part1
            // Consider the following data. An experimenter would
            // like to infer a relationship between two variables
            // A and B and a corresponding outcome variable R.

            double[][] example = 
            {
                //                A    B      R
                new double[] {  6.41, 10.11, 26.1 },
                new double[] {  6.61, 22.61, 33.8 },
                new double[] {  8.45, 11.11, 52.7 },
                new double[] {  1.22, 18.11, 16.2 },
                new double[] {  7.42, 12.81, 87.3 },
                new double[] {  4.42, 10.21, 12.5 },
                new double[] {  8.61, 11.94, 77.5 },
                new double[] {  1.73, 13.13, 12.1 },
                new double[] {  7.47, 17.11, 86.5 },
                new double[] {  6.11, 15.13, 62.8 },
                new double[] {  1.42, 16.11, 17.5 },
            };

            // For this, we first extract the input and output
            // pairs. The first two columns have values for the
            // input variables, and the last for the output:

            double[][] inputs = example.GetColumns(new[] { 0, 1 });
            double[] output = example.GetColumn(2);

            // We can create a new multiple linear analysis for the variables
            var mlra = new MultipleLinearRegressionAnalysis(intercept: true);

            // Compute the analysis and obtain the estimated regression
            MultipleLinearRegression regression = mlra.Learn(inputs, output);
            #endregion

            // We can also show a summary ANOVA
            // Accord.Controls.DataGridBox.Show(regression.Table);

            #region doc_learn_part2
            // And also extract other useful information, such
            // as the linear coefficients' values and std errors:
            double[] coef = mlra.CoefficientValues;
            double[] stde = mlra.StandardErrors;

            // Coefficients of performance, such as r²
            double rsquared = mlra.RSquared; // 0.62879

            // Hypothesis tests for the whole model
            ZTest ztest = mlra.ZTest; // 0.99999
            FTest ftest = mlra.FTest; // 0.01898

            // and for individual coefficients
            TTest ttest0 = mlra.Coefficients[0].TTest; // 0.00622
            TTest ttest1 = mlra.Coefficients[1].TTest; // 0.53484

            // and also extract confidence intervals
            DoubleRange ci = mlra.Coefficients[0].Confidence; // [3.2616, 14.2193]
            #endregion


            Assert.AreEqual(3, coef.Length);
            Assert.AreEqual(8.7405051051757816, coef[0]);
            Assert.AreEqual(1.1198079243314365, coef[1], 1e-10);
            Assert.AreEqual(-19.604474518407862, coef[2], 1e-10);
            Assert.IsFalse(coef.HasNaN());

            Assert.AreEqual(2.375916659234715, stde[0], 1e-10);
            Assert.AreEqual(1.7268508921418664, stde[1], 1e-10);
            Assert.AreEqual(30.989640986710953, stde[2], 1e-10);
            Assert.IsFalse(coef.HasNaN());

            Assert.AreEqual(0.62879941171298936, rsquared, 1e-10);

            Assert.AreEqual(0.99999999999999822, ztest.PValue, 1e-10);
            Assert.AreEqual(0.018986050133298293, ftest.PValue, 1e-10);

            Assert.AreEqual(0.0062299844256985537, ttest0.PValue, 1e-10);
            Assert.AreEqual(0.53484850318449118, ttest1.PValue, 1e-14);
            Assert.IsFalse(Double.IsNaN(ttest1.PValue));

            Assert.AreEqual(3.2616314640800566, ci.Min, 1e-10);
            Assert.AreEqual(14.219378746271506, ci.Max, 1e-10);
        }

        [Test]
        public void RegressTest7()
        {
            double[][] example2 =
            {
                new double[] { -0.47, 1.16, -1.25 },
                new double[] {  0.55, 1.15, -0.78 },
                new double[] {  1.38, 0.63, -0.84 },
                new double[] {  0.99, 0.63, -0.81 },
                new double[] {  1.72, 0.62, -1.59 },
                new double[] {  1.05, 0.62, -1.05 },
                new double[] { -0.51, 0.62, -0.98 },
                new double[] {  1.83, 0.61,  0.86 },
                new double[] {  1.16, 0.61,  0.15 },
                new double[] {  0.59, 0.61, -0.28 },
                new double[] {  0.40, 0.60, -0.30 },
                new double[] {  0.48, 0.60, -0.41 },
                new double[] {  1.28, 0.53, -0.31 },
                new double[] {  0.36, 0.53, -0.41 },
                new double[] {  0.93, 0.16, -0.19 },
                new double[] { -0.61, 0.16, -0.32 },
                new double[] { -0.58, 0.16, -0.01 },
                new double[] {  0.53, 0.16, -0.13 },
                new double[] {  1.48, 0.16,  1.12 },
                new double[] { -0.34, 0.15, -0.10 },
                new double[] {  0.81, 0.15,  0.14 },
                new double[] {  0.85, 0.15, -0.02 },
                new double[] {  0.69, 0.15, -0.16 },
                new double[] {  0.39, 0.15, -0.33 },
                new double[] {  0.70, 0.00,  2.00 },
                new double[] {  0.25, 0.00, -0.01 },
                new double[] { -0.96, 0.85, -0.19 },
                new double[] {  1.04, 0.84,  0.35 },
                new double[] {  0.30, 0.83,  0.05 },
                new double[] {  0.28, 0.83,  0.84 },
                new double[] {  0.18, 0.82,  0.06 },
                new double[] {  0.49, 0.81,  0.41 },
                new double[] {  0.40, 0.81,  0.50 },
                new double[] {  0.41, 0.80,  0.00 },
                new double[] {  0.06, 0.79,  0.39 },
                new double[] {  0.55, 0.79,  0,55 },
            };


            double[][] inputs = example2.GetColumns(new[] { 1, 2 });
            double[] outputs = example2.GetColumn(0);

            bool thrown = false;

            MultipleLinearRegressionAnalysis target;

            try
            {
                target = new MultipleLinearRegressionAnalysis(inputs, outputs, new string[0], "Test", false);
            }
            catch (ArgumentException) { thrown = true; }

            Assert.IsTrue(thrown);

            target = new MultipleLinearRegressionAnalysis(inputs, outputs, new string[2], "Test", false);

            target.Compute();

            Assert.AreEqual(2, target.NumberOfInputs);
            Assert.AreEqual(1, target.NumberOfOutputs);

            Assert.AreEqual(target.Array, inputs);
            Assert.AreEqual(target.Outputs, outputs);

            Assert.AreEqual(-0.19371930561139417, target.RSquared, 1e-5);
            Assert.AreEqual(-0.26606593019390279, target.RSquareAdjusted, 1e-5);

            Assert.AreEqual(2, target.Table[0].DegreesOfFreedom);
            Assert.AreEqual(33, target.Table[1].DegreesOfFreedom);
            Assert.AreEqual(35, target.Table[2].DegreesOfFreedom);

            Assert.AreEqual(-2.9165797494934651, target.Table[0].SumOfSquares, 1e-10);
            Assert.AreEqual(17.972279749493463, target.Table[1].SumOfSquares, 1e-10);
            Assert.AreEqual(15.055699999999998, target.Table[2].SumOfSquares, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Table[0].SumOfSquares));
            Assert.IsFalse(Double.IsNaN(target.Table[1].SumOfSquares));
            Assert.IsFalse(Double.IsNaN(target.Table[2].SumOfSquares));

            Assert.AreEqual(-1.4582898747467326, target.Table[0].MeanSquares, 1e-10);
            Assert.AreEqual(0.54461453786343827, target.Table[1].MeanSquares, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Table[0].MeanSquares));
            Assert.IsFalse(Double.IsNaN(target.Table[1].MeanSquares));

            Assert.AreEqual(-2.6776550630978524, target.Table[0].Statistic.Value, 1e-10);
            Assert.AreEqual(1, target.Table[0].Significance.PValue, 1e-16);
            Assert.IsFalse(Double.IsNaN(target.Table[0].Significance.PValue));

            Assert.AreEqual(0.72195200211671728, target.Coefficients[0].Value, 1e-10);
            Assert.AreEqual(0.15872233321508125, target.Coefficients[1].Value, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Coefficients[0].Value));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[1].Value));

            Assert.IsFalse(target.Coefficients[0].IsIntercept);
            Assert.IsFalse(target.Coefficients[1].IsIntercept);

            Assert.AreEqual(0.20506051379737225, target.Coefficients[0].StandardError, 1e-10);
            Assert.AreEqual(0.18842330299464302, target.Coefficients[1].StandardError, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Coefficients[0].StandardError));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[1].StandardError));

            Assert.AreEqual(3.5206778172325479, target.Coefficients[0].TTest.Statistic, 1e-10);
            Assert.AreEqual(0.84237103740609942, target.Coefficients[1].TTest.Statistic, 1e-10);

            Assert.IsFalse(Double.IsNaN(target.Coefficients[0].TTest.Statistic));
            Assert.IsFalse(Double.IsNaN(target.Coefficients[1].TTest.Statistic));

            DoubleRange range;

            range = target.Coefficients[0].TTest.GetConfidenceInterval(0.9);
            Assert.AreEqual(0.37491572761667513, range.Min, 1e-10);
            Assert.AreEqual(1.0689882766167593, range.Max, 1e-10);

            range = target.Coefficients[1].TTest.GetConfidenceInterval(0.9);
            Assert.AreEqual(-0.16015778606945111, range.Min, 1e-10);
            Assert.AreEqual(0.47760245249961364, range.Max, 1e-10);


            MultipleLinearRegression mlr = new MultipleLinearRegression(2, false);
            mlr.Regress(inputs, outputs);

            Assert.AreEqual(2, target.NumberOfInputs);
            Assert.AreEqual(1, target.NumberOfOutputs);

            double[] actual = target.Transform(inputs);
            double[] expected = mlr.Transform(inputs);
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-8));

            double r2 = mlr.CoefficientOfDetermination(inputs, outputs, false);
            double r2a = mlr.CoefficientOfDetermination(inputs, outputs, true);

            Assert.AreEqual(r2, target.RSquared, 1e-6);
            Assert.AreEqual(r2a, target.RSquareAdjusted, 1e-6);
        }
    }
}
