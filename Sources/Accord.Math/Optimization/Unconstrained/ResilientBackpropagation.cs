﻿// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="BoundedBroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// <seealso cref="TrustRegionNewtonMethod"/>
    /// 
    public class ResilientBackpropagation : BaseGradientOptimizationMethod, IGradientOptimizationMethod
    {

        private RelativeConvergence convergence;

        private double initialStep = 0.0125;
        private double deltaMax = 50.0;
        private double deltaMin = 1e-6;

        private double etaMinus = 0.5;
        private double etaPlus = 1.2;

        private double[] gradient;
        private double[] previousGradient;

        // update values, also known as deltas
        private double[] weightsUpdates;



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
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="function">The function to be optimized.</param>
        /// 
        public ResilientBackpropagation(NonlinearObjectiveFunction function)
            : this(function.NumberOfVariables, function.Function, function.Gradient)
        {
        }

        /// <summary>
        ///   Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// 
        public ResilientBackpropagation(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : base(numberOfVariables, function, gradient)
        {
            init(numberOfVariables);
        }

        /// <summary>
        ///   Creates a new <see cref="ResilientBackpropagation"/> function optimizer.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of parameters in the function to be optimized.</param>
        /// 
        public ResilientBackpropagation(int numberOfVariables)
            : base(numberOfVariables)
        {
            init(numberOfVariables);
        }

        private void init(int numberOfVariables)
        {
            convergence = new RelativeConvergence();

            gradient = new double[numberOfVariables];
            previousGradient = new double[numberOfVariables];
            weightsUpdates = new double[numberOfVariables];

            // Initialize steps
            Reset(initialStep);
        }


        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            convergence.Clear();

            do
            {
                runEpoch();
                if (Token.IsCancellationRequested)
                    break;
            }
            while (!convergence.HasConverged);

            return true;
        }




        private double runEpoch()
        {
            // Compute the true gradient
            gradient = Gradient(Solution);

            double[] parameters = Solution;

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

            Accord.Diagnostics.Debug.Assert(!parameters.HasNaN());

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
