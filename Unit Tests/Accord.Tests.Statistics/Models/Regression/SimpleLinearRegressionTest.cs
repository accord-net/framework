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
    using Accord.Statistics.Models.Regression.Linear;
    using NUnit.Framework;
    using Accord.Math;
    using System;
    using Accord.Statistics;
    using System.Globalization;
#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

    [TestFixture]
    public class SimpleLinearRegressionTest
    {

        [Test]
        public void RegressTest()
        {
            // Let's say we have some univariate, continuous sets of input data,
            // and a corresponding univariate, continuous set of output data, such
            // as a set of points in R². A simple linear regression is able to fit
            // a line relating the input variables to the output variables in which
            // the minimum-squared-error of the line and the actual output points
            // is minimum.

            // Declare some sample test data.
            double[] inputs = { 80, 60, 10, 20, 30 };
            double[] outputs = { 20, 40, 30, 50, 60 };

            // Create a new simple linear regression
            SimpleLinearRegression regression = new SimpleLinearRegression();

            // Compute the linear regression
            regression.Regress(inputs, outputs);

            // Compute the output for a given input. The
            double y = regression.Compute(85); // The answer will be 28.088

            // We can also extract the slope and the intercept term
            // for the line. Those will be -0.26 and 50.5, respectively.
            double s = regression.Slope;
            double c = regression.Intercept;

            // Expected slope and intercept
            double eSlope = -0.264706;
            double eIntercept = 50.588235;

            Assert.AreEqual(28.088235294117649, y, 1e-10);
            Assert.AreEqual(eSlope, s, 1e-5);
            Assert.AreEqual(eIntercept, c, 1e-5);

            Assert.IsFalse(double.IsNaN(y));
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Let's say we have some univariate, continuous sets of input data,
            // and a corresponding univariate, continuous set of output data, such
            // as a set of points in R². A simple linear regression is able to fit
            // a line relating the input variables to the output variables in which
            // the minimum-squared-error of the line and the actual output points
            // is minimum.

            // Declare some sample test data.
            double[] inputs = { 80, 60, 10, 20, 30 };
            double[] outputs = { 20, 40, 30, 50, 60 };

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Use OLS to learn the simple linear regression
            SimpleLinearRegression regression = ols.Learn(inputs, outputs);

            // Compute the output for a given input:
            double y = regression.Transform(85); // The answer will be 28.088

            // We can also extract the slope and the intercept term
            // for the line. Those will be -0.26 and 50.5, respectively.
            double s = regression.Slope;     // -0.264706
            double c = regression.Intercept; // 50.588235
            #endregion

            // Expected slope and intercept
            double eSlope = -0.264706;
            double eIntercept = 50.588235;

            Assert.AreEqual(28.088235294117649, y, 1e-10);
            Assert.AreEqual(eSlope, s, 1e-5);
            Assert.AreEqual(eIntercept, c, 1e-5);

            Assert.IsFalse(double.IsNaN(y));
        }

        [Test]
        public void prediction_test()
        {
            // example data from http://www.real-statistics.com/regression/confidence-and-prediction-intervals/
            double[][] input =
            {
                new double[] { 5, 80 },
                new double[] { 23, 78 },
                new double[] { 25, 60 },
                new double[] { 48, 53 },
                new double[] { 17, 85 },
                new double[] { 8, 84 },
                new double[] { 4, 73 },
                new double[] { 26, 79 },
                new double[] { 11, 81 },
                new double[] { 19, 75 },
                new double[] { 14, 68 },
                new double[] { 35, 72 },
                new double[] { 29, 58 },
                new double[] { 4, 92 },
                new double[] { 23, 65 },
            };

            double[] cig = input.GetColumn(0);
            double[] exp = input.GetColumn(1);

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Use OLS to learn the simple linear regression
            SimpleLinearRegression regression = ols.Learn(cig, exp);

            Assert.AreEqual(1, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);

            double x0 = 20;
            double y0 = regression.Transform(x0);
            Assert.AreEqual(y0, 73.1564, 1e-4);

            double syx = regression.GetStandardError(cig, exp);
            Assert.AreEqual(7.974682, syx, 1e-5);

            double ssx = cig.Subtract(cig.Mean()).Pow(2).Sum();
            Assert.AreEqual(2171.6, ssx, 1e-5);

            double n = exp.Length;
            double x0c = x0 - cig.Mean();
            double var = 1 / n + (x0c * x0c) / ssx;
            Assert.AreEqual(0.066832443052741455, var, 1e-10);
            double expected = syx * Math.Sqrt(var);
            double actual = regression.GetStandardError(x0, cig, exp);

            Assert.AreEqual(2.061612, expected, 1e-5);
            Assert.AreEqual(expected, actual, 1e-10);

            DoubleRange ci = regression.GetConfidenceInterval(x0, cig, exp);
            Assert.AreEqual(ci.Min, 68.702569616457751, 1e-5);
            Assert.AreEqual(ci.Max, 77.610256563931543, 1e-5);

            actual = regression.GetPredictionStandardError(x0, cig, exp);
            Assert.AreEqual(8.2368569010499666, actual, 1e-10);

            DoubleRange pi = regression.GetPredictionInterval(x0, cig, exp);
            Assert.AreEqual(pi.Min, 55.361765613397054, 1e-5);
            Assert.AreEqual(pi.Max, 90.95106056699224, 1e-5);
        }

        [Test]
        public void ToStringTest()
        {
            // Issue 51:
            SimpleLinearRegression regression = new SimpleLinearRegression();
            var x = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var y = new double[] { 1, 6, 17, 34, 57, 86, 121, 162, 209, 262, 321 };

            regression.Regress(x, y);

            {
                string expected = "y(x) = 32x + -44";
                expected = expected.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = regression.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 32x + -44";
                string actual = regression.ToString(null, CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 32.0x + -44.0";
                string actual = regression.ToString("N1", CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 32,00x + -44,00";
                string actual = regression.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void weight_test_linear()
        {
            SimpleLinearRegression reference;
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

                double[] x = data.GetColumn(1);
                double[] y = data.GetColumn(2);

                var ols = new OrdinaryLeastSquares();
                reference = ols.Learn(x, y);
                referenceR2 = reference.CoefficientOfDetermination(x, y);
            }

            SimpleLinearRegression target;
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
                double[] x = data.GetColumn(1);
                double[] y = data.GetColumn(2);

                OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
                target = ols.Learn(x, y, weights);
                targetR2 = target.CoefficientOfDetermination(x, y, weights);
            }

            Assert.AreEqual(reference.Slope, target.Slope);
            Assert.AreEqual(reference.Intercept, target.Intercept, 1e-8);
            Assert.AreEqual(0.16387475666214069, target.Slope, 1e-6);
            Assert.AreEqual(0.59166925681755056, target.Intercept, 1e-6);

            Assert.AreEqual(referenceR2, targetR2, 1e-8);
            Assert.AreEqual(0.91476129548901486, targetR2);
        }
    }

}
