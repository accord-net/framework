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
    using Math;
    using Accord.Math.Decompositions;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Polynomial Least-Squares.
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear regression, the model specification is that the dependent
    ///   variable, y is a linear combination of the parameters (but need not
    ///   be linear in the independent variables). As the linear regression
    ///   has a closed form solution, the regression coefficients can be
    ///   efficiently computed using the Regress method of this class.
    /// </remarks>
    /// 
    /// <seealso cref="PolynomialRegression"/>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\PolynomialRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
    public class PolynomialLeastSquares :
        ISupervisedLearning<PolynomialRegression, double, double>
    {
        int degree = 1;

        [NonSerialized]
        CancellationToken token = new CancellationToken();

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
        ///   Gets or sets whether to always use a robust Least-Squares 
        ///   estimate using the <see cref="SingularValueDecomposition"/>.
        ///   Default is false.
        /// </summary>
        /// 
        public bool IsRobust { get; set; }

        /// <summary>
        ///   Gets or sets the polynomial degree to use
        ///   in the polynomial regression.
        /// </summary>
        /// 
        public int Degree
        {
            get { return degree; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "Degree should be higher than zero.");
                degree = value;
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
        public PolynomialRegression Learn(double[] x, double[] y, double[] weights = null)
        {
            double[][] z = Jagged.Zeros(x.Length, Degree);
            for (int i = 0; i < x.Length; i++)
                for (int j = 0; j < z[i].Length; j++)
                    z[i][j] = Math.Pow(x[i], Degree - j);

            var lls = new OrdinaryLeastSquares()
            {
                UseIntercept = true,
                IsRobust = IsRobust
            };

            var linear = lls.Learn(z, y, weights);

            return new PolynomialRegression(linear);
        }
    }
}
