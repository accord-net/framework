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
    using Accord.Math.Decompositions;
    using Accord.Statistics.Models;
    using Accord.Compat;
    using System.Threading.Tasks;
    using Accord.Math.Differentiation;

    /// <summary>
    ///   Levenberg-Marquardt algorithm for solving Least-Squares problems.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   While it is possible to use the <see cref="LevenbergMarquardt"/> class as a standalone
    ///   method for solving least squares problems, this class is intended to be used as
    ///   a strategy for NonlinearLestSquares, as shown in the example below:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\NonlinearLeastSquaresTest.cs" region="doc_learn_lm" lang="cs"/>
    ///   <code source="Unit Tests\Accord.Tests.Statistics.VB\Models\Regression\NonlinearLeastSquaresTest.vb" region="doc_learn_lm" lang="vb"/>
    ///   
    /// <para>
    ///   However, as mentioned above it is also possible to use <see cref="LevenbergMarquardt"/> 
    ///   as a standalone class, as shown in the example below:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\LevenbergMarquardtTest.cs" region="doc_minimize"/>
    /// </example>
    /// 
    /// <seealso cref="GaussNewton"/>
    /// <seealso cref="FiniteDifferences"/>
    /// 
    public class LevenbergMarquardt : BaseLeastSquaresMethod, ILeastSquaresMethod, IConvergenceLearning
    {
        private const double lambdaMax = 1e25;
        private double eps = 1e-12;

        // Levenberg-Marquardt variables
        private double[][] jacobian;
        private double[][] hessian;

        private double[] diagonal;
        private double[] gradient;
        private double[] weights;
        private double[] deltas;
        private double[] errors;


        // Levenberg damping factor
        private double lambda = 0.1;

        // The amount the damping factor is adjusted
        // when searching the minimum error surface
        private double v = 10.0;


        private int blocks = 1;
        private int outputCount = 1;

        JaggedCholeskyDecomposition decomposition;



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
        ///   Gets or sets a small epsilon value to be added to the
        ///   diagonal of the Hessian matrix. Default is 1e-12.
        /// </summary>
        /// 
        public double Epsilon
        {
            get { return eps; }
            set { eps = value; }
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
        /// Initializes a new instance of the <see cref="LevenbergMarquardt" /> class.
        /// </summary>
        public LevenbergMarquardt()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LevenbergMarquardt"/> class.
        /// </summary>
        /// 
        /// <param name="parameters">The number of free parameters in the optimization problem.</param>
        /// 
        public LevenbergMarquardt(int parameters)
            : this()
        {
            this.NumberOfParameters = parameters;
        }

        /// <summary>
        /// This method should be implemented by child classes to initialize
        /// their fields once the <see cref="BaseLeastSquaresMethod.NumberOfParameters" /> is known.
        /// </summary>
        /// 
        protected override void Initialize()
        {
            this.weights = new double[NumberOfParameters];
            this.diagonal = new double[NumberOfParameters];
            this.gradient = new double[NumberOfParameters];

            this.jacobian = new double[NumberOfParameters][];
            this.hessian = Jagged.Zeros(NumberOfParameters, NumberOfParameters);
            for (int i = 0; i < hessian.Length; i++)
                hessian[i] = new double[NumberOfParameters];
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
            if (NumberOfParameters == 0)
                throw new InvalidOperationException("Please set the NumberOfVariables property first.");

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

            Convergence.CurrentIteration = 0;

            do
            {
                Convergence.NewValue = iterate(inputs, outputs, blockSize, finalBlock, jacobianSize);
            } while (!Convergence.HasConverged);


            return Value = Convergence.NewValue;
        }

        private double iterate(double[][] inputs, double[] outputs, int blockSize, int finalBlock, int jacobianSize)
        {
            double sumOfSquaredErrors = 0;

            // Set upper triangular Hessian to zero
            for (int i = 0; i < hessian.Length; i++)
                Array.Clear(hessian[i], i, hessian.Length - i);

            // Set Gradient vector to zero
            Array.Clear(gradient, 0, gradient.Length);

            // For each block
            for (int s = 0; s <= Blocks; s++)
            {
                if (s == Blocks && finalBlock == 0)
                    continue;

                int B = (s == Blocks) ? finalBlock : blockSize;
                int[] block = Vector.Range(s * blockSize, s * blockSize + B);

                // Compute the partial residuals vector
                sumOfSquaredErrors += computeErrors(inputs, outputs, block);

                // Compute the partial Jacobian
                computeJacobian(inputs, block);

                if (Double.IsNaN(sumOfSquaredErrors))
                {
                    throw new ArithmeticException("Error calculation has produced a non-finite number."
                        + " Please make sure that there are no constant columns in the input data.");
                }


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
                Parallel.For(0, jacobian.Length, ParallelOptions, i =>
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
                diagonal[i] = hessian[i][i] + eps;

            // Create the initial weights vector
            for (int i = 0; i < Solution.Length; i++)
                weights[i] = Solution[i];


            // Define the objective function:
            double objective = sumOfSquaredErrors;
            double current = objective + 1.0;


            // Begin of the main Levenberg-Marquardt method
            this.lambda /= this.v;

            // We'll try to find a direction with less error
            //  (or where the objective function is smaller)
            while (current >= objective && lambda < lambdaMax)
            {
                if (Token.IsCancellationRequested)
                    break;

                this.lambda *= this.v;

                // Update diagonal (Levenberg-Marquardt)
                for (int i = 0; i < diagonal.Length; i++)
                    hessian[i][i] = diagonal[i] * (1 + lambda);


                // Decompose to solve the linear system. The Cholesky decomposition
                // is done in place, occupying the Hessian's lower-triangular part.
                this.decomposition = new JaggedCholeskyDecomposition(hessian, robust: true, inPlace: true);


                // Check if the decomposition exists
                if (this.decomposition.IsUndefined)
                {
                    // The Hessian is singular. Continue to the next
                    // iteration until the diagonal update transforms
                    // it back to non-singular.
                    continue;
                }


                // Solve using Cholesky decomposition
                this.deltas = this.decomposition.Solve(gradient);


                // Update weights using the calculated deltas
                for (int i = 0; i < Solution.Length; i++)
                    this.Solution[i] = this.weights[i] + this.deltas[i];


                // Calculate the new error
                sumOfSquaredErrors = ComputeError(inputs, outputs);

                // Update the objective function
                current = sumOfSquaredErrors;

                // If the object function is bigger than before, the method
                // is tried again using a greater damping factor.
            }

            // If this iteration caused a error drop, then next iteration
            //  will use a smaller damping factor.
            this.lambda /= this.v;

            return sumOfSquaredErrors;
        }



        private double computeErrors(double[][] input, double[] output, int[] block)
        {
            double sumOfSquaredErrors = 0.0;

            // for each input sample
            foreach (int i in block)
            {
                double actual = Function(Solution, input[i]);
                double expected = output[i];

                double e = expected - actual;
                sumOfSquaredErrors += e * e;

                errors[i] = e;
            }

            return sumOfSquaredErrors / 2.0;
        }

        private void computeJacobian(double[][] input, int[] block)
        {
            double[] derivatives = new double[NumberOfParameters];

            // for each input sample
            foreach (int i in block)
            {
                Gradient(Solution, input[i], derivatives);

                // copy the gradient vector into the Jacobian
                for (int j = 0; j < derivatives.Length; j++)
                    jacobian[j][i] = derivatives[j];
            }
        }

    }
}
