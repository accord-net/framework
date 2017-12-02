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
    using Accord.Math;
    using Accord.MachineLearning;
    using Math.Optimization;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Gradient optimization for Multinomial logistic regression fitting.
    /// </summary>
    /// 
    /// <examples>
    ///   <para>
    ///     The gradient optimization class allows multinomial logistic regression models to be learnt
    ///     using any mathematical optimization algorithm that implements the <see cref="IFunctionOptimizationMethod{TInput, TOutput}"/>
    ///     interface. </para>
    ///   <code source = "Unit Tests\Accord.Tests.Statistics\Models\Regression\MultinomialLogisticGradientDescentTest.cs" region="doc_learn_0" />
    ///   
    /// <para>Using Conjugate Gradient:</para>
    ///   <code source = "Unit Tests\Accord.Tests.Statistics\Models\Regression\MultinomialLogisticGradientDescentTest.cs" region="doc_learn_cg" />
    ///   
    /// <para>Using Gradient Descent:</para>
    ///   <code source = "Unit Tests\Accord.Tests.Statistics\Models\Regression\MultinomialLogisticGradientDescentTest.cs" region="doc_learn_gd" />
    ///   
    /// <para>Using BFGS:</para>
    ///   <code source = "Unit Tests\Accord.Tests.Statistics\Models\Regression\MultinomialLogisticGradientDescentTest.cs" region="doc_learn_bfgs" />
    /// </examples>
    /// 
    public class MultinomialLogisticLearning<TMethod> :
        ISupervisedLearning<MultinomialLogisticRegression, double[], int>,
        ISupervisedLearning<MultinomialLogisticRegression, double[], int[]>,
        ISupervisedLearning<MultinomialLogisticRegression, double[], double[]>,
        ISupervisedLearning<MultinomialLogisticRegression, double[], bool[]>
        where TMethod : IFunctionOptimizationMethod<double[], double>, new()
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private MultinomialLogisticRegression regression;
        private TMethod method = new TMethod();

        private double[][] inputs;
        private int[] outputs;

        private double[] gradient;
        private double[] log_y_hat;

        int miniBatchSize;
        IntRange[] miniBatches;
        int current = 0;

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
        ///   Gets or sets the optimization method used to optimize
        ///   the parameters (learn) the <see cref="MultinomialLogisticRegression"/>.
        /// </summary>
        /// 
        public TMethod Method
        {
            get { return method; }
            set { method = value; }
        }

        /// <summary>
        ///   Gets or sets the number of samples to be used as the mini-batch.
        ///   If set to 0 (or a negative number) the total number of training
        ///   samples will be used as the mini-batch.
        /// </summary>
        /// 
        /// <value>The size of the mini batch.</value>
        /// 
        public int MiniBatchSize
        {
            get { return miniBatchSize; }
            set { miniBatchSize = value; }
        }

        /// <summary>
        ///   Creates a new <see cref="MultinomialLogisticLearning{TMethod}"/>.
        /// </summary>
        /// 
        public MultinomialLogisticLearning()
        {
        }

        /// <summary>
        ///   Creates a new <see cref="MultinomialLogisticLearning{TMethod}"/>.
        /// </summary>
        /// <param name="regression">The regression to estimate.</param>
        /// 
        public MultinomialLogisticLearning(MultinomialLogisticRegression regression)
            : this()
        {
            init(regression);
        }

        private void init(MultinomialLogisticRegression regression)
        {
            this.regression = regression;
        }

        private void compute(double[] w, double[] x, double[] log_responses)
        {
            log_responses[0] = 0;
            for (int j = 1, c = 0; j < log_responses.Length; j++)
            {
                double logit = w[c++]; // intercept
                for (int k = 0; k < x.Length; k++)
                    logit += w[c++] * x[k];

                log_responses[j] = logit;
            }

            double sum = Special.LogSumExp(log_responses);

            // Normalize the probabilities
            for (int j = 0; j < log_responses.Length; j++)
                log_responses[j] -= sum;

#if DEBUG
            double[] exp = log_responses.Exp();
            double one = exp.Sum();
            Accord.Diagnostics.Debug.Assert(one.IsEqual(1, atol: 1e-5));
#endif
        }

        internal double crossEntropy(double[] w)
        {
            double sum = 0;

            // Negative error log-likelihood / cross-entropy error function
            for (int j = 0; j < inputs.Length; j++)
            {
                this.compute(w, inputs[j], log_y_hat);
                sum -= log_y_hat[outputs[j]];
            }

            return sum / (double)inputs.Length;
        }

        internal double[] crossEntropyGradient(double[] w)
        {
            gradient.Clear();

            IntRange miniBatch = miniBatches[current++];
            if (current >= miniBatches.Length)
                current = 0;

            for (int i = miniBatch.Min; i < miniBatch.Max; i++)
            {
                double[] x = inputs[i];
                int y = outputs[i];

                this.compute(w, x, log_y_hat);

                for (int s = 1, c = 0; s < log_y_hat.Length; s++)
                {
                    double h = Math.Exp(log_y_hat[s]);

                    if (s == y)
                    {
                        gradient[c++] += 1 * h - 1;
                        for (int p = 0; p < x.Length; p++)
                            gradient[c++] += x[p] * h - x[p];
                    }
                    else
                    {
                        gradient[c++] += h;
                        for (int p = 0; p < x.Length; p++)
                            gradient[c++] += x[p] * h;
                    }
                }
            }

            for (int i = 0; i < gradient.Length; i++)
                gradient[i] /= (double)miniBatch.Length;

            return gradient;
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
        public MultinomialLogisticRegression Learn(double[][] x, int[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
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
        public MultinomialLogisticRegression Learn(double[][] x, double[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
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
        public MultinomialLogisticRegression Learn(double[][] x, bool[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
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
        public MultinomialLogisticRegression Learn(double[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            this.inputs = x;
            this.outputs = y;

            if (regression == null)
                regression = new Regression.MultinomialLogisticRegression(x.Columns(), y.Max() + 1);

            if (method.NumberOfVariables != regression.NumberOfParameters)
                method.NumberOfVariables = regression.NumberOfParameters;

            method.Function = crossEntropy;

            var gom = method as IGradientOptimizationMethod;
            if (gom != null)
                gom.Gradient = crossEntropyGradient;

            var sc = method as ISupportsCancellation;
            if (sc != null)
                sc.Token = Token;

            if (miniBatchSize <= 0)
                miniBatchSize = x.Length;

            this.gradient = new double[regression.NumberOfParameters];
            this.log_y_hat = new double[regression.NumberOfOutputs];

            this.current = 0;
            this.miniBatches = new IntRange[(int)Math.Floor(x.Length / (double)miniBatchSize)];
            for (int i = 0; i < miniBatches.Length; i++)
                miniBatches[i] = new IntRange(i, Math.Min(i + miniBatchSize, x.Length));

            bool success = method.Minimize();

            for (int i = 0, k = 0; i < regression.Coefficients.Length; i++)
                for (int j = 0; j < regression.Coefficients[i].Length; j++, k++)
                    regression.Coefficients[i][j] = method.Solution[k];

            return regression;
        }
    }
}
