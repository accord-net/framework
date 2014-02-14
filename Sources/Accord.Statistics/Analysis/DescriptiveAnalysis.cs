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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using AForge;

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
    ///   <code>
    ///   // Suppose we would like to compute descriptive
    ///   // statistics from the following data samples:
    ///   double[,] data =
    ///   {
    ///       { 1, 52, 5 },
    ///       { 2, 12, 5 },
    ///       { 1, 65, 5 },
    ///       { 1, 25, 5 },
    ///       { 2, 62, 5 },
    ///   };
    ///
    ///   // Create the analysis
    ///   DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);
    ///
    ///   // Compute
    ///   analysis.Compute();
    ///
    ///   // Retrieve interest measures
    ///   double[] means = analysis.Means; // { 1.4, 43.2, 5.0 }
    ///   double[] modes = analysis.Modes; // { 1.0, 52.0, 5.0 }
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="Statistics.Tools"/>
    /// <seealso cref="DescriptiveMeasures"/>
    ///
    [Serializable]
    public class DescriptiveAnalysis : IMultivariateAnalysis
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

        private DoubleRange[] ranges;
        private DoubleRange[] confidence;

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


        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
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
            else
            {
                this.samples = array.Length;
                this.variables = array[0].Length;
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

            // Mean centered and standardized data
            this.dScores = DeviationScores;
            this.zScores = StandardScores;

            // Covariance and correlation
            this.covarianceMatrix = CovarianceMatrix;
            this.correlationMatrix = CorrelationMatrix;

            this.confidence = Confidence;
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
        }

        #region Properties

        /// <summary>
        ///   Gets the source matrix from which the analysis was run.
        /// </summary>
        /// 
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
        public double[][] Array
        {
            get
            {
                if (this.sourceArray == null)
                    sourceArray = sourceMatrix.ToArray();
                return sourceArray;
            }
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
                    this.dScores = Statistics.Tools.Center(Source, Means, inPlace: false);
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
                        covarianceMatrix = Statistics.Tools.Covariance(sourceMatrix, Means);
                    else covarianceMatrix = Statistics.Tools.Covariance(sourceArray, Means);
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
                        correlationMatrix = Statistics.Tools.Correlation(sourceMatrix, Means, StandardDeviations);
                    else correlationMatrix = Statistics.Tools.Correlation(sourceArray, Means, StandardDeviations);
                }

                return correlationMatrix;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Mean of each column of data.
        /// </summary>
        /// 
        public double[] Means
        {
            get
            {
                if (means == null)
                {
                    if (sourceMatrix != null)
                        means = Statistics.Tools.Mean(sourceMatrix, Sums);
                    else means = Statistics.Tools.Mean(sourceArray, Sums);
                }
                return means;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Standard Deviation of each column of data.
        /// </summary>
        /// 
        public double[] StandardDeviations
        {
            get
            {
                if (standardDeviations == null)
                {
                    if (sourceMatrix != null)
                        standardDeviations = Statistics.Tools.StandardDeviation(sourceMatrix, Means);
                    else standardDeviations = Statistics.Tools.StandardDeviation(sourceArray, Means);
                }

                return standardDeviations;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Standard Error of the Mean of each column of data.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get
            {
                if (standardErrors == null)
                    standardErrors = Statistics.Tools.StandardError(samples, StandardDeviations);

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
        ///   Gets a vector containing the Mode of each column of data.
        /// </summary>
        /// 
        public double[] Modes
        {
            get
            {
                if (modes == null)
                {
                    if (sourceMatrix != null)
                        modes = Statistics.Tools.Mode(sourceMatrix);
                    else modes = Statistics.Tools.Mode(sourceArray);
                }

                return modes;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Median of each column of data.
        /// </summary>
        /// 
        public double[] Medians
        {
            get
            {
                if (medians == null)
                {
                    if (sourceMatrix != null)
                        medians = Statistics.Tools.Median(sourceMatrix);
                    else medians = Statistics.Tools.Median(sourceArray);
                }

                return medians;
            }
        }

        /// <summary>
        ///   Gets a vector containing the Variance of each column of data.
        /// </summary>
        /// 
        public double[] Variances
        {
            get
            {
                if (variances == null)
                {
                    if (sourceMatrix != null)
                        variances = Statistics.Tools.Variance(sourceMatrix, Means);
                    else variances = Statistics.Tools.Variance(sourceArray, Means);
                }

                return variances;
            }
        }

        /// <summary>
        ///   Gets a vector containing the number of distinct elements for each column of data.
        /// </summary>
        /// 
        public int[] Distinct
        {
            get
            {
                if (distinct == null)
                {
                    if (sourceMatrix != null)
                        distinct = Statistics.Tools.DistinctCount(sourceMatrix);
                    else distinct = Statistics.Tools.DistinctCount(sourceArray);
                }

                return distinct;
            }
        }

        /// <summary>
        ///   Gets an array containing the Ranges of each column of data.
        /// </summary>
        /// 
        public DoubleRange[] Ranges
        {
            get
            {
                if (ranges == null)
                {
                    if (sourceMatrix != null)
                        this.ranges = Matrix.Range(sourceMatrix, 0);
                    else this.ranges = Matrix.Range(sourceArray, 0);
                }

                return ranges;
            }
        }

        /// <summary>
        ///   Gets an array containing the sum of each column of data.
        /// </summary>
        /// 
        public double[] Sums
        {
            get
            {
                if (sums == null)
                {
                    if (sourceMatrix != null)
                        this.sums = Accord.Math.Matrix.Sum(sourceMatrix);
                    else this.sums = Accord.Math.Matrix.Sum(sourceArray);
                }

                return sums;
            }
        }

        /// <summary>
        /// Gets an array containing the skewness for of each column of data.
        /// </summary>
        /// 
        public double[] Skewness
        {
            get
            {
                if (skewness == null)
                {
                    if (sourceMatrix != null)
                        this.skewness = Statistics.Tools.Skewness(sourceMatrix);
                    else this.skewness = Statistics.Tools.Skewness(sourceArray);
                }

                return skewness;
            }
        }

        /// <summary>
        /// Gets an array containing the kurtosis for of each column of data.
        /// </summary>
        /// 
        public double[] Kurtosis
        {
            get
            {
                if (kurtosis == null)
                {
                    if (sourceMatrix != null)
                        this.kurtosis = Statistics.Tools.Kurtosis(sourceMatrix);
                    else this.kurtosis = Statistics.Tools.Kurtosis(sourceArray);
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

        #endregion


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

    }

    /// <summary>
    ///   Descriptive measures for a variable.
    /// </summary>
    /// 
    /// <seealso cref="DescriptiveAnalysis"/>
    /// 
    [Serializable]
    public class DescriptiveMeasures
    {

        private DescriptiveAnalysis analysis;
        private int index;

        internal DescriptiveMeasures(DescriptiveAnalysis analysis, int index)
        {
            this.analysis = analysis;
            this.index = index;
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
        ///   Gets the variable's observations.
        /// </summary>
        /// 
        public double[] Samples
        {
            get { return analysis.Source.GetColumn(index); }
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
    public class DescriptiveMeasureCollection : System.Collections.ObjectModel.ReadOnlyCollection<DescriptiveMeasures>
    {
        internal DescriptiveMeasureCollection(DescriptiveMeasures[] components)
            : base(components) { }
    }

}
