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
    using Accord.Math;
    using Accord.Statistics.Models.Regression;

    [TestFixture]
    public class LogisticRegressionAnalysisTest
    {


        [Test]
        public void ComputeTest1()
        {
            double[][] inputs = training.Submatrix(null, 0, 3);
            double[] outputs = training.GetColumn(4);

            var regression = new LogisticRegressionAnalysis(inputs, outputs);

            bool converged = regression.Compute();
            Assert.IsTrue(converged);

            double[] actual = regression.Result;

            double[] expected = 
            {
                0.000012, 0.892611, 0.991369, 0.001513, 0.904055,
                0.001446, 0.998673, 0.001260, 0.629312, 0.004475,
                0.505362, 0.999791, 0.000050, 1.000000, 0.990362,
                0.985265, 1.000000, 1.000000, 0.001319, 0.000001,
                0.000001, 0.000050, 0.702488, 0.003049, 0.000046,
                0.000419, 0.026276, 0.036813, 0.000713, 0.001484,
                0.000008, 0.000009, 0.278950, 0.001402, 0.025764,
                0.002464, 0.000219, 0.007328, 0.000106, 0.002619,
                0.002913, 0.000002,
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-6);
        }

        [Test]
        public void learn1()
        {
            double[][] inputs = training.Submatrix(null, 0, 3);
            double[] outputs = training.GetColumn(4);

            var lra = new LogisticRegressionAnalysis()
            {
                ComputeInnerModels = true
            };

            var regression = lra.Learn(inputs, outputs);

            double[] actual = regression.Score(inputs);

            double[] expected = 
            {
                0.000012, 0.892611, 0.991369, 0.001513, 0.904055,
                0.001446, 0.998673, 0.001260, 0.629312, 0.004475,
                0.505362, 0.999791, 0.000050, 1.000000, 0.990362,
                0.985265, 1.000000, 1.000000, 0.001319, 0.000001,
                0.000001, 0.000050, 0.702488, 0.003049, 0.000046,
                0.000419, 0.026276, 0.036813, 0.000713, 0.001484,
                0.000008, 0.000009, 0.278950, 0.001402, 0.025764,
                0.002464, 0.000219, 0.007328, 0.000106, 0.002619,
                0.002913, 0.000002,
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-6);

            Assert.AreEqual(5, lra.LikelihoodRatioTests.Length);
            Assert.IsNull(lra.LikelihoodRatioTests[0]);
            Assert.AreEqual(0.99999995244237416, lra.LikelihoodRatioTests[1].PValue, 1e-7);
            Assert.AreEqual(0.99999993274336052, lra.LikelihoodRatioTests[2].PValue, 1e-7);
            Assert.AreEqual(0.99999992480479116, lra.LikelihoodRatioTests[3].PValue, 1e-7);
            Assert.AreEqual(0.0000000059730130622305527, lra.LikelihoodRatioTests[4].PValue, 1e-20);
        }

        [Test]
        public void ComputeTest2()
        {
            // Test instance 01
            double[][] trainInput =
            {
               new double[] { 1, 1 },
               new double[] { 0, 0 },
            };

            double[] trainOutput = { 1, 0 };
            double[] testInput = { 0, 0.2 };

            var target = new LogisticRegressionAnalysis(trainInput, trainOutput);

            target.Regularization = 1e-10;

            target.Compute();

            double[] coef = target.Coefficients.Apply(x => x.Value);
            Assert.AreEqual(coef[0], -19.360661491141897);
            Assert.AreEqual(coef[1], 19.702873967721807);
            Assert.AreEqual(coef[2], 19.702873967721807);

            double output = target.Regression.Compute(testInput);
            Assert.AreEqual(0, output, 1e-6);

            // Test instance 02
            trainInput = new double[][]
            {
                new double[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 0 },
                new double[] { 0, 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                new double[] { 1, 1, 0, 0, 1, 1, 0, 1, 1, 1 },
                new double[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 0 },
                new double[] { 0, 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                new double[] { 1, 1, 0, 0, 1, 1, 0, 1, 1, 1 },
            };

            trainOutput = new double[6] { 1, 1, 0, 0, 1, 1 };

            target = new LogisticRegressionAnalysis(trainInput, trainOutput);

            Assert.IsTrue(target.Compute());


            double[] actual = target.Regression.Compute(trainInput);
            //string str = actual.ToCSharp();
            double[] expected = { 0.500000000158903, 0.999999998410966, 0.500000000913694, 0.500000000158903, 0.999999998410966, 0.500000000913694 };
            Assert.IsTrue(actual.IsEqual(expected, 1e-6));

            coef = target.Coefficients.Apply(x => x.Value);
            //string str = coef.ToCSharp();
            expected = new double[] { 1.86680346470929, -3.87720719574071, 2.44120453079343, -0.574401066088034, 5.16960959435804, 2.44120453079343, -3.87720719574087, 5.16960959435804, 2.44120453079343, -3.87720719574087, 2.44120453079343 };
            Assert.IsTrue(coef.IsEqual(expected, 1e-6));
        }

        [Test]
        public void learn2()
        {
            // Test instance 01
            double[][] trainInput =
            {
               new double[] { 1, 1 },
               new double[] { 0, 0 },
            };

            double[] trainOutput = { 1, 0 };
            double[] testInput = { 0, 0.2 };

            var target = new LogisticRegressionAnalysis();

            target.Regularization = 1e-10;

            var regression = target.Learn(trainInput, trainOutput);
            Assert.AreSame(regression, target.Regression);

            double[] coef = target.Coefficients.Apply(x => x.Value);
            Assert.AreEqual(coef[0], -19.360661491141897, 1e-6);
            Assert.AreEqual(coef[1], 19.702873967721807, 1e-6);
            Assert.AreEqual(coef[2], 19.702873967721807, 1e-6);

            double output = target.Regression.Score(testInput);
            Assert.AreEqual(0, output, 1e-6);

            // Test instance 02
            trainInput = new double[][]
            {
                new double[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 0 },
                new double[] { 0, 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                new double[] { 1, 1, 0, 0, 1, 1, 0, 1, 1, 1 },
                new double[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 0 },
                new double[] { 0, 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                new double[] { 1, 1, 0, 0, 1, 1, 0, 1, 1, 1 },
            };

            trainOutput = new double[6] { 1, 1, 0, 0, 1, 1 };

            target = new LogisticRegressionAnalysis();

            regression = target.Learn(trainInput, trainOutput);

            double[] actual = regression.Score(trainInput);
            double[] expected = { 0.500000000158903, 0.999999998410966, 0.500000000913694, 0.500000000158903, 0.999999998410966, 0.500000000913694 };
            Assert.IsTrue(actual.IsEqual(expected, 1e-6));

            coef = target.Coefficients.Apply(x => x.Value);
            //string str = coef.ToCSharp();
            expected = new double[] { 1.86680346470929, -3.87720719574071, 2.44120453079343, -0.574401066088034, 5.16960959435804, 2.44120453079343, -3.87720719574087, 5.16960959435804, 2.44120453079343, -3.87720719574087, 2.44120453079343 };
            Assert.IsTrue(coef.IsEqual(expected, 1e-6));
        }

        [Test]
        public void ComputeTest3()
        {
            double[][] inputs = training.Submatrix(null, 0, 3);
            double[] outputs = training.GetColumn(4);

            LogisticRegressionAnalysis regression = new LogisticRegressionAnalysis(inputs, outputs);

            bool expected = false;
            regression.Iterations = 3;
            bool actual = regression.Compute();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ComputeTest4()
        {
            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (this is completely fictional data).

            double[][] inputs =
            {
                //            Age  Smoking
                new double[] { 55,    0   }, 
                new double[] { 28,    0   }, 
                new double[] { 65,    1   }, 
                new double[] { 46,    0   }, 
                new double[] { 86,    1   }, 
                new double[] { 56,    1   }, 
                new double[] { 85,    0   }, 
                new double[] { 33,    0   }, 
                new double[] { 21,    1   }, 
                new double[] { 42,    1   }, 
            };

            // Additionally, we also have information about whether
            // or not they those patients had lung cancer. The array
            // below gives 0 for those who did not, and 1 for those
            // who did.

            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };

            // Create a Logistic Regression analysis
            var regression = new LogisticRegressionAnalysis(inputs, output);

            regression.Regularization = 0;

            regression.Compute(); // compute the analysis.

            // Now we can show a summary of the analysis
            // Accord.Controls.DataGridBox.Show(regression.Coefficients);


            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at the
            // vector

            double[] coef = regression.CoefficientValues;

            // The first value refers to the model's intercept term. We
            // can also retrieve the odds ratios and standard errors:

            double[] odds = regression.OddsRatios;
            double[] stde = regression.StandardErrors;


            // Finally, we can also use the analysis to classify a new patient
            double y = regression.Regression.Compute(new double[] { 87, 1 });

            // For those inputs, the answer probability is approximately 75%.

            Assert.AreEqual(0.085627701183146374, odds[0], 1e-8);
            Assert.AreEqual(1.0208597029292648, odds[1], 1e-8);
            Assert.AreEqual(5.8584748981777919, odds[2], 1e-8);

            Assert.AreEqual(2.1590686019473897, stde[0], 1e-8);
            Assert.AreEqual(0.033790422321041035, stde[1], 1e-8);
            Assert.AreEqual(1.4729903935788211, stde[2], 1e-8);

            Assert.AreEqual(0.75143272858389798, y, 1e-8);
        }

        [Test]
        public void FromSummaryTest1()
        {
            // Suppose we have a (fictitious) data set about patients who 
            // underwent cardiac surgery. The first column gives the number
            // of arterial bypasses performed during the surgery. The second
            // column gives the number of patients whose surgery went well,
            // while the third column gives the number of patients who had
            // at least one complication during the surgery.
            // 
            int[,] data =
            {
                // # of stents       success     complications
                {       1,             140,           45       },
                {       2,             130,           60       },
                {       3,             150,           31       },
                {       4,              96,           65       }
            };


            double[][] inputs = data.GetColumn(0).ToDouble().ToJagged();

            int[] positive = data.GetColumn(1);
            int[] negative = data.GetColumn(2);

            // Create a new Logistic Regression Analysis from the summary data
            var regression = LogisticRegressionAnalysis.FromSummary(inputs, positive, negative);

            regression.Compute(); // compute the analysis.

            // Now we can show a summary of the analysis
            // Accord.Controls.DataGridBox.Show(regression.Coefficients);


            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at the
            // vector

            double[] coef = regression.CoefficientValues;

            // The first value refers to the model's intercept term. We
            // can also retrieve the odds ratios and standard errors:

            double[] odds = regression.OddsRatios;
            double[] stde = regression.StandardErrors;


            // Finally, we can use it to estimate risk for a new patient
            double y = regression.Regression.Compute(new double[] { 4 });

            Assert.AreEqual(3.7586367581050162, odds[0], 1e-8);
            Assert.AreEqual(0.85772731075090014, odds[1], 1e-8);

            Assert.AreEqual(0.20884336554629004, stde[0], 1e-8);
            Assert.AreEqual(0.075837785246620285, stde[1], 1e-8);

            Assert.AreEqual(0.67044096045332713, y, 1e-8);

            LogisticRegressionAnalysis expected;


            {
                int[] qtr = data.GetColumn(0);

                var expanded = Accord.Statistics.Tools.Expand(qtr, positive, negative);

                double[][] inp = expanded.GetColumn(0).ToDouble().ToJagged();
                double[] outputs = expanded.GetColumn(1).ToDouble();

                expected = new LogisticRegressionAnalysis(inp, outputs);

                expected.Compute();

                double slope = expected.Coefficients[1].Value; // should return -0.153
                double inter = expected.Coefficients[0].Value;
                double value = expected.ChiSquare.PValue;      // should return 0.042
                Assert.AreEqual(-0.15346904821339602, slope, 1e-8);
                Assert.AreEqual(1.324056323049271, inter, 1e-8);
                Assert.AreEqual(0.042491262992507946, value, 1e-8);
            }



            var actual = regression;
            Assert.AreEqual(expected.Coefficients[0].Value, actual.Coefficients[0].Value, 1e-8);
            Assert.AreEqual(expected.Coefficients[1].Value, actual.Coefficients[1].Value, 1e-8);

            Assert.AreEqual(expected.ChiSquare.PValue, actual.ChiSquare.PValue, 1e-8);
            Assert.AreEqual(expected.WaldTests[0].PValue, actual.WaldTests[0].PValue, 1e-8);
            Assert.AreEqual(expected.WaldTests[1].PValue, actual.WaldTests[1].PValue, 1e-8);

            Assert.AreEqual(expected.Confidences[0].Max, actual.Confidences[0].Max, 1e-6);
            Assert.AreEqual(expected.Confidences[0].Min, actual.Confidences[0].Min, 1e-6);
            Assert.AreEqual(expected.Confidences[1].Max, actual.Confidences[1].Max, 1e-6);
            Assert.AreEqual(expected.Confidences[1].Min, actual.Confidences[1].Min, 1e-6);
        }

        [Test]
        public void example_learn()
        {
            #region doc_learn_part1
            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (this is completely fictional data).

            double[][] inputs =
            {
                //            Age  Smoking
                new double[] { 55,    0   }, 
                new double[] { 28,    0   }, 
                new double[] { 65,    1   }, 
                new double[] { 46,    0   }, 
                new double[] { 86,    1   }, 
                new double[] { 56,    1   }, 
                new double[] { 85,    0   }, 
                new double[] { 33,    0   }, 
                new double[] { 21,    1   }, 
                new double[] { 42,    1   }, 
            };

            // Additionally, we also have information about whether
            // or not they those patients had lung cancer. The array
            // below gives 0 for those who did not, and 1 for those
            // who did.

            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };

            // Create a Logistic Regression analysis
            var lra = new LogisticRegressionAnalysis()
            {
                Regularization = 0
            };

            // compute the analysis
            LogisticRegression regression = lra.Learn(inputs, output);

            // Now we can show a summary of the analysis
            // Accord.Controls.DataGridBox.Show(regression.Coefficients);
            #endregion

            #region doc_learn_part2
            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at the
            // vector

            double[] coef = lra.CoefficientValues;

            // The first value refers to the model's intercept term. We
            // can also retrieve the odds ratios and standard errors:

            double[] odds = lra.OddsRatios;
            double[] stde = lra.StandardErrors;

            // We can use the analysis to predict a score for a new patient:
            double y = lra.Regression.Score(new double[] { 87, 1 }); // 0.75

            // For those inputs, the answer probability is approximately 75%.
            #endregion

            Assert.AreEqual(0.085627701183146374, odds[0], 1e-8);
            Assert.AreEqual(1.0208597029292648, odds[1], 1e-8);
            Assert.AreEqual(5.8584748981777919, odds[2], 1e-8);

            Assert.AreEqual(2.1590686019473897, stde[0], 1e-8);
            Assert.AreEqual(0.033790422321041035, stde[1], 1e-8);
            Assert.AreEqual(1.4729903935788211, stde[2], 1e-8);

            Assert.AreEqual(0.75143272858389798, y, 1e-8);
        }

        [Test]
        public void FromSummary_new_method()
        {
            #region doc_learn_summary
            // Suppose we have a (fictitious) data set about patients who 
            // underwent cardiac surgery. The first column gives the number
            // of arterial bypasses performed during the surgery. The second
            // column gives the number of patients whose surgery went well,
            // while the third column gives the number of patients who had
            // at least one complication during the surgery.
            // 
            int[,] data =
            {
                // # of stents       success     complications
                {       1,             140,           45       },
                {       2,             130,           60       },
                {       3,             150,           31       },
                {       4,              96,           65       }
            };

            // Get input variable and number of positives and negatives
            double[][] inputs = data.GetColumn(0).ToDouble().ToJagged();
            int[] positive = data.GetColumn(1);
            int[] negative = data.GetColumn(2);

            // Create a new Logistic Regression Analysis from the summary data
            var lra = new LogisticRegressionAnalysis();

            // compute the analysis
            LogisticRegression regression = lra.Learn(inputs, positive, negative);

            // Now we can show a summary of the analysis
            // Accord.Controls.DataGridBox.Show(regression.Coefficients);


            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at the
            // vector

            double[] coef = lra.CoefficientValues;

            // The first value refers to the model's intercept term. We
            // can also retrieve the odds ratios and standard errors:

            double[] odds = lra.OddsRatios;
            double[] stde = lra.StandardErrors;


            // Finally, we can use it to estimate risk for a new patient
            double y = lra.Regression.Score(new double[] { 4 }); // 67.0
            #endregion


            Assert.AreEqual(3.7586367581050162, odds[0], 1e-8);
            Assert.AreEqual(0.85772731075090014, odds[1], 1e-8);

            Assert.AreEqual(0.20884336554629004, stde[0], 1e-6);
            Assert.AreEqual(0.075837785246620285, stde[1], 1e-6);

            Assert.AreEqual(0.67044096045332713, y, 1e-8);

            LogisticRegressionAnalysis expected;


            {
                int[] qtr = data.GetColumn(0);

                var expanded = Accord.Statistics.Tools.Expand(qtr, positive, negative);

                double[][] inp = expanded.GetColumn(0).ToDouble().ToJagged();
                double[] outputs = expanded.GetColumn(1).ToDouble();

                expected = new LogisticRegressionAnalysis();

                expected.Learn(inp, outputs);

                double slope = expected.Coefficients[1].Value; // should return -0.153
                double inter = expected.Coefficients[0].Value;
                double value = expected.ChiSquare.PValue;      // should return 0.042
                Assert.AreEqual(-0.15346904821339602, slope, 1e-8);
                Assert.AreEqual(1.324056323049271, inter, 1e-8);
                Assert.AreEqual(0.042491262992507946, value, 1e-8);
            }



            var actual = lra;
            Assert.AreEqual(expected.Coefficients[0].Value, actual.Coefficients[0].Value, 1e-8);
            Assert.AreEqual(expected.Coefficients[1].Value, actual.Coefficients[1].Value, 1e-8);

            Assert.AreEqual(expected.ChiSquare.PValue, actual.ChiSquare.PValue, 1e-8);
            Assert.AreEqual(expected.WaldTests[0].PValue, actual.WaldTests[0].PValue, 1e-8);
            Assert.AreEqual(expected.WaldTests[1].PValue, actual.WaldTests[1].PValue, 1e-8);

            Assert.AreEqual(expected.Confidences[0].Max, actual.Confidences[0].Max, 1e-6);
            Assert.AreEqual(expected.Confidences[0].Min, actual.Confidences[0].Min, 1e-6);
            Assert.AreEqual(expected.Confidences[1].Max, actual.Confidences[1].Max, 1e-6);
            Assert.AreEqual(expected.Confidences[1].Min, actual.Confidences[1].Min, 1e-6);
        }

        private static double[][] training = new double[][]
        {
                #region sample data
                new double[] { 0567, 0568, 0001, 0002,    0 },
                new double[] { 0839, 1043, 0204, 0011,    1 },
                new double[] { 0506, 1400, 0894, 0020,    1 },
                new double[] { 0066, 0066, 0000, 0001,    0 },
                new double[] { 0208, 0223, 0015, 0005,    1 },
                new double[] { 0069, 0069, 0000, 0001,    0 },
                new double[] { 0417, 0458, 0041, 0008,    1 },
                new double[] { 0078, 0078, 0000, 0001,    0 },
                new double[] { 0137, 0150, 0013, 0004,    1 },
                new double[] { 0108, 0136, 0028, 0002,    0 },
                new double[] { 0235, 0294, 0059, 0005,    0 },
                new double[] { 0350, 0511, 0161, 0010,    1 },
                new double[] { 0271, 0418, 0147, 0003,    0 },
                new double[] { 0195, 0217, 0022, 0010,    1 },
                new double[] { 0259, 0267, 0008, 0006,    1 },
                new double[] { 0298, 0372, 0074, 0007,    1 },
                new double[] { 0709, 0994, 0285, 0016,    1 },
                new double[] { 1041, 1348, 0307, 0039,    1 },
                new double[] { 0075, 0075, 0000, 0001,    0 },
                new double[] { 0529, 0597, 0068, 0002,    0 },
                new double[] { 0509, 0584, 0075, 0002,    0 },
                new double[] { 0289, 0289, 0000, 0001,    0 },
                new double[] { 0110, 0125, 0015, 0004,    0 },
                new double[] { 0020, 0020, 0000, 0001,    0 },
                new double[] { 0295, 0295, 0000, 0001,    0 },
                new double[] { 0250, 0283, 0033, 0002,    0 },
                new double[] { 0031, 0044, 0013, 0002,    0 },
                new double[] { 0178, 0198, 0020, 0003,    0 },
                new double[] { 0835, 0848, 0013, 0005,    0 },
                new double[] { 0132, 0178, 0046, 0002,    0 },
                new double[] { 0429, 0632, 0203, 0004,    0 },
                new double[] { 0740, 0894, 0154, 0005,    0 },
                new double[] { 0056, 0065, 0009, 0003,    1 },
                new double[] { 0071, 0071, 0000, 0001,    0 },
                new double[] { 0248, 0321, 0073, 0004,    0 },
                new double[] { 0034, 0034, 0000, 0001,    0 },
                new double[] { 0589, 0652, 0063, 0004,    0 },
                new double[] { 0124, 0134, 0010, 0002,    0 },
                new double[] { 0426, 0427, 0001, 0002,    0 },
                new double[] { 0030, 0030, 0000, 0001,    0 },
                new double[] { 0023, 0023, 0000, 0001,    0 },
                new double[] { 0499, 0499, 0000, 0001,    0 },
                #endregion
        };
    }
}
