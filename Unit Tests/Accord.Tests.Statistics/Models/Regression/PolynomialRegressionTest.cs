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
    public class PolynomialRegressionTest
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

            Assert.AreEqual(expected[0], actual[0], 000.1);
            Assert.AreEqual(expected[1], actual[1], 000.1);
            Assert.AreEqual(expected[2], actual[2], 000.1);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            // Issue 51:
            PolynomialRegression poly = new PolynomialRegression(2);
            var x = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var y = new double[] { 1, 6, 17, 34, 57, 86, 121, 162, 209, 262, 321 };

            poly.Regress(x, y);

            {
                string expected = "y(x) = 3x^2 + 1.99999999999999x^1 + 1.00000000000006x^0";
                expected = expected.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string actual = poly.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3x^2 + 1.99999999999999x^1 + 1.00000000000006x^0";
                string actual = poly.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3.0x^2 + 2.0x^1 + 1.0x^0";
                string actual = poly.ToString("N1", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "y(x) = 3,00x^2 + 2,00x^1 + 1,00x^0";
                string actual = poly.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
