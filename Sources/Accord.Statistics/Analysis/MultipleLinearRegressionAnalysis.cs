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

namespace Accord.Statistics.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Accord.Math;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Statistics.Testing;
    using AForge;

    /// <summary>
    ///   Multiple Linear Regression Analysis
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Linear regression is an approach to model the relationship between 
    ///   a single scalar dependent variable <c>y</c> and one or more explanatory
    ///   variables <c>x</c>. This class uses a <see cref="MultipleLinearRegression"/>
    ///   to extract information about a given problem, such as confidence intervals,
    ///   hypothesis tests and performance measures.</para>
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
    ///       Wikipedia contributors. "Linear regression." Wikipedia, The Free Encyclopedia, 4 Nov. 2012.
    ///       Available at: http://en.wikipedia.org/wiki/Linear_regression </description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Consider the following data. An experimenter would
    /// // like to infer a relationship between two variables
    /// // A and B and a corresponding outcome variable R.
    /// 
    /// double[][] example = 
    /// {
    ///     //                A    B      R
    ///     new double[] {  6.41, 10.11, 26.1 },
    ///     new double[] {  6.61, 22.61, 33.8 },
    ///     new double[] {  8.45, 11.11, 52.7 },
    ///     new double[] {  1.22, 18.11, 16.2 },
    ///     new double[] {  7.42, 12.81, 87.3 },
    ///     new double[] {  4.42, 10.21, 12.5 },
    ///     new double[] {  8.61, 11.94, 77.5 },
    ///     new double[] {  1.73, 13.13, 12.1 },
    ///     new double[] {  7.47, 17.11, 86.5 },
    ///     new double[] {  6.11, 15.13, 62.8 },
    ///     new double[] {  1.42, 16.11, 17.5 },
    /// };
    /// 
    /// // For this, we first extract the input and output
    /// // pairs. The first two columns have values for the
    /// // input variables, and the last for the output:
    /// 
    /// double[][] inputs = example.GetColumns(0, 1);
    /// double[] output = example.GetColumn(2);
    /// 
    /// // Next, we can create a new multiple linear regression for the variables
    /// var regression = new MultipleLinearRegressionAnalysis(inputs, output, intercept: true);
    /// 
    /// regression.Compute(); // compute the analysis
    /// 
    /// // Now we can show a summary of analysis
    /// DataGridBox.Show(regression.Coefficients);
    /// </code>
    /// 
    ///   <img src="..\images\linear-regression.png" />
    /// 
    /// <code>
    /// // We can also show a summary ANOVA
    /// DataGridBox.Show(regression.Table);
    /// </code>
    /// 
    ///   <img src="..\images\linear-anova.png" />
    ///   
    /// <code>
    /// // And also extract other useful information, such
    /// // as the linear coefficients' values and std errors:
    /// double[] coef = regression.CoefficientValues;
    /// double[] stde = regression.StandardErrors;
    /// 
    /// // Coefficients of performance, such as r²
    /// double rsquared = regression.RSquared;
    /// 
    /// // Hypothesis tests for the whole model
    /// ZTest ztest = regression.ZTest;
    /// FTest ftest = regression.FTest;
    /// 
    /// // and for individual coefficients
    /// TTest ttest0 = regression.Coefficients[0].TTest;
    /// TTest ttest1 = regression.Coefficients[1].TTest;
    /// 
    /// // and also extract confidence intervals
    /// DoubleRange ci = regression.Coefficients[0].Confidence;
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class MultipleLinearRegressionAnalysis : IRegressionAnalysis, IAnova
    {

        int inputCount;
        int coefficientCount;

        internal MultipleLinearRegression regression;

        private string[] inputNames;
        private string outputName;

        private double[][] inputData;
        private double[] outputData;

        private double[] results;



        private double SSe; // Error sum of squares
        private double SSr; // Regression sum of squares
        private double SSt; // Total sum of squares

        private double MSe; // Error mean sum of squares
        private double MSr; // Regression sum of squares
        private double MSt; // Total sum of squares

        private int DFe; // Error degrees of freedom
        private int DFr; // Regression degrees of freedom
        private int DFt; // Total degrees of freedom

        // Result related measures
        private double outputMean;
        private double stdError;
        private double rSquared;
        private double rAdjusted;
        private FTest ftest;
        private TTest ttest;
        private ZTest ztest;
        private ChiSquareTest chiSquareTest;

        // Coefficient measures
        internal double[] standardErrors;
        internal DoubleRange[] confidences;
        internal double confidencePercent = 0.95;
        internal TTest[] ttests;
        internal FTest[] ftests;

        private AnovaSourceCollection anovaTable;
        private LinearRegressionCoefficientCollection coefficientCollection;


        /// <summary>
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        public double[,] Source { get; private set; }


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
        ///   Gets the resulting values obtained
        ///   by the linear regression model.
        /// </summary>
        /// 
        public double[] Results
        {
            get { return results; }
        }


        /// <summary>
        ///   Gets the standard deviation of the errors. 
        /// </summary>
        /// 
        public double StandardError
        {
            get { return stdError; }
        }

        /// <summary>
        ///   Gets the coefficient of determination, as known as R²
        /// </summary>
        /// 
        public double RSquared
        {
            get { return rSquared; }
        }

        /// <summary>
        ///   Gets the adjusted coefficient of determination, as known as R² adjusted
        /// </summary>
        /// 
        public double RSquareAdjusted
        {
            get { return rAdjusted; }
        }

        /// <summary>
        ///   Gets a F-Test between the expected outputs and results.
        /// </summary>
        /// 
        public FTest FTest
        {
            get { return ftest; }
        }

        /// <summary>
        ///   Gets a Z-Test between the expected outputs and the results.
        /// </summary>
        /// 
        public ZTest ZTest
        {
            get { return ztest; }
        }

        /// <summary>
        ///   Gets a Chi-Square Test between the expected outputs and the results.
        /// </summary>
        /// 
        public ChiSquareTest ChiSquareTest
        {
            get { return chiSquareTest; }
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
        ///   Gets the Regression model created
        ///   and evaluated by this analysis.
        /// </summary>
        /// 
        public MultipleLinearRegression Regression
        {
            get { return this.regression; }
        }

        /// <summary>
        ///   Gets the value of each coefficient.
        /// </summary>
        /// 
        public double[] CoefficientValues
        {
            get { return this.regression.Coefficients; }
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
        ///   Gets the Confidence Intervals (C.I.)
        ///   for each coefficient found in the regression.
        /// </summary>
        /// 
        public DoubleRange[] Confidences
        {
            get { return this.confidences; }
        }

        /// <summary>
        ///   Gets the ANOVA table for the analysis.
        /// </summary>
        /// 
        public AnovaSourceCollection Table { get { return anovaTable; } }

        /// <summary>
        ///   Gets the collection of coefficients of the model.
        /// </summary>
        /// 
        public LinearRegressionCoefficientCollection Coefficients { get { return coefficientCollection; } }

        /// <summary>
        ///   Constructs a Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// <param name="intercept">True to use an intercept term, false otherwise. Default is false.</param>
        /// 
        public MultipleLinearRegressionAnalysis(double[][] inputs, double[] outputs, bool intercept = false)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");

            this.inputCount = inputs[0].Length;

            for (int i = 0; i < inputs.Length; i++)
                if (inputs[i].Length != inputCount)
                    throw new ArgumentException("All input vectors must have the same length.");

            // Store data sets
            this.inputData = inputs;
            this.outputData = outputs;



            // Create the linear regression
            regression = new MultipleLinearRegression(inputCount, intercept: intercept);

            // Create additional structures
            this.coefficientCount = regression.Coefficients.Length;
            this.standardErrors = new double[coefficientCount];
            this.confidences = new DoubleRange[coefficientCount];
            this.ftests = new FTest[coefficientCount];
            this.ttests = new TTest[coefficientCount];

            this.outputName = "Output";
            this.inputNames = new string[inputCount];
            for (int i = 0; i < inputNames.Length; i++)
                inputNames[i] = "Input " + i;


            // Create object-oriented structure to represent the analysis
            var coefs = new LinearRegressionCoefficient[coefficientCount];
            for (int i = 0; i < coefs.Length; i++)
                coefs[i] = new LinearRegressionCoefficient(this, i);
            this.coefficientCollection = new LinearRegressionCoefficientCollection(this, coefs);

            this.Source = inputs.ToMatrix();
        }

        /// <summary>
        ///   Constructs a Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// <param name="intercept">True to use an intercept term, false otherwise. Default is false.</param>
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="outputName">The name of the output variable.</param>
        /// 
        public MultipleLinearRegressionAnalysis(double[][] inputs, double[] outputs,
            String[] inputNames, String outputName, bool intercept = false)
            : this(inputs, outputs, intercept)
        {
            this.inputNames = inputNames;
            this.outputName = outputName;
        }

        /// <summary>
        ///   Computes the Multiple Linear Regression Analysis.
        /// </summary>
        /// 
        public void Compute()
        {
            int n = inputData.Length;
            int p = inputCount;

            SSt = SSe = outputMean = 0.0;

            // Compute the regression
            double[,] informationMatrix;
            regression.Regress(inputData, outputData,
                out informationMatrix);


            // Calculate mean of the expected outputs
            for (int i = 0; i < outputData.Length; i++)
                outputMean += outputData[i];
            outputMean /= outputData.Length;

            // Calculate actual outputs (results)
            results = new double[inputData.Length];
            for (int i = 0; i < inputData.Length; i++)
                results[i] = regression.Compute(inputData[i]);

            // Calculate SSe and SSt
            for (int i = 0; i < inputData.Length; i++)
            {
                double d;

                d = outputData[i] - results[i];
                SSe += d * d;

                d = outputData[i] - outputMean;
                SSt += d * d;
            }

            // Calculate SSr
            SSr = SSt - SSe;

            // Calculate R-Squared
            rSquared = (SSt != 0) ? 1.0 - (SSe / SSt) : 1.0;

            // Calculated Adjusted R-Squared
            if (rSquared == 1)
            {
                rAdjusted = 1;
            }
            else
            {
                if (n - p == 1)
                {
                    rAdjusted = double.NaN;
                }
                else
                {
                    rAdjusted = 1.0 - (1.0 - rSquared) * ((n - 1.0) / (n - p - 1.0));
                }
            }

            // Calculate Degrees of Freedom
            DFr = p;
            DFe = n - (p + 1);
            DFt = DFr + DFe;

            // Calculate Sum of Squares Mean
            MSe = SSe / DFe;
            MSr = SSr / DFr;
            MSt = SSt / DFt;

            // Calculate the F statistic
            ftest = new FTest(MSr / MSe, DFr, DFe);
            stdError = Math.Sqrt(MSe);


            // Create the ANOVA table
            List<AnovaVariationSource> table = new List<AnovaVariationSource>();
            table.Add(new AnovaVariationSource(this, "Regression", SSr, DFr, MSr, ftest));
            table.Add(new AnovaVariationSource(this, "Error", SSe, DFe, MSe, null));
            table.Add(new AnovaVariationSource(this, "Total", SSt, DFt, MSt, null));
            this.anovaTable = new AnovaSourceCollection(table);


            // Compute coefficient standard errors;
            standardErrors = new double[coefficientCount];
            for (int i = 0; i < standardErrors.Length; i++)
                standardErrors[i] = Math.Sqrt(MSe * informationMatrix[i, i]);


            // Compute coefficient tests
            for (int i = 0; i < regression.Coefficients.Length; i++)
            {
                double tStatistic = regression.Coefficients[i] / standardErrors[i];

                ttests[i] = new TTest(estimatedValue: regression.Coefficients[i],
                    standardError: standardErrors[i], degreesOfFreedom: DFe);

                ftests[i] = new FTest(tStatistic * tStatistic, 1, DFe);

                confidences[i] = ttests[i].GetConfidenceInterval(confidencePercent);
            }


            // Compute model performance tests
            ttest = new TTest(results, outputMean);
            ztest = new ZTest(results, outputMean);
            chiSquareTest = new ChiSquareTest(outputData, results, n - p - 1);
        }

        internal void setConfidenceIntervals(double percent)
        {
            this.confidencePercent = percent;
            for (int i = 0; i < ttests.Length; i++)
                confidences[i] = ttest.GetConfidenceInterval(percent);
        }
    }

    #region Support Classes
    /// <summary>
    /// <para>
    ///   Represents a Linear Regression coefficient found in the Multiple
    ///   Linear Regression Analysis allowing it to be bound to controls like
    ///   the DataGridView. </para>
    /// <para>
    ///   This class cannot be instantiated.</para>   
    /// </summary>
    /// 
    [Serializable]
    public class LinearRegressionCoefficient
    {

        private int index;
        private MultipleLinearRegressionAnalysis analysis;


        /// <summary>
        ///   Creates a regression coefficient representation.
        /// </summary>
        /// 
        /// <param name="analysis">The analysis to which this coefficient belongs.</param>
        /// <param name="index">The coefficient index.</param>
        /// 
        internal LinearRegressionCoefficient(MultipleLinearRegressionAnalysis analysis, int index)
        {
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
        public MultipleLinearRegressionAnalysis Analysis
        {
            get { return this.analysis; }
        }

        /// <summary>
        ///   Gets the name for the current coefficient.
        /// </summary>
        /// 
        public string Name
        {
            get
            {
                if (IsIntercept) return "Intercept";
                else return analysis.Inputs[index];
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this coefficient is an intercept term.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if this coefficient is the intercept; otherwise, <c>false</c>.
        /// </value>
        /// 
        [DisplayName("Intercept?")]
        public bool IsIntercept { get { return Analysis.regression.HasIntercept && index == Analysis.regression.Coefficients.Length - 1; } }

        /// <summary>
        ///   Gets the coefficient value.
        /// </summary>
        /// 
        [DisplayName("Value")]
        public double Value { get { return Analysis.regression.Coefficients[index]; } }

        /// <summary>
        ///   Gets the Standard Error for the current coefficient.
        /// </summary>
        /// 
        [DisplayName("Std. Error")]
        public double StandardError { get { return Analysis.StandardErrors[index]; } }

        /// <summary>
        ///   Gets the T-test performed for this coefficient.
        /// </summary>
        /// 
        public TTest TTest { get { return Analysis.ttests[index]; } }

        /// <summary>
        ///   Gets the F-test performed for this coefficient.
        /// </summary>
        /// 
        public FTest FTest { get { return Analysis.ftests[index]; } }

        /// <summary>
        ///   Gets the confidence interval (C.I.) for the current coefficient.
        /// </summary>
        /// 
        [Browsable(false)]
        public DoubleRange Confidence
        {
            get { return analysis.Confidences[index]; }
        }

        /// <summary>
        ///   Gets the upper limit for the confidence interval.
        /// </summary>
        /// 
        [DisplayName("Upper confidence limit")]
        public double ConfidenceUpper { get { return Analysis.Confidences[index].Max; } }

        /// <summary>
        ///   Gets the lower limit for the confidence interval.
        /// </summary>
        /// 
        [DisplayName("Lower confidence limit")]
        public double ConfidenceLower { get { return Analysis.Confidences[index].Min; } }

    }

    /// <summary>
    ///   Represents a Collection of Linear Regression Coefficients found in the 
    ///   <see cref="MultipleLinearRegressionAnalysis"/>. This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class LinearRegressionCoefficientCollection : ReadOnlyCollection<LinearRegressionCoefficient>
    {

        MultipleLinearRegressionAnalysis analysis;

        /// <summary>
        ///   Gets or sets the size of the confidence
        ///   intervals reported for the coefficients.
        ///   Default is 0.95.
        /// </summary>
        /// 
        public double ConfidencePercent
        {
            get { return analysis.confidencePercent; }
            set { analysis.setConfidenceIntervals(value); }
        }

        internal LinearRegressionCoefficientCollection(MultipleLinearRegressionAnalysis analysis,
            LinearRegressionCoefficient[] components)
            : base(components)
        {
            this.analysis = analysis;
        }
    }
    #endregion

}
