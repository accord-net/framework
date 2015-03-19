﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    ///   Logistic Regression Analysis.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Logistic Regression Analysis tries to extract useful
    ///   information about a logistic regression model. </para>
    ///   
    /// <para>
    ///   This class can also be bound to standard controls such as the 
    ///   <a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView</a>
    ///   by setting their DataSource property to the analysis' <see cref="Coefficients"/> property.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       E. F. Connor. Logistic Regression. Available on:
    ///       http://userwww.sfsu.edu/~efc/classes/biol710/logistic/logisticreg.htm </description></item>
    ///     <item><description>
    ///       C. Shalizi. Logistic Regression and Newton's Method. Lecture notes. Available on:
    ///       http://www.stat.cmu.edu/~cshalizi/350/lectures/26/lecture-26.pdf </description></item>
    ///     <item><description>
    ///       A. Storkey. Learning from Data: Learning Logistic Regressors. Available on:
    ///       http://www.inf.ed.ac.uk/teaching/courses/lfd/lectures/logisticlearn-print.pdf </description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Suppose we have the following data about some patients.
    /// // The first variable is continuous and represent patient
    /// // age. The second variable is dichotomic and give whether
    /// // they smoke or not (this is completely fictional data).
    /// 
    /// double[][] inputs =
    /// {
    ///     //            Age  Smoking
    ///     new double[] { 55,    0   }, 
    ///     new double[] { 28,    0   }, 
    ///     new double[] { 65,    1   }, 
    ///     new double[] { 46,    0   }, 
    ///     new double[] { 86,    1   }, 
    ///     new double[] { 56,    1   }, 
    ///     new double[] { 85,    0   }, 
    ///     new double[] { 33,    0   }, 
    ///     new double[] { 21,    1   }, 
    ///     new double[] { 42,    1   }, 
    /// };
    /// 
    /// // Additionally, we also have information about whether
    /// // or not they those patients had lung cancer. The array
    /// // below gives 0 for those who did not, and 1 for those
    /// // who did.
    /// 
    /// double[] output =
    /// {
    ///     0, 0, 0, 1, 1, 1, 0, 0, 0, 1
    /// };
    /// 
    /// // Create a Logistic Regression analysis
    /// var regression = new LogisticRegressionAnalysis(inputs, output);
    /// 
    /// regression.Compute(); // compute the analysis.
    /// 
    /// // Now we can show a summary of analysis
    /// DataGridBox.Show(regression.Coefficients);
    /// </code>
    /// 
    /// <para>
    ///   The resulting table is shown below.</para>
    ///   <img src="..\images\logistic-regression.png" />
    /// 
    /// <code>
    /// // We can also investigate all parameters individually. For
    /// // example the coefficients values will be available at the
    /// // vector
    ///             
    /// double[] coef = regression.CoefficientValues;
    /// 
    /// // The first value refers to the model's intercept term. We
    /// // can also retrieve the odds ratios and standard errors:
    /// 
    /// double[] odds = regression.OddsRatios;
    /// double[] stde = regression.StandardErrors;
    /// 
    ///             
    /// // Finally, we can also use the analysis to classify a new patient
    /// double y = regression.Regression.Compute(new double[] { 87, 1 });
    ///             
    /// // For those inputs, the answer probability is approximately 75%.
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class LogisticRegressionAnalysis : IRegressionAnalysis
    {
        private LogisticRegression regression;

        private int inputCount;
        private double[] coefficients;
        private double[] standardErrors;
        private double[] oddsRatios;

        private WaldTest[] waldTests;
        private ChiSquareTest[] ratioTests;

        private DoubleRange[] confidences;

        private double deviance;
        private double logLikelihood;
        private ChiSquareTest chiSquare;

        private double[][] inputData;
        private double[] outputData;
        private double[] weights;

        private string[] inputNames;
        private string outputName;

        private double[,] sourceMatrix;
        private double[] result;

        double regularization = 1e-10;

        private LogisticCoefficientCollection coefficientCollection;

        private int iterations = 50;
        private double tolerance = 1e-5;

        private bool innerComputed = false;


        #region Constructors
        /// <summary>
        ///   Constructs a Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// 
        public LogisticRegressionAnalysis(double[][] inputs, double[] outputs)
        {
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
            {
                throw new ArgumentException("The number of rows in the input array"
                    + " must match the number of given outputs.", "outputs");
            }


            initialize(inputs, outputs);

            // Start regression using the Null Model
            this.regression = new LogisticRegression(inputCount);
        }

        /// <summary>
        ///   Constructs a Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// <param name="weights">The weights associated with each input vector.</param>
        /// 
        public LogisticRegressionAnalysis(double[][] inputs, double[] outputs, double[] weights)
            : this(inputs, outputs)
        {
            if (weights == null)
                throw new ArgumentNullException("weights");

            if (weights.Length != outputs.Length)
            {
                throw new ArgumentException("The number weights"
                    + " must match the number of given input rows.", "outputs");
            }

            this.weights = weights;
        }

        /// <summary>
        ///   Constructs a Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output, binary data for the analysis.</param>
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="outputName">The name of the output variable.</param>
        /// 
        public LogisticRegressionAnalysis(double[][] inputs, double[] outputs,
            String[] inputNames, String outputName)
            : this(inputs, outputs)
        {
            this.inputNames = inputNames;
            this.outputName = outputName;
        }

        private void initialize(double[][] inputs, double[] outputs)
        {
            this.inputCount = inputs[0].Length;
            int coefficientCount = inputCount + 1;

            // Store data sets
            this.inputData = inputs;
            this.outputData = outputs;


            // Create additional structures
            this.coefficients = new double[coefficientCount];
            this.waldTests = new WaldTest[coefficientCount];
            this.standardErrors = new double[coefficientCount];
            this.oddsRatios = new double[coefficientCount];
            this.confidences = new DoubleRange[coefficientCount];
            this.ratioTests = new ChiSquareTest[coefficientCount];

            this.outputName = "Output";
            this.inputNames = new string[inputCount];
            for (int i = 0; i < inputNames.Length; i++)
                inputNames[i] = "Input " + i;

            // Create object-oriented structure to represent the analysis
            var logCoefs = new List<LogisticCoefficient>(coefficientCount);
            for (int i = 0; i < coefficientCount; i++)
                logCoefs.Add(new LogisticCoefficient(this, i));
            this.coefficientCollection = new LogisticCoefficientCollection(logCoefs);
        }


        #endregion



        #region Public Properties

        /// <summary>
        ///   Gets or sets the maximum number of iterations to be
        ///   performed by the regression algorithm. Default is 50.
        /// </summary>
        /// 
        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        /// <summary>
        ///   Gets or sets the difference between two iterations of the regression 
        ///   algorithm when the algorithm should stop. The difference is calculated
        ///   based on the largest absolute parameter change of the regression. Default
        ///   is 1e-5.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the regularization value to be
        ///   added in the objective function. Default is
        ///   1e-10.
        /// </summary>
        /// 
        public double Regularization
        {
            get { return regularization; }
            set { regularization = value; }
        }

        /// <summary>
        ///   Gets the source matrix from which the analysis was run.
        /// </summary>
        /// 
        public double[,] Source
        {
            get
            {
                if (sourceMatrix == null)
                    this.sourceMatrix = inputData.ToMatrix();
                return sourceMatrix;
            }
        }

        /// <summary>
        ///   Gets the source matrix from which the analysis was run.
        /// </summary>
        /// 
        public double[][] Array
        {
            get { return inputData; }
        }

        /// <summary>
        ///   Gets the dependent variable value
        ///   for each of the source input points.
        /// </summary>
        /// 
        public double[] Outputs
        {
            get { return outputData; }
        }

        /// <summary>
        ///   Gets the resulting probabilities obtained
        ///   by the logistic regression model.
        /// </summary>
        /// 
        public double[] Result
        {
            get { return result; }
        }

        /// <summary>
        ///   Gets the sample weight associated with each input vector.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return weights; }
        }

        /// <summary>
        ///   Gets the Logistic Regression model created
        ///   and evaluated by this analysis.
        /// </summary>
        /// 
        public LogisticRegression Regression
        {
            get { return regression; }
        }

        /// <summary>
        ///   Gets the collection of coefficients of the model.
        /// </summary>
        /// 
        public LogisticCoefficientCollection Coefficients
        {
            get { return coefficientCollection; }
        }

        /// <summary>
        ///   Gets the Log-Likelihood for the model.
        /// </summary>
        /// 
        public double LogLikelihood
        {
            get { return this.logLikelihood; }
        }

        /// <summary>
        ///   Gets the Chi-Square (Likelihood Ratio) Test for the model.
        /// </summary>
        /// 
        public ChiSquareTest ChiSquare
        {
            get { return this.chiSquare; }
        }

        /// <summary>
        ///   Gets the Deviance of the model.
        /// </summary>
        /// 
        public double Deviance
        {
            get { return deviance; }
        }

        /// <summary>
        ///   Gets the name of the input variables for the model.
        /// </summary>
        /// 
        public String[] Inputs
        {
            get { return inputNames; }
        }

        /// <summary>
        ///   Gets the name of the output variable for the model.
        /// </summary>
        /// 
        public String Output
        {
            get { return outputName; }
        }

        /// <summary>
        ///   Gets the Odds Ratio for each coefficient
        ///   found during the logistic regression.
        /// </summary>
        /// 
        public double[] OddsRatios
        {
            get { return this.oddsRatios; }
        }

        /// <summary>
        ///   Gets the Standard Error for each coefficient
        ///   found during the logistic regression.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get { return this.standardErrors; }
        }

        /// <summary>
        ///   Gets the Wald Tests for each coefficient.
        /// </summary>
        /// 
        public WaldTest[] WaldTests
        {
            get { return this.waldTests; }
        }

        /// <summary>
        ///   Gets the Likelihood-Ratio Tests for each coefficient.
        /// </summary>
        /// 
        /// <remarks>
        ///   Since this operation might be potentially time-consuming, the likelihood-ratio
        ///   tests will be computed on the first time this property is acessed.
        /// </remarks>
        /// 
        public ChiSquareTest[] LikelihoodRatioTests
        {
            get
            {
                if (innerComputed == false)
                    computeInner();

                return this.ratioTests;
            }
        }

        /// <summary>
        ///   Gets the value of each coefficient.
        /// </summary>
        /// 
        public double[] CoefficientValues
        {
            get { return this.coefficients; }
        }

        /// <summary>
        ///   Gets the 95% Confidence Intervals (C.I.)
        ///   for each coefficient found in the regression.
        /// </summary>
        /// 
        public DoubleRange[] Confidences
        {
            get { return this.confidences; }
        }

        #endregion


        //---------------------------------------------


        #region Public Methods
        /// <summary>
        ///   Gets the Log-Likelihood Ratio between this model and another model.
        /// </summary>
        /// 
        /// <param name="model">Another logistic regression model.</param>
        /// <returns>The Likelihood-Ratio between the two models.</returns>
        /// 
        public double GetLikelihoodRatio(GeneralizedLinearRegression model)
        {
            return regression.GetLogLikelihoodRatio(inputData, outputData, model);
        }


        /// <summary>
        ///   Computes the Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <remarks>
        ///   The likelihood surface for the logistic regression learning 
        ///   is convex, so there will be only one peak. Any local maxima 
        ///   will be also a global maxima.
        /// </remarks>
        /// 
        /// <returns>
        ///   True if the model converged, false otherwise.
        /// </returns>
        /// 
        public bool Compute()
        {
            return compute();
        }

        /// <summary>
        ///   Computes the Logistic Regression Analysis for an already computed regression.
        /// </summary>
        /// 
        public void Compute(LogisticRegression regression)
        {
            this.regression = regression;

            computeInformation();

            innerComputed = false;
        }

        /// <summary>
        ///   Computes the Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <remarks>The likelihood surface for the
        ///   logistic regression learning is convex, so there will be only one
        ///   peak. Any local maxima will be also a global maxima.
        /// </remarks>
        /// 
        /// <param name="limit">
        ///   The difference between two iterations of the regression algorithm
        ///   when the algorithm should stop. If not specified, the value of
        ///   1e-5 will be used. The difference is calculated based on the largest
        ///   absolute parameter change of the regression.
        /// </param>
        /// 
        /// <param name="maxIterations">
        ///   The maximum number of iterations to be performed by the regression
        ///   algorithm.
        /// </param>
        /// 
        /// <returns>
        ///   True if the model converged, false otherwise.
        /// </returns>
        /// 
        [Obsolete("Please set up the Iterations and Tolerance properties and call Compute() instead.")]
        public bool Compute(double limit = 1e-5, int maxIterations = 50)
        {
            this.iterations = maxIterations;
            this.tolerance = limit;

            return compute();
        }

        #endregion

        /// <summary>
        ///   Creates a new <see cref="LogisticRegressionAnalysis"/> from summarized data.
        ///   In summary data, instead of having a set of inputs and their associated outputs,
        ///   we have the number of times an input vector had a positive label in the data set
        ///   and how many times it had a negative label.
        /// </summary>
        /// 
        /// <param name="data">The input data.</param>
        /// <param name="positives">The number of positives labels for each input vector.</param>
        /// <param name="negatives">The number of negative labels for each input vector.</param>
        /// 
        /// <returns>
        ///   A <see cref="LogisticRegressionAnalysis"/> created from the given summary data.
        /// </returns>
        /// 
        public static LogisticRegressionAnalysis FromSummary(double[][] data, int[] positives, int[] negatives)
        {
            double[] rate = new double[data.Length];
            double[] weights = new double[data.Length];

            for (int i = 0; i < rate.Length; i++)
            {
                rate[i] = positives[i] / (double)(positives[i] + negatives[i]);
                weights[i] = positives[i] + negatives[i];
            }

            return new LogisticRegressionAnalysis(data, rate, weights);
        }


        private bool compute()
        {
            double delta;
            int iteration = 0;

            var learning = new IterativeReweightedLeastSquares(regression);

            learning.Regularization = regularization;

            do // learning iterations until convergence
            {
                delta = learning.Run(inputData, outputData, weights);
                iteration++;

            } while (delta > tolerance && iteration < iterations);

            // Check if the full model has converged
            bool converged = iteration < iterations;


            computeInformation();

            innerComputed = false;

            return converged;
        }

        private void computeInner()
        {
            double limit = tolerance;
            int maxIterations = iterations;

            // Perform likelihood-ratio tests against diminished nested models
            var innerModel = new LogisticRegression(inputCount - 1);
            var learning = new IterativeReweightedLeastSquares(innerModel);

            learning.Regularization = regularization;

            for (int i = 0; i < inputCount; i++)
            {
                // Create a diminished inner model without the current variable
                double[][] data = inputData.RemoveColumn(i);

                int iteration = 0;
                double delta = 0;

                do // learning iterations until convergence
                {
                    delta = learning.Run(data, outputData, weights);
                    iteration++;

                } while (delta > limit && iteration < maxIterations);

                double ratio = 2.0 * (logLikelihood - innerModel.GetLogLikelihood(data, outputData));
                ratioTests[i + 1] = new ChiSquareTest(ratio, 1);
            }

            innerComputed = true;
        }

        private void computeInformation()
        {
            // Store model information
            this.result = regression.Compute(inputData);

            if (weights == null)
            {
                this.deviance = regression.GetDeviance(inputData, outputData);
                this.logLikelihood = regression.GetLogLikelihood(inputData, outputData);
                this.chiSquare = regression.ChiSquare(inputData, outputData);
            }
            else
            {
                this.deviance = regression.GetDeviance(inputData, outputData, weights);
                this.logLikelihood = regression.GetLogLikelihood(inputData, outputData, weights);
                this.chiSquare = regression.ChiSquare(inputData, outputData, weights);
            }

            // Store coefficient information
            for (int i = 0; i < regression.Coefficients.Length; i++)
            {
                this.standardErrors[i] = regression.StandardErrors[i];

                this.waldTests[i] = regression.GetWaldTest(i);
                this.coefficients[i] = regression.Coefficients[i];
                this.confidences[i] = regression.GetConfidenceInterval(i);
                this.oddsRatios[i] = regression.GetOddsRatio(i);
            }
        }


        /// <summary>
        ///   Computes the analysis using given source data and parameters.
        /// </summary>
        /// 
        void IAnalysis.Compute()
        {
            Compute();
        }

    }


    #region Support Classes

    /// <summary>
    ///   Represents a Logistic Regression Coefficient found in the Logistic Regression,
    ///   allowing it to be bound to controls like the DataGridView. This class cannot
    ///   be instantiated outside the <see cref="LogisticRegressionAnalysis"/>.
    /// </summary>
    /// 
    [Serializable]
    public class LogisticCoefficient
    {

        private LogisticRegressionAnalysis analysis;

        private int index;


        internal LogisticCoefficient(LogisticRegressionAnalysis analysis, int index)
        {
            this.analysis = analysis;
            this.index = index;
        }

        /// <summary>
        ///   Gets the name for the current coefficient.
        /// </summary>
        /// 
        public string Name
        {
            get
            {
                if (index == 0) return "Intercept";
                else return analysis.Inputs[index - 1];
            }
        }

        /// <summary>
        ///   Gets the Odds ratio for the current coefficient.
        /// </summary>
        /// 
        [DisplayName("Odds ratio")]
        public double OddsRatio
        {
            get { return analysis.OddsRatios[index]; }
        }

        /// <summary>
        ///   Gets the Standard Error for the current coefficient.
        /// </summary>
        /// 
        [DisplayName("Std. Error")]
        public double StandardError
        {
            get { return analysis.StandardErrors[index]; }
        }

        /// <summary>
        ///   Gets the 95% confidence interval (C.I.) for the current coefficient.
        /// </summary>
        /// 
        [Browsable(false)]
        public DoubleRange Confidence
        {
            get { return analysis.Confidences[index]; }
        }

        /// <summary>
        ///   Gets the upper limit for the 95% confidence interval.
        /// </summary>
        /// 
        [DisplayName("Upper confidence limit")]
        public double ConfidenceUpper
        {
            get { return Confidence.Max; }
        }

        /// <summary>
        ///   Gets the lower limit for the 95% confidence interval.
        /// </summary>
        /// 
        [DisplayName("Lower confidence limit")]
        public double ConfidenceLower
        {
            get { return Confidence.Min; }
        }

        /// <summary>
        ///   Gets the coefficient value.
        /// </summary>
        /// 
        [DisplayName("Value")]
        public double Value
        {
            get { return analysis.CoefficientValues[index]; }
        }

        /// <summary>
        ///   Gets the Wald's test performed for this coefficient.
        /// </summary>
        /// 
        [DisplayName("Wald p-value")]
        public WaldTest Wald
        {
            get { return analysis.WaldTests[index]; }
        }

        /// <summary>
        ///   Gets the Likelihood-Ratio test performed for this coefficient.
        /// </summary>
        /// 
        /// <remarks>
        ///   Since this operation might be potentially time-consuming, the likelihood-ratio
        ///   tests will be computed on the first time this property is acessed.
        /// </remarks>
        /// 
        [DisplayName("Likelihood-Ratio p-value")]
        public ChiSquareTest LikelihoodRatio
        {
            get { return analysis.LikelihoodRatioTests[index]; }
        }

    }

    /// <summary>
    ///   Represents a collection of Logistic Coefficients found in the
    ///   <see cref="LogisticRegressionAnalysis"/>. This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class LogisticCoefficientCollection : ReadOnlyCollection<LogisticCoefficient>
    {
        internal LogisticCoefficientCollection(IList<LogisticCoefficient> coefficients)
            : base(coefficients) { }
    }
    #endregion

}
