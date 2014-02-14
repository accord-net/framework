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
    using Accord.Math.Decompositions;
    using Accord.Statistics.Kernels;
    using Accord.Math.Comparers;

    /// <summary>
    ///   Kernel Principal Component Analysis.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Kernel principal component analysis (kernel PCA) is an extension of principal
    ///   component analysis (PCA) using techniques of kernel methods. Using a kernel,
    ///   the originally linear operations of PCA are done in a reproducing kernel Hilbert
    ///   space with a non-linear mapping.</para>
    /// 
    /// <para>
    ///   This class can also be bound to standard controls such as the 
    ///   <a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx">DataGridView</a>
    ///   by setting their DataSource property to the analysis' <see cref="PrincipalComponentAnalysis.Components"/>
    ///   property.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.heikohoffmann.de/htmlthesis/hoffmann_diss.html">
    ///       Heiko Hoffmann, Unsupervised Learning of Visuomotor Associations (Kernel PCA topic).
    ///       PhD thesis. 2005. Available on: http://www.heikohoffmann.de/htmlthesis/hoffmann_diss.html </a>
    ///       </description></item>
    ///     <item><description><a href="http://www.hpl.hp.com/conferences/icml2003/papers/345.pdf">
    ///       James T. Kwok, Ivor W. Tsang. The Pre-Image Problem in Kernel Methods. 2003. Available on:
    ///       http://www.hpl.hp.com/conferences/icml2003/papers/345.pdf </a></description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    ///<example>
    ///  <para>
    ///    The example below shows a typical usage of the analysis. We will be replicating
    ///    the exact same example which can be found on the <see cref="PrincipalComponentAnalysis"/>
    ///    documentation page. However, while we will be using a <see cref="Linear"/> kernel,
    ///    any other kernel function could have been used.</para>
    ///    
    ///  <code>
    ///  // Below is the same data used on the excellent paper "Tutorial
    ///  //   On Principal Component Analysis", by Lindsay Smith (2002).
    ///  
    ///  double[,] sourceMatrix = 
    ///  {
    ///      { 2.5,  2.4 },
    ///      { 0.5,  0.7 },
    ///      { 2.2,  2.9 },
    ///      { 1.9,  2.2 },
    ///      { 3.1,  3.0 },
    ///      { 2.3,  2.7 },
    ///      { 2.0,  1.6 },
    ///      { 1.0,  1.1 },
    ///      { 1.5,  1.6 },
    ///      { 1.1,  0.9 }
    ///  }; 
    /// 
    ///  // Create a new linear kernel
    ///  IKernel kernel = new Linear();
    ///  
    ///  // Creates the Kernel Principal Component Analysis of the given data
    ///  var kpca = new KernelPrincipalComponentAnalysis(sourceMatrix, kernel);
    ///    
    ///  // Compute the Kernel Principal Component Analysis
    ///  kpca.Compute();
    ///    
    ///  // Creates a projection considering 80% of the information
    ///  double[,] components = kpca.Transform(sourceMatrix, 0.8f);
    ///  </code>
    /// </example>
    /// 
    [Serializable]
    public class KernelPrincipalComponentAnalysis : PrincipalComponentAnalysis
    {

        private IKernel kernel;
        private double[,] sourceCentered;
        private double[,] kernelMatrix;
        private bool centerFeatureSpace;
        private double threshold = 0.001; // 0.001


        //---------------------------------------------


        #region Constructor
        /// <summary>
        ///   Constructs the Kernel Principal Component Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// <param name="method">The analysis method to perform.</param>
        /// <param name="centerInFeatureSpace">True to center the data in feature space, false otherwise. Default is true.</param>
        /// 
        public KernelPrincipalComponentAnalysis(double[,] data, IKernel kernel,
            AnalysisMethod method, bool centerInFeatureSpace)
            : base(data, method)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            this.kernel = kernel;
            this.centerFeatureSpace = centerInFeatureSpace;
        }

        /// <summary>
        ///   Constructs the Kernel Principal Component Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// <param name="method">The analysis method to perform.</param>
        /// <param name="centerInFeatureSpace">True to center the data in feature space, false otherwise. Default is true.</param>
        /// 
        public KernelPrincipalComponentAnalysis(double[][] data, IKernel kernel,
            AnalysisMethod method, bool centerInFeatureSpace)
            : base(data, method)
        {
            if (kernel == null) 
                throw new ArgumentNullException("kernel");

            this.kernel = kernel;
            this.centerFeatureSpace = centerInFeatureSpace;
        }

        /// <summary>
        ///   Constructs the Kernel Principal Component Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// <param name="method">The analysis method to perform.</param>
        /// 
        public KernelPrincipalComponentAnalysis(double[,] data, IKernel kernel, AnalysisMethod method)
            : this(data, kernel, method, true) { }

        /// <summary>
        ///   Constructs the Kernel Principal Component Analysis.
        /// </summary>
        /// 
        /// <param name="data">The source data to perform analysis. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// <param name="method">The analysis method to perform.</param>
        /// 
        public KernelPrincipalComponentAnalysis(double[][] data, IKernel kernel, AnalysisMethod method)
            : this(data, kernel, method, true) { }

        /// <summary>Constructs the Kernel Principal Component Analysis.</summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// 
        public KernelPrincipalComponentAnalysis(double[,] data, IKernel kernel)
            : this(data, kernel, AnalysisMethod.Center, true) { }

        /// <summary>Constructs the Kernel Principal Component Analysis.</summary>
        /// 
        /// <param name="data">The source data to perform analysis.</param>
        /// <param name="kernel">The kernel to be used in the analysis.</param>
        /// 
        public KernelPrincipalComponentAnalysis(double[][] data, IKernel kernel)
            : this(data, kernel, AnalysisMethod.Center, true) { }
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
        ///   Gets or sets whether the points should be centered in feature space.
        /// </summary>
        /// 
        public bool Center
        {
            get { return centerFeatureSpace; }
            set { centerFeatureSpace = value; }
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
            set { threshold = value; }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods
        /// <summary>
        ///   Computes the Kernel Principal Component Analysis algorithm.
        /// </summary>
        /// 
        public override void Compute()
        {
            Compute(Source.GetLength(0));
        }

        /// <summary>
        ///   Computes the Kernel Principal Component Analysis algorithm.
        /// </summary>
        /// 
        public void Compute(int components)
        {
            if (components < 0 || components > Source.GetLength(0))
            {
                throw new ArgumentException(
                    "The number of components must be between 0 and " +
                    "the number of rows in your source data matrix.");
            }

            int dimension = Source.GetLength(0);

            // If needed, center the source matrix
            sourceCentered = Adjust(Source, Overwrite);


            // Create the Gram (Kernel) Matrix
            this.kernelMatrix = new double[dimension, dimension];
            for (int i = 0; i < dimension; i++)
            {
                double[] row = sourceCentered.GetRow(i);
                for (int j = i; j < dimension; j++)
                {
                    double k = kernel.Function(row, sourceCentered.GetRow(j));
                    kernelMatrix[i, j] = k; // Kernel matrix is symmetric
                    kernelMatrix[j, i] = k;
                }
            }


            // Center the Gram (Kernel) Matrix if requested
            double[,] Kc = centerFeatureSpace ? centerKernel(kernelMatrix) : kernelMatrix;


            // Perform the Eigenvalue Decomposition (EVD) of the Kernel matrix
            EigenvalueDecomposition evd = new EigenvalueDecomposition(Kc, assumeSymmetric: true);

            // Gets the Eigenvalues and corresponding Eigenvectors
            double[] evals = evd.RealEigenvalues;
            double[,] eigs = evd.Eigenvectors;


            // Sort eigenvalues and vectors in descending order
            eigs = Matrix.Sort(evals, eigs, new GeneralComparer(ComparerDirection.Descending, true));


            // Eliminate unwanted components
            if (components != Source.GetLength(0))
            {
                eigs = eigs.Submatrix(null, 0, components - 1);
                evals = evals.Submatrix(0, components - 1);
            }

            if (threshold > 0)
            {
                // We will be discarding less important
                // eigenvectors to conserve memory.

                // Calculate component proportions
                double sum = 0.0; // total variance
                for (int i = 0; i < evals.Length; i++)
                    sum += Math.Abs(evals[i]);

                if (sum > 0)
                {
                    int keep = 0;

                    // Now we will detect how many components we have
                    //  have to keep in order to achieve the level of
                    //  explained variance specified by the threshold.

                    while (keep < components)
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

                    if (keep != components)
                    {
                        if (keep > 0)
                        {
                            // Resize the vectors keeping only needed components
                            eigs = eigs.Submatrix(0, components - 1, 0, keep - 1);
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
            }


            // Normalize eigenvectors
            if (centerFeatureSpace)
            {
                for (int j = 0; j < evals.Length; j++)
                {
                    double val = Math.Sqrt(Math.Abs(evals[j]));
                    for (int i = 0; i < eigs.GetLength(0); i++)
                        eigs[i, j] = eigs[i, j] / val;
                }
            }



            // Set analysis properties
            this.SingularValues = new double[evals.Length];
            this.Eigenvalues = evals;
            this.ComponentMatrix = eigs;


            // Project the original data into principal component space
            this.Result = Kc.Multiply(eigs);


            // Computes additional information about the analysis and creates the
            //  object-oriented structure to hold the principal components found.
            CreateComponents();
        }


        /// <summary>
        ///   Projects a given matrix into the principal component space.
        /// </summary>
        /// 
        /// <param name="data">The matrix to be projected. The matrix should contain
        /// variables as columns and observations of each variable as rows.</param>
        /// <param name="dimensions">The number of components to use in the transformation.</param>
        /// 
        public override double[,] Transform(double[,] data, int dimensions)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (sourceCentered == null)
                throw new InvalidOperationException("The analysis must have been computed first.");

            if (data.GetLength(1) != Source.GetLength(1))
                throw new DimensionMismatchException("data", "The input data should have the same number of columns as the original data.");

            if (dimensions < 0 || dimensions > Components.Count)
            {
                throw new ArgumentOutOfRangeException("dimensions",
                    "The specified number of dimensions must be equal or less than the " +
                    "number of components available in the Components collection property.");
            }

            int samples = data.GetLength(0);
            int rows = sourceCentered.GetLength(0);

            // Center the data
            data = Adjust(data, false);

            // Create the Kernel matrix
            double[,] newK = new double[samples, rows];
            for (int i = 0; i < samples; i++)
            {
                double[] row = data.GetRow(i);
                for (int j = 0; j < rows; j++)
                    newK[i, j] = kernel.Function(row, sourceCentered.GetRow(j));
            }

            if (centerFeatureSpace)
                centerKernel(newK, kernelMatrix);

            // Project into the kernel principal components
            double[,] result = new double[samples, dimensions];
            newK.Multiply(ComponentMatrix, result: result);

            return result;
        }

        /// <summary>
        ///   Reverts a set of projected data into it's original form. Complete reverse
        ///   transformation is not always possible and is not even guaranteed to exist.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method works using a closed-form MDS approach as suggested by
        ///   Kwok and Tsang. It is currently a direct implementation of the algorithm
        ///   without any kind of optimization.
        ///   
        ///   Reference:
        ///   - http://cmp.felk.cvut.cz/cmp/software/stprtool/manual/kernels/preimage/list/rbfpreimg3.html
        /// </remarks>
        /// 
        /// <param name="data">The kpca-transformed data.</param>
        /// 
        public override double[,] Revert(double[,] data)
        {
            return Revert(data, 10);
        }

        /// <summary>
        ///   Reverts a set of projected data into it's original form. Complete reverse
        ///   transformation is not always possible and is not even guaranteed to exist.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This method works using a closed-form MDS approach as suggested by
        ///   Kwok and Tsang. It is currently a direct implementation of the algorithm
        ///   without any kind of optimization.
        /// </para>
        /// <para>
        ///   Reference:
        ///   - http://cmp.felk.cvut.cz/cmp/software/stprtool/manual/kernels/preimage/list/rbfpreimg3.html
        /// </para>
        /// </remarks>
        /// 
        /// <param name="data">The kpca-transformed data.</param>
        /// <param name="neighbors">The number of nearest neighbors to use while constructing the pre-image.</param>
        /// 
        public double[,] Revert(double[,] data, int neighbors)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (sourceCentered == null)
                throw new InvalidOperationException("The analysis must have been computed first.");

            if (neighbors < 2)
                throw new ArgumentOutOfRangeException("neighbors", "At least two neighbors are necessary.");

            // Verify if the current kernel supports
            // distance calculation in feature space.
            IDistance distance = kernel as IDistance;

            if (distance == null)
                throw new NotSupportedException("Current kernel does not support distance calculation in feature space.");


            int rows = data.GetLength(0);

            double[,] reversion = new double[rows, sourceCentered.GetLength(1)];

            // number of neighbors cannot exceed the number of training vectors.
            int nn = System.Math.Min(neighbors, sourceCentered.GetLength(0));


            // For each point to be reversed
            for (int p = 0; p < rows; p++)
            {
                // 1. Get the point in feature space
                double[] y = data.GetRow(p);

                // 2. Select nn nearest neighbors of the feature space
                double[,] X = sourceCentered;
                double[] d2 = new double[Result.GetLength(0)];
                int[] inx = new int[Result.GetLength(0)];

                // 2.1 Calculate distances
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    inx[i] = i;
                    d2[i] = distance.Distance(y, Result.GetRow(i).Submatrix(y.Length));
                }

                // 2.2 Order them
                Array.Sort(d2, inx);

                // 2.3 Select nn neighbors
                inx = inx.Submatrix(nn);
                X = X.Submatrix(inx).Transpose(); // X is in input space
                d2 = d2.Submatrix(nn);       // distances in input space


                // 3. Perform SVD
                //    [U,L,V] = svd(X*H);

                // TODO: If X has more columns than rows, the SV decomposition should be
                //  computed on the transpose of X and the left and right vectors should
                //  be swapped. This should be fixed after more unit tests are elaborated.
                SingularValueDecomposition svd = new SingularValueDecomposition(X);
                double[,] U = svd.LeftSingularVectors;
                double[,] L = Matrix.Diagonal(nn, svd.Diagonal);
                double[,] V = svd.RightSingularVectors;


                // 4. Compute projections
                //    Z = L*V';
                double[,] Z = L.Multiply(V.Transpose());


                // 5. Calculate distances
                //    d02 = sum(Z.^2)';
                double[] d02 = Matrix.Sum(Matrix.ElementwisePower(Z, 2));


                // 6. Get the pre-image using z = -0.5*inv(Z')*(d2-d02)
                double[,] inv = Matrix.PseudoInverse(Z.Transpose());

                double[] z = (-0.5).Multiply(inv).Multiply(d2.Subtract(d02)).Submatrix(U.GetLength(0));


                // 8. Project the pre-image on the original basis
                //    using x = U*z + sum(X,2)/nn;
                double[] x = (U.Multiply(z)).Add(Matrix.Sum(X.Transpose()).Multiply(1.0 / nn));


                // 9. Store the computed pre-image.
                for (int i = 0; i < reversion.GetLength(1); i++)
                    reversion[p, i] = x[i];
            }



            // if the data has been standardized or centered,
            //  we need to revert those operations as well
            if (this.Method == AnalysisMethod.Standardize)
            {
                // multiply by standard deviation and add the mean
                for (int i = 0; i < reversion.GetLength(0); i++)
                    for (int j = 0; j < reversion.GetLength(1); j++)
                        reversion[i, j] = (reversion[i, j] * StandardDeviations[j]) + Means[j];
            }
            else if (this.Method == AnalysisMethod.Center)
            {
                // only add the mean
                for (int i = 0; i < reversion.GetLength(0); i++)
                    for (int j = 0; j < reversion.GetLength(1); j++)
                        reversion[i, j] = reversion[i, j] + Means[j];
            }


            return reversion;
        }

        #endregion


        //---------------------------------------------


        #region Private Methods
        private static double[,] centerKernel(double[,] K)
        {
            int rows = K.GetLength(0);
            double[,] Kc = new double[rows, rows];

            double[] rowMean = Accord.Statistics.Tools.Mean(K, 1);
#if DEBUG
            // row mean and column means should be equal on a symmetric matrix
            double[] colMean = Accord.Statistics.Tools.Mean(K, 0);
            for (int i = 0; i < colMean.Length; i++)
               System.Diagnostics.Debug.Assert(colMean[i] == rowMean[i]);
#endif
            double mean = Accord.Statistics.Tools.Mean(K, -1)[0];

            for (int i = 0; i < rows; i++)
                for (int j = i; j < rows; j++)
                    Kc[i, j] = Kc[j, i] = K[i, j] - rowMean[i] - rowMean[j] + mean;

            return Kc;
        }

        private static void centerKernel(double[,] newK, double[,] K)
        {
            int samples = newK.GetLength(0);
            int dimension = K.GetLength(0);

            double[] rowMean1 = Statistics.Tools.Mean(newK, 1);
            double[] rowMean2 = Statistics.Tools.Mean(K, 1);
            double mean = Matrix.Sum(K, -1)[0] / (samples * dimension);

            for (int i = 0; i < samples; i++)
                for (int j = 0; j < dimension; j++)
                    newK[i, j] = newK[i, j] - rowMean1[i] - rowMean2[j] + mean;
        }

        #endregion


    }

}
