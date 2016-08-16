// Accord Statistics Library
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

namespace Accord.Statistics.Models.Regression.Fitting
{
    using System;
    using Accord.Math.Optimization;
using Accord.MachineLearning;
    using System.Threading;

    /// <summary>
    ///   Non-linear Least Squares for <see cref="NonlinearRegression"/> optimization.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Suppose we would like to map the continuous values in the
    /// // second column to the integer values in the first column.
    /// double[,] data =
    /// {
    ///     { -40,    -21142.1111111111 },
    ///     { -30,    -21330.1111111111 },
    ///     { -20,    -12036.1111111111 },
    ///     { -10,      7255.3888888889 },
    ///     {   0,     32474.8888888889 },
    ///     {  10,     32474.8888888889 },
    ///     {  20,      9060.8888888889 },
    ///     {  30,    -11628.1111111111 },
    ///     {  40,    -15129.6111111111 },
    /// };
    /// 
    /// // Extract inputs and outputs
    /// double[][] inputs = data.GetColumn(0).ToArray();
    /// double[] outputs = data.GetColumn(1);
    /// 
    /// // Create a Nonlinear regression using 
    /// var regression = new NonlinearRegression(3,
    /// 
    ///     // Let's assume a quadratic model function: ax² + bx + c
    ///     function: (w, x) => w[0] * x[0] * x[0] + w[1] * x[0] + w[2], 
    /// 
    ///     // Derivative in respect to the weights:
    ///     gradient: (w, x, r) =>
    ///     {
    ///         r[0] = 2 * w[0]; // w.r.t a: 2a  
    ///         r[1] = w[1];     // w.r.t b: b
    ///         r[2] = w[2];     // w.r.t c: 0
    ///     }
    /// );
    /// 
    /// // Create a non-linear least squares teacher
    /// var nls = new NonlinearLeastSquares(regression);
    /// 
    /// // Initialize to some random values
    /// regression.Coefficients[0] = 4.2;
    /// regression.Coefficients[1] = 0.3;
    /// regression.Coefficients[2] = 1;
    /// 
    /// // Run the function estimation algorithm
    /// double error;
    /// for (int i = 0; i &lt; 100; i++)
    ///     error = nls.Run(inputs, outputs);
    /// 
    /// // Use the function to compute the input values
    /// double[] predict = inputs.Apply(regression.Compute);
    /// </code>
    /// </example>
#pragma warning disable 612, 618
    public class NonlinearLeastSquares : 
        ISupervisedLearning<NonlinearRegression, double[], double>,
        IRegressionFitting
#pragma warning restore 612, 618
    {
        private ILeastSquaresMethod solver;
        private NonlinearRegression regression;
        private bool computeStandardErrors = true;


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
        ///   Gets the <see cref="ILeastSquaresMethod">Least-Squares</see>
        ///   optimization algorithm used to perform the actual learning.
        /// </summary>
        /// 
        public ILeastSquaresMethod Algorithm
        {
            get { return solver; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NonlinearLeastSquares"/> class.
        /// </summary>
        /// 
        /// <param name="regression">The regression model.</param>
        /// 
        public NonlinearLeastSquares(NonlinearRegression regression)
            : this(regression, new LevenbergMarquardt(regression.Coefficients.Length))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NonlinearLeastSquares"/> class.
        /// </summary>
        /// 
        /// <param name="regression">The regression model.</param>
        /// <param name="algorithm">The <see cref="ILeastSquaresMethod">least squares</see>
        /// algorithm to be used to estimate the regression parameters. Default is to
        /// use a <see cref="LevenbergMarquardt">Levenberg-Marquardt</see> algorithm.</param>
        /// 
        public NonlinearLeastSquares(NonlinearRegression regression, ILeastSquaresMethod algorithm)
        {
            if (regression == null)
                throw new ArgumentNullException("regression");

            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            if (regression.Gradient == null)
                throw new ArgumentException("The regression must have a gradient function defined.", "regression");

            this.solver = algorithm;
            this.solver.Solution = regression.Coefficients;
            this.solver.Function = new LeastSquaresFunction(regression.Function);
            this.solver.Gradient = new LeastSquaresGradientFunction(regression.Gradient);
            this.regression = regression;
        }



        /// <summary>
        ///   Runs the fitting algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input training data.</param>
        /// <param name="outputs">The output associated with each of the outputs.</param>
        /// 
        /// <returns>
        ///   The sum of squared errors after the learning.
        /// </returns>
        /// 
        [Obsolete("Please use the Learn() method instead.")]
        public double Run(double[][] inputs, double[] outputs)
        {
            Learn(inputs, outputs);
            return solver.Value;
        }


        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token { get; set; }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public NonlinearRegression Learn(double[][] x, double[] y, double[] weights = null)
        {
            double error = solver.Minimize(x, y);

            if (computeStandardErrors)
            {
                double[] errors = solver.StandardErrors;
                for (int i = 0; i < errors.Length; i++)
                    regression.StandardErrors[i] = solver.StandardErrors[i];
            }

#if DEBUG
            if (Double.IsNaN(error) || Double.IsInfinity(error))
                throw new Exception();
#endif

            return regression;
        }
    }
}
