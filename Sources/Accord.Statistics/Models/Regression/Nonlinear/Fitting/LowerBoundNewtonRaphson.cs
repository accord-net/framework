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
    ///   Lower-Bound Newton-Raphson for Multinomial logistic regression fitting.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Lower Bound principle consists of replacing the second derivative
    ///   matrix by a global lower bound in the Leowner ordering [Böhning, 92].
    ///   In the case of multinomial logistic regression estimation, the Hessian
    ///   of the negative log-likelihood function can be replaced by one of those
    ///   lower bounds, leading to a monotonically converging sequence of iterates.
    ///   Furthermore, [Krishnapuram, Carin, Figueiredo and Hartemink, 2005] also
    ///   have shown that a lower bound can be achieved which does not depend on 
    ///   the coefficients for the current iteration.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      B. Krishnapuram, L. Carin, M.A.T. Figueiredo, A. Hartemink. Sparse Multinomial 
    ///      Logistic Regression: Fast Algorithms and Generalization Bounds. 2005. Available on: 
    ///      http://www.lx.it.pt/~mtf/Krishnapuram_Carin_Figueiredo_Hartemink_2005.pdf </description></item>
    ///     <item><description>
    ///       D. Böhning. Multinomial logistic regression algorithm. Annals of the Institute
    ///       of Statistical Mathematics, 44(9):197 ˝U200, 1992. 2. M. Corney.</description></item>
    ///     <item><description>
    ///       Bishop, Christopher M.; Pattern Recognition and Machine Learning. 
    ///       Springer; 1st ed. 2006.</description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <example>
    ///   <code>
    ///   // Create a new Multinomial Logistic Regression for 3 categories
    ///   var mlr = new MultinomialLogisticRegression(inputs: 2, categories: 3);
    ///   
    ///   // Create a estimation algorithm to estimate the regression
    ///   LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson(mlr);
    ///   
    ///   // Now, we will iteratively estimate our model. The Run method returns
    ///   // the maximum relative change in the model parameters and we will use
    ///   // it as the convergence criteria.
    ///   
    ///   double delta;
    ///   int iteration = 0;
    ///   
    ///   do
    ///   {
    ///       // Perform an iteration
    ///       delta = lbnr.Run(inputs, outputs);
    ///       iteration++;
    ///   
    ///   } while (iteration &lt; 100 &amp;&amp; delta > 1e-6);
    ///   </code>
    /// </example>
    ///
    public class LowerBoundNewtonRaphson : IMultipleRegressionFitting
    {

        private MultinomialLogisticRegression regression;

        private double[] previous;
        private double[] solution;
        private double[] deltas;

        private double[] errors;
        private double[] output;

        private double[,] weights;

        private double[,] lowerBound;
        private double[] gradient;
        private double[,] xxt;

        private int K;
        private int M;
        private int parameterCount;

        private bool computeStandardErrors = true;
        private bool updateLowerBound = true;
        private ISolverMatrixDecomposition<double> decomposition = null;


        /// <summary>
        ///   Gets the previous values for the coefficients which were
        ///   in place before the last learning iteration was performed.
        /// </summary>
        /// 
        public double[] Previous
        {
            get { return previous; }
        }

        /// <summary>
        ///   Gets the current values for the coefficients.
        /// </summary>
        /// 
        public double[] Solution
        {
            get { return solution; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the
        ///   lower bound should be updated using new data.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if the lower bound should be
        ///   updated; otherwise, <c>false</c>.</value>
        ///   
        public bool UpdateLowerBound
        {
            get { return updateLowerBound; }
            set { updateLowerBound = value; }
        }

        /// <summary>
        ///   Gets the Lower-Bound matrix being used in place of
        ///   the Hessian matrix in the Newton-Raphson iterations.
        /// </summary>
        /// 
        public double[,] HessianLowerBound
        {
            get { return lowerBound; }
        }

        /// <summary>
        ///   Gets the Gradient vector computed in
        ///   the last Newton-Raphson iteration.
        /// </summary>
        /// 
        public double[] Gradient
        {
            get { return gradient; }
        }

        /// <summary>
        ///   Gets the total number of parameters in the model.
        /// </summary>
        /// 
        public int Parameters { get { return parameterCount; } }


        /// <summary>
        ///   Gets or sets a value indicating whether standard
        ///   errors should be computed in the next iteration.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to compute standard errors; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool ComputeStandardErrors
        {
            get { return computeStandardErrors; }
            set { computeStandardErrors = value; }
        }




        /// <summary>
        ///   Creates a new <see cref="LowerBoundNewtonRaphson"/>.
        /// </summary>
        /// <param name="regression">The regression to estimate.</param>
        /// 
        public LowerBoundNewtonRaphson(MultinomialLogisticRegression regression)
        {
            this.regression = regression;

            K = regression.Categories - 1;
            M = regression.Inputs + 1;
            parameterCount = K * M;

            solution = regression.Coefficients.Reshape(1);

            xxt = new double[M, M];
            errors = new double[K];
            output = new double[K];

            lowerBound = new double[parameterCount, parameterCount];
            gradient = new double[parameterCount];

            // Differently from the IRLS iteration, the weight matrix can be fixed
            // as it does not depend on the current coefficients anymore [I - 11/m]

            // TODO: Avoid the multiple allocations in the line below
            weights = (-0.5).Multiply(Matrix.Identity(K).Subtract(Matrix.Create(K, K, 1.0 / M)));
        }


        /// <summary>
        ///   Runs one iteration of the Lower-Bound Newton-Raphson iteration.
        /// </summary>
        /// <param name="inputs">The input data.</param>
        /// <param name="classes">The outputs associated with each input vector.</param>
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        public double Run(double[][] inputs, int[] classes)
        {
            return run(inputs, Statistics.Tools.Expand(classes));
        }

        /// <summary>
        ///   Runs one iteration of the Lower-Bound Newton-Raphson iteration.
        /// </summary>
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The outputs associated with each input vector.</param>
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        public double Run(double[][] inputs, double[][] outputs)
        {
            return run(inputs, outputs);
        }


        private double run(double[][] inputs, double[][] outputs)
        {
            // Regress using Lower-Bound Newton-Raphson estimation
            //
            // The main idea is to replace the Hessian matrix with a 
            //   suitable lower bound. Indeed, the Hessian is lower
            //   bounded by a negative definite matrix that does not
            //   even depend on w [Krishnapuram et al].
            //
            //   - http://www.lx.it.pt/~mtf/Krishnapuram_Carin_Figueiredo_Hartemink_2005.pdf
            //


            // Initial definitions and memory allocations
            int N = inputs.Length;

            double[][] design = new double[N][];
            double[][] coefficients = this.regression.Coefficients;

            // Compute the regression matrix
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] row = design[i] = new double[M];

                row[0] = 1; // for intercept
                for (int j = 0; j < inputs[i].Length; j++)
                    row[j + 1] = inputs[i][j];
            }


            // Reset Hessian matrix and gradient
            for (int i = 0; i < gradient.Length; i++)
                gradient[i] = 0;

            if (UpdateLowerBound)
            {
                for (int i = 0; i < gradient.Length; i++)
                    for (int j = 0; j < gradient.Length; j++)
                        lowerBound[i, j] = 0;
            }

            // In the multinomial logistic regression, the objective
            // function is the log-likelihood function l(w). As given
            // by Krishnapuram et al and Böhning, this is a concave 
            // function with Hessian given by:
            //
            //       H(w) = -sum(P(w) - p(w)p(w)')  (x)  xx'
            //      (see referenced paper for proper indices)
            //       
            // In which (x) denotes the Kronecker product. By using
            // the lower bound principle, Krishnapuram has shown that
            // we can replace H(w) with a lower bound approximation B
            // which does not depend on w (eq. 8 on aforementioned paper):
            // 
            //      B = -(1/2) [I - 11/M]  (x)  sum(xx')
            //
            // Thus we can compute and invert this matrix only once.
            //


            // For each input sample in the dataset
            for (int i = 0; i < inputs.Length; i++)
            {
                // Grab variables related to the sample
                double[] x = design[i];
                double[] y = outputs[i];

                // Compute and estimate outputs
                this.compute(inputs[i], output);

                // Compute errors for the sample
                for (int j = 0; j < errors.Length; j++)
                    errors[j] = y[j + 1] - output[j];


                // Compute current gradient and Hessian
                //   We can take advantage of the block structure of the 
                //   Hessian matrix and gradient vector by employing the
                //   Kronecker product. See [Böhning, 1992]

                // (Re-) Compute error gradient
                double[] g = Matrix.KroneckerProduct(errors, x);
                for (int j = 0; j < g.Length; j++)
                    gradient[j] += g[j];

                if (UpdateLowerBound)
                {
                    // Compute xxt matrix
                    for (int k = 0; k < x.Length; k++)
                        for (int j = 0; j < x.Length; j++)
                            xxt[k, j] = x[k] * x[j];

                    // (Re-) Compute weighted "Hessian" matrix 
                    double[,] h = Matrix.KroneckerProduct(weights, xxt);
                    for (int j = 0; j < parameterCount; j++)
                        for (int k = 0; k < parameterCount; k++)
                            lowerBound[j, k] += h[j, k];
                }
            }


            if (UpdateLowerBound)
            {
                UpdateLowerBound = false;

                // Decompose to solve the linear system. Usually the Hessian will
                // be invertible and LU will succeed. However, sometimes the Hessian
                // may be singular and a Singular Value Decomposition may be needed.

                // The SVD is very stable, but is quite expensive, being on average
                // about 10-15 times more expensive than LU decomposition. There are
                // other ways to avoid a singular Hessian. For a very interesting 
                // reading on the subject, please see:
                //
                //  - Jeff Gill & Gary King, "What to Do When Your Hessian Is Not Invertible",
                //    Sociological Methods & Research, Vol 33, No. 1, August 2004, 54-87.
                //    Available in: http://gking.harvard.edu/files/help.pdf
                //

                // Moreover, the computation of the inverse is optional, as it will
                // be used only to compute the standard errors of the regression.


                // Hessian Matrix is singular, try pseudo-inverse solution
                decomposition = new SingularValueDecomposition(lowerBound);
                deltas = decomposition.Solve(gradient);
            }
            else
            {
                deltas = decomposition.Solve(gradient);
            }


            previous = coefficients.Reshape(1);

            // Update coefficients using the calculated deltas
            for (int i = 0, k = 0; i < coefficients.Length; i++)
                for (int j = 0; j < coefficients[i].Length; j++)
                    coefficients[i][j] -= deltas[k++];

            solution = coefficients.Reshape(1);


            if (computeStandardErrors)
            {
                // Grab the regression information matrix
                double[,] inverse = decomposition.Inverse();

                // Calculate coefficients' standard errors
                double[][] standardErrors = regression.StandardErrors;
                for (int i = 0, k = 0; i < standardErrors.Length; i++)
                    for (int j = 0; j < standardErrors[i].Length; j++, k++)
                        standardErrors[i][j] = Math.Sqrt(Math.Abs(inverse[k, k]));
            }



            // Return the relative maximum parameter change
            for (int i = 0; i < deltas.Length; i++)
                deltas[i] = Math.Abs(deltas[i]) / Math.Abs(previous[i]);

            return Matrix.Max(deltas);
        }


        private void compute(double[] x, double[] responses)
        {
            double[][] coefficients = this.regression.Coefficients;

            double sum = 1;

            // For each category (except the first)
            for (int j = 0; j < coefficients.Length; j++)
            {
                // Get category coefficients
                double[] c = coefficients[j];

                double logit = c[0]; // intercept

                for (int i = 0; i < x.Length; i++)
                    logit += c[i + 1] * x[i];

                sum += responses[j] = Math.Exp(logit);
            }

            // Normalize the probabilities
            for (int j = 0; j < responses.Length; j++)
                responses[j] /= sum;
        }


    }
}
