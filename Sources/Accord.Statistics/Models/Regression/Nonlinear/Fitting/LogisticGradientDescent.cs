// Accord Statistics Library
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

namespace Accord.Statistics.Models.Regression.Fitting
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Stochastic Gradient Descent learning for Logistic Regression fitting.
    /// </summary>
    /// 
    public class LogisticGradientDescent : IRegressionFitting
    {

        private LogisticRegression regression;

        private int parameterCount;
        private bool stochastic = false;

        private double rate = 0.1;

        private double[] gradient;
        private double[] previous;
        private double[] deltas;

        /// <summary>
        ///   Gets the previous values for the coefficients which were
        ///   in place before the last learning iteration was performed.
        /// </summary>
        /// 
        public double[] Previous { get { return previous; } }

        /// <summary>
        ///   Gets the current values for the coefficients.
        /// </summary>
        /// 
        public double[] Solution { get { return regression.Coefficients; } }

        /// <summary>
        ///   Gets the Gradient vector computed in
        ///   the last Newton-Raphson iteration.
        /// </summary>
        /// 
        public double[] Gradient { get { return gradient; } }

        /// <summary>
        ///   Gets the total number of parameters in the model.
        /// </summary>
        /// 
        public int Parameters { get { return parameterCount; } }

        /// <summary>
        ///   Gets or sets whether this algorithm should use
        ///   stochastic updates or not. Default is false.
        /// </summary>
        /// 
        public bool Stochastic
        {
            get { return stochastic; }
            set { stochastic = value; }
        }

        /// <summary>
        ///   Gets or sets the algorithm
        ///   learning rate. Default is 0.1.
        /// </summary>
        /// 
        public double LearningRate
        {
            get { return rate; }
            set { rate = value; }
        }


        /// <summary>
        ///   Constructs a new Gradient Descent algorithm.
        /// </summary>
        /// 
        /// <param name="regression">The regression to estimate.</param>
        /// 
        public LogisticGradientDescent(LogisticRegression regression)
        {
            this.regression = regression;

            this.parameterCount = regression.Coefficients.Length;

            this.gradient = new double[parameterCount];
            this.deltas = new double[parameterCount];
        }

        /// <summary>
        ///   Runs one iteration of the Reweighted Least Squares algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The outputs associated with each input vector.</param>
        /// 
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        public double Run(double[][] inputs, double[][] outputs)
        {
            if (outputs[0].Length != 1)
                throw new ArgumentException("Function must have a single output.", "outputs");

            double[] output = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
                output[i] = outputs[i][0];

            return Run(inputs, output);
        }

        /// <summary>
        ///   Runs a single pass of the gradient descent algorithm.
        /// </summary>
        /// 
        public double Run(double[] input, double output)
        {
            // Initial definitions and memory allocations
            double[] coefficients = regression.Coefficients;
            this.previous = (double[])coefficients.Clone();


            // 1. Compute local gradient estimate
            double actual = regression.Compute(input);
            double error = output - actual;

            gradient[0] = error;
            for (int i = 0; i < input.Length; i++)
                gradient[i + 1] = input[i] * error;

            // 2. Update using the local estimate
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] += rate * gradient[i];

            // 3. Return maximum parameter change
            for (int i = 0; i < previous.Length; i++)
                deltas[i] = Math.Abs(coefficients[i] - previous[i]) / Math.Abs(previous[i]);

            return Matrix.Max(deltas);
        }

        /// <summary>
        ///   Runs one iteration of the Reweighted Least Squares algorithm.
        /// </summary>
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The outputs associated with each input vector.</param>
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        public double Run(double[][] inputs, double[] outputs)
        {
            // Initial definitions and memory allocations
            double[] coefficients = this.regression.Coefficients;
            double[] previous = (double[])coefficients.Clone();


            if (stochastic)
            {
                // Use stochastic gradient estimates 
                for (int i = 0; i < inputs.Length; i++)
                {
                    Run(inputs[i], outputs[i]);
                }
            }
            else
            {
                // Compute the complete error gradient
                Array.Clear(gradient, 0, gradient.Length);

                for (int i = 0; i < inputs.Length; i++)
                {
                    double actual = regression.Compute(inputs[i]);
                    double error = outputs[i] - actual;

                    gradient[0] += error;
                    for (int j = 0; j < inputs[i].Length; j++)
                        gradient[j + 1] += inputs[i][j] * error;
                }

                // Update coefficients using the gradient
                for (int i = 0; i < coefficients.Length; i++)
                    coefficients[i] += rate * gradient[i];
            }


            // Return the maximum parameter change
            this.previous = previous;
            for (int i = 0; i < previous.Length; i++)
                deltas[i] = Math.Abs(coefficients[i] - previous[i]) / Math.Abs(previous[i]);

            return Matrix.Max(deltas);
        }

        /// <summary>
        ///   Computes the sum-of-squared error between the
        ///   model outputs and the expected outputs.
        /// </summary>
        /// 
        /// <param name="inputs">The input data set.</param>
        /// <param name="outputs">The output values.</param>
        /// 
        /// <returns>The sum-of-squared errors.</returns>
        /// 
        public double ComputeError(double[][] inputs, double[] outputs)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                double actual = regression.Compute(inputs[i]);
                double expected = outputs[i];
                double delta = actual - expected;
                sum += delta * delta;
            }

            return sum;
        }

    }
}
