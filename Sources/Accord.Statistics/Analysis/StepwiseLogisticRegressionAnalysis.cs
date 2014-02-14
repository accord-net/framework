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
    using System.Text;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using AForge;

    /// <summary>
    ///   Backward Stepwise Logistic Regression Analysis.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Backward Stepwise regression is an exploratory analysis procedure,
    ///   where the analysis begins with a full (saturated) model and at each step
    ///   variables are eliminated from the model in a iterative fashion.</para>
    /// <para>
    ///   Significance tests are performed after each removal to track which of
    ///   the variables can be discarded safely without implying in degradation.
    ///   When no more variables can be removed from the model without causing
    ///   a significant loss in the model likelihood, the method can stop.</para>  
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
    ///     new double[] { 55,    0   },  // 1
    ///     new double[] { 28,    0   },  // 2
    ///     new double[] { 65,    1   },  // 3
    ///     new double[] { 46,    0   },  // 4
    ///     new double[] { 86,    1   },  // 5
    ///     new double[] { 56,    1   },  // 6
    ///     new double[] { 85,    0   },  // 7
    ///     new double[] { 33,    0   },  // 8
    ///     new double[] { 21,    1   },  // 9
    ///     new double[] { 42,    1   },  // 10
    ///     new double[] { 33,    0   },  // 11
    ///     new double[] { 20,    1   },  // 12
    ///     new double[] { 43,    1   },  // 13
    ///     new double[] { 31,    1   },  // 14
    ///     new double[] { 22,    1   },  // 15
    ///     new double[] { 43,    1   },  // 16
    ///     new double[] { 46,    0   },  // 17
    ///     new double[] { 86,    1   },  // 18
    ///     new double[] { 56,    1   },  // 19
    ///     new double[] { 55,    0   },  // 20
    /// };
    /// 
    /// // Additionally, we also have information about whether
    /// // or not they those patients had lung cancer. The array
    /// // below gives 0 for those who did not, and 1 for those
    /// // who did.
    /// 
    /// double[] output =
    /// {
    ///     0, 0, 0, 1, 1, 1, 0, 0, 0, 1,
    ///     0, 1, 1, 1, 1, 1, 0, 1, 1, 0
    /// };
    /// 
    /// 
    /// // Create a Stepwise Logistic Regression analysis
    /// var regression = new StepwiseLogisticRegressionAnalysis(inputs, output,
    ///     new[] { "Age", "Smoking" }, "Cancer");
    /// 
    /// regression.Compute(); // compute the analysis.
    /// 
    /// // The full model will be stored in the complete property:
    /// StepwiseLogisticRegressionModel full = regression.Complete;
    /// 
    /// // The best model will be stored in the current property:
    /// StepwiseLogisticRegressionModel best = regression.Current;
    /// 
    /// // Let's check the full model results
    /// DataGridBox.Show(full.Coefficients); 
    /// 
    /// // We can see only the Smoking variable is statistically significant.
    /// // This is an indication the Age variable could be discarded from
    /// // the model.
    /// 
    /// // And check the best inner model result
    /// DataGridBox.Show(best.Coefficients);
    /// 
    /// // This is the best nested model found. This model only has the 
    /// // Smoking variable, which is still significant. Since no other
    /// // variables can be dropped, this is the best final model.
    /// 
    /// // The variables used in the current best model are
    /// string[] inputVariableNames = best.Inputs; // Smoking
    /// 
    /// // The best model likelihood ratio p-value is
    /// ChiSquareTest test = best.ChiSquare; // {0.816990081334823}
    /// 
    /// // so the model is distinguishable from a null model. We can also
    /// // query the other nested models by checking the Nested property:
    /// 
    /// DataGridBox.Show(regression.Nested);
    /// 
    /// // Finally, we can also use the analysis to classify a new patient
    /// double y = regression.Current.Regression.Compute(new double[] { 1 });
    /// 
    /// // For a smoking person, the answer probability is approximately 83%.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="LogisticRegressionAnalysis"/>
    /// 
    [Serializable]
    public class StepwiseLogisticRegressionAnalysis : IRegressionAnalysis
    {

        private double[][] inputData;
        private double[] outputData;

        private string[] inputNames;
        private string outputName;

        private double[,] source;
        private double[] result;
        private int[] resultVariables;


        private StepwiseLogisticRegressionModel currentModel;
        private StepwiseLogisticRegressionModel completeModel;
        private StepwiseLogisticRegressionModelCollection nestedModelCollection;
        private double fullLikelihood;

        private double threshold = 0.15;

        // Fitting parameters
        private int maxIterations = 100;
        private double limit = 10e-4;


        //---------------------------------------------


        #region Constructors
        /// <summary>
        ///   Constructs a Stepwise Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// 
        public StepwiseLogisticRegressionAnalysis(double[][] inputs, double[] outputs)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");


            this.inputData = inputs;
            this.outputData = outputs;

            this.inputNames = new String[inputs[0].Length];
            for (int i = 0; i < this.inputNames.Length; i++)
                inputNames[i] = "Input " + i;
            this.outputName = "Output";

            this.source = inputs.ToMatrix();
        }

        /// <summary>
        ///   Constructs a Stepwise Logistic Regression Analysis.
        /// </summary>
        /// 
        /// <param name="inputs">The input data for the analysis.</param>
        /// <param name="outputs">The output data for the analysis.</param>
        /// <param name="inputNames">The names for the input variables.</param>
        /// <param name="outputName">The name for the output variable.</param>
        /// 
        public StepwiseLogisticRegressionAnalysis(double[][] inputs, double[] outputs, String[] inputNames, String outputName)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new ArgumentException("The number of rows in the input array must match the number of given outputs.");


            this.inputData = inputs;
            this.outputData = outputs;

            this.inputNames = inputNames;
            this.outputName = outputName;

            this.source = inputs.ToMatrix();
        }
        #endregion


        //---------------------------------------------


        #region Properties
        /// <summary>
        ///   Source data used in the analysis.
        /// </summary>
        /// 
        public double[,] Source
        {
            get { return source; }
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
        ///   by the most likely logistic regression model.
        /// </summary>
        /// 
        public double[] Result
        {
            get { return result; }
        }

        /// <summary>
        ///   Gets the current best nested model.
        /// </summary>
        /// 
        public StepwiseLogisticRegressionModel Current
        {
            get { return this.currentModel; }
        }

        /// <summary>
        ///   Gets the full model.
        /// </summary>
        /// 
        public StepwiseLogisticRegressionModel Complete
        {
            get { return this.completeModel; }
        }

        /// <summary>
        ///   Gets the collection of nested models obtained after 
        ///   a step of the backward stepwise procedure.
        /// </summary>
        /// 
        public StepwiseLogisticRegressionModelCollection Nested
        {
            get { return nestedModelCollection; }
        }

        /// <summary>
        ///   Gets the name of the input variables.
        /// </summary>
        /// 
        public String[] Inputs
        {
            get { return this.inputNames; }
        }

        /// <summary>
        ///   Gets the name of the output variables.
        /// </summary>
        /// 
        public String Output
        {
            get { return this.outputName; }
        }

        /// <summary>
        ///   Gets or sets the significance threshold used to
        ///   determine if a nested model is significant or not.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }


        /// <summary>
        ///   Gets the final set of input variables indices
        ///   as selected by the stepwise procedure.
        /// </summary>
        /// 
        public int[] Variables
        {
            get { return this.resultVariables; }
        }
        #endregion


        //---------------------------------------------


        /// <summary>
        ///   Computes the Stepwise Logistic Regression.
        /// </summary>
        /// 
        public void Compute()
        {
            int changed;
            do
            {
                changed = DoStep();

            } while (changed != -1);

            
            // Get the final variable selection
            resultVariables = currentModel.Variables;

            double[][] resultInput = inputData
                .Submatrix(null, resultVariables);

            // Compute the final model output probabilities
            result = currentModel.Regression.Compute(resultInput);
        }

        /// <summary>
        ///   Computes one step of the Stepwise Logistic Regression Analysis.
        /// </summary>
        /// <returns>
        ///   Returns the index of the variable discarded in the step or -1
        ///   in case no variable could be discarded.
        /// </returns>
        /// 
        public int DoStep()
        {
            ChiSquareTest[] tests = null;

            // Check if we are performing the first step
            if (currentModel == null)
            {
                // This is the first step. We should create the full model.
                int inputCount = inputData[0].Length;
                LogisticRegression regression = new LogisticRegression(inputCount);
                int[] variables = Matrix.Indices(0, inputCount);
                fit(regression, inputData, outputData);
                ChiSquareTest test = regression.ChiSquare(inputData, outputData);
                fullLikelihood = regression.GetLogLikelihood(inputData, outputData);

                if (Double.IsNaN(fullLikelihood))
                {
                    throw new ConvergenceException(
                        "Perfect separation detected. Please rethink the use of logistic regression.");
                }

                tests = new ChiSquareTest[regression.Coefficients.Length];
                currentModel = new StepwiseLogisticRegressionModel(this, regression, variables, test, tests);
                completeModel = currentModel;
            }


            // Verify first if a variable reduction is possible
            if (currentModel.Regression.Inputs == 1)
                return -1; // cannot reduce further


            // Now go and create the diminished nested models
            var nestedModels = new StepwiseLogisticRegressionModel[currentModel.Regression.Inputs];
            for (int i = 0; i < nestedModels.Length; i++)
            {
                // Create a diminished nested model without the current variable
                LogisticRegression regression = new LogisticRegression(currentModel.Regression.Inputs - 1);
                int[] variables = currentModel.Variables.RemoveAt(i);
                double[][] subset = inputData.Submatrix(0, inputData.Length - 1, variables);
                fit(regression, subset, outputData);

                // Check the significance of the nested model
                double logLikelihood = regression.GetLogLikelihood(subset, outputData);
                double ratio = 2.0 * (fullLikelihood - logLikelihood);
                ChiSquareTest test = new ChiSquareTest(ratio, inputNames.Length - variables.Length) { Size = threshold };

                if (tests != null)
                    tests[i + 1] = test;

                // Store the nested model
                nestedModels[i] = new StepwiseLogisticRegressionModel(this, regression, variables, test, null);
            }

            // Select the model with the highest p-value
            double pmax = 0; int imax = -1;
            for (int i = 0; i < nestedModels.Length; i++)
            {
                if (nestedModels[i].ChiSquare.PValue >= pmax)
                {
                    imax = i;
                    pmax = nestedModels[i].ChiSquare.PValue;
                }
            }

            // Create the read-only nested model collection
            this.nestedModelCollection = new StepwiseLogisticRegressionModelCollection(nestedModels);


            // If the model with highest p-value is not significant,
            if (imax >= 0 && pmax > threshold)
            {
                // Then this means the variable can be safely discarded from the full model
                int removed = currentModel.Variables[imax];

                // Our diminished nested model will become our next full model.
                this.currentModel = nestedModels[imax];

                // Finally, return the index of the removed variable
                return removed;
            }
            else
            {
                // Else we can not safely remove any variable from the model.
                return -1;
            }
        }


        /// <summary>
        ///   Fits a logistic regression model to data until convergence.
        /// </summary>
        /// 
        private bool fit(LogisticRegression regression, double[][] input, double[] output)
        {
            IterativeReweightedLeastSquares irls =
                new IterativeReweightedLeastSquares(regression);

            double delta;
            int iteration = 0;

            do // learning iterations until convergence
            {
                delta = irls.Run(input, output);
                iteration++;

            } while (delta > limit && iteration < maxIterations);

            // Check if the full model has converged
            return iteration <= maxIterations;
        }

    }

    /// <summary>
    ///   Stepwise Logistic Regression Nested Model.
    /// </summary>
    /// 
    [Serializable]
    public class StepwiseLogisticRegressionModel
    {

        /// <summary>
        ///   Gets information about the regression model
        ///   coefficients in a object-oriented structure.
        /// </summary>
        /// 
        [Browsable(false)]
        public NestedLogisticCoefficientCollection Coefficients { get; private set; }


        /// <summary>
        ///   Gets the Stepwise Logistic Regression Analysis
        ///   from which this model belongs to.
        /// </summary>
        /// 
        [Browsable(false)]
        public StepwiseLogisticRegressionAnalysis Analysis { get; private set; }

        /// <summary>
        ///   Gets the regression model.
        /// </summary>
        /// 
        [Browsable(false)]
        public LogisticRegression Regression { get; private set; }

        /// <summary>
        ///   Gets the subset of the original variables used by the model.
        /// </summary>
        /// 
        [Browsable(false)]
        public int[] Variables { get; private set; }

        /// <summary>
        ///   Gets the name of the variables used in
        ///   this model combined as a single string.
        /// </summary>
        /// 
        [DisplayName("Inputs")]
        public string Names { get; private set; }

        /// <summary>
        ///   Gets the Chi-Square Likelihood Ratio test for the model.
        /// </summary>
        /// 
        [DisplayName("Likelihood-ratio")]
        public ChiSquareTest ChiSquare { get; private set; }

        /// <summary>
        ///   Gets the subset of the original variables used by the model.
        /// </summary>
        /// 
        [Browsable(false)]
        public string[] Inputs { get; private set; }

        /// <summary>
        ///   Gets the Odds Ratio for each coefficient
        ///   found during the logistic regression.
        /// </summary>
        /// 
        [Browsable(false)]
        public double[] OddsRatios { get; private set; }

        /// <summary>
        ///   Gets the Standard Error for each coefficient
        ///   found during the logistic regression.
        /// </summary>
        /// 
        [Browsable(false)]
        public double[] StandardErrors { get; internal set; }

        /// <summary>
        ///   Gets the Wald Tests for each coefficient.
        /// </summary>
        /// 
        [Browsable(false)]
        public WaldTest[] WaldTests { get; internal set; }

        /// <summary>
        ///   Gets the value of each coefficient.
        /// </summary>
        /// 
        [Browsable(false)]
        public double[] CoefficientValues { get; private set; }

        /// <summary>
        ///   Gets the 95% Confidence Intervals (C.I.)
        ///   for each coefficient found in the regression.
        /// </summary>
        /// 
        [Browsable(false)]
        public DoubleRange[] Confidences { get; private set; }

        /// <summary>
        ///   Gets the Likelihood-Ratio Tests for each coefficient.
        /// </summary>
        /// 
        [Browsable(false)]
        public ChiSquareTest[] LikelihoodRatioTests { get; private set; }

        /// <summary>
        ///   Constructs a new Logistic regression model.
        /// </summary>
        /// 
        internal StepwiseLogisticRegressionModel(StepwiseLogisticRegressionAnalysis analysis, LogisticRegression regression,
            int[] variables, ChiSquareTest chiSquare, ChiSquareTest[] tests)
        {
            this.Analysis = analysis;
            this.Regression = regression;

            int coefficientCount = regression.Coefficients.Length;

            this.Inputs = analysis.Inputs.Submatrix(variables);
            this.ChiSquare = chiSquare;
            this.LikelihoodRatioTests = tests;
            this.Variables = variables;
            this.StandardErrors = new double[coefficientCount];
            this.WaldTests = new WaldTest[coefficientCount];
            this.CoefficientValues = new double[coefficientCount];
            this.Confidences = new DoubleRange[coefficientCount];
            this.OddsRatios = new double[coefficientCount];

            // Store coefficient information
            for (int i = 0; i < regression.Coefficients.Length; i++)
            {
                this.StandardErrors[i] = regression.StandardErrors[i];
                this.WaldTests[i] = regression.GetWaldTest(i);
                this.CoefficientValues[i] = regression.Coefficients[i];
                this.Confidences[i] = regression.GetConfidenceInterval(i);
                this.OddsRatios[i] = regression.GetOddsRatio(i);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Inputs.Length; i++)
            {
                sb.Append(Inputs[i]);
                if (i < Inputs.Length - 1)
                    sb.Append(", ");
            }
            this.Names = sb.ToString();


            var logCoefs = new List<NestedLogisticCoefficient>(coefficientCount);
            for (int i = 0; i < coefficientCount; i++)
                logCoefs.Add(new NestedLogisticCoefficient(this, i));
            this.Coefficients = new NestedLogisticCoefficientCollection(logCoefs);
        }

    }

    /// <summary>
    ///   Stepwise Logistic Regression Nested Model collection.
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class StepwiseLogisticRegressionModelCollection :
        ReadOnlyCollection<StepwiseLogisticRegressionModel>
    {
        internal StepwiseLogisticRegressionModelCollection(StepwiseLogisticRegressionModel[] models)
            : base(models) { }
    }

    /// <summary>
    ///   Represents a Logistic Regression Coefficient found in the Logistic Regression,
    ///   allowing it to be bound to controls like the DataGridView. This class cannot
    ///   be instantiated outside the <see cref="LogisticRegressionAnalysis"/>.
    /// </summary>
    /// 
    [Serializable]
    public class NestedLogisticCoefficient
    {
        private StepwiseLogisticRegressionModel analysis;
        private int index;


        internal NestedLogisticCoefficient(StepwiseLogisticRegressionModel analysis, int index)
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
        [DisplayName("Likelihood-Ratio p-value")]
        public ChiSquareTest LikelihoodRatio
        {
            get
            {
                if (analysis.LikelihoodRatioTests == null)
                    return null;
                return analysis.LikelihoodRatioTests[index];
            }
        }

    }

    /// <summary>
    ///   Represents a collection of Logistic Coefficients found in the
    ///   <see cref="LogisticRegressionAnalysis"/>. This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class NestedLogisticCoefficientCollection : ReadOnlyCollection<NestedLogisticCoefficient>
    {
        internal NestedLogisticCoefficientCollection(IList<NestedLogisticCoefficient> coefficients)
            : base(coefficients) { }
    }
}
