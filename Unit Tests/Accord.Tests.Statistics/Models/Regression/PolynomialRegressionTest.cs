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
    using Math;
    using Math.Optimization.Losses;
    using NUnit.Framework;
    using System;
    using System.Globalization;
#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

    [TestFixture]
    public class PolynomialRegressionTest
    {

        [Test]
        public void learn_test()
        {
            double[] inputs = { 15.2, 229.7, 3500 };
            double[] outputs = { 0.51, 105.66, 1800 };

            var ls = new PolynomialLeastSquares()
            {
                Degree = 2
            };

            PolynomialRegression target = ls.Learn(inputs, outputs);

            double[] expected = { 8.003175717e-6, 4.882498125e-1, -6.913246203 };
            double[] actual;

            actual = target.Weights;

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(expected[0], actual[0], 1e-3);
            Assert.AreEqual(expected[1], actual[1], 1e-3);
            Assert.AreEqual(expected[2], target.Intercept, 1e-3);
        }

        [Test]
        public void learn_test_2()
        {
#if NETCORE
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.CurrentCulture = culture;
#endif

            #region doc_learn
            // Let's say we would like to learn 2nd degree polynomial that 
            // can map the first column X into its second column Y. We have 
            // 5 examples of those (x,y) pairs that we can use to learn this 
            // function:

            double[,] data =
            {
                // X       Y
                { 12,     144 }, // example #1
                { 15,     225 }, // example #2
                { 20,     400 }, // example #3
                { 25,     625 }, // example #4
                { 35,    1225 }, // example #5
            };

            // Let's retrieve the input and output data:
            double[] inputs = data.GetColumn(0);  // X
            double[] outputs = data.GetColumn(1); // Y

            // We can create a learning algorithm
            var ls = new PolynomialLeastSquares()
            {
                Degree = 2
            };

            // Now, we can use the algorithm to learn a polynomial
            PolynomialRegression poly = ls.Learn(inputs, outputs);

            // The learned polynomial will be given by
            string str = poly.ToString("N1"); // "y(x) = 1.0x^2 + 0.0x^1 + 0.0"

            // Where its weights can be accessed using
            double[] weights = poly.Weights;   // { 1.0000000000000024, -1.2407665029287351E-13 }
            double intercept = poly.Intercept; // 1.5652369518855253E-12

            // Finally, we can use this polynomial
            // to predict values for the input data
            double[] pred = poly.Transform(inputs);

            // Where the mean-squared-error (MSE) should be
            double error = new SquareLoss(outputs).Loss(pred); // 0.0
            #endregion

            Assert.AreEqual(0, error, 1e-10);

            string ex = weights.ToCSharp();
            double[] expected = { 1, 0 };

            Assert.AreEqual("y(x) = 1.0x^2 + 0.0x^1 + 0.0", str);
            Assert.IsTrue(weights.IsEqual(expected, 1e-6));
            Assert.AreEqual(0, intercept, 1e-6);
        }

        [Test]
        public void PolynomialRegressionRegressTest()
        {
            double[] inputs = { 15.2, 229.7, 3500 };
            double[] outputs = { 0.51, 105.66, 1800 };

            int degree = 2;
            PolynomialRegression target = new PolynomialRegression(degree);

            double[] expected = { 8.003175717e-6, 4.882498125e-1, -6.913246203 };
            double[] actual;

            target.Regress(inputs, outputs);
            actual = target.Coefficients;

            Assert.AreEqual(expected[0], actual[0], 1e-3);
            Assert.AreEqual(expected[1], actual[1], 1e-3);
            Assert.AreEqual(expected[2], actual[2], 1e-3);
        }

        [Test]
        public void ToStringTest()
        {
            // Issue 51:
            PolynomialRegression poly = new PolynomialRegression(2);
            var x = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var y = new double[] { 1, 6, 17, 34, 57, 86, 121, 162, 209, 262, 321 };

            poly.Regress(x, y);

            {
                string expected = "y(x) = 3x^2 + 1.99999999999998x^1 + 1.00000000000005";
                expected = expected.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = poly.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3x^2 + 1.99999999999998x^1 + 1.00000000000005";
                string actual = poly.ToString(null, CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3.0x^2 + 2.0x^1 + 1.0";
                string actual = poly.ToString("N1", CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3,00x^2 + 2,00x^1 + 1,00";
                string actual = poly.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void learn_ToStringTest_degree_is_0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PolynomialLeastSquares()
            {
                Degree = 0
            }, "");
        }

        [Test]
        public void learn_ToStringTest()
        {
            var x = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var y = new double[] { 1, 6, 17, 34, 57, 86, 121, 162, 209, 262, 321 };

            var pls = new PolynomialLeastSquares()
            {
                Degree = 2
            };

            PolynomialRegression poly = pls.Learn(x, y);

            {
                string expected = "y(x) = 3x^2 + 1.99999999999998x^1 + 1.00000000000005";
                expected = expected.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = poly.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3x^2 + 1.99999999999998x^1 + 1.00000000000005";
                string actual = poly.ToString(null, CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3.0x^2 + 2.0x^1 + 1.0";
                string actual = poly.ToString("N1", CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3,00x^2 + 2,00x^1 + 1,00";
                string actual = poly.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void weight_test_linear()
        {
            PolynomialRegression reference;
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

                var ols = new PolynomialLeastSquares();
                reference = ols.Learn(x, y);

                Assert.AreEqual(1, reference.NumberOfInputs);
                Assert.AreEqual(1, reference.NumberOfOutputs);

                referenceR2 = reference.CoefficientOfDetermination(x, y);
            }

            PolynomialRegression target;
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

                Assert.AreEqual(1, reference.NumberOfInputs);
                Assert.AreEqual(1, reference.NumberOfOutputs);

                var ols = new PolynomialLeastSquares();
                target = ols.Learn(x, y, weights);
                targetR2 = target.CoefficientOfDetermination(x, y, weights);
            }

            Assert.IsTrue(reference.Weights.IsEqual(target.Weights));
            Assert.AreEqual(reference.Intercept, target.Intercept, 1e-8);
            Assert.AreEqual(0.16387475666214069, target.Weights[0], 1e-6);
            Assert.AreEqual(0.59166925681755056, target.Intercept, 1e-6);

            Assert.AreEqual(referenceR2, targetR2, 1e-8);
            Assert.AreEqual(0.91476129548901486, targetR2);
        }


        [Test]
        public void weight_test_square()
        {
            PolynomialRegression reference;
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

                var ols = new PolynomialLeastSquares()
                {
                    Degree = 2,
                    IsRobust = true
                };

                reference = ols.Learn(x, y);

                Assert.AreEqual(1, reference.NumberOfInputs);
                Assert.AreEqual(1, reference.NumberOfOutputs);

                referenceR2 = reference.CoefficientOfDetermination(x, y);
            }

            PolynomialRegression target;
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

                Assert.AreEqual(1, reference.NumberOfInputs);
                Assert.AreEqual(1, reference.NumberOfOutputs);

                var ols = new PolynomialLeastSquares()
                {
                    Degree = 2,
                    IsRobust = true
                };

                target = ols.Learn(x, y, weights);
                targetR2 = target.CoefficientOfDetermination(x, y, weights);
            }

            Assert.IsTrue(reference.Weights.IsEqual(target.Weights, 1e-5));
            Assert.AreEqual(reference.Intercept, target.Intercept, 1e-8);
            Assert.AreEqual(-0.023044161067521208, target.Weights[0], 1e-6);
            Assert.AreEqual(-10.192933942631839, target.Intercept, 1e-6);

            Assert.AreEqual(referenceR2, targetR2, 1e-8);
            Assert.AreEqual(0.97660035938038947, targetR2);
        }

    }
}
