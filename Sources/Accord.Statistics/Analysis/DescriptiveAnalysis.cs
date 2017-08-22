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
    using Accord.Collections;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.ComponentModel;
    using Accord.Compat;

    /// <summary>
    ///   Descriptive statistics analysis.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Descriptive statistics are used to describe the basic features of the data
    ///   in a study. They provide simple summaries about the sample and the measures.
    ///   Together with simple graphics analysis, they form the basis of virtually
    ///   every quantitative analysis of data.</para>
    ///   
    /// <para>
    ///   This class can also be bound to standard controls such as the 
    ///   <a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView</a>
    ///   by setting their DataSource property to the analysis' <see cref="Measures"/> property.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///        Wikipedia, The Free Encyclopedia. Descriptive Statistics. Available on:
    ///        http://en.wikipedia.org/wiki/Descriptive_statistics </description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Analysis\DescriptiveAnalysisTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="Statistics.Tools"/>
    /// <seealso cref="DescriptiveMeasures"/>
    ///
    [Serializable]
#pragma warning disable 612, 618
    public class DescriptiveAnalysis : IMultivariateAnalysis,
        IDescriptiveLearning<DescriptiveAnalysis, double[]>
#pragma warning restore 612, 618
    {

        private int samples;
        private int variables;

        private double[] sums;
        private double[] means;
        private double[] standardDeviations;
        private double[] variances;
        private double[] medians;
        private double[] modes;
        private int[] distinct;

        private string[] columnNames;

        private QuantileMethod quantileMethod = QuantileMethod.Default;
        private DoubleRange[] ranges;
        private DoubleRange[] quartiles;
        private DoubleRange[] innerFences;
        private DoubleRange[] outerFences;
        private DoubleRange[] confidence;
        private DoubleRange[] deviance;

        private double[] kurtosis;
        private Double[] skewness;
        private double[] standardErrors;


        private double[,] covarianceMatrix;
        private double[,] correlationMatrix;

        private double[,] zScores;
        private double[,] dScores;

        private double[,] sourceMatrix;
        private double[][] sourceArray;

        private DescriptiveMeasureCollection measuresCollection;

        private bool lazy = true;

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        public DescriptiveAnalysis()
        {
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="columnNames">Names for the analyzed variables.</param>
        /// 
        public DescriptiveAnalysis(string[] columnNames)
        {
            if (columnNames == null)
                throw new ArgumentNullException("columnNames");

            init(null, null, columnNames);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
        [Obsolete("Please call the Learn() method passing the data to be analyzed.")]
        public DescriptiveAnalysis(double[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            double[,] matrix = new double[data.Length, 1];

            System.Buffer.BlockCopy(data, 0, matrix, 0, data.Length * sizeof(double));

            init(matrix, null, null);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
        [Obsolete("Please call the Learn() method passing the data to be analyzed.")]
        public DescriptiveAnalysis(double[,] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            init(data, null, null);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="columnNames">Names for the analyzed variables.</param>
        /// 
        [Obsolete("Please pass only columnNames and call the Learn() method passing the data to be analyzed.")]
        public DescriptiveAnalysis(double[,] data, string[] columnNames)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (columnNames == null)
                throw new ArgumentNullException("columnNames");

            init(data, null, columnNames);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
        [Obsolete("Please call the Learn() method passing the data to be analyzed.")]
        public DescriptiveAnalysis(double[][] data)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            init(null, data, null);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="columnNames">Names for the analyzed variables.</param>
        /// 
        [Obsolete("Please pass only columnNames and call the Learn() method passing the data to be analyzed.")]
        public DescriptiveAnalysis(double[][] data, string[] columnNames)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (columnNames == null)
                throw new ArgumentNullException("columnNames");

            init(null, data, columnNames);
        }

        private void init(double[,] matrix, double[][] array, string[] columnNames)
        {
            this.sourceMatrix = matrix;
            this.sourceArray = array;
            this.columnNames = columnNames;

            if (matrix != null)
            {
                this.samples = matrix.GetLength(0);
                this.variables = matrix.GetLength(1);
            }
            else if (array != null)
            {
                this.samples = array.Length;
                this.variables = array[0].Length;
            }
            else
            {
                return;
            }

            // Create object-oriented structure to access data
            DescriptiveMeasures[] measures = new DescriptiveMeasures[variables];
            for (int i = 0; i < measures.Length; i++)
                measures[i] = new DescriptiveMeasures(this, i);
            this.measuresCollection = new DescriptiveMeasureCollection(measures);
        }


        /// <summary>
        ///   Computes the analysis using given source data and parameters.
        /// </summary>
        /// 
        [Obsolete("Please use Learn() instead.")]
        public void Compute()
        {
            // Clear analysis
            reset();

            this.sums = Sums;
            this.means = Means;
            this.standardDeviations = StandardDeviations;
            this.ranges = Ranges;
            this.kurtosis = Kurtosis;
            this.skewness = Skewness;
            this.medians = Medians;
            this.modes = Modes;
            this.variances = Variances;
            this.standardErrors = StandardErrors;
            this.distinct = Distinct;
            this.quartiles = Quartiles;
            this.innerFences = InnerFences;
            this.outerFences = OuterFences;

            // Mean centered and standardized data
            this.dScores = DeviationScores;
            this.zScores = StandardScores;

            // Covariance and correlation
            this.covarianceMatrix = CovarianceMatrix;
            this.correlationMatrix = CorrelationMatrix;

            this.confidence = Confidence;
            this.deviance = Deviance;
        }

        private void reset()
        {
            this.sums = null;
            this.means = null;
            this.standardDeviations = null;
            this.ranges = null;
            this.kurtosis = null;
            this.skewness = null;
            this.medians = null;
            this.modes = null;
            this.variances = null;
            this.standardErrors = null;
            this.distinct = null;
            this.dScores = null;
            this.zScores = null;
            this.covarianceMatrix = null;
            this.correlationMatrix = null;
            this.deviance = null;
            this.quartiles = null;
            this.innerFences = null;
            this.outerFences = null;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <returns>
        /// A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.
        /// </returns>
        public DescriptiveAnalysis Learn(double[][] x)
        {
            reset();

            init(null, x, columnNames);

            if (!lazy)
            {
                this.sums = Sums;
                this.means = Means;
                this.standardDeviations = StandardDeviations;
                this.ranges = Ranges;
                this.kurtosis = Kurtosis;
                this.skewness = Skewness;
                this.medians = Medians;
                this.modes = Modes;
                this.variances = Variances;
                this.standardErrors = StandardErrors;
                this.distinct = Distinct;
                this.quartiles = Quartiles;
                this.innerFences = InnerFences;
                this.outerFences = OuterFences;

                // Mean centered and standardized data
                this.dScores = DeviationScores;
                this.zScores = StandardScores;

                // Covariance and correlation
                this.covarianceMatrix = CovarianceMatrix;
                this.correlationMatrix = CorrelationMatrix;

                this.confidence = Confidence;
                this.deviance = Deviance;

                this.sourceArray = null;
                this.sourceMatrix = null;
            }

            return this;
        }

        /// <summary>
        ///   Gets or sets whether the properties of this class should
        ///   be computed only when necessary. If set to true, a copy
        ///   of the input data will be maintained inside an instance
        ///   of this class, using more memory.
        /// </summary>
        /// 
        public bool Lazy
        {
            get { return lazy; }
            set { lazy = value; }
        }

        /// <summary>
        ///   Gets the source matrix from which the analysis was run.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Source
        {
            get
            {
                if (this.sourceMatrix == null)
                    sourceMatrix = sourceArray.ToMatrix();
                return sourceMatrix;
            }
        }

        /// <summary>
        ///   Gets the source matrix from which the analysis was run.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[][] Array
        {
            get
            {
                if (this.sourceArray == null)
                    sourceArray = sourceMatrix.ToJagged();
                return sourceArray;
            }
        }

        /// <summary>
        ///   Gets or sets the method to be used when computing quantiles (median and quartiles).
        /// </summary>
        /// 
        /// <value>The quantile method.</value>
        /// 
        public QuantileMethod QuantileMethod
        {
            get { return quantileMethod; }
            set { quantileMethod = value; }
        }

        /// <summary>
        ///   Gets the column names from the variables in the data.
        /// </summary>
        /// 
        public String[] ColumnNames
        {
            get
            {
                if (columnNames == null)
                {
                    columnNames = new string[variables];
                    for (int i = 0; i < columnNames.Length; i++)
                        columnNames[i] = "Column " + i;
                }

                return this.columnNames;
            }
            set
            {
                this.columnNames = value;
            }
        }

        /// <summary>
        ///   Gets the mean subtracted data.
        /// </summary>
        /// 
        public double[,] DeviationScores
        {
            get
            {
                if (this.dScores == null)
                {
                    if (sourceMatrix != null)
                        this.dScores = sourceMatrix.Center(Means, inPlace: false);
                    else this.dScores = sourceArray.Center(Means, inPlace: false).ToMatrix();
                }

                return this.dScores;
            }
        }

        /// <summary>
        /// Gets the mean subtracted and deviation divided data. Also known as Z-Scores.
        /// </summary>
        /// 
        public double[,] StandardScores
        {
            get
            {
                if (this.zScores == null)
                    this.zScores = Statistics.Tools.Standardize(DeviationScores, StandardDeviations, inPlace: false);
                return this.zScores;
            }
        }

        /// <summary>
        ///   Gets the Covariance Matrix
        /// </summary>
        /// 
        public double[,] CovarianceMatrix
        {
            get
            {
                if (covarianceMatrix == null)
                {
                    if (sourceMatrix != null)
                        covarianceMatrix = sourceMatrix.Covariance(Means);
                    else covarianceMatrix = sourceArray.Covariance(Means).ToMatrix();
                }

                return covarianceMatrix;
            }
        }

        /// <summary>
        ///   Gets the Correlation Matrix
        /// </summary>
        /// 
        public double[,] CorrelationMatrix
        {
            get
            {
                if (correlationMatrix == null)
                {
                    if (sourceMatrix != null)
                        correlationMatrix = sourceMatrix.Correlation(Means, StandardDeviations);
                    else correlationMatrix = sourceArray.Correlation(Means, StandardDeviations).ToMatrix();
                }

                return correlationMatrix;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Mean of each data column.
        /// </summary>
        /// 
        public double[] Means
        {
            get
            {
                if (means == null)
                {
                    if (sourceMatrix != null)
                        means = sourceMatrix.Mean(Sums);
                    else means = sourceArray.Mean(Sums);
                }
                return means;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Standard Deviation of each data column.
        /// </summary>
        /// 
        public double[] StandardDeviations
        {
            get
            {
                if (standardDeviations == null)
                {
                    if (sourceMatrix != null)
                        standardDeviations = sourceMatrix.StandardDeviation(Means);
                    else standardDeviations = sourceArray.StandardDeviation(Means);
                }

                return standardDeviations;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Standard Error of the Mean of each data column.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get
            {
                if (standardErrors == null)
                    standardErrors = Statistics.Measures.StandardError(samples, StandardDeviations);

                return standardErrors;
            }
        }

        /// <summary>
        ///   Gets the 95% confidence intervals for the <see cref="Means"/>.
        /// </summary>
        /// 
        public DoubleRange[] Confidence
        {
            get
            {
                if (confidence == null)
                {
                    confidence = new DoubleRange[variables];
                    for (int i = 0; i < confidence.Length; i++)
                        confidence[i] = GetConfidenceInterval(i);
                }

                return confidence;
            }
        }

        /// <summary>
        ///   Gets the 95% deviance intervals for the <see cref="Means"/>.
        /// </summary>
        /// 
        /// <remarks>
        ///   A deviance interval uses the standard deviation rather 
        ///   than the standard error to compute the range interval 
        ///   for a variable.
        /// </remarks>
        /// 
        public DoubleRange[] Deviance
        {
            get
            {
                if (deviance == null)
                {
                    deviance = new DoubleRange[variables];
                    for (int i = 0; i < deviance.Length; i++)
                        deviance[i] = GetDevianceInterval(i);
                }

                return deviance;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Mode of each data column.
        /// </summary>
        /// 
        public double[] Modes
        {
            get
            {
                if (modes == null)
                {
                    if (sourceMatrix != null)
                        modes = sourceMatrix.Mode();
                    else modes = sourceArray.Mode();
                }

                return modes;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Median of each data column.
        /// </summary>
        /// 
        public double[] Medians
        {
            get
            {
                if (medians == null)
                {
                    if (sourceMatrix != null)
                        medians = sourceMatrix.Median(type: quantileMethod);
                    else medians = sourceArray.Median(type: quantileMethod);
                }

                return medians;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Variance of each data column.
        /// </summary>
        /// 
        public double[] Variances
        {
            get
            {
                if (variances == null)
                {
                    if (sourceMatrix != null)
                        variances = sourceMatrix.Variance(Means);
                    else variances = sourceArray.Variance(Means);
                }

                return variances;
            }
        }

        /// <summary>
        ///   Gets a vector containing the number of distinct elements for each data column.
        /// </summary>
        /// 
        public int[] Distinct
        {
            get
            {
                if (distinct == null)
                {
                    if (sourceMatrix != null)
                        distinct = sourceMatrix.DistinctCount();
                    else distinct = sourceArray.DistinctCount();
                }

                return distinct;
            }
        }

        /// <summary>
        ///   Gets an array containing the Ranges of each data column.
        /// </summary>
        /// 
        public DoubleRange[] Ranges
        {
            get
            {
                if (ranges == null)
                {
                    if (sourceMatrix != null)
                        this.ranges = Matrix.GetRange(sourceMatrix, 0);
                    else this.ranges = Matrix.GetRange(sourceArray, 0);
                }

                return ranges;
            }
        }

        /// <summary>
        ///   Gets an array containing the interquartile range of each data column.
        /// </summary>
        /// 
        public DoubleRange[] Quartiles
        {
            get
            {
                if (quartiles == null)
                {
                    if (sourceMatrix != null)
                        this.medians = sourceMatrix.Quartiles(out this.quartiles, type: quantileMethod);
                    else this.medians = sourceArray.Quartiles(out this.quartiles, type: quantileMethod);
                }

                return quartiles;
            }
        }

        /// <summary>
        ///   Gets an array containing the inner fences of each data column.
        /// </summary>
        /// 
        public DoubleRange[] InnerFences
        {
            get
            {
                if (innerFences == null)
                {
                    DoubleRange[] Q = Quartiles;

                    innerFences = new DoubleRange[variables];
                    for (int i = 0; i < innerFences.Length; i++)
                        innerFences[i] = Statistics.Tools.InnerFence(Q[i]);
                }

                return innerFences;
            }
        }

        /// <summary>
        ///   Gets an array containing the outer fences of each data column.
        /// </summary>
        /// 
        public DoubleRange[] OuterFences
        {
            get
            {
                if (outerFences == null)
                {
                    DoubleRange[] Q = Quartiles;

                    outerFences = new DoubleRange[variables];
                    for (int i = 0; i < outerFences.Length; i++)
                        outerFences[i] = Statistics.Tools.OuterFence(Q[i]);
                }

                return outerFences;
            }
        }

        /// <summary>
        ///   Gets an array containing the sum of each data column.
        /// </summary>
        /// 
        public double[] Sums
        {
            get
            {
                if (sums == null)
                {
                    if (sourceMatrix != null)
                        this.sums = Accord.Math.Matrix.Sum(sourceMatrix, 0);
                    else this.sums = Accord.Math.Matrix.Sum(sourceArray, 0);
                }

                return sums;
            }
        }

        /// <summary>
        /// Gets an array containing the skewness for of each data column.
        /// </summary>
        /// 
        public double[] Skewness
        {
            get
            {
                if (skewness == null)
                {
                    if (sourceMatrix != null)
                        this.skewness = sourceMatrix.Skewness();
                    else this.skewness = sourceArray.Skewness();
                }

                return skewness;
            }
        }

        /// <summary>
        /// Gets an array containing the kurtosis for of each data column.
        /// </summary>
        /// 
        public double[] Kurtosis
        {
            get
            {
                if (kurtosis == null)
                {
                    if (sourceMatrix != null)
                        this.kurtosis = sourceMatrix.Kurtosis();
                    else this.kurtosis = sourceArray.Kurtosis();
                }

                return kurtosis;
            }
        }

        /// <summary>
        ///   Gets the number of samples (or observations) in the data.
        /// </summary>
        /// 
        public int Samples
        {
            get { return samples; }
        }

        /// <summary>
        ///   Gets the number of variables (or features) in the data.
        /// </summary>
        /// 
        public int Variables
        {
            get { return variables; }
        }

        /// <summary>
        /// Gets a collection of DescriptiveMeasures objects that can be bound to a DataGridView.
        /// </summary>
        /// 
        public DescriptiveMeasureCollection Measures
        {
            get { return measuresCollection; }
        }


        /// <summary>
        ///   Gets a confidence interval for the <see cref="Means"/>
        ///   within the given confidence level percentage.
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// <param name="index">The index of the data column whose confidence
        ///   interval should be calculated.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        public DoubleRange GetConfidenceInterval(int index, double percent = 0.95)
        {
            double z = NormalDistribution.Standard
                .InverseDistributionFunction(0.5 + percent / 2.0);

            return new DoubleRange(
                Means[index] - z * StandardErrors[index],
                Means[index] + z * StandardErrors[index]);
        }

        /// <summary>
        ///   Gets a deviance interval for the <see cref="Means"/>
        ///   within the given confidence level percentage (i.e. uses
        ///   the standard deviation rather than the standard error to
        ///   compute the range interval for the variable).
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// <param name="index">The index of the data column whose confidence
        ///   interval should be calculated.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        public DoubleRange GetDevianceInterval(int index, double percent = 0.95)
        {
            double z = NormalDistribution.Standard
                .InverseDistributionFunction(0.5 + percent / 2.0);

            return new DoubleRange(
                Means[index] - z * StandardDeviations[index],
                Means[index] + z * StandardDeviations[index]);
        }

    }

    /// <summary>
    ///   Descriptive measures for a variable.
    /// </summary>
    /// 
    /// <seealso cref="DescriptiveAnalysis"/>
    /// 
    [Serializable]
    public class DescriptiveMeasures : IDescriptiveMeasures
    {

        private DescriptiveAnalysis analysis;
        private int index;

        internal DescriptiveMeasures(DescriptiveAnalysis analysis, int index)
        {
            this.analysis = analysis;
            this.index = index;
        }

        /// <summary>
        ///   Gets the descriptive analysis 
        ///   that originated this measure.
        /// </summary>
        /// 
        [Browsable(false)]
        public DescriptiveAnalysis Analysis
        {
            get { return analysis; }
        }

        /// <summary>
        ///   Gets the variable's index.
        /// </summary>
        /// 
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        ///   Gets the variable's name
        /// </summary>
        /// 
        public string Name
        {
            get { return analysis.ColumnNames[index]; }
        }

        /// <summary>
        ///   Gets the variable's total sum.
        /// </summary>
        /// 
        public double Sum
        {
            get { return analysis.Sums[index]; }
        }

        /// <summary>
        ///   Gets the variable's mean.
        /// </summary>
        /// 
        public double Mean
        {
            get { return analysis.Means[index]; }
        }

        /// <summary>
        ///   Gets the variable's standard deviation.
        /// </summary>
        /// 
        public double StandardDeviation
        {
            get { return analysis.StandardDeviations[index]; }
        }

        /// <summary>
        ///   Gets the variable's median.
        /// </summary>
        /// 
        public double Median
        {
            get { return analysis.Medians[index]; }
        }

        /// <summary>
        ///   Gets the variable's outer fences range.
        /// </summary>
        /// 
        public DoubleRange OuterFence
        {
            get { return analysis.OuterFences[index]; }
        }

        /// <summary>
        ///   Gets the variable's inner fence range.
        /// </summary>
        /// 
        public DoubleRange InnerFence
        {
            get { return analysis.InnerFences[index]; }
        }

        /// <summary>
        ///   Gets the variable's interquartile range.
        /// </summary>
        /// 
        public DoubleRange Quartiles
        {
            get { return analysis.Quartiles[index]; }
        }

        /// <summary>
        ///   Gets the variable's mode.
        /// </summary>
        /// 
        public double Mode
        {
            get { return analysis.Modes[index]; }
        }

        /// <summary>
        ///   Gets the variable's variance.
        /// </summary>
        /// 
        public double Variance
        {
            get { return analysis.Variances[index]; }
        }

        /// <summary>
        ///   Gets the variable's skewness.
        /// </summary>
        /// 
        public double Skewness
        {
            get { return analysis.Skewness[index]; }
        }

        /// <summary>
        ///   Gets the variable's kurtosis.
        /// </summary>
        /// 
        public double Kurtosis
        {
            get { return analysis.Kurtosis[index]; }
        }

        /// <summary>
        ///   Gets the variable's standard error of the mean.
        /// </summary>
        /// 
        public double StandardError
        {
            get { return analysis.StandardErrors[index]; }
        }

        /// <summary>
        ///   Gets the variable's maximum value.
        /// </summary>
        /// 
        public double Max
        {
            get { return analysis.Ranges[index].Max; }
        }

        /// <summary>
        ///   Gets the variable's minimum value.
        /// </summary>
        /// 
        public double Min
        {
            get { return analysis.Ranges[index].Min; }
        }

        /// <summary>
        ///   Gets the variable's length.
        /// </summary>
        /// 
        public double Length
        {
            get { return analysis.Ranges[index].Length; }
        }

        /// <summary>
        ///   Gets the number of distinct values for the variable.
        /// </summary>
        /// 
        public int Distinct
        {
            get { return analysis.Distinct[index]; }
        }

        /// <summary>
        ///   Gets the number of samples for the variable.
        /// </summary>
        /// 
        public int Count
        {
            get { return analysis.Samples; }
        }

        /// <summary>
        ///   Gets the 95% confidence interval around the <see cref="Mean"/>.
        /// </summary>
        /// 
        public DoubleRange Confidence
        {
            get { return analysis.Confidence[index]; }
        }

        /// <summary>
        ///   Gets the 95% deviance interval around the <see cref="Mean"/>.
        /// </summary>
        /// 
        public DoubleRange Deviance
        {
            get { return analysis.Deviance[index]; }
        }

        /// <summary>
        ///   Gets the variable's observations.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[] Samples
        {
            get { return analysis.Source.GetColumn(index); }
        }

        /// <summary>
        ///   Gets a confidence interval for the <see cref="Mean"/>
        ///   within the given confidence level percentage.
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        public DoubleRange GetConfidenceInterval(double percent = 0.95)
        {
            return analysis.GetConfidenceInterval(index, percent);
        }

        /// <summary>
        ///   Gets a deviance interval for the <see cref="Mean"/>
        ///   within the given confidence level percentage (i.e. uses
        ///   the standard deviation rather than the standard error to
        ///   compute the range interval for the variable).
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        public DoubleRange GetDevianceInterval(double percent = 0.95)
        {
            return analysis.GetDevianceInterval(index, percent);
        }

    }

    /// <summary>
    ///   Collection of descriptive measures.
    /// </summary>
    /// 
    /// <seealso cref="DescriptiveMeasures"/>
    /// <seealso cref="DescriptiveAnalysis"/>
    /// 
    [Serializable]
    public class DescriptiveMeasureCollection : ReadOnlyKeyedCollection<string, DescriptiveMeasures>
    {
        internal DescriptiveMeasureCollection(DescriptiveMeasures[] components)
            : base(components)
        {
        }

        /// <summary>
        ///   Gets the key for item.
        /// </summary>
        /// 
        protected override string GetKeyForItem(DescriptiveMeasures item)
        {
            return item.Name;
        }
    }

}
