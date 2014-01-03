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
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using AForge;

    /// <summary>
    ///   Cox's Proportional Hazards Survival Analysis.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Proportional hazards models are a class of survival models in statistics. Survival models
    ///   relate the time that passes before some event occurs to one or more covariates that may be
    ///   associated with that quantity. In a proportional hazards model, the unique effect of a unit
    ///   increase in a covariate is multiplicative with respect to the hazard rate.</para>
    ///   
    /// <para>
    ///   For example, taking a drug may halve one's hazard rate for a stroke occurring, or, changing
    ///   the material from which a manufactured component is constructed may double its hazard rate 
    ///   for failure. Other types of survival models such as accelerated failure time models do not 
    ///   exhibit proportional hazards. These models could describe a situation such as a drug that 
    ///   reduces a subject's immediate risk of having a stroke, but where there is no reduction in 
    ///   the hazard rate after one year for subjects who do not have a stroke in the first year of 
    ///   analysis.</para>
    ///   
    /// <para>
    ///   This class uses the <see cref="ProportionalHazards"/> to extract more detailed
    ///   information about a given problem, such as confidence intervals, hypothesis tests
    ///   and performance measures. </para>
    ///   
    /// <para>
    ///   This class can also be bound to standard controls such as the 
    ///   <a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView</a>
    ///   by setting their DataSource property to the analysis' <see cref="Coefficients"/> property.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Consider the following example data, adapted from John C. Pezzullo's
    /// // example for his great Cox's proportional hazards model example in
    /// // JavaScript (http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html). 
    /// 
    /// // In this data, we have three columns. The first column denotes the
    /// // input variables for the problem. The second column, the survival
    /// // times. And the last one is the output of the experiment (if the
    /// // subject has died [1] or has survived [0]).
    /// 
    /// double[,] example =
    /// {
    ///     // input  time censor
    ///     {   50,    1,    0   },
    ///     {   70,    2,    1   },
    ///     {   45,    3,    0   },
    ///     {   35,    5,    0   },
    ///     {   62,    7,    1   },
    ///     {   50,   11,    0   },
    ///     {   45,    4,    0   },
    ///     {   57,    6,    0   },
    ///     {   32,    8,    0   },
    ///     {   57,    9,    1   },
    ///     {   60,   10,    1   },
    /// };
    /// 
    /// // First we will extract the input, times and outputs
    /// double[,] inputs = example.GetColumns(0);
    /// double[] times = example.GetColumn(1);
    /// int[] output = example.GetColumn(2).ToInt32();
    /// 
    /// // Now we can proceed and create the analysis
    /// var cox = new ProportionalHazardsAnalysis(inputs, times, output);
    /// 
    /// cox.Compute(); // compute the analysis
    /// 
    /// // Now we can show an analysis summary
    /// DataGridBox.Show(cox.Coefficients);
    /// </code>
    /// 
    /// <para>
    ///   The resulting table is shown below.</para>
    ///   <img src="..\images\cox-hazards.png" />
    /// 
    /// <code>
    /// // We can also investigate all parameters individually. For
    /// // example the coefficients values will be available at
        /// 
    /// double[] coef = cox.CoefficientValues;
    /// double[] stde = cox.StandardErrors;
    /// 
    /// // We can also obtain the hazards ratios
    /// double[] ratios = cox.HazardRatios;
    /// 
    /// // And other information such as the partial
    /// // likelihood, the deviance and also make 
    /// // hypothesis tests on the parameters
    /// 
    /// double partial = cox.LogLikelihood;
    /// double deviance = cox.Deviance;
    /// 
    /// // Chi-Square for whole model
    /// ChiSquareTest chi = cox.ChiSquare;
    /// 
    /// // Wald tests for individual parameters
    /// WaldTest wald = cox.Coefficients[0].Wald;
    /// 
    ///             
    /// // Finally, we can also use the model to predict
    /// // scores for new observations (without considering time)
    /// 
    /// double y1 = cox.Regression.Compute(new double[] { 63 });
    /// double y2 = cox.Regression.Compute(new double[] { 32 });
    /// 
    /// // Those scores can be interpreted by comparing then
    /// // to 1. If they are greater than one, the odds are
    /// // the patient will not survive. If the value is less
    /// // than one, the patient is likely to survive.
    /// 
    /// // The first value, y1, gives approximately 86.138,
    /// // while the second value, y2, gives about 0.00072.
    /// 
    /// 
    /// // We can also consider instant estimates for a given time:
    /// double p1 = cox.Regression.Compute(new double[] { 63 }, 2);
    /// double p2 = cox.Regression.Compute(new double[] { 63 }, 10);
    /// 
    /// // Here, p1 is the score after 2 time instants, with a 
    /// // value of 0.0656. The second value, p2, is the time
    /// // after 10 time instants, with a value of 6.2907.
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class ProportionalHazardsAnalysis : IRegressionAnalysis
    {
        private ProportionalHazards regression;

        private int inputCount;
        private double[] coefficients;
        private double[] standardErrors;
        private double[] hazardRatios;

        private WaldTest[] waldTests;
        private ChiSquareTest[] ratioTests;

        private DoubleRange[] confidences;

        private double deviance;
        private double logLikelihood;
        private ChiSquareTest chiSquare;

        private double[][] inputData;
        private double[] timeData;
        private int[] censorData;

        private string[] inputNames;
        private string timeName;
        private string censorName;

        private double[,] source;
        private double[] result;

        private HazardCoefficientCollection coefficientCollection;


        //---------------------------------------------


        #region Constructors

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="times">The output data for the analysis.</param>
        /// <param name="censor">The right-censoring indicative values.</param>
        /// 
        public ProportionalHazardsAnalysis(double[,] inputs, double[] times, int[] censor)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (times == null) throw new ArgumentNullException("times");

            if (inputs.GetLength(0) != times.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");

            initialize(inputs.ToArray(), times, censor);

            // Start regression using the Null Model
            this.regression = new ProportionalHazards(inputCount);
        }

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="times">The output data for the analysis.</param>
        /// <param name="censor">The right-censoring indicative values.</param>
        /// 
        public ProportionalHazardsAnalysis(double[][] inputs, double[] times, int[] censor)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (times == null) throw new ArgumentNullException("times");

            if (inputs.Length != times.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");

            initialize(inputs, times, censor);

            // Start regression using the Null Model
            this.regression = new ProportionalHazards(inputCount);
        }

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="times">The output, binary data for the analysis.</param>
        /// <param name="censor">The right-censoring indicative values.</param>
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="timeName">The name of the time variable.</param>
        /// <param name="censorName">The name of the event indication variable.</param>
        /// 
        public ProportionalHazardsAnalysis(double[][] inputs, double[] times, int[] censor,
            String[] inputNames, String timeName, String censorName)
            : this(inputs, times, censor)
        {
            this.inputNames = inputNames;
            this.timeName = timeName;
            this.censorName = censorName;
        }

        private void initialize(double[][] inputs, double[] outputs, int[] censor)
        {
            this.inputCount = inputs[0].Length;
            int coefficientCount = inputCount;

            // Store data sets
            this.inputData = inputs;
            this.timeData = outputs;
            this.censorData = censor;


            // Create additional structures
            this.coefficients = new double[coefficientCount];
            this.waldTests = new WaldTest[coefficientCount];
            this.standardErrors = new double[coefficientCount];
            this.hazardRatios = new double[coefficientCount];
            this.confidences = new DoubleRange[coefficientCount];
            this.ratioTests = new ChiSquareTest[coefficientCount];

            this.timeName = "Time";
            this.censorName = "Outcome";
            this.inputNames = new string[inputCount];
            for (int i = 0; i < inputNames.Length; i++)
                inputNames[i] = "Input " + i;

            // Create object-oriented structure to represent the analysis
            var logCoefs = new List<HazardCoefficient>(coefficientCount);
            for (int i = 0; i < coefficientCount; i++)
                logCoefs.Add(new HazardCoefficient(this, i));
            this.coefficientCollection = new HazardCoefficientCollection(logCoefs);

            this.source = inputs.ToMatrix();
        }

        #endregion


        //---------------------------------------------


        #region Public Properties

        /// <summary>
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        public double[,] Source
        {
            get { return source; }
        }

        /// <summary>
        ///   Gets the time passed until the event
        ///   occurred or until the observation was
        ///   censored.
        /// </summary>
        /// 
        public double[] TimeToEvent
        {
            get { return timeData; }
        }

        /// <summary>
        ///   Gets whether the event of
        ///   interest happened or not.
        /// </summary>
        /// 
        public int[] Events
        {
            get { return censorData; }
        }

        /// <summary>
        ///   Gets the dependent variable value
        ///   for each of the source input points.
        /// </summary>
        /// 
        public double[] Outputs
        {
            get { return censorData.ToDouble(); }
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
        ///   Gets the Proportional Hazards model created
        ///   and evaluated by this analysis.
        /// </summary>
        /// 
        public ProportionalHazards Regression
        {
            get { return regression; }
        }

        /// <summary>
        ///   Gets the collection of coefficients of the model.
        /// </summary>
        /// 
        public ReadOnlyCollection<HazardCoefficient> Coefficients
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
        public String[] InputNames
        {
            get { return inputNames; }
        }

        /// <summary>
        ///   Gets the name of the output variable for the model.
        /// </summary>
        /// 
        public String TimeName
        {
            get { return timeName; }
        }

        /// <summary>
        ///   Gets the name of event occurrence variable in the model.
        /// </summary>
        /// 
        public String EventName
        {
            get { return censorName; }
        }

        /// <summary>
        ///   Gets the Hazard Ratio for each coefficient
        ///   found during the proportional hazards.
        /// </summary>
        /// 
        public double[] HazardRatios
        {
            get { return this.hazardRatios; }
        }

        /// <summary>
        ///   Gets the Standard Error for each coefficient
        ///   found during the proportional hazards.
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
        public ChiSquareTest[] LikelihoodRatioTests
        {
            get { return this.ratioTests; }
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
        /// <param name="model">Another proportional hazards model.</param>
        /// 
        /// <returns>The Likelihood-Ratio between the two models.</returns>
        /// 
        public double GetLikelihoodRatio(ProportionalHazards model)
        {
            return regression.GetLogLikelihoodRatio(inputData, timeData, censorData, model);
        }

        /// <summary>
        ///   Computes the Proportional Hazards Analysis for an already computed regression.
        /// </summary>
        /// 
        public void Compute(ProportionalHazards regression, double limit = 1e-4, int maxIterations = 50)
        {
            this.regression = regression;

            computeInformation();

            if (inputCount > 0)
                computeInner(limit, maxIterations);
        }


        /// <summary>
        ///   Computes the Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="limit">
        ///   The difference between two iterations of the regression algorithm
        ///   when the algorithm should stop. If not specified, the value of
        ///   1e-4 will be used. The difference is calculated based on the largest
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
        public bool Compute(double limit = 1e-4, int maxIterations = 50)
        {
            ProportionalHazardsNewtonRaphson learning =
                new ProportionalHazardsNewtonRaphson(regression);

            Array.Clear(regression.Coefficients, 0, regression.Coefficients.Length);


            learning.Iterations = maxIterations;
            learning.Tolerance = limit;

            learning.Run(inputData, timeData, censorData);

            // Check if the full model has converged
            bool converged = learning.CurrentIteration < maxIterations;


            computeInformation();

            if (inputCount > 2)
                computeInner(limit, maxIterations);


            // Returns true if the full model has converged, false otherwise.
            return converged;
        }

        private void computeInner(double limit, int maxIterations)
        {
            // Perform likelihood-ratio tests against diminished nested models
            ProportionalHazards innerModel = new ProportionalHazards(inputCount - 1);
            ProportionalHazardsNewtonRaphson learning = new ProportionalHazardsNewtonRaphson(innerModel);

            for (int i = 0; i < inputCount; i++)
            {
                // Create a diminished inner model without the current variable
                double[][] data = inputData.RemoveColumn(i);

                System.Diagnostics.Trace.Assert(data[0].Length > 0);

                Array.Clear(innerModel.Coefficients, 0, inputCount - 1);

                learning.Iterations = maxIterations;
                learning.Tolerance = limit;

                learning.Run(data, timeData, censorData);


                double ratio = 2.0 * (logLikelihood - innerModel.GetPartialLogLikelihood(data, timeData, censorData));
                ratioTests[i] = new ChiSquareTest(ratio, 1);
            }
        }

        private void computeInformation()
        {
            // Store model information
            this.result = regression.Compute(inputData, timeData);
            this.deviance = regression.GetDeviance(inputData, timeData, censorData);
            this.logLikelihood = regression.GetPartialLogLikelihood(inputData, timeData, censorData);
            this.chiSquare = regression.ChiSquare(inputData, timeData, censorData);

            // Store coefficient information
            for (int i = 0; i < regression.Coefficients.Length; i++)
            {
                this.standardErrors[i] = regression.StandardErrors[i];

                this.waldTests[i] = regression.GetWaldTest(i);
                this.coefficients[i] = regression.Coefficients[i];
                this.confidences[i] = regression.GetConfidenceInterval(i);
                this.hazardRatios[i] = regression.GetHazardRatio(i);
            }
        }
        #endregion




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
    ///   Represents a Proportional Hazards Coefficient found in the Cox's Hazards model,
    ///   allowing it to be bound to controls like the DataGridView. This class cannot
    ///   be instantiated outside the <see cref="LogisticRegressionAnalysis"/>.
    /// </summary>
    /// 
    [Serializable]
    public class HazardCoefficient
    {
        private ProportionalHazardsAnalysis analysis;
        private int index;


        internal HazardCoefficient(ProportionalHazardsAnalysis analysis, int index)
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
                if (analysis.InputNames.Length == 0)
                    return String.Empty;

                return analysis.InputNames[index];
            }
        }

        /// <summary>
        ///   Gets the Odds ratio for the current coefficient.
        /// </summary>
        /// 
        [DisplayName("Hazard ratio")]
        public double HazardRatio
        {
            get { return analysis.HazardRatios[index]; }
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
        [DisplayName("Likelihood-Ratio p-value")]
        public ChiSquareTest LikelihoodRatio
        {
            get { return analysis.LikelihoodRatioTests[index]; }
        }


    }

    /// <summary>
    ///   Represents a collection of Hazard Coefficients found in the
    ///   <see cref="ProportionalHazardsAnalysis"/>. This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class HazardCoefficientCollection : ReadOnlyCollection<HazardCoefficient>
    {
        internal HazardCoefficientCollection(IList<HazardCoefficient> coefficients)
            : base(coefficients) { }
    }
    #endregion

}
