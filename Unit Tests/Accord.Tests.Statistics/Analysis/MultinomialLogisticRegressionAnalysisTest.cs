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
    using Accord.DataSets;
    using Accord.IO;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Filters;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System.Data;

    [TestFixture]
    public class MultinomialLogisticRegressionAnalysisTest
    {

#if !NO_DATA_TABLE
        [Test]
        public void AnalyzeExample1()
        {
            // Note: results perfectly match the example at
            // https://web.archive.org/web/20170210050820/http://www.ats.ucla.edu/stat/stata/dae/mlogit.htm

            CsvReader reader = CsvReader.FromText(Properties.Resources.hsbdemo, hasHeaders: true);

            var table = reader.ToTable();

            var codification = new Codification(table);
            codification["ses"].VariableType = CodificationVariable.CategoricalWithBaseline;
            codification["prog"].VariableType = CodificationVariable.Categorical;
            codification["write"].VariableType = CodificationVariable.Continuous;
            codification["ses"].Remap("low", 0);
            codification["ses"].Remap("middle", 1);
            codification["prog"].Remap("academic", 0);
            codification["prog"].Remap("general", 1);

            var inputs = codification.Apply(table, "write", "ses");
            var output = codification.Apply(table, "prog");


            // Get inputs
            string[] inputNames;
            var inputsData = inputs.ToArray(out inputNames);

            // Get outputs
            string[] outputNames;
            var outputData = output.ToArray(out outputNames);


            var analysis = new MultinomialLogisticRegressionAnalysis(inputsData, outputData, inputNames, outputNames);

            analysis.Compute();

            testmlr(analysis);
        }

        private static void testmlr(MultinomialLogisticRegressionAnalysis analysis)
        {
            Assert.AreEqual(9, analysis.Coefficients.Count);
            Assert.AreEqual(3, analysis.NumberOfInputs);
            Assert.AreEqual(3, analysis.NumberOfOutputs);

            Assert.AreEqual(new[] { "write", "ses: middle", "ses: high" }, analysis.InputNames);
            Assert.AreEqual(new[] { "prog: academic", "prog: general", "prog: vocation" }, analysis.OutputNames);

            int i = 0;

            Assert.AreEqual("(baseline)", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: academic", analysis.Coefficients[i].Class);
            Assert.AreEqual(0, analysis.Coefficients[i].Value);

            i++;
            Assert.AreEqual("Intercept", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(2.8521777575247067, analysis.Coefficients[i].Value, 1e-4);

            i++;
            Assert.AreEqual("write", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.057928272352042558, analysis.Coefficients[i].Value, 1e-4);

            i++;
            Assert.AreEqual("ses: middle", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.533293368378012, analysis.Coefficients[i].Value, 1e-4);

            i++;
            Assert.AreEqual("ses: high", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(-1.1628385060528876, analysis.Coefficients[i].Value, 1e-4);

            i++;
            Assert.AreEqual("Intercept", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(5.21813357698422, analysis.Coefficients[i].Value, 1e-3);

            i++;
            Assert.AreEqual("write", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.11360118666081663, analysis.Coefficients[i].Value, 1e-4);

            i++;
            Assert.AreEqual("ses: middle", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(0.29138704135836724, analysis.Coefficients[i].Value, 1e-4);

            i++;
            Assert.AreEqual("ses: high", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.98263693874810032, analysis.Coefficients[i].Value, 1e-3);

            double[][] coef = analysis.CoefficientValues;
            string str = coef.ToCSharp();

            double[][] expectedCoef = new double[][]
            {
                new double[] { 2.85217775752471, -0.0579282723520426, -0.533293368378012, -1.16283850605289 },
                new double[] { 5.21813357698422, -0.113601186660817, 0.291387041358367, -0.9826369387481 }
            };

            Assert.IsTrue(expectedCoef.IsEqual(coef, 1e-3));
        }

        [Test]
        public void learn_test()
        {
            // http://www.ats.ucla.edu/stat/stata/dae/mlogit.htm
            #region doc_learn_1
            // This example downloads an example dataset from the web and learns a multinomial logistic 
            // regression on it. However, please keep in mind that the Multinomial Logistic Regression 
            // can also work without many of the elements that will be shown below, like the codebook, 
            // DataTables, and a CsvReader. 

            // Let's download an example dataset from the web to learn a multinomial logistic regression:
            CsvReader reader = CsvReader.FromUrl("https://raw.githubusercontent.com/rlowrance/re/master/hsbdemo.csv", hasHeaders: true);

            // Let's read the CSV into a DataTable. As mentioned above, this step
            // can help, but is not necessarily required for learning a the model:
            DataTable table = reader.ToTable();

            // We will learn a MLR regression between the following input and output fields of this table:
            string[] inputNames = new[] { "write", "ses" };
            string[] outputNames = new[] { "prog" };

            // Now let's create a codification codebook to convert the string fields in the data 
            // into integer symbols. This is required because the MLR model can only learn from 
            // numeric data, so strings have to be transformed first. We can force a particular
            // interpretation for those columns if needed, as shown in the initializer below:
            var codification = new Codification()
            {
                { "write", CodificationVariable.Continuous },
                { "ses", CodificationVariable.CategoricalWithBaseline, new[] { "low", "middle", "high" } },
                { "prog", CodificationVariable.Categorical, new[] { "academic", "general" } },
            };

            // Learn the codification
            codification.Learn(table);

            // Now, transform symbols into a vector representation, growing the number of inputs:
            double[][] x = codification.Transform(table, inputNames, out inputNames).ToDouble();
            double[][] y = codification.Transform(table, outputNames, out outputNames).ToDouble();

            // Create a new Multinomial Logistic Regression Analysis:
            var analysis = new MultinomialLogisticRegressionAnalysis()
            {
                InputNames = inputNames,
                OutputNames = outputNames,
            };

            // Learn the regression from the input and output pairs:
            MultinomialLogisticRegression regression = analysis.Learn(x, y);

            // Let's retrieve some information about what we just learned:
            int coefficients = analysis.Coefficients.Count; // should be 9
            int numberOfInputs = analysis.NumberOfInputs;   // should be 3
            int numberOfOutputs = analysis.NumberOfOutputs; // should be 3

            inputNames = analysis.InputNames; // should be "write", "ses: middle", "ses: high"
            outputNames = analysis.OutputNames; // should be "prog: academic", "prog: general", "prog: vocation"

            // The regression is best visualized when it is data-bound to a 
            // Windows.Forms DataGridView or WPF DataGrid. You can get the
            // values for all different coefficients and discrete values:

            // DataGridBox.Show(regression.Coefficients); // uncomment this line

            // You can get the matrix of coefficients:
            double[][] coef = analysis.CoefficientValues;

            // Should be equal to:
            double[][] expectedCoef = new double[][]
            {
                new double[] { 2.85217775752471, -0.0579282723520426, -0.533293368378012, -1.16283850605289 },
                new double[] { 5.21813357698422, -0.113601186660817, 0.291387041358367, -0.9826369387481 }
            };

            // And their associated standard errors:
            double[][] stdErr = analysis.StandardErrors;

            // Should be equal to:
            double[][] expectedErr = new double[][]
            {
                new double[] { -2.02458003380033, -0.339533576505471, -1.164084923948, -0.520961533343425, 0.0556314901718 },
                new double[] { -3.73971589217449, -1.47672790071382, -1.76795568348094, -0.495032307980058, 0.113563519656386 }
            };

            // We can also get statistics and hypothesis tests:
            WaldTest[][] wald = analysis.WaldTests;        // should all have p < 0.05
            ChiSquareTest chiSquare = analysis.ChiSquare;  // should be p=1.06300120956871E-08
            double logLikelihood = analysis.LogLikelihood; // should be -179.98173272217591

            // You can use the regression to predict the values:
            int[] pred = regression.Transform(x);

            // And get the accuracy of the prediction if needed:
            var cm = GeneralConfusionMatrix.Estimate(regression, x, y.ArgMax(dimension: 1));

            double acc = cm.Accuracy; // should be 0.61
            double kappa = cm.Kappa;  // should be 0.2993487536492252
            #endregion


            Assert.AreEqual(9, coefficients);
            Assert.AreEqual(3, numberOfInputs);
            Assert.AreEqual(3, numberOfOutputs);

            Assert.AreEqual(new[] { "write", "ses: middle", "ses: high" }, inputNames);
            Assert.AreEqual(new[] { "prog: academic", "prog: general", "prog: vocation" }, outputNames);

            Assert.AreEqual(0.61, acc, 1e-10);
            Assert.AreEqual(0.2993487536492252, kappa, 1e-10);
            Assert.AreEqual(1.06300120956871E-08, chiSquare.PValue, 1e-8);
            Assert.AreEqual(-179.98172637136295, logLikelihood, 1e-8);

            testmlr(analysis);
        }
#endif

        [Test]
        public void ComputeTest1()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample1(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            int inputCount = 2;
            int outputCount = 3;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;
            Assert.AreEqual(inputCount, mlr.Inputs);
            Assert.AreEqual(outputCount, mlr.Categories);

            Assert.AreEqual(inputCount, analysis.Inputs.Length);
            Assert.AreEqual(outputCount, analysis.OutputNames.Length);

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            analysis.Compute();

            Assert.AreEqual(outputCount - 1, analysis.CoefficientValues.Length);
            Assert.AreEqual(outputCount - 1, analysis.StandardErrors.Length);

            Assert.AreEqual(outputCount - 1, analysis.WaldTests.Length);
            Assert.AreEqual(outputCount - 1, analysis.Confidences.Length);

            for (int i = 0; i < analysis.CoefficientValues.Length; i++)
            {
                Assert.AreEqual(coeffCount, analysis.CoefficientValues[i].Length);
                Assert.AreEqual(coeffCount, analysis.StandardErrors[i].Length);

                Assert.AreEqual(coeffCount, analysis.WaldTests[i].Length);
                Assert.AreEqual(coeffCount, analysis.Confidences[i].Length);

                for (int j = 0; j < analysis.CoefficientValues[i].Length; j++)
                {
                    Assert.IsFalse(double.IsNaN(analysis.CoefficientValues[i][j]));
                    Assert.IsFalse(double.IsNaN(analysis.StandardErrors[i][j]));
                }
            }

            var coefficients = analysis.CoefficientValues;

            // brand 2
            Assert.AreEqual(-11.774655, coefficients[0][0], 1e-4); // intercept
            Assert.AreEqual(0.523814, coefficients[0][1], 1e-4); // female
            Assert.AreEqual(0.368206, coefficients[0][2], 1e-4); // age

            // brand 3
            Assert.AreEqual(-22.721396, coefficients[1][0], 1e-4); // intercept
            Assert.AreEqual(0.465941, coefficients[1][1], 1e-4); // female
            Assert.AreEqual(0.685908, coefficients[1][2], 1e-4); // age

            var standard = analysis.StandardErrors;

            // Using the lower-bound approximation
            Assert.AreEqual(1.047378039787443, standard[0][0], 1e-6);
            Assert.AreEqual(0.153150051082552, standard[0][1], 1e-6);
            Assert.AreEqual(0.031640507386863, standard[0][2], 1e-6);

            Assert.AreEqual(1.047378039787443, standard[1][0], 1e-6);
            Assert.AreEqual(0.153150051082552, standard[1][1], 1e-6);
            Assert.AreEqual(0.031640507386863, standard[1][2], 1e-6);

            Assert.AreEqual(-702.97, analysis.LogLikelihood, 1e-2);
            Assert.AreEqual(185.85, analysis.ChiSquare.Statistic, 1e-2);
            Assert.AreEqual(1405.9414080469473, analysis.Deviance, 1e-10);

            var wald = analysis.WaldTests;
            Assert.AreEqual(-11.241995503283842, wald[0][0].Statistic, 1e-4);
            Assert.AreEqual(3.4202662152119889, wald[0][1].Statistic, 1e-4);
            Assert.AreEqual(11.637150673342207, wald[0][2].Statistic, 1e-4);

            Assert.AreEqual(-21.693553825772664, wald[1][0].Statistic, 1e-4);
            Assert.AreEqual(3.0423802097069097, wald[1][1].Statistic, 1e-4);
            Assert.AreEqual(21.678124991086548, wald[1][2].Statistic, 1e-4);
        }

        [Test]
        public void learn_test_2()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample1(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis();

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            var regression = analysis.Learn(inputs, outputs);

            int inputCount = 2;
            int outputCount = 3;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;
            Assert.AreEqual(inputCount, mlr.Inputs);
            Assert.AreEqual(outputCount, mlr.Categories);

            Assert.AreEqual(inputCount, analysis.Inputs.Length);
            Assert.AreEqual(outputCount, analysis.OutputNames.Length);

            Assert.AreEqual(outputCount - 1, analysis.CoefficientValues.Length);
            Assert.AreEqual(outputCount - 1, analysis.StandardErrors.Length);

            Assert.AreEqual(outputCount - 1, analysis.WaldTests.Length);
            Assert.AreEqual(outputCount - 1, analysis.Confidences.Length);

            for (int i = 0; i < analysis.CoefficientValues.Length; i++)
            {
                Assert.AreEqual(coeffCount, analysis.CoefficientValues[i].Length);
                Assert.AreEqual(coeffCount, analysis.StandardErrors[i].Length);

                Assert.AreEqual(coeffCount, analysis.WaldTests[i].Length);
                Assert.AreEqual(coeffCount, analysis.Confidences[i].Length);
            }

            var coefficients = analysis.CoefficientValues;

            // brand 2
            Assert.AreEqual(-11.774655, coefficients[0][0], 1e-3); // intercept
            Assert.AreEqual(0.523814, coefficients[0][1], 1e-3); // female
            Assert.AreEqual(0.368206, coefficients[0][2], 1e-3); // age

            // brand 3
            Assert.AreEqual(-22.721396, coefficients[1][0], 1e-3); // intercept
            Assert.AreEqual(0.465941, coefficients[1][1], 1e-3); // female
            Assert.AreEqual(0.685908, coefficients[1][2], 1e-3); // age

            var standard = analysis.StandardErrors;

            // Using the lower-bound approximation
            Assert.AreEqual(1.047378039787443, standard[0][0], 1e-6);
            Assert.AreEqual(0.153150051082552, standard[0][1], 1e-6);
            Assert.AreEqual(0.031640507386863, standard[0][2], 1e-6);

            Assert.AreEqual(1.047378039787443, standard[1][0], 1e-6);
            Assert.AreEqual(0.153150051082552, standard[1][1], 1e-6);
            Assert.AreEqual(0.031640507386863, standard[1][2], 1e-6);

            Assert.AreEqual(-702.97, analysis.LogLikelihood, 1e-2);
            Assert.AreEqual(185.85, analysis.ChiSquare.Statistic, 1e-2);
            Assert.AreEqual(1405.9414080469473, analysis.Deviance, 1e-4);

            var wald = analysis.WaldTests;
            Assert.AreEqual(-11.241995503283842, wald[0][0].Statistic, 1e-3);
            Assert.AreEqual(3.4202662152119889, wald[0][1].Statistic, 1e-4);
            Assert.AreEqual(11.637150673342207, wald[0][2].Statistic, 1e-3);

            Assert.AreEqual(-21.693553825772664, wald[1][0].Statistic, 1e-3);
            Assert.AreEqual(3.0423802097069097, wald[1][1].Statistic, 1e-4);
            Assert.AreEqual(21.678124991086548, wald[1][2].Statistic, 1e-3);
        }

        [Test]
        public void ComputeTest2()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample2(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            int inputCount = 5;
            int outputCount = 3;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;
            Assert.AreEqual(inputCount, mlr.Inputs);
            Assert.AreEqual(outputCount, mlr.Categories);

            Assert.AreEqual(inputCount, analysis.Inputs.Length);
            Assert.AreEqual(outputCount, analysis.OutputNames.Length);

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            analysis.Compute();

            Assert.AreEqual(outputCount - 1, analysis.CoefficientValues.Length);
            Assert.AreEqual(outputCount - 1, analysis.StandardErrors.Length);

            Assert.AreEqual(outputCount - 1, analysis.WaldTests.Length);
            Assert.AreEqual(outputCount - 1, analysis.Confidences.Length);

            for (int i = 0; i < analysis.CoefficientValues.Length; i++)
            {
                Assert.AreEqual(coeffCount, analysis.CoefficientValues[i].Length);
                Assert.AreEqual(coeffCount, analysis.StandardErrors[i].Length);

                Assert.AreEqual(coeffCount, analysis.WaldTests[i].Length);
                Assert.AreEqual(coeffCount, analysis.Confidences[i].Length);

                for (int j = 0; j < analysis.CoefficientValues[i].Length; j++)
                {
                    Assert.IsFalse(double.IsNaN(analysis.CoefficientValues[i][j]));
                    Assert.IsFalse(double.IsNaN(analysis.StandardErrors[i][j]));
                }
            }

            var coefficients = analysis.CoefficientValues;

            // brand 2
            Assert.AreEqual(-11.774655, coefficients[0][0], 1e-3); // intercept
            Assert.AreEqual(0.523814, coefficients[0][1], 1e-3); // female
            Assert.AreEqual(0.368206, coefficients[0][2], 1e-3); // age

            // brand 3
            Assert.AreEqual(-22.721396, coefficients[1][0], 1e-3); // intercept
            Assert.AreEqual(0.465941, coefficients[1][1], 1e-3); // female
            Assert.AreEqual(0.685908, coefficients[1][2], 1e-3); // age

            var standard = analysis.StandardErrors;

            Assert.AreEqual(-702.97, analysis.LogLikelihood, 1e-2);
            Assert.AreEqual(185.85, analysis.ChiSquare.Statistic, 1e-2);
            Assert.AreEqual(1405.9414080469473, analysis.Deviance, 1e-5);
        }

        [Test]
        public void CoefficientsTest1()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample2(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            int inputCount = 5;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            analysis.Compute();

            foreach (var coefficient in analysis.Coefficients)
            {
                Assert.IsNotNull(coefficient.Analysis);
                Assert.IsNotNull(coefficient.Confidence);
                Assert.IsNotNull(coefficient.ConfidenceLower);
                Assert.IsNotNull(coefficient.ConfidenceUpper);
                Assert.IsNotNull(coefficient.Name);
                Assert.IsNotNull(coefficient.Class);
                Assert.IsNotNull(coefficient.StandardError);
                Assert.IsNotNull(coefficient.Value);
            }


            Assert.AreEqual(13, analysis.Coefficients.Count);
            Assert.AreEqual(2, analysis.CoefficientValues.Length);

            var class1 = analysis.CoefficientValues[0];
            Assert.AreEqual(-11.774547061739975, class1[0], 1e-10);
            Assert.AreEqual(0.523813075806107, class1[1], 1e-10);
            Assert.AreEqual(0.36820307277024716, class1[2], 1e-10);
            Assert.AreEqual(0, class1[3], 1e-6);
            Assert.AreEqual(0, class1[4], 1e-6);
            Assert.AreEqual(0, class1[5], 1e-6);

            var class2 = analysis.CoefficientValues[1];
            Assert.AreEqual(-22.721272157115514, class2[0], 1e-10);
            Assert.AreEqual(0.46593949381162203, class2[1], 1e-10);
            Assert.AreEqual(0.68590438098052586, class2[2], 1e-10);
            Assert.AreEqual(0, class2[3], 1e-6);
            Assert.AreEqual(0, class2[4], 1e-6);
            Assert.AreEqual(0, class2[5], 1e-6);
        }

        [Test]
        public void learn_test_3()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample2(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis();

            int inputCount = 5;
            int coeffCount = inputCount + 1;

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            var mlr = analysis.Learn(inputs, outputs);

            foreach (var coefficient in analysis.Coefficients)
            {
                Assert.IsNotNull(coefficient.Analysis);
                Assert.IsNotNull(coefficient.Confidence);
                Assert.IsNotNull(coefficient.ConfidenceLower);
                Assert.IsNotNull(coefficient.ConfidenceUpper);
                Assert.IsNotNull(coefficient.Name);
                Assert.IsNotNull(coefficient.Class);
                Assert.IsNotNull(coefficient.StandardError);
                Assert.IsNotNull(coefficient.Value);
            }


            Assert.AreEqual(13, analysis.Coefficients.Count);
            Assert.AreEqual(2, analysis.CoefficientValues.Length);

            var class1 = analysis.CoefficientValues[0];
            Assert.AreEqual(-11.774547061739975, class1[0], 1e-3);
            Assert.AreEqual(0.523813075806107, class1[1], 1e-4);
            Assert.AreEqual(0.36820307277024716, class1[2], 1e-4);
            Assert.AreEqual(0, class1[3], 1e-6);
            Assert.AreEqual(0, class1[4], 1e-6);
            Assert.AreEqual(0, class1[5], 1e-6);

            var class2 = analysis.CoefficientValues[1];
            Assert.AreEqual(-22.721272157115514, class2[0], 1e-3);
            Assert.AreEqual(0.46593949381162203, class2[1], 1e-4);
            Assert.AreEqual(0.68590438098052586, class2[2], 1e-4);
            Assert.AreEqual(0, class2[3], 1e-6);
            Assert.AreEqual(0, class2[4], 1e-6);
            Assert.AreEqual(0, class2[5], 1e-6);
        }

        [Test]
        public void learn_test_4()
        {
            #region doc_learn_2
            // This example shows how to learn a multinomial logistic regression
            // analysis in the famous Fisher's Iris dataset. It should serve to
            // demonstrate that this class does not really need to be used with
            // DataTables, Codification codebooks and other supplementary features.

            Iris iris = new Iris();

            // Load Fisher's Iris dataset:
            double[][] x = iris.Instances;
            int[] y = iris.ClassLabels;

            // Create a new Multinomial Logistic Regression Analysis:
            var analysis = new MultinomialLogisticRegressionAnalysis();

            // Note: we could have passed the class names from iris.ClassNames and 
            // variable names from iris.VariableNames during MLR instantiation as:
            //
            // var analysis = new MultinomialLogisticRegressionAnalysis()
            // {
            //     InputNames = iris.VariableNames,
            //     OutputNames = iris.ClassNames
            // };

            // However, this example is also intended to demonstrate that 
            // those are not required when learning a regression analysis.

            // Learn the regression from the input and output pairs:
            MultinomialLogisticRegression regression = analysis.Learn(x, y);

            // Let's retrieve some information about what we just learned:
            int coefficients = analysis.Coefficients.Count; // should be 11
            int numberOfInputs = analysis.NumberOfInputs;   // should be 4
            int numberOfOutputs = analysis.NumberOfOutputs; // should be 3

            string[] inputNames = analysis.InputNames; // should be "Input 1", "Input 2", "Input 3", "Input 4"
            string[] outputNames = analysis.OutputNames; // should be "Class 0", "class 1", "class 2"

            // The regression is best visualized when it is data-bound to a 
            // Windows.Forms DataGridView or WPF DataGrid. You can get the
            // values for all different coefficients and discrete values:

            // DataGridBox.Show(regression.Coefficients); // uncomment this line

            // You can get the matrix of coefficients:
            double[][] coef = analysis.CoefficientValues;

            // Should be equal to:
            double[][] expectedCoef = new double[][]
            {
                new double[] { 2.85217775752471, -0.0579282723520426, -0.533293368378012, -1.16283850605289 },
                new double[] { 5.21813357698422, -0.113601186660817, 0.291387041358367, -0.9826369387481 }
            };

            // And their associated standard errors:
            double[][] stdErr = analysis.StandardErrors;

            // Should be equal to:
            double[][] expectedErr = new double[][]
            {
                new double[] { -2.02458003380033, -0.339533576505471, -1.164084923948, -0.520961533343425, 0.0556314901718 },
                new double[] { -3.73971589217449, -1.47672790071382, -1.76795568348094, -0.495032307980058, 0.113563519656386 }
            };

            // We can also get statistics and hypothesis tests:
            WaldTest[][] wald = analysis.WaldTests;        // should all have p < 0.05
            ChiSquareTest chiSquare = analysis.ChiSquare;  // should be p=0
            double logLikelihood = analysis.LogLikelihood; // should be -29.558338705646587

            // You can use the regression to predict the values:
            int[] pred = regression.Transform(x);

            // And get the accuracy of the prediction if needed:
            var cm = GeneralConfusionMatrix.Estimate(regression, x, y);

            double acc = cm.Accuracy; // should be 0.94666666666666666
            double kappa = cm.Kappa;  // should be 0.91999999999999982
            #endregion

            Assert.AreEqual(11, coefficients);
            Assert.AreEqual(4, numberOfInputs);
            Assert.AreEqual(3, numberOfOutputs);

            Assert.AreEqual(new[] { "Input 0", "Input 1", "Input 2", "Input 3" }, inputNames);
            Assert.AreEqual(new[] { "Class 0", "Class 1", "Class 2" }, outputNames);

            Assert.AreEqual(0.94666666666666666, acc, 1e-10);
            Assert.AreEqual(0.91999999999999982, kappa, 1e-10);
            Assert.AreEqual(7.8271969268290043E-54, chiSquare.PValue, 1e-8);
            Assert.AreEqual(-29.558338705646587, logLikelihood, 1e-8);
        }
    }
}
