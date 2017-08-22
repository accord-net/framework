// Accord Machine Learning Library
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
// The code contained in this class was adapted from Laurens van der Maaten excellent 
// BH T-SNE code from https://github.com/lvdmaaten/bhtsne/. The original license is 
// listed below:
//  
//    Copyright (c) 2014, Laurens van der Maaten (Delft University of Technology)
//    All rights reserved.
//   
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//    3. All advertising materials mentioning features or use of this software
//       must display the following acknowledgement:
//       This product includes software developed by the Delft University of Technology.
//    4. Neither the name of the Delft University of Technology nor the names of 
//       its contributors may be used to endorse or promote products derived from 
//       this software without specific prior written permission.
//   
//    THIS SOFTWARE IS PROVIDED BY LAURENS VAN DER MAATEN ''AS IS'' AND ANY EXPRESS
//    OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
//    OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO 
//    EVENT SHALL LAURENS VAN DER MAATEN BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
//    BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
//    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
//    IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
//    OF SUCH DAMAGE.
//   

namespace Accord.MachineLearning.Clustering
{
#if !MONO

    using Accord.Collections;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Barnes-Hutt t-SNE.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The code contained in this class was adapted from Laurens van der Maaten excellent 
    ///   BH T-SNE code from https://github.com/lvdmaaten/bhtsne/. The original license is 
    ///   listed below:</para>
    /// <code>
    ///    Copyright (c) 2014, Laurens van der Maaten (Delft University of Technology)
    ///    All rights reserved.
    ///   
    ///    Redistribution and use in source and binary forms, with or without
    ///    modification, are permitted provided that the following conditions are met:
    ///    1. Redistributions of source code must retain the above copyright
    ///       notice, this list of conditions and the following disclaimer.
    ///    2. Redistributions in binary form must reproduce the above copyright
    ///       notice, this list of conditions and the following disclaimer in the
    ///       documentation and/or other materials provided with the distribution.
    ///    3. All advertising materials mentioning features or use of this software
    ///       must display the following acknowledgement:
    ///       This product includes software developed by the Delft University of Technology.
    ///    4. Neither the name of the Delft University of Technology nor the names of 
    ///       its contributors may be used to endorse or promote products derived from 
    ///       this software without specific prior written permission.
    ///   
    ///    THIS SOFTWARE IS PROVIDED BY LAURENS VAN DER MAATEN ''AS IS'' AND ANY EXPRESS
    ///    OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
    ///    OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO 
    ///    EVENT SHALL LAURENS VAN DER MAATEN BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
    ///    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
    ///    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
    ///    BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
    ///    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
    ///    IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
    ///    OF SUCH DAMAGE.
    /// </code> 
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\TSNETest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class TSNE : MultipleTransformBase<double[], double>
    {
        const double DBL_MAX = -1.7976931348623157e+308;
        const double DBL_MIN = 2.2250738585072014e-308;

        const double FLT_MIN = 1.175494351e-38F;

        double perplexity = 50;
        double theta = 0.5;


        /// <summary>
        ///   Initializes a new instance of the <see cref="TSNE"/> class.
        /// </summary>
        /// 
        public TSNE()
        {
            NumberOfOutputs = 2;
        }

        /// <summary>
        ///   Gets or sets t-SNE's perplexity value. Default is 50.
        /// </summary>
        /// 
        public double Perplexity
        {
            get { return perplexity; }
            set { perplexity = value; }
        }

        /// <summary>
        ///   Gets or sets t-SNE's Theta value. Default is 0.5
        /// </summary>
        /// 
        public double Theta
        {
            get { return theta; }
            set { theta = value; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double[] Transform(double[] input, double[] result)
        {
            return Transform(new[] { input }, new[] { result })[0];
        }

        /// <summary>
        ///   Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// 
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">The location to where to store the result of this transformation.</param>
        /// <returns>
        ///   The output generated by applying this transformation to the given input.
        /// </returns>
        /// 
        public override double[][] Transform(double[][] input, double[][] result)
        {
            run(input, result, perplexity, theta);
            return result;
        }




        internal static void run(double[][] X, double[][] Y, double perplexity, double theta, bool skip_random_init = false)
        {
            int N = X.Rows();
            int D = X.Columns();
            int no_dims = Y.Columns();

            // Determine whether we are using an exact algorithm
            if (N - 1 < 3 * perplexity)
                throw new Exception(String.Format("Perplexity too large for the number of data points. For {0} points, should be less than {1}", N, (N - 1) / 3.0));

            Debug.Write(String.Format("Using no_dims = {0}, perplexity = {1}, and theta = {2}", no_dims, perplexity, theta));

            bool exact = (theta == 0.0);

            // Set learning parameters
            TimeSpan total_time = TimeSpan.Zero;
            Stopwatch start;
            TimeSpan end;
            int max_iter = 1000;
            int stop_lying_iter = 250;
            int mom_switch_iter = 250;
            double momentum = 0.5;
            double final_momentum = 0.8;
            double eta = 200.0;

            // Allocate some memory
            double[][] dY = Jagged.Create<double>(N, no_dims);
            double[][] uY = Jagged.Create<double>(N, no_dims);
            double[][] gains = Jagged.Ones<double>(N, no_dims);

            // Normalize input data (to prevent numerical problems)
            Debug.Write("Computing input similarities...");
            start = Stopwatch.StartNew();
            Accord.Statistics.Tools.Center(X, inPlace: true);

            X.Divide(X.Max(), result: X);

            // Compute input similarities for exact t-SNE
            double[][] P = null;
            int[] row_P = null;
            int[] col_P = null;
            double[] val_P = null;
            if (exact)
            {
                Trace.Write("Exact?");
                // Compute similarities
                P = Jagged.Create<double>(N, N);

                computeGaussianPerplexity(X, N, D, ref P, perplexity);

                // Symmetrize input similarities
                Debug.Write("Symmetrizing...");
                for (int n = 0; n < N; n++)
                {
                    for (int m = n + 1; m < N; m++)
                    {
                        P[n][m] += P[m][n];
                        P[m][n] = P[n][m];
                    }
                }

                P.Divide(P.Sum(), result: P);
            }

            // Compute input similarities for approximate t-SNE
            else
            {
                // Compute asymmetric pairwise input similarities
                computeGaussianPerplexity(X, N, D, ref row_P, ref col_P, ref val_P, perplexity, (int)(3 * perplexity));

                // Symmetrize input similarities
                symmetrizeMatrix(ref row_P, ref col_P, ref val_P, N);
                double sum_P = 0.0;
                for (int i = 0; i < row_P[N]; i++)
                    sum_P += val_P[i];
                for (int i = 0; i < row_P[N]; i++)
                    val_P[i] /= sum_P;
            }
            end = start.Elapsed;

            // Lie about the P-values
            if (exact)
            {
                P.Multiply(12.0, result: P);
            }
            else
            {
                for (int i = 0; i < row_P[N]; i++)
                    val_P[i] *= 12.0;
            }

            if (!skip_random_init)
            {
                // Initialize solution (randomly)
                for (int i = 0; i < Y.Length; i++)
                    for (int j = 0; j < Y[i].Length; j++)
                        Y[i][j] = randn() * 0.0001;
            }

            // Perform main training loop
            if (exact)
            {
                Debug.Write(String.Format("Input similarities computed in {0} seconds!", end));
                Debug.Write("Learning embedding...");
            }
            else
            {
                Debug.Write(String.Format("Input similarities computed in {0} seconds (sparsity = {1})!", end, (double)row_P[N] / ((double)N * (double)N)));
                Debug.Write("Learning embedding...");
            }

            start = Stopwatch.StartNew();
            for (int iter = 0; iter < max_iter; iter++)
            {
                // Compute (approximate) gradient
                if (exact)
                    computeExactGradient(P, Y, N, no_dims, dY);
                else
                    computeGradient(P, row_P, col_P, val_P, Y, N, no_dims, dY, theta);

                // Update gains
                for (int i = 0; i < gains.Length; i++)
                    for (int j = 0; j < gains[i].Length; j++)
                        gains[i][j] = (System.Math.Sign(dY[i][j]) != System.Math.Sign(uY[i][j])) ? (gains[i][j] + 0.2) : (gains[i][j] * 0.8);

                for (int i = 0; i < gains.Length; i++)
                    for (int j = 0; j < gains[i].Length; j++)
                        if (gains[i][j] < 0.01)
                            gains[i][j] = 0.01;

                // Perform gradient update (with momentum and gains)
                for (int i = 0; i < uY.Length; i++)
                    for (int j = 0; j < uY[i].Length; j++)
                        uY[i][j] = momentum * uY[i][j] - eta * gains[i][j] * dY[i][j];

                for (int i = 0; i < Y.Length; i++)
                    for (int j = 0; j < Y[i].Length; j++)
                        Y[i][j] = Y[i][j] + uY[i][j];

                // Make solution zero-mean
                Accord.Statistics.Tools.Center(Y, inPlace: true);

                // Stop lying about the P-values after a while, and switch momentum
                if (iter == stop_lying_iter)
                {
                    if (exact)
                    {
                        P.Divide(12.0, result: P);
                    }
                    else
                    {
                        for (int i = 0; i < row_P[N]; i++)
                            val_P[i] /= 12.0;
                    }
                }

                if (iter == mom_switch_iter)
                    momentum = final_momentum;

                // Print out progress
                if (iter > 0 && (iter % 50 == 0 || iter == max_iter - 1))
                {
                    end = start.Elapsed;
                    double C = 0.0;
                    if (exact)
                    {
                        C = evaluateError(P, Y, N, no_dims);
                    }
                    else
                    {
                        C = evaluateError(row_P, col_P, val_P, Y, N, no_dims, theta);  // doing approximate computation here!
                    }

                    if (iter == 0)
                    {
                        Debug.WriteLine(String.Format("Iteration {0}: error is {1}", iter + 1, C));
                    }
                    else
                    {
                        total_time += end;
                        Debug.WriteLine(String.Format("Iteration {0}: error is {1} (50 iterations in {2} seconds)", iter, C, end));
                    }
                    start = Stopwatch.StartNew();
                }
            }
            end = start.Elapsed;
            total_time += end;

            Debug.WriteLine(String.Format("Fitting performed in {0} seconds.", total_time));
        }


        // Compute gradient of the t-SNE cost function (using Barnes-Hut algorithm)
        internal static void computeGradient(double[][] P, int[] inp_row_P, int[] inp_col_P, double[] inp_val_P, double[][] Y, int N, int D, double[][] dC, double theta)
        {
            // Construct space-partitioning tree on current map
            var tree = SPTree.FromData(Y);

            // Compute all terms required for t-SNE gradient
            var pos_f = Jagged.Create<double>(N, D);
            var neg_f = Jagged.Create<double>(N, D);
            tree.ComputeEdgeForces(Y, inp_row_P, inp_col_P, inp_val_P, pos_f);

            double sum_Q = 0.0;
            for (int n = 0; n < Y.Length; n++)
                tree.ComputeNonEdgeForces(Y[n], theta, neg_f[n], ref sum_Q);

            // Compute final t-SNE gradient
            for (int i = 0; i < dC.Length; i++)
                for (int j = 0; j < dC[i].Length; j++)
                    dC[i][j] = pos_f[i][j] - (neg_f[i][j] / sum_Q);
        }

        // Compute gradient of the t-SNE cost function (exact)
        internal static void computeExactGradient(double[][] P, double[][] Y, int N, int D, double[][] dC)
        {
            // Make sure the current gradient contains zeros
            for (int i = 0; i < dC.Length; i++)
                for (int j = 0; j < dC[i].Length; j++)
                    dC[i][j] = 0.0;

            // Compute the squared Euclidean distance matrix
            double[][] DD = Jagged.Create<double>(N, N);
            computeSquaredEuclideanDistance(Y, N, D, DD);

            // Compute Q-matrix and normalization sum
            double[][] Q = Jagged.Zeros(N, N);

            double sum_Q = 0.0;
            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    if (n != m)
                    {
                        Q[n][m] = 1.0 / (1.0 + DD[n][m]);
                        sum_Q += Q[n][m];
                    }
                }
            }

            // Perform the computation of the gradient
            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    if (n != m)
                    {
                        double mult = (P[n][m] - (Q[n][m] / sum_Q)) * Q[n][m];
                        for (int d = 0; d < D; d++)
                            dC[n][d] += (Y[n][d] - Y[m][d]) * mult;
                    }
                }
            }
        }


        // Evaluate t-SNE cost function (exactly)
        internal static double evaluateError(double[][] P, double[][] Y, int N, int D)
        {
            // Compute the squared Euclidean distance matrix
            double[][] DD = Jagged.Zeros(N, N);
            double[][] Q = Jagged.Zeros(N, N);
            computeSquaredEuclideanDistance(Y, N, D, DD);

            // Compute Q-matrix and normalization sum
            double sum_Q = DBL_MIN;
            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    if (n != m)
                    {
                        Q[n][m] = 1.0 / (1.0 + DD[n][m]);
                        sum_Q += Q[n][m];
                    }
                    else
                    {
                        Q[n][m] = DBL_MIN;
                    }
                }
            }

            Q.Divide(sum_Q, result: Q);

            // Sum t-SNE error
            double C = 0.0;
            for (int n = 0; n < P.Length; n++)
                for (int j = 0; j < P[n].Length; j++)
                    C += P[n][j] * System.Math.Log((P[n][j] + FLT_MIN) / (Q[n][j] + FLT_MIN));

            return C;
        }

        // Evaluate t-SNE cost function (approximately)
        internal static double evaluateError(int[] row_P, int[] col_P, double[] val_P, double[][] Y, int N, int D, double theta)
        {
            // Get estimate of normalization term
            var tree = SPTree.FromData(Y);
            double[] buff = new double[D];
            double sum_Q = 0.0;
            for (int n = 0; n < Y.Length; n++)
                tree.ComputeNonEdgeForces(Y[n], theta, buff, ref sum_Q);

            // Loop over all edges to compute t-SNE error
            double C = 0.0;
            for (int n = 0; n < N; n++)
            {
                for (int i = row_P[n]; i < row_P[n + 1]; i++)
                {
                    int j = col_P[i];
                    for (int d = 0; d < buff.Length; d++)
                        buff[d] = Y[n][d];
                    for (int d = 0; d < buff.Length; d++)
                        buff[d] -= Y[j][d];

                    double Q = 0.0;
                    for (int d = 0; d < buff.Length; d++)
                        Q += buff[d] * buff[d];
                    Q = (1.0 / (1.0 + Q)) / sum_Q;

                    C += val_P[i] * System.Math.Log((val_P[i] + FLT_MIN) / (Q + FLT_MIN));
                }
            }

            return C;
        }


        // Compute input similarities with a fixed perplexity
        internal static void computeGaussianPerplexity(double[][] X, int N, int D, ref double[][] P, double perplexity)
        {
            // Compute the squared Euclidean distance matrix
            double[][] DD = Jagged.Zeros(N, N);
            computeSquaredEuclideanDistance(X, N, D, DD);

            // Compute the Gaussian kernel row by row
            for (int n = 0; n < P.Length; n++)
            {
                // Initialize some variables
                bool found = false;
                double beta = 1.0;
                double min_beta = -DBL_MAX;
                double max_beta = DBL_MAX;
                double tol = 1e-5;
                double sum_P = 0;

                // Iterate until we found a good perplexity
                int iter = 0;
                while (!found && iter < 200)
                {
                    // Compute Gaussian kernel row
                    for (int m = 0; m < DD[n].Length; m++)
                        P[n][m] = System.Math.Exp(-beta * DD[n][m]);
                    P[n][n] = DBL_MIN;

                    // Compute entropy of current row
                    sum_P = DBL_MIN;
                    for (int m = 0; m < P[n].Length; m++)
                        sum_P += P[n][m];

                    double H = 0.0;
                    for (int m = 0; m < DD[n].Length; m++)
                        H += beta * (DD[n][m] * P[n][m]);
                    H = (H / sum_P) + System.Math.Log(sum_P);

                    // Evaluate whether the entropy is within the tolerance level
                    double Hdiff = H - System.Math.Log(perplexity);

                    if (Hdiff < tol && -Hdiff < tol)
                    {
                        found = true;
                    }
                    else
                    {
                        if (Hdiff > 0)
                        {
                            min_beta = beta;
                            if (max_beta == DBL_MAX || max_beta == -DBL_MAX)
                                beta *= 2.0;
                            else
                                beta = (beta + max_beta) / 2.0;
                        }
                        else
                        {
                            max_beta = beta;
                            if (min_beta == -DBL_MAX || min_beta == DBL_MAX)
                                beta /= 2.0;
                            else
                                beta = (beta + min_beta) / 2.0;
                        }
                    }

                    // Update iteration counter
                    iter++;
                }

                // Row normalize P
                P[n].Divide(sum_P, result: P[n]);
            }
        }


        // Compute input similarities with a fixed perplexity using ball trees (this function allocates memory another function should free)
        internal static void computeGaussianPerplexity(double[][] X, int N, int D, ref int[] _row_P, ref int[] _col_P, ref double[] _val_P, double perplexity, int K)
        {
            if (perplexity > K)
                throw new Exception(String.Format("Perplexity should be lower than K ({0}).", K));

            // Allocate the memory we need
            _row_P = new int[N + 1];
            _col_P = new int[N * K];
            _val_P = new double[N * K];

            int[] row_P = _row_P;
            int[] col_P = _col_P;
            double[] val_P = _val_P;
            double[] cur_P = new double[N - 1];

            row_P[0] = 0;
            for (int n = 0; n < N; n++)
                row_P[n + 1] = row_P[n] + (int)K;

            // Build ball tree on data set
            var idx = Vector.Range(0, N);
            var tree = VPTree.FromData(X, idx, inPlace: false);

            // Loop over all points to find nearest neighbors
            var results = new List<NodeDistance<VPTreeNode<double[], int>>>();

            Debug.Write("Building tree...");
            for (int n = 0; n < X.Length; n++)
            {
                if (n % 10000 == 0)
                    Debug.Write(String.Format(" - point {0} of {1}", n, N));

                // Find nearest neighbors
                results.Clear();
                tree.Nearest(X[n], K + 1, results);

                // Initialize some variables for binary search
                bool found = false;
                double beta = 1.0;
                double min_beta = -DBL_MIN;
                double max_beta = DBL_MAX;
                double tol = 1e-5;

                // Iterate until we found a good perplexity
                int iter = 0;
                double sum_P = 0;
                while (!found && iter < 200)
                {
                    // Compute Gaussian kernel row
                    for (int m = 0; m < K; m++)
                    {
                        double d = results[m + 1].Distance;
                        cur_P[m] = System.Math.Exp(-beta * d * d);
                    }

                    // Compute entropy of current row
                    sum_P = DBL_MIN;
                    for (int m = 0; m < K; m++)
                        sum_P += cur_P[m];

                    double H = 0.0;
                    for (int m = 0; m < K; m++)
                    {
                        double d = results[m + 1].Distance;
                        H += beta * (d * d * cur_P[m]);
                    }
                    H = (H / sum_P) + System.Math.Log(sum_P);

                    // Evaluate whether the entropy is within the tolerance level
                    double Hdiff = H - System.Math.Log(perplexity);

                    if (Hdiff < tol && -Hdiff < tol)
                    {
                        found = true;
                    }
                    else
                    {
                        if (Hdiff > 0)
                        {
                            min_beta = beta;
                            if (max_beta == DBL_MAX || max_beta == -DBL_MAX)
                                beta *= 2.0;
                            else
                                beta = (beta + max_beta) / 2.0;
                        }
                        else
                        {
                            max_beta = beta;
                            if (min_beta == -DBL_MAX || min_beta == DBL_MAX)
                                beta /= 2.0;
                            else
                                beta = (beta + min_beta) / 2.0;
                        }
                    }

                    // Update iteration counter
                    iter++;
                }

                // Row-normalize current row of P and store in matrix
                cur_P.Divide(sum_P, result: cur_P);

                for (int m = 0; m < K; m++)
                {
                    int j = results[m + 1].Node.Value; // holds the index
                    col_P[row_P[n] + m] = j;
                    val_P[row_P[n] + m] = cur_P[m];
                }
            }
        }


        // Symmetrizes a sparse matrix
        internal static void symmetrizeMatrix(ref int[] _row_P, ref int[] _col_P, ref double[] _val_P, int N)
        {
            // Get sparse matrix
            int[] row_P = _row_P;
            int[] col_P = _col_P;
            double[] val_P = _val_P;

            // Count number of elements and row counts of symmetric matrix
            int[] row_counts = new int[N];

            for (int n = 0; n < row_P.Length - 1; n++)
            {
                for (int i = row_P[n]; i < row_P[n + 1]; i++)
                {
                    // Check whether element (col_P[i], n) is present
                    bool present = false;
                    for (int m = row_P[col_P[i]]; m < row_P[col_P[i] + 1]; m++)
                    {
                        if (col_P[m] == n)
                            present = true;
                    }

                    if (present)
                    {
                        row_counts[n]++;
                    }
                    else
                    {
                        row_counts[n]++;
                        row_counts[col_P[i]]++;
                    }
                }
            }

            int no_elem = 0;
            for (int n = 0; n < N; n++)
                no_elem += row_counts[n];

            // Allocate memory for symmetrized matrix
            int[] sym_row_P = new int[N + 1];
            int[] sym_col_P = new int[no_elem];
            double[] sym_val_P = new double[no_elem];

            // Construct new row indices for symmetric matrix
            sym_row_P[0] = 0;
            for (int n = 0; n < N; n++)
                sym_row_P[n + 1] = sym_row_P[n] + (int)row_counts[n];

            // Fill the result matrix
            int[] offset = new int[N];

            for (int n = 0; n < N; n++)
            {
                for (int i = row_P[n]; i < row_P[n + 1]; i++)
                {
                    // considering element(n, col_P[i])

                    // Check whether element (col_P[i], n) is present
                    bool present = false;
                    for (int m = row_P[col_P[i]]; m < row_P[col_P[i] + 1]; m++)
                    {
                        if (col_P[m] == n)
                        {
                            present = true;
                            if (n <= col_P[i])
                            {
                                // make sure we do not add elements twice
                                sym_col_P[sym_row_P[n] + offset[n]] = col_P[i];
                                sym_col_P[sym_row_P[col_P[i]] + offset[col_P[i]]] = n;
                                sym_val_P[sym_row_P[n] + offset[n]] = val_P[i] + val_P[m];
                                sym_val_P[sym_row_P[col_P[i]] + offset[col_P[i]]] = val_P[i] + val_P[m];
                            }
                        }
                    }

                    // If (col_P[i], n) is not present, there is no addition involved
                    if (!present)
                    {
                        sym_col_P[sym_row_P[n] + offset[n]] = col_P[i];
                        sym_col_P[sym_row_P[col_P[i]] + offset[col_P[i]]] = n;
                        sym_val_P[sym_row_P[n] + offset[n]] = val_P[i];
                        sym_val_P[sym_row_P[col_P[i]] + offset[col_P[i]]] = val_P[i];
                    }

                    // Update offsets
                    if (!present || (present && n <= col_P[i]))
                    {
                        offset[n]++;
                        if (col_P[i] != n)
                            offset[col_P[i]]++;
                    }
                }
            }

            // Divide the result by two
            for (int i = 0; i < no_elem; i++)
                sym_val_P[i] /= 2.0;

            // Return symmetrized matrices
            _row_P = sym_row_P;
            _col_P = sym_col_P;
            _val_P = sym_val_P;
        }

        // Compute squared Euclidean distance matrix (using BLAS)
        internal static void computeSquaredEuclideanDistance(double[][] X, int N, int D, double[][] DD)
        {
            double[] dataSums = new double[N];

            for (int n = 0; n < X.Length; n++)
                for (int d = 0; d < X[n].Length; d++)
                    dataSums[n] += (X[n][d] * X[n][d]);

            for (int n = 0; n < DD.Length; n++)
                for (int m = 0; m < DD[n].Length; m++)
                    DD[n][m] = dataSums[n] + dataSums[m];

            for (int n = 0; n < X.Length; n++)
            {
                DD[n][n] = 0.0;
                for (int m = n + 1; m < X.Length; m++)
                {
                    DD[n][m] = 0.0;
                    for (int d = 0; d < D; d++)
                        DD[n][m] += (X[n][d] - X[m][d]) * (X[n][d] - X[m][d]);
                    DD[m][n] = DD[n][m];
                }
            }
        }

        // Generates a Gaussian random number
        private static double randn()
        {
            return NormalDistribution.Standard.Generate();
        }

        // Function that loads data from a t-SNE file
        // Note: this function does a malloc that should be freed elsewhere
        private static bool load_data(ref double[][] data, ref int n, ref int d, ref int no_dims, ref double theta, ref double perplexity, ref int rand_seed)
        {
            // Open file, read first 2 integers, allocate memory, and read the data
            using (BinaryReader h = new BinaryReader(new FileStream("data.dat", FileMode.Open, FileAccess.Read)))
            {
                n = h.ReadInt32();             // number of datapoints
                d = h.ReadInt32();             // original dimensionality
                theta = h.ReadDouble();        // gradient accuracy
                perplexity = h.ReadDouble();   // perplexity
                no_dims = h.ReadInt32();       // output dimensionality

                data = h.ReadJagged<double>(n, d);
                if (h.BaseStream.Position != h.BaseStream.Length)
                    rand_seed = h.ReadInt32();// random seed
            }

            Debug.WriteLine(String.Format("Read the {0} x {1} data matrix successfully!", n, d));
            return true;
        }

        // Function that saves map to a t-SNE file
        private static void save_data(double[][] data, int[] landmarks, double[] costs, int n, int d)
        {
            // Open file, write first 2 integers and then the data
            using (BinaryWriter h = new BinaryWriter(new FileStream("result.dat", FileMode.Create, FileAccess.Write)))
            {
                h.Write(n);
                h.Write(d);
                h.Write(data);
                h.Write(landmarks);
                h.Write(costs);
                Debug.WriteLine(String.Format("Wrote the {0} x {1} data matrix successfully!", n, d));
            }
        }
    }

#endif
}
