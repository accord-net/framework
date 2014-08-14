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

using Accord.Statistics.Models.Regression.Linear;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for MultivariateLinearRegressionTest and is intended
    ///to contain all MultivariateLinearRegressionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultivariateLinearRegressionTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

            MultivariateLinearRegression target = new MultivariateLinearRegression(1, 1, true);

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

            MultivariateLinearRegression target2 = new MultivariateLinearRegression(2, 1, false);

            target2.Regress(X1, Y);

            Assert.AreEqual(target2.Coefficients[0, 0], eB, 0.001);
            Assert.AreEqual(target2.Coefficients[1, 0], eA, 0.001);

            Assert.AreEqual(target2.Inputs, 2);
            Assert.AreEqual(target2.Outputs, 1);
        }


        [TestMethod()]
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

    }
}
