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
    using Accord.Math.Differentiation;
    using Accord.Math.Optimization;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class NonlinearLestSquaresTest
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

        double function(double[] parameters, double[] input)
        {
            double m = parameters[0];
            double s = parameters[1];
            double a = parameters[2];
            double b = parameters[3];
            double x = input[0];

            return a * Math.Exp(-0.5 * Math.Pow((x - m) / s, 2)) + b;
        }

        void gradient(double[] parameters, double[] input, double[] result)
        {
            double m = parameters[0];
            double s = parameters[1];
            double a = parameters[2];
            double b = parameters[3];
            double x = input[0];

            Func<double, double> exp = System.Math.Exp;
            Func<double, double, double> pow = System.Math.Pow;

            // diff a*exp(-0.5((m-x)/s)²) + b wrt m
            result[0] = -(a * (m - x) * exp(-(0.5 * pow(m - x, 2)) / (s * s))) / (s * s);

            //diff a*exp(-0.5((m-x)/s)²) + b wrt s
            result[1] = (a * pow(m - x, 2) * exp(-(0.5 * pow(m - x, 2)) / (s * s))) / (s * s * s);

            // diff a*exp(-0.5((m-x)/s)²) + b wrt a
            result[2] = exp(-(0.5 * pow(x - m, 2)) / (s * s));

            // diff a*exp(-0.5((m-x)/s)²) + b wrt b
            result[3] = 1;
        }

        [TestMethod()]
        public void RunTest()
        {
            double[,] data =
            {
                { -40,    -21142.1111111111 },
                { -30,    -21330.1111111111 },
                { -20,    -12036.1111111111 },
                { -10,      7255.3888888889 },
                {   0,     32474.8888888889 },
                {  10,     32474.8888888889 },
                {  20,      9060.8888888889 },
                {  30,    -11628.1111111111 },
                {  40,    -15129.6111111111 },
            };

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] outputs = data.GetColumn(1);

            NonlinearRegression regression = new NonlinearRegression(4, function, gradient);


            NonlinearLeastSquares nls = new NonlinearLeastSquares(regression);

            Assert.IsTrue(nls.Algorithm is LevenbergMarquardt);

            regression.Coefficients[0] = 0; // m
            regression.Coefficients[1] = 80; // s
            regression.Coefficients[2] = 53805; // a
            regression.Coefficients[3] = -21330.11; //b

            double error = 0;
            for (int i = 0; i < 100; i++)
                error = nls.Run(inputs, outputs);

            double m = regression.Coefficients[0];
            double s = regression.Coefficients[1];
            double a = regression.Coefficients[2];
            double b = regression.Coefficients[3];

            Assert.AreEqual(5.316196154830604, m, 1e-3);
            Assert.AreEqual(12.792301798208918, s, 1e-3);
            Assert.AreEqual(56794.832645792514, a, 1e-3);
            Assert.AreEqual(-20219.675997523173, b, 1e-2);

            Assert.IsFalse(Double.IsNaN(m));
            Assert.IsFalse(Double.IsNaN(s));
            Assert.IsFalse(Double.IsNaN(a));
            Assert.IsFalse(Double.IsNaN(b));
        }

        [TestMethod()]
        public void RunTest2()
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

            NonlinearRegression regression = new NonlinearRegression(2,  rate, grad);

            NonlinearLeastSquares nls = new NonlinearLeastSquares(regression);

            Assert.IsTrue(nls.Algorithm is LevenbergMarquardt);

            regression.Coefficients[0] = 0.9; // β1
            regression.Coefficients[1] = 0.2; // β2

            int iterations = 10;
            double[] errors = new double[iterations];
            for (int i = 0; i < errors.Length; i++)
                errors[i] = nls.Run(inputs, outputs);

            double b1 = regression.Coefficients[0];
            double b2 = regression.Coefficients[1];

            Assert.AreEqual(0.362, b1, 1e-3);
            Assert.AreEqual(0.556, b2, 1e-2);

            Assert.IsFalse(Double.IsNaN(b1));
            Assert.IsFalse(Double.IsNaN(b2));

            for (int i = 1; i < errors.Length; i++)
                Assert.IsTrue(errors[i - 1] >= errors[i]);
        }


    }
}
