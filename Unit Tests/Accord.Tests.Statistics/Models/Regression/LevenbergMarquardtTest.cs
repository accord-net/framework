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
    public class LevenbergMarquardtTest
    {
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

            double[][] inputs = Jagged.ColumnVector(new[] { 0.03, 0.1947, 0.425, 0.626, 1.253, 2.500, 3.740 });
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

            // Create a new Levenberg-Marquardt algorithm
            var gn = new LevenbergMarquardt(parameters: 2)
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



        [Test]
        public void misra1a_test()
        {
            LevenbergMarquardt lm = new LevenbergMarquardt(2);

            double[] outputs = new double[] { 10.07E0, 14.73E0, 17.94E0, 23.93E0, 29.61E0, 35.18E0, 40.02E0, 44.82E0, 50.76E0, 55.05E0, 61.01E0, 66.40E0, 75.47E0, 81.78E0 };
            double[][] inputs = Jagged.ColumnVector(77.6E0, 114.9E0, 141.1E0, 190.8E0, 239.9E0, 289.0E0, 332.8E0, 378.4E0, 434.8E0, 477.3E0, 536.8E0, 593.1E0, 689.1E0, 760.0E0);

            lm.Function = Function;
            lm.Gradient = Gradient;
            lm.Solution = new double[] { 250, 0.0005 }; // starting values
            double error = lm.Minimize(inputs, outputs);

            Assert.AreEqual(238.944658680792, lm.Solution[0], 1e-5);
            Assert.AreEqual(0.00055014847409921093, lm.Solution[1], 1e-5);
        }

        double Function(double[] parameters, double[] input)
        {
            return parameters[0] * (1 - Math.Exp(-parameters[1] * input[0]));
        }

        void Gradient(double[] parameters, double[] input, double[] result)
        {
            result[0] = 1 - Math.Exp(-parameters[1] * input[0]);
            result[1] = parameters[0] * input[0] / Math.Exp(parameters[1] * input[0]);
        }
    }
}
