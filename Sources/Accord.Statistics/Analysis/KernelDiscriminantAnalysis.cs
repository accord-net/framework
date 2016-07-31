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
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Kernels;
    using Accord.MachineLearning;
    using System.Threading;
    using Models.Regression;

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
    ///   by setting their DataSource property to the analysis' <see cref="BaseDiscriminantAnalysis.Discriminants"/> property.</para>
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
    public class KernelDiscriminantAnalysis : BaseDiscriminantAnalysis,
        ISupervisedLearning<KernelDiscriminantAnalysis.Pipeline, double[], int>
    {
        private IKernel kernel;
        private double regularization = 1e-4;
        private double threshold = 1e-3;
        private double[][] input;

        /// <summary>
        ///   Gets or sets the matrix of original values used to create
        ///   this analysis. Those values are required to build kernel 
        ///  (Gram) matrices when classifying new samples.
        /// </summary>
        /// 
        public double[][] Input
        {
            get { return this.input; }
            set { this.input = value; }
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
        [Obsolete("Please pass the 'inputs' and 'outputs' parameters to the Learn method instead.")]
        public KernelDiscriminantAnalysis(double[,] inputs, int[] output, IKernel kernel)
        {
            this.kernel = kernel;
            init(inputs, output);
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
        [Obsolete("Please pass the 'inputs' and 'outputs' parameters to the Learn method instead.")]
        public KernelDiscriminantAnalysis(double[][] inputs, int[] output, IKernel kernel)
        {
            this.kernel = kernel;
            init(inputs.ToMatrix(), output);
        }


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




        /// <summary>
        ///   Computes the Multi-Class Kernel Discriminant Analysis algorithm.
        /// </summary>
        /// 
        [Obsolete("Please use Learn(x, y) instead.")]
        public void Compute()
        {
            this.input = Source.ToJagged();

            // Get some initial information
            int dimension = Source.GetLength(0);
            double total = dimension;

            // Create the Gram (Kernel) Matrix
            double[,] K = new double[dimension, dimension];
            for (int i = 0; i < dimension; i++)
            {
                double[] row = Source.GetRow(i);
                for (int j = i; j < dimension; j++)
                {
                    double s = kernel.Function(row, Source.GetRow(j));
                    K[i, j] = s; // Assume K will be symmetric
                    K[j, i] = s;
                }
            }


            // Compute entire data set measures
            base.Means = Measures.Mean(K, dimension: 0);
            base.StandardDeviations = Measures.StandardDeviation(K, Means);


            // Initialize the kernel analogous scatter matrices
            double[,] Sb = new double[dimension, dimension];
            double[,] Sw = new double[dimension, dimension];


            // For each class
            for (int c = 0; c < Classes.Count; c++)
            {
                // Get the Kernel matrix class subset
                double[,] Kc = K.Submatrix(Matrix.Find(Classifications, y_i => y_i == Classes[c].Number));
                int count = Kc.GetLength(0);

                // Get the Kernel matrix class mean
                double[] mean = Measures.Mean(Kc, dimension: 0);


                // Construct the Kernel equivalent of the Within-Class Scatter matrix
                double[,] Swi = Measures.Scatter(Kc, mean, (double)count);

                // Sw = Sw + Swi
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        Sw[i, j] += Swi[i, j];


                // Construct the Kernel equivalent of the Between-Class Scatter matrix
                double[] d = mean.Subtract(base.Means);
                double[,] Sbi = Matrix.Outer(d, d).Multiply(total);

                // Sb = Sb + Sbi
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        Sb[i, j] += Sbi[i, j];


                // Store additional information
                base.ClassScatter[c] = Swi.ToJagged();
                base.ClassCount[c] = count;
                base.ClassMeans[c] = mean;
                base.ClassStandardDeviations[c] = Measures.StandardDeviation(Kc, mean);
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
            base.DiscriminantVectors = eigs.ToJagged().Transpose();
            base.ScatterBetweenClass = Sb.ToJagged();
            base.ScatterWithinClass = Sw.ToJagged();

            // Project into the kernel discriminant space
            this.Result = Matrix.Dot(K, eigs);

            // Compute feature space means for later classification
            for (int c = 0; c < Classes.Count; c++)
                ProjectionMeans[c] = ClassMeans[c].Dot(eigs);

            // Computes additional information about the analysis and creates the
            //  object-oriented structure to hold the discriminants found.
            CreateDiscriminants();
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[][] Transform(double[][] input, double[][] result)
        {
            // TODO: Do without forming the kernel matrix
            double[][] K = kernel.ToJagged2(x: input, y: this.input);
            return K.DotWithTransposed(DiscriminantVectors);
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        /// 
        public Pipeline Learn(double[][] x, int[] y, double[] weights = null)
        {
            this.NumberOfInputs = x.Columns();
            this.NumberOfOutputs = y.Max();

            // Create the Gram (Kernel) Matrix
            var K = kernel.ToJagged(x);

            // Compute entire data set measures
            base.Means = Measures.Mean(K, dimension: 0);
            base.StandardDeviations = Measures.StandardDeviation(K, Means);

            // Initialize the kernel analogous scatter matrices
            int dimension = x.Columns();
            double[][] Sb = Jagged.Zeros(dimension, dimension);
            double[][] Sw = Jagged.Zeros(dimension, dimension);

            // For each class
            for (int c = 0; c < Classes.Count; c++)
            {
                var idx = Matrix.Find(y, y_i => y_i == c);

                // Get the Kernel matrix class subset
                double[][] Kc = K.Get(idx);
                int count = Kc.GetLength(0);

                // Get the Kernel matrix class mean
                double[] mean = Measures.Mean(Kc, dimension: 0);

                // Construct the Kernel equivalent of the Within-Class Scatter matrix
                double[][] Swi = Measures.Scatter(Kc, dimension: 0, means: mean);
                Swi.Divide((double)count, result: Swi);
                Sw.Add(Swi, result: Sw); // Sw = Sw + Swi

                // Construct the Kernel equivalent of the Between-Class Scatter matrix
                double[] d = mean.Subtract(base.Means);
                double[][] Sbi = Jagged.Outer(d, d);
                Sbi.Multiply(NumberOfOutputs, result: Sbi);

                Sb.Add(Sbi, result: Sb); // Sb = Sb + Sbi

                // Store additional information
                base.ClassScatter[c] = Swi;
                base.ClassCount[c] = count;
                base.ClassMeans[c] = mean;
                base.ClassStandardDeviations[c] = Measures.StandardDeviation(Kc, mean);
            }

            // Add regularization to avoid singularity
            Sw.AddToDiagonal(regularization, result: Sw);

            // Compute the generalized eigenvalue decomposition
            var gevd = new JaggedGeneralizedEigenvalueDecomposition(Sb, Sw, sort: true);

            if (gevd.IsSingular) // check validity of the results
            {
                throw new SingularMatrixException("One of the matrices is singular. Please retry " +
                    "the method with a higher regularization constant.");
            }

            // Get the eigenvalues and corresponding eigenvectors
            double[] evals = gevd.RealEigenvalues;
            double[][] eigs = gevd.Eigenvectors;

            int nonzero = gevd.Rank;
            if (NumberOfInputs != 0)
                nonzero = Math.Min(nonzero, NumberOfInputs);
            if (NumberOfOutputs != 0)
                nonzero = Math.Min(nonzero, NumberOfOutputs);

            // Eliminate unwanted components
            eigs = eigs.Get(null, 0, nonzero - 1);
            evals = evals.Get(0, nonzero - 1);

            // Store information
            this.input = x;
            base.Eigenvalues = evals;
            base.DiscriminantVectors = eigs.Transpose();
            base.ScatterBetweenClass = Sb;
            base.ScatterWithinClass = Sw;

            // Compute feature space means for later classification
            for (int c = 0; c < Classes.Count; c++)
                ProjectionMeans[c] = ClassMeans[c].Dot(eigs);

            // Computes additional information about the analysis and creates the
            //  object-oriented structure to hold the discriminants found.
            CreateDiscriminants();

            return new Pipeline()
            {
                First = CreateRegression(NumberOfOutputs),
                Second = new MinimumMeanDistanceClassifier()
                {
                    Means = projectedMeans
                }
            };
        }

        private MultivariateKernelRegression CreateRegression(int components)
        {
            double[][] weights = DiscriminantVectors;
            double[] bias = Means.Dot(weights);
            bias.Multiply(-1, result: bias);

            return new MultivariateKernelRegression()
            {
                Weights = weights,
                Intercept = bias,
                Inputs = input
            };
        }

        /// <summary>
        ///   Standard regression and classification pipeline for <see cref="LinearDiscriminantAnalysis"/>.
        /// </summary>
        /// 
        [Serializable]
        public sealed class Pipeline : MulticlassScoreClassifierBase<double[]>
        {
            /// <summary>
            /// Gets or sets the first step in the pipeline.
            /// </summary>
            /// 
            public MultivariateKernelRegression First { get; set; }

            /// <summary>
            /// Gets or sets the second step in the pipeline.
            /// </summary>
            /// 
            public MinimumMeanDistanceClassifier Second { get; set; }

            /// <summary>
            /// Computes a numerical score measuring the association between
            /// the given <paramref name="input" /> vector and each class.
            /// </summary>
            /// <param name="input">The input vector.</param>
            /// <param name="result">An array where the result will be stored,
            /// avoiding unnecessary memory allocations.</param>
            /// <returns></returns>
            public override double[][] Scores(double[][] input, double[][] result)
            {
                return Second.Scores(First.Transform(input), result);
            }
        }
    }
}
