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
    using Fitting;
    using MachineLearning;
    using Math.Optimization.Losses;
    using System;
    using System.Text;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Polynomial Linear Regression.
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
    /// <seealso cref="PolynomialLeastSquares"/>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\PolynomialRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
#pragma warning disable 612, 618
    public class PolynomialRegression : TransformBase<double, double>, ILinearRegression, IFormattable
#pragma warning restore 612, 618
    {
        private MultipleLinearRegression regression;


        /// <summary>
        ///   Creates a new Polynomial Linear Regression.
        /// </summary>
        /// 
        /// <param name="degree">The degree of the polynomial used by the model.</param>
        /// 
        public PolynomialRegression(int degree)
            : this()
        {
            regression = new MultipleLinearRegression()
            {
                NumberOfInputs = degree
            };
        }

        /// <summary>
        ///   Creates a new Polynomial Linear Regression.
        /// </summary>
        /// 
        public PolynomialRegression(MultipleLinearRegression regression)
            : this()
        {
            this.regression = regression;
        }

        /// <summary>
        ///   Creates a new Polynomial Linear Regression.
        /// </summary>
        /// 
        public PolynomialRegression()
        {
            NumberOfOutputs = 1;
            NumberOfInputs = 1;
        }

        /// <summary>
        ///   Gets the degree of the polynomial used by the regression.
        /// </summary>
        /// 
        public int Degree
        {
            get { return regression.Weights.Length; }
        }

        /// <summary>
        ///   Gets the coefficients of the polynomial regression,
        ///   with the first being the higher-order term and the last
        ///   the intercept term.
        /// </summary>
        /// 
        [Obsolete("Please use Weights instead.")]
        public double[] Coefficients
        {
#pragma warning disable 612, 618
            get { return regression.Weights.Concatenate(Intercept); }
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Gets or sets the linear weights of the regression model. The
        ///   intercept term is not stored in this vector, but is instead
        ///   available through the <see cref="Intercept"/> property.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return regression.Weights; }
            set
            {
                regression.Weights = value;
                NumberOfInputs = value.Length;
            }
        }

        /// <summary>
        ///   Gets or sets the intercept value for the regression.
        /// </summary>
        /// 
        public double Intercept
        {
            get { return regression.Intercept; }
            set { regression.Intercept = value; }
        }

        /// <summary>
        ///   Performs the regression using the input and output
        ///   data, returning the sum of squared errors of the fit.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The output data.</param>
        /// 
        /// <returns>The regression Sum-of-Squares error.</returns>
        /// 
        [Obsolete("Please use the OrdinaryLeastSquares class instead.")]
        public double Regress(double[] inputs, double[] outputs)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            var fit = new PolynomialLeastSquares()
            {
                Degree = Degree,
            }.Learn(inputs, outputs);
            regression.Weights.SetTo(fit.Weights);
            regression.Intercept = fit.Intercept;

            return new SquareLoss(outputs).Loss(Transform(inputs));
        }

        /// <summary>
        ///   Computes the regressed model output for the given inputs.
        /// </summary>
        /// 
        /// <param name="input">The input data.</param>
        /// <returns>The computed outputs.</returns>
        /// 
        [Obsolete("Please use Transform instead.")]
        public double[] Compute(double[] input)
        {
            return Transform(input);
        }

        /// <summary>
        ///   Computes the regressed model output for the given input.
        /// </summary>
        /// 
        /// <param name="input">The input value.</param>
        /// <returns>The computed output.</returns>
        /// 
        [Obsolete("Please use Transform instead.")]
        public double Compute(double input)
        {
            return Transform(input);
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
        public double CoefficientOfDetermination(double[] inputs, double[] outputs, double[] weights = null)
        {
            return CoefficientOfDetermination(inputs, outputs, false, weights);
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
            var polynomial = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
            {
                polynomial[i] = new double[this.Degree];
                for (int j = 0; j < polynomial[i].Length; j++)
                    polynomial[i][j] = Math.Pow(inputs[i], this.Degree - j);
            }

            return regression.CoefficientOfDetermination(polynomial, outputs, adjust, weights);
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
            StringBuilder sb = new StringBuilder();

            sb.Append("y(x) = ");
            for (int i = 0; i < regression.Weights.Length; i++)
            {
                int degree = regression.Weights.Length - i;
                double coeff = regression.Weights[i];

                string coefStr = format == null ?
                    coeff.ToString(formatProvider) :
                    coeff.ToString(format, formatProvider);

                sb.AppendFormat(formatProvider, "{0}x^{1}", coefStr, degree);

                if (i < regression.Weights.Length - 1)
                    sb.Append(" + ");
            }

            string interceptStr = format == null ?
                    Intercept.ToString(formatProvider) :
                    Intercept.ToString(format, formatProvider);

            sb.AppendFormat(formatProvider, " + {0}", interceptStr);

            return sb.ToString();
        }

        /// <summary>
        ///   Creates a new polynomial regression directly from data points.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree to use.</param>
        /// <param name="x">The input vectors <c>x</c>.</param>
        /// <param name="y">The output vectors <c>y</c>.</param>
        /// 
        /// <returns>A polynomial regression f(x) that most approximates y.</returns>
        /// 
        public static PolynomialRegression FromData(int degree, double[] x, double[] y)
        {
            return new PolynomialLeastSquares()
            {
                Degree = degree
            }.Learn(x, y);
        }

#pragma warning disable 612, 618
        [Obsolete("Please use Transform instead.")]
        double[] ILinearRegression.Compute(double[] inputs)
        {
            if (inputs.Length > 1)
                throw new ArgumentException("Polynomial regression supports only one-length input vectors", "inputs");
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
            var polynomial = new double[this.Degree];
            for (int j = 0; j < polynomial.Length; j++)
                polynomial[j] = Math.Pow(input, this.Degree - j);
            return regression.Transform(polynomial);
        }


        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">The location to where to store the result of this transformation.</param>
        /// <returns>The output generated by applying this transformation to the given input.</returns>
        public override double[] Transform(double[] input, double[] result)
        {
            var polynomial = new double[this.Degree];
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < polynomial.Length; j++)
                    polynomial[j] = Math.Pow(input[i], this.Degree - j);

                result[i] = regression.Transform(polynomial);
            }

            return result;
        }
    }
}
