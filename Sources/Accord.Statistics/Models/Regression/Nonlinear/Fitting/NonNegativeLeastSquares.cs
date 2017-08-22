// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Clement Schiano di Colella, 2015-2016
// clement.schiano@gmail.com
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
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.MachineLearning;
    using Accord.Math.Optimization.Losses;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Non-negative Least Squares for <see cref="MultipleLinearRegression"/> optimization.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      Donghui Chen and Robert J.Plemmons, Nonnegativity Constraints in Numerical Analysis.
    ///      Available on: http://users.wfu.edu/plemmons/papers/nonneg.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///  <para>
    ///   The following example shows how to fit a multiple linear regression model with the 
    ///   additional constraint that none of its coefficients should be negative. For this
    ///   we can use the <see cref="NonNegativeLeastSquares"/> learning algorithm instead of
    ///   the <see cref="OrdinaryLeastSquares"/> used above.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\NonNegativeLeastSquaresTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="OrdinaryLeastSquares"/>
    /// <seealso cref="MultipleLinearRegression"/>
    /// 
#pragma warning disable 612, 618
    public class NonNegativeLeastSquares : IRegressionFitting,
        ISupervisedLearning<MultipleLinearRegression, double[], double>
#pragma warning restore 612, 618
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        MultipleLinearRegression regression;

        List<int> p = new List<int>();
        List<int> r = new List<int>();
        double[] s;
        double tolerance = 0.001;
        double[][] scatter;
        double[] vector;
        double[] W;
        double[][] X;
        int cols;
        int maxIter;

        /// <summary>
        ///   Gets the coefficient vector being fitted.
        /// </summary>
        /// 
        public double[] Coefficients { get { return regression.Weights; } }

        /// <summary>
        ///   Gets or sets the maximum number of iterations to be performed.
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return maxIter; }
            set { maxIter = value; }
        }

        /// <summary>
        ///   Gets or sets the tolerance for detecting
        ///   convergence. Default is 0.001.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
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
        ///   Initializes a new instance of the <see cref="NonNegativeLeastSquares"/> class.
        /// </summary>
        /// 
        public NonNegativeLeastSquares()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NonNegativeLeastSquares"/> class.
        /// </summary>
        /// 
        /// <param name="regression">The regression to be fitted.</param>
        /// 
        public NonNegativeLeastSquares(MultipleLinearRegression regression)
        {
            init(regression);
        }

        private void init(MultipleLinearRegression regression)
        {
            this.regression = regression;
            this.cols = regression.Weights.Length;
            this.s = new double[cols];
            this.W = new double[cols];
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
            this.regression = Learn(inputs, outputs);
            return new SquareLoss(inputs).Loss(regression.Transform(inputs));
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
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (this.regression == null)
                init(new MultipleLinearRegression { NumberOfInputs = x.Columns() });

            this.X = x;
            this.scatter = X.TransposeAndDot(X);
            this.vector = X.TransposeAndDot(y);

            // Initialization
            p.Clear();
            r.Clear();
            for (var i = 0; i < cols; i++)
                r.Add(i);

            var w = Coefficients;

            ComputeWeights(w);
            var iter = 0;
            int maxWeightIndex;
            W.Max(out maxWeightIndex);

            while (r.Count > 0 && W[maxWeightIndex] > tolerance && iter < maxIter)
            {
                if (Token.IsCancellationRequested)
                    break;

                // Include the index j in P and remove it from R
                if (!p.Contains(maxWeightIndex))
                    p.Add(maxWeightIndex);

                if (r.Contains(maxWeightIndex))
                    r.Remove(maxWeightIndex);

                GetSP();
                int iter2 = 0;

                while (GetElements(s, p).Min() <= 0 && iter2 < maxIter)
                {
                    InnerLoop(w);
                    iter2++;
                }

                // 5
                Array.Copy(s, w, s.Length);

                // 6
                ComputeWeights(w);

                W.Max(out maxWeightIndex);
                iter++;
            }

            //Coefficients = x;
            return regression;
        }

        private void InnerLoop(double[] x)
        {
            var alpha = double.PositiveInfinity;
            foreach (int i in p)
            {
                if (s[i] <= 0)
                    alpha = System.Math.Min(alpha, x[i] / (x[i] - s[i]));
            }

            if (System.Math.Abs(alpha) < 0.001 || double.IsNaN(alpha))
                return;

            x = (s.Subtract(x)).Multiply(alpha).Add(x);

            // 4.4 Update R and P
            for (var i = 0; i < p.Count;)
            {
                int pItem = p[i];
                if (System.Math.Abs(x[pItem]) < double.Epsilon)
                {
                    r.Add(pItem);
                    p.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // 4.5 
            GetSP();

            // 4.6
            foreach (var i in r)
                s[i] = 0;
        }

        private void ComputeWeights(double[] x)
        {
            W = vector.Subtract(scatter.Dot(x));
        }

        private void GetSP()
        {
            int[] array = p.ToArray();
            double[][] left = scatter
                .GetColumns(array)
                .GetRows(array)
                .PseudoInverse();

            double[] columnVector = GetElements(vector, p);
            double[] result = left.Dot(columnVector);
            for (int i = 0; i < p.Count; i++)
                s[p[i]] = result[i];
        }

        private static double[] GetElements(double[] vector, List<int> elementsIndex)
        {
            var z = new double[elementsIndex.Count];
            for (var i = 0; i < elementsIndex.Count; i++)
                z[i] = vector[elementsIndex[i]];
            return z;
        }

    }
}
