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
    using Accord.Math;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Gauss-Newton algorithm for solving Least-Squares problems.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class isn't suitable for most real-world problems. Instead, this class
    ///   is intended to be use as a baseline comparison to help debug and check other
    ///   optimization methods, such as <see cref="LevenbergMarquardt"/>.
    /// </remarks>
    /// 
    public class GaussNewton : ILeastSquaresMethod
    {

        private int numberOfParameters;

        private double[] weights;
        
        private double[] gradient;
        private double[] errors;
        private double[] deltas;

        private double[,] hessian;
        private double[,] jacobian;

        private SingularValueDecomposition decomposition;


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
        ///   Gets the number of variables (free parameters) in the optimization problem.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters.
        /// </value>
        /// 
        public int Parameters
        {
            get { return numberOfParameters; }
        }

        /// <summary>
        ///   Gets the approximate Hessian matrix of second derivatives
        ///   created during the last algorithm iteration.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   Please note that this value is actually just an approximation to the
        ///   actual Hessian matrix using the outer Jacobian approximation (H ~ J'J).
        /// </para>
        /// </remarks>
        /// 
        public double[,] Hessian
        {
            get { return hessian; }
        }

        /// <summary>
        ///   Gets the solution found, the values of the parameters which
        ///   optimizes the function, in a least squares sense.
        /// </summary>
        /// 
        public double[] Solution
        {
            get { return weights; }
            set
            {
                if (value.Length != numberOfParameters)
                    throw new ArgumentException("Parameter vectors must have the same length", "value");
                this.weights = value;
            }
        }

        /// <summary>
        ///   Gets the vector of residuals computed in the last iteration.
        ///   The residuals are computed as <c>(y - f(w, x))</c>, in which 
        ///   <c>y</c> are the expected output values, and <c>f</c> is the
        ///   parameterized model function.
        /// </summary>
        /// 
        public double[] Residuals
        {
            get { return errors; }
        }

        /// <summary>
        ///   Gets the Jacobian matrix of first derivatives computed in the
        ///   last iteration.
        /// </summary>
        /// 
        public double[,] Jacobian
        {
            get { return jacobian; }
        }

        /// <summary>
        ///   Gets the vector of coefficient updates computed in the last iteration.
        /// </summary>
        /// 
        public double[] Deltas
        {
            get { return deltas; }
        }

        /// <summary>
        ///   Gets standard error for each parameter in the solution.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get { return decomposition.Inverse().Diagonal().Sqrt(); }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussNewton"/> class.
        /// </summary>
        /// 
        /// <param name="parameters">The number of variables (free parameters)
        ///   in the objective function.</param>
        /// 
        public GaussNewton(int parameters)
        {
            this.numberOfParameters = parameters;

            this.hessian = new double[numberOfParameters, numberOfParameters];
            this.gradient = new double[numberOfParameters];
            this.weights = new double[numberOfParameters];
        }


        /// <summary>
        ///   Attempts to find the best values for the parameter vector
        ///   minimizing the discrepancy between the generated outputs
        ///   and the expected outputs for a given set of input data.
        /// </summary>
        /// 
        /// <param name="inputs">A set of input data.</param>
        /// <param name="outputs">The values associated with each 
        ///   vector in the <paramref name="inputs"/> data.</param>
        /// 
        public double Minimize(double[][] inputs, double[] outputs)
        {
            Array.Clear(hessian, 0, hessian.Length);
            Array.Clear(gradient, 0, gradient.Length);


            errors = new double[inputs.Length];
            jacobian = new double[inputs.Length, numberOfParameters];


            for (int i = 0; i < inputs.Length; i++)
                errors[i] = outputs[i] - Function(weights, inputs[i]);

            double[] g = new double[numberOfParameters];
            for (int i = 0; i < inputs.Length; i++)
            {
                Gradient(weights, inputs[i], result: g);

                for (int j = 0; j < gradient.Length; j++)
                    jacobian[i, j] = -g[j];
            }


            // Compute error gradient using Jacobian
            jacobian.TransposeAndMultiply(errors, result: gradient);

            // Compute Quasi-Hessian Matrix approximation
            jacobian.TransposeAndMultiply(jacobian, result: hessian);

            decomposition = new SingularValueDecomposition(hessian,
                computeLeftSingularVectors: true, computeRightSingularVectors: true, autoTranspose: true);

            deltas = decomposition.Solve(gradient);

            for (int i = 0; i < deltas.Length; i++)
                weights[i] -= deltas[i];

            return ComputeError(inputs, outputs);
        }



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
                double y = Function(weights, input[i]);

                double e = y - output[i];
                sumOfSquaredErrors += e * e;
            }

            return sumOfSquaredErrors;
        }

    }
}

