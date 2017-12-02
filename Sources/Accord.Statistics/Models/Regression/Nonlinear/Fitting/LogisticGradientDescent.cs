// Accord Statistics Library
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

namespace Accord.Statistics.Models.Regression.Fitting
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.MachineLearning;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Stochastic Gradient Descent learning for Logistic Regression fitting.
    /// </summary>
    /// 
#pragma warning disable 612, 618
    public class LogisticGradientDescent :
        ISupervisedLearning<LogisticRegression, double[], int>,
        ISupervisedLearning<LogisticRegression, double[], bool>,
        ISupervisedLearning<LogisticRegression, double[], double>,
        IRegressionFitting, IConvergenceLearning
#pragma warning restore 612, 618
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private LogisticRegression regression;

        private int parameterCount;
        private bool stochastic = false;
        private RelativeParameterConvergence convergence;

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
        public double[] Solution
        {
            get { return regression.Intercept.Concatenate(regression.Weights); }
        }

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
        ///   Please use MaxIterations instead.
        /// </summary>
        [Obsolete("Please use MaxIterations instead.")]
        public int Iterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of iterations
        /// performed by the learning algorithm.
        /// </summary>
        public int MaxIterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        /// Gets or sets the tolerance value used to determine
        /// whether the algorithm has converged.
        /// </summary>
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        /// Gets the current iteration number.
        /// </summary>
        /// <value>The current iteration.</value>
        public int CurrentIteration
        {
            get { return convergence.CurrentIteration; }
        }

        /// <summary>
        /// Gets or sets whether the algorithm has converged.
        /// </summary>
        /// <value><c>true</c> if this instance has converged; otherwise, <c>false</c>.</value>
        public bool HasConverged
        {
            get { return convergence.HasConverged; }
        }

        /// <summary>
        ///   Constructs a new Gradient Descent algorithm.
        /// </summary>
        /// 
        public LogisticGradientDescent()
        {
            convergence = new RelativeParameterConvergence()
            {
                MaxIterations = 0,
                Tolerance = 1e-8
            };
        }

        /// <summary>
        ///   Constructs a new Gradient Descent algorithm.
        /// </summary>
        /// 
        /// <param name="regression">The regression to estimate.</param>
        /// 
        public LogisticGradientDescent(LogisticRegression regression)
            : this()
        {
            init(regression);
        }

        private void init(LogisticRegression regression)
        {
            this.regression = regression;

            this.parameterCount = regression.NumberOfParameters;

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
        [Obsolete("Please use the Learn(x, y) method instead.")]
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
        [Obsolete("Please use the Learn(x, y) method instead.")]
        public double Run(double[] input, double output)
        {
            int old = convergence.Iterations;
            convergence.Iterations = 1;
            Learn(new[] { input }, new[] { output });
            convergence.Iterations = old;
            return Matrix.Max(deltas);
        }

        /// <summary>
        ///   Runs one iteration of the Reweighted Least Squares algorithm.
        /// </summary>
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The outputs associated with each input vector.</param>
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        [Obsolete("Please use the Learn(x, y) method instead.")]
        public double Run(double[][] inputs, double[] outputs)
        {
            int old = convergence.Iterations;
            convergence.Iterations = 1;
            Learn(inputs, outputs);
            convergence.Iterations = old;
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
        [Obsolete("Please use the LogLikelihoodLoss class instead.")]
        public double ComputeError(double[][] inputs, double[] outputs)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
#pragma warning disable 612, 618
                double actual = regression.Compute(inputs[i]);
#pragma warning restore 612, 618
                double expected = outputs[i];
                double delta = actual - expected;
                sum += delta * delta;
            }

            return sum;
        }


        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public LogisticRegression Learn(double[][] x, int[] y, double[] weights = null)
        {
            return Learn(x, y.ToDouble(), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public LogisticRegression Learn(double[][] x, bool[] y, double[] weights = null)
        {
            return Learn(x, y.ToDouble(), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public LogisticRegression Learn(double[][] x, double[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (regression == null)
            {
                init(new LogisticRegression()
                {
                    NumberOfInputs = x.Columns()
                });
            }

            // Initial definitions and memory allocations
            double[] previous = (double[])Solution.Clone();
            convergence.Clear();

            do
            {
                if (Token.IsCancellationRequested)
                    break;

                if (stochastic)
                {
                    for (int j = 0; j < x.Length; j++)
                    {
                        // 1. Compute local gradient estimate
                        double z = regression.Linear.Transform(x[j]);
                        double actual = regression.Link.Inverse(z);
                        double error = y[j] - actual;

                        gradient[0] = error;
                        for (int i = 0; i < x[j].Length; i++)
                            gradient[i + 1] = x[j][i] * error;

                        // 2. Update using the local estimate
                        for (int i = 0; i < regression.Weights.Length; i++)
                            regression.Weights[i] += rate * gradient[i + 1];
                        regression.Intercept += rate * gradient[0];
                    }
                }
                else
                {
                    // Compute the complete error gradient
                    Array.Clear(gradient, 0, gradient.Length);

                    for (int i = 0; i < x.Length; i++)
                    {
                        // 1. Compute local gradient estimate
                        double z = regression.Linear.Transform(x[i]);
                        double actual = regression.Link.Inverse(z);
                        double error = y[i] - actual;

                        gradient[0] += error;
                        for (int j = 0; j < x[i].Length; j++)
                            gradient[j + 1] += x[i][j] * error;
                    }

                    // Update coefficients using the gradient
                    for (int i = 0; i < regression.Weights.Length; i++)
                        regression.Weights[i] += rate * gradient[i + 1] / x.Length;
                    regression.Intercept += rate * gradient[0] / x.Length;
                }

                // Return the maximum parameter change
                this.previous = previous;
                for (int i = 0; i < previous.Length; i++)
                    deltas[i] = Math.Abs(regression.GetCoefficient(i) - previous[i]) / Math.Abs(previous[i]);

                convergence.NewValues = this.Solution;

                if (Token.IsCancellationRequested)
                    return regression;

            } while (!convergence.HasConverged);

            return regression;
        }
    }
}
