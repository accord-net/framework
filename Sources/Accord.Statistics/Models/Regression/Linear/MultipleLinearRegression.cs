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
    using System;
    using System.Text;
    using Accord.Math.Decompositions;
    using Accord.Math;
    using Accord.MachineLearning;
    using Fitting;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing;
    using Accord.Compat;

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
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\MultipleLinearRegressionTest.cs" region="doc_learn" />
    /// 
    ///  <para>
    ///   The next example shows how to fit a multiple linear regression model
    ///   in conjunction with a discrete codebook to learn from discrete variables
    ///   using one-hot encodings when applicable:</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\MultipleLinearRegressionTest.cs" region="doc_learn_2" />
    /// 
    ///  <para>
    ///   The next example shows how to fit a multiple linear regression model with the 
    ///   additional constraint that none of its coefficients should be negative. For this
    ///   we can use the <see cref="NonNegativeLeastSquares"/> learning algorithm instead of
    ///   the <see cref="OrdinaryLeastSquares"/> used above.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\NonNegativeLeastSquaresTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="OrdinaryLeastSquares"/>
    /// <seealso cref="NonNegativeLeastSquares"/>
    /// <seealso cref="SimpleLinearRegression"/>
    /// <seealso cref="MultivariateLinearRegression"/>
    /// <seealso cref="MultipleLinearRegressionAnalysis"/>
    /// 
    [Serializable]
#pragma warning disable 612, 618
    public class MultipleLinearRegression : TransformBase<double[], double>,
        ILinearRegression, IFormattable, ICloneable
#pragma warning restore 612, 618
    {
        private double[] coefficients;

        [Obsolete]
        private bool addIntercept;
        private double intercept;


        /// <summary>
        ///   Creates a new Multiple Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// 
        [Obsolete("Please use the default constructor and set NumberOfInputs instead.")]
        public MultipleLinearRegression(int inputs)
            : this(inputs, 0)
        {
        }

        /// <summary>
        ///   Creates a new Multiple Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// <param name="intercept">True to use an intercept term, false otherwise. Default is false.</param>
        /// 
        [Obsolete("Please do not pass a boolean value as the intercept value.")]
        public MultipleLinearRegression(int inputs, bool intercept)
            : this()
        {
            if (intercept)
                inputs++;
            this.coefficients = new double[inputs];
#pragma warning disable 612, 618
            this.addIntercept = intercept;
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Creates a new Multiple Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// <param name="intercept">True to use an intercept term, false otherwise. Default is false.</param>
        /// 
        [Obsolete("Please use the default constructor and set NumberOfInputs instead.")]
        public MultipleLinearRegression(int inputs, double intercept = 0)
            : this()
        {
            this.coefficients = new double[inputs];
            this.intercept = intercept;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleLinearRegression"/> class.
        /// </summary>
        public MultipleLinearRegression()
        {
            NumberOfOutputs = 1;
        }


        /// <summary>
        ///   Gets the coefficients used by the regression model. If the model
        ///   contains an intercept term, it will be in the end of the vector.
        /// </summary>
        /// 
        [Obsolete("Please use Weights instead.")]
        public double[] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        ///   Gets the number of inputs accepted by the model.
        /// </summary>
        /// 
        public override int NumberOfInputs
        {
            get { return base.NumberOfInputs; }
            set
            {
                base.NumberOfInputs = value;
                this.coefficients = Vector.Create(value, coefficients);
            }
        }

        /// <summary>
        ///   Gets or sets the linear weights of the regression model. The
        ///   intercept term is not stored in this vector, but is instead
        ///   available through the <see cref="Intercept"/> property.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return coefficients; }
            set
            {
                coefficients = value;
                NumberOfInputs = value.Length;
            }
        }

        /// <summary>
        ///   Gets the number of parameters in this model (equals the NumberOfInputs + 1).
        /// </summary>
        /// 
        public int NumberOfParameters { get { return NumberOfInputs + 1; } }

        /// <summary>
        ///   Gets the number of inputs for the regression model.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Inputs
        {
#pragma warning disable 612, 618
            get { return coefficients.Length - (addIntercept ? 1 : 0); }
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Gets whether this model has an additional intercept term.
        /// </summary>
        /// 
        [Obsolete("Please check the Intercept value instead.")]
        public bool HasIntercept
        {
#pragma warning disable 612, 618
            get { return addIntercept; }
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Gets or sets the intercept value for the regression.
        /// </summary>
        /// 
        public double Intercept
        {
            get { return intercept; }
            set { intercept = value; }
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
        [Obsolete("Please use the OrdinaryLeastSquares class instead.")]
        public virtual double Regress(double[][] inputs, double[] outputs, bool robust)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[,] design;
#pragma warning disable 612, 618
            return regress(inputs, outputs, out design, robust);
#pragma warning restore 612, 618
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
        [Obsolete("Please use the OrdinaryLeastSquares class instead.")]
        public virtual double Regress(double[][] inputs, double[] outputs)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[,] design;
#pragma warning disable 612, 618
            return regress(inputs, outputs, out design, true);
#pragma warning restore 612, 618
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
        [Obsolete("Please use the OrdinaryLeastSquares class instead.")]
        public double Regress(double[][] inputs, double[] outputs,
            out double[,] informationMatrix, bool robust = true)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            double[,] design;

#pragma warning disable 612, 618
            double error = regress(inputs, outputs, out design, robust);
#pragma warning restore 612, 618

            double[,] cov = design.TransposeAndDot(design);
            informationMatrix = new SingularValueDecomposition(cov,
                computeLeftSingularVectors: true,
                computeRightSingularVectors: true,
                autoTranspose: true, inPlace: true).Inverse();

            return error;
        }

        [Obsolete]
        private double regress(double[][] inputs, double[] outputs, out double[,] designMatrix, bool robust)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            int rows = inputs.Length;    // number of instance points
            int cols = inputs[0].Length; // dimension of each point
            NumberOfInputs = cols;

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
            if (addIntercept)
                intercept = coefficients[coefficients.Length - 1];

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
        public double CoefficientOfDetermination(double[][] inputs, double[] outputs, bool adjust = false, double[] weights = null)
        {
            var rsquared = new RSquaredLoss(NumberOfInputs, outputs);

            rsquared.Adjust = adjust;

            if (weights != null)
                rsquared.Weights = weights;

            return rsquared.Loss(Transform(inputs));
        }

        /// <summary>
        /// Gets the overall regression standard error.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs used to train the model.</param>
        /// <param name="outputs">The outputs used to train the model.</param>
        /// 
        public double GetStandardError(double[][] inputs, double[] outputs)
        {
            double SSe = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double d = outputs[i] - Transform(inputs[i]);
                SSe += d * d;
            }

            return Math.Sqrt(SSe / GetDegreesOfFreedom(inputs.Length));
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
        /// Gets the standard error for each coefficient.
        /// </summary>
        /// 
        /// <param name="mse">The overall regression standard error (can be computed from <see cref="GetStandardError(double[][], double[])"/>.</param>
        /// <param name="informationMatrix">The information matrix obtained when training the model (see <see cref="OrdinaryLeastSquares.GetInformationMatrix()"/>).</param>
        /// 
        public double[] GetStandardErrors(double mse, double[][] informationMatrix)
        {
            double[] se = new double[informationMatrix.Length];
            for (int i = 0; i < se.Length; i++)
                se[i] = mse * Math.Sqrt(informationMatrix[i][i]);
            return se;
        }

        /// <summary>
        /// Gets the standard error of the fit for a particular input vector.
        /// </summary>
        /// 
        /// <param name="input">The input vector where the standard error of the fit should be computed.</param>
        /// <param name="mse">The overall regression standard error (can be computed from <see cref="GetStandardError(double[][], double[])"/>.</param>        
        /// <param name="informationMatrix">The information matrix obtained when training the model (see <see cref="OrdinaryLeastSquares.GetInformationMatrix()"/>).</param>
        /// 
        /// <returns>The standard error of the fit at the given input point.</returns>
        /// 
        public double GetStandardError(double[] input, double mse, double[][] informationMatrix)
        {
            double rim = predictionVariance(input, informationMatrix);
            return mse * Math.Sqrt(rim);
        }

        /// <summary>
        /// Gets the standard error of the prediction for a particular input vector.
        /// </summary>
        /// 
        /// <param name="input">The input vector where the standard error of the prediction should be computed.</param>
        /// <param name="mse">The overall regression standard error (can be computed from <see cref="GetStandardError(double[][], double[])"/>.</param>
        /// <param name="informationMatrix">The information matrix obtained when training the model (see <see cref="OrdinaryLeastSquares.GetInformationMatrix()"/>).</param>
        /// 
        /// <returns>The standard error of the prediction given for the input point.</returns>
        /// 
        public double GetPredictionStandardError(double[] input, double mse, double[][] informationMatrix)
        {
            double rim = predictionVariance(input, informationMatrix);
            return mse * Math.Sqrt(1 + rim);
        }

        /// <summary>
        /// Gets the confidence interval for an input point.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="mse">The overall regression standard error (can be computed from <see cref="GetStandardError(double[][], double[])"/>.</param>
        /// <param name="numberOfSamples">The number of training samples used to fit the model.</param>
        /// <param name="informationMatrix">The information matrix obtained when training the model (see <see cref="OrdinaryLeastSquares.GetInformationMatrix()"/>).</param>
        /// <param name="percent">The prediction interval confidence (default is 95%).</param>
        /// 
        public DoubleRange GetConfidenceInterval(double[] input, double mse, int numberOfSamples, double[][] informationMatrix, double percent = 0.95)
        {
            double se = GetStandardError(input, mse, informationMatrix);
            return computeInterval(input, numberOfSamples, percent, se);
        }

        /// <summary>
        /// Gets the prediction interval for an input point.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="mse">The overall regression standard error (can be computed from <see cref="GetStandardError(double[][], double[])"/>.</param>
        /// <param name="numberOfSamples">The number of training samples used to fit the model.</param>
        /// <param name="informationMatrix">The information matrix obtained when training the model (see <see cref="OrdinaryLeastSquares.GetInformationMatrix()"/>).</param>
        /// <param name="percent">The prediction interval confidence (default is 95%).</param>
        /// 
        public DoubleRange GetPredictionInterval(double[] input, double mse, int numberOfSamples, double[][] informationMatrix, double percent = 0.95)
        {
            double se = GetPredictionStandardError(input, mse, informationMatrix);
            return computeInterval(input, numberOfSamples, percent, se);
        }

        private static double predictionVariance(double[] input, double[][] im)
        {
            if (input.Length < im.Length)
                input = input.Concatenate(1);
            return input.Dot(im).Dot(input);
        }

        private DoubleRange computeInterval(double[] input, int numberOfSamples, double percent, double se)
        {
            double y = Transform(input);
            double df = GetDegreesOfFreedom(numberOfSamples);
            var t = new TTest(estimatedValue: y, standardError: se, degreesOfFreedom: df);
            return t.GetConfidenceInterval(percent);
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression for an input vector.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>The calculated output.</returns>
        /// 
        [Obsolete("Please use Transform instead.")]
        public double Compute(double[] input)
        {
            return Transform(input);
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression for input vectors.
        /// </summary>
        /// 
        /// <param name="input">The input vector data.</param>
        /// 
        /// <returns>The calculated outputs.</returns>
        /// 
        [Obsolete("Please use Transform instead.")]
        public double[] Compute(double[][] input)
        {
            return Transform(input);
        }


        /// <summary>
        ///   Returns a System.String representing the regression.
        /// </summary>
        /// 
        public override string ToString()
        {
            return ToString(null, System.Globalization.CultureInfo.CurrentCulture);
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
        public static MultipleLinearRegression FromData(double[][] x, double[] y)
        {
            return new OrdinaryLeastSquares().Learn(x, y);
        }

        /// <summary>
        ///  Creates a new linear regression from the regression coefficients.
        /// </summary>
        /// 
        /// <param name="coefficients">The linear coefficients.</param>
        /// <param name="intercept">Whether to include an intercept (bias) term.</param>
        /// 
        /// <returns>A linear regression with the given coefficients.</returns>
        /// 
        [Obsolete("Please use the parameterless constructor and set Weights and Intercept directly.")]
        public static MultipleLinearRegression FromCoefficients(double[] coefficients, bool intercept)
        {
            var regression = new MultipleLinearRegression(coefficients.Length, intercept);
            regression.coefficients = coefficients;
            return regression;
        }

#pragma warning disable 612, 618
        [Obsolete("Please use Transform instead.")]
        double[] ILinearRegression.Compute(double[] inputs)
        {
            return new double[] { this.Compute(inputs) };
        }
#pragma warning restore 612, 618


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

            sb.Append("y(");
            for (int i = 0; i < NumberOfInputs; i++)
            {
                sb.AppendFormat("x{0}", i);

                if (i < NumberOfInputs - 1)
                    sb.Append(", ");
            }

            sb.Append(") = ");

            for (int i = 0; i < NumberOfInputs; i++)
            {
                sb.AppendFormat("{0}*x{1}", Weights[i].ToString(format, formatProvider), i);

                if (i < NumberOfInputs - 1)
                    sb.Append(" + ");
            }

            if (Intercept != 0)
                sb.AppendFormat(" + {0}", Intercept.ToString(format, formatProvider));

            return sb.ToString();
        }


        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double Transform(double[] input)
        {
            double output = intercept;
            for (int i = 0; i < input.Length; i++)
                output += coefficients[i] * input[i];
            return output;
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new MultipleLinearRegression()
            {
                Weights = Weights.Copy(),
                Intercept = Intercept
            };
        }

    }
}
