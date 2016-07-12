// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using System.ComponentModel;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Decompositions;
    using Accord.MachineLearning;
    using System.Threading;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Math.Distances;

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
#pragma warning disable 612, 618
    public class LinearDiscriminantAnalysis : BaseDiscriminantAnalysis,
        IDiscriminantAnalysis, IProjectionAnalysis,
        ISupervisedLearning<LinearDiscriminantAnalysis.Pipeline, double[], int>
#pragma warning restore 612, 618
    {

        /// <summary>
        ///   Constructs a new Linear Discriminant Analysis object.
        /// </summary>
        /// 
        /// <param name="inputs">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="outputs">The labels for each observation row in the input matrix.</param>
        /// 
        [Obsolete("Please pass the 'inputs' and 'outputs' parameters to the Learn method instead.")]
        public LinearDiscriminantAnalysis(double[,] inputs, int[] outputs)
        {
            this.source = inputs;
            this.outputs = outputs;
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
        [Obsolete("Please pass the 'inputs' and 'outputs' parameters to the Learn method instead.")]
        public LinearDiscriminantAnalysis(double[][] inputs, int[] outputs)
        {
            this.source = inputs.ToMatrix();
            this.outputs = outputs;
            init(inputs.ToMatrix(), outputs);
        }

        private void init(double[,] inputs, int[] outputs)
        {
            // Gets the number of classes
            int startingClass = outputs.Min();
            this.NumberOfClasses = outputs.Max() - startingClass + 1;
            this.NumberOfSamples = inputs.Rows();
            this.NumberOfInputs = inputs.Columns();
            this.NumberOfOutputs = inputs.Columns();

            // Store the original data
            this.source = inputs;
            this.outputs = outputs;

            // Creates simple structures to hold information later
            this.classCount = new int[NumberOfClasses];
            this.classMeans = new double[NumberOfClasses][];
            this.classStdDevs = new double[NumberOfClasses][];
            this.classScatter = new double[NumberOfClasses][][];
            this.projectedMeans = new double[NumberOfClasses][];

            // Creates the object-oriented structure to hold information about the classes
            var collection = new DiscriminantAnalysisClass[NumberOfClasses];
            for (int i = 0; i < collection.Length; i++)
                collection[i] = new DiscriminantAnalysisClass(this, i, startingClass + i);
            this.classCollection = new DiscriminantAnalysisClassCollection(collection);
        }


        /// <summary>
        ///   Computes the Multi-Class Linear Discriminant Analysis algorithm.
        /// </summary>
        /// 
        [Obsolete("Please use the Learn method instead.")]
        public virtual void Compute()
        {
            int dimension = NumberOfOutputs;

            // Compute entire data set measures
            Means = Measures.Mean(source, dimension: 0);
            StandardDeviations = Measures.StandardDeviation(source, totalMeans);
            double total = dimension;

            // Initialize the scatter matrices
            this.Sw = Jagged.Zeros(dimension, dimension);
            this.Sb = Jagged.Zeros(dimension, dimension);


            // For each class
            for (int c = 0; c < Classes.Count; c++)
            {
                int[] idx = Matrix.Find(outputs, y => y == Classes[c].Number);

                // Get the class subset
                double[][] subset = source.Submatrix(idx).ToJagged();
                int count = subset.GetLength(0);

                // Get the class mean
                double[] mean = Measures.Mean(subset, dimension: 0);

                // Continue constructing the Within-Class Scatter Matrix
                double[][] Swi = Measures.Scatter(subset, mean, (double)count);

                Sw.Add(Swi, result: Sw); // Sw = Sw + Swi

                // Continue constructing the Between-Class Scatter Matrix
                double[] d = mean.Subtract(totalMeans);
                double[][] Sbi = Jagged.Outer(d, d);
                Sbi.Multiply(total, result: Sbi);

                Sb.Add(Sbi, result: Sb); // Sb = Sb + Sbi

                // Store some additional information
                this.classScatter[c] = Swi;
                this.classCount[c] = count;
                this.classMeans[c] = mean;
                this.classStdDevs[c] = Measures.StandardDeviation(subset, mean);
            }


            // Compute the generalized eigenvalue decomposition
            var gevd = new JaggedGeneralizedEigenvalueDecomposition(Sb, Sw, sort: true);

            // Get the eigenvalues and corresponding eigenvectors
            double[] evals = gevd.RealEigenvalues;
            double[][] eigs = gevd.Eigenvectors;

            // Store information
            this.Eigenvalues = evals;
            base.eigenvectors = eigs.Transpose();

            // Create projections into latent space
            this.result = Matrix.Dot(source, eigs).ToMatrix();

            // Compute feature space means for later classification
            for (int c = 0; c < Classes.Count; c++)
                projectedMeans[c] = classMeans[c].Dot(eigs);

            // Computes additional information about the analysis and creates the
            //  object-oriented structure to hold the discriminants found.
            CreateDiscriminants();
        }

        public override double[] Transform(double[] input, double[] result)
        {
            for (int j = 0; j < eigenvectors.Length; j++)
                for (int k = 0; k < input.Length; k++)
                    result[j] += input[k] * eigenvectors[j][k]; // already inverted
            return result;
        }

        public override double[][] Transform(double[][] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                for (int j = 0; j < eigenvectors.Length; j++)
                    for (int k = 0; k < input[i].Length; k++)
                        result[i][j] += input[i][k] * eigenvectors[j][k]; // already inverted
            return result;
        }





        public CancellationToken Token { get; set; }

        public Pipeline Learn(double[][] x, int[] y, double[] weights = null)
        {
            int dimension = NumberOfOutputs;

            // Compute entire data set measures
            Means = Measures.Mean(source, dimension: 0);
            StandardDeviations = Measures.StandardDeviation(source, totalMeans);
            double total = dimension;

            // Initialize the scatter matrices
            this.Sw = Jagged.Zeros(dimension, dimension);
            this.Sb = Jagged.Zeros(dimension, dimension);


            // For each class
            for (int c = 0; c < Classes.Count; c++)
            {
                int[] idx = Matrix.Find(y, y_i => y_i == c);

                // Get the class subset
                double[][] subset = source.Submatrix(idx).ToJagged();
                int count = subset.GetLength(0);

                // Get the class mean
                double[] mean = Measures.Mean(subset, dimension: 0);

                // Continue constructing the Within-Class Scatter Matrix
                double[][] Swi = Measures.Scatter(subset, mean, (double)count);

                Sw.Add(Swi, result: Sw); // Sw = Sw + Swi

                // Continue constructing the Between-Class Scatter Matrix
                double[] d = mean.Subtract(totalMeans);
                double[,] Sbi = Matrix.Outer(d, d).Multiply(total);

                Sb.Add(Sbi, result: Sb); // Sb = Sb + Sbi

                // Store some additional information
                this.classScatter[c] = Swi;
                this.classCount[c] = count;
                this.classMeans[c] = mean;
                this.classStdDevs[c] = Measures.StandardDeviation(subset, mean);
            }


            // Compute the generalized eigenvalue decomposition
            var gevd = new JaggedGeneralizedEigenvalueDecomposition(Sb, Sw, sort: true);

            // Get the eigenvalues and corresponding eigenvectors
            double[] evals = gevd.RealEigenvalues;
            double[][] eigs = gevd.Eigenvectors;

            // Store information
            this.Eigenvalues = evals;
            base.eigenvectors = eigs.Transpose();

            // Compute feature space means for later classification
            for (int c = 0; c < Classes.Count; c++)
                projectedMeans[c] = classMeans[c].Dot(eigs);

            // Computes additional information about the analysis and creates the
            //  object-oriented structure to hold the discriminants found.
            CreateDiscriminants();

            return new Pipeline()
            {
                First = CreateRegression(NumberOfOutputs),
                Second = new MinimumMeanDistanceClassifier()
                {
                    Means = projectedMeans,
                    Distance = new SquareEuclidean()
                }
            };
        }

        public MultivariateLinearRegression CreateRegression(int components)
        {
            double[,] weights = DiscriminantVectors.ToMatrix();

            double[] bias = Means.Dot(weights);
            bias.Multiply(-1, result: bias);

            return new MultivariateLinearRegression(weights, bias, false);
        }

        public class Pipeline : Pipeline<double[], int, MultivariateLinearRegression, MinimumMeanDistanceClassifier>
        {
            public override int[] Transform(double[][] input, int[] result)
            {
                return Second.Transform(First.Transform(input));
            }
        }
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
        private BaseDiscriminantAnalysis analysis;
        private int classNumber;
        private int index;

        /// <summary>
        ///   Creates a new Class representation
        /// </summary>
        /// 
        internal DiscriminantAnalysisClass(BaseDiscriminantAnalysis analysis, int index, int classNumber)
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
            get { return (double)Count / analysis.NumberOfSamples; }
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
        public double[][] Scatter
        {
            get { return analysis.classScatter[index]; }
        }

#pragma warning disable 612, 618
        /// <summary>
        ///   Gets the indices of the rows in the original data which belong to this class.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public int[] Indices
        {
            get { return Matrix.Find(analysis.Classifications, y => y == classNumber); }
        }

        /// <summary>
        ///   Gets the subset of the original data spawned by this class.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Subset
        {

            get { return analysis.Source.Submatrix(Indices); }
        }
#pragma warning restore 612, 618

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
            return analysis.Distance(projection, classIndex: index);
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
        private BaseDiscriminantAnalysis analysis;
        private int index;

        /// <summary>
        ///   Creates a new discriminant factor representation.
        /// </summary>
        /// 
        internal Discriminant(BaseDiscriminantAnalysis analysis, int index)
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
            get { return analysis.DiscriminantVectors[index]; }
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
        [DisplayName("Cumulative")]
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


