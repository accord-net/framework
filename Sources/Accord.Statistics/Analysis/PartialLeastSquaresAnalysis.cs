﻿// Accord Statistics Library
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

namespace Accord.Statistics.Analysis
{
    using System;
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.MachineLearning;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   The PLS algorithm to use in the Partial Least Squares Analysis.
    /// </summary>
    /// 
    public enum PartialLeastSquaresAlgorithm
    {
        /// <summary>
        ///   Sijmen de Jong's SIMPLS algorithm.
        /// </summary>
        /// <remarks>
        ///   The SIMPLS algorithm is considerably faster than NIPALS, especially when the number of
        ///   input variables increases; but gives slightly different results in the case of multiple
        ///   outputs.
        /// </remarks>
        /// 
        SIMPLS,

        /// <summary>
        ///   Traditional NIPALS algorithm.
        /// </summary>
        /// 
        NIPALS
    }

    /// <summary>
    ///   Partial Least Squares Regression/Analysis (a.k.a Projection To Latent Structures)
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Partial least squares regression (PLS-regression) is a statistical method that bears
    ///   some relation to principal components regression; instead of finding hyperplanes of 
    ///   maximum variance between the response and independent variables, it finds a linear 
    ///   regression model by projecting the predicted variables and the observable variables 
    ///   to a new space. Because both the X and Y data are projected to new spaces, the PLS 
    ///   family of methods are known as bilinear factor models.</para>
    ///
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Abdi, H. (2010). Partial least square regression, projection on latent structure regression,
    ///       PLS-Regression. Wiley Interdisciplinary Reviews: Computational Statistics, 2, 97-106. 
    ///       Available in: http://www.utdallas.edu/~herve/abdi-wireCS-PLS2010.pdf </description></item>
    ///     <item><description>
    ///       Abdi, H. (2007). Partial least square regression (PLS regression). In N.J. Salkind (Ed.):
    ///       Encyclopedia of Measurement and Statistics. Thousand Oaks (CA): Sage. pp. 740-744.
    ///       Resource available online in: http://www.utdallas.edu/~herve/Abdi-PLS-pretty.pdf </description></item>
    ///     <item><description>
    ///       Martin Anderson, "A comparison of nine PLS1 algorithms". Available on:
    ///       http://onlinelibrary.wiley.com/doi/10.1002/cem.1248/pdf </description></item>
    ///     <item><description>
    ///       Mevik, B-H. Wehrens, R. (2007). The pls Package: Principal Component and Partial Least Squares
    ///       Regression in R. Journal of Statistical Software, Volume 18, Issue 2.
    ///       Resource available online in: http://www.jstatsoft.org/v18/i02/paper </description></item>
    ///     <item><description>
    ///       Garson, D. Partial Least Squares Regression (PLS).
    ///       http://faculty.chass.ncsu.edu/garson/PA765/pls.htm </description></item>
    ///     <item><description>
    ///       De Jong, S. (1993). SIMPLS: an alternative approach to partial least squares regression.
    ///       Chemometrics and Intelligent Laboratory Systems, 18: 251–263.
    ///       http://dx.doi.org/10.1016/0169-7439(93)85002-X </description></item>
    ///     <item><description>
    ///       Rosipal, Roman and Nicole Kramer. (2006). Overview and Recent Advances in Partial Least
    ///       Squares, in Subspace, Latent Structure and Feature Selection Techniques, pp 34–51.
    ///       http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.85.7735 </description></item>
    ///     <item><description>
    ///       Yi Cao. (2008). Partial Least-Squares and Discriminant Analysis: A tutorial and tool
    ///       using PLS for discriminant analysis.</description></item>
    ///     <item><description>
    ///       Wikipedia contributors. Partial least squares regression. Wikipedia, The Free Encyclopedia;
    ///       2009. Available from: http://en.wikipedia.org/wiki/Partial_least_squares_regression. </description></item>
    ///   </list></para>   
    /// </remarks>
    ///
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Analysis\PartialLeastSquaresAnalysisTest.cs" region="doc_learn" />
    /// </example>
    ///
    [Serializable]
#pragma warning disable 612, 618
    public class PartialLeastSquaresAnalysis : MultipleTransformBase<double[], double>, IMultivariateRegressionAnalysis, IProjectionAnalysis,
        ISupervisedLearning<MultivariateLinearRegression, double[], double[]>
#pragma warning restore 612, 618
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        [Obsolete]
        internal double[,] sourceX;
        [Obsolete]
        internal double[,] sourceY;

        internal double[] meanX;
        internal double[] meanY;
        internal double[] stdDevX;
        internal double[] stdDevY;

        internal double[][] loadingsX;
        internal double[][] loadingsY;
        internal double[][] scoresX;
        internal double[][] scoresY;
        private double[][] weights;
        private double[][] coeffbase;

        private double[][] vip;

        internal double[] componentProportionX;
        internal double[] componentProportionY;
        internal double[] cumulativeProportionX;
        internal double[] cumulativeProportionY;

        private AnalysisMethod analysisMethod;
        private PartialLeastSquaresAlgorithm algorithm;
        private PartialLeastSquaresFactorCollection factorCollection;

        private PartialLeastSquaresVariables inputVariables;
        private PartialLeastSquaresVariables outputVariables;

        private bool overwriteSourceMatrix;
        private int numberOfFactors;


        /// <summary>
        ///   Constructs a new Partial Least Squares Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input source data to perform analysis.</param>
        /// <param name="outputs">The output source data to perform analysis.</param>
        /// 
        [Obsolete("Please pass the 'inputs' and 'outputs' matrices to the Learn method instead.")]
        public PartialLeastSquaresAnalysis(double[,] inputs, double[,] outputs)
            : this(inputs, outputs, AnalysisMethod.Center, PartialLeastSquaresAlgorithm.NIPALS) { }

        /// <summary>
        ///   Constructs a new Partial Least Squares Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input source data to perform analysis.</param>
        /// <param name="outputs">The output source data to perform analysis.</param>
        /// <param name="algorithm">The PLS algorithm to use in the analysis. Default is <see cref="PartialLeastSquaresAlgorithm.NIPALS"/>.</param>
        /// 
        [Obsolete("Please pass the 'inputs' and 'outputs' matrices to the Learn method instead.")]
        public PartialLeastSquaresAnalysis(double[,] inputs, double[,] outputs, PartialLeastSquaresAlgorithm algorithm)
            : this(inputs, outputs, AnalysisMethod.Center, algorithm) { }

        /// <summary>
        ///   Constructs a new Partial Least Squares Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input source data to perform analysis.</param>
        /// <param name="outputs">The output source data to perform analysis.</param>
        /// <param name="method">The analysis method to perform. Default is <see cref="AnalysisMethod.Center"/>.</param>
        /// <param name="algorithm">The PLS algorithm to use in the analysis. Default is <see cref="PartialLeastSquaresAlgorithm.NIPALS"/>.</param>
        /// 
        [Obsolete("Please pass the 'inputs' and 'outputs' matrices to the Learn method instead.")]
        public PartialLeastSquaresAnalysis(double[,] inputs, double[,] outputs, AnalysisMethod method, PartialLeastSquaresAlgorithm algorithm)
        {
            // Initial argument checking
            if (inputs == null)
                throw new ArgumentNullException("inputs");
            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.GetLength(0) != outputs.GetLength(0))
                throw new ArgumentException("The number of rows in the inputs array must match the number of rows in the outputs array.");


            this.analysisMethod = method;
            this.algorithm = algorithm;

#pragma warning disable 612, 618
            this.sourceX = inputs;
            this.sourceY = outputs;
#pragma warning restore 612, 618

            // Calculate common measures to speedup other calculations
            this.meanX = Measures.Mean(inputs, dimension: 0);
            this.meanY = Measures.Mean(outputs, dimension: 0);
            this.stdDevX = Measures.StandardDeviation(inputs, meanX);
            this.stdDevY = Measures.StandardDeviation(outputs, meanY);

            base.NumberOfInputs = sourceX.Columns();
            base.NumberOfOutputs = NumberOfInputs;

            this.inputVariables = new PartialLeastSquaresVariables(this, true);
            this.outputVariables = new PartialLeastSquaresVariables(this, false);
        }

        /// <summary>
        ///   Constructs a new Partial Least Squares Analysis.
        /// </summary>
        /// 
        /// <param name="method">The analysis method to perform. Default is <see cref="AnalysisMethod.Center"/>.</param>
        /// <param name="algorithm">The PLS algorithm to use in the analysis. Default is <see cref="PartialLeastSquaresAlgorithm.NIPALS"/>.</param>
        /// 
        public PartialLeastSquaresAnalysis(AnalysisMethod method = AnalysisMethod.Center,
            PartialLeastSquaresAlgorithm algorithm = PartialLeastSquaresAlgorithm.NIPALS)
        {
            this.analysisMethod = method;
            this.algorithm = algorithm;
        }



#pragma warning disable 612, 618
        /// <summary>
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Source
        {
            get { return sourceX; }
        }

        /// <summary>
        ///   Gets the dependent variables' values
        ///   for each of the source input points.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Output
        {
            get { return sourceY; }
        }
#pragma warning restore 612, 618

        /// <summary>
        ///   Gets information about independent (input) variables.
        /// </summary>
        /// 
        public PartialLeastSquaresVariables Predictors
        {
            get { return inputVariables; }
        }

        /// <summary>
        ///   Gets information about dependent (output) variables.
        /// </summary>
        /// 
        public PartialLeastSquaresVariables Dependents
        {
            get { return outputVariables; }
        }

        /// <summary>
        ///   Gets the Weight matrix obtained during the analysis. For the NIPALS algorithm
        ///   this is the W matrix. For the SIMPLS algorithm this is the R matrix.
        /// </summary>
        /// 
        public double[][] Weights
        {
            get { return weights; }
        }

        /// <summary>
        ///   Gets information about the factors discovered during the analysis in a
        ///   object-oriented structure which can be data-bound directly to many controls.
        /// </summary>
        /// 
        public PartialLeastSquaresFactorCollection Factors
        {
            get { return factorCollection; }
        }

        /// <summary>
        ///   Gets or sets the PLS algorithm used by the analysis.
        /// </summary>
        /// 
        public PartialLeastSquaresAlgorithm Algorithm
        {
            get { return algorithm; }
            set { algorithm = value; }
        }

        /// <summary>
        ///   Gets or sets the method used by this analysis.
        /// </summary>
        /// 
        public AnalysisMethod Method
        {
            get { return this.analysisMethod; }
            set { this.analysisMethod = value; }
        }

        /// <summary>
        ///   Gets the Variable Importance in Projection (VIP).
        /// </summary>
        /// <remarks>
        ///   This method has been implemented considering only PLS
        ///   models fitted using the NIPALS algorithm containing a
        ///   single response (output) variable.
        /// </remarks>
        /// 
        public double[][] Importance
        {
            get { return vip; }
        }

        /// <summary>
        ///   Gets or sets whether calculations will be performed overwriting
        ///   data in the original source matrix, using less memory.
        /// </summary>
        /// 
        public bool Overwrite
        {
            get { return overwriteSourceMatrix; }
            set { overwriteSourceMatrix = value; }
        }

        /// <summary>
        ///   Gets or sets the number of latent factors that can be considered in this model.
        /// </summary>
        /// 
        public int NumberOfLatentFactors
        {
            get { return this.numberOfFactors; }
            set
            {
                if (value < 0 || (MaximumNumberOfFactors > 0 && value > MaximumNumberOfFactors))
                    throw new ArgumentOutOfRangeException("value", "Number of latent factors should be less than or equal the number of columns in the input data {0}.".Format(MaximumNumberOfFactors));
                this.numberOfFactors = value;
            }
        }

        /// <summary>
        /// Gets the number of inputs accepted by the model.
        /// </summary>
        /// <value>The number of inputs.</value>
        /// <exception cref="InvalidOperationException">This property is read-only.</exception>
        public override int NumberOfInputs
        {
            get { return base.NumberOfInputs; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }

        /// <summary>
        /// Gets the number of outputs generated by the model.
        /// </summary>
        /// <value>The number of outputs.</value>
        /// <exception cref="InvalidOperationException">This property is read-only.</exception>
        public override int NumberOfOutputs
        {
            get { return base.NumberOfOutputs; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }

        /// <summary>
        ///   Gets the maximum number of latent factors that can be considered in this model.
        /// </summary>
        /// 
        public int MaximumNumberOfFactors { get; private set; }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public MultivariateLinearRegression Learn(double[][] x, double[][] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            // maxFactors = min(rows-1, cols)
            MaximumNumberOfFactors = System.Math.Min(x.Rows() - 1, x.Columns());

            this.inputVariables = new PartialLeastSquaresVariables(this, true);
            this.outputVariables = new PartialLeastSquaresVariables(this, false);

            base.NumberOfInputs = x.Columns();
            base.NumberOfOutputs = y.Columns();
            if (this.NumberOfLatentFactors == 0 || this.NumberOfLatentFactors > MaximumNumberOfFactors)
                this.NumberOfLatentFactors = MaximumNumberOfFactors;

            meanX = x.Mean(dimension: 0);
            meanY = y.Mean(dimension: 0);
            stdDevX = x.StandardDeviation(meanX);
            stdDevY = y.StandardDeviation(meanY);

            // Initialize and prepare the data
            var inputs = Adjust(x, meanX, stdDevX, Overwrite);
            var outputs = Adjust(y, meanY, null, Overwrite);


            // Run selected algorithm
            if (algorithm == PartialLeastSquaresAlgorithm.SIMPLS)
            {
                simpls(inputs, outputs, MaximumNumberOfFactors);
            }
            else
            {
                nipals(inputs, outputs, MaximumNumberOfFactors, 0);
            }


            // Calculate cumulative proportions
            this.cumulativeProportionX = new double[MaximumNumberOfFactors];
            this.cumulativeProportionY = new double[MaximumNumberOfFactors];
            this.cumulativeProportionX[0] = this.componentProportionX[0];
            this.cumulativeProportionY[0] = this.componentProportionY[0];
            for (int i = 1; i < MaximumNumberOfFactors; i++)
            {
                this.cumulativeProportionX[i] = this.cumulativeProportionX[i - 1] + this.componentProportionX[i];
                this.cumulativeProportionY[i] = this.cumulativeProportionY[i - 1] + this.componentProportionY[i];
            }


            // Compute Variable Importance in Projection (VIP)
            this.vip = ComputeVariableImportanceInProjection(MaximumNumberOfFactors);


            // Create the object-oriented structure to hold the partial least squares factors
            var array = new PartialLeastSquaresFactor[MaximumNumberOfFactors];
            for (int i = 0; i < array.Length; i++)
                array[i] = new PartialLeastSquaresFactor(this, i);
            this.factorCollection = new PartialLeastSquaresFactorCollection(array);

            return CreateRegression();
        }

        /// <summary>
        ///   Computes the Partial Least Squares Analysis.
        /// </summary>
        /// 
        [Obsolete("Please use the Learn method instead.")]
        public void Compute()
        {
            // maxFactors = min(rows-1,cols)
            MaximumNumberOfFactors = System.Math.Min(sourceX.GetLength(0) - 1, sourceX.GetLength(1));

            Compute(MaximumNumberOfFactors);
        }

        /// <summary>
        ///   Computes the Partial Least Squares Analysis.
        /// </summary>
        /// <param name="factors">
        ///   The number of factors to compute. The number of factors
        ///   should be a value between 1 and min(rows-1,cols) where
        ///   rows and columns are the number of observations and
        ///   variables in the input source data matrix. </param>
        ///   
        [Obsolete("Please set the NumberOfOutputs property and use the Learn method instead.")]
        public void Compute(int factors)
        {
#pragma warning disable 612, 618
            // maxFactors = min(rows-1,cols)
            MaximumNumberOfFactors = System.Math.Min(sourceX.GetLength(0) - 1, sourceX.GetLength(1));

            if (factors > MaximumNumberOfFactors)
                throw new ArgumentOutOfRangeException("factors");

            // Initialize and prepare the data
            double[,] inputs = Adjust(sourceX, meanX, stdDevX, Overwrite);
            double[,] outputs = Adjust(sourceY, meanY, null, Overwrite);

            this.NumberOfLatentFactors = factors;
            base.NumberOfInputs = sourceX.Columns();
            base.NumberOfOutputs = sourceY.Columns();

            // Run selected algorithm
            if (algorithm == PartialLeastSquaresAlgorithm.SIMPLS)
            {
                simpls(inputs.ToJagged(), outputs.ToJagged(), factors);
            }
            else
            {
                nipals(inputs.ToJagged(), outputs.ToJagged(), factors, 0);
            }


            // Calculate cumulative proportions
            this.cumulativeProportionX = new double[factors];
            this.cumulativeProportionY = new double[factors];
            this.cumulativeProportionX[0] = this.componentProportionX[0];
            this.cumulativeProportionY[0] = this.componentProportionY[0];
            for (int i = 1; i < factors; i++)
            {
                this.cumulativeProportionX[i] = this.cumulativeProportionX[i - 1] + this.componentProportionX[i];
                this.cumulativeProportionY[i] = this.cumulativeProportionY[i - 1] + this.componentProportionY[i];
            }


            // Compute Variable Importance in Projection (VIP)
            this.vip = ComputeVariableImportanceInProjection(factors);

            // Create the object-oriented structure to hold the partial least squares factors
            var array = new PartialLeastSquaresFactor[factors];
            for (int i = 0; i < array.Length; i++)
                array[i] = new PartialLeastSquaresFactor(this, i);
            this.factorCollection = new PartialLeastSquaresFactorCollection(array);
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Projects a given set of inputs into latent space.
        /// </summary>
        /// 
        [Obsolete("Please use jagged matrices instead.")]
        public double[,] Transform(double[,] data)
        {
            return Transform(data, loadingsX.GetLength(1));
        }

        /// <summary>
        ///   Projects a given set of inputs into latent space.
        /// </summary>
        /// 
        [Obsolete("Please use jagged matrices instead.")]
        public double[,] Transform(double[,] data, int dimensions)
        {
            if (data == null) throw new ArgumentNullException("data");

            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            if (cols > loadingsX.GetLength(0))
            {
                throw new DimensionMismatchException("data",
                    "The data matrix should have a number of columns less than or equal to"
                    + " the number of rows in the loadings matrix for the input variables.");
            }

            double[,] result = new double[rows, dimensions];
            double[,] source = Adjust(data, meanX, stdDevX, false);

            // multiply the data matrix by the selected factors
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < dimensions; j++)
                    for (int k = 0; k < cols; k++)
                        result[i, j] += source[i, k] * loadingsX[k][j];

            return result;
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[][] Transform(double[][] input, double[][] result)
        {
            double[][] source = Adjust(input, meanX, stdDevX, false);

            // multiply the data matrix by the selected factors
            for (int i = 0; i < source.Length; i++)
                for (int j = 0; j < NumberOfOutputs; j++)
                    for (int k = 0; k < source[i].Length; k++)
                        result[i][j] += source[i][k] * loadingsX[k][j];

            return result;
        }



        /// <summary>
        ///   Projects a given set of outputs into latent space.
        /// </summary>
        /// 
        public double[][] TransformOutput(double[][] outputs)
        {
            return TransformOutput(outputs, loadingsY.Columns());
        }

        /// <summary>
        ///   Projects a given set of outputs into latent space.
        /// </summary>
        /// 
        [Obsolete("Please use jagged matrices instead.")]
        public double[,] TransformOutput(double[,] outputs)
        {
            return TransformOutput(outputs.ToJagged(), loadingsY.Columns()).ToMatrix();
        }

        /// <summary>
        ///   Projects a given set of outputs into latent space.
        /// </summary>
        /// 
        [Obsolete("Please use jagged matrices instead.")]
        public double[,] TransformOutput(double[,] outputs, int dimensions)
        {
            return TransformOutput(outputs.ToJagged(), dimensions).ToMatrix();
        }

        /// <summary>
        ///   Projects a given set of outputs into latent space.
        /// </summary>
        /// 
        public double[][] TransformOutput(double[][] outputs, int dimensions)
        {
            int rows = outputs.Rows();
            int cols = outputs.Columns();

            if (cols > loadingsY.Rows())
            {
                throw new DimensionMismatchException("outputs",
                    "The data matrix should have a number of columns less than or equal to"
                    + " the number of rows in the loadings matrix for the input variables.");
            }

            double[][] result = Jagged.Zeros(rows, dimensions);
            double[][] source = Adjust(outputs, meanY, stdDevY, false);

            // multiply the data matrix by the selected factors
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < dimensions; j++)
                    for (int k = 0; k < cols; k++)
                        result[i][j] += source[i][k] * loadingsY[k][j];

            return result;
        }

        /// <summary>
        ///   Creates a Multivariate Linear Regression model using
        ///   coefficients obtained by the Partial Least Squares.
        /// </summary>
        /// 
        public MultivariateLinearRegression CreateRegression()
        {
            return CreateRegression(NumberOfLatentFactors);
        }

        /// <summary>
        ///   Creates a Multivariate Linear Regression model using
        ///   coefficients obtained by the Partial Least Squares.
        /// </summary>
        /// 
        public MultivariateLinearRegression CreateRegression(int factors)
        {
            if (factors > factorCollection.Count)
                throw new ArgumentOutOfRangeException("factors",
                    "The number of factors should be equal to or less than the number of factors computed in the analysis.");

            //  Compute regression coefficients B of Y on X as B = RQ'
            double[][] B = Jagged.Zeros(coeffbase.Length, loadingsY.Length);
            for (int i = 0; i < coeffbase.Length; i++)
                for (int j = 0; j < loadingsY.Length; j++)
                    for (int k = 0; k < factors; k++)
                        B[i][j] += coeffbase[i][k] * loadingsY[j][k];

            // Divide by standard deviation if X has been normalized
            if (analysisMethod == AnalysisMethod.Standardize)
            {
                for (int i = 0; i < B.Length; i++)
                    for (int j = 0; j < B[i].Length; j++)
                        B[i][j] = B[i][j] / stdDevX[i];
            }

            // Compute regression intercepts A as A = meanY - meanX'*B
            double[] A = new double[loadingsY.Length];
            for (int i = 0; i < meanY.Length; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < B.Length; j++)
                    sum += meanX[j] * B[j][i];
                A[i] = meanY[i] - sum;
            }

            return new MultivariateLinearRegression()
            {
                Weights = B,
                Intercepts = A
            };
        }



        #region Partial Least Squares Algorithms
        /// <summary>
        ///   Computes PLS parameters using NIPALS algorithm.
        /// </summary>
        /// 
        /// <param name="factors">The number of factors to compute.</param>
        /// <param name="inputsX">The mean-centered input values X.</param>
        /// <param name="outputsY">The mean-centered output values Y.</param>
        /// <param name="tolerance">The tolerance for convergence.</param>
        /// 
        /// <remarks>
        /// <para>
        ///   The algorithm implementation follows the original paper by Hervé
        ///   Abdi, with overall structure as suggested in Yi Cao's tutorial.</para>
        ///   
        /// <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       Abdi, H. (2010). Partial least square regression, projection on latent structure regression,
        ///       PLS-Regression. Wiley Interdisciplinary Reviews: Computational Statistics, 2, 97-106. 
        ///       Available in: http://www.utdallas.edu/~herve/abdi-wireCS-PLS2010.pdf </description></item>
        ///     <item><description>
        ///       Yi Cao. (2008). Partial Least-Squares and Discriminant Analysis: A tutorial and tool
        ///       using PLS for discriminant analysis.</description></item>
        ///    </list></para>
        /// </remarks>
        /// 
        private void nipals(double[][] inputsX, double[][] outputsY, int factors, double tolerance)
        {
            // Initialize and prepare the data
            int rows = inputsX.Rows();
            int xcols = inputsX.Columns();
            int ycols = outputsY.Columns();

            // Initialize storage variables
            var E = inputsX.Copy();
            var F = outputsY.Copy();

            var T = Jagged.Zeros(rows, factors);  // factor score matrix T
            var U = Jagged.Zeros(rows, factors);  // factor score matrix U
            var P = Jagged.Zeros(xcols, factors); // loading matrix P, the loadings for X such that X = TP + F
            var C = Jagged.Zeros(ycols, factors); // loading matrix C, the loadings for Y such that Y = TC + E
            var W = Jagged.Zeros(xcols, xcols);   // weight matrix W
            double[] B = new double[factors];

            double[] varX = new double[factors];
            double[] varY = new double[factors];


            // Initialize the algorithm
            bool stop = false;

            #region NIPALS
            for (int factor = 0; factor < factors && !stop; factor++)
            {
                // Select t as the largest column from X,
                double[] t = E.GetColumn(largest(E));

                // Select u as the largest column from Y.
                double[] u = F.GetColumn(largest(F));

                // Will store weights for X and Y
                double[] w = new double[xcols];
                double[] c = new double[ycols];

                double norm_t = Norm.Euclidean(t);

                while (norm_t > 1e-14)
                {
                    #region Iteration

                    // Store initial t to check convergence
                    double[] t0 = (double[])t.Clone();


                    // Step 1. Estimate w (X weights): w ∝ E'*u
                    //   (in Abdi's paper, E is the residuals for X).

                    // 1.1. Compute w = E'*u;
                    w = new double[xcols];
                    for (int j = 0; j < w.Length; j++)
                        for (int i = 0; i < u.Length; i++)
                            w[j] += E[i][j] * u[i];

                    // 1.2. Normalize w (w = w/norm(w))
                    w = w.Divide(Norm.Euclidean(w));


                    // Step 2. Estimate t (X factor scores): t ∝ E*w
                    //   (in Abdi's paper, E is the residuals for X).

                    // 2.1. Compute t = E*w
                    t = new double[rows];
                    for (int i = 0; i < t.Length; i++)
                        for (int j = 0; j < w.Length; j++)
                            t[i] += E[i][j] * w[j];

                    // 2.2. Normalize t: t = t/norm(t)
                    t = t.Divide(norm_t = Norm.Euclidean(t));


                    // Step 3. Estimate c (Y weights): c ∝ F't
                    //   (in Abdi's paper, F is the residuals for Y).

                    // 3.1. Compute c = F'*t0;
                    c = new double[ycols];
                    for (int j = 0; j < c.Length; j++)
                        for (int i = 0; i < t.Length; i++)
                            c[j] += F[i][j] * t[i];

                    // 3.2. Normalize q: c = c/norm(q)
                    c = c.Divide(Norm.Euclidean(c));


                    // Step 4. Estimate u (Y scores): u = F*q
                    //   (in Abdi's paper, F is the residuals for Y).

                    // 4.1. Compute u = F*q;
                    u = new double[rows];
                    for (int i = 0; i < u.Length; i++)
                        for (int j = 0; j < c.Length; j++)
                            u[i] += F[i][j] * c[j];


                    // Recalculate norm of the difference
                    norm_t = 0.0;
                    for (int i = 0; i < t.Length; i++)
                    {
                        double d = (t0[i] - t[i]);
                        norm_t += d * d;
                    }

                    norm_t = Math.Sqrt(norm_t);
                    #endregion
                }

                // Compute the value of b which is used to
                // predict Y from t as b = t'u [Abdi, 2010]
                double b = t.Dot(u);

                // Compute factor loadings for X as p = E'*t [Abdi, 2010]
                double[] p = new double[xcols];
                for (int j = 0; j < p.Length; j++)
                    for (int i = 0; i < rows; i++)
                        p[j] += E[i][j] * t[i];

                // Perform deflation of X and Y
                for (int i = 0; i < t.Length; i++)
                {
                    // Deflate X as X = X - t*p';
                    for (int j = 0; j < p.Length; j++)
                        E[i][j] -= t[i] * p[j];

                    // Deflate Y as Y = Y - b*t*q';
                    for (int j = 0; j < c.Length; j++)
                        F[i][j] -= b * t[i] * c[j];
                }

                // Calculate explained variances
                varY[factor] = b * b;
                varX[factor] = p.Dot(p);

                // Save iteration cols
                T.SetColumn(factor, t);
                P.SetColumn(factor, p);
                U.SetColumn(factor, u);
                C.SetColumn(factor, c);
                W.SetColumn(factor, w);
                B[factor] = b;

                // Check for residuals as stop criteria
                double[] norm_x = Norm.Euclidean(E, dimension: 0);
                double[] norm_y = Norm.Euclidean(F, dimension: 0);

                stop = true;
                if (!Token.IsCancellationRequested)
                {
                    for (int i = 0; i < norm_x.Length && stop == true; i++)
                    {
                        // If any of the residuals is higher than the tolerance
                        if (norm_x[i] > tolerance || norm_y[i] > tolerance)
                            stop = false;
                    }
                }
            }
            #endregion


            // Solve the linear system R = inv(P')*B
            //var svd = new JaggedSingularValueDecomposition(P.Transpose());
            //this.coeffbase = svd.SolveForDiagonal(B);
            this.coeffbase = P.Transpose().SolveForDiagonal(B, leastSquares: true);

            // Set class variables
            this.scoresX = T;      // factor score matrix T
            this.scoresY = U;      // factor score matrix U
            this.loadingsX = P;    // loading matrix P, the loadings for X such that X = TP + F
            this.loadingsY = C;    // loading matrix C, the loadings for Y such that Y = TC + E
            this.weights = W;      // the columns of W are weight vectors


            // Calculate variance explained proportions
            this.componentProportionX = new double[factors];
            this.componentProportionY = new double[factors];

            double sumX = 0.0, sumY = 0.0;
            for (int i = 0; i < rows; i++)
            {
                // Sum of squares for matrix X
                for (int j = 0; j < xcols; j++)
                    sumX += inputsX[i][j] * inputsX[i][j];

                // Sum of squares for matrix Y
                for (int j = 0; j < ycols; j++)
                    sumY += outputsY[i][j] * outputsY[i][j];
            }

            // Calculate variance proportions
            for (int i = 0; i < factors; i++)
            {
                componentProportionY[i] = varY[i] / sumY;
                componentProportionX[i] = varX[i] / sumX;
            }
        }

        /// <summary>
        ///   Computes PLS parameters using SIMPLS algorithm.
        /// </summary>
        /// 
        /// <param name="factors">The number of factors to compute.</param>
        /// <param name="inputsX">The mean-centered input values X.</param>
        /// <param name="outputsY">The mean-centered output values Y.</param>
        ///
        /// <remarks>
        /// <para>
        ///   The algorithm implementation is based on the appendix code by Martin Anderson,
        ///   with modifications for multiple output variables as given in the sources listed
        ///   below.</para>
        ///   
        /// <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       Martin Anderson, "A comparison of nine PLS1 algorithms". Available on:
        ///       http://onlinelibrary.wiley.com/doi/10.1002/cem.1248/pdf </description></item>
        ///     <item><description>
        ///       Abdi, H. (2010). Partial least square regression, projection on latent structure regression,
        ///       PLS-Regression. Wiley Interdisciplinary Reviews: Computational Statistics, 2, 97-106. 
        ///       Available from: http://www.utdallas.edu/~herve/abdi-wireCS-PLS2010.pdf </description></item>
        ///     <item><description>
        ///       StatSoft, Inc. (2012). Electronic Statistics Textbook: Partial Least Squares (PLS).
        ///       Tulsa, OK: StatSoft. Available from: http://www.statsoft.com/textbook/partial-least-squares/#SIMPLS
        /// </description></item>
        ///     <item><description>
        ///       De Jong, S. (1993). SIMPLS: an alternative approach to partial least squares regression.
        ///       Chemometrics and Intelligent Laboratory Systems, 18: 251–263.
        ///       http://dx.doi.org/10.1016/0169-7439(93)85002-X </description></item>
        ///    </list></para>
        /// </remarks>
        /// 
        private void simpls(double[][] inputsX, double[][] outputsY, int factors)
        {
            // Initialize and prepare the data
            int rows = inputsX.Rows();
            int xcols = inputsX.Columns();
            int ycols = outputsY.Columns();

            // Initialize storage variables
            var T = Jagged.Zeros(rows, factors);  // factor score matrix T
            var U = Jagged.Zeros(rows, factors);  // factor score matrix U
            var P = Jagged.Zeros(xcols, factors); // loading matrix P, the loadings for X such that X = TP + F
            var C = Jagged.Zeros(ycols, factors); // loading matrix C, the loadings for Y such that Y = TC + E
            var W = Jagged.Zeros(xcols, factors); // weight matrix W
            var varX = new double[factors];
            var varY = new double[factors];

            // Orthogonal loadings
            var V = Jagged.Zeros(xcols, factors);


            // Create covariance matrix C = X'Y
            var covariance = inputsX.TransposeAndDot(outputsY);

            #region SIMPLS
            for (int factor = 0; factor < factors; factor++)
            {
                // Step 1. Obtain the dominant eigenvector w of C'C. However, we
                //   can avoid computing the matrix multiplication by using the
                //   singular value decomposition instead, which is also more
                //   stable. The first weight vector w is the left singular vector
                //   of C=X'Y [Abdi, 2007].

                var svd = new JaggedSingularValueDecomposition(covariance,
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: false,
                    autoTranspose: true);

                double[] w = svd.LeftSingularVectors.GetColumn(0);
                double[] c = covariance.TransposeAndDot(w);

                // Step 2. Estimate X factor scores: t ∝ X*w
                //   Similarly to NIPALS, the T factor of SIMPLS
                //   is computed as T=X*W [Statsoft] [Abdi, 2010].

                // 2.1. Estimate t (X factor scores): t = X*w [Abdi, 2010]
                double[] t = new double[rows];
                for (int i = 0; i < t.Length; i++)
                    for (int j = 0; j < w.Length; j++)
                        t[i] += inputsX[i][j] * w[j];

                // 2.2. Normalize t (X factor scores): t = t/norm(t)
                double norm_t = Norm.Euclidean(t);
                t.Divide(norm_t, result: t);

                // Step 3. Estimate p (X factor loadings): p = X'*t
                double[] p = new double[xcols];
                for (int i = 0; i < p.Length; i++)
                    for (int j = 0; j < t.Length; j++)
                        p[i] += inputsX[j][i] * t[j];


                // Step 4. Estimate X and Y weights. Actually, the weights have
                //   been computed in the first step during SVD. However, since
                //   the X factor scores have been normalized, we also have to
                //   normalize weights accordingly: w = w/norm(t), c = c/norm(t)
                w = w.Divide(norm_t);
                c = c.Divide(norm_t);

                // Step 5. Estimate u (Y factor scores): u = Y*c [Abdi, 2010]
                double[] u = new double[rows];
                for (int i = 0; i < u.Length; i++)
                    for (int j = 0; j < c.Length; j++)
                        u[i] += outputsY[i][j] * c[j];

                // Step 6. Create orthogonal loading
                double[] v = (double[])p.Clone();

                // Step 7. Make v orthogonal to the previous loadings
                // http://en.wikipedia.org/wiki/Gram%E2%80%93Schmidt_process

                if (factor > 0)
                {
                    // 7.1. MGS for v [Martin Anderson, 2009]
                    for (int j = 0; j < factor; j++)
                    {
                        double proj = 0.0;
                        for (int k = 0; k < v.Length; k++)
                            proj += v[k] * V[k][j];

                        for (int k = 0; k < v.Length; k++)
                            v[k] -= proj * V[k][j];
                    }

                    // 7.1. MGS for u [Martin Anderson, 2009]
                    for (int j = 0; j < factor; j++)
                    {
                        double proj = 0.0;
                        for (int k = 0; k < u.Length; k++)
                            proj += u[k] * T[k][j];

                        for (int k = 0; k < u.Length; k++)
                            u[k] -= proj * T[k][j];
                    }
                }

                // 7.2. Normalize orthogonal loadings
                v = v.Divide(Norm.Euclidean(v));

                // Step 8. Deflate covariance matrix as s = s - v * (v' * s)
                //   as shown in simpls1 in [Martin Anderson, 2009] appendix.
                var cov = covariance.Copy();
                for (int i = 0; i < v.Length; i++)
                {
                    for (int j = 0; j < v.Length; j++)
                    {
                        double d = v[i] * v[j];

                        for (int k = 0; k < ycols; k++)
                            cov[i][k] -= d * covariance[j][k];
                    }
                }

                covariance = cov;

                // Save iteration cols
                W.SetColumn(factor, w);
                U.SetColumn(factor, u);
                C.SetColumn(factor, c);
                T.SetColumn(factor, t);
                P.SetColumn(factor, p);
                V.SetColumn(factor, v);

                // Compute explained variance
                varX[factor] = p.Dot(p);
                varY[factor] = c.Dot(c);

                if (Token.IsCancellationRequested)
                    break;
            }
            #endregion

            // Set class variables
            this.scoresX = T;      // factor score matrix T
            this.scoresY = U;      // factor score matrix U
            this.loadingsX = P;    // loading matrix P, the loadings for X such that X = TP + F
            this.loadingsY = C;    // loading matrix C, the loadings for Y such that Y = TC + E
            this.weights = W;      // the columns of W are weight vectors
            this.coeffbase = W;


            // Calculate variance explained proportions
            this.componentProportionX = new double[factors];
            this.componentProportionY = new double[factors];

            double sumX = 0.0, sumY = 0.0;
            for (int i = 0; i < rows; i++)
            {
                // Sum of squares for matrix X
                for (int j = 0; j < xcols; j++)
                    sumX += inputsX[i][j] * inputsX[i][j];

                // Sum of squares for matrix Y
                for (int j = 0; j < ycols; j++)
                    sumY += outputsY[i][j] * outputsY[i][j];
            }

            // Calculate variance proportions
            for (int i = 0; i < factors; i++)
            {
                componentProportionY[i] = varY[i] / sumY;
                componentProportionX[i] = varX[i] / sumX;
            }

        }
        #endregion




        /// <summary>
        ///   Adjusts a data matrix, centering and standardizing its values
        ///   using the already computed column's means and standard deviations.
        /// </summary>
        /// 
        protected double[,] Adjust(double[,] matrix, double[] columnMeans, double[] columnStdDev, bool inPlace)
        {
            // Center the data around the mean. Will have no effect if
            //  the data is already centered (the mean will be zero).
            double[,] result = matrix.Center(columnMeans, inPlace);

            // Check if we also have to standardize our data (convert to Z Scores).
            if (columnStdDev != null && this.analysisMethod == AnalysisMethod.Standardize)
            {
                for (int j = 0; j < columnStdDev.Length; j++)
                    if (columnStdDev[j] == 0) throw new ArithmeticException("Standard deviation cannot be" +
                        " zero (cannot standardize the constant variable at column index " + j + ").");

                // Yes. Divide by standard deviation
                result.Standardize(columnStdDev, true);
            }

            return result;
        }

        /// <summary>
        ///   Adjusts a data matrix, centering and standardizing its values
        ///   using the already computed column's means and standard deviations.
        /// </summary>
        /// 
        protected double[][] Adjust(double[][] matrix, double[] columnMeans, double[] columnStdDev, bool inPlace)
        {
            // Center the data around the mean. Will have no effect if
            //  the data is already centered (the mean will be zero).
            var result = matrix.Center(columnMeans, inPlace);

            // Check if we also have to standardize our data (convert to Z Scores).
            if (columnStdDev != null && this.analysisMethod == AnalysisMethod.Standardize)
            {
                for (int j = 0; j < columnStdDev.Length; j++)
                    if (columnStdDev[j] == 0) throw new ArithmeticException("Standard deviation cannot be" +
                        " zero (cannot standardize the constant variable at column index " + j + ").");

                // Yes. Divide by standard deviation
                result.Standardize(columnStdDev, true);
            }

            return result;
        }

        /// <summary>
        ///   Returns the index for the column with largest squared sum.
        /// </summary>
        /// 
        private static int largest(double[][] matrix)
        {
            int rows = matrix.Rows();
            int cols = matrix.Columns();

            int index = 0;
            double max = 0;
            for (int i = 0; i < cols; i++)
            {
                double squareSum = 0.0;

                for (int j = 0; j < rows; j++)
                    squareSum += matrix[j][i] * matrix[j][i];

                if (squareSum > max)
                {
                    max = squareSum;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        ///   Computes the variable importance in projection (VIP).
        /// </summary>
        /// 
        /// <returns>
        ///   A predictor factors matrix in which each row represents
        ///   the importance of the variable in a projection considering
        ///   the number of factors indicated by the column's index.
        /// </returns>
        /// 
        /// <remarks>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///      Il-Gyo Chong, Chi-Hyuck Jun, Performance of some variable selection methods
        ///      when multicollinearity is present, Chemometrics and Intelligent Laboratory 
        ///      Systems, Volume 78, Issues 1-2, 28 July 2005, Pages 103-112, ISSN 0169-7439,
        ///      DOI: 10.1016/j.chemolab.2004.12.011.</description></item></list>
        /// </remarks>
        /// 
        protected double[][] ComputeVariableImportanceInProjection(int factors)
        {
            // Tested against VIP.R code from Bjørn-Helge Mevik.
            // Available on http://mevik.net/work/software/VIP.R

            int xcols = NumberOfInputs;

            double[][] importance = Jagged.Zeros(xcols, factors);

            // For each input variable
            for (int j = 0; j < xcols; j++)
            {
                double[] SS1 = new double[factors];
                double[] SS2 = new double[factors];

                // For each latent factor
                for (int k = 0; k < factors; k++)
                {
                    // Assume single response variable
                    double b = loadingsY.GetColumn(k)[0];
                    double[] t = scoresX.GetColumn(k);
                    double[] w = loadingsX.GetColumn(k);

                    double ss = (b * b) * (t.Dot(t));
                    double wn = (w[j] * w[j]) / Norm.SquareEuclidean(w);

                    SS1[k] = ss * wn;
                    SS2[k] = ss;
                }

                double[] sum1 = Matrix.CumulativeSum(SS1);
                double[] sum2 = Matrix.CumulativeSum(SS2);

                for (int k = 0; k < factors; k++)
                    importance[j][k] = Math.Sqrt(xcols * sum1[k] / sum2[k]);
            }

            return importance;
        }
    }



    #region Support Classes

    /// <summary>
    ///   Represents a Partial Least Squares Factor found in the Partial Least Squares
    ///   Analysis, allowing it to be directly bound to controls like the DataGridView.
    /// </summary>
    /// 
    [Serializable]
    public sealed class PartialLeastSquaresFactor : IAnalysisComponent
    {

        private int index;
        private PartialLeastSquaresAnalysis analysis;


        /// <summary>
        ///   Creates a partial least squares factor representation.
        /// </summary>
        /// 
        /// <param name="analysis">The analysis to which this component belongs.</param>
        /// <param name="index">The component index.</param>
        /// 
        internal PartialLeastSquaresFactor(PartialLeastSquaresAnalysis analysis, int index)
        {
            this.index = index;
            this.analysis = analysis;
        }


        /// <summary>
        ///   Gets the Index of this component on the original factor collection.
        /// </summary>
        /// 
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        ///   Returns a reference to the parent analysis object.
        /// </summary>
        /// 
        public PartialLeastSquaresAnalysis Analysis
        {
            get { return this.analysis; }
        }

        /// <summary>
        ///   Gets the proportion of prediction variables
        ///   variance explained by this factor.
        /// </summary>
        /// 
        public double PredictorProportion
        {
            get { return this.analysis.componentProportionX[index]; }
        }

        /// <summary>
        ///   Gets the cumulative proportion of dependent variables
        ///   variance explained by this factor.
        /// </summary>
        /// 
        public double PredictorCumulativeProportion
        {
            get { return this.analysis.cumulativeProportionX[index]; }
        }

        /// <summary>
        ///   Gets the proportion of dependent variable
        ///   variance explained by this factor.
        /// </summary>
        /// 
        public double DependentProportion
        {
            get { return this.analysis.componentProportionY[index]; }
        }

        /// <summary>
        ///   Gets the cumulative proportion of dependent variable
        ///   variance explained by this factor.
        /// </summary>
        /// 
        public double DependentCumulativeProportion
        {
            get { return this.analysis.cumulativeProportionY[index]; }
        }

        /// <summary>
        ///   Gets the input variable's latent vectors for this factor.
        /// </summary>
        /// 
        public double[] IndependentLatentVectors
        {
            get { return this.analysis.loadingsX.GetColumn(index); }
        }

        /// <summary>
        ///   Gets the output variable's latent vectors for this factor.
        /// </summary>
        /// 
        public double[] DependentLatentVector
        {
            get { return this.analysis.loadingsY.GetColumn(index); }
        }

        /// <summary>
        ///   Gets the importance of each variable for the given component.
        /// </summary>
        /// 
        public double[] VariableImportance
        {
            get { return this.analysis.Importance.GetColumn(index); }
        }

        /// <summary>
        ///   Gets the proportion, or amount of information explained by this component.
        /// </summary>
        /// 
        double IAnalysisComponent.Proportion
        {
            get { return DependentProportion; }
        }

        /// <summary>
        ///   Gets the cumulative proportion of all discriminants until this component.
        /// </summary>
        /// 
        double IAnalysisComponent.CumulativeProportion
        {
            get { return DependentCumulativeProportion; }
        }
    }

    /// <summary>
    ///   Represents a Collection of Partial Least Squares Factors found in
    ///   the Partial Least Squares Analysis. This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class PartialLeastSquaresFactorCollection : ReadOnlyCollection<PartialLeastSquaresFactor>
    {
        internal PartialLeastSquaresFactorCollection(PartialLeastSquaresFactor[] components)
            : base(components) { }
    }


    /// <summary>
    ///   Represents source variables used in Partial Least Squares Analysis. Can represent either
    ///   input variables (predictor variables) or output variables (independent variables or regressors).
    /// </summary>
    /// 
    [Serializable]
    public class PartialLeastSquaresVariables
    {

        private PartialLeastSquaresAnalysis analysis;
        private bool inputs;

        internal PartialLeastSquaresVariables(PartialLeastSquaresAnalysis analysis, bool inputs)
        {
            this.analysis = analysis;
            this.inputs = inputs;
        }

        /// <summary>
        ///   Source data used in the analysis. Can be either input data
        ///   or output data depending if the variables chosen are predictor
        ///   variables or dependent variables, respectively.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Source
        {
#pragma warning disable 612, 618
            get { return inputs ? analysis.sourceX : analysis.sourceY; }
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Gets the resulting projection (scores) of the source data
        ///   into latent space. Can be either from input data or output
        ///   data depending if the variables chosen are predictor variables
        ///   or dependent variables, respectively.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Result
        {
            get { return inputs ? analysis.scoresX.ToMatrix() : analysis.scoresY.ToMatrix(); }
        }

        /// <summary>
        ///   Gets the column means of the source data. Can be either from
        ///   input data or output data, depending if the variables chosen
        ///   are predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        public double[] Means
        {
            get { return inputs ? analysis.meanX : analysis.meanY; }
        }

        /// <summary>
        ///   Gets the column standard deviations of the source data. Can be either 
        ///   from input data or output data, depending if the variables chosen are
        ///   predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        public double[] StandardDeviations
        {
            get { return inputs ? analysis.stdDevX : analysis.stdDevY; }
        }

        /// <summary>
        ///   Gets the loadings (a.k.a factors or components) for the 
        ///   variables obtained during the analysis. Can be either from
        ///   input data or output data, depending if the variables chosen
        ///   are predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        public double[][] FactorMatrix
        {
            get { return inputs ? analysis.loadingsX : analysis.loadingsY; }
        }

        /// <summary>
        ///   Gets the amount of variance explained by each latent factor.
        ///   Can be either by input variables' latent factors or output
        ///   variables' latent factors, depending if the variables chosen
        ///   are predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        public double[] FactorProportions
        {
            get { return inputs ? analysis.componentProportionX : analysis.componentProportionY; }
        }

        /// <summary>
        ///   Gets the cumulative variance explained by each latent factor.
        ///   Can be either by input variables' latent factors or output
        ///   variables' latent factors, depending if the variables chosen
        ///   are predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        public double[] CumulativeProportions
        {
            get { return inputs ? analysis.cumulativeProportionX : analysis.cumulativeProportionY; }
        }

        /// <summary>
        ///   Projects a given dataset into latent space. Can be either input variable's
        ///   latent space or output variable's latent space, depending if the variables
        ///   chosen are predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        public double[][] Transform(double[][] data)
        {
            return inputs ? analysis.Transform(data) : analysis.TransformOutput(data);
        }

        /// <summary>
        ///   Projects a given dataset into latent space. Can be either input variable's
        ///   latent space or output variable's latent space, depending if the variables
        ///   chosen are predictor variables or dependent variables, respectively.
        /// </summary>
        /// 
        [Obsolete("Please set the analysis NumberOfOutputs to the desired number of factors.")]
        public double[][] Transform(double[][] data, int factors)
        {
            int previous = analysis.NumberOfOutputs;
            analysis.NumberOfOutputs = factors;
            double[][] result = inputs ? analysis.Transform(data) : analysis.TransformOutput(data);
            analysis.NumberOfOutputs = previous;
            return result;
        }
    }

    #endregion

}