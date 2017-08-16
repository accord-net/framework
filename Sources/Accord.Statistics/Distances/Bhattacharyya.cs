// Accord Math Library
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

namespace Accord.Math.Distances
{
    using Accord.Math.Decompositions;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Runtime.CompilerServices;
    using Accord.Compat;

    /// <summary>
    ///   Bhattacharyya distance.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Bhattacharyya :
        IDistance<double[]>,
        IDistance<double[,]>, IDistance<double[][]>,
        IDistance<GeneralDiscreteDistribution>,
        IDistance<UnivariateDiscreteDistribution>,
        IDistance<MultivariateNormalDistribution>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Bhattacharyya"/> class.
        /// </summary>
        /// 
        public Bhattacharyya()
        {
        }

        /// <summary>
        ///   Bhattacharyya distance between two histograms.
        /// </summary>
        /// 
        /// <param name="x">The first histogram.</param>
        /// <param name="y">The second histogram.</param>
        /// 
        /// <returns>
        ///   The Bhattacharyya between the two histograms.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] x, double[] y)
        {
            double b = 0;
            for (int i = 0; i < x.Length; i++)
                b += System.Math.Sqrt(x[i] * y[i]);

            return System.Math.Sqrt(1.0 - b);
        }

        /// <summary>
        ///   Bhattacharyya distance between two histograms.
        /// </summary>
        /// 
        /// <param name="x">The first histogram.</param>
        /// <param name="y">The second histogram.</param>
        /// 
        /// <returns>
        ///   The Bhattacharyya between the two histograms.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(GeneralDiscreteDistribution x, GeneralDiscreteDistribution y)
        {
            double b = 0;
            for (int i = 0; i < x.Length; i++)
                b += System.Math.Sqrt(x.Frequencies[i] * y.Frequencies[i]);

            return System.Math.Sqrt(1.0 - b);
        }

        /// <summary>
        ///   Bhattacharyya distance between two histograms.
        /// </summary>
        /// 
        /// <param name="x">The first histogram.</param>
        /// <param name="y">The second histogram.</param>
        /// 
        /// <returns>
        ///   The Bhattacharyya between the two histograms.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(UnivariateDiscreteDistribution x, UnivariateDiscreteDistribution y)
        {
            double b = 0;
            foreach (int i in x.Support.Intersection(y.Support))
                b += System.Math.Sqrt(x.ProbabilityMassFunction(i) * y.ProbabilityMassFunction(i));

            return System.Math.Sqrt(1.0 - b);
        }

        /// <summary>
        ///   Bhattacharyya distance between two datasets, assuming
        ///   their contents can be modelled by multivariate Gaussians.
        /// </summary>
        /// 
        /// <param name="x">The first dataset.</param>
        /// <param name="y">The second dataset.</param>
        /// 
        /// <returns>
        ///   The Bhattacharyya between the two datasets.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[][] x, double[][] y)
        {
            double[] meanX = x.Mean(dimension: 0);
            double[] meanY = y.Mean(dimension: 0);
            double[][] covX = x.Covariance(meanX);
            double[][] covY = y.Covariance(meanY);

            return Distance(meanX, covX, meanY, covY);
        }

        /// <summary>
        ///   Bhattacharyya distance between two datasets, assuming
        ///   their contents can be modelled by multivariate Gaussians.
        /// </summary>
        /// 
        /// <param name="x">The first dataset.</param>
        /// <param name="y">The second dataset.</param>
        /// 
        /// <returns>
        ///   The Bhattacharyya between the two datasets.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[,] x, double[,] y)
        {
            double[] meanX = x.Mean(dimension: 0);
            double[] meanY = y.Mean(dimension: 0);
            double[,] covX = x.Covariance(meanX);
            double[,] covY = y.Covariance(meanY);

            return Distance(meanX, covX, meanY, covY);
        }

        /// <summary>
        ///   Bhattacharyya distance between two Gaussian distributions.
        /// </summary>
        /// 
        /// <param name="meanX">Mean for the first distribution.</param>
        /// <param name="covX">Covariance matrix for the first distribution.</param>
        /// <param name="meanY">Mean for the second distribution.</param>
        /// <param name="covY">Covariance matrix for the second distribution.</param>
        /// 
        /// <returns>The Bhattacharyya distance between the two distributions.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] meanX, double[,] covX, double[] meanY, double[,] covY)
        {
            int n = meanX.Length;

            double lnDetX = covX.LogPseudoDeterminant();
            double lnDetY = covY.LogPseudoDeterminant();

            return Distance(meanX, covX, lnDetX, meanY, covY, lnDetY);
        }

        /// <summary>
        ///   Bhattacharyya distance between two Gaussian distributions.
        /// </summary>
        /// 
        /// <param name="meanX">Mean for the first distribution.</param>
        /// <param name="covX">Covariance matrix for the first distribution.</param>
        /// <param name="meanY">Mean for the second distribution.</param>
        /// <param name="covY">Covariance matrix for the second distribution.</param>
        /// 
        /// <returns>The Bhattacharyya distance between the two distributions.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] meanX, double[][] covX, double[] meanY, double[][] covY)
        {
            int n = meanX.Length;

            double lnDetX = covX.LogPseudoDeterminant();
            double lnDetY = covY.LogPseudoDeterminant();

            return Distance(meanX, covX, lnDetX, meanY, covY, lnDetY);
        }

        /// <summary>
        ///   Bhattacharyya distance between two Gaussian distributions.
        /// </summary>
        /// 
        /// <param name="meanX">Mean for the first distribution.</param>
        /// <param name="covX">Covariance matrix for the first distribution.</param>
        /// <param name="meanY">Mean for the second distribution.</param>
        /// <param name="covY">Covariance matrix for the second distribution.</param>
        /// <param name="lnDetCovX">The logarithm of the determinant for 
        ///   the covariance matrix of the first distribution.</param>
        /// <param name="lnDetCovY">The logarithm of the determinant for 
        ///   the covariance matrix of the second distribution.</param>
        /// 
        /// <returns>The Bhattacharyya distance between the two distributions.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(
            double[] meanX, double[,] covX, double lnDetCovX,
            double[] meanY, double[,] covY, double lnDetCovY)
        {
            int n = meanX.Length;

            // P = (covX + covY) / 2
            var P = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    P[i, j] = (covX[i, j] + covY[i, j]) / 2.0;

            var svd = new SingularValueDecomposition(P);
            double detP = svd.LogPseudoDeterminant;

            double[] d = new double[meanX.Length];
            for (int i = 0; i < meanX.Length; i++)
                d[i] = meanX[i] - meanY[i];

            double[] z = svd.Solve(d);

            double r = 0.0;
            for (int i = 0; i < d.Length; i++)
                r += d[i] * z[i];

            double mahalanobis = Math.Abs(r);

            double a = (1.0 / 8.0) * mahalanobis;
            double b = (0.5) * (detP - 0.5 * (lnDetCovX + lnDetCovY));

            return a + b;
        }

        /// <summary>
        ///   Bhattacharyya distance between two Gaussian distributions.
        /// </summary>
        /// 
        /// <param name="meanX">Mean for the first distribution.</param>
        /// <param name="covX">Covariance matrix for the first distribution.</param>
        /// <param name="meanY">Mean for the second distribution.</param>
        /// <param name="covY">Covariance matrix for the second distribution.</param>
        /// <param name="lnDetCovX">The logarithm of the determinant for 
        ///   the covariance matrix of the first distribution.</param>
        /// <param name="lnDetCovY">The logarithm of the determinant for 
        ///   the covariance matrix of the second distribution.</param>
        /// 
        /// <returns>The Bhattacharyya distance between the two distributions.</returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(
            double[] meanX, double[][] covX, double lnDetCovX,
            double[] meanY, double[][] covY, double lnDetCovY)
        {
            int n = meanX.Length;

            // P = (covX + covY) / 2
            var P = Jagged.Zeros(n, n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    P[i][j] = (covX[i][j] + covY[i][j]) / 2.0;

            var svd = new JaggedSingularValueDecomposition(P);
            double detP = svd.LogPseudoDeterminant;

            double[] d = new double[meanX.Length];
            for (int i = 0; i < meanX.Length; i++)
                d[i] = meanX[i] - meanY[i];

            double[] z = svd.Solve(d);

            double r = 0.0;
            for (int i = 0; i < d.Length; i++)
                r += d[i] * z[i];

            double mahalanobis = Math.Abs(r);

            double a = (1.0 / 8.0) * mahalanobis;
            double b = (0.5) * (detP - 0.5 * (lnDetCovX + lnDetCovY));

            return a + b;
        }

        /// <summary>
        ///   Bhattacharyya distance between two Gaussian distributions.
        /// </summary>
        /// 
        /// <param name="x">The first Normal distribution.</param>
        /// <param name="y">The second Normal distribution.</param>
        /// 
        /// <returns>The Bhattacharyya distance between the two distributions.</returns>
        /// 
        public double Distance(MultivariateNormalDistribution x, MultivariateNormalDistribution y)
        {
            return Distance(x.Mean, x.Covariance, y.Mean, y.Covariance);
        }
    }
}
