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
    using Accord.Math.Optimization;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Non-linear Least Squares for <see cref="NonlinearRegression"/> optimization.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   The first example shows how to fit a non-linear least squares problem with <see cref="LevenbergMarquardt"/>.</para>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\NonlinearLeastSquaresTest.cs" region="doc_learn_lm" lang="cs"/>
    /// <code source="Unit Tests\Accord.Tests.Statistics.VB\Models\Regression\NonlinearLeastSquaresTest.vb" region="doc_learn_lm" lang="vb"/>
    /// 
    /// <para>
    ///   The second example shows how to fit a non-linear least squares problem with <see cref="GaussNewton"/>.</para>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\NonlinearLeastSquaresTest.cs" region="doc_learn_gn" lang="cs"/>
    /// <code source="Unit Tests\Accord.Tests.Statistics.VB\Models\Regression\NonlinearLeastSquaresTest.vb" region="doc_learn_gn" lang="vb"/>
    /// </example>
    /// 
#pragma warning disable 612, 618
    public class NonlinearLeastSquares :
        ISupervisedLearning<NonlinearRegression, double[], double>,
        IRegressionFitting
#pragma warning restore 612, 618
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private ILeastSquaresMethod solver;
        private NonlinearRegression regression;
        private bool computeStandardErrors = true;
        private int numberOfParameters;

        RegressionFunction function;
        RegressionGradientFunction gradient;


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
            set { solver = value; }
        }

        /// <summary>
        ///   Gets the number of variables (free parameters) in the non-linear model specified in <see cref="Function"/>.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters of <see cref="Function"/>.
        /// </value>
        /// 
        public int NumberOfParameters
        {
            get { return numberOfParameters; }
            set { numberOfParameters = value; }
        }

        /// <summary>
        ///   Gets or sets the model function, mapping inputs to 
        ///   outputs given a suitable parameter vector.
        /// </summary>
        /// 
        public RegressionFunction Function
        {
            get { return function; }
            set { function = value; }
        }

        /// <summary>
        ///   Gets or sets a function that computes the gradient of the
        ///   <see cref="Function"/> in respect to the current parameters.
        /// </summary>
        /// 
        public RegressionGradientFunction Gradient
        {
            get { return gradient; }
            set { gradient = value; }
        }

        /// <summary>
        /// Gets or sets the vector of initial values to be used at the beginning
        /// of the optimization. Setting a suitable set of initial values can be
        /// important to achieve good convergence or avoid poor local minimas.
        /// </summary>
        /// 
        public double[] StartValues
        {
            get; set;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="NonlinearLeastSquares" /> class.
        /// </summary>
        /// 
        public NonlinearLeastSquares()
        {

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

            this.regression = regression;
            this.NumberOfParameters = regression.Coefficients.Length;

            this.solver = algorithm;
            this.solver.Solution = regression.Coefficients;
            this.solver.Function = new LeastSquaresFunction(regression.Function);
            this.solver.Gradient = new LeastSquaresGradientFunction(regression.Gradient);
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
            var c = this.solver as IConvergenceLearning;

            if (c != null)
            {
                c.MaxIterations = 1;
                c.Tolerance = 0;
            } 

            Learn(inputs, outputs);
            return solver.Value;
        }


        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
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
        public NonlinearRegression Learn(double[][] x, double[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (NumberOfParameters == 0)
            {
                if (regression == null)
                {
                    if (StartValues == null)
                        throw new InvalidOperationException("Please set the number of parameters, starting values, or the initial regression model.");
                    NumberOfParameters = StartValues.Length;
                }
            }

            if (regression == null)
            {
                this.regression = new NonlinearRegression(numberOfParameters, function, gradient);
                if (StartValues != null)
                    this.regression.Coefficients.SetTo(StartValues);
            }

            if (this.solver == null)
                this.solver = new LevenbergMarquardt(numberOfParameters);

            this.solver.NumberOfVariables = numberOfParameters;
            this.solver.Solution = regression.Coefficients;
            this.solver.Function = new LeastSquaresFunction(regression.Function);
            this.solver.Gradient = new LeastSquaresGradientFunction(regression.Gradient);
            this.solver.Token = Token;

            double error = solver.Minimize(x, y);

            if (Double.IsNaN(error) || Double.IsInfinity(error))
                throw new Exception();

            if (computeStandardErrors)
            {
                double[] errors = solver.StandardErrors;
                for (int i = 0; i < errors.Length; i++)
                    regression.StandardErrors[i] = solver.StandardErrors[i];
            }


            return regression;
        }
    }
}
