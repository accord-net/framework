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
    using Accord.Statistics.Analysis;
    using NUnit.Framework;
    using System;
    using AForge;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Math;
    using Accord.Statistics.Testing;
    using System.Data;
    using Accord.Statistics.Filters;

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

            // We can use the analysis to predict an output for a sample
            double y = mlra.Regression.Transform(new double[] { 10, 15 });

            // We can also obtain confidence intervals for the prediction:
            DoubleRange pci = mlra.GetConfidenceInterval(new double[] { 10, 15 });

            // and also prediction intervals for the same prediction:
            DoubleRange ppi = mlra.GetPredictionInterval(new double[] { 10, 15 });
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




            double[][] im = mlra.InformationMatrix;
            double mse = regression.GetStandardError(inputs, output);
            DoubleRange epci = regression.GetConfidenceInterval(new double[] { 10, 15 }, mse, inputs.Length, im);

            Assert.AreEqual(epci.Min, pci.Min, 1e-10);
            Assert.AreEqual(epci.Max, pci.Max, 1e-10);

            Assert.AreEqual(55.27840511658215, pci.Min, 1e-10);
            Assert.AreEqual(113.91698568006086, pci.Max, 1e-10);

            Assert.AreEqual(28.783074454641557, ppi.Min, 1e-10);
            Assert.AreEqual(140.41231634200145, ppi.Max, 1e-10);
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

#if !NO_DATA_TABLE
        [Test]
        public void gh_937()
        {
            #region doc_learn_database
            // Note: this example uses a System.Data.DataTable to represent input data,
            // but note that this is not required. The data could have been represented
            // as jagged double matrices (double[][]) directly.

            // If you have to handle heterogeneus data in your application, such as user records
            // in a database, this data is best represented within the framework using a .NET's 
            // DataTable object. In order to try to learn a classification or regression model
            // using this datatable, first we will need to convert the table into a representation
            // that the machine learning model can understand. Such representation is quite often,
            // a matrix of doubles (double[][]).
            var data = new DataTable("Customer Revenue Example");

            data.Columns.Add("Day", "CustomerId", "Time (hour)", "Weather", "Revenue");
            data.Rows.Add("D1", 0, 8, "Sunny", 101.2);
            data.Rows.Add("D2", 1, 10, "Sunny", 24.1);
            data.Rows.Add("D3", 2, 10, "Rain", 107);
            data.Rows.Add("D4", 3, 16, "Rain", 223);
            data.Rows.Add("D5", 4, 15, "Rain", 1);
            data.Rows.Add("D6", 5, 20, "Rain", 42);
            data.Rows.Add("D7", 6, 12, "Cloudy", 123);
            data.Rows.Add("D8", 7, 12, "Sunny", 64);

            // One way to perform this conversion is by using a Codification filter. The Codification
            // filter can take care of converting variables that actually denote symbols (i.e. the 
            // weather in the example above) into representations that make more sense given the assumption
            // of a real vector-based classifier.

            // Create a codification codebook
            var codebook = new Codification()
            {
                { "Weather", CodificationVariable.Categorical },
                { "Time (hour)", CodificationVariable.Continuous },
                { "Revenue", CodificationVariable.Continuous },
            };

            // Learn from the data
            codebook.Learn(data);

            // Now, we will use the codebook to transform the DataTable into double[][] vectors. Due
            // the way the conversion works, we can end up with more columns in your output vectors
            // than the ones started with. If you would like more details about what those columns
            // represent, you can pass then as 'out' parameters in the methods that follow below.
            string[] inputNames;  // (note: if you do not want to run this example yourself, you 
            string outputName;    // can see below the new variable names that will be generated)

            // Now, we can translate our training data into integer symbols using our codebook:
            double[][] inputs = codebook.Apply(data, "Weather", "Time (hour)").ToJagged(out inputNames);
            double[] outputs = codebook.Apply(data, "Revenue").ToVector(out outputName);
            // (note: the Apply method transform a DataTable into another DataTable containing the codified 
            //  variables. The ToJagged and ToVector methods are then used to transform those tables into
            //  double[][] matrices and double[] vectors, respectively.

            // If we would like to learn a linear regression model for this data, there are two possible
            // ways depending on which aspect of the linear regression we are interested the most. If we
            // are interested in interpreting the linear regression, performing hypothesis tests with the
            // coefficients and performing an actual _linear regression analysis_, then we can use the
            // MultipleLinearRegressionAnalysis class for this. If however we are only interested in using
            // the learned model directly to predict new values for the dataset, then we could be using the
            // MultipleLinearRegression and OrdinaryLeastSquares classes directly instead. 

            // This example deals with the former case. For the later, please see the documentation page
            // for the MultipleLinearRegression class.

            // We can create a new multiple linear analysis for the variables
            var mlra = new MultipleLinearRegressionAnalysis(intercept: true)
            {
                // We can also inform the names of the new variables that have been created by the
                // codification filter. Those can help in the visualizing the analysis once it is 
                // data-bound to a visual control such a Windows.Forms.DataGridView or WPF DataGrid:

                Inputs = inputNames, // will be { "Weather: Sunny", "Weather: Rain, "Weather: Cloudy", "Time (hours)" }
                Output = outputName  // will be "Revenue"
            };

            // To overcome linear dependency errors
            mlra.OrdinaryLeastSquares.IsRobust = true;

            // Compute the analysis and obtain the estimated regression
            MultipleLinearRegression regression = mlra.Learn(inputs, outputs);

            // And then predict the label using
            double predicted = mlra.Transform(inputs[0]); // result will be ~72.3

            // Because we opted for doing a MultipleLinearRegressionAnalysis instead of a simple
            // linear regression, we will have further information about the regression available:
            int inputCount = mlra.NumberOfInputs;   // should be 4
            int outputCount = mlra.NumberOfOutputs; // should be 1
            double r2 = mlra.RSquared;              // should be 0.12801838425195311
            AnovaSourceCollection a = mlra.Table;   // ANOVA table (bind to a visual control for quick inspection)
            double[][] h = mlra.InformationMatrix;  // should contain Fisher's information matrix for the problem
            ZTest z = mlra.ZTest;                   // should be 0 (p=0.999, non-significant)
            #endregion

            Assert.AreEqual(72.279574468085144d, predicted, 1e-8);
            Assert.AreEqual(4, inputCount, 1e-8);
            Assert.AreEqual(1, outputCount, 1e-8);
            Assert.AreEqual(0.12801838425195311, r2, 1e-8);
            Assert.AreEqual(0.11010987669344097, a[0].Statistic, 1e-8);

            string str = h.ToCSharp();
            double[][] expectedH = new double[][] 
            {
                new double[] { 0.442293243337911, -0.069833718526197, -0.228692384542512, -0.0141758263063635, 0.143767140269202 },
                new double[] { -0.0698337185261971, 0.717811616891116, -0.112258662892007, -0.0655549422852099, 0.535719235472913 },
                new double[] { -0.228692384542512, -0.112258662892007, 0.717434922237013, -0.0232803210243207, 0.376483874802496 },
                new double[] { -0.0141758263063635, -0.0655549422852099, -0.0232803210243207, 0.0370082984668314, -0.103011089615894 },
                new double[] { 0.143767140269202, 0.535719235472913, 0.376483874802496, -0.103011089615894, 1.05597025054461 }
            };

            Assert.IsTrue(expectedH.IsEqual(h, 1e-8));
            Assert.AreEqual(0, z.Statistic, 1e-8);
            Assert.AreEqual(1, z.PValue, 1e-8);
        }
#endif
    }
}
