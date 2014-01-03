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

namespace Accord.Statistics.Models.Regression
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Testing;
    using AForge;

    /// <summary>
    ///   Nominal Multinomial Logistic Regression.
    /// </summary>
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
    [Serializable]
    public class MultinomialLogisticRegression : ICloneable
    {

        private int inputs;
        private int categories;

        private double[][] coefficients;
        private double[][] standardErrors;


        //---------------------------------------------


        #region Constructors
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

            this.categories = categories;
            this.inputs = inputs;
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
        #endregion


        /// <summary>
        ///   Gets the coefficient vectors, in which the
        ///   first columns are always the intercept values.
        /// </summary>
        /// 
        public double[][] Coefficients
        {
            get { return coefficients; }
            set { coefficients = value; }
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
        public int Categories
        {
            get { return categories; }
        }

        /// <summary>
        ///   Gets the number of inputs of the model.
        /// </summary>
        public int Inputs
        {
            get { return inputs; }
        }



        /// <summary>
        ///   Computes the model output for the given input vector.
        /// </summary>
        /// 
        /// <remarks>
        ///   The first category is always 
        ///   considered the baseline category.
        /// </remarks>
        /// 
        /// <param name="input">The input vector.</param>
        /// <returns>The output value.</returns>
        /// 
        public double[] Compute(double[] input)
        {
            if (input.Length != inputs)
                throw new DimensionMismatchException("input");


            double[] responses = new double[categories];
            double sum = responses[0] = 1;

            for (int j = 1; j <= coefficients.Length; j++)
            {
                // Get category coefficients
                double[] c = coefficients[j - 1];

                double logit = c[0]; // intercept

                for (int i = 0; i < input.Length; i++)
                    logit += c[i + 1] * input[i];

                sum += responses[j] = Math.Exp(logit);
            }

            // Normalize the probabilities
            for (int i = 0; i < responses.Length; i++)
                responses[i] /= sum;

            return responses;
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
            double[] sums = output.Sum();

            double[] intercept = new double[Categories - 1];
            for (int i = 0; i < intercept.Length; i++)
                intercept[i] = Math.Log(sums[i + 1] / sums[0]);

            MultinomialLogisticRegression regression =
                new MultinomialLogisticRegression(Inputs, Categories, intercept);

            double ratio = GetLogLikelihoodRatio(input, output, regression);

            return new ChiSquareTest(ratio, (Inputs) * (Categories - 1));
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
            return ChiSquare(input, Statistics.Tools.Expand(classes));
        }

        /// <summary>
        ///   Gets the 95% confidence interval for the
        ///   Odds Ratio for a given coefficient.
        /// </summary>
        /// 
        /// <param name="category">
        ///   The category's index. 
        /// </param>
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
        ///   Gets the Odds Ratio for a given coefficient.
        /// </summary>
        /// <remarks>
        ///   The odds ratio can be computed raising Euler's number
        ///   (e ~~ 2.71) to the power of the associated coefficient.
        /// </remarks>
        /// 
        /// <param name="category">
        ///   The category's index. 
        /// </param>
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
        /// <param name="category">
        ///   The category's index. 
        /// </param>
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
            return GetLogLikelihood(inputs, Statistics.Tools.Expand(classes));
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
                double[] y = Compute(input[i]);
                double[] o = output[i];
                y = y.Multiply(o.Sum());

                for (int j = 0; j < y.Length; j++)
                {
                    if (o[j] > 0)
                        sum += o[j] * (Math.Log(y[j] / o[j]));

                    System.Diagnostics.Debug.Assert(!Double.IsNaN(sum));
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


        #region ICloneable Members


        /// <summary>
        ///   Creates a new MultinomialLogisticRegression that is a copy of the current instance.
        /// </summary>
        /// 
        public object Clone()
        {
            var mlr = new MultinomialLogisticRegression(Inputs, Categories);
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
