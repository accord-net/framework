// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.Math
{
    using System;
    using System.Collections;
    using Accord.Math.Decompositions;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Static class Distance. Defines a set of extension methods defining distance measures.
    /// </summary>
    /// 
    public static class Distance
    {

        /// <summary>
        /// Gets the Bray Curtis distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Bray Curtis distance between x and y.</returns>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double BrayCurtis(this double[] x, double[] y)
        {
            double sumP, sumN;
            sumP = sumN = 0;

            for (int i = 0; i < x.GetLength(0); i++)
            {
                sumN += Math.Abs(x[i] - y[i]);
                sumP += Math.Abs(x[i] + y[i]);
            }

            return sumN / sumP;
        }

        /// <summary>
        /// Gets the Canberra distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Canberra distance between x and y.</returns>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Canberra(this double[] x, double[] y)
        {
            double distance = 0;

            for (int i = 0; i < x.GetLength(0); i++)
            {
                distance += Math.Abs(x[i] - y[i]) / (Math.Abs(x[i]) + Math.Abs(y[i]));
            }

            return distance;
        }

        /// <summary>
        /// Gets the Chessboard distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Chessboard distance between x and y.</returns>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Chessboard(this double[] x, double[] y)
        {
            double d = 0;
            for (int i = 0; i < x.GetLength(0); i++)
            {
                d = Math.Max(d, x[i] - y[i]);
            }

            return d;
        }

        /// <summary>
        /// Gets the Correlation distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Correlation distance between x and y.</returns>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Correlation(this double[] x, double[] y)
        {
            double p = 0;
            double q = 0;

            for (int i = 0; i < x.GetLength(0); i++)
            {
                p += -x[i];
                q += -y[i];
            }

            p /= x.GetLength(0);
            q /= y.GetLength(0);

            double num = 0;
            double den1 = 0;
            double den2 = 0;
            for (int i = 0; i < x.GetLength(0); i++)
            {
                num += (x[i] + p) * (y[i] + q);

                den1 += Math.Abs(Math.Pow(x[i] + p, 2));
                den2 += Math.Abs(Math.Pow(y[i] + p, 2));
            }

            return 1 - (num / (Math.Sqrt(den1) * Math.Sqrt(den2)));
        }

        /// <summary>
        /// Gets the Cosine distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Cosine distance between x and y.</returns>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Cosine(this double[] x, double[] y)
        {
            double sumProduct = 0;
            double sumP = 0, sumQ = 0;

            for (int i = 0; i < x.GetLength(0); i++)
            {
                sumProduct += x[i] * y[i];
                sumP += Math.Pow(Math.Abs(x[i]), 2);
                sumQ += Math.Pow(Math.Abs(y[i]), 2);
            }

            sumP = Math.Sqrt(sumP);
            sumQ = Math.Sqrt(sumQ);

            double result = 1 - (sumProduct / (sumP * sumQ));

            return result;
        }

        /// <summary>
        ///   Gets the Square Mahalanobis distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="precision">
        ///   The inverse of the covariance matrix of the distribution for the two points x and y.
        /// </param>
        /// 
        /// <returns>The Square Mahalanobis distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double SquareMahalanobis(this double[] x, double[] y, double[,] precision)
        {
            double[] d = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                d[i] = x[i] - y[i];

            double[] z = precision.Multiply(d);

            double r = 0.0;
            for (int i = 0; i < d.Length; i++)
                r += d[i] * z[i];
            return Math.Abs(r);
        }

        /// <summary>
        ///   Gets the Square Mahalanobis distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="covariance">
        ///   The <see cref="SingularValueDecomposition"/> of the covariance 
        ///   matrix of the distribution for the two points x and y.
        /// </param>
        /// 
        /// <returns>The Square Mahalanobis distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double SquareMahalanobis(this double[] x, double[] y, SingularValueDecomposition covariance)
        {
            double[] d = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                d[i] = x[i] - y[i];

            double[] z = covariance.Solve(d);

            double r = 0.0;
            for (int i = 0; i < d.Length; i++)
                r += d[i] * z[i];
            return Math.Abs(r);
        }


        /// <summary>
        ///   Gets the Mahalanobis distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="precision">
        ///   The inverse of the covariance matrix of the distribution for the two points x and y.
        /// </param>
        /// 
        /// <returns>The Mahalanobis distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Mahalanobis(this double[] x, double[] y, double[,] precision)
        {
            return System.Math.Sqrt(SquareMahalanobis(x, y, precision));
        }

        /// <summary>
        ///   Gets the Mahalanobis distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="covariance">
        ///   The <see cref="SingularValueDecomposition"/> of the covariance 
        ///   matrix of the distribution for the two points x and y.
        /// </param>
        /// 
        /// <returns>The Mahalanobis distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Mahalanobis(this double[] x, double[] y, SingularValueDecomposition covariance)
        {
            return System.Math.Sqrt(SquareMahalanobis(x, y, covariance));
        }

        /// <summary>
        ///   Gets the Manhattan distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// 
        /// <returns>The Manhattan distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Manhattan(this double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
                sum += System.Math.Abs(x[i] - y[i]);
            return sum;
        }

        /// <summary>
        ///   Gets the Manhattan distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// 
        /// <returns>The Manhattan distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Manhattan(this int[] x, int[] y)
        {
            int sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += System.Math.Abs(x[i] - y[i]);
            return sum;
        }

        /// <summary>
        ///   Gets the Minkowski distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="r">Factor.</param>
        /// 
        /// <returns>The Minkowski distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Minkowski(this double[] x, double[] y, int r)
        {
            double distance = 0;
            for (int i = 0; i < x.GetLength(0); i++)
            {
                distance += Math.Pow(Math.Abs(x[i] - y[i]), r);
            }
            return Math.Pow(distance, 1 / r);
        }

        /// <summary>
        ///   Gets the Chebyshev distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// 
        /// <returns>The Chebyshev distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Chebyshev(double[] x, double[] y)
        {
            double max = System.Math.Abs(x[0] - y[0]);

            for (int i = 1; i < x.Length; i++)
            {
                double abs = System.Math.Abs(x[i] - y[i]);

                if (abs > max) max = abs;
            }

            return max;
        }

        /// <summary>
        ///   Gets the Square Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// 
        /// <returns>The Square Euclidean distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double SquareEuclidean(this double[] x, double[] y)
        {
            double d = 0.0;

            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] - y[i];
                d += u * u;
            }

            return d;
        }

        /// <summary>
        ///   Gets the Square Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="x1">The first coordinate of first point in space.</param>
        /// <param name="y1">The second coordinate of first point in space.</param>
        /// <param name="x2">The first coordinate of second point in space.</param>
        /// <param name="y2">The second coordinate of second point in space.</param>
        /// 
        /// <returns>The Square Euclidean distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double SquareEuclidean(double x1, double x2, double y1, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return dx * dx + dy * dy; ;
        }

        /// <summary>
        ///   Gets the Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// 
        /// <returns>The Euclidean distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Euclidean(this double[] x, double[] y)
        {
            return System.Math.Sqrt(SquareEuclidean(x, y));
        }

        /// <summary>
        ///   Gets the Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="x1">The first coordinate of first point in space.</param>
        /// <param name="y1">The second coordinate of first point in space.</param>
        /// <param name="x2">The first coordinate of second point in space.</param>
        /// <param name="y2">The second coordinate of second point in space.</param>
        /// 
        /// <returns>The Euclidean distance between x and y.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Euclidean(double x1, double y1, double x2, double y2)
        {
            return System.Math.Sqrt(SquareEuclidean(x1, y1, x2, y2));
        }

        /// <summary>
        ///   Gets the Modulo-m distance between two integers <c>a</c> and <c>b</c>.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Modular(int a, int b, int modulo)
        {
            return System.Math.Min(Tools.Mod(a - b, modulo), Tools.Mod(b - a, modulo));
        }

        /// <summary>
        ///   Gets the Modulo-m distance between two real values <c>a</c> and <c>b</c>.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Modular(double a, double b, double modulo)
        {
            return System.Math.Min(Tools.Mod(a - b, modulo), Tools.Mod(b - a, modulo));
        }

        /// <summary>
        ///   Bhattacharyya distance between two normalized histograms.
        /// </summary>
        /// 
        /// <param name="histogram1">A normalized histogram.</param>
        /// <param name="histogram2">A normalized histogram.</param>
        /// <returns>The Bhattacharyya distance between the two histograms.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Bhattacharyya(double[] histogram1, double[] histogram2)
        {
            int bins = histogram1.Length; // histogram bins
            double b = 0; // Bhattacharyya's coefficient

            for (int i = 0; i < bins; i++)
                b += System.Math.Sqrt(histogram1[i]) * System.Math.Sqrt(histogram2[i]);

            // Bhattacharyya distance between the two distributions
            return System.Math.Sqrt(1.0 - b);
        }

        /// <summary>
        ///   Bhattacharyya distance between two matrices.
        /// </summary>
        /// 
        /// <param name="x">The first matrix <c>x</c>.</param>
        /// <param name="y">The first matrix <c>y</c>.</param>
        /// 
        /// <returns>The Bhattacharyya distance between the two matrices.</returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Bhattacharyya(double[,] x, double[,] y)
        {
            double[] meanX = mean(x);
            double[] meanY = mean(y);
            double[,] covX = cov(x, meanX);
            double[,] covY = cov(y, meanY);

            return Bhattacharyya(meanX, covX, meanY, covY);
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Bhattacharyya(double[] meanX, double[,] covX, double[] meanY, double[,] covY)
        {
            int n = meanX.Length;

            double lnDetX = covX.LogPseudoDeterminant();
            double lnDetY = covY.LogPseudoDeterminant();

            return Bhattacharyya(meanX, covX, lnDetX, meanY, covY, lnDetY);
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Bhattacharyya(
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

            double mahalanobis = SquareMahalanobis(meanY, meanX, svd);

            double a = (1.0 / 8.0) * mahalanobis;
            double b = (0.5) * (detP - 0.5 * (lnDetCovX + lnDetCovY));

            return a + b;
        }


        /// <summary>
        ///   Levenshtein distance between two strings.
        /// </summary>
        /// 
        /// <param name="x">The first string <c>x</c>.</param>
        /// <param name="y">The first string <c>y</c>.</param>
        /// 
        /// <remarks>
        ///   Based on the standard implementation available on Wikibooks:
        ///   http://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Levenshtein_distance
        /// </remarks>
        /// 
        /// <returns></returns>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Levenshtein(string x, string y)
        {
            if (x == null || x.Length == 0)
            {
                if (y == null || y.Length != 0)
                    return 0;
                return y.Length;
            }
            else
            {
                if (y == null || y.Length == 0)
                    return x.Length;
            }

            int[,] d = new int[x.Length + 1, y.Length + 1];

            for (int i = 0; i <= x.Length; i++)
                d[i, 0] = i;

            for (int i = 0; i <= y.Length; i++)
                d[0, i] = i;

            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < y.Length; j++)
                {
                    int cost = (x[i] == y[j]) ? 0 : 1;

                    int a = d[i, j + 1] + 1;
                    int b = d[i + 1, j] + 1;
                    int c = d[i, j] + cost;

                    d[i + 1, j + 1] = Math.Min(Math.Min(a, b), c);
                }
            }

            return d[x.Length, y.Length];
        }

        /// <summary>
        ///   Hamming distance between two Boolean vectors.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Hamming(bool[] x, bool[] y)
        {
            int d = 0;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i]) d++;
            return d;
        }

        /// <summary>
        ///   Hamming distance between two double vectors
        ///   containing only 0 (false) or 1 (true) values.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Hamming(double[] x, double[] y)
        {
            int d = 0;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i]) d++;
            return d;
        }

        /// <summary>
        ///   Bitwise hamming distance between two sequences of bytes.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double BitwiseHamming(byte[] x, byte[] y)
        {
            int d = 0;
            for (int i = 0; i < x.Length; i++)
            {
                byte xor = (byte)(x[i] ^ y[i]);
                d += lookup[xor];
            }
            return d;
        }

        private readonly static byte[] lookup =
        {
            0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8,
        };

        /// <summary>
        ///   Bitwise hamming distance between two bit arrays.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double BitwiseHamming(BitArray x, BitArray y)
        {
            int d = 0;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i]) d++;
            return d;
        }


        #region Private methods
        private static double[] mean(double[,] matrix)
        {
            double[] mean = new double[matrix.GetLength(1)];
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double N = matrix.GetLength(0);

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                    mean[j] += matrix[i, j];
                mean[j] /= N;
            }

            return mean;
        }

        private static double[,] cov(double[,] matrix, double[] means)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double divisor = rows - 1;

            double[,] cov = new double[cols, cols];
            for (int i = 0; i < cols; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    double s = 0.0;
                    for (int k = 0; k < rows; k++)
                        s += (matrix[k, j] - means[j]) * (matrix[k, i] - means[i]);
                    s /= divisor;
                    cov[i, j] = s;
                    cov[j, i] = s;
                }
            }

            return cov;
        }
        #endregion

    }
}
