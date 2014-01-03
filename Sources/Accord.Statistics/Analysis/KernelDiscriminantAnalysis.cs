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
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Kernels;
    using System.Collections.Generic;
    using System;
    using Accord.Math.Comparers;

    /// <summary>
    ///   Kernel (Fisher) Discriminant Analysis.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Kernel (Fisher) discriminant analysis (kernel FDA) is a non-linear generalization
    ///   of linear discriminant analysis (LDA) using techniques of kernel methods. Using a
    ///   kernel, the originally linear operations of LDA are done in a reproducing kernel
    ///   Hilbert space with a non-linear mapping.</para>
    /// <para>
    ///   The algorithm used is a multi-class generalization of the original algorithm by
    ///   Mika et al. in Fisher discriminant analysis with kernels (1999).</para>  
    ///   
    /// <para>
    ///   This class can also be bound to standard controls such as the 
    ///   <a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView</a>
    ///   by setting their DataSource property to the analysis' <see cref="LinearDiscriminantAnalysis.Discriminants"/> property.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Mika et al, Fisher discriminant analysis with kernels (1999). Available on
    ///       <a href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.35.9904">
    ///       http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.35.9904 </a></description></item>
    ///  </list></para>  
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
    /// // Now we can chose a kernel function to 
    /// // use, such as a linear kernel function.
    /// IKernel kernel = new Linear();
    /// 
    /// // Then, we will create a KDA using this linear kernel.
    /// var kda = new KernelDiscriminantAnalysis(inputs, output, kernel);
    /// 
    /// kda.Compute(); // Compute the analysis
    /// 
    /// 
    /// // Now we can project the data into KDA space:
    /// double[][] projection = kda.Transform(inputs);
    /// 
    /// // Or perform classification using:
    /// int[] results = kda.Classify(inputs);
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class KernelDiscriminantAnalysis : LinearDiscriminantAnalysis
    {
        private IKernel kernel;
        private double regularization = 1e-4;
        private double threshold = 1e-3;


        //---------------------------------------------


        #region Constructor
        /// <summary>
        ///   Constructs a new Kernel Discriminant Analysis object.
        /// </summary>
        /// 
        /// <param name="inputs">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="output">The labels for each observation row in the input matrix.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// 
        public KernelDiscriminantAnalysis(double[,] inputs, int[] output, IKernel kernel)
            : base(inputs, output)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            this.kernel = kernel;
        }

        /// <summary>
        ///   Constructs a new Kernel Discriminant Analysis object.
        /// </summary>
        /// 
        /// <param name="inputs">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="output">The labels for each observation row in the input matrix.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// 
        public KernelDiscriminantAnalysis(double[][] inputs, int[] output, IKernel kernel)
            : base(inputs, output)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            this.kernel = kernel;
        }
        #endregion


        //---------------------------------------------


        #region Public Properties
        /// <summary>
        ///   Gets the Kernel used in the analysis.
        /// </summary>
        /// 
        public IKernel Kernel
        {
            get { return kernel; }
        }

        /// <summary>
        ///   Gets or sets the regularization parameter to
        ///   avoid non-singularities at the solution.
        /// </summary>
        /// 
        public double Regularization
        {
            get { return regularization; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Value must be positive.");
                regularization = value;
            }
        }

        /// <summary>
        ///   Gets or sets the minimum variance proportion needed to keep a
        ///   discriminant component. If set to zero, all components will be
        ///   kept. Default is 0.001 (all components which contribute less
        ///   than 0.001 to the variance in the data will be discarded).
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("value", "Value must be between 0 and 1.");

                threshold = value;
            }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        /// <summary>
        ///   Computes the Multi-Class Kernel Discriminant Analysis algorithm.
        /// </summary>
        /// 
        public override void Compute()
        {
            // Get some initial information
            int dimension = Source.GetLength(0);
            double[,] source = Source;
            double total = dimension;


            // Create the Gram (Kernel) Matrix
            double[,] K = new double[dimension, dimension];
            for (int i = 0; i < dimension; i++)
            {
                double[] row = source.GetRow(i);
                for (int j = i; j < dimension; j++)
                {
                    double s = kernel.Function(row, source.GetRow(j));
                    K[i, j] = s; // Assume K will be symmetric
                    K[j, i] = s;
                }
            }


            // Compute entire data set measures
            base.Means = Statistics.Tools.Mean(K);
            base.StandardDeviations = Statistics.Tools.StandardDeviation(K, Means);


            // Initialize the kernel analogous scatter matrices
            double[,] Sb = new double[dimension, dimension];
            double[,] Sw = new double[dimension, dimension];


            // For each class
            for (int c = 0; c < Classes.Count; c++)
            {
                // Get the Kernel matrix class subset
                double[,] Kc = K.Submatrix(Classes[c].Indices);
                int count = Kc.GetLength(0);

                // Get the Kernel matrix class mean
                double[] mean = Statistics.Tools.Mean(Kc);


                // Construct the Kernel equivalent of the Within-Class Scatter matrix
                double[,] Swi = Statistics.Tools.Scatter(Kc, mean, (double)count);

                // Sw = Sw + Swi
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        Sw[i, j] += Swi[i, j];


                // Construct the Kernel equivalent of the Between-Class Scatter matrix
                double[] d = mean.Subtract(base.Means);
                double[,] Sbi = Matrix.OuterProduct(d, d).Multiply(total);

                // Sb = Sb + Sbi
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        Sb[i, j] += Sbi[i, j];


                // Store additional information
                base.ClassScatter[c] = Swi;
                base.ClassCount[c] = count;
                base.ClassMeans[c] = mean;
                base.ClassStandardDeviations[c] = Statistics.Tools.StandardDeviation(Kc, mean);
            }


            // Add regularization to avoid singularity
            for (int i = 0; i < dimension; i++)
                Sw[i, i] += regularization;


            // Compute the generalized eigenvalue decomposition
            GeneralizedEigenvalueDecomposition gevd = new GeneralizedEigenvalueDecomposition(Sb, Sw);

            if (gevd.IsSingular) // check validity of the results
            {
                throw new SingularMatrixException("One of the matrices is singular. Please retry " +
                    "the method with a higher regularization constant.");
            }


            // Get the eigenvalues and corresponding eigenvectors
            double[] evals = gevd.RealEigenvalues;
            double[,] eigs = gevd.Eigenvectors;

            // Sort eigenvalues and vectors in descending order
            eigs = Matrix.Sort(evals, eigs, new GeneralComparer(ComparerDirection.Descending, true));


            if (threshold > 0)
            {
                // We will be discarding less important
                // eigenvectors to conserve memory.

                // Calculate component proportions
                double sum = 0.0; // total variance
                for (int i = 0; i < dimension; i++)
                    sum += Math.Abs(evals[i]);

                if (sum > 0)
                {
                    int keep = 0;

                    // Now we will detect how many components we have
                    //  have to keep in order to achieve the level of
                    //  explained variance specified by the threshold.

                    while (keep < dimension)
                    {
                        // Get the variance explained by the component
                        double explainedVariance = Math.Abs(evals[keep]);

                        // Check its proportion
                        double proportion = explainedVariance / sum;

                        // Now, if the component explains an
                        // enough proportion of the variance,
                        if (proportion > threshold)
                            keep++; // We can keep it.
                        else
                            break;  // Otherwise we can stop, since the
                        // components are ordered by variance.
                    }

                    if (keep > 0)
                    {
                        // Resize the vectors keeping only needed components
                        eigs = eigs.Submatrix(0, dimension - 1, 0, keep - 1);
                        evals = evals.Submatrix(0, keep - 1);
                    }
                    else
                    {
                        // No component will be kept.
                        eigs = new double[dimension, 0];
                        evals = new double[0];
                    }
                }
            }


            // Store information
            base.Eigenvalues = evals;
            base.DiscriminantMatrix = eigs;
            base.ScatterBetweenClass = Sb;
            base.ScatterWithinClass = Sw;


            // Project into the kernel discriminant space
            this.Result = K.Multiply(eigs);


            // Compute feature space means for later classification
            for (int c = 0; c < Classes.Count; c++)
            {
                ProjectionMeans[c] = ClassMeans[c].Multiply(eigs);
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
        /// <param name="dimensions">
        ///   The number of discriminant dimensions to use in the projection.
        /// </param>
        /// 
        public override double[,] Transform(double[,] data, int dimensions)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (DiscriminantMatrix == null)
                throw new InvalidOperationException("The analysis must have been computed first.");

            if (data.GetLength(1) != Source.GetLength(1))
                throw new DimensionMismatchException("data", 
                    "The input data should have the same number of columns as the original data.");

            if (dimensions < 0 || dimensions > Discriminants.Count)
            {
                throw new ArgumentOutOfRangeException("dimensions",
                    "The specified number of dimensions must be equal or less than the " +
                    "number of discriminants available in the Discriminants collection property.");
            }

            // Get some information
            int rows = data.GetLength(0);
            int N = Source.GetLength(0);

            // Create the Kernel matrix
            double[,] K = new double[rows, N];
            for (int i = 0; i < rows; i++)
            {
                double[] row = data.GetRow(i);
                for (int j = 0; j < N; j++)
                    K[i, j] = kernel.Function(Source.GetRow(j), row);
            }

            // Project into the kernel discriminant space
            double[,] result = new double[rows, dimensions];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < dimensions; j++)
                    for (int k = 0; k < N; k++)
                        result[i, j] += K[i, k] * DiscriminantMatrix[k, j];

            return result;
        }

        /// <summary>
        ///   Projects a given matrix into discriminant space.
        /// </summary>
        /// 
        /// <param name="data">The matrix to be projected.</param>
        /// <param name="dimensions">
        ///   The number of discriminant dimensions to use in the projection.
        /// </param>
        /// 
        public override double[][] Transform(double[][] data, int dimensions)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (DiscriminantMatrix == null)
                throw new InvalidOperationException("The analysis must have been computed first.");

            for (int i = 0; i < data.Length; i++)
                if (data[i].Length != Source.GetLength(1))
                    throw new DimensionMismatchException("data", 
                        "The input data should have the same number of columns as the original data.");

            if (dimensions < 0 || dimensions > Discriminants.Count)
            {
                throw new ArgumentOutOfRangeException("dimensions",
                    "The specified number of dimensions must be equal or less than the " +
                    "number of discriminants available in the Discriminants collection property.");
            }

            // Get some information
            int rows = data.GetLength(0);
            int N = Source.GetLength(0);

            // Create the Kernel matrix
            double[,] K = new double[rows, N];
            for (int i = 0; i < rows; i++)
            {
                double[] row = data[i];
                for (int j = 0; j < N; j++)
                    K[i, j] = kernel.Function(Source.GetRow(j), row);
            }

            // Project into the kernel discriminant space
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[dimensions];
                for (int j = 0; j < dimensions; j++)
                    for (int k = 0; k < N; k++)
                        result[i][j] += K[i, k] * DiscriminantMatrix[k, j];
            }

            return result;
        }

        #endregion

    }
}
