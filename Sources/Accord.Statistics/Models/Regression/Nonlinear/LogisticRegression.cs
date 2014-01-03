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
    using Accord.Statistics.Testing;
    using AForge;
    using Accord.Statistics.Links;

    /// <summary>
    ///   Binary Logistic Regression.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, logistic regression (sometimes called the logistic model or
    ///   Logit model) is used for prediction of the probability of occurrence of an
    ///   event by fitting data to a logistic curve. It is a generalized linear model
    ///   used for binomial regression.</para>
    /// <para>
    ///   Like many forms of regression analysis, it makes use of several predictor
    ///   variables that may be either numerical or categorical. For example, the
    ///   probability that a person has a heart attack within a specified time period
    ///   might be predicted from knowledge of the person's age, sex and body mass index.</para>
    /// <para>
    ///   Logistic regression is used extensively in the medical and social sciences
    ///   as well as marketing applications such as prediction of a customer's
    ///   propensity to purchase a product or cease a subscription.</para>  
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
    ///    // To verify this hypothesis, we are going to create a logistic
    ///    // regression model for those two inputs (age and smoking).
    ///    LogisticRegression regression = new LogisticRegression(inputs: 2);
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
    ///    // At this point, we can compute the odds ratio of our variables.
    ///    // In the model, the variable at 0 is always the intercept term, 
    ///    // with the other following in the sequence. Index 1 is the age
    ///    // and index 2 is whether the patient smokes or not.
    ///
    ///    // For the age variable, we have that individuals with
    ///    //   higher age have 1.021 greater odds of getting lung
    ///    //   cancer controlling for cigarette smoking.
    ///    double ageOdds = regression.GetOddsRatio(1); // 1.0208597028836701
    ///
    ///    // For the smoking/non smoking category variable, however, we
    ///    //   have that individuals who smoke have 5.858 greater odds
    ///    //   of developing lung cancer compared to those who do not 
    ///    //   smoke, controlling for age (remember, this is completely
    ///    //   fictional and for demonstration purposes only).
    ///    double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class LogisticRegression : GeneralizedLinearRegression
    {

        /// <summary>
        ///   Creates a new Logistic Regression Model.
        /// </summary>
        /// 
        /// <param name="inputs">The number of input variables for the model.</param>
        /// 
        public LogisticRegression(int inputs)
            : base(new LogitLinkFunction(), inputs) { }

        /// <summary>
        ///   Creates a new Logistic Regression Model.
        /// </summary>
        /// 
        /// <param name="inputs">The number of input variables for the model.</param>
        /// <param name="intercept">The starting intercept value. Default is 0.</param>
        /// 
        public LogisticRegression(int inputs, double intercept)
            : base(new LogitLinkFunction(), inputs, intercept) { }

        /// <summary>
        ///   Gets the 95% confidence interval for the
        ///   Odds Ratio for a given coefficient.
        /// </summary>
        /// 
        /// <param name="index">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        public DoubleRange GetConfidenceInterval(int index)
        {
            double coeff = Coefficients[index];
            double error = StandardErrors[index];

            double upper = coeff + 1.9599 * error;
            double lower = coeff - 1.9599 * error;

            DoubleRange ci = new DoubleRange(Math.Exp(lower), Math.Exp(upper));

            return ci;
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
        /// <param name="index">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        /// <returns>
        ///   The Odds Ratio for the given coefficient.
        /// </returns>
        /// 
        public double GetOddsRatio(int index)
        {
            return Math.Exp(Coefficients[index]);
        }
    }
}
