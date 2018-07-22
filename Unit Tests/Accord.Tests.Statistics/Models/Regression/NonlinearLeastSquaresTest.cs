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
    using System;
    using Accord.Math;
    using Accord.Math.Differentiation;
    using Accord.Math.Optimization;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using NUnit.Framework;
    using Accord.Math.Optimization.Losses;
    using System.Threading.Tasks;

    [TestFixture]
    public class NonLinearLeastSquaresTest
    {

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
            // double b = parameters[3]; // not needed
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

        [Test]
        public void learn_test()
        {
            #region doc_learn_lm
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
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

            // Extract inputs and outputs
            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] outputs = data.GetColumn(1);

            // Create a Nonlinear regression using 
            var nls = new NonlinearLeastSquares()
            {
                NumberOfParameters = 3,

                // Initialize to some random values
                StartValues = new[] { 4.2, 0.3, 1 },

                // Let's assume a quadratic model function: ax² + bx + c
                Function = (w, x) => w[0] * x[0] * x[0] + w[1] * x[0] + w[2],

                // Derivative in respect to the weights:
                Gradient = (w, x, r) =>
                {
                    r[0] = w[0] * w[0]; // w.r.t a: a²  // https://www.wolframalpha.com/input/?i=diff+ax²+%2B+bx+%2B+c+w.r.t.+a
                    r[1] = w[0];       // w.r.t b: b   // https://www.wolframalpha.com/input/?i=diff+ax²+%2B+bx+%2B+c+w.r.t.+b
                    r[2] = 1;          // w.r.t c: 1   // https://www.wolframalpha.com/input/?i=diff+ax²+%2B+bx+%2B+c+w.r.t.+c
                },

                Algorithm = new LevenbergMarquardt()
                {
                    MaxIterations = 100,
                    Tolerance = 0
                }
            };


            var regression = nls.Learn(inputs, outputs);

            // Use the function to compute the input values
            double[] predict = regression.Transform(inputs);
            #endregion

            Assert.IsTrue(nls.Algorithm is LevenbergMarquardt);

            double error = new SquareLoss(outputs)
            {
                Mean = false
            }.Loss(predict) / 2.0;

            Assert.AreEqual(1616964052.1048875, error, 1e7);

            Assert.AreEqual(-16.075187551945078, regression.Coefficients[0], 1e-3);
            Assert.AreEqual(-221.50453233335202, regression.Coefficients[1], 1e-3);
            Assert.AreEqual(1995.1774385125705, regression.Coefficients[2], 1e-3);

            Assert.AreEqual(-14864.941351259276, predict[0], 1e-10);
            Assert.AreEqual(-5827.35538823598, predict[1], 1e-10);
            Assert.AreEqual(-4.8069356009871171, predict[2], 1e-10);
            Assert.AreEqual(-1827.3866400257925, predict[5], 1e-10);
        }

        [Test]
        public void simple_gauss_newton_test()
        {
            #region doc_learn_gn
            // Suppose we would like to map the continuous values in the
            // second row to the integer values in the first row.
            double[,] data =
            {
                { 0.03, 0.1947, 0.425, 0.626, 1.253, 2.500, 3.740 },
                { 0.05, 0.127, 0.094, 0.2122, 0.2729, 0.2665, 0.3317}
            };

            // Extract inputs and outputs
            double[][] inputs = data.GetRow(0).ToJagged();
            double[] outputs = data.GetRow(1);

            // Create a Nonlinear regression using 
            var nls = new NonlinearLeastSquares()
            {
                // Initialize to some random values
                StartValues = new[] { 0.9, 0.2 },

                // Let's assume a quadratic model function: ax² + bx + c
                Function = (w, x) => (w[0] * x[0]) / (w[1] + x[0]),

                // Derivative in respect to the weights:
                Gradient = (w, x, r) =>
                {
                    r[0] = -((-x[0]) / (w[1] + x[0]));
                    r[1] = -((w[0] * x[0]) / Math.Pow(w[1] + x[0], 2));
                },

                Algorithm = new GaussNewton()
                {
                    MaxIterations = 0,
                    Tolerance = 1e-5
                }
            };


            var regression = nls.Learn(inputs, outputs);

            // Use the function to compute the input values
            double[] predict = regression.Transform(inputs);
            #endregion

            var alg = nls.Algorithm as GaussNewton;
            Assert.AreEqual(0, alg.MaxIterations);
            Assert.AreEqual(1e-5, alg.Tolerance);
            Assert.AreEqual(6, alg.CurrentIteration);

            double error = new SquareLoss(outputs)
            {
                Mean = false
            }.Loss(predict) / 2.0;

            Assert.AreEqual(0.004048452937977628, error, 1e-8);

            double b1 = regression.Coefficients[0];
            double b2 = regression.Coefficients[1];

            Assert.AreEqual(0.362, b1, 1e-3);
            Assert.AreEqual(0.556, b2, 3e-3);

            Assert.AreEqual(1.23859, regression.StandardErrors[0], 1e-3);
            Assert.AreEqual(6.06352, regression.StandardErrors[1], 5e-3);
        }


        [Test]
        public void ExampleTest()
        {
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
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

            // Extract inputs and outputs
            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] outputs = data.GetColumn(1);

            // Create a Nonlinear regression using 
            var regression = new NonlinearRegression(3,

                // Let's assume a quadratic model function: ax² + bx + c
                function: (w, x) => w[0] * x[0] * x[0] + w[1] * x[0] + w[2],

                // Derivative in respect to the weights:
                gradient: (w, x, r) =>
                {
                    r[0] = 2 * w[0]; // w.r.t a: 2a  
                    r[1] = w[1];     // w.r.t b: b
                    r[2] = w[2];     // w.r.t c: 0
                }
            );

            // Create a non-linear least squares teacher
            var nls = new NonlinearLeastSquares(regression);

            // Initialize to some random values
            regression.Coefficients[0] = 4.2;
            regression.Coefficients[1] = 0.3;
            regression.Coefficients[2] = 1;

            // Run the function estimation algorithm
            double error = Double.PositiveInfinity;
            for (int i = 0; i < 100; i++)
                error = nls.Run(inputs, outputs);

            // Use the function to compute the input values
            double[] predict = inputs.Apply(regression.Compute);

            Assert.IsTrue(nls.Algorithm is LevenbergMarquardt);

            Assert.AreEqual(2145404235.739383, error, 1e-7);

            Assert.AreEqual(-11.916652026711853, regression.Coefficients[0], 1e-3);
            Assert.AreEqual(-358.9758898959638, regression.Coefficients[1], 1e-3);
            Assert.AreEqual(-107.31273008811895, regression.Coefficients[2], 1e-3);

            Assert.AreEqual(-4814.9203769986034, predict[0], 1e-10);
            Assert.AreEqual(-63.02285725721211, predict[1], 1e-10);
            Assert.AreEqual(2305.5442571416661, predict[2], 1e-10);
            Assert.AreEqual(-4888.736831716782, predict[5], 1e-10);
        }


        [Test]
        public void RunTest()
        {
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
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

            // Extract inputs and outputs
            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] outputs = data.GetColumn(1);

            // Create a Nonlinear regression using 
            NonlinearRegression regression = new NonlinearRegression(4, function, gradient);


            NonlinearLeastSquares nls = new NonlinearLeastSquares(regression);

            Assert.IsTrue(nls.Algorithm is LevenbergMarquardt);

            regression.Coefficients[0] = 0; // m
            regression.Coefficients[1] = 80; // s
            regression.Coefficients[2] = 53805; // a
            regression.Coefficients[3] = -21330.11; //b

            double error = Double.PositiveInfinity;
            for (int i = 0; i < 100; i++)
                error = nls.Run(inputs, outputs);

            double m = regression.Coefficients[0];
            double s = regression.Coefficients[1];
            double a = regression.Coefficients[2];
            double b = regression.Coefficients[3];

            Assert.AreEqual(10345587.465428978, error, 1e-6);

            Assert.AreEqual(5.3161961121998953, m, 1e-3);
            Assert.AreEqual(-12.792301015831979, s, 1e-3);
            Assert.AreEqual(56794.832645792514, a, 1e-3);
            Assert.AreEqual(-20219.675997523173, b, 1e-2);
        }

        [Test]
        public void RunTest1()
        {
            // Example from https://en.wikipedia.org/wiki/Gauss%E2%80%93Newton_algorithm

            double[,] data =
            {
                { 0.03, 0.1947, 0.425, 0.626, 1.253, 2.500, 3.740 },
                { 0.05, 0.127, 0.094, 0.2122, 0.2729, 0.2665, 0.3317}
            };

            double[][] inputs = data.GetRow(0).ToJagged();
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

            var gn = new GaussNewton(2);
            gn.ParallelOptions.MaxDegreeOfParallelism = 1;
            NonlinearLeastSquares nls = new NonlinearLeastSquares(regression, gn);

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

            for (int i = 1; i < errors.Length; i++)
                Assert.IsTrue(errors[i - 1] >= errors[i]);

            Assert.AreEqual(1.23859, regression.StandardErrors[0], 1e-3);
            Assert.AreEqual(6.06352, regression.StandardErrors[1], 3e-3);
        }

    }
}
