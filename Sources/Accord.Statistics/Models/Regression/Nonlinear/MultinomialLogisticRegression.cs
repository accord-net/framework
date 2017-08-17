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

namespace Accord.Statistics.Models.Regression
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Testing;
    using Accord.MachineLearning;
    using System.Runtime.Serialization;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Math.Optimization;
    using Accord.Compat;

    /// <summary>
    ///   Nominal Multinomial Logistic Regression.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     The default optimizer for <see cref="MultinomialLogisticRegression"/> is the <see cref="LowerBoundNewtonRaphson"/> class:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\MultinomialLogisticRegressionTest.cs" region="doc_learn" />
    ///   
    ///   <para>
    ///     Additionally, the <see cref="MultinomialLogisticLearning{TMethod}"/> class allows multinomial logistic regression models to be learnt using any 
    ///     mathematical  optimization algorithm that implements the <see cref="IFunctionOptimizationMethod{TInput, TOutput}"/> interface. </para>
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
    /// </example>
    /// 
    [Serializable]
    public class MultinomialLogisticRegression : MulticlassLikelihoodClassifierBase<double[]>,
        ICloneable
    {

#pragma warning disable 0649
        [Obsolete]
        private int inputs;
        [Obsolete]
        private int categories;
#pragma warning restore 0649

        private double[][] coefficients;
        private double[][] standardErrors;



        /// <summary>
        ///   Creates a new Multinomial Logistic Regression Model.
        /// </summary>
        /// 
        /// <param name="inputs">The number of input variables for the model.</param>
        /// <param name="categories">The number of categories for the model.</param>
        /// 
        public MultinomialLogisticRegression(int inputs, int categories)
        {
            if (inputs < 1)
                throw new ArgumentOutOfRangeException("inputs");
            if (categories < 1)
                throw new ArgumentOutOfRangeException("categories");

            this.NumberOfOutputs = categories;
            this.NumberOfClasses = categories;
            this.NumberOfInputs = inputs;
            this.coefficients = new double[categories - 1][];
            this.standardErrors = new double[categories - 1][];

            for (int i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] = new double[inputs + 1];
                standardErrors[i] = new double[inputs + 1];
            }
        }

        /// <summary>
        ///   Creates a new Multinomial Logistic Regression Model.
        /// </summary>
        /// 
        /// <param name="inputs">The number of input variables for the model.</param>
        /// <param name="categories">The number of categories for the model.</param>
        /// <param name="intercepts">The initial values for the intercepts.</param>
        /// 
        public MultinomialLogisticRegression(int inputs, int categories, double[] intercepts)
            : this(inputs, categories)
        {
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i][0] = intercepts[i];
        }


        /// <summary>
        ///   Gets the coefficient vectors, in which the
        ///   first column are the intercept values.
        /// </summary>
        /// 
        public double[][] Coefficients
        {
            get { return coefficients; }
            set { coefficients = value; }
        }

        /// <summary>
        ///   Gets the total number of parameters in the multinomial 
        ///   logistic regression [(categories - 1) * (inputs + 1)].
        /// </summary>
        /// 
        public int NumberOfParameters
        {
            get { return coefficients.GetTotalLength(); }
        }

        /// <summary>
        ///   Gets the standard errors associated with each
        ///   coefficient during the model estimation phase.
        /// </summary>
        /// 
        public double[][] StandardErrors
        {
            get { return standardErrors; }
        }

        /// <summary>
        ///   Gets the number of categories of the model.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfOutputs instead.")]
        public int Categories
        {
            get { return NumberOfOutputs; }
        }

        /// <summary>
        ///   Gets the number of inputs of the model.
        /// </summary>
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Inputs
        {
            get { return NumberOfInputs; }
        }



        /// <summary>
        ///   Computes the model output for the given input vector.
        /// </summary>
        /// 
        /// <remarks>
        ///   The first category is always considered the baseline category.
        /// </remarks>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>The output value.</returns>
        /// 
        [Obsolete("Please use Probabilities() instead.")]
        public double[] Compute(double[] input)
        {
            return Probabilities(input);
        }

        /// <summary>
        ///   Computes the model outputs for the given input vectors.
        /// </summary>
        /// 
        /// <remarks>
        ///   The first category is always considered the baseline category.
        /// </remarks>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>The output value.</returns>
        /// 
        [Obsolete("Please use Probabilities() instead.")]
        public double[][] Compute(double[][] input)
        {
            return Probabilities(input);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <returns>System.Double.</returns>
        public override double LogLikelihood(double[] input, int classIndex)
        {
            if (classIndex == 0)
                return 0;

            // Get category coefficients
            double[] c = coefficients[classIndex - 1];

            double logit = c[0]; // intercept
            for (int i = 0; i < input.Length; i++)
                logit += c[i + 1] * input[i];

            return logit;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns>System.Double[].</returns>
        public override double[] LogLikelihoods(double[] input, out int decision, double[] result)
        {
            for (int j = 1; j < coefficients.Length; j++)
            {
                // Get category coefficients
                double[] c = coefficients[j - 1];

                double logit = c[0]; // intercept
                for (int i = 0; i < input.Length; i++)
                    logit += c[i + 1] * input[i];

                result[j] = logit;
            }

            decision = result.ArgMax();

            return result;
        }

        /// <summary>
        ///   The likelihood ratio test of the overall model, also called the model chi-square test.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>
        ///   The Chi-square test, also called the likelihood ratio test or the log-likelihood test
        ///   is based on the deviance of the model (-2*log-likelihood). The log-likelihood ratio test 
        ///   indicates whether there is evidence of the need to move from a simpler model to a more
        ///   complicated one (where the simpler model is nested within the complicated one).</para>
        ///   <para>
        ///   The difference between the log-likelihood ratios for the researcher's model and a
        ///   simpler model is often called the "model chi-square".</para>
        /// </remarks>
        /// 
        public ChiSquareTest ChiSquare(double[][] input, double[][] output)
        {
            double[] sums = output.Sum(0);

            double[] intercept = new double[NumberOfOutputs - 1];
            for (int i = 0; i < intercept.Length; i++)
                intercept[i] = Math.Log(sums[i + 1] / sums[0]);

            var regression = new MultinomialLogisticRegression(NumberOfInputs, NumberOfOutputs, intercept);

            double ratio = GetLogLikelihoodRatio(input, output, regression);

            return new ChiSquareTest(ratio, (NumberOfInputs) * (NumberOfOutputs - 1));
        }

        /// <summary>
        ///   The likelihood ratio test of the overall model, also called the model chi-square test.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>
        ///   The Chi-square test, also called the likelihood ratio test or the log-likelihood test
        ///   is based on the deviance of the model (-2*log-likelihood). The log-likelihood ratio test 
        ///   indicates whether there is evidence of the need to move from a simpler model to a more
        ///   complicated one (where the simpler model is nested within the complicated one).</para>
        ///   <para>
        ///   The difference between the log-likelihood ratios for the researcher's model and a
        ///   simpler model is often called the "model chi-square".</para>
        /// </remarks>
        /// 
        public ChiSquareTest ChiSquare(double[][] input, int[] classes)
        {
            return ChiSquare(input, Jagged.OneHot(classes));
        }

        /// <summary>
        ///   Gets the 95% confidence interval for the Odds Ratio for a given coefficient.
        /// </summary>
        /// 
        /// <param name="category">The category's index. </param>
        /// 
        /// <param name="coefficient">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        public DoubleRange GetConfidenceInterval(int category, int coefficient)
        {
            double coeff = coefficients[category][coefficient];
            double error = standardErrors[category][coefficient];

            double upper = coeff + 1.9599 * error;
            double lower = coeff - 1.9599 * error;

            DoubleRange ci = new DoubleRange(Math.Exp(lower), Math.Exp(upper));

            return ci;
        }

        /// <summary>
        ///   Gets the 95% confidence intervals for the Odds Ratios for all coefficients.
        /// </summary>
        /// 
        /// <param name="category">The category's index.</param>
        /// 
        public DoubleRange[] GetConfidenceInterval(int category)
        {
            var ranges = new DoubleRange[NumberOfInputs + 1];
            for (int i = 0; i < ranges.Length; i++)
                ranges[i] = GetConfidenceInterval(category, i);
            return ranges;
        }

        /// <summary>
        ///   Gets the Odds Ratio for a given coefficient.
        /// </summary>
        /// 
        /// <remarks>
        ///   The odds ratio can be computed raising Euler's number
        ///   (e ~~ 2.71) to the power of the associated coefficient.
        /// </remarks>
        /// 
        /// <param name="category">The category index.</param>
        /// 
        /// <param name="coefficient">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        /// <returns>
        ///   The Odds Ratio for the given coefficient.
        /// </returns>
        /// 
        public double GetOddsRatio(int category, int coefficient)
        {
            return Math.Exp(coefficients[category][coefficient]);
        }

        /// <summary>
        ///   Gets the Odds Ratio for all coefficients.
        /// </summary>
        /// 
        /// <remarks>
        ///   The odds ratio can be computed raising Euler's number
        ///   (e ~~ 2.71) to the power of the associated coefficient.
        /// </remarks>
        /// 
        /// <param name="category">The category index.</param>
        /// 
        /// <returns>
        ///   The Odds Ratio for the given coefficient.
        /// </returns>
        /// 
        public double[] GetOddsRatio(int category)
        {
            var odds = new double[NumberOfInputs + 1];
            for (int i = 0; i < odds.Length; i++)
                odds[i] = GetOddsRatio(category, i);
            return odds;
        }

        /// <summary>
        ///   Gets the Wald Test for a given coefficient.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Wald statistical test is a test for a model parameter in which
        ///   the estimated parameter θ is compared with another proposed parameter
        ///   under the assumption that the difference between them will be approximately
        ///   normal. There are several problems with the use of the Wald test. Please
        ///   take a look on substitute tests based on the log-likelihood if possible.
        /// </remarks>
        /// 
        /// <param name="category">The category index.</param>
        /// 
        /// <param name="coefficient">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        public WaldTest GetWaldTest(int category, int coefficient)
        {
            return new WaldTest(coefficients[category][coefficient], 0.0, standardErrors[category][coefficient]);
        }

        /// <summary>
        ///   Gets the Wald Test for all coefficients.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Wald statistical test is a test for a model parameter in which
        ///   the estimated parameter θ is compared with another proposed parameter
        ///   under the assumption that the difference between them will be approximately
        ///   normal. There are several problems with the use of the Wald test. Please
        ///   take a look on substitute tests based on the log-likelihood if possible.
        /// </remarks>
        /// 
        /// <param name="category">The category's index.</param>
        /// 
        public WaldTest[] GetWaldTest(int category)
        {
            var tests = new WaldTest[NumberOfInputs + 1];
            for (int i = 0; i < tests.Length; i++)
                tests[i] = GetWaldTest(category, i);
            return tests;
        }

        /// <summary>
        ///   Gets the Deviance for the model.
        /// </summary>
        /// 
        /// <remarks>
        ///   The deviance is defined as -2*Log-Likelihood.
        /// </remarks>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="output">A set of output data.</param>
        /// <returns>
        ///   The deviance (a measure of performance) of the model
        ///   calculated over the given data sets.
        /// </returns>
        /// 
        public double GetDeviance(double[][] input, double[][] output)
        {
            return -2.0 * GetLogLikelihood(input, output);
        }

        /// <summary>
        ///   Gets the Deviance for the model.
        /// </summary>
        /// 
        /// <remarks>
        ///   The deviance is defined as -2*Log-Likelihood.
        /// </remarks>
        /// 
        /// <param name="inputs">A set of input data.</param>
        /// <param name="classes">A set of output data.</param>
        /// <returns>
        ///   The deviance (a measure of performance) of the model
        ///   calculated over the given data sets.
        /// </returns>
        /// 
        public double GetDeviance(double[][] inputs, int[] classes)
        {
            return -2.0 * GetLogLikelihood(inputs, classes);
        }

        /// <summary>
        ///   Gets the Deviance for the model.
        /// </summary>
        /// 
        /// <remarks>
        ///   The deviance is defined as -2*Log-Likelihood.
        /// </remarks>
        /// 
        /// <param name="inputs">A set of input data.</param>
        /// <param name="classes">A set of output data.</param>
        /// <returns>
        ///   The deviance (a measure of performance) of the model
        ///   calculated over the given data sets.
        /// </returns>
        /// 
        public double GetLogLikelihood(double[][] inputs, int[] classes)
        {
            return GetLogLikelihood(inputs, Jagged.OneHot(classes));
        }

        /// <summary>
        ///   Gets the Log-Likelihood for the model.
        /// </summary>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="output">A set of output data.</param>
        /// <returns>
        ///   The Log-Likelihood (a measure of performance) of
        ///   the model calculated over the given data sets.
        /// </returns>
        /// 
        public double GetLogLikelihood(double[][] input, double[][] output)
        {
            double sum = 0;

            for (int i = 0; i < input.Length; i++)
            {
                double[] y = Probabilities(input[i]);
                double[] o = output[i];
                y = y.Multiply(o.Sum());

                for (int j = 0; j < y.Length; j++)
                {
                    if (o[j] > 0)
                        sum += o[j] * (Math.Log(y[j] / o[j]));

                    Accord.Diagnostics.Debug.Assert(!Double.IsNaN(sum));
                }
            }

            return sum;
        }

        /// <summary>
        ///   Gets the Log-Likelihood Ratio between two models.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Log-Likelihood ratio is defined as 2*(LL - LL0).
        /// </remarks>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="output">A set of output data.</param>
        /// <param name="regression">Another Logistic Regression model.</param>
        /// <returns>The Log-Likelihood ratio (a measure of performance
        /// between two models) calculated over the given data sets.</returns>
        /// 
        public double GetLogLikelihoodRatio(double[][] input, double[][] output, MultinomialLogisticRegression regression)
        {
            return 2.0 * (this.GetLogLikelihood(input, output) - regression.GetLogLikelihood(input, output));
        }


        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            if (NumberOfInputs == 0 && NumberOfOutputs == 0)
            {
#pragma warning disable 0618, 0612
                NumberOfOutputs = inputs;
                NumberOfOutputs = categories;
#pragma warning restore 0618, 0612
            }
        }

        #region ICloneable Members


        /// <summary>
        ///   Creates a new MultinomialLogisticRegression that is a copy of the current instance.
        /// </summary>
        /// 
        public object Clone()
        {
            var mlr = new MultinomialLogisticRegression(NumberOfInputs, NumberOfOutputs);
            for (int i = 0; i < coefficients.Length; i++)
            {
                for (int j = 0; j < coefficients[i].Length; j++)
                {
                    mlr.coefficients[i][j] = coefficients[i][j];
                    mlr.standardErrors[i][j] = standardErrors[i][j];
                }
            }

            return mlr;
        }

        #endregion

    }
}
