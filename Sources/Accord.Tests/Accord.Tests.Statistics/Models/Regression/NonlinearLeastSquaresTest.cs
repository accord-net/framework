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
    using System;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Math.Optimization;
    using Accord.Math.Differentiation;

    [TestClass()]
    public class NonLinearLeastSquaresTest
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
        public void RunTest1()
        {
            // Example from https://en.wikipedia.org/wiki/Gauss%E2%80%93Newton_algorithm

            double[,] data =
            {
                { 0.03, 0.1947, 0.425, 0.626, 1.253, 2.500, 3.740 },
                { 0.05, 0.127, 0.094, 0.2122, 0.2729, 0.2665, 0.3317}
            };

            double[][] inputs = data.GetRow(0).ToArray();
            double[] outputs = data.GetRow(1);


            RegressionFunction rate = (double[] weights, double[] xi) =>
            {
                double x = xi[0];
                return (weights[0] * x) / (weights[1] + x);
            };

            RegressionGradientFunction grad = (double[] weights, double[] xi, double[] result) =>
            {
                double x = xi[0];

                FiniteDifferences diff = new FiniteDifferences(2);
                diff.Function = (bla) => rate(bla, xi);
                double[] compare = diff.Compute(weights);

                result[0] = -((-x) / (weights[1] + x));
                result[1] = -((weights[0] * x) / Math.Pow(weights[1] + x, 2));
            };


            NonlinearRegression regression = new NonlinearRegression(2, rate, grad);

            NonlinearLeastSquares nls = new NonlinearLeastSquares(regression, new GaussNewton(2));

            Assert.IsTrue(nls.Algorithm is GaussNewton);

            regression.Coefficients[0] = 0.9; // β1
            regression.Coefficients[1] = 0.2; // β2

            int iterations = 10;
            double[] errors = new double[iterations];
            for (int i = 0; i < errors.Length; i++)
                errors[i] = nls.Run(inputs, outputs);

            double b1 = regression.Coefficients[0];
            double b2 = regression.Coefficients[1];

            Assert.AreEqual(0.362, b1, 1e-3);
            Assert.AreEqual(0.556, b2, 3e-3);

            Assert.IsFalse(Double.IsNaN(b1));
            Assert.IsFalse(Double.IsNaN(b2));

            for (int i = 1; i < errors.Length; i++)
            {
                Assert.IsFalse(Double.IsNaN(errors[i - 1]));
                Assert.IsTrue(errors[i - 1] >= errors[i]);
            }

            Assert.AreEqual(1.23859, regression.StandardErrors[0], 1e-3);
            Assert.AreEqual(6.06352, regression.StandardErrors[1], 3e-3);
        }


    }
}
