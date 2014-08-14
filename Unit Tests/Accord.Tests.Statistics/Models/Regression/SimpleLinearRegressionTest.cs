// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass()]
    public class SimpleLinearRegressionTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void RegressTest()
        {
            // Let's say we have some univariate, continuous sets of input data,
            // and a corresponding univariate, continuous set of output data, such
            // as a set of points in R². A simple linear regression is able to fit
            // a line relating the input variables to the output variables in which
            // the minimum-squared-error of the line and the actual output points
            // is minimum.

            // Declare some sample test data.
            double[] inputs =  { 80, 60, 10, 20, 30 };
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

        [TestMethod()]
        public void ToStringTest()
        {
            // Issue 51:
            SimpleLinearRegression regression = new SimpleLinearRegression();
            var x = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var y = new double[] { 1, 6, 17, 34, 57, 86, 121, 162, 209, 262, 321 };

            regression.Regress(x, y);

            {
                string expected = "y(x) = 32x + -44";
                expected = expected.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = regression.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 32x + -44";
                string actual = regression.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 32.0x + -44.0";
                string actual = regression.ToString("N1", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 32,00x + -44,00";
                string actual = regression.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
