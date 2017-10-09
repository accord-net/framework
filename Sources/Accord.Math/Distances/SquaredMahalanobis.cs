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
    using System;
    using System.Runtime.CompilerServices;
    using Accord.Compat;

    /// <summary>
    ///   Squared Mahalanobis distance.
    /// </summary>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
    /// </example>
    /// 
    [Serializable]
    public struct SquareMahalanobis : IMetric<double[]>
    {
        CholeskyDecomposition chol;
        SingularValueDecomposition svd;
        double[,] precision;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="chol">A Cholesky decomposition of the covariance matrix.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
        /// </example>
        /// 
        public SquareMahalanobis(CholeskyDecomposition chol)
        {
            this.chol = chol;
            this.svd = null;
            this.precision = null;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="svd">A Singular Value decomposition of the covariance matrix.</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
        /// </example>
        /// 
        public SquareMahalanobis(SingularValueDecomposition svd)
        {
            this.chol = null;
            this.svd = svd;
            this.precision = null;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Mahalanobis"/> class.
        /// </summary>
        /// 
        /// <param name="precision">The precision matrix (the inverse of the covariance matrix).</param>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
        /// </example>
        /// 
        public SquareMahalanobis(double[,] precision)
        {
            this.chol = null;
            this.svd = null;
            this.precision = precision;
        }

        /// <summary>
        ///   Creates a new Square-Mahalanobis distance from a covariance matrix.
        /// </summary>
        /// 
        /// <param name="covariance">A covariance matrix.</param>
        /// 
        /// <returns>
        ///   A square Mahalanobis distance using the <see cref="SingularValueDecomposition"/>
        ///   of the given covariance matrix.
        /// </returns>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
        /// </example>
        /// 
        public static SquareMahalanobis FromCovarianceMatrix(double[,] covariance)
        {
            return new SquareMahalanobis(new CholeskyDecomposition(covariance));
        }

        /// <summary>
        ///   Creates a new Square-Mahalanobis distance from a precision matrix.
        /// </summary>
        /// 
        /// <param name="precision">A precision matrix.</param>
        /// 
        /// <returns>
        ///   A square Mahalanobis distance using the given precision matrix.
        /// </returns>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
        /// </example>
        /// 
        public static SquareMahalanobis FromPrecisionMatrix(double[,] precision)
        {
            return new SquareMahalanobis(precision);
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Math\DistanceTest.cs" region="doc_square_mahalanobis_3" />
        /// </example>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Distance(double[] x, double[] y)
        {
            double[] d = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                d[i] = x[i] - y[i];

            double[] z;
            if (svd != null)
                z = svd.Solve(d);
            else if (chol != null)
                z = chol.Solve(d);
            else
                z = precision.Dot(d);

            double sum = 0.0;
            for (int i = 0; i < d.Length; i++)
                sum += d[i] * z[i];
            return Math.Abs(sum);
        }

    }
}
