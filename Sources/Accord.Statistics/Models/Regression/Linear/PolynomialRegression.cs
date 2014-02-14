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

namespace Accord.Statistics.Models.Regression.Linear
{
    using System;
    using System.Text;

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
    [Serializable]
    public class PolynomialRegression : ILinearRegression, IFormattable
    {
        private MultipleLinearRegression regression;


        /// <summary>
        ///   Creates a new Polynomial Linear Regression.
        /// </summary>
        /// 
        /// <param name="degree">The degree of the polynomial used by the model.</param>
        /// 
        public PolynomialRegression(int degree)
        {
            // degree plus the independent constant
            regression = new MultipleLinearRegression(degree + 1);
        }

        /// <summary>
        ///   Gets the degree of the polynomial used by the regression.
        /// </summary>
        /// 
        public int Degree
        {
            get { return regression.Coefficients.Length - 1; }
        }

        /// <summary>
        ///   Gets the coefficients of the polynomial regression,
        ///   with the first being the higher-order term and the last
        ///   the intercept term.
        /// </summary>
        /// 
        public double[] Coefficients
        {
            get { return regression.Coefficients; }
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
        public double Regress(double[] inputs, double[] outputs)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            int N = inputs.Length;
            int order = this.Degree + 1;

            // Create polynomials to regress over
            double[][] X = new double[N][];
            for (int i = 0; i < inputs.Length; i++)
            {
                X[i] = new double[order];

                for (int j = 0; j < order; j++)
                    X[i][j] = Math.Pow(inputs[i], order - j - 1);
            }

            return regression.Regress(X, outputs);
        }

        /// <summary>
        ///   Computes the regressed model output for the given inputs.
        /// </summary>
        /// 
        /// <param name="input">The input data.</param>
        /// <returns>The computed outputs.</returns>
        /// 
        public double[] Compute(double[] input)
        {
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = Compute(input[i]);

            return output;
        }

        /// <summary>
        ///   Computes the regressed model output for the given input.
        /// </summary>
        /// 
        /// <param name="input">The input value.</param>
        /// <returns>The computed output.</returns>
        /// 
        public double Compute(double input)
        {
            // Creates the polynomial
            int order = this.Degree + 1;
            double[] polynomial = new double[order];

            for (int j = 0; j < order; j++)
                polynomial[j] = Math.Pow(input, order - j - 1);

            return regression.Compute(polynomial);
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
        /// </remarks>
        /// 
        /// <returns>The R² (r-squared) coefficient for the given data.</returns>
        /// 
        public double CoefficientOfDetermination(double[] inputs, double[] outputs)
        {
            return CoefficientOfDetermination(inputs, outputs, false);
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
        /// </remarks>
        /// 
        /// <returns>The R² (r-squared) coefficient for the given data.</returns>
        /// 
        public double CoefficientOfDetermination(double[] inputs, double[] outputs, bool adjust)
        {
            double[][] X = new double[inputs.Length][];

            for (int i = 0; i < inputs.Length; i++)
            {
                X[i] = new double[regression.Coefficients.Length];

                for (int j = 0; j < X[i].Length; j++)
                {
                    // Create polynomial members
                    X[i][j] = Math.Pow(inputs[i], j);
                }
            }

            return regression.CoefficientOfDetermination(X, outputs, adjust);
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
            for (int i = 0; i < regression.Coefficients.Length; i++)
            {
                int degree = regression.Coefficients.Length - i - 1;
                double coeff = regression.Coefficients[i];

                string coefStr = format == null ? 
                    coeff.ToString(formatProvider) :
                    coeff.ToString(format, formatProvider);

                sb.AppendFormat(formatProvider, "{0}x^{1}", coefStr, degree);

                if (i < regression.Coefficients.Length - 1)
                    sb.Append(" + ");
            }

            return sb.ToString();
        }


        #region ILinearRegression Members
        /// <summary>
        ///   Computes the model output for a given input.
        /// </summary>
        double[] ILinearRegression.Compute(double[] inputs)
        {
            if (inputs.Length > 1)
                throw new ArgumentException("Polynomial regression supports only one-length input vectors", "inputs");
            return new double[] { this.Compute(inputs[0]) };
        }
        #endregion

    }
}
