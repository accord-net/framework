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

            Assert.AreEqual(str, "y(x) = 1.0x^2 + 0.0x^1 + 0.0");
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
                expected = expected.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = poly.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3x^2 + 1.99999999999998x^1 + 1.00000000000005";
                string actual = poly.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3.0x^2 + 2.0x^1 + 1.0";
                string actual = poly.ToString("N1", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3,00x^2 + 2,00x^1 + 1,00";
                string actual = poly.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
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
                expected = expected.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = poly.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3x^2 + 1.99999999999998x^1 + 1.00000000000005";
                string actual = poly.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3.0x^2 + 2.0x^1 + 1.0";
                string actual = poly.ToString("N1", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3,00x^2 + 2,00x^1 + 1,00";
                string actual = poly.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
