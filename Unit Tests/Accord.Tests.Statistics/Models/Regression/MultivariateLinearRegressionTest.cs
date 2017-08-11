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

    [TestFixture]
    public class MultivariateLinearRegressionTest
    {

        [Test]
        public void RegressTest()
        {
            double[][] X =
            {
                new double[] {    4.47 },
                new double[] {  208.30 },
                new double[] { 3400.00 },
            };


            double[][] Y =
            {
                new double[] {    0.51 },
                new double[] {  105.66 },
                new double[] { 1800.00 },
            };



            double eB = 0.5303528166;
            double eA = -3.290915095;

            var target = new MultivariateLinearRegression(1, 1, true);

            target.Regress(X, Y);

            Assert.AreEqual(target.Coefficients[0, 0], eB, 0.001);
            Assert.AreEqual(target.Intercepts[0], eA, 0.001);

            Assert.AreEqual(target.Inputs, 1);
            Assert.AreEqual(target.Outputs, 1);



            // Test manually including an constant term to generate an intercept
            double[][] X1 =
            {
                new double[] {    4.47, 1 },
                new double[] {  208.30, 1 },
                new double[] { 3400.00, 1 },
            };

            var target2 = new MultivariateLinearRegression(2, 1, false);

            target2.Regress(X1, Y);

            Assert.AreEqual(target2.Coefficients[0, 0], eB, 0.001);
            Assert.AreEqual(target2.Coefficients[1, 0], eA, 0.001);

            Assert.AreEqual(target2.Inputs, 2);
            Assert.AreEqual(target2.Outputs, 1);
        }


        [Test]
        public void RegressTest2()
        {
            // The multivariate linear regression is a generalization of
            // the multiple linear regression. In the multivariate linear
            // regression, not only the input variables are multivariate,
            // but also are the output dependent variables.

            // In the following example, we will perform a regression of
            // a 2-dimensional output variable over a 3-dimensional input
            // variable.

            double[][] inputs =
            {
                // variables:  x1  x2  x3
                new double[] {  1,  1,  1 }, // input sample 1
                new double[] {  2,  1,  1 }, // input sample 2
                new double[] {  3,  1,  1 }, // input sample 3
            };

            double[][] outputs =
            {
                // variables:  y1  y2
                new double[] {  2,  3 }, // corresponding output to sample 1
                new double[] {  4,  6 }, // corresponding output to sample 2
                new double[] {  6,  9 }, // corresponding output to sample 3
            };

            // With a quick eye inspection, it is possible to see that
            // the first output variable y1 is always the double of the
            // first input variable. The second output variable y2 is
            // always the triple of the first input variable. The other
            // input variables are unused. Nevertheless, we will fit a
            // multivariate regression model and confirm the validity
            // of our impressions:

            // Create a new multivariate linear regression with 3 inputs and 2 outputs
            var regression = new MultivariateLinearRegression(3, 2);

            // Now, compute the multivariate linear regression:
            double error = regression.Regress(inputs, outputs);

            // At this point, the regression error will be 0 (the fit was
            // perfect). The regression coefficients for the first input
            // and first output variables will be 2. The coefficient for
            // the first input and second output variables will be 3. All
            // others will be 0.
            //
            // regression.Coefficients should be the matrix given by
            //
            // double[,] coefficients = {
            //                              { 2, 3 },
            //                              { 0, 0 },
            //                              { 0, 0 },
            //                          };
            //

            // The first input variable coefficients will be 2 and 3:
            Assert.AreEqual(2, regression.Coefficients[0, 0], 1e-10);
            Assert.AreEqual(3, regression.Coefficients[0, 1], 1e-10);

            // And all other coefficients will be 0:
            Assert.AreEqual(0, regression.Coefficients[1, 0], 1e-10);
            Assert.AreEqual(0, regression.Coefficients[1, 1], 1e-10);
            Assert.AreEqual(0, regression.Coefficients[2, 0], 1e-10);
            Assert.AreEqual(0, regression.Coefficients[2, 1], 1e-10);

            Assert.AreEqual(3, regression.NumberOfInputs);
            Assert.AreEqual(2, regression.NumberOfOutputs);

            // We can also check the r-squared coefficients of determination:
            double[] r2 = regression.CoefficientOfDetermination(inputs, outputs);

            // Which should be one for both output variables:
            Assert.AreEqual(1, r2[0]);
            Assert.AreEqual(1, r2[1]);

            foreach (var e in regression.Coefficients)
                Assert.IsFalse(double.IsNaN(e));

            Assert.AreEqual(0, error, 1e-10);
            Assert.IsFalse(double.IsNaN(error));
        }

        [Test]
        public void learn_test1()
        {
            #region doc_learn
            // The multivariate linear regression is a generalization of
            // the multiple linear regression. In the multivariate linear
            // regression, not only the input variables are multivariate,
            // but also are the output dependent variables.

            // In the following example, we will perform a regression of
            // a 2-dimensional output variable over a 3-dimensional input
            // variable.

            double[][] inputs =
            {
                // variables:  x1  x2  x3
                new double[] {  1,  1,  1 }, // input sample 1
                new double[] {  2,  1,  1 }, // input sample 2
                new double[] {  3,  1,  1 }, // input sample 3
            };

            double[][] outputs =
            {
                // variables:  y1  y2
                new double[] {  2,  3 }, // corresponding output to sample 1
                new double[] {  4,  6 }, // corresponding output to sample 2
                new double[] {  6,  9 }, // corresponding output to sample 3
            };

            // With a quick eye inspection, it is possible to see that
            // the first output variable y1 is always the double of the
            // first input variable. The second output variable y2 is
            // always the triple of the first input variable. The other
            // input variables are unused. Nevertheless, we will fit a
            // multivariate regression model and confirm the validity
            // of our impressions:

            // Use Ordinary Least Squares to create the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Now, compute the multivariate linear regression:
            MultivariateLinearRegression regression = ols.Learn(inputs, outputs);

            // We can obtain predictions using
            double[][] predictions = regression.Transform(inputs);

            // The prediction error is
            double error = new SquareLoss(outputs).Loss(predictions); // 0

            // At this point, the regression error will be 0 (the fit was
            // perfect). The regression coefficients for the first input
            // and first output variables will be 2. The coefficient for
            // the first input and second output variables will be 3. All
            // others will be 0.
            //
            // regression.Coefficients should be the matrix given by
            //
            // double[,] coefficients = {
            //                              { 2, 3 },
            //                              { 0, 0 },
            //                              { 0, 0 },
            //                          };
            //

            // We can also check the r-squared coefficients of determination:
            double[] r2 = regression.CoefficientOfDetermination(inputs, outputs);
            #endregion

            // The first input variable coefficients will be 2 and 3:
            Assert.AreEqual(2, regression.Coefficients[0, 0], 1e-10);
            Assert.AreEqual(3, regression.Coefficients[0, 1], 1e-10);

            // And all other coefficients will be 0:
            Assert.AreEqual(0, regression.Coefficients[1, 0], 1e-10);
            Assert.AreEqual(0, regression.Coefficients[1, 1], 1e-10);
            Assert.AreEqual(0, regression.Coefficients[2, 0], 1e-10);
            Assert.AreEqual(0, regression.Coefficients[2, 1], 1e-10);

            Assert.AreEqual(3, regression.NumberOfInputs);
            Assert.AreEqual(2, regression.NumberOfOutputs);

            // Which should be one for both output variables:
            Assert.AreEqual(1, r2[0]);
            Assert.AreEqual(1, r2[1]);

            foreach (var e in regression.Coefficients)
                Assert.IsFalse(double.IsNaN(e));

            Assert.AreEqual(0, error, 1e-10);
            Assert.IsFalse(double.IsNaN(error));
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

            double[][] y = dt.ToArray("Poverty");
            double[][] x = dt.ToArray("Infant Mort", "White", "Crime");

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Use OLS to learn the multiple linear regression
            MultivariateLinearRegression regression = ols.Learn(x, y);

            Assert.AreEqual(3, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);

            Assert.AreEqual(0.443650703716698, regression.Intercepts[0], 1e-5);
            Assert.AreEqual(1.2791842411083394, regression.Weights[0][0], 1e-5);
            Assert.AreEqual(0.036259242392669415, regression.Weights[1][0], 1e-5);
            Assert.AreEqual(0.0014225014835705938, regression.Weights[2][0], 1e-5);

            double rse = regression.GetStandardError(x, y)[0];
            Assert.AreEqual(rse, 2.4703520840798507, 1e-5);


            double[][] im = ols.GetInformationMatrix();
            double[] mse = regression.GetStandardError(x, y);
            double[][] se = regression.GetStandardErrors(mse, im);

            Assert.AreEqual(0.30063086032754965, se[0][0], 1e-10);
            Assert.AreEqual(0.033603448179240082, se[0][1], 1e-10);
            Assert.AreEqual(0.0022414548866296342, se[0][2], 1e-10);
            Assert.AreEqual(3.9879881671805824, se[0][3], 1e-10);

            double[] x0 = new double[] { 7, 80, 400 };
            double y0 = regression.Transform(x0)[0];
            Assert.AreEqual(y0, 12.867680376316864, 1e-5);

            double actual = regression.GetStandardError(x0, mse, im)[0];

            Assert.AreEqual(0.35902764658470271, actual, 1e-10);

            DoubleRange ci = regression.GetConfidenceInterval(x0, mse, x.Length, im)[0];
            Assert.AreEqual(ci.Min, 12.144995206616116, 1e-5);
            Assert.AreEqual(ci.Max, 13.590365546017612, 1e-5);

            actual = regression.GetPredictionStandardError(x0, mse, im)[0];
            Assert.AreEqual(2.4963053239397244, actual, 1e-10);

            DoubleRange pi = regression.GetPredictionInterval(x0, mse, x.Length, im)[0];
            Assert.AreEqual(pi.Min, 7.8428783761994554, 1e-5);
            Assert.AreEqual(pi.Max, 17.892482376434273, 1e-5);
        }
#endif

        [Test]
        public void weight_test()
        {
            MultivariateLinearRegression reference;
            double[] referenceR2;

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
                double[][] y = Jagged.ColumnVector(data.GetColumn(2));

                var ols = new OrdinaryLeastSquares();
                reference = ols.Learn(x, y);
                referenceR2 = reference.CoefficientOfDetermination(x, y);
            }

            MultivariateLinearRegression target;
            double[] targetR2;

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
                double[][] y = Jagged.ColumnVector(data.GetColumn(2));

                OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
                target = ols.Learn(x, y, weights);
                targetR2 = target.CoefficientOfDetermination(x, y, weights: weights);
            }

            Assert.IsTrue(reference.Weights.IsEqual(target.Weights));
            Assert.IsTrue(reference.Intercepts.IsEqual(target.Intercepts, 1e-8));
            Assert.AreEqual(0.16387475666214069, target.Weights[0][0], 1e-6);
            Assert.AreEqual(0.59166925681755056, target.Intercepts[0], 1e-6);

            Assert.AreEqual(referenceR2[0], targetR2[0], 1e-8);
            Assert.AreEqual(0.91476129548901486, targetR2[0], 1e-10);
        }
    }
}
