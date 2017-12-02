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

namespace Accord.Statistics.Analysis
{
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Accord.Compat;
    using System.Threading;
    using Accord.Statistics.Filters;

    /// <summary>
    ///   Multinomial Logistic Regression Analysis
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, multinomial logistic regression is a classification method that 
    ///   generalizes logistic regression to multiclass problems, i.e. with more than two 
    ///   possible discrete outcomes.[1] That is, it is a model that is used to predict the
    ///   probabilities of the different possible outcomes of a categorically distributed 
    ///   dependent variable, given a set of independent variables (which may be real-valued,
    ///   binary-valued, categorical-valued, etc.).</para>
    ///   
    /// <para>
    ///   Multinomial logistic regression is known by a variety of other names, including
    ///   multiclass LR, multinomial regression,[2] softmax regression, multinomial logit,
    ///   maximum entropy (MaxEnt) classifier, conditional maximum entropy model.</para>para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Multinomial logistic regression." Wikipedia, The Free Encyclopedia, 1st April, 2015.
    ///       Available at: https://en.wikipedia.org/wiki/Multinomial_logistic_regression </description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The first example shows how to reproduce a textbook example using categorical and categorical-with-baseline
    ///   variables. Those variables can be transformed/factored to their respective representations using the 
    ///   <see cref="Codification"/> class. However, please note that while this example uses features from the 
    ///   <see cref="Codification"/> class, the use of this class is not required when learning a <see cref="MultinomialLogisticRegression"/>
    ///   modoel.</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Analysis\MultinomialLogisticRegressionAnalysisTest.cs" region="doc_learn_1" />
    ///   
    /// <para>
    ///   The second example shows how to learn a <see cref="MultinomialLogisticRegressionAnalysis"/> from the 
    ///   famous Fisher's Iris dataset. This example should demonstrate that <see cref="Codification"/> filters
    ///   are not required to successfully learn multinomial logistic regression analyses.</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Analysis\MultinomialLogisticRegressionAnalysisTest.cs" region="doc_learn_2" />
    /// </example>
    /// 
    [Serializable]
    public class MultinomialLogisticRegressionAnalysis : TransformBase<double[], int>,
        IMultivariateRegressionAnalysis,
        ISupervisedLearning<MultinomialLogisticRegression, double[], double[]>,
        ISupervisedLearning<MultinomialLogisticRegression, double[], int>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        int inputCount;
        int outputCount;
        int coefficientCount;

        internal MultinomialLogisticRegression regression;

        private string[] inputNames;
        private string[] outputNames;

        [Obsolete]
        private double[][] inputData;
        [Obsolete]
        private double[][] outputData;
        [Obsolete]
        private double[][] results;

        private double[][] coefficients;
        private double[][] standardErrors;
        private double[][] oddsRatios;

        private WaldTest[][] waldTests;

        private double deviance;
        private double logLikelihood;
        private ChiSquareTest chiSquare;


        // Coefficient measures
        internal DoubleRange[][] confidences;
        internal double confidencePercent = 0.95;

        private MultinomialCoefficientCollection coefficientCollection;

        private int iterations = 50;
        private double tolerance = 1e-5;


#pragma warning disable 612, 618
        /// <summary>
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[][] Array { get { return inputData; } }

        /// <summary>
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Source { get { return inputData.ToMatrix(); } }

        /// <summary>
        ///   Gets the dependent variable value
        ///   for each of the source input points.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Output { get { return outputData.ToMatrix(); } }

        /// <summary>
        ///   Gets the dependent variable value
        ///   for each of the source input points.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[][] Outputs
        {
            get { return outputData; }
        }

        /// <summary>
        ///   Gets the resulting values obtained by the regression model.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[][] Results
        {
            get { return results; }
        }
#pragma warning restore 612, 618

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
        ///  Gets the number of outputs in the regression problem.
        /// </summary>
        /// 
        public int OutputCount { get { return outputCount; } }

        /// <summary>
        ///   Gets the Standard Error for each coefficient
        ///   found during the logistic regression.
        /// </summary>
        /// 
        public double[][] StandardErrors
        {
            get { return this.standardErrors; }
        }

        /// <summary>
        ///   Gets the Regression model created
        ///   and evaluated by this analysis.
        /// </summary>
        /// 
        public MultinomialLogisticRegression Regression
        {
            get { return this.regression; }
        }

        /// <summary>
        ///   Gets the value of each coefficient.
        /// </summary>
        /// 
        public double[][] CoefficientValues
        {
            get { return this.coefficients; }
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
        ///   Gets the Wald Tests for each coefficient.
        /// </summary>
        /// 
        public WaldTest[][] WaldTests
        {
            get { return this.waldTests; }
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="InputNames"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use InputNames instead.")]
        public String[] Inputs
        {
            get { return InputNames; }
            set { InputNames = value; }
        }

        /// <summary>
        ///   Gets or sets the name of the input variables for the model.
        /// </summary>
        /// 
        public String[] InputNames
        {
            get { return inputNames; }
            set { inputNames = value; }
        }

        /// <summary>
        ///   Gets or sets the name of the output variable for the model.
        /// </summary>
        /// 
        public String[] OutputNames
        {
            get { return outputNames; }
            set { outputNames = value; }
        }

        /// <summary>
        ///   Gets the Confidence Intervals (C.I.)
        ///   for each coefficient found in the regression.
        /// </summary>
        /// 
        public DoubleRange[][] Confidences
        {
            get { return this.confidences; }
        }

        /// <summary>
        ///   Gets the collection of coefficients of the model.
        /// </summary>
        /// 
        public MultinomialCoefficientCollection Coefficients
        {
            get { return coefficientCollection; }
        }

        /// <summary>
        ///   Constructs a Multinomial Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// 
        [Obsolete("Please pass the inputs and outputs to the Learn() method.")]
        public MultinomialLogisticRegressionAnalysis(double[][] inputs, int[] outputs)
        {
            // Initial argument checking
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");

            init(inputs, Jagged.OneHot(outputs));
        }

        /// <summary>
        ///   Constructs a Multinomial Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// 
        [Obsolete("Please pass the inputs and outputs to the Learn() method.")]
        public MultinomialLogisticRegressionAnalysis(double[][] inputs, double[][] outputs)
        {
            // Initial argument checking
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");

            init(inputs, outputs);
        }

        /// <summary>
        ///   Constructs a Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="outputNames">The names of the output variables.</param>
        /// 
#pragma warning disable 612, 618
        [Obsolete("Please pass the inputs and outputs to the Learn() method.")]
        public MultinomialLogisticRegressionAnalysis(double[][] inputs, int[] outputs,
            String[] inputNames, String[] outputNames)
            : this(inputs, outputs)
#pragma warning restore 612, 618
        {
            names(inputNames, outputNames);
        }

        /// <summary>
        ///   Constructs a Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="outputNames">The names of the output variables.</param>
        /// 
#pragma warning disable 612, 618
        [Obsolete("Please pass the inputs and outputs to the Learn() method.")]
        public MultinomialLogisticRegressionAnalysis(double[][] inputs, double[][] outputs,
            String[] inputNames, String[] outputNames)
            : this(inputs, outputs)
        {
            names(inputNames, outputNames);
        }

        /// <summary>
        ///   Constructs a Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="outputNames">The names of the output variables.</param>
        /// 
        public MultinomialLogisticRegressionAnalysis(String[] inputNames, String[] outputNames)
        {
            this.inputNames = inputNames;
            this.outputNames = outputNames;
        }

        /// <summary>
        ///   Constructs a Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        public MultinomialLogisticRegressionAnalysis()
        {
        }

        private void names(String[] inputNames, String[] outputNames)
        {
            if (inputNames.Length != this.inputNames.Length)
            {
                throw new ArgumentException("The input names vector should have the same length"
                  + " as the number of variables in the analysis. In this analysis, there are "
                  + this.inputNames.Length + " variables expected.");
            }

            if (outputNames.Length != this.outputNames.Length)
            {
                throw new ArgumentException("The output names vector should have the same length"
                  + " as the number of output classes in the analysis. In this analysis, there are "
                  + this.outputNames.Length + " class labels expected.");
            }

            this.inputNames = inputNames;
            this.outputNames = outputNames;
        }

        private void init(double[][] inputs, double[][] outputs)
        {
            this.inputCount = inputs[0].Length;
            this.outputCount = outputs[0].Length;

            for (int i = 0; i < inputs.Length; i++)
                if (inputs[i].Length != inputCount)
                    throw new ArgumentException("All input vectors must have the same length.");

            for (int i = 0; i < outputs.Length; i++)
                if (outputs[i].Length != outputCount)
                    throw new ArgumentException("All output vectors must have the same length.");

            // Store data sets
            this.inputData = inputs;
            this.outputData = outputs;

            // Create the linear regression
            regression = new MultinomialLogisticRegression(inputCount, outputCount);

            // Create additional structures
            this.coefficientCount = regression.Coefficients[0].Length;
            this.coefficients = regression.Coefficients;
            this.standardErrors = regression.StandardErrors;
            this.confidences = new DoubleRange[outputCount - 1][];
            this.oddsRatios = new double[outputCount - 1][];
            this.waldTests = new WaldTest[outputCount - 1][];
            this.NumberOfInputs = inputs.Columns();
            this.NumberOfOutputs = outputs.Columns();

            for (int i = 0; i < confidences.Length; i++)
            {
                this.confidences[i] = new DoubleRange[coefficientCount];
                this.oddsRatios[i] = new double[coefficientCount];
                this.waldTests[i] = new WaldTest[coefficientCount];
            }

            if (this.inputNames == null)
            {
                this.inputNames = new string[inputCount];
                for (int i = 0; i < inputNames.Length; i++)
                    inputNames[i] = "Input " + i;
            }

            if (this.outputNames == null)
            {
                this.outputNames = new string[outputCount];
                for (int i = 0; i < outputNames.Length; i++)
                    outputNames[i] = "Class " + i;
            }

            // Create object-oriented structure to represent the analysis
            var coefs = new MultinomialCoefficient[(outputCount - 1) * coefficientCount + 1];
            coefs[0] = new MultinomialCoefficient(this, 0, 0);
            for (int k = 1, j = 1; j < outputCount; j++)
                for (int i = 0; i < coefficientCount; i++, k++)
                    coefs[k] = new MultinomialCoefficient(this, j, i);

            this.coefficientCollection = new MultinomialCoefficientCollection(coefs);
        }
#pragma warning restore 612, 618

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public MultinomialLogisticRegression Learn(int[][] x, int[][] y, double[] weights = null)
        {
            return Learn(x.ToDouble(), y.ToDouble(), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public MultinomialLogisticRegression Learn(double[][] x, double[][] y, double[] weights = null)
        {
            init(x, y);

            var learning = new LowerBoundNewtonRaphson(regression)
            {
                Tolerance = tolerance,
                MaxIterations = iterations,
                Token = Token
            };

            learning.Learn(x, y, weights);

            computeInformation(x, y);

            return regression;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public MultinomialLogisticRegression Learn(double[][] x, int[] y, double[] weights = null)
        {
            return Learn(x, Jagged.OneHot(y), weights);
        }

        /// <summary>
        ///   Computes the Multinomial Logistic Regression Analysis.
        /// </summary>
        /// 
        [Obsolete("Please use the Learn method instead.")]
        public bool Compute()
        {
#pragma warning disable 612, 618
            double delta;
            int iteration = 0;

            var learning = new LowerBoundNewtonRaphson(regression);

            do // learning iterations until convergence
            {
                delta = learning.Run(inputData, outputData);
                iteration++;

            } while (delta > tolerance && iteration < iterations);

            // Check if the full model has converged
            bool converged = iteration < iterations;

            computeInformation(inputData, outputData);

            return converged;
#pragma warning restore 612, 618
        }

        private void computeInformation(double[][] inputData, double[][] outputData)
        {
            // Store model information
#pragma warning disable 612, 618
            results = regression.Compute(inputData);
#pragma warning restore 612, 618

            this.deviance = regression.GetDeviance(inputData, outputData);
            this.logLikelihood = regression.GetLogLikelihood(inputData, outputData);
            this.chiSquare = regression.ChiSquare(inputData, outputData);

            this.coefficients = regression.Coefficients;
            this.standardErrors = regression.StandardErrors;

            // Store coefficient information
            for (int i = 0; i < waldTests.Length; i++)
            {
                this.waldTests[i] = regression.GetWaldTest(i);
                this.confidences[i] = regression.GetConfidenceInterval(i);
                this.oddsRatios[i] = regression.GetOddsRatio(i);
            }
        }

#pragma warning disable 612, 618
        [Obsolete("Please use the Learn method instead.")]
        void IAnalysis.Compute()
        {
            Compute();
        }
#pragma warning restore 612, 618

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override int Transform(double[] input)
        {
            return regression.Transform(input);
        }
    }

    #region Support Classes
    /// <summary>
    /// <para>
    ///   Represents a Multinomial Logistic Regression coefficient found in the 
    ///   <see cref="MultinomialLogisticRegressionAnalysis">multinomial logistic
    ///   regression analysis</see> allowing it to be bound to controls like the 
    ///   DataGridView. </para>
    ///   
    /// <para>
    ///   This class cannot be instantiated.</para>   
    /// </summary>
    /// 
    [Serializable]
    public class MultinomialCoefficient
    {

        private int index;
        private int category;

        private MultinomialLogisticRegressionAnalysis analysis;


        /// <summary>
        ///   Creates a regression coefficient representation.
        /// </summary>
        /// 
        /// <param name="analysis">The analysis to which this coefficient belongs.</param>
        /// <param name="index">The coefficient's index.</param>
        /// <param name="category">The coefficient's category.</param>
        /// 
        internal MultinomialCoefficient(MultinomialLogisticRegressionAnalysis analysis, int category, int index)
        {
            this.category = category;
            this.index = index;
            this.analysis = analysis;
        }


        /// <summary>
        ///   Gets the Index of this coefficient on the original analysis coefficient collection.
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
        [Browsable(false)]
        public MultinomialLogisticRegressionAnalysis Analysis
        {
            get { return this.analysis; }
        }

        /// <summary>
        ///   Gets the name of the category that this coefficient belongs to.
        /// </summary>
        /// 
        public string Class
        {
            get
            {
                return analysis.OutputNames[category];
            }
        }

        /// <summary>
        ///   Gets the name for the current coefficient.
        /// </summary>
        /// 
        public string Name
        {
            get
            {
                if (category == 0)
                    return "(baseline)";

                if (index == 0)
                    return "Intercept";
                return analysis.InputNames[index - 1];
            }
        }

        /// <summary>
        ///   Gets the coefficient value.
        /// </summary>
        /// 
        [DisplayName("Value")]
        public double Value
        {
            get
            {
                if (category == 0)
                    return 0.0;

                return Analysis.regression.Coefficients[category - 1][index];
            }
        }

        /// <summary>
        ///   Gets the Standard Error for the current coefficient.
        /// </summary>
        /// 
        [DisplayName("Std. Error")]
        public double StandardError
        {
            get
            {
                if (category == 0)
                    return 0.0;

                return Analysis.StandardErrors[category - 1][index];
            }
        }

        /// <summary>
        ///   Gets the confidence interval (C.I.) for the current coefficient.
        /// </summary>
        /// 
        [Browsable(false)]
        public DoubleRange Confidence
        {
            get
            {
                if (category == 0)
                    return new DoubleRange(0, 0);
                return analysis.Confidences[category - 1][index];
            }
        }

        /// <summary>
        ///   Gets the upper limit for the confidence interval.
        /// </summary>
        /// 
        [DisplayName("Upper confidence limit")]
        public double ConfidenceUpper
        {
            get
            {
                if (category == 0)
                    return 0.0;
                return Analysis.Confidences[category - 1][index].Max;
            }
        }

        /// <summary>
        ///   Gets the lower limit for the confidence interval.
        /// </summary>
        /// 
        [DisplayName("Lower confidence limit")]
        public double ConfidenceLower
        {
            get
            {
                if (category == 0)
                    return 0.0;
                return Analysis.Confidences[category - 1][index].Min;
            }
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            if (category == 0)
                return Class + " (baseline)";
            return String.Format("{0}, {1}; {2} ({3}, {4})", Class, Name, Value, ConfidenceLower, ConfidenceUpper);
        }

    }

    /// <summary>
    ///   Represents a Collection of Multinomial Logistic Regression Coefficients found in the 
    ///   <see cref="MultinomialLogisticRegressionAnalysis"/>. This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class MultinomialCoefficientCollection : ReadOnlyCollection<MultinomialCoefficient>
    {

        internal MultinomialCoefficientCollection(MultinomialCoefficient[] components)
            : base(components)
        {
        }
    }
    #endregion

}
