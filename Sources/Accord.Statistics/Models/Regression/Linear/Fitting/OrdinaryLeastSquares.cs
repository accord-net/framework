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

namespace Accord.Statistics.Models.Regression.Linear
{
    using MachineLearning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Least Squares learning algorithm for linear regression models.
    /// </summary>
    /// 
    public class OrdinaryLeastSquares :
        ISupervisedLearning<MultivariateLinearRegression, double[], double[]>,
        ISupervisedLearning<MultipleLinearRegression, double[], double>,
        ISupervisedLearning<SimpleLinearRegression, double, double>
    {
        private ISolverArrayDecomposition<double> decomposition;

        /// <summary>
        ///   Gets or sets wether to include an intercept 
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
        public SimpleLinearRegression Learn(double[] x, double[] y, double[] weights = null)
        {
            double[][] X = Jagged.ColumnVector(x);

            if (UseIntercept)
                X = X.InsertColumn(value: 1);

            decomposition = X.Decompose(leastSquares: IsRobust);
            double[] coefficients = decomposition.Solve(y);

            if (UseIntercept)
            {
                return new SimpleLinearRegression()
                {
                    Slope = coefficients[1],
                    Intercept = coefficients[0]
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
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public MultipleLinearRegression Learn(double[][] x, double[] y, double[] weights = null)
        {
            //var Xt = x.Transpose();
            if (UseIntercept)
                x = x.InsertColumn(value: 1.0);

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
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public MultivariateLinearRegression Learn(double[][] x, double[][] y, double[] weights = null)
        {
            if (UseIntercept)
                x = x.InsertColumn(value: 1.0);

            decomposition = x.Decompose(leastSquares: IsRobust);
            double[][] coefficients = decomposition.Solve(y);

            if (UseIntercept)
            {
                return new MultivariateLinearRegression()
                {
                    Weights = coefficients.Get(null, 0, -1),
                    Intercepts = coefficients.GetColumn(-1)
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
