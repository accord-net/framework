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
    using AForge;
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Accord.Compat;

    /// <summary>
    ///   Descriptive statistics analysis for circular data.
    /// </summary>
    /// 
    /// 
    /// <seealso cref="Statistics.Tools"/>
    /// <seealso cref="DescriptiveAnalysis"/>
    /// <seealso cref="DescriptiveMeasures"/>
    ///
    [Serializable]
#pragma warning disable 612, 618
    public class CircularDescriptiveAnalysis : IMultivariateAnalysis,
        IDescriptiveLearning<CircularDescriptiveAnalysis, double[]>
#pragma warning restore 612, 618
    {

        private int samples;
        private int variables;

        private double[][] angles;
        private double[] lengths;

        private double[] sums;
        private double[] sin;
        private double[] cos;

        private double[] means;
        private double[] standardDeviations;
        private double[] standardErrors;
        private double[] variances;
        private double[] medians;
        private double[] modes;
        private double[] kurtosis;
        private double[] skewness;
        private int[] distinct;

        private double[] angularMeans;
        private double[] angularMedians;

        private string[] columnNames;

        private QuantileMethod quantileMethod = QuantileMethod.Default;
        private DoubleRange[] ranges;
        private DoubleRange[] quartiles;
        private DoubleRange[] innerFences;
        private DoubleRange[] outerFences;

        DoubleRange[] confidences;
        DoubleRange[] deviances;

        private double[] concentration;


        private double[,] sourceMatrix;
        private double[][] sourceArray;
        private double[] sourceRow;

        private CircularDescriptiveMeasureCollection measuresCollection;

        private bool useStrictRanges = true;
        private bool lazy = true;

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// <param name="columnName">The names for the analyzed variable.</param>
        /// <param name="inPlace">
        ///   Whether the analysis should conserve memory by doing 
        ///   operations over the original <paramref name="data"/> array.
        /// </param>
        /// 
        [Obsolete("Please pass only the lengths and columnNames parameters and call the Learn() method passing the data to be analyzed.")]
        public CircularDescriptiveAnalysis(double[] data, double length, String columnName, bool inPlace = false)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            compute(data, null, null, new[] { length }, new[] { columnName }, inPlace);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// <param name="inPlace">
        ///   Whether the analysis should conserve memory by doing 
        ///   operations over the original <paramref name="data"/> array.
        /// </param>
        /// 
        [Obsolete("Please pass only the lengths and columnNames parameters and call the Learn() method passing the data to be analyzed.")]
        public CircularDescriptiveAnalysis(double[] data, double length, bool inPlace = false)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            compute(data, null, null, new[] { length }, null, inPlace);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// 
        [Obsolete("Please pass only the lengths and columnNames parameters and call the Learn() method passing the data to be analyzed.")]
        public CircularDescriptiveAnalysis(double[,] data, double[] length)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (length == null)
                throw new ArgumentNullException("length");

            compute(null, data, null, length, null);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// <param name="columnNames">Names for the analyzed variables.</param>
        /// 
        [Obsolete("Please pass only the lengths and columnNames parameters and call the Learn() method passing the data to be analyzed.")]
        public CircularDescriptiveAnalysis(double[,] data, double[] length, string[] columnNames)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (length == null)
                throw new ArgumentNullException("length");

            if (columnNames == null)
                throw new ArgumentNullException("columnNames");

            compute(null, data, null, length, columnNames);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// 
        [Obsolete("Please pass only the lengths and columnNames parameters and call the Learn() method passing the data to be analyzed.")]
        public CircularDescriptiveAnalysis(double[][] data, double[] length)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (length == null)
                throw new ArgumentNullException("length");

            compute(null, null, data, length, null);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// <param name="columnNames">Names for the analyzed variables.</param>
        /// 
        [Obsolete("Please pass only the lengths and columnNames parameters and call the Learn() method passing the data to be analyzed.")]
        public CircularDescriptiveAnalysis(double[][] data, double[] length, string[] columnNames)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (length == null)
                throw new ArgumentNullException("length");

            if (columnNames == null)
                throw new ArgumentNullException("columnNames");

            compute(null, null, data, length, columnNames);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// <param name="columnNames">Names for the analyzed variables.</param>
        /// 
        public CircularDescriptiveAnalysis(double[] length, string[] columnNames)
        {
            compute(null, null, null, length, columnNames);
        }

        /// <summary>
        ///   Constructs the Circular Descriptive Analysis.
        /// </summary>
        /// 
        /// <param name="length">The length of each circular variable (i.e. 24 for hours).</param>
        /// 
        public CircularDescriptiveAnalysis(double[] length)
        {
            compute(null, null, null, length, null);
        }


        private void compute(double[] row, double[,] matrix, double[][] array,
            double[] length, string[] columnNames, bool inPlace = false)
        {
            this.columnNames = columnNames;
            this.sourceArray = array;
            this.sourceMatrix = matrix;
            this.sourceRow = row;

            if (matrix != null)
            {
                this.samples = matrix.Rows();
                this.variables = matrix.Columns();
                if (lengths == null)
                    lengths = matrix.Max(dimension: 0);
                if (lengths.Length != variables)
                    throw new DimensionMismatchException("length");
                this.lengths = length;
                this.angles = new double[variables][];
                for (int i = 0; i < angles.Length; i++)
                {
                    angles[i] = new double[samples];
                    for (int j = 0; j < angles[i].Length; j++)
                        angles[i][j] = Circular.ToRadians(matrix[j, i], lengths[i]);
                }
            }
            else if (array != null)
            {
                this.samples = array.Length;
                this.variables = array[0].Length;
                this.angles = new double[variables][];
                if (lengths == null)
                    lengths = array.Max(dimension: 1);
                if (lengths.Length != variables)
                    throw new DimensionMismatchException("length");
                this.lengths = length;
                for (int i = 0; i < angles.Length; i++)
                {
                    angles[i] = new double[samples];
                    for (int j = 0; j < angles[i].Length; j++)
                        angles[i][j] = Circular.ToRadians(array[j][i], length[i]);
                }
            }
            else
            {
                this.samples = sourceRow.Length;
                this.variables = 1;
                this.angles = new double[variables][];
                if (lengths == null)
                    lengths = new[] { sourceRow.Max() };
                if (lengths.Length != variables)
                    throw new DimensionMismatchException("length");
                this.lengths = length;
                angles[0] = inPlace ? sourceRow : new double[samples];
                for (int j = 0; j < angles[0].Length; j++)
                    angles[0][j] = Circular.ToRadians(sourceRow[j], length[0]);
            }


            // Create object-oriented structure to access data
            var measures = new CircularDescriptiveMeasures[variables];
            for (int i = 0; i < measures.Length; i++)
                measures[i] = new CircularDescriptiveMeasures(this, i);
            this.measuresCollection = new CircularDescriptiveMeasureCollection(measures);
        }


        /// <summary>
        ///   Computes the analysis using given source data and parameters.
        /// </summary>
        /// 
        [Obsolete("Please use Learn() instead.")]
        public void Compute()
        {
            reset();

            this.sums = Sums;
            this.means = Means;
            this.standardDeviations = StandardDeviations;
            this.ranges = Ranges;
            this.medians = Medians;
            this.variances = Variances;
            this.distinct = Distinct;
            this.quartiles = Quartiles;
            this.innerFences = InnerFences;
            this.outerFences = OuterFences;
            this.modes = Modes;
            this.cos = CosineSum;
            this.sin = SineSum;
            this.skewness = Skewness;
            this.kurtosis = Kurtosis;
            this.concentration = Concentration;
            this.deviances = Deviance;
            this.confidences = Confidence;
        }

        private void reset()
        {
            this.sums = null;
            this.means = null;
            this.standardDeviations = null;
            this.ranges = null;
            this.medians = null;
            this.variances = null;
            this.standardErrors = null;
            this.distinct = null;
            this.modes = null;
            this.deviances = null;
            this.confidences = null;
            this.quartiles = null;
            this.innerFences = null;
            this.outerFences = null;
            this.sin = null;
            this.cos = null;
            this.skewness = null;
            this.kurtosis = null;
            this.concentration = null;
            this.standardErrors = null;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <returns>
        /// A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.
        /// </returns>
        public CircularDescriptiveAnalysis Learn(double[][] x)
        {
            reset();

            this.compute(null, null, x, this.lengths, this.columnNames);

            if (!lazy)
            {
                this.sums = Sums;
                this.means = Means;
                this.standardDeviations = StandardDeviations;
                this.ranges = Ranges;
                this.medians = Medians;
                this.variances = Variances;
                this.distinct = Distinct;
                this.quartiles = Quartiles;
                this.innerFences = InnerFences;
                this.outerFences = OuterFences;
                this.modes = Modes;
                this.cos = CosineSum;
                this.sin = SineSum;
                this.skewness = Skewness;
                this.kurtosis = Kurtosis;
                this.concentration = Concentration;
                this.deviances = Deviance;
                this.confidences = Confidence;
                this.sourceArray = null;
                this.sourceMatrix = null;
                this.sourceRow = null;
            }

            return this;
        }

        /// <summary>
        ///   Gets or sets whether all reported statistics should respect the circular 
        ///   interval. For example, setting this property to <c>false</c> would allow
        ///   the <see cref="Confidence"/>, <see cref="Deviance"/>, <see cref="InnerFences"/>
        ///   and <see cref="OuterFences"/> properties report minimum and maximum values 
        ///   outside the variable's allowed circular range. Default is <c>true</c>.
        /// </summary>
        /// 
        public bool UseStrictRanges
        {
            get { return useStrictRanges; }
            set
            {
                if (useStrictRanges != value)
                {
                    useStrictRanges = value;
                    innerFences = null;
                    outerFences = null;
                    deviances = null;
                    confidences = null;
                }
            }
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
                {
                    if (this.sourceArray == null)
                    {
                        sourceMatrix = sourceRow.ToMatrix(asColumnVector: true);
                    }
                    else
                    {
                        sourceMatrix = sourceArray.ToMatrix();
                    }
                }

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
                {
                    if (this.sourceRow == null)
                    {
                        sourceArray = sourceMatrix.ToJagged();
                    }
                    else
                    {
                        sourceArray = sourceRow.ToJagged(asColumnVector: true);
                    }
                }

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
        public double[][] Angles
        {
            get { return this.angles; }
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
        ///   Gets a vector containing the length of
        ///   the circular domain for each data column.
        /// </summary>
        /// 
        public double[] Lengths
        {
            get { return lengths; }
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
                    double[] c = CosineSum, s = SineSum;

                    means = new double[variables];
                    angularMeans = new double[variables];
                    for (int i = 0; i < means.Length; i++)
                    {
                        angularMeans[i] = Circular.Mean(samples, c[i], s[i]);
                        means[i] = Circular.ToCircular(angularMeans[i], lengths[i]);
                    }
                }

                return means;
            }
            private set { means = value; }
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
                    else if (sourceArray != null)
                        modes = sourceArray.Mode();
                    else modes = new[] { sourceRow.Mode() };
                }

                return modes;
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
                    double[] c = CosineSum, s = SineSum;

                    standardDeviations = new double[variables];
                    for (int i = 0; i < standardDeviations.Length; i++)
                    {
                        standardDeviations[i] = Circular.StandardDeviation(samples, c[i], s[i])
                            * lengths[i] / (2 * Math.PI);
                    }
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
                {
                    double[] c = CosineSum, s = SineSum;

                    standardErrors = new double[variables];
                    for (int i = 0; i < standardErrors.Length; i++)
                    {
                        standardErrors[i] = Circular.StandardError(samples, c[i], s[i], 0.05)
                            * lengths[i] / (2 * Math.PI);
                    }
                }

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
                if (confidences == null)
                {
                    confidences = new DoubleRange[variables];
                    for (int i = 0; i < confidences.Length; i++)
                        confidences[i] = GetConfidenceInterval(i);
                }

                return confidences;
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
                if (deviances == null)
                {
                    deviances = new DoubleRange[variables];
                    for (int i = 0; i < deviances.Length; i++)
                        deviances[i] = GetDevianceInterval(i);
                }

                return deviances;
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
                    computeMedians();

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
                    double[] c = CosineSum, s = SineSum;

                    variances = new double[variables];
                    for (int i = 0; i < variances.Length; i++)
                    {
                        double scale = lengths[i] / (2 * Math.PI);
                        variances[i] = Circular.Variance(samples, c[i], s[i]) * scale * scale;
                    }
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
                    else if (sourceArray != null)
                        distinct = sourceArray.DistinctCount();
                    else
                        distinct = new[] { sourceRow.DistinctCount() };
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
                    this.ranges = new DoubleRange[variables];
                    for (int i = 0; i < ranges.Length; i++)
                        ranges[i] = new DoubleRange(0, lengths[i]);
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
                    if (medians == null)
                        computeMedians();

                    quartiles = new DoubleRange[variables];
                    for (int i = 0; i < variances.Length; i++)
                    {
                        Circular.Quartiles(angles[i], out quartiles[i], angularMedians[i], wrap: useStrictRanges, type: quantileMethod);

                        quartiles[i].Min = Circular.ToCircular(quartiles[i].Min, lengths[i], useStrictRanges);
                        quartiles[i].Max = Circular.ToCircular(quartiles[i].Max, lengths[i], useStrictRanges);
                    }
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
                    {
                        innerFences[i] = Statistics.Tools.InnerFence(Q[i]);

                        if (useStrictRanges)
                        {
                            innerFences[i].Min = Tools.Mod(innerFences[i].Min, lengths[i]);
                            innerFences[i].Max = Tools.Mod(innerFences[i].Max, lengths[i]);
                        }
                    }
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
                    {
                        outerFences[i] = Statistics.Tools.OuterFence(Q[i]);

                        if (useStrictRanges)
                        {
                            outerFences[i].Min = Tools.Mod(outerFences[i].Min, lengths[i]);
                            outerFences[i].Max = Tools.Mod(outerFences[i].Max, lengths[i]);
                        }
                    }
                }

                return outerFences;
            }
        }

        /// <summary>
        ///   Gets an array containing the sum of each data column. If 
        ///   the analysis has been computed in place, this will contain 
        ///   the sum of the transformed angle values instead.
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
                    else if (sourceArray != null)
                        this.sums = Accord.Math.Matrix.Sum(sourceArray, 0);
                    else this.sums = new[] { Accord.Math.Matrix.Sum(sourceRow) };
                }

                return sums;
            }
        }

        /// <summary>
        ///   Gets an array containing the sum of cosines for each data column.
        /// </summary>
        /// 
        public double[] CosineSum
        {
            get
            {
                if (cos == null)
                    computeSums();

                return cos;
            }
        }

        /// <summary>
        ///   Gets an array containing the sum of sines for each data column.
        /// </summary>
        /// 
        public double[] SineSum
        {
            get
            {
                if (sin == null)
                    computeSums();

                return sin;
            }
        }

        /// <summary>
        ///   Gets an array containing the circular concentration for each data column.
        /// </summary>
        /// 
        public double[] Concentration
        {
            get
            {
                if (concentration == null)
                {
                    var m = Means;

                    concentration = new double[variables];
                    for (int i = 0; i < variances.Length; i++)
                        concentration[i] = Circular.Concentration(angles[i], m[i]);
                }

                return variances;
            }
        }

        /// <summary>
        ///   Gets an array containing the skewness for of each data column.
        /// </summary>
        /// 
        public double[] Skewness
        {
            get
            {
                if (skewness == null)
                {
                    skewness = new double[variables];
                    for (int i = 0; i < skewness.Length; i++)
                    {
                        skewness[i] = Circular.Skewness(angles[i])
                            * lengths[i] / (2 * Math.PI);
                    }
                }

                return skewness;
            }
        }

        /// <summary>
        ///   Gets an array containing the kurtosis for of each data column.
        /// </summary>
        /// 
        public double[] Kurtosis
        {
            get
            {
                if (kurtosis == null)
                {
                    kurtosis = new double[variables];
                    for (int i = 0; i < kurtosis.Length; i++)
                    {
                        kurtosis[i] = Circular.Kurtosis(angles[i])
                            * lengths[i] / (2 * Math.PI);
                    }
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
        public CircularDescriptiveMeasureCollection Measures
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
            double[] c = CosineSum, s = SineSum;

            double t = Circular.StandardError(samples, c[index], s[index], 1 - percent);

            t *= lengths[index] / (2 * Math.PI);

            double min = Means[index] - t;
            double max = Means[index] + t;

            if (useStrictRanges)
            {
                min = Tools.Mod(min, lengths[index]);
                max = Tools.Mod(max, lengths[index]);
            }

            return new DoubleRange(min, max);
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

            double min = Means[index] - z * StandardDeviations[index];
            double max = Means[index] + z * StandardDeviations[index];

            if (useStrictRanges)
            {
                min = Tools.Mod(min, lengths[index]);
                max = Tools.Mod(max, lengths[index]);
            }

            return new DoubleRange(min, max);
        }



        private void computeSums()
        {
            cos = new double[variables];
            sin = new double[variables];
            for (int i = 0; i < angles.Length; i++)
                Circular.Sum(angles[i], out cos[i], out sin[i]);
        }

        private void computeMedians()
        {
            medians = new double[variables];
            angularMedians = new double[variables];
            for (int i = 0; i < medians.Length; i++)
            {
                angularMedians[i] = Circular.Median(angles[i]);
                medians[i] = Circular.ToCircular(angularMedians[i], lengths[i]);
            }
        }

    }

    /// <summary>
    ///   Circular descriptive measures for a variable.
    /// </summary>
    /// 
    /// <seealso cref="CircularDescriptiveAnalysis"/>
    /// 
    [Serializable]
    public class CircularDescriptiveMeasures : IDescriptiveMeasures
    {

        private CircularDescriptiveAnalysis analysis;
        private int index;

        internal CircularDescriptiveMeasures(CircularDescriptiveAnalysis analysis, int index)
        {
            this.analysis = analysis;
            this.index = index;
        }

        /// <summary>
        ///   Gets the circular analysis 
        ///   that originated this measure.
        /// </summary>
        /// 
        [Browsable(false)]
        public CircularDescriptiveAnalysis Analysis
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
        ///   Gets the variable's mode.
        /// </summary>
        /// 
        public double Mode
        {
            get { return analysis.Modes[index]; }
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
        ///   Gets the variable's variance.
        /// </summary>
        /// 
        public double Variance
        {
            get { return analysis.Variances[index]; }
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
        ///   Gets the sum of cosines for the variable.
        /// </summary>
        /// 
        public double CosineSum
        {
            get { return analysis.CosineSum[index]; }
        }

        /// <summary>
        ///   Gets the sum of sines for the variable.
        /// </summary>
        /// 
        public double SineSum
        {
            get { return analysis.SineSum[index]; }
        }

        /// <summary>
        ///   Gets the transformed variable's observations.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[] Angles
        {
            get { return analysis.Angles[index]; }
        }

        /// <summary>
        ///   Gets the variable's standard error of the mean.
        /// </summary>
        /// 
        public double StandardError
        {
            get { return analysis.StandardErrors[index]; ; }
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
        ///   Gets the variable <see cref="Circular.Skewness">skewness</see>.
        /// </summary>
        /// 
        public double Skewness
        {
            get { return analysis.Skewness[index]; }
        }

        /// <summary>
        ///   Gets the variable <see cref="Circular.Skewness">kurtosis</see>.
        /// </summary>
        /// 
        public double Kurtosis
        {
            get { return analysis.Kurtosis[index]; }
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
    /// <seealso cref="CircularDescriptiveMeasures"/>
    /// <seealso cref="CircularDescriptiveAnalysis"/>
    /// 
    [Serializable]
    public class CircularDescriptiveMeasureCollection : ReadOnlyKeyedCollection<string, CircularDescriptiveMeasures>
    {
        internal CircularDescriptiveMeasureCollection(CircularDescriptiveMeasures[] components)
            : base(components)
        {
        }

        /// <summary>
        ///   Gets the key for item.
        /// </summary>
        /// 
        protected override string GetKeyForItem(CircularDescriptiveMeasures item)
        {
            return item.Name;
        }
    }
}
