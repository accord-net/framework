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
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Models.Regression.Linear;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.Tests.Statistics.Properties;
    using System.Globalization;
    using Accord.DataSets;
    using Accord.Statistics.Filters;
#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

    [TestFixture]
    public class MultipleLinearRegressionTest
    {

        [Test]
        public void RegressTest()
        {
            MultipleLinearRegression target = new MultipleLinearRegression(1, true);

            double[][] inputs =
            {
                new double[] { 80 },
                new double[] { 60 },
                new double[] { 10 },
                new double[] { 20 },
                new double[] { 30 },
            };

            double[] outputs = { 20, 40, 30, 50, 60 };


            double error = target.Regress(inputs, outputs);
            Assert.AreEqual(1, target.NumberOfInputs);
            Assert.AreEqual(1, target.NumberOfOutputs);

            double slope = target.Coefficients[0];
            double intercept = target.Coefficients[1];

            Assert.AreEqual(-0.264706, slope, 1e-5);
            Assert.AreEqual(50.588235, intercept, 1e-5);
            Assert.AreEqual(761.764705, error, 1e-5);


            double r = target.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(0.23823529, r, 1e-6);

            string str = target.ToString(null, CultureInfo.GetCultureInfo("pt-BR"));

            Assert.AreEqual("y(x0) = -0,264705882352941*x0 + 50,5882352941176", str);
        }

        [Test]
        public void RegressTest2()
        {
            var target = new MultipleLinearRegression(2, false);

            Assert.IsFalse(target.HasIntercept);

            double[][] inputs =
            {
                new double[] { 80, 1 },
                new double[] { 60, 1 },
                new double[] { 10, 1 },
                new double[] { 20, 1 },
                new double[] { 30, 1 },
            };

            double[] outputs = { 20, 40, 30, 50, 60 };


            double error = target.Regress(inputs, outputs);
            Assert.AreEqual(2, target.NumberOfInputs);
            Assert.AreEqual(1, target.NumberOfOutputs);
            double slope = target.Coefficients[0];
            double intercept = target.Coefficients[1];

            Assert.AreEqual(-0.264706, slope, 1e-5);
            Assert.AreEqual(50.588235, intercept, 1e-5);
            Assert.AreEqual(761.764705, error, 1e-5);


            double r = target.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(0.23823529, r, 1e-6);

            string str = target.ToString(null, CultureInfo.GetCultureInfo("en-US"));

            Assert.AreEqual("y(x0, x1) = -0.264705882352941*x0 + 50.5882352941176*x1", str);
        }

        [Test]
        public void RegressTest3()
        {
            // We will try to model a plane as an equation in the form
            // "ax + by + c = z". We have two input variables (x and y)
            // and we will be trying to find two parameters a and b and 
            // an intercept term c.

            // Create a multiple linear regression for two input and an intercept
            var target = new MultipleLinearRegression(2, true);

            // Now suppose you have some points
            double[][] inputs =
            {
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 0 },
            };

            // located in the same Z (z = 1)
            double[] outputs = { 1, 1, 1, 1 };

            // Now we will try to fit a regression model
            double error = target.Regress(inputs, outputs);

            Assert.AreEqual(2, target.NumberOfInputs);
            Assert.AreEqual(1, target.NumberOfOutputs);

            // As result, we will be given the following:
            double a = target.Coefficients[0]; // a = 0
            double b = target.Coefficients[1]; // b = 0
            double c = target.Coefficients[2]; // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            Assert.AreEqual(0.0, a, 1e-6);
            Assert.AreEqual(0.0, b, 1e-6);
            Assert.AreEqual(1.0, c, 1e-6);
            Assert.AreEqual(0.0, error, 1e-6);

            double[] expected = target.Compute(inputs);
            double[] actual = target.Transform(inputs);
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            double r = target.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(1.0, r);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // We will try to model a plane as an equation in the form
            // "ax + by + c = z". We have two input variables (x and y)
            // and we will be trying to find two parameters a and b and 
            // an intercept term c.

            // We will use Ordinary Least Squares to create a
            // linear regression model with an intercept term
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };

            // Now suppose you have some points
            double[][] inputs =
            {
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 0 },
            };

            // located in the same Z (z = 1)
            double[] outputs = { 1, 1, 1, 1 };

            // Use Ordinary Least Squares to estimate a regression model
            MultipleLinearRegression regression = ols.Learn(inputs, outputs);

            // As result, we will be given the following:
            double a = regression.Weights[0]; // a = 0
            double b = regression.Weights[1]; // b = 0
            double c = regression.Intercept;  // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            // We can compute the predicted points using
            double[] predicted = regression.Transform(inputs);

            // And the squared error loss using 
            double error = new SquareLoss(outputs).Loss(predicted);

            // We can also compute other measures, such as the coefficient of determination r²
            double r2 = new RSquaredLoss(numberOfInputs: 2, expected: outputs).Loss(predicted); // should be 1

            // We can also compute the adjusted or weighted versions of r² using
            var r2loss = new RSquaredLoss(numberOfInputs: 2, expected: outputs)
            {
                Adjust = true,
                // Weights = weights; // (if you have a weighted problem)
            };

            double ar2 = r2loss.Loss(predicted); // should be 1

            // Alternatively, we can also use the less generic, but maybe more user-friendly method directly:
            double ur2 = regression.CoefficientOfDetermination(inputs, outputs, adjust: true); // should be 1
            #endregion

            Assert.AreEqual(2, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);


            Assert.AreEqual(0.0, a, 1e-6);
            Assert.AreEqual(0.0, b, 1e-6);
            Assert.AreEqual(1.0, c, 1e-6);
            Assert.AreEqual(0.0, error, 1e-6);

            double[] expected = regression.Compute(inputs);
            double[] actual = regression.Transform(inputs);
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            Assert.AreEqual(1.0, r2);
            Assert.AreEqual(1.0, ar2);
            Assert.AreEqual(1.0, ur2);
        }

        [Test]
        public void learn_test_2()
        {
            #region doc_learn_2
            // Let's say we would like predict a continuous number from a set 
            // of discrete and continuous input variables. For this, we will 
            // be using the Servo dataset from UCI's Machine Learning repository 
            // as an example: http://archive.ics.uci.edu/ml/datasets/Servo

            // Create a Servo dataset
            Servo servo = new Servo();
            object[][] instances = servo.Instances; // 167 x 4 
            double[] outputs = servo.Output;        // 167 x 1

            // This dataset contains 4 columns, where the first two are 
            // symbolic (having possible values A, B, C, D, E), and the
            // last two are continuous.

            // We will use a codification filter to transform the symbolic 
            // variables into one-hot vectors, while keeping the other two
            // continuous variables intact:
            var codebook = new Codification<object>()
            {
                { "motor", CodificationVariable.Categorical },
                { "screw", CodificationVariable.Categorical },
                { "pgain", CodificationVariable.Continuous },
                { "vgain", CodificationVariable.Continuous },
            };

            // Learn the codebook
            codebook.Learn(instances);

            // We can gather some info about the problem:
            int numberOfInputs = codebook.NumberOfInputs;   // should be 4 (since there are 4 variables)
            int numberOfOutputs = codebook.NumberOfOutputs; // should be 12 (due their one-hot encodings)

            // Now we can use it to obtain double[] vectors:
            double[][] inputs = codebook.ToDouble().Transform(instances);

            // We will use Ordinary Least Squares to create a
            // linear regression model with an intercept term
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };

            // Use Ordinary Least Squares to estimate a regression model:
            MultipleLinearRegression regression = ols.Learn(inputs, outputs);

            // We can compute the predicted points using:
            double[] predicted = regression.Transform(inputs);

            // And the squared error using the SquareLoss class:
            double error = new SquareLoss(outputs).Loss(predicted);

            // We can also compute other measures, such as the coefficient of determination r² using:
            double r2 = new RSquaredLoss(numberOfOutputs, outputs).Loss(predicted); // should be 0.55086630162967354

            // Or the adjusted or weighted versions of r² using:
            var r2loss = new RSquaredLoss(numberOfOutputs, outputs)
            {
                Adjust = true,        
                // Weights = weights; // (uncomment if you have a weighted problem)
            };

            double ar2 = r2loss.Loss(predicted); // should be 0.51586887058782993

            // Alternatively, we can also use the less generic, but maybe more user-friendly method directly:
            double ur2 = regression.CoefficientOfDetermination(inputs, outputs, adjust: true); // should be 0.51586887058782993
            #endregion

            Assert.AreEqual(4, numberOfInputs);
            Assert.AreEqual(12, numberOfOutputs);
            Assert.AreEqual(12, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);

            Assert.AreEqual(1.0859586717266123, error, 1e-6);

            double[] expected = regression.Compute(inputs);
            double[] actual = regression.Transform(inputs);
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            Assert.AreEqual(0.55086630162967354, r2);
            Assert.AreEqual(0.51586887058782993, ar2);
            Assert.AreEqual(0.51586887058782993, ur2);
        }


        [Test]
        public void issue_602()
        {
            // Robust multivariate regression causes IndexOutOfRangeException #602
            // https://github.com/accord-net/framework/issues/602

            var inputs = new double[][]
            {
                new double[] { 1, 2, 3 },
                new double[] { 2, 3, 4 },
                new double[] { 3, 4, 5 },
                new double[] { 4, 5, 6 },
                new double[] { 5, 6, 7 },
                new double[] { 6, 7, 8 },
            };

            var outputs = new double[][]
            {
                new double[] { 3, 4 },
                new double[] { 4, 5 },
                new double[] { 5, 6 },
                new double[] { 6, 7 },
                new double[] { 7, 8 },
                new double[] { 8, 9 },
            };

            var ols = new OrdinaryLeastSquares() { IsRobust = true };

            var regression = ols.Learn(inputs, outputs);

            //string a = regression.Weights.ToCSharp();
            //string b = regression.Intercepts.ToCSharp();

            double[][] expectedWeights = new double[][]
            {
                new double[] { 4.44089209850063E-16, -0.333333333333333 },
                new double[] { 0.333333333333333, 0.333333333333333 },
                new double[] { 0.666666666666667, 1 }
            };

            double[] expectedIntercepts = new double[] {
                0.333333333333333, 0.666666666666666
            };

            Assert.IsTrue(expectedWeights.IsEqual(regression.Weights, rtol: 1e-5));
            Assert.IsTrue(expectedIntercepts.IsEqual(regression.Intercepts, rtol: 1e-5));
        }

        [Test]
        public void RegressTest4()
        {
            int count = 1000;
            double[][] inputs = new double[count][];
            double[] output = new double[count];

            for (int i = 0; i < inputs.Length; i++)
            {
                double x = i + 1;
                double y = 2 * (i + 1) - 1;
                inputs[i] = new[] { x, y };
                output[i] = 4 * x - y + 3;
            }

            {
                MultipleLinearRegression target = new MultipleLinearRegression(2, true);

                double error = target.Regress(inputs, output);

                Assert.AreEqual(2, target.NumberOfInputs);
                Assert.AreEqual(1, target.NumberOfOutputs);

                Assert.AreEqual(3, target.Coefficients.Length);
                Assert.IsTrue(target.HasIntercept);
                Assert.AreEqual(0, error, 1e-10);
            }

            {
                MultipleLinearRegression target = new MultipleLinearRegression(2, false);

                double error = target.Regress(inputs, output);

                Assert.AreEqual(2, target.NumberOfInputs);
                Assert.AreEqual(1, target.NumberOfOutputs);

                Assert.AreEqual(2, target.Coefficients.Length);
                Assert.IsFalse(target.HasIntercept);
                Assert.AreEqual(0, error, 1e-10);
            }
        }

        [Test]
        public void RegressTest5()
        {
            int count = 1000;
            double[][] inputs = new double[count][];
            double[] output = new double[count];

            for (int i = 0; i < inputs.Length; i++)
            {
                double x = i + 1;
                double y = 2 * (i + 1) - 1;
                inputs[i] = new[] { x, y };
                output[i] = 4 * x - y; // no constant term
            }

            {
                MultipleLinearRegression target = new MultipleLinearRegression(2, true);

                double error = target.Regress(inputs, output);

                Assert.IsTrue(target.HasIntercept);
                Assert.AreEqual(0, error, 1e-10);
            }

            {
                MultipleLinearRegression target = new MultipleLinearRegression(2, false);

                double error = target.Regress(inputs, output);

                Assert.IsFalse(target.HasIntercept);
                Assert.AreEqual(0, error, 1e-10);
            }
        }

        [Test]
        public void RegressTest6()
        {
            MultipleLinearRegression target = new MultipleLinearRegression(2, false);

            double[][] inputs =
            {
                new double[] { 0, 0 },
                new double[] { 0, 0 },
                new double[] { 0, 0 },
                new double[] { 0, 0 },
                new double[] { 0, 0 },
            };

            double[] outputs = { 20, 40, 30, 50, 60 };


            double error = target.Regress(inputs, outputs);

            double slope = target.Coefficients[0];
            double intercept = target.Coefficients[1];

            Assert.AreEqual(0, slope, 1e-5);
            Assert.AreEqual(0, intercept, 1e-5);
            Assert.AreEqual(9000, error);


            double r = target.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(-8, r, 1e-6);

            string str = target.ToString(null, CultureInfo.GetCultureInfo("pt-BR"));

            Assert.AreEqual("y(x0, x1) = 0*x0 + 0*x1", str);
        }

#if !NO_DATA_TABLE
        [Test]
        public void prediction_test()
        {
#if NETCORE
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
#endif

            // Example from http://www.real-statistics.com/multiple-regression/confidence-and-prediction-intervals/
            var dt = Accord.IO.CsvReader.FromText(Resources.linreg, true).ToTable();

            double[] y = dt.Columns["Poverty"].ToArray();
            double[][] x = dt.ToArray("Infant Mort", "White", "Crime");

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Use OLS to learn the multiple linear regression
            MultipleLinearRegression regression = ols.Learn(x, y);

            Assert.AreEqual(3, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);

            Assert.AreEqual(0.443650703716698, regression.Intercept, 1e-5);
            Assert.AreEqual(1.2791842411083394, regression.Weights[0], 1e-5);
            Assert.AreEqual(0.036259242392669415, regression.Weights[1], 1e-5);
            Assert.AreEqual(0.0014225014835705938, regression.Weights[2], 1e-5);

            double rse = regression.GetStandardError(x, y);
            Assert.AreEqual(rse, 2.4703520840798507, 1e-5);


            double[][] im = ols.GetInformationMatrix();
            double mse = regression.GetStandardError(x, y);
            double[] se = regression.GetStandardErrors(mse, im);

            Assert.AreEqual(0.30063086032754965, se[0], 1e-10);
            Assert.AreEqual(0.033603448179240082, se[1], 1e-10);
            Assert.AreEqual(0.0022414548866296342, se[2], 1e-10);
            Assert.AreEqual(3.9879881671805824, se[3], 1e-10);

            double[] x0 = new double[] { 7, 80, 400 };
            double y0 = regression.Transform(x0);
            Assert.AreEqual(y0, 12.867680376316864, 1e-5);

            double actual = regression.GetStandardError(x0, mse, im);

            Assert.AreEqual(0.35902764658470271, actual, 1e-10);

            DoubleRange ci = regression.GetConfidenceInterval(x0, mse, x.Length, im);
            Assert.AreEqual(ci.Min, 12.144995206616116, 1e-5);
            Assert.AreEqual(ci.Max, 13.590365546017612, 1e-5);

            actual = regression.GetPredictionStandardError(x0, mse, im);
            Assert.AreEqual(2.4963053239397244, actual, 1e-10);

            DoubleRange pi = regression.GetPredictionInterval(x0, mse, x.Length, im);
            Assert.AreEqual(pi.Min, 7.8428783761994554, 1e-5);
            Assert.AreEqual(pi.Max, 17.892482376434273, 1e-5);
        }
#endif

        [Test]
        public void weight_test()
        {
            MultipleLinearRegression reference;
            double referenceR2;

            {
                double[][] data =
                {
                    new[] { 1.0, 10.7, 2.4 }, // 
                    new[] { 1.0, 10.7, 2.4 }, // 
                    new[] { 1.0, 10.7, 2.4 }, // 
                    new[] { 1.0, 10.7, 2.4 }, // 
                    new[] { 1.0, 10.7, 2.4 }, // 5 times weight 1
                    new[] { 1.0, 12.5, 3.6 },
                    new[] { 1.0, 43.2, 7.6 },
                    new[] { 1.0, 10.2, 1.1 },
                };

                double[][] x = Jagged.ColumnVector(data.GetColumn(1));
                double[] y = data.GetColumn(2);

                var ols = new OrdinaryLeastSquares();
                reference = ols.Learn(x, y);
                referenceR2 = reference.CoefficientOfDetermination(x, y);
            }

            MultipleLinearRegression target;
            double targetR2;

            {
                double[][] data =
                {
                    new[] { 5.0, 10.7, 2.4 }, // 1 times weight 5
                    new[] { 1.0, 12.5, 3.6 },
                    new[] { 1.0, 43.2, 7.6 },
                    new[] { 1.0, 10.2, 1.1 },
                };

                double[] weights = data.GetColumn(0);
                double[][] x = Jagged.ColumnVector(data.GetColumn(1));
                double[] y = data.GetColumn(2);

                OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
                target = ols.Learn(x, y, weights);
                targetR2 = target.CoefficientOfDetermination(x, y, weights: weights);
            }

            Assert.IsTrue(reference.Weights.IsEqual(target.Weights));
            Assert.AreEqual(reference.Intercept, target.Intercept, 1e-8);
            Assert.AreEqual(0.16387475666214069, target.Weights[0], 1e-6);
            Assert.AreEqual(0.59166925681755056, target.Intercept, 1e-6);

            Assert.AreEqual(referenceR2, targetR2, 1e-8);
            Assert.AreEqual(0.91476129548901486, targetR2);
        }
    }
}
