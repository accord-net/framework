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
    using Accord.Math.Optimization.Losses;
    using Fitting;
    using System;
    using MachineLearning;
    using Accord.Statistics.Testing;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Compat;

    /// <summary>
    ///   Simple Linear Regression of the form y = Ax + B.
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
    /// <para>
    ///   Now, let's say we would like to perform a regression using an
    ///   intermediary transformation, such as for example logarithmic
    ///   regression. In this case, all we have to do is to first transform
    ///   the input variables into the desired domain, then apply the
    ///   regression as normal:</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\LogarithmRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="OrdinaryLeastSquares"/>
    /// <seealso cref="MultipleLinearRegression"/>
    /// <seealso cref="MultivariateLinearRegression"/>
    /// 
    [Serializable]
#pragma warning disable 612, 618
    public class SimpleLinearRegression : TransformBase<double, double>, ILinearRegression
#pragma warning restore 612, 618
    {
        [Obsolete]
        private MultipleLinearRegression regression;

        private double slope;
        private double intercept;

        /// <summary>
        ///   Creates a new Simple Linear Regression of the form y = Ax + B.
        /// </summary>
        /// 
        public SimpleLinearRegression()
        {
            NumberOfInputs = 1;
            NumberOfOutputs = 1;
        }

        /// <summary>
        ///   Angular coefficient (Slope).
        /// </summary>
        /// 
        public double Slope
        {
            get { return slope; }
            set { slope = value; }
        }

        /// <summary>
        ///   Linear coefficient (Intercept).
        /// </summary>
        /// 
        public double Intercept
        {
            get { return intercept; }
            set { intercept = value; }
        }

        /// <summary>
        ///   Gets the number of parameters in the model (returns 2).
        /// </summary>
        /// 
        public int NumberOfParameters
        {
            get { return 2; }
        }

        /// <summary>
        ///   Performs the regression using the input and output
        ///   data, returning the sum of squared errors of the fit.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The output data.</param>
        /// <returns>The regression Sum-of-Squares error.</returns>
        /// 
        [Obsolete("Please use the OrdinaryLeastSquares class.")]
        public double Regress(double[] inputs, double[] outputs)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[][] X = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                X[i] = new double[] { 1.0, inputs[i] };

#pragma warning disable 612, 618
            regression = new MultipleLinearRegression(2, false);
            double err = regression.Regress(X, outputs);
            slope = regression.Coefficients[1];
            intercept = regression.Coefficients[0];
#pragma warning restore 612, 618
            return err;
        }

        /// <summary>
        ///   Computes the regression output for a given input.
        /// </summary>
        /// 
        /// <param name="input">An array of input values.</param>
        /// <returns>The array of calculated output values.</returns>
        /// 
        [Obsolete("Please use Transform instead.")]
        public double[] Compute(double[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                output[i] = Compute(input[i]);
            return output;
        }

        /// <summary>
        ///   Computes the regression for a single input.
        /// </summary>
        /// 
        /// <param name="input">The input value.</param>
        /// <returns>The calculated output.</returns>
        /// 
        [Obsolete("Please use Transform instead.")]
        public double Compute(double input)
        {
            return Slope * input + Intercept;
        }

        /// <summary>
        ///   Gets the coefficient of determination, as known as R² (r-squared).
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>
        ///    The coefficient of determination is used in the context of statistical models
        ///    whose main purpose is the prediction of future outcomes on the basis of other
        ///    related information. It is the proportion of variability in a data set that
        ///    is accounted for by the statistical model. It provides a measure of how well
        ///    future outcomes are likely to be predicted by the model.</para>
        ///   <para>
        ///    The R² coefficient of determination is a statistical measure of how well the
        ///    regression line approximates the real data points. An R² of 1.0 indicates
        ///    that the regression line perfectly fits the data.</para> 
        ///   <para>
        ///    This method uses the <see cref="RSquaredLoss"/> class to compute the R²
        ///    coefficient. Please see the documentation for <see cref="RSquaredLoss"/>
        ///    for more details, including usage examples.</para>
        /// </remarks>
        /// 
        /// <returns>The R² (r-squared) coefficient for the given data.</returns>
        /// 
        /// <seealso cref="RSquaredLoss"/>
        /// 
        public double CoefficientOfDetermination(double[] inputs, double[] outputs, bool adjust, double[] weights = null)
        {
            var rsquared = new RSquaredLoss(NumberOfInputs, outputs);

            rsquared.Adjust = adjust;

            if (weights != null)
                rsquared.Weights = weights;

            return rsquared.Loss(Transform(inputs));
        }

        /// <summary>
        ///   Gets the coefficient of determination, or R² (r-squared).
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>
        ///    The coefficient of determination is used in the context of statistical models
        ///    whose main purpose is the prediction of future outcomes on the basis of other
        ///    related information. It is the proportion of variability in a data set that
        ///    is accounted for by the statistical model. It provides a measure of how well
        ///    future outcomes are likely to be predicted by the model.</para>
        ///   <para>
        ///    The R² coefficient of determination is a statistical measure of how well the
        ///    regression line approximates the real data points. An R² of 1.0 indicates
        ///    that the regression line perfectly fits the data.</para> 
        ///   <para>
        ///    This method uses the <see cref="RSquaredLoss"/> class to compute the R²
        ///    coefficient. Please see the documentation for <see cref="RSquaredLoss"/>
        ///    for more details, including usage examples.</para>
        /// </remarks>
        /// 
        /// <returns>The R² (r-squared) coefficient for the given data.</returns>
        /// 
        /// <seealso cref="RSquaredLoss"/>
        /// 
        public double CoefficientOfDetermination(double[] inputs, double[] outputs, double[] weights = null)
        {
            return CoefficientOfDetermination(inputs, outputs, false, weights);
        }


        /// <summary>
        ///   Returns a System.String representing the regression.
        /// </summary>
        /// 
        public override string ToString()
        {
            return ToString(null as string);
        }

        /// <summary>
        ///   Returns a System.String representing the regression.
        /// </summary>
        /// 
        public string ToString(string format)
        {
            return ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///   Returns a System.String representing the regression.
        /// </summary>
        /// 
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        ///   Returns a System.String representing the regression.
        /// </summary>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string a = format != null ?
                Slope.ToString(format, formatProvider) :
                Slope.ToString(formatProvider);

            string b = format != null ?
                Intercept.ToString(format, formatProvider) :
                Intercept.ToString(formatProvider);

            return String.Format("y(x) = {0}x + {1}", a, b);
        }

        /// <summary>
        ///   Creates a new linear regression directly from data points.
        /// </summary>
        /// 
        /// <param name="x">The input vectors <c>x</c>.</param>
        /// <param name="y">The output vectors <c>y</c>.</param>
        /// 
        /// <returns>A linear regression f(x) that most approximates y.</returns>
        /// 
        public static SimpleLinearRegression FromData(double[] x, double[] y)
        {
            return new OrdinaryLeastSquares().Learn(x, y);
        }


#pragma warning disable 612, 618
        [Obsolete("Please use Transform instead.")]
        double[] ILinearRegression.Compute(double[] inputs)
        {
            if (inputs.Length > 1)
                throw new ArgumentException("Simple regression supports only one-length input vectors", "inputs");

            return new double[] { this.Compute(inputs[0]) };
        }
#pragma warning restore 612, 618

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double Transform(double input)
        {
            return Slope * input + Intercept;
        }

        /// <summary>
        /// Gets the degrees of freedom when fitting the regression.
        /// </summary>
        /// 
        public double GetDegreesOfFreedom(int numberOfSamples)
        {
            return numberOfSamples - NumberOfParameters;
        }

        /// <summary>
        /// Gets the overall regression standard error.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs used to train the model.</param>
        /// <param name="outputs">The outputs used to train the model.</param>
        /// 
        public double GetStandardError(double[] inputs, double[] outputs)
        {
            double meanX = inputs.Mean();
            double meanY = outputs.Mean();

            double xx = 0;
            double yy = 0;
            double xy = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double x = inputs[i] - meanX;
                double y = outputs[i] - meanY;
                xx += x * x;
                xy += x * y;
                yy += y * y;
            }

            double ss = yy - (xy * xy) / xx;

            return Math.Sqrt(ss / GetDegreesOfFreedom(inputs.Length));
        }

        /// <summary>
        /// Gets the standard error of the fit for a particular input point.
        /// </summary>
        /// 
        /// <param name="input">The input vector where the standard error of the fit should be computed.</param>
        /// <param name="inputs">The inputs used to train the model.</param>
        /// <param name="outputs">The outputs used to train the model.</param>
        /// 
        /// <returns>The standard error of the fit at the given input point.</returns>
        /// 
        public double GetStandardError(double input, double[] inputs, double[] outputs)
        {
            double se = GetStandardError(inputs, outputs);
            double a = predictionVariance(input, inputs, outputs);
            return se * Math.Sqrt(a);
        }

        /// <summary>
        /// Gets the standard error of the prediction for a particular input point.
        /// </summary>
        /// 
        /// <param name="input">The input vector where the standard error of the prediction should be computed.</param>
        /// <param name="inputs">The inputs used to train the model.</param>
        /// <param name="outputs">The outputs used to train the model.</param>
        /// 
        /// <returns>The standard error of the prediction given for the input point.</returns>
        /// 
        public double GetPredictionStandardError(double input, double[] inputs, double[] outputs)
        {
            // http://www2.stat.duke.edu/~tjl13/s101/slides/unit6lec3H.pdf
            double se = GetStandardError(inputs, outputs);
            double a = predictionVariance(input, inputs, outputs);
            return se * Math.Sqrt(1 + a);
        }

        /// <summary>
        /// Gets the confidence interval for an input point.
        /// </summary>
        /// 
        /// <param name="input">The input point.</param>
        /// <param name="inputs">The inputs used to train the model.</param>
        /// <param name="outputs">The outputs used to train the model.</param>
        /// <param name="percent">The prediction interval confidence (default is 95%).</param>
        /// 
        public DoubleRange GetConfidenceInterval(double input, double[] inputs, double[] outputs, double percent = 0.95)
        {
            double se = GetStandardError(input, inputs, outputs);
            return createInterval(input, inputs, percent, se);
        }

        /// <summary>
        /// Gets the prediction interval for an input point.
        /// </summary>
        /// 
        /// <param name="input">The input point.</param>
        /// <param name="inputs">The inputs used to train the model.</param>
        /// <param name="outputs">The outputs used to train the model.</param>
        /// <param name="percent">The prediction interval confidence (default is 95%).</param>
        /// 
        public DoubleRange GetPredictionInterval(double input, double[] inputs, double[] outputs, double percent = 0.95)
        {
            double se = GetPredictionStandardError(input, inputs, outputs);
            return createInterval(input, inputs, percent, se);
        }

        private DoubleRange createInterval(double input, double[] inputs, double percent, double se)
        {
            double y = Transform(input);
            double df = GetDegreesOfFreedom(inputs.Length);
            var t = new TTest(estimatedValue: y, standardError: se, degreesOfFreedom: df);
            return t.GetConfidenceInterval(percent);
        }

        private static double predictionVariance(double input, double[] inputs, double[] outputs)
        {
            double n = outputs.Length;
            double meanX = inputs.Mean();

            double sx = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double d = inputs[i] - meanX;
                sx += d * d;
            }

            double z = (input - meanX);
            return 1 / n + (z * z) / sx;
        }
    }
}
