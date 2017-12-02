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

namespace Accord.Statistics.Models.Regression.Linear
{
    using MachineLearning;
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Least Squares learning algorithm for linear regression models.
    /// </summary>
    /// 
    /// <example>
    ///  <para>
    ///   Let's say we have some univariate, continuous sets of input data,
    ///   and a corresponding univariate, continuous set of output data, such
    ///   as a set of points in R². A simple linear regression is able to fit
    ///   a line relating the input variables to the output variables in which
    ///   the minimum-squared-error of the line and the actual output points
    ///   is minimum.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\SimpleLinearRegressionTest.cs" region="doc_learn" />
    /// 
    ///  <para>
    ///   The following example shows how to fit a multiple linear regression model
    ///   to model a plane as an equation in the form ax + by + c = z. </para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\MultipleLinearRegressionTest.cs" region="doc_learn" />
    /// 
    ///  <para>
    ///   The following example shows how to fit a multivariate linear regression model,
    ///   producing multidimensional outputs for each input.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\MultivariateLinearRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="SimpleLinearRegression"/>
    /// <seealso cref="MultipleLinearRegression"/>
    /// <seealso cref="MultivariateLinearRegression"/>
    /// 
    public class OrdinaryLeastSquares :
        ISupervisedLearning<MultivariateLinearRegression, double[], double[]>,
        ISupervisedLearning<MultipleLinearRegression, double[], double>,
        ISupervisedLearning<SimpleLinearRegression, double, double>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private ISolverArrayDecomposition<double> decomposition;

        /// <summary>
        ///   Gets or sets whether to include an intercept 
        ///   term in the learned models. Default is true.
        /// </summary>
        /// 
        public bool UseIntercept { get; set; }

        /// <summary>
        ///   Gets or sets whether to always use a robust Least-Squares 
        ///   estimate using the <see cref="SingularValueDecomposition"/>.
        ///   Default is false.
        /// </summary>
        /// 
        public bool IsRobust { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="OrdinaryLeastSquares"/> class.
        /// </summary>
        /// 
        public OrdinaryLeastSquares()
        {
            UseIntercept = true;
        }

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// 
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
        public SimpleLinearRegression Learn(double[] x, double[] y, double[] weights = null)
        {
            double[][] X = Jagged.ColumnVector(x);

            if (UseIntercept)
                X = X.InsertColumn(value: 1);

            if (weights != null)
            {
                double[] sqrtW = weights.Sqrt();
                X = Elementwise.Multiply(X, sqrtW, dimension: (VectorType)1, result: X);
                y = Elementwise.Multiply(y, sqrtW);
            }

            decomposition = X.Decompose(leastSquares: IsRobust);
            double[] coefficients = decomposition.Solve(y);

            if (UseIntercept)
            {
                return new SimpleLinearRegression()
                {
                    Slope = coefficients[0],
                    Intercept = coefficients[1]
                };
            }
            else
            {
                return new SimpleLinearRegression()
                {
                    Slope = coefficients[0],
                };
            }
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
        public MultipleLinearRegression Learn(double[][] x, double[] y, double[] weights = null)
        {
            //var Xt = x.Transpose();
            if (UseIntercept)
                x = x.InsertColumn(value: 1.0);

            if (weights != null)
            {
                double[] sqrtW = weights.Sqrt();
                x = Elementwise.Multiply(x, sqrtW, dimension: (VectorType)1);
                y = Elementwise.Multiply(y, sqrtW);
            }

            decomposition = x.Decompose(leastSquares: IsRobust);
            double[] coefficients = decomposition.Solve(y);

            if (UseIntercept)
            {
                return new MultipleLinearRegression()
                {
                    Weights = coefficients.Get(0, -1),
                    Intercept = coefficients.Get(-1)
                };
            }
            else
            {
                return new MultipleLinearRegression()
                {
                    Weights = coefficients,
                };
            }
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
        public MultivariateLinearRegression Learn(double[][] x, double[][] y, double[] weights = null)
        {
            if (UseIntercept)
                x = x.InsertColumn(value: 1.0);

            if (weights != null)
            {
                double[] sqrtW = weights.Sqrt();
                x = Elementwise.Multiply(x, sqrtW, dimension: (VectorType)1);
                y = Elementwise.Multiply(y, sqrtW, dimension: (VectorType)1);
            }

            decomposition = x.Decompose(leastSquares: IsRobust);
            double[][] coefficients = decomposition.Solve(y);

            if (UseIntercept)
            {
                return new MultivariateLinearRegression()
                {
                    Weights = coefficients.Get(0, -1, null),
                    Intercepts = coefficients.GetRow(-1)
                };
            }

            return new MultivariateLinearRegression()
            {
                Weights = coefficients,
            };
        }

        /// <summary>
        ///   Gets the information matrix used to update the regression
        ///   weights in the last call to <see cref="Learn(double[], double[], double[])"/>
        /// </summary>
        /// 
        public double[][] GetInformationMatrix()
        {
            return decomposition.GetInformationMatrix();
        }
    }
}
