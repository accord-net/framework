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
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Levenberg-Marquardt algorithm for solving Least-Squares problems.
    /// </summary>
    /// 
    public class LevenbergMarquardt : ILeastSquaresMethod
    {
        private const double lambdaMax = 1e25;


        // Levenberg-Marquardt variables
        private double[][] jacobian;
        private double[][] hessian;

        private double[] diagonal;
        private double[] gradient;
        private double[] weights;
        private double[] solution;
        private double[] deltas;
        private double[] errors;

        // Levenberg damping factor
        private double lambda = 0.1;

        // The amount the damping factor is adjusted
        // when searching the minimum error surface
        private double v = 10.0;

        // Total of weights in the network
        private int numberOfParameters;

        private int blocks = 1;
        private int outputCount = 1;

        JaggedCholeskyDecomposition decomposition;


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
        ///   Levenberg's damping factor, also known as lambda.
        /// </summary>
        /// 
        /// <remarks><para>The value determines speed of learning.</para>
        /// 
        /// <para>Default value is <b>0.1</b>.</para>
        /// </remarks>
        ///
        public double LearningRate
        {
            get { return lambda; }
            set { lambda = value; }
        }

        /// <summary>
        ///   Learning rate adjustment. 
        /// </summary>
        /// 
        /// <remarks><para>The value by which the learning rate
        /// is adjusted when searching for the minimum cost surface.</para>
        /// 
        /// <para>Default value is <b>10</b>.</para>
        /// </remarks>
        ///
        public double Adjustment
        {
            get { return v; }
            set { v = value; }
        }


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
        ///   Gets or sets the number of blocks to divide the 
        ///   Jacobian matrix in the Hessian calculation to
        ///   preserve memory. Default is 1.
        /// </summary>
        /// 
        public int Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }

        /// <summary>
        ///   Gets the approximate Hessian matrix of second derivatives 
        ///   generated in the last algorithm iteration. The Hessian is 
        ///   stored in the upper triangular part of this matrix. See 
        ///   remarks for details.
        ///   </summary>
        ///   
        /// <remarks>
        /// <para>
        ///   The Hessian needs only be upper-triangular, since
        ///   it is symmetric. The Cholesky decomposition will
        ///   make use of this fact and use the lower-triangular
        ///   portion to hold the decomposition, conserving memory</para>
        ///   
        /// <para>
        ///   Thus said, this property will hold the Hessian matrix
        ///   in the upper-triangular part of this matrix, and store
        ///   its Cholesky decomposition on its lower triangular part.</para>
        ///   
        /// <para>
        ///   Please note that this value is actually just an approximation to the
        ///   actual Hessian matrix using the outer Jacobian approximation (H ~ J'J).
        /// </para>
        /// </remarks>
        ///  
        public double[][] Hessian
        {
            get { return hessian; }
        }


        /// <summary>
        ///   Gets standard error for each parameter in the solution.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get { return decomposition.InverseDiagonal().Sqrt(); }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="LevenbergMarquardt"/> class.
        /// </summary>
        /// 
        /// <param name="parameters">The number of free parameters in the optimization problem.</param>
        /// 
        public LevenbergMarquardt(int parameters)
        {
            this.numberOfParameters = parameters;

            this.weights = new double[numberOfParameters];
            this.diagonal = new double[numberOfParameters];
            this.gradient = new double[numberOfParameters];
            this.solution = new double[numberOfParameters];

            this.jacobian = new double[numberOfParameters][];
            this.hessian = new double[numberOfParameters][];
            for (int i = 0; i < hessian.Length; i++)
                hessian[i] = new double[numberOfParameters];
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
            double sumOfSquaredErrors = 0.0;

            // Set upper triangular Hessian to zero
            for (int i = 0; i < hessian.Length; i++)
                Array.Clear(hessian[i], i, hessian.Length - i);

            // Set Gradient vector to zero
            Array.Clear(gradient, 0, gradient.Length);


            // Divide the problem into blocks. Instead of computing
            // a single Jacobian and a single error vector, we will
            // be computing multiple Jacobians for smaller problems
            // and then sum all blocks into the final Hessian matrix
            // and gradient vector.

            int blockSize = inputs.Length / Blocks;
            int finalBlock = inputs.Length % Blocks;
            int jacobianSize = blockSize * outputCount;

            // Re-allocate the partial Jacobian matrix only if needed
            if (jacobian[0] == null || jacobian[0].Length < jacobianSize)
            {
                for (int i = 0; i < jacobian.Length; i++)
                    this.jacobian[i] = new double[jacobianSize];
            }

            // Re-allocate error vector only if needed
            if (errors == null || errors.Length < jacobianSize)
                errors = new double[jacobianSize];



            // For each block
            for (int s = 0; s <= Blocks; s++)
            {
                if (s == Blocks && finalBlock == 0)
                    continue;

                int B = (s == Blocks) ? finalBlock : blockSize;
                int[] block = Matrix.Indices(s * blockSize, s * blockSize + B);

                // Compute the partial residuals vector
                sumOfSquaredErrors += computeErrors(inputs, outputs, block);

                // Compute the partial Jacobian
                computeJacobian(inputs, block);

                if (Double.IsNaN(sumOfSquaredErrors))
                    throw new ArithmeticException("Error calculation has produced a non-finite number.");


                // Compute error gradient using Jacobian
                for (int i = 0; i < jacobian.Length; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < jacobianSize; j++)
                        sum += jacobian[i][j] * errors[j];
                    gradient[i] += sum;
                }


                // Compute Quasi-Hessian Matrix approximation
                //  using the outer product Jacobian (H ~ J'J)
                //
                Parallel.For(0, jacobian.Length, i =>
                {
                    double[] ji = jacobian[i];
                    double[] hi = hessian[i];

                    for (int j = i; j < hi.Length; j++)
                    {
                        double[] jj = jacobian[j];

                        double sum = 0;
                        for (int k = 0; k < jj.Length; k++)
                            sum += ji[k] * jj[k];

                        // The Hessian need only be upper-triangular, since
                        // it is symmetric. The Cholesky decomposition will
                        // make use of this fact and use the lower-triangular
                        // portion to hold the decomposition, conserving memory.

                        hi[j] += 2 * sum;
                    }
                });
            }


            // Store the Hessian's diagonal for future computations. The
            // diagonal will be destroyed in the decomposition, so it can
            // still be updated on every iteration by restoring this copy.
            //
            for (int i = 0; i < hessian.Length; i++)
                diagonal[i] = hessian[i][i];

            // Create the initial weights vector
            for (int i = 0; i < solution.Length; i++)
                weights[i] = solution[i];


            // Define the objective function:
            double objective = sumOfSquaredErrors;
            double current = objective + 1.0;


            // Begin of the main Levenberg-Marquardt method
            lambda /= v;

            // We'll try to find a direction with less error
            //  (or where the objective function is smaller)
            while (current >= objective && lambda < lambdaMax)
            {
                lambda *= v;

                // Update diagonal (Levenberg-Marquardt)
                for (int i = 0; i < diagonal.Length; i++)
                    hessian[i][i] = diagonal[i] + 2 * lambda;


                // Decompose to solve the linear system. The Cholesky decomposition
                // is done in place, occupying the Hessian's lower-triangular part.
                decomposition = new JaggedCholeskyDecomposition(hessian, robust: true, inPlace: true);


                // Check if the decomposition exists
                if (decomposition.IsNotDefined)
                {
                    // The Hessian is singular. Continue to the next
                    // iteration until the diagonal update transforms
                    // it back to non-singular.
                    continue;
                }


                // Solve using Cholesky decomposition
                deltas = decomposition.Solve(gradient);


                // Update weights using the calculated deltas
                for (int i = 0; i < solution.Length; i++)
                    solution[i] = weights[i] + deltas[i];


                // Calculate the new error
                sumOfSquaredErrors = ComputeError(inputs, outputs);

                // Update the objective function
                current = sumOfSquaredErrors;

                // If the object function is bigger than before, the method
                // is tried again using a greater damping factor.
            }

            // If this iteration caused a error drop, then next iteration
            //  will use a smaller damping factor.
            lambda /= v;


            return sumOfSquaredErrors;
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
                double actual = Function(solution, input[i]);
                double expected = output[i];

                double e = expected - actual;
                sumOfSquaredErrors += e * e;
            }

            return sumOfSquaredErrors / 2.0;
        }



        private double computeErrors(double[][] input, double[] output, int[] block)
        {
            double sumOfSquaredErrors = 0.0;

            // for each input sample
            foreach (int i in block)
            {
                double actual = Function(solution, input[i]);
                double expected = output[i];

                double e = expected - actual;
                sumOfSquaredErrors += e * e;

                errors[i] = e;
            }

            return sumOfSquaredErrors / 2.0;
        }

        private void computeJacobian(double[][] input, int[] block)
        {
            double[] derivatives = new double[numberOfParameters];

            // for each input sample
            foreach (int i in block)
            {
                // TODO: transpose the Jacobian to remove copying

                Gradient(solution, input[i], derivatives);

                // copy the gradient vector into the Jacobian
                for (int j = 0; j < derivatives.Length; j++)
                    jacobian[j][i] = derivatives[j];
            }
        }

    }
}
