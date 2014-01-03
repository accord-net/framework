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
    using Accord.Math.Optimization;

    /// <summary>
    ///   Non-linear Least Squares for <see cref="NonlinearRegression"/> optimization.
    /// </summary>
    /// 
    public class NonlinearLeastSquares : IRegressionFitting
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
        public double Run(double[][] inputs, double[] outputs)
        {
            double error = solver.Minimize(inputs, outputs);

            if (computeStandardErrors)
            {
                double[] errors = solver.StandardErrors;
                for (int i = 0; i < errors.Length; i++)
                    regression.StandardErrors[i] = solver.StandardErrors[i];
            }

            return error;
        }

    }
}
