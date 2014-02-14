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
    using Accord.Math.Decompositions;
    using Accord.Math;

    /// <summary>
    ///   Multiple Linear Regression.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In multiple linear regression, the model specification is that the dependent
    ///   variable, denoted y_i, is a linear combination of the parameters (but need not
    ///   be linear in the independent x_i variables). As the linear regression has a
    ///   closed form solution, the regression coefficients can be computed by calling
    ///   the <see cref="Regress(double[][], double[])"/> method only once.</para>
    /// </remarks>
    /// 
    /// <example>
    ///  <para>
    ///   The following example shows how to fit a multiple linear regression model
    ///   to model a plane as an equation in the form ax + by + c = z. </para>
    ///   
    ///   <code>
    ///   // We will try to model a plane as an equation in the form
    ///   // "ax + by + c = z". We have two input variables (x and y)
    ///   // and we will be trying to find two parameters a and b and 
    ///   // an intercept term c.
    ///   
    ///   // Create a multiple linear regression for two input and an intercept
    ///   MultipleLinearRegression target = new MultipleLinearRegression(2, true);
    ///   
    ///   // Now suppose we have some points
    ///   double[][] inputs = 
    ///   {
    ///       new double[] { 1, 1 },
    ///       new double[] { 0, 1 },
    ///       new double[] { 1, 0 },
    ///       new double[] { 0, 0 },
    ///   };
    ///   
    ///   // located in the same Z (z = 1)
    ///   double[] outputs = { 1, 1, 1, 1 };
    ///   
    ///   
    ///   // Now we will try to fit a regression model
    ///   double error = target.Regress(inputs, outputs);
    ///   
    ///   // As result, we will be given the following:
    ///   double a = target.Coefficients[0]; // a = 0
    ///   double b = target.Coefficients[1]; // b = 0
    ///   double c = target.Coefficients[2]; // c = 1
    ///   
    ///   // Now, considering we were trying to find a plane, which could be
    ///   // described by the equation ax + by + c = z, and we have found the
    ///   // aforementioned coefficients, we can conclude the plane we were
    ///   // trying to find is giving by the equation:
    ///   //
    ///   //   ax + by + c = z
    ///   //     -> 0x + 0y + 1 = z
    ///   //     -> 1 = z.
    ///   //
    ///   // The plane containing the aforementioned points is, in fact,
    ///   // the plane given by z = 1.
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class MultipleLinearRegression : ILinearRegression, IFormattable
    {

        private double[] coefficients;
        private bool addIntercept;


        /// <summary>
        ///   Creates a new Multiple Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// 
        public MultipleLinearRegression(int inputs)
            : this(inputs, false) { }

        /// <summary>
        ///   Creates a new Multiple Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// <param name="intercept">True to use an intercept term, false otherwise. Default is false.</param>
        /// 
        public MultipleLinearRegression(int inputs, bool intercept)
        {
            if (intercept) inputs++;
            this.coefficients = new double[inputs];
            this.addIntercept = intercept;
        }


        /// <summary>
        ///   Gets the coefficients used by the regression model. If the model
        ///   contains an intercept term, it will be in the end of the vector.
        /// </summary>
        /// 
        public double[] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        ///   Gets the number of inputs for the regression model.
        /// </summary>
        /// 
        public int Inputs
        {
            get { return coefficients.Length - (addIntercept ? 1 : 0); }
        }

        /// <summary>
        ///   Gets whether this model has an additional intercept term.
        /// </summary>
        /// 
        public bool HasIntercept
        {
            get { return addIntercept; }
        }

        /// <summary>
        ///   Performs the regression using the input vectors and output
        ///   data, returning the sum of squared errors of the fit.
        /// </summary>
        /// 
        /// <param name="inputs">The input vectors to be used in the regression.</param>
        /// <param name="outputs">The output values for each input vector.</param>
        /// <param name="robust">
        ///    Set to <c>true</c> to force the use of the <see cref="SingularValueDecomposition"/>.
        ///    This will avoid any rank exceptions, but might be more computing intensive.</param>
        ///    
        /// <returns>The Sum-Of-Squares error of the regression.</returns>
        /// 
        public virtual double Regress(double[][] inputs, double[] outputs, bool robust)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[,] design;
            return regress(inputs, outputs, out design, true);
        }

        /// <summary>
        ///   Performs the regression using the input vectors and output
        ///   data, returning the sum of squared errors of the fit.
        /// </summary>
        /// 
        /// <param name="inputs">The input vectors to be used in the regression.</param>
        /// <param name="outputs">The output values for each input vector.</param>
        /// <returns>The Sum-Of-Squares error of the regression.</returns>
        /// 
        public virtual double Regress(double[][] inputs, double[] outputs)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[,] design;
            return regress(inputs, outputs, out design, true);
        }

        /// <summary>
        ///   Performs the regression using the input vectors and output
        ///   data, returning the sum of squared errors of the fit.
        /// </summary>
        /// 
        /// <param name="inputs">The input vectors to be used in the regression.</param>
        /// <param name="outputs">The output values for each input vector.</param>
        /// <param name="informationMatrix">Gets the Fisher's information matrix.</param>
        /// <param name="robust">
        ///    Set to <c>true</c> to force the use of the <see cref="SingularValueDecomposition"/>.
        ///    This will avoid any rank exceptions, but might be more computing intensive.</param>
        /// 
        /// <returns>The Sum-Of-Squares error of the regression.</returns>
        /// 
        public double Regress(double[][] inputs, double[] outputs, 
            out double[,] informationMatrix, bool robust = true)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[,] design;

            double error = regress(inputs, outputs, out design, robust);

            double[,] cov = design.TransposeAndMultiply(design);
            informationMatrix = new SingularValueDecomposition(cov,
                computeLeftSingularVectors: true,
                computeRightSingularVectors: true,
                autoTranspose: true, inPlace: true).Inverse();

            return error;
        }


        private double regress(double[][] inputs, double[] outputs, out double[,] designMatrix, bool robust)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            int parameters = Inputs;
            int rows = inputs.Length;   // number of instance points
            int cols = Inputs;          // dimension of each point

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].Length != parameters)
                {
                    throw new DimensionMismatchException("inputs", String.Format(
                        "Input vectors should have length {0}. The row at index {1} of the" +
                        " inputs matrix has length {2}.", parameters, i, inputs[i].Length));
                }
            }

            ISolverMatrixDecomposition<double> solver;


            // Create the problem's design matrix. If we
            //  have to add an intercept term, add a new
            //  extra column at the end and fill with 1s.

            if (!addIntercept)
            {
                // Just copy values over
                designMatrix = new double[rows, cols];
                for (int i = 0; i < inputs.Length; i++)
                    for (int j = 0; j < inputs[i].Length; j++)
                        designMatrix[i, j] = inputs[i][j];
            }
            else
            {
                // Add an intercept term
                designMatrix = new double[rows, cols + 1];
                for (int i = 0; i < inputs.Length; i++)
                {
                    for (int j = 0; j < inputs[i].Length; j++)
                        designMatrix[i, j] = inputs[i][j];
                    designMatrix[i, cols] = 1;
                }
            }

            // Check if we have an overdetermined or underdetermined
            //  system to select an appropriate matrix solver method.

            if (robust || cols >= rows)
            {
                // We have more variables than equations, an
                // underdetermined system. Solve using a SVD:
                solver = new SingularValueDecomposition(designMatrix,
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true);
            }
            else
            {
                // We have more equations than variables, an
                // overdetermined system. Solve using the QR:
                solver = new QrDecomposition(designMatrix);
            }


            // Solve V*C = B to find C (the coefficients)
            coefficients = solver.Solve(outputs);


            // Calculate Sum-Of-Squares error
            double error = 0.0;
            double e;
            for (int i = 0; i < outputs.Length; i++)
            {
                e = outputs[i] - Compute(inputs[i]);
                error += e * e;
            }

            return error;
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
        public double CoefficientOfDetermination(double[][] inputs, double[] outputs)
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
        public double CoefficientOfDetermination(double[][] inputs, double[] outputs, bool adjust)
        {
            // R-squared = 100 * SS(regression) / SS(total)

            int n = inputs.Length;
            int p = Inputs;
            double SSe = 0.0;
            double SSt = 0.0;
            double avg = 0.0;
            double d;

            // Calculate output mean
            for (int i = 0; i < outputs.Length; i++)
                avg += outputs[i];
            avg /= inputs.Length;

            // Calculate SSe and SSt
            for (int i = 0; i < outputs.Length; i++)
            {
                d = outputs[i] - Compute(inputs[i]);
                SSe += d * d;

                d = outputs[i] - avg;
                SSt += d * d;
            }

            // Calculate R-Squared
            double r2 = (SSt != 0) ? 1.0 - (SSe / SSt) : 1.0;

            if (!adjust)
            {
                // Return ordinary R-Squared
                return r2;
            }
            else
            {
                if (r2 == 1)
                    return 1;

                if (n - p == 1.0)
                {
                    return double.NaN;
                }
                else
                {
                    // Return adjusted R-Squared
                    return 1.0 - (1.0 - r2) * ((n - 1.0) / (n - p - 1.0));
                }
            }
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression for an input vector.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>The calculated output.</returns>
        /// 
        public double Compute(double[] input)
        {
            if (input.Length != Inputs)
                throw new DimensionMismatchException("input",
                    String.Format("Input vectors should have length {0}.", Inputs));

            double output = 0.0;

            for (int i = 0; i < input.Length; i++)
                output += coefficients[i] * input[i];

            if (addIntercept)
                output += coefficients[input.Length];

            return output;
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression for input vectors.
        /// </summary>
        /// 
        /// <param name="input">The input vector data.</param>
        /// 
        /// <returns>The calculated outputs.</returns>
        /// 
        public double[] Compute(double[][] input)
        {
            double[] output = new double[input.Length];

            for (int j = 0; j < input.Length; j++)
            {
                output[j] = Compute(input[j]);
            }

            return output;
        }


        /// <summary>
        ///   Returns a System.String representing the regression.
        /// </summary>
        /// 
        public override string ToString()
        {
            return ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }

        #region ILinearRegression Members
        double[] ILinearRegression.Compute(double[] inputs)
        {
            return new double[] { this.Compute(inputs) };
        }
        #endregion

        #region IFormattable Members

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use
        ///     the default format defined for the type of the System.IFormattable implementation. </param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in
        ///     Visual Basic) to obtain the numeric format information from the current locale
        ///     setting of the operating system.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();

            int inputs = (addIntercept) ? coefficients.Length - 1 : coefficients.Length;

            sb.Append("y(");
            for (int i = 0; i < inputs; i++)
            {
                sb.AppendFormat("x{0}", i);

                if (i < inputs - 1)
                    sb.Append(", ");
            }

            sb.Append(") = ");

            for (int i = 0; i < inputs; i++)
            {
                sb.AppendFormat("{0}*x{1}", Coefficients[i].ToString(format, formatProvider), i);

                if (i < inputs - 1)
                    sb.Append(" + ");
            }

            if (addIntercept)
                sb.AppendFormat(" + {0}", coefficients[inputs].ToString(format, formatProvider));

            return sb.ToString();
        }

        #endregion
    }
}
