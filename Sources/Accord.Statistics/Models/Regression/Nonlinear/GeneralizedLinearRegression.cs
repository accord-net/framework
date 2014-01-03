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
    using System.Linq;
    using Accord.Statistics.Links;
    using Accord.Statistics.Testing;

    /// <summary>
    ///   Generalized Linear Model Regression.
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Bishop, Christopher M.; Pattern Recognition and Machine Learning. 
    ///       Springer; 1st ed. 2006.</description></item>
    ///     <item><description>
    ///       Amos Storkey. (2005). Learning from Data: Learning Logistic Regressors. School of Informatics.
    ///       Available on: http://www.inf.ed.ac.uk/teaching/courses/lfd/lectures/logisticlearn-print.pdf </description></item>
    ///     <item><description>
    ///       Cosma Shalizi. (2009). Logistic Regression and Newton's Method. Available on:
    ///       http://www.stat.cmu.edu/~cshalizi/350/lectures/26/lecture-26.pdf </description></item>
    ///     <item><description>
    ///       Edward F. Conor. Logistic Regression. Website. Available on: 
    ///       http://userwww.sfsu.edu/~efc/classes/biol710/logistic/logisticreg.htm </description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///    // Suppose we have the following data about some patients.
    ///    // The first variable is continuous and represent patient
    ///    // age. The second variable is dichotomic and give whether
    ///    // they smoke or not (This is completely fictional data).
    ///    double[][] input =
    ///    {
    ///        new double[] { 55, 0 }, // 0 - no cancer
    ///        new double[] { 28, 0 }, // 0
    ///        new double[] { 65, 1 }, // 0
    ///        new double[] { 46, 0 }, // 1 - have cancer
    ///        new double[] { 86, 1 }, // 1
    ///        new double[] { 56, 1 }, // 1
    ///        new double[] { 85, 0 }, // 0
    ///        new double[] { 33, 0 }, // 0
    ///        new double[] { 21, 1 }, // 0
    ///        new double[] { 42, 1 }, // 1
    ///    };
    ///
    ///    // We also know if they have had lung cancer or not, and 
    ///    // we would like to know whether smoking has any connection
    ///    // with lung cancer (This is completely fictional data).
    ///    double[] output =
    ///    {
    ///        0, 0, 0, 1, 1, 1, 0, 0, 0, 1
    ///    };
    ///
    ///
    ///    // To verify this hypothesis, we are going to create a GLM
    ///    // regression model for those two inputs (age and smoking).
    ///    var regression = new GeneralizedLinearRegression(new ProbitLinkFunction(), inputs: 2);
    ///
    ///    // Next, we are going to estimate this model. For this, we
    ///    // will use the Iteratively Reweighted Least Squares method.
    ///    var teacher = new IterativeReweightedLeastSquares(regression);
    ///
    ///    // Now, we will iteratively estimate our model. The Run method returns
    ///    // the maximum relative change in the model parameters and we will use
    ///    // it as the convergence criteria.
    ///
    ///    double delta = 0;
    ///    do
    ///    {
    ///        // Perform an iteration
    ///        delta = teacher.Run(input, output);
    ///
    ///    } while (delta > 0.001);
    ///
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class GeneralizedLinearRegression : ICloneable
    {

        private ILinkFunction linkFunction;
        private double[] coefficients;
        private double[] standardErrors;


        //---------------------------------------------


        #region Constructor
        /// <summary>
        ///   Creates a new Generalized Linear Regression Model.
        /// </summary>
        /// 
        /// <param name="function">The link function to use.</param>
        /// <param name="inputs">The number of input variables for the model.</param>
        /// 
        public GeneralizedLinearRegression(ILinkFunction function, int inputs)
        {
            this.linkFunction = function;
            this.coefficients = new double[inputs + 1];
            this.standardErrors = new double[inputs + 1];
        }

        /// <summary>
        ///   Creates a new Generalized Linear Regression Model.
        /// </summary>
        /// 
        /// <param name="function">The link function to use.</param>
        /// <param name="inputs">The number of input variables for the model.</param>
        /// <param name="intercept">The starting intercept value. Default is 0.</param>
        /// 
        public GeneralizedLinearRegression(ILinkFunction function, int inputs, double intercept)
            : this(function, inputs)
        {
            this.coefficients[0] = intercept;
        }

        /// <summary>
        ///   Creates a new Generalized Linear Regression Model.
        /// </summary>
        /// 
        /// <param name="function">The link function to use.</param>
        /// <param name="coefficients">The coefficient vector.</param>
        /// <param name="standardErrors">The standard error vector.</param>
        /// 
        public GeneralizedLinearRegression(ILinkFunction function, 
            double[] coefficients, double[] standardErrors)
        {
            this.linkFunction = function;
            this.coefficients = coefficients;
            this.standardErrors = standardErrors;
        }
        #endregion


        //---------------------------------------------


        #region Properties
        /// <summary>
        ///   Gets the coefficient vector, in which the
        ///   first value is always the intercept value.
        /// </summary>
        /// 
        public double[] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        ///   Gets the standard errors associated with each
        ///   coefficient during the model estimation phase.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get { return standardErrors; }
        }

        /// <summary>
        ///   Gets the number of inputs handled by this model.
        /// </summary>
        /// 
        public int Inputs
        {
            get { return coefficients.Length - 1; }
        }

        /// <summary>
        ///   Gets the link function used by
        ///   this generalized linear model.
        /// </summary>
        /// 
        public ILinkFunction Link
        {
            get { return linkFunction; }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        /// <summary>
        ///   Computes the model output for the given input vector.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <returns>The output value.</returns>
        /// 
        public double Compute(double[] input)
        {
            double sum = coefficients[0];

            for (int i = 1; i < coefficients.Length; i++)
                sum += input[i - 1] * coefficients[i];

            return linkFunction.Inverse(sum);
        }

        /// <summary>
        ///   Computes the model output for each of the given input vectors.
        /// </summary>
        /// 
        /// <param name="input">The array of input vectors.</param>
        /// <returns>The array of output values.</returns>
        /// 
        public double[] Compute(double[][] input)
        {
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = Compute(input[i]);

            return output;
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
        /// <param name="index">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        public WaldTest GetWaldTest(int index)
        {
            return new WaldTest(coefficients[index], 0.0, standardErrors[index]);
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
        public double GetLogLikelihood(double[][] input, double[] output)
        {
            double sum = 0;

            for (int i = 0; i < input.Length; i++)
            {
                double actualOutput = Compute(input[i]);
                double expectedOutput = output[i];

                if (actualOutput != 0)
                    sum += expectedOutput * Math.Log(actualOutput);

                if (actualOutput != 1)
                    sum += (1 - expectedOutput) * Math.Log(1 - actualOutput);

                System.Diagnostics.Debug.Assert(!Double.IsNaN(sum));
            }

            return sum;
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
        public double GetDeviance(double[][] input, double[] output)
        {
            return -2.0 * GetLogLikelihood(input, output);
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
        /// 
        /// <returns>The Log-Likelihood ratio (a measure of performance
        /// between two models) calculated over the given data sets.</returns>
        /// 
        public double GetLogLikelihoodRatio(double[][] input, double[] output, GeneralizedLinearRegression regression)
        {
            return 2.0 * (this.GetLogLikelihood(input, output) - regression.GetLogLikelihood(input, output));
        }


        /// <summary>
        ///   The likelihood ratio test of the overall model, also called the model chi-square test.
        /// </summary>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="output">A set of output data.</param>
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
        public ChiSquareTest ChiSquare(double[][] input, double[] output)
        {
            double y0 = output.Count(y => y == 0.0);
            double y1 = output.Length - y0;

            GeneralizedLinearRegression regression = new GeneralizedLinearRegression(linkFunction, 
                Inputs, Math.Log(y1 / y0));

            double ratio = GetLogLikelihoodRatio(input, output, regression);
            return new ChiSquareTest(ratio, coefficients.Length - 1);
        }



        /// <summary>
        ///   Creates a new GeneralizedLinearRegression that is a copy of the current instance.
        /// </summary>
        /// 
        public object Clone()
        {
            ILinkFunction function = (ILinkFunction)linkFunction.Clone();

            var regression = new GeneralizedLinearRegression(function, coefficients.Length);
            regression.coefficients = (double[])this.coefficients.Clone();
            regression.standardErrors = (double[])this.standardErrors.Clone();

            return regression;
        }


        /// <summary>
        ///   Creates a GeneralizedLinearRegression from a <see cref="LogisticRegression"/> object. 
        /// </summary>
        /// 
        /// <param name="regression">A <see cref="LogisticRegression"/> object.</param>
        /// <param name="makeCopy">True to make a copy of the logistic regression values, false
        /// to use the actual values. If the actual values are used, changes done on one model
        /// will be reflected on the other model.</param>
        /// 
        /// <returns>A new <see cref="GeneralizedLinearRegression"/> which is a copy of the 
        /// given <see cref="LogisticRegression"/>.</returns>
        /// 
        public static GeneralizedLinearRegression FromLogisticRegression(
            LogisticRegression regression, bool makeCopy)
        {
            if (makeCopy)
            {
                double[] coefficients = (double[])regression.Coefficients.Clone();
                double[] standardErrors = (double[])regression.StandardErrors.Clone();
                return new GeneralizedLinearRegression(new LogitLinkFunction(),
                    coefficients, standardErrors);
            }
            else
            {
                return new GeneralizedLinearRegression(new LogitLinkFunction(),
                    regression.Coefficients, regression.StandardErrors);
            }
        }
        #endregion


    }
}
