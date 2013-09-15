// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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

        private double[] kurtosis;
        private Double[] skewness;
        private double[] standardErrors;


        private double[,] covarianceMatrix;
        private double[,] correlationMatrix;

        private double[,] zScores;
        private double[,] dScores;
        private double[,] sourceMatrix;

        private DescriptiveMeasureCollection measuresCollection;


        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
        public DescriptiveAnalysis(double[] data)
        {
            Compute(Matrix.ColumnVector(data), null);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
        public DescriptiveAnalysis(double[,] data)
        {
            Compute(data, null);
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
            // Initial argument checking
            if (data == null) throw new ArgumentNullException("data");

            if (columnNames == null) throw new ArgumentNullException("columnNames");

            Compute(data, columnNames);
        }

        /// <summary>
        ///   Constructs the Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// 
        public DescriptiveAnalysis(double[][] data)
        {
            Compute(data.ToMatrix(), null);
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
            if (data == null) throw new ArgumentNullException("data");

            if (columnNames == null) throw new ArgumentNullException("columnNames");

            Compute(data.ToMatrix(), columnNames);
        }

        private void Compute(double[,] data, string[] columnNames)
        {
            if (columnNames == null)
            {
                // Generate column names as Column 1, Column 2, ...
                columnNames = new string[data.GetLength(0)];
                for (int i = 0; i < columnNames.Length; i++)
                    columnNames[i] = "Column " + i;
            }

            this.sourceMatrix = data;
            this.columnNames = columnNames;

            this.samples = data.GetLength(0);
            this.variables = data.GetLength(1);


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
            // Run analysis
            this.sums = Accord.Math.Matrix.Sum(sourceMatrix);
            this.means = Statistics.Tools.Mean(sourceMatrix, sums);
            this.standardDeviations = Statistics.Tools.StandardDeviation(sourceMatrix, means);
            this.ranges = Matrix.Range(sourceMatrix, 0);
            this.kurtosis = Statistics.Tools.Kurtosis(sourceMatrix, means);
            this.skewness = Statistics.Tools.Skewness(sourceMatrix, means);
            this.medians = Statistics.Tools.Median(sourceMatrix);
            this.modes = Statistics.Tools.Mode(sourceMatrix);
            this.variances = Statistics.Tools.Variance(sourceMatrix, means);
            this.standardErrors = Statistics.Tools.StandardError(samples, standardDeviations);
            this.distinct = Statistics.Tools.DistinctCount(sourceMatrix);

            // Mean centered data
            this.dScores = Statistics.Tools.Center(sourceMatrix, means, inPlace: false);

            // Mean centered and standardized data
            this.zScores = Statistics.Tools.Standardize(dScores, standardDeviations, inPlace: false);

            // Covariance and correlation
            this.covarianceMatrix = Statistics.Tools.Covariance(sourceMatrix, means);
            this.correlationMatrix = Statistics.Tools.Covariance(zScores);
        }


        #region Properties

        /// <summary>
        ///   Gets the source matrix from which the analysis was run.
        /// </summary>
        /// 
        public double[,] Source
        {
            get { return this.sourceMatrix; }
        }

        /// <summary>
        ///   Gets the column names from the variables in the data.
        /// </summary>
        /// 
        public String[] ColumnNames
        {
            get { return this.columnNames; }
        }

        /// <summary>
        ///   Gets the mean subtracted data.
        /// </summary>
        /// 
        public double[,] DeviationScores
        {
            get { return this.dScores; }
        }

        /// <summary>
        /// Gets the mean subtracted and deviation divided data. Also known as Z-Scores.
        /// </summary>
        /// 
        public double[,] StandardScores
        {
            get { return this.zScores; }
        }

        /// <summary>
        ///   Gets the Covariance Matrix
        /// </summary>
        /// 
        public double[,] CovarianceMatrix
        {
            get { return covarianceMatrix; }
        }

        /// <summary>
        ///   Gets the Correlation Matrix
        /// </summary>
        /// 
        public double[,] CorrelationMatrix
        {
            get { return correlationMatrix; }
        }

        /// <summary>
        ///   Gets a vector containing the Mean of each column of data.
        /// </summary>
        /// 
        public double[] Means
        {
            get { return means; }
        }

        /// <summary>
        ///   Gets a vector containing the Standard Deviation of each column of data.
        /// </summary>
        /// 
        public double[] StandardDeviations
        {
            get { return standardDeviations; }
        }

        /// <summary>
        ///   Gets a vector containing the Standard Error of the Mean of each column of data.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get { return standardErrors; }
        }

        /// <summary>
        ///   Gets a vector containing the Mode of each column of data.
        /// </summary>
        /// 
        public double[] Modes
        {
            get { return modes; }
        }

        /// <summary>
        ///   Gets a vector containing the Median of each column of data.
        /// </summary>
        /// 
        public double[] Medians
        {
            get { return medians; }
        }

        /// <summary>
        ///   Gets a vector containing the Variance of each column of data.
        /// </summary>
        /// 
        public double[] Variances
        {
            get { return variances; }
        }

        /// <summary>
        ///   Gets a vector containing the number of distinct elements for each column of data.
        /// </summary>
        /// 
        public int[] Distinct
        {
            get { return distinct; }
        }

        /// <summary>
        ///   Gets an array containing the Ranges of each column of data.
        /// </summary>
        /// 
        public DoubleRange[] Ranges
        {
            get { return ranges; }
        }

        /// <summary>
        ///   Gets an array containing the sum of each column of data.
        /// </summary>
        /// 
        public double[] Sums
        {
            get { return sums; }
        }

        /// <summary>
        /// Gets an array containing the skewness for of each column of data.
        /// </summary>
        /// 
        public double[] Skewness
        {
            get { return skewness; }
        }

        /// <summary>
        /// Gets an array containing the kurtosis for of each column of data.
        /// </summary>
        /// 
        public double[] Kurtosis
        {
            get { return kurtosis; }
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


    }

    /// <summary>
    ///   Descriptive measures for a variable.
    /// </summary>
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
    [Serializable]
    public class DescriptiveMeasureCollection : System.Collections.ObjectModel.ReadOnlyCollection<DescriptiveMeasures>
    {
        internal DescriptiveMeasureCollection(DescriptiveMeasures[] components)
            : base(components) { }
    }

}
