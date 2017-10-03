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
    public class GaussianNewtonTest
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

            // diff a*exp(-0.5((m-x)/s)²) + b wrt s
            result[1] = (a * pow(m - x, 2) * exp(-(0.5 * pow(m - x, 2)) / (s * s))) / (s * s * s);

            // diff a*exp(-0.5((m-x)/s)²) + b wrt a
            result[2] = exp(-(0.5 * pow(x - m, 2)) / (s * s));

            // diff a*exp(-0.5((m-x)/s)²) + b wrt b
            result[3] = 1;
        }

        [Test]
        public void minimize_test()
        {
            #region doc_minimize
            // Example from https://en.wikipedia.org/wiki/Gauss%E2%80%93Newton_algorithm

            // In this example, the Gauss–Newton algorithm will be used to fit a model to 
            // some data by minimizing the sum of squares of errors between the data and 
            // model's predictions.

            // In a biology experiment studying the relation between substrate concentration [S]
            // and reaction rate in an enzyme-mediated reaction, the data in the following table
            // were obtained:

            double[][] inputs = Jagged.ColumnVector(new [] { 0.03, 0.1947, 0.425, 0.626, 1.253, 2.500, 3.740 });
            double[] outputs = new[] { 0.05, 0.127, 0.094, 0.2122, 0.2729, 0.2665, 0.3317 };

            // It is desired to find a curve (model function) of the form
            // 
            //   rate = \frac{V_{max}[S]}{K_M+[S]}
            //
            // that fits best the data in the least squares sense, with the parameters V_max
            // and K_M to be determined. Let's start by writing model equation below:

            LeastSquaresFunction function = (double[] parameters, double[] input) =>
            {
                return (parameters[0] * input[0]) / (parameters[1] + input[0]);
            };

            // Now, we can either write the gradient function of the model by hand or let
            // the model compute it automatically using Newton's finite differences method:

            LeastSquaresGradientFunction gradient = (double[] parameters, double[] input, double[] result) =>
            {
                result[0] = -((-input[0]) / (parameters[1] + input[0]));
                result[1] = -((parameters[0] * input[0]) / Math.Pow(parameters[1] + input[0], 2));
            };

            // Create a new Gauss-Newton algorithm
            var gn = new GaussNewton(parameters: 2)
            {
                Function = function,
                Gradient = gradient,
                Solution = new[] { 0.9, 0.2 } // starting from b1 = 0.9 and b2 = 0.2
            };

            // Find the minimum value:
            gn.Minimize(inputs, outputs);

            // The solution will be at:
            double b1 = gn.Solution[0]; // will be 0.362
            double b2 = gn.Solution[1]; // will be 0.556
            #endregion

            Assert.AreEqual(0.362, b1, 1e-3);
            Assert.AreEqual(0.556, b2, 3e-3);
        }

    }
}
