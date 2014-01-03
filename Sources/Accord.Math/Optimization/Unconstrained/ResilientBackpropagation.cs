// Accord Math Library
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

namespace Accord.Math.Optimization
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    ///   Resilient Backpropagation method for unconstrained optimization.
    /// </summary>
    /// 
    public class ResilientBackpropagation : IGradientOptimizationMethod
    {

        private AbsoluteConvergence convergence;

        private double initialStep = 0.0125;
        private double deltaMax = 50.0;
        private double deltaMin = 1e-6;

        private double etaMinus = 0.5;
        private double etaPlus = 1.2;

        private double[] solution;
        private double[] gradient;
        private double[] previousGradient;

        // update values, also known as deltas
        private double[] weightsUpdates;

        /// <summary>
        ///   Gets the solution found; the values for  
        ///   the parameters that optimize the function.
        /// </summary>
        /// 
        public double[] Solution
        {
            get { return solution; }
        }

        /// <summary>
        ///   Gets or sets the function to be optimized.
        /// </summary>
        /// 
        /// <value>The function to be optimized.</value>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets or sets a function returning the gradient
        ///   vector of the function to be optimized for a
        ///   given value of its free parameters.
        /// </summary>
        /// 
        /// <value>The gradient function.</value>
        /// 
        public Func<double[], double[]> Gradient { get; set; }

        /// <summary>
        ///   Occurs when the current learning progress has changed.
        /// </summary>
        /// 
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;



        /// <summary>
        ///   Gets or sets the maximum possible update step,
        ///   also referred as delta min. Default is 50.
        /// </summary>
        /// 
        public double UpdateUpperBound
        {
            get { return deltaMax; }
            set { deltaMax = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum possible update step,
        ///   also referred as delta max. Default is 1e-6.
        /// </summary>
        /// 
        public double UpdateLowerBound
        {
            get { return deltaMin; }
            set { deltaMin = value; }
        }

        /// <summary>
        ///   Gets the decrease parameter, also 
        ///   referred as eta minus. Default is 0.5.
        /// </summary>
        /// 
        public double DecreaseFactor
        {
            get { return etaMinus; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException("value", "Value should be between 0 and 1.");
                etaMinus = value;
            }
        }

        /// <summary>
        ///   Gets the increase parameter, also
        ///   referred as eta plus. Default is 1.2.
        /// </summary>
        /// 
        public double IncreaseFactor
        {
            get { return etaPlus; }
            set
            {
                if (value <= 1)
                    throw new ArgumentOutOfRangeException("value", "Value should be higher than 1.");
                etaPlus = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        public int Iterations
        {
            get { return convergence.Iterations; }
            set { convergence.Iterations = value; }
        }

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters.
        /// </value>
        /// 
        public int Parameters
        {
            get { return solution.Length; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ResilientBackpropagation"/> class.
        /// </summary>
        /// 
        /// <param name="parameters">The number of parameters in the function to be optimized.</param>
        /// 
        public ResilientBackpropagation(int parameters)
        {
            convergence = new AbsoluteConvergence();

            solution = new double[parameters];
            gradient = new double[parameters];
            previousGradient = new double[parameters];
            weightsUpdates = new double[parameters];

            // Initialize steps
            Reset(initialStep);

            for (int i = 0; i < solution.Length; i++)
                solution[i] = Accord.Math.Tools.Random.NextDouble() * 2.0 - 1.0;
        }


        /// <summary>
        ///   Optimizes the defined function.
        /// </summary>
        /// 
        public double Minimize()
        {
            return Minimize(Solution);
        }

        /// <summary>
        ///   Optimizes the defined function.
        /// </summary>
        /// 
        /// <param name="values">The initial guess values for the parameters.</param>
        /// 
        public double Minimize(double[] values)
        {
            solution = values;
            convergence.Clear();

            do
            {
                runEpoch();
            }
            while (!convergence.HasConverged);

            return convergence.NewValue;
        }


        private double runEpoch()
        {
            // Compute the true gradient
            gradient = Gradient(solution);

            double[] parameters = solution;

            // Do the Resilient Backpropagation parameter update
            for (int k = 0; k < parameters.Length; k++)
            {
                if (Double.IsInfinity(parameters[k])) continue;

                double S = previousGradient[k] * gradient[k];

                if (S > 0.0)
                {
                    weightsUpdates[k] = Math.Min(weightsUpdates[k] * etaPlus, deltaMax);
                    parameters[k] -= Math.Sign(gradient[k]) * weightsUpdates[k];
                    previousGradient[k] = gradient[k];
                }
                else if (S < 0.0)
                {
                    weightsUpdates[k] = Math.Max(weightsUpdates[k] * etaMinus, deltaMin);
                    previousGradient[k] = 0.0;
                }
                else
                {
                    parameters[k] -= Math.Sign(gradient[k]) * weightsUpdates[k];
                    previousGradient[k] = gradient[k];
                }
            }

            System.Diagnostics.Debug.Assert(!parameters.HasNaN());

            double value = Function(parameters);

            return convergence.NewValue = value;
        }

        /// <summary>
        ///   Raises the <see cref="E:ProgressChanged"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnProgressChanged(ProgressChangedEventArgs args)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, args);
        }

        /// <summary>
        ///   Resets the current update steps using the given learning rate.
        /// </summary>
        /// 
        public void Reset(double rate)
        {
            convergence.Clear();

            Parallel.For(0, weightsUpdates.Length, i =>
            {
                for (int j = 0; j < weightsUpdates.Length; j++)
                    weightsUpdates[i] = rate;
            });
        }

    }
}
