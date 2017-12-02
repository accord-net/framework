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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Accord.Compat;
    using System.Threading;

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
    /// <code source="Unit Tests\Accord.Tests.Statistics\Analysis\ProportionalHazardsAnalysisTest.cs" region="doc_learn_part1" lang="cs"/>
    /// <code source="Unit Tests\Accord.Tests.Statistics.VB\Analysis\ProportionalHazardsAnalysisTest.vb" region="doc_learn_part1" lang="vb"/>
    /// 
    /// <para>
    ///   The resulting table is shown below.</para>
    ///   <img src="..\images\cox-hazards.png" />
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Analysis\ProportionalHazardsAnalysisTest.cs" region="doc_learn_part2" />
    /// <code source="Unit Tests\Accord.Tests.Statistics.VB\Analysis\ProportionalHazardsAnalysisTest.vb" region="doc_learn_part2" lang="vb"/>
    /// </example>
    /// 
    /// <seealso cref="ProportionalHazards"/>
    /// <seealso cref="ProportionalHazardsNewtonRaphson"/>
    /// 
    [Serializable]
    public class ProportionalHazardsAnalysis : IRegressionAnalysis,
        ISupervisedLearning<ProportionalHazards, Tuple<double[], double>, int>
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
        private SurvivalOutcome[] censorData;

        private string[] inputNames;
        private string timeName;
        private string censorName;

        private double[,] source;
        private double[] result;

        private HazardCoefficientCollection coefficientCollection;

        private int iterations = 50;
        private double tolerance = 1e-5;

        private bool innerComputed = false;


        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="times">The output data for the analysis.</param>
        /// <param name="censor">The right-censoring indicative values.</param>
        /// 
        [Obsolete("Please do not pass data to this constructor, and use the Learn method instead.")]
        public ProportionalHazardsAnalysis(double[,] inputs, double[] times, int[] censor)
            : this(inputs, times, censor.To<SurvivalOutcome[]>())
        {
        }

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="times">The output data for the analysis.</param>
        /// <param name="censor">The right-censoring indicative values.</param>
        /// 
        [Obsolete("Please do not pass data to this constructor, and use the Learn method instead.")]
        public ProportionalHazardsAnalysis(double[][] inputs, double[] times, int[] censor)
            : this(inputs, times, censor.To<SurvivalOutcome[]>())
        {
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
        [Obsolete("Please do not pass data to this constructor, and use the Learn method instead.")]
        public ProportionalHazardsAnalysis(double[][] inputs, double[] times, int[] censor,
            String[] inputNames, String timeName, String censorName)
            : this(inputs, times, censor.To<SurvivalOutcome[]>(), inputNames, timeName, censorName)
        {
        }

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="times">The output data for the analysis.</param>
        /// <param name="censor">The right-censoring indicative values.</param>
        /// 
        [Obsolete("Please do not pass data to this constructor, and use the Learn method instead.")]
        public ProportionalHazardsAnalysis(double[,] inputs, double[] times, SurvivalOutcome[] censor)
        {
            // Initial argument checking
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (times == null)
                throw new ArgumentNullException("times");

            if (inputs.GetLength(0) != times.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");

            initialize(inputs.ToJagged(), times, censor);

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
        [Obsolete("Please do not pass data to this constructor, and use the Learn method instead.")]
        public ProportionalHazardsAnalysis(double[][] inputs, double[] times, SurvivalOutcome[] censor)
        {
            // Initial argument checking
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (times == null)
                throw new ArgumentNullException("times");

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
        [Obsolete("Please do not pass data to this constructor, and use the Learn method instead.")]
        public ProportionalHazardsAnalysis(double[][] inputs, double[] times, SurvivalOutcome[] censor,
            String[] inputNames, String timeName, String censorName)
            : this(inputs, times, censor)
        {
            this.inputNames = inputNames;
            this.timeName = timeName;
            this.censorName = censorName;
        }

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <param name="inputNames">The names of the input variables.</param>
        /// <param name="timeName">The name of the time variable.</param>
        /// <param name="censorName">The name of the event indication variable.</param>
        /// 
        public ProportionalHazardsAnalysis(String[] inputNames, String timeName, String censorName)
            : this()
        {
            this.inputNames = inputNames;
            this.timeName = timeName;
            this.censorName = censorName;
        }

        /// <summary>
        ///   Constructs a new Cox's Proportional Hazards Analysis.
        /// </summary>
        /// 
        public ProportionalHazardsAnalysis()
        {
        }

        private void initialize(double[][] inputs, double[] outputs, SurvivalOutcome[] censor)
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

            if (this.timeName == null)
                this.timeName = "Time";
            if (this.censorName == null)
                this.censorName = "Outcome";
            if (this.inputNames == null)
            {
                this.inputNames = new string[inputCount];
                for (int i = 0; i < inputNames.Length; i++)
                    inputNames[i] = "Input " + i;
            }

            // Create object-oriented structure to represent the analysis
            var logCoefs = new List<HazardCoefficient>(coefficientCount);
            for (int i = 0; i < coefficientCount; i++)
                logCoefs.Add(new HazardCoefficient(this, i));
            this.coefficientCollection = new HazardCoefficientCollection(logCoefs);

            this.source = inputs.ToMatrix();
        }



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
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
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
        public SurvivalOutcome[] Events
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
            get { return censorData.To<double[]>(); }
        }

        /// <summary>
        ///   Gets the resulting probabilities obtained
        ///   by the logistic regression model.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
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
        ///   Gets or sets the name of the input variables for the model.
        /// </summary>
        /// 
        public String[] InputNames
        {
            get { return inputNames; }
            set { InputNames = value; }
        }

        /// <summary>
        ///   Gets or sets the name of the output variable for the model.
        /// </summary>
        /// 
        public String TimeName
        {
            get { return timeName; }
            set { TimeName = value; }
        }

        /// <summary>
        ///   Gets or sets the name of event occurrence variable in the model.
        /// </summary>
        /// 
        public String EventName
        {
            get { return censorName; }
            set { censorName = value; }
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
        [Obsolete("Please use Learn(x, y) instead.")]
        public void Compute(ProportionalHazards regression)
        {
            this.regression = regression;

            computeInformation();

            innerComputed = false;
        }

        /// <summary>
        ///   Computes the Proportional Hazards Analysis.
        /// </summary>
        /// 
        /// <returns>
        ///   True if the model converged, false otherwise.
        /// </returns>
        /// 
        [Obsolete("Please use Learn(x, y) instead.")]
        public bool Compute()
        {
            return compute();
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
        [Obsolete("Please use Learn(x, y) instead.")]
        public bool Compute(double limit = 1e-4, int maxIterations = 50)
        {
            this.iterations = maxIterations;
            this.tolerance = limit;

            return compute();
        }




        private bool compute()
        {
            var learning = new ProportionalHazardsNewtonRaphson(regression);

            Array.Clear(regression.Coefficients, 0, regression.Coefficients.Length);


            learning.MaxIterations = Iterations;
            learning.Tolerance = Tolerance;

            learning.Learn(inputData, timeData, censorData);

            // Check if the full model has converged
            bool converged = learning.CurrentIteration < Iterations;


            computeInformation();

            innerComputed = false;

            // Returns true if the full model has converged, false otherwise.
            return converged;
        }

        private void computeInner()
        {
            if (inputCount <= 2)
                return;

            // Perform likelihood-ratio tests against diminished nested models
            var innerModel = new ProportionalHazards(inputCount - 1);
            var learning = createLearner(innerModel);

            for (int i = 0; i < inputCount; i++)
            {
                // Create a diminished inner model without the current variable
                double[][] data = inputData.RemoveColumn(i);

#if DEBUG
                if (data[0].Length == 0)
                    throw new Exception();
#endif

                Array.Clear(innerModel.Coefficients, 0, inputCount - 1);

                learning.MaxIterations = Iterations;
                learning.Tolerance = Tolerance;

                learning.Learn(data, timeData, censorData);


                double ratio = 2.0 * (logLikelihood - innerModel.GetPartialLogLikelihood(data, timeData, censorData));
                ratioTests[i] = new ChiSquareTest(ratio, 1);
            }

            innerComputed = true;
        }

        private void computeInformation()
        {
            // Store model information
#pragma warning disable 612, 618
            this.result = regression.Compute(inputData, timeData);
#pragma warning restore 612, 618
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

#pragma warning disable 612, 618
        [Obsolete("Please use the Learn method instead.")]
        void IAnalysis.Compute()
        {
            Compute();
        }
#pragma warning restore 612, 618


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="inputs">The model inputs.</param>
        /// <param name="censor">The output (event) associated with each input vector.</param>
        /// <param name="time">The time-to-event for the non-censored training samples.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="censor" /> given <paramref name="inputs" /> and <paramref name="time" />.
        /// </returns>
        public ProportionalHazards Learn(double[][] inputs, double[] time, SurvivalOutcome[] censor, double[] weights = null)
        {
            var learning = createLearner(regression);
            this.regression = learning.Learn(inputs, time, censor, weights);
            initialize(inputs, time, censor);
            return store();
        }



        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="inputs">The model inputs.</param>
        /// <param name="censor">The output (event) associated with each input vector.</param>
        /// <param name="time">The time-to-event for the non-censored training samples.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="censor" /> given <paramref name="inputs" /> and <paramref name="time" />.
        /// </returns>
        public ProportionalHazards Learn(double[][] inputs, double[] time, int[] censor, double[] weights = null)
        {
            var learning = createLearner(regression);
            this.regression = learning.Learn(inputs, time, censor, weights);
            initialize(inputs, time, censor.To<SurvivalOutcome[]>());
            return store();
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
        public ProportionalHazards Learn(Tuple<double[], double>[] x, int[] y, double[] weights = null)
        {
            var learning = createLearner(regression);
            this.regression = learning.Learn(x, y, weights);
            initialize(x.Apply(a => a.Item1), x.Apply(a => a.Item2), y.To<SurvivalOutcome[]>());
            return store();
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
        public ProportionalHazards Learn(Tuple<double[], double>[] x, SurvivalOutcome[] y, double[] weights = null)
        {
            var learning = createLearner(regression);
            this.regression = learning.Learn(x, y, weights);
            initialize(x.Apply(a => a.Item1), x.Apply(a => a.Item2), y);
            return store();
        }


        private ProportionalHazardsNewtonRaphson createLearner(ProportionalHazards model)
        {
            return new ProportionalHazardsNewtonRaphson()
            {
                Model = model,
                MaxIterations = Iterations,
                Tolerance = Tolerance,
                Token = Token
            };
        }

        private ProportionalHazards store()
        {
            computeInformation();
            innerComputed = false;
            return regression;
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
