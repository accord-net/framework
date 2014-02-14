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
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Math.Comparers;

    /// <summary>
    ///   Linear Discriminant Analysis (LDA).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Linear Discriminant Analysis (LDA) is a method of finding such a linear
    ///   combination of variables which best separates two or more classes.</para>
    /// <para>
    ///   In itself LDA is not a classification algorithm, although it makes use of class
    ///   labels. However, the LDA result is mostly used as part of a linear classifier.
    ///   The other alternative use is making a dimension reduction before using nonlinear
    ///   classification algorithms.</para>
    /// <para>
    ///   It should be noted that several similar techniques (differing in requirements to the sample)
    ///   go together under the general name of Linear Discriminant Analysis. Described below is one of
    ///   these techniques with only two requirements:</para>  
    ///   <list type="number">
    ///     <item><description>The sample size shall exceed the number of variables, and </description></item>
    ///     <item><description>Classes may overlap, but their centers shall be distant from each other. </description></item>
    ///   </list>
    ///   
    /// <para>
    ///   Moreover, LDA requires the following assumptions to be true:</para>
    ///   <list type="bullet">
    ///     <item><description>Independent subjects.</description></item>
    ///     <item><description>Normality: the variance-covariance matrix of the
    ///     predictors is the same in all groups.</description></item>
    ///   </list>
    ///   
    /// <para>
    ///   If the latter assumption is violated, it is common to use quadratic discriminant analysis in
    ///   the same manner as linear discriminant analysis instead.</para>
    ///   
    /// <para>
    ///   This class can also be bound to standard controls such as the 
    ///   <a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView</a>
    ///   by setting their DataSource property to the analysis' <see cref="Discriminants"/> property.</para>
    ///   
    /// <para>
    ///    References:
    ///    <list type="bullet">
    ///      <item><description>
    ///        R. Gutierrez-Osuna, Linear Discriminant Analysis. Available on:
    ///        http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf </description></item>
    ///     </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example creates an analysis for a set of 
    ///   data specified as a jagged (double[][]) array. However,
    ///   the same can also be accomplished using multidimensional
    ///   double[,] arrays.</para>
    /// 
    /// <code>
    /// // Create some sample input data instances. This is the same
    /// // data used in the Gutierrez-Osuna's example available on:
    /// // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf
    /// 
    /// double[][] inputs = 
    /// {
    ///     // Class 0
    ///     new double[] {  4,  1 }, 
    ///     new double[] {  2,  4 },
    ///     new double[] {  2,  3 },
    ///     new double[] {  3,  6 },
    ///     new double[] {  4,  4 },
    /// 
    ///     // Class 1
    ///     new double[] {  9, 10 },
    ///     new double[] {  6,  8 },
    ///     new double[] {  9,  5 },
    ///     new double[] {  8,  7 },
    ///     new double[] { 10,  8 }
    /// };
    /// 
    /// int[] output = 
    /// {
    ///     0, 0, 0, 0, 0, // The first five are from class 0
    ///     1, 1, 1, 1, 1  // The last five are from class 1
    /// };
    /// 
    /// // Then, we will create a LDA for the given instances.
    /// var lda = new LinearDiscriminantAnalysis(inputs, output);
    /// 
    /// lda.Compute(); // Compute the analysis
    /// 
    /// 
    /// // Now we can project the data into KDA space:
    /// double[][] projection = lda.Transform(inputs);
    /// 
    /// // Or perform classification using:
    /// int[] results = lda.Classify(inputs);
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class LinearDiscriminantAnalysis : IDiscriminantAnalysis, IProjectionAnalysis
    {
        private int dimension;
        private int classes;

        private double[] totalMeans;
        private double[] totalStdDevs;

        internal int[] classCount;
        internal double[][] classMeans;
        internal double[][] classStdDevs;
        internal double[][,] classScatter;

        internal double[][] projectedMeans;

        // TODO: Use Mahalanobis distance instead of Euclidean
        // for classification, considering projection covariances.
        // internal double[][] projectedPrecision;

        private double[,] eigenvectors;
        private double[] eigenvalues;

        private double[,] result;
        private double[,] source;
        private int[] outputs;

        double[,] Sw, Sb, St; // Scatter matrices

        private double[] discriminantProportions;
        private double[] discriminantCumulative;

        DiscriminantCollection discriminantCollection;
        DiscriminantAnalysisClassCollection classCollection;


        //---------------------------------------------


        #region Constructor
        /// <summary>
        ///   Constructs a new Linear Discriminant Analysis object.
        /// </summary>
        /// 
        /// <param name="inputs">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="outputs">The labels for each observation row in the input matrix.</param>
        /// 
        public LinearDiscriminantAnalysis(double[,] inputs, int[] outputs)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");

            if (inputs.GetLength(0) != outputs.Length)
                throw new ArgumentException(
                    "The number of rows in the input array must match the number of given outputs.");

            init(inputs, outputs);
        }

        /// <summary>
        ///   Constructs a new Linear Discriminant Analysis object.
        /// </summary>
        /// 
        /// <param name="inputs">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="outputs">The labels for each observation row in the input matrix.</param>
        /// 
        public LinearDiscriminantAnalysis(double[][] inputs, int[] outputs)
        {
            // Initial argument checking
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new ArgumentException(
                    "The number of rows in the input array must match the number of given outputs.");

            init(inputs.ToMatrix(), outputs);
        }

        private void init(double[,] inputs, int[] outputs)
        {

            // Gets the number of classes
            int startingClass = outputs.Min();
            this.classes = outputs.Max() - startingClass + 1;

            // Store the original data
            this.source = inputs;
            this.outputs = outputs;
            this.dimension = inputs.GetLength(1);

            // Creates simple structures to hold information later
            this.classCount = new int[classes];
            this.classMeans = new double[classes][];
            this.classStdDevs = new double[classes][];
            this.classScatter = new double[classes][,];
            this.projectedMeans = new double[classes][];


            // Creates the object-oriented structure to hold information about the classes
            DiscriminantAnalysisClass[] collection = new DiscriminantAnalysisClass[classes];
            for (int i = 0; i < classes; i++)
                collection[i] = new DiscriminantAnalysisClass(this, i, startingClass + i);
            this.classCollection = new DiscriminantAnalysisClassCollection(collection);
        }
        #endregion


        //---------------------------------------------


        #region Properties
        /// <summary>
        ///   Returns the original supplied data to be analyzed.
        /// </summary>
        /// 
        public double[,] Source
        {
            get { return this.source; }
        }

        /// <summary>
        ///   Gets the resulting projection of the source data given on
        ///   the creation of the analysis into discriminant space.
        /// </summary>
        /// 
        public double[,] Result
        {
            get { return this.result; }
            protected set { this.result = value; }
        }

        /// <summary>
        ///   Gets the original classifications (labels) of the source data
        ///   given on the moment of creation of this analysis object.
        /// </summary>
        /// 
        public int[] Classifications
        {
            get { return this.outputs; }
        }

        /// <summary>
        ///   Gets the mean of the original data given at method construction.
        /// </summary>
        /// 
        public double[] Means
        {
            get { return totalMeans; }
            protected set { totalMeans = value; }
        }

        /// <summary>
        ///   Gets the standard mean of the original data given at method construction.
        /// </summary>
        /// 
        public double[] StandardDeviations
        {
            get { return totalStdDevs; }
            protected set { totalStdDevs = value; }
        }

        /// <summary>
        ///   Gets the Within-Class Scatter Matrix for the data.
        /// </summary>
        /// 
        public double[,] ScatterWithinClass
        {
            get { return Sw; }
            protected set { Sw = value; }
        }

        /// <summary>
        ///   Gets the Between-Class Scatter Matrix for the data.
        /// </summary>
        /// 
        public double[,] ScatterBetweenClass
        {
            get { return Sb; }
            protected set { Sb = value; }
        }

        /// <summary>
        ///   Gets the Total Scatter Matrix for the data.
        /// </summary>
        /// 
        public double[,] ScatterMatrix
        {
            get { return St; }
            protected set { St = value; }
        }

        /// <summary>
        ///   Gets the Eigenvectors obtained during the analysis,
        ///   composing a basis for the discriminant factor space.
        /// </summary>
        /// 
        public double[,] DiscriminantMatrix
        {
            get { return eigenvectors; }
            protected set { eigenvectors = value; }
        }

        /// <summary>
        ///   Gets the Eigenvalues found by the analysis associated
        ///   with each vector of the ComponentMatrix matrix.
        /// </summary>
        /// 
        public double[] Eigenvalues
        {
            get { return eigenvalues; }
            protected set { eigenvalues = value; }
        }

        /// <summary>
        ///   Gets the level of importance each discriminant factor has in
        ///   discriminant space. Also known as amount of variance explained.
        /// </summary>
        /// 
        public double[] DiscriminantProportions
        {
            get { return discriminantProportions; }
        }

        /// <summary>
        ///   The cumulative distribution of the discriminants factors proportions.
        ///   Also known as the cumulative energy of the first dimensions of the discriminant
        ///   space or as the amount of variance explained by those dimensions.
        /// </summary>
        /// 
        public double[] CumulativeProportions
        {
            get { return discriminantCumulative; }
        }

        /// <summary>
        ///   Gets the discriminant factors in a object-oriented fashion.
        /// </summary>
        /// 
        public DiscriminantCollection Discriminants
        {
            get { return discriminantCollection; }
        }

        /// <summary>
        ///   Gets information about the distinct classes in the analyzed data.
        /// </summary>
        ///   
        public DiscriminantAnalysisClassCollection Classes
        {
            get { return classCollection; }
        }

        /// <summary>
        ///   Gets the Scatter matrix for each class.
        /// </summary>
        /// 
        protected double[][,] ClassScatter
        {
            get { return classScatter; }
        }

        /// <summary>
        ///   Gets the Mean vector for each class.
        /// </summary>
        /// 
        protected double[][] ClassMeans
        {
            get { return classMeans; }
        }

        /// <summary>
        ///   Gets the feature space mean of the projected data.
        /// </summary>
        /// 
        protected double[][] ProjectionMeans
        {
            get { return projectedMeans; }
        }

        /// <summary>
        ///   Gets the Standard Deviation vector for each class.
        /// </summary>
        /// 
        protected double[][] ClassStandardDeviations
        {
            get { return classStdDevs; }
        }

        /// <summary>
        ///   Gets the observation count for each class.
        /// </summary>
        /// 
        protected int[] ClassCount
        {
            get { return classCount; }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        /// <summary>
        ///   Computes the Multi-Class Linear Discriminant Analysis algorithm.
        /// </summary>
        /// 
        public virtual void Compute()
        {
            // Compute entire data set measures
            Means = Statistics.Tools.Mean(source);
            StandardDeviations = Statistics.Tools.StandardDeviation(source, totalMeans);
            double total = dimension;

            // Initialize the scatter matrices
            this.Sw = new double[dimension, dimension];
            this.Sb = new double[dimension, dimension];


            // For each class
            for (int c = 0; c < Classes.Count; c++)
            {
                // Get the class subset
                double[,] subset = Classes[c].Subset;
                int count = subset.GetLength(0);

                // Get the class mean
                double[] mean = Statistics.Tools.Mean(subset);


                // Continue constructing the Within-Class Scatter Matrix
                double[,] Swi = Statistics.Tools.Scatter(subset, mean, (double)count);

                // Sw = Sw + Swi
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        Sw[i, j] += Swi[i, j];


                // Continue constructing the Between-Class Scatter Matrix
                double[] d = mean.Subtract(totalMeans);
                double[,] Sbi = Matrix.OuterProduct(d, d).Multiply(total);

                // Sb = Sb + Sbi
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        Sb[i, j] += Sbi[i, j];


                // Store some additional information
                this.classScatter[c] = Swi;
                this.classCount[c] = count;
                this.classMeans[c] = mean;
                this.classStdDevs[c] = Statistics.Tools.StandardDeviation(subset, mean);
            }


            // Compute the generalized eigenvalue decomposition
            GeneralizedEigenvalueDecomposition gevd = new GeneralizedEigenvalueDecomposition(Sb, Sw);

            // Get the eigenvalues and corresponding eigenvectors
            double[] evals = gevd.RealEigenvalues;
            double[,] eigs = gevd.Eigenvectors;

            // Sort eigenvalues and vectors in descending order
            eigs = Matrix.Sort(evals, eigs, new GeneralComparer(ComparerDirection.Descending, true));


            // Store information
            this.Eigenvalues = evals;
            this.DiscriminantMatrix = eigs;

            // Create projections into latent space
            this.result = source.Multiply(eigenvectors);


            // Compute feature space means for later classification
            for (int c = 0; c < Classes.Count; c++)
            {
                projectedMeans[c] = classMeans[c].Multiply(eigs);
            }


            // Computes additional information about the analysis and creates the
            //  object-oriented structure to hold the discriminants found.
            CreateDiscriminants();
        }

        /// <summary>
        ///   Projects a given matrix into discriminant space.
        /// </summary>
        /// 
        /// <param name="data">The matrix to be projected.</param>
        /// 
        public double[,] Transform(double[,] data)
        {
            return Transform(data, discriminantCollection.Count);
        }

        /// <summary>
        ///   Projects a given matrix into discriminant space.
        /// </summary>
        /// 
        /// <param name="data">The matrix to be projected.</param>
        /// 
        public double[][] Transform(double[][] data)
        {
            return Transform(data, discriminantCollection.Count);
        }

        /// <summary>
        ///   Projects a given matrix into latent discriminant variable space.
        /// </summary>
        /// 
        /// <param name="data">The matrix to be projected.</param>
        /// <param name="dimensions">
        ///   The number of discriminants to use in the projection.
        /// </param>
        /// 
        public virtual double[,] Transform(double[,] data, int dimensions)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (eigenvectors == null)
                throw new InvalidOperationException("The analysis must have been computed first.");

            if (data.GetLength(1) != source.GetLength(1))
                throw new DimensionMismatchException("data", "The input data should have the same number of columns as the original data.");

            if (dimensions < 0 || dimensions > Discriminants.Count)
            {
                throw new ArgumentOutOfRangeException("dimensions",
                    "The specified number of dimensions must be equal or less than the " +
                    "number of discriminants available in the Discriminants collection property.");
            }

            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            // multiply the data matrix by the selected eigenvectors
            // TODO: Use cache-friendly multiplication
            double[,] r = new double[rows, dimensions];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < dimensions; j++)
                    for (int k = 0; k < cols; k++)
                        r[i, j] += data[i, k] * eigenvectors[k, j];

            return r;
        }

        /// <summary>
        ///   Projects a given matrix into latent discriminant variable space.
        /// </summary>
        /// 
        /// <param name="data">The matrix to be projected.</param>
        /// <param name="dimensions">
        ///   The number of discriminants to use in the projection.
        /// </param>
        /// 
        public virtual double[][] Transform(double[][] data, int dimensions)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (eigenvectors == null)
                throw new InvalidOperationException("The analysis must have been computed first.");

            for (int i = 0; i < data.Length; i++)
                if (data[i].Length != source.GetLength(1))
                    throw new DimensionMismatchException("data", "The input data should have the same number of columns as the original data.");

            if (dimensions < 0 || dimensions > Discriminants.Count)
            {
                throw new ArgumentOutOfRangeException("dimensions",
                    "The specified number of dimensions must be equal or less than the " +
                    "number of discriminants available in the Discriminants collection property.");
            }

            int rows = data.Length;
            int cols = data[0].Length;

            // multiply the data matrix by the selected eigenvectors
            // TODO: Use cache-friendly multiplication
            double[][] r = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                r[i] = new double[dimensions];
                for (int j = 0; j < dimensions; j++)
                    for (int k = 0; k < cols; k++)
                        r[i][j] += data[i][k] * eigenvectors[k, j];
            }

            return r;
        }

        /// <summary>
        ///   Projects a given point into discriminant space.
        /// </summary>
        /// 
        /// <param name="data">The point to be projected.</param>
        /// 
        public double[] Transform(double[] data)
        {
            return Transform(data.ToMatrix()).GetRow(0);
        }

        /// <summary>
        ///   Projects a given point into latent discriminant variable space.
        /// </summary>
        /// 
        /// <param name="data">The point to be projected.</param>
        /// <param name="discriminants">The number of discriminant variables to use in the projection.</param>
        /// 
        public double[] Transform(double[] data, int discriminants)
        {
            return Transform(data.ToMatrix(), discriminants).GetRow(0);
        }

        /// <summary>
        ///   Returns the minimum number of discriminant space dimensions (discriminant
        ///   factors) required to represent a given percentile of the data.
        /// </summary>
        /// 
        /// <param name="threshold">The percentile of the data requiring representation.</param>
        /// <returns>The minimal number of dimensions required.</returns>
        /// 
        public int GetNumberOfDimensions(float threshold)
        {
            if (threshold < 0 || threshold > 1.0)
                throw new ArgumentException("Threshold should be a value between 0 and 1", "threshold");

            for (int i = 0; i < discriminantCumulative.Length; i++)
            {
                if (discriminantCumulative[i] >= threshold)
                    return i + 1;
            }

            return discriminantCumulative.Length;
        }

        /// <summary>
        ///   Classifies a new instance into one of the available classes.
        /// </summary>
        /// 
        public int Classify(double[] input)
        {
            double[] projection = Transform(input);

            // Select class which higher discriminant function fy
            int imax = 0;
            double max = DiscriminantFunction(0, projection);
            for (int i = 1; i < classCollection.Count; i++)
            {
                double fy = DiscriminantFunction(i, projection);
                if (fy > max)
                {
                    max = fy;
                    imax = i;
                }
            }

            return classCollection[imax].Number;
        }

        /// <summary>
        ///   Classifies a new instance into one of the available classes.
        /// </summary>
        /// 
        public int Classify(double[] input, out double[] responses)
        {
            double[] projection = Transform(input);
            responses = new double[classCollection.Count];

            // Select class which higher discriminant function fy
            int imax = 0;
            double max = DiscriminantFunction(0, projection);
            responses[0] = max;
            for (int i = 1; i < classCollection.Count; i++)
            {
                double fy = DiscriminantFunction(i, projection);
                responses[i] = fy;
                if (fy > max)
                {
                    max = fy;
                    imax = i;
                }
            }

            return classCollection[imax].Number;
        }

        /// <summary>
        ///   Classifies new instances into one of the available classes.
        /// </summary>
        /// 
        public int[] Classify(double[][] inputs)
        {
            int[] output = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                output[i] = Classify(inputs[i]);
            return output;
        }

        /// <summary>
        ///   Gets the discriminant function output for class c.
        /// </summary>
        /// 
        /// <param name="c">The class index.</param>
        /// <param name="projection">The projected input.</param>
        /// 
        internal double DiscriminantFunction(int c, double[] projection)
        {
            return -Distance.SquareEuclidean(projection, projectedMeans[c]);
        }
        #endregion


        //---------------------------------------------


        #region Protected Methods
        /// <summary>
        ///   Creates additional information about principal components.
        /// </summary>
        /// 
        protected void CreateDiscriminants()
        {
            int numDiscriminants = eigenvalues.Length;
            discriminantProportions = new double[numDiscriminants];
            discriminantCumulative = new double[numDiscriminants];


            // Calculate total scatter matrix
            int size = Sw.GetLength(0);
            St = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    St[i, j] = Sw[i, j] + Sb[i, j];


            // Calculate proportions
            double sum = 0.0;
            for (int i = 0; i < numDiscriminants; i++)
                sum += System.Math.Abs(eigenvalues[i]);
            sum = (sum == 0) ? 0 : (1.0 / sum);

            for (int i = 0; i < numDiscriminants; i++)
                discriminantProportions[i] = System.Math.Abs(eigenvalues[i]) * sum;


            // Calculate cumulative proportions
            if (numDiscriminants > 0)
            {
                this.discriminantCumulative[0] = this.discriminantProportions[0];
                for (int i = 1; i < this.discriminantCumulative.Length; i++)
                    this.discriminantCumulative[i] = this.discriminantCumulative[i - 1] + this.discriminantProportions[i];
            }


            // Creates the object-oriented structure to hold the linear discriminants
            Discriminant[] discriminants = new Discriminant[numDiscriminants];
            for (int i = 0; i < numDiscriminants; i++)
                discriminants[i] = new Discriminant(this, i);
            this.discriminantCollection = new DiscriminantCollection(discriminants);
        }
        #endregion

    }


    #region Support Classes
    /// <summary>
    ///   Represents a class found during Discriminant Analysis, allowing it to
    ///   be bound to controls like the DataGridView.
    ///   
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class DiscriminantAnalysisClass
    {
        private LinearDiscriminantAnalysis analysis;
        private int classNumber;
        private int index;

        /// <summary>
        ///   Creates a new Class representation
        /// </summary>
        /// 
        internal DiscriminantAnalysisClass(LinearDiscriminantAnalysis analysis, int index, int classNumber)
        {
            this.analysis = analysis;
            this.index = index;
            this.classNumber = classNumber;
        }

        /// <summary>
        ///   Gets the Index of this class on the original analysis collection.
        /// </summary>
        /// 
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        ///   Gets the number labeling this class.
        /// </summary>
        /// 
        public int Number
        {
            get { return classNumber; }
        }

        /// <summary>
        ///   Gets the prevalence of the class on the original data set.
        /// </summary>
        /// 
        public double Prevalence
        {
            get { return (double)Count / analysis.Source.GetLength(0); }
        }

        /// <summary>
        ///   Gets the class' mean vector.
        /// </summary>
        /// 
        public double[] Mean
        {
            get { return analysis.classMeans[index]; }
        }

        /// <summary>
        ///   Gets the feature-space means of the projected data.
        /// </summary>
        /// 
        public double[] ProjectionMean
        {
            get { return analysis.projectedMeans[index]; }
        }

        /// <summary>
        ///   Gets the class' standard deviation vector.
        /// </summary>
        /// 
        public double[] StandardDeviation
        {
            get { return analysis.classStdDevs[index]; }
        }

        /// <summary>
        ///   Gets the Scatter matrix for this class.
        /// </summary>
        /// 
        public double[,] Scatter
        {
            get { return analysis.classScatter[index]; }
        }

        /// <summary>
        ///   Gets the indices of the rows in the original data which belong to this class.
        /// </summary>
        /// 
        public int[] Indices
        {
            get { return Matrix.Find(analysis.Classifications, y => y == classNumber); }
        }

        /// <summary>
        ///   Gets the subset of the original data spawned by this class.
        /// </summary>
        /// 
        public double[,] Subset
        {
            get { return analysis.Source.Submatrix(Indices); }
        }

        /// <summary>
        ///   Gets the number of observations inside this class.
        /// </summary>
        /// 
        public int Count
        {
            get { return analysis.classCount[index]; }
        }

        /// <summary>
        ///   Discriminant function for the class.
        /// </summary>
        /// 
        public double DiscriminantFunction(double[] projection)
        {
            //return Mean.Multiply(projection) + Bias[index];
            return analysis.DiscriminantFunction(index, projection);
        }
    }

    /// <summary>
    /// <para>
    ///   Represents a discriminant factor found during Discriminant Analysis,
    ///   allowing it to be bound to controls like the DataGridView.</para>
    /// <para>
    ///   This class cannot be instantiated.</para>  
    /// </summary>
    /// 
    [Serializable]
    public class Discriminant : IAnalysisComponent
    {
        private LinearDiscriminantAnalysis analysis;
        private int index;

        /// <summary>
        ///   Creates a new discriminant factor representation.
        /// </summary>
        /// 
        internal Discriminant(LinearDiscriminantAnalysis analysis, int index)
        {
            this.analysis = analysis;
            this.index = index;
        }

        /// <summary>
        ///   Gets the index of this discriminant factor
        ///   on the original analysis collection.
        /// </summary>
        /// 
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        ///   Gets the Eigenvector for this discriminant factor.
        /// </summary>
        /// 
        public double[] Eigenvector
        {
            get { return analysis.DiscriminantMatrix.GetColumn(index); }
        }

        /// <summary>
        ///   Gets the Eigenvalue for this discriminant factor.
        /// </summary>
        /// 
        public double Eigenvalue
        {
            get { return analysis.Eigenvalues[index]; }
        }

        /// <summary>
        ///   Gets the proportion, or amount of information explained by this discriminant factor.
        /// </summary>
        /// 
        public double Proportion
        {
            get { return analysis.DiscriminantProportions[index]; }
        }

        /// <summary>
        ///   Gets the cumulative proportion of all discriminant factors until this factor.
        /// </summary>
        /// 
        public double CumulativeProportion
        {
            get { return analysis.CumulativeProportions[index]; }
        }

    }

    /// <summary>
    /// <para>
    ///   Represents a collection of Discriminants factors found in the Discriminant Analysis.</para>
    /// <para>
    ///   This class cannot be instantiated.</para>
    /// </summary>
    /// 
    [Serializable]
    public class DiscriminantCollection : ReadOnlyCollection<Discriminant>
    {
        internal DiscriminantCollection(Discriminant[] components)
            : base(components)
        {
        }
    }

    /// <summary>
    /// <para>
    ///   Represents a collection of classes found in the Discriminant Analysis.</para>
    /// <para>
    ///   This class cannot be instantiated.</para>  
    /// </summary>
    /// 
    [Serializable]
    public class DiscriminantAnalysisClassCollection : ReadOnlyCollection<DiscriminantAnalysisClass>
    {
        internal DiscriminantAnalysisClassCollection(DiscriminantAnalysisClass[] components)
            : base(components) { }
    }

    #endregion

}


