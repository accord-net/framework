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
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Multivariate Linear Regression.
    /// </summary>
    /// <remarks>
    ///   Multivariate Linear Regression is a generalization of
    ///   Multiple Linear Regression to allow for multiple outputs.
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // The multivariate linear regression is a generalization of
    ///   // the multiple linear regression. In the multivariate linear
    ///   // regression, not only the input variables are multivariate,
    ///   // but also are the output dependent variables.
    ///   
    ///   // In the following example, we will perform a regression of
    ///   // a 2-dimensional output variable over a 3-dimensional input
    ///   // variable.
    ///   
    ///   double[][] inputs = 
    ///   {
    ///       // variables:  x1  x2  x3
    ///       new double[] {  1,  1,  1 }, // input sample 1
    ///       new double[] {  2,  1,  1 }, // input sample 2
    ///       new double[] {  3,  1,  1 }, // input sample 3
    ///   };
    ///   
    ///   double[][] outputs = 
    ///   {
    ///       // variables:  y1  y2
    ///       new double[] {  2,  3 }, // corresponding output to sample 1
    ///       new double[] {  4,  6 }, // corresponding output to sample 2
    ///       new double[] {  6,  9 }, // corresponding output to sample 3
    ///   };
    /// 
    ///   // With a quick eye inspection, it is possible to see that
    ///   // the first output variable y1 is always the double of the
    ///   // first input variable. The second output variable y2 is
    ///   // always the triple of the first input variable. The other
    ///   // input variables are unused. Nevertheless, we will fit a
    ///   // multivariate regression model and confirm the validity
    ///   // of our impressions:
    ///   
    ///   // Create a new multivariate linear regression with 3 inputs and 2 outputs
    ///   var regression = new MultivariateLinearRegression(3, 2);
    ///   
    ///   // Now, compute the multivariate linear regression:
    ///   double error = regression.Regress(inputs, outputs);
    ///   
    ///   // At this point, the regression error will be 0 (the fit was
    ///   // perfect). The regression coefficients for the first input
    ///   // and first output variables will be 2. The coefficient for
    ///   // the first input and second output variables will be 3. All
    ///   // others will be 0.
    ///   //
    ///   // regression.Coefficients should be the matrix given by
    ///   //
    ///   // double[,] coefficients = {
    ///   //                              { 2, 3 },
    ///   //                              { 0, 0 },
    ///   //                              { 0, 0 },
    ///   //                          };
    ///   //
    /// 
    ///   // The first input variable coefficients will be 2 and 3:
    ///   Assert.AreEqual(2, regression.Coefficients[0, 0], 1e-10);
    ///   Assert.AreEqual(3, regression.Coefficients[0, 1], 1e-10);
    /// 
    ///   // And all other coefficients will be 0:
    ///   Assert.AreEqual(0, regression.Coefficients[1, 0], 1e-10);
    ///   Assert.AreEqual(0, regression.Coefficients[1, 1], 1e-10);
    ///   Assert.AreEqual(0, regression.Coefficients[2, 0], 1e-10);
    ///   Assert.AreEqual(0, regression.Coefficients[2, 1], 1e-10);
    /// 
    ///   // We can also check the r-squared coefficients of determination:
    ///   double[] r2 = regression.CoefficientOfDetermination(inputs, outputs);
    /// 
    ///   // Which should be one for both output variables:
    ///   Assert.AreEqual(1, r2[0]);
    ///   Assert.AreEqual(1, r2[1]);
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class MultivariateLinearRegression : ILinearRegression
    {

        private double[,] coefficients;
        private double[] intercepts;
        private bool insertConstant;


        /// <summary>
        ///   Creates a new Multivariate Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// <param name="outputs">The number of outputs for the regression.</param>
        /// 
        public MultivariateLinearRegression(int inputs, int outputs)
            : this(inputs, outputs, false)
        {
        }

        /// <summary>
        ///   Creates a new Multivariate Linear Regression.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the regression.</param>
        /// <param name="outputs">The number of outputs for the regression.</param>
        /// <param name="intercept">True to use an intercept term, false otherwise. Default is false.</param>
        /// 
        public MultivariateLinearRegression(int inputs, int outputs, bool intercept)
        {
            this.coefficients = new double[inputs, outputs];
            this.intercepts = new double[outputs];
            this.insertConstant = intercept;
        }

        /// <summary>
        ///   Creates a new Multivariate Linear Regression.
        /// </summary>
        /// 
        public MultivariateLinearRegression(double[,] coefficients, double[] intercepts, bool insertConstant)
        {
            this.coefficients = coefficients;
            this.intercepts = intercepts;
            this.insertConstant = insertConstant;
        }

        /// <summary>
        ///   Gets the coefficient matrix used by the regression model. Each
        ///   column corresponds to the coefficient vector for each of the outputs.
        /// </summary>
        /// 
        public double[,] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        ///   Gets the intercept vector used by the multivariate regression model.
        /// </summary>
        /// 
        public double[] Intercepts
        {
            get { return intercepts; }
        }

        /// <summary>
        ///   Gets the number of inputs in the model.
        /// </summary>
        /// 
        public int Inputs
        {
            get { return coefficients.GetLength(0); }
        }

        /// <summary>
        ///   Gets the number of outputs in the model.
        /// </summary>
        /// 
        public int Outputs
        {
            get { return coefficients.GetLength(1); }
        }

        /// <summary>
        ///   Performs the regression using the input vectors and output
        ///   vectors, returning the sum of squared errors of the fit.
        /// </summary>
        /// 
        /// <param name="inputs">The input vectors to be used in the regression.</param>
        /// <param name="outputs">The output values for each input vector.</param>
        /// <returns>The Sum-Of-Squares error of the regression.</returns>
        /// 
        public virtual double Regress(double[][] inputs, double[][] outputs)
        {
            if (inputs.Length != outputs.Length)
                throw new ArgumentException("Number of input and output samples does not match", "outputs");

            int cols = inputs[0].Length;      // inputs
            int rows = inputs.Length;         // points

            if (insertConstant) cols++;

            for (int c = 0; c < coefficients.GetLength(1); c++)
            {
                double[] B = new double[cols];
                double[,] V = new double[cols, cols];


                // Compute V and B matrices
                for (int i = 0; i < cols; i++)
                {
                    // Least Squares Matrix
                    for (int j = 0; j < cols; j++)
                    {
                        for (int k = 0; k < rows; k++)
                        {
                            if (insertConstant)
                            {
                                double a = (i == cols - 1) ? 1 : inputs[k][i];
                                double b = (j == cols - 1) ? 1 : inputs[k][j];

                                V[i, j] += a * b;
                            }
                            else
                            {
                                V[i, j] += inputs[k][i] * inputs[k][j];
                            }
                        }
                    }

                    // Function to minimize
                    for (int k = 0; k < rows; k++)
                    {
                        if (insertConstant && (i == cols - 1))
                        {
                            B[i] += outputs[k][c];
                        }
                        else
                        {
                            B[i] += inputs[k][i] * outputs[k][c];
                        }
                    }
                }


                // Solve V*C = B to find C (the coefficients)
                double[] coef = new SingularValueDecomposition(V).Solve(B);

                if (insertConstant)
                {
                    intercepts[c] = coef[coef.Length - 1];
                    for (int i = 0; i < cols - 1; i++)
                        coefficients[i, c] = coef[i];
                }
                else
                {
                    for (int i = 0; i < cols; i++)
                        coefficients[i, c] = coef[i];
                }
            }


            // Calculate Sum-Of-Squares error
            double error = 0.0, e;
            for (int i = 0; i < outputs.Length; i++)
            {
                double[] y = Compute(inputs[i]);

                for (int c = 0; c < y.Length; c++)
                {
                    e = outputs[i][c] - y[c];
                    error += e * e;
                }
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
        public double[] CoefficientOfDetermination(double[][] inputs, double[][] outputs)
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
        public double[] CoefficientOfDetermination(double[][] inputs, double[][] outputs, bool adjust)
        {
            // R-squared = 100 * SS(regression) / SS(total)

            int N = inputs.Length;
            int M = coefficients.GetLength(1);
            int P = coefficients.GetLength(0);
            double[] SSe = new double[M];
            double[] SSt = new double[M];
            double[] avg = new double[M];
            double[] r2 = new double[M];
            double d;

            // For each output variable
            for (int c = 0; c < M; c++)
            {
                // Calculate mean
                for (int i = 0; i < N; i++)
                    avg[c] += outputs[i][c];
                avg[c] /= N;
            }

            // Calculate SSe and SSt
            for (int i = 0; i < N; i++)
            {
                double[] y = Compute(inputs[i]);
                for (int c = 0; c < M; c++)
                {
                    d = outputs[i][c] - y[c];
                    SSe[c] += d * d;

                    d = outputs[i][c] - avg[c];
                    SSt[c] += d * d;
                }
            }

            // Calculate R-Squared
            for (int c = 0; c < M; c++)
                r2[c] = 1.0 - (SSe[c] / SSt[c]);

            if (adjust)
            {
                // Return adjusted R-Squared
                for (int c = 0; c < M; c++)
                {
                    if (r2[c] == 1.0)
                        continue;

                    if (N == P + 1)
                    {
                        r2[c] = double.NaN;
                    }
                    else
                    {
                        r2[c] = 1.0 - (1.0 - r2[c]) * ((N - 1.0) / (N - P - 1.0));
                    }
                }
            }

            return r2;
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression output for a given input.
        /// </summary>
        /// 
        /// <param name="input">A input vector.</param>
        /// <returns>The computed output.</returns>
        /// 
        public double[] Compute(double[] input)
        {
            int N = input.Length;
            int M = coefficients.GetLength(1);

            double[] result = new double[M];
            for (int i = 0; i < M; i++)
            {
                result[i] = intercepts[i];

                for (int j = 0; j < N; j++)
                    result[i] += input[j] * coefficients[j, i];
            }

            return result;
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression output for a given input.
        /// </summary>
        /// 
        /// <param name="input">An array of input vectors.</param>
        /// <returns>The computed outputs.</returns>
        /// 
        public double[][] Compute(double[][] input)
        {
            double[][] output = new double[input.Length][];

            for (int j = 0; j < input.Length; j++)
                output[j] = Compute(input[j]);

            return output;
        }


        #region ILinearRegression Members
        /// <summary>
        ///   Computes the model output for a given input.
        /// </summary>
        double[] ILinearRegression.Compute(double[] inputs)
        {
            return this.Compute(inputs);
        }
        #endregion

    }
}
