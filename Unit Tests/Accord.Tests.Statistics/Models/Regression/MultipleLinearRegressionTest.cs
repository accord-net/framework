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

            string str = target.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));

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

            string str = target.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("en-US"));

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
            double c = regression.Intercept; // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            // We can compute the predicted points using
            double[] predicted = regression.Transform(inputs);

            // And the squared error loss using 
            double error = new SquareLoss(outputs).Loss(predicted);
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

            double r = regression.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(1.0, r);
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

            string str = target.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));

            Assert.AreEqual("y(x0, x1) = 0*x0 + 0*x1", str);
        }

        [Test]
        public void prediction_test()
        {
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
    }
}
