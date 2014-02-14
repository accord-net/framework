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
    public class MultipleLinearRegressionTest
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

        [TestMethod()]
        public void RegressTest2()
        {
            MultipleLinearRegression target = new MultipleLinearRegression(2, false);

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

        [TestMethod()]
        public void RegressTest3()
        {
            // We will try to model a plane as an equation in the form
            // "ax + by + c = z". We have two input variables (x and y)
            // and we will be trying to find two parameters a and b and 
            // an intercept term c.

            // Create a multiple linear regression for two input and an intercept
            MultipleLinearRegression target = new MultipleLinearRegression(2, true);

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


            double r = target.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(1.0, r);

        }

        [TestMethod()]
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

                Assert.AreEqual(3, target.Coefficients.Length);
                Assert.IsTrue(target.HasIntercept);
                Assert.AreEqual(0, error, 1e-10);
            }

            {
                MultipleLinearRegression target = new MultipleLinearRegression(2, false);

                double error = target.Regress(inputs, output);

                Assert.AreEqual(2, target.Coefficients.Length);
                Assert.IsFalse(target.HasIntercept);
                Assert.AreEqual(0, error, 1e-10);
            }
        }

        [TestMethod()]
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

        [TestMethod()]
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
    }
}
