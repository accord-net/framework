// Accord Math Library
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

namespace Accord.Math.Optimization
{
    using System;
    using Accord.Math;
    using Accord.MachineLearning;
    using Accord.Statistics.Models;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for least-squares optimizers implementing the <see cref="ILeastSquaresMethod"/> interface.
    /// </summary>
    /// 
    /// <seealso cref="Accord.MachineLearning.ParallelLearningBase" />
    /// <seealso cref="Accord.Statistics.Models.IConvergenceLearning" />
    /// 
    public abstract class BaseLeastSquaresMethod : ParallelLearningBase, IConvergenceLearning
    {
        // Total of weights in the model
        private int numberOfParameters;

        double[] solution;

        RelativeConvergence convergence;


        /// <summary>
        ///   Gets or sets a parameterized model function mapping input vectors
        ///   into output values, whose optimum parameters must be found.
        /// </summary>
        /// 
        /// <value>
        ///   The function to be optimized.
        /// </value>
        /// 
        public LeastSquaresFunction Function { get; set; }


        /// <summary>
        ///   Gets or sets a function that computes the gradient vector in respect
        ///   to the function parameters, given a set of input and output values.
        /// </summary>
        /// 
        /// <value>
        ///   The gradient function.
        /// </value>
        /// 
        public LeastSquaresGradientFunction Gradient { get; set; }

        /// <summary>
        ///   Gets the solution found, the values of the parameters which
        ///   optimizes the function, in a least squares sense.
        /// </summary>
        /// 
        public double[] Solution
        {
            get { return solution; }
            set
            {
                if (value.Length != numberOfParameters)
                    throw new ArgumentException("Parameter vectors must have the same length", "value");
                this.solution = value;
            }
        }

        /// <summary>
        ///   Gets the number of variables (free parameters) in the optimization problem.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters.
        /// </value>
        /// 
        [Obsolete("Please use NumberOfParameters instead.")]
        public int NumberOfVariables
        {
            get { return NumberOfParameters; }
            set { NumberOfParameters = value; }
        }

        /// <summary>
        ///   Gets the number of variables (free parameters) in the optimization problem.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters.
        /// </value>
        /// 
        public int NumberOfParameters
        {
            get { return numberOfParameters; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                init(value);
            }
        }

        /// <summary>
        /// Gets the value at the solution found. This should be
        /// the minimum value found for the objective function.
        /// </summary>
        /// 
        public double Value { get; set; }

        /// <summary>
        ///   Gets or sets the convergence verification method.
        /// </summary>
        /// 
        protected RelativeConvergence Convergence
        {
            get { return convergence; }
            set { convergence = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the iterative algorithm. Default
        ///   is 100.
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum relative change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        ///   Default is zero.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Please use MaxIterations instead.
        /// </summary>
        /// 
        [Obsolete("Please use MaxIterations instead.")]
        public int Iterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        ///   Gets the current iteration number.
        /// </summary>
        /// 
        public int CurrentIteration
        {
            get { return convergence.CurrentIteration; }
            set { convergence.CurrentIteration = value; }
        }

        /// <summary>
        ///   Gets whether the algorithm has converged.
        /// </summary>
        /// 
        public bool HasConverged
        {
            get { return convergence.HasConverged; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLeastSquaresMethod"/> class.
        /// </summary>
        /// 
        public BaseLeastSquaresMethod()
        {
            this.convergence = new RelativeConvergence(0, 1e-5);
        }

        private void init(int numberOfParameters)
        {
            this.numberOfParameters = numberOfParameters;

            if (this.solution == null || this.solution.Length != numberOfParameters)
            {
                this.solution = new double[numberOfParameters];
                Initialize();
            }
        }

        /// <summary>
        ///   This method should be implemented by child classes to initialize
        ///   their fields once the <see cref="NumberOfParameters"/> is known.
        /// </summary>
        /// 
        protected abstract void Initialize();

        /// <summary>
        ///   Compute model error for a given data set.
        /// </summary>
        /// 
        /// <param name="input">The input points.</param>
        /// <param name="output">The output points.</param>
        /// 
        /// <returns>The sum of squared errors for the data.</returns>
        /// 
        public double ComputeError(double[][] input, double[] output)
        {
            double sumOfSquaredErrors = 0;

            for (int i = 0; i < input.Length; i++)
            {
                double actual = Function(Solution, input[i]);
                double expected = output[i];

                double e = expected - actual;
                sumOfSquaredErrors += e * e;
            }

            return sumOfSquaredErrors / 2.0;
        }

    }
}
