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

namespace Accord.Statistics
{
    using Accord.Diagnostics;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static partial class Tools
    {

        /// <summary>
        ///   Creates Tukey's box plot inner fence.
        /// </summary>
        /// 
        public static DoubleRange InnerFence(this DoubleRange quartiles)
        {
            return new DoubleRange(
                quartiles.Min - 1.5 * quartiles.Length,
                quartiles.Max + 1.5 * quartiles.Length);
        }

        /// <summary>
        ///   Creates Tukey's box plot outer fence.
        /// </summary>
        /// 
        public static DoubleRange OuterFence(this DoubleRange quartiles)
        {
            return new DoubleRange(
                quartiles.Min - 3 * quartiles.Length,
                quartiles.Max + 3 * quartiles.Length);
        }


        /// <summary>
        ///   Gets the rank of a sample, often used with order statistics.
        /// </summary>
        /// 
        public static double[] Rank(this double[] samples, bool alreadySorted = false, bool adjustForTies = true)
        {
            bool hasTies;
            return Rank(samples, hasTies: out hasTies, alreadySorted: alreadySorted, adjustForTies: adjustForTies);
        }

        /// <summary>
        ///   Gets the rank of a sample, often used with order statistics.
        /// </summary>
        /// 
        public static double[] Rank(this double[] samples, out bool hasTies,
            bool alreadySorted = false, bool adjustForTies = true)
        {
            int[] idx = Vector.Range(0, samples.Length);

            if (!alreadySorted)
            {
                samples = (double[])samples.Clone();
                Array.Sort(samples, idx);
            }

            double[] ranks = new double[samples.Length];

            double tieSum = 0;
            int tieSize = 0;
            hasTies = false;

            if (samples.Length == 0)
                return new double[0];

            ranks[0] = 1;

            if (adjustForTies)
            {
                int r = 1;
                for (int i = 1; i < ranks.Length; i++)
                {
                    // Check if we have a tie
                    if (samples[i] != samples[i - 1])
                    {
                        // This is not a tie.
                        // Was a tie before?
                        if (tieSize > 0)
                        {
                            // Yes. Then set the previous
                            // elements with the average.

                            for (int j = 0; j < tieSize + 1; j++)
                            {
                                int k = i - j - 1;
                                ranks[k] = (r + tieSum) / (tieSize + 1.0);
                            }

                            tieSize = 0;
                            tieSum = 0;
                        }

                        ranks[i] = ++r;
                    }
                    else
                    {
                        // This is a tie. Compute how 
                        // long we have been in a tie.
                        tieSize++;
                        tieSum += r++;
                        hasTies = true;
                    }
                }

                // Handle the last ties
                if (tieSize > 0)
                {
                    // We were still in the middle of a tie
                    for (int j = 0; j < tieSize + 1; j++)
                    {
                        int k = samples.Length - j - 1;
                        ranks[k] = (r + tieSum) / (tieSize + 1.0);
                    }
                }
            }
            else
            {
                // No need to adjust for ties
                for (int i = 1, r = 1; i < ranks.Length; i++)
                {
                    if (samples[i] == samples[i - 1])
                        hasTies = true;

                    ranks[i] = ++r;
                }
            }

            if (!alreadySorted)
                Array.Sort(idx, ranks);

            return ranks;
        }

        /// <summary>
        ///   Gets the number of ties and distinct elements in a rank vector.
        /// </summary>
        /// 
        public static int[] Ties(this double[] ranks)
        {
            SortedDictionary<double, int> ties;
            return Ties(ranks, out ties);
        }

        /// <summary>
        ///   Gets the number of ties and distinct elements in a rank vector.
        /// </summary>
        /// 
        public static int[] Ties(this double[] ranks, out SortedDictionary<double, int> counts)
        {
            counts = new SortedDictionary<double, int>();
            for (int i = 0; i < ranks.Length; i++)
            {
                double r = ranks[i];

                int c;
                if (!counts.TryGetValue(r, out c))
                    c = 0;

                counts[r] = c + 1;
            }

            int[] ties = new int[counts.Count];
            double[] sorted = counts.Keys.Sorted();
            for (int i = 0; i < sorted.Length; i++)
                ties[i] = counts[sorted[i]];
            return ties;
        }




        /// <summary>
        ///   Generates a random <see cref="Measures.Covariance(double[], double[], bool)"/> matrix.
        /// </summary>
        /// 
        /// <param name="size">The size of the square matrix.</param>
        /// <param name="minValue">The minimum value for a diagonal element.</param>
        /// <param name="maxValue">The maximum size for a diagonal element.</param>
        /// 
        /// <returns>A square, positive-definite matrix which 
        ///   can be interpreted as a covariance matrix.</returns>
        /// 
        public static double[,] RandomCovariance(int size, double minValue, double maxValue)
        {
            double[,] A = Accord.Math.Matrix.Random(size, minValue, maxValue, symmetric: true);

            var gso = new GramSchmidtOrthogonalization(A);
            double[,] Q = gso.OrthogonalFactor;

            double[] diagonal = Vector.Random(size, minValue, maxValue).Abs();
            double[,] psd = Matrix.Dot(Q.TransposeAndDotWithDiagonal(diagonal), Q);

            Accord.Diagnostics.Debug.Assert(psd.IsPositiveDefinite());

            return psd;
        }


        /// <summary>
        ///   Computes the kernel distance for a kernel function even if it doesn't
        ///   implement the <see cref="Accord.Math.Distances.IDistance"/> interface. Can be used to check
        ///   the proper implementation of the distance function.
        /// </summary>
        /// 
        /// <param name="kernel">The kernel function whose distance needs to be evaluated.</param>
        /// <param name="x">An input point <c>x</c> given in input space.</param>
        /// <param name="y">An input point <c>y</c> given in input space.</param>
        /// 
        /// <returns>
        ///   The distance between <paramref name="x"/> and <paramref name="y"/> in kernel (feature) space.
        /// </returns>
        /// 
        public static double Distance(this IKernel kernel, double[] x, double[] y)
        {
            return kernel.Function(x, x) + kernel.Function(y, y) - 2 * kernel.Function(x, y);
        }




        /// <summary> 
        ///   Generates the Standard Scores, also known as Z-Scores, from the given data. 
        /// </summary> 
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param> 
        /// 
        /// <returns>The Z-Scores for the matrix.</returns> 
        /// 
        public static double[,] ZScores(this double[,] matrix)
        {
            double[] mean = Measures.Mean(matrix, dimension: 0);
            return ZScores(matrix, mean, Measures.StandardDeviation(matrix, mean));
        }

        /// <summary> 
        ///   Generates the Standard Scores, also known as Z-Scores, from the given data. 
        /// </summary> 
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param> 
        /// <param name="means">The mean value of the given values, if already known.</param> 
        /// <param name="standardDeviations">The values' standard deviation vector, if already known.</param> 
        /// 
        /// <returns>The Z-Scores for the matrix.</returns> 
        /// 
        public static double[,] ZScores(this double[,] matrix, double[] means, double[] standardDeviations)
        {
            return Center(matrix, means, inPlace: false).Standardize(standardDeviations, inPlace: true);
        }

        /// <summary> 
        ///   Generates the Standard Scores, also known as Z-Scores, from the given data. 
        /// </summary> 
        /// 
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param> 
        /// 
        /// <returns>The Z-Scores for the matrix.</returns> 
        /// 
        public static double[][] ZScores(this double[][] matrix)
        {
            double[] mean = Measures.Mean(matrix, dimension: 0);
            return ZScores(matrix, mean, Measures.StandardDeviation(matrix, mean));
        }

        /// <summary> 
        ///   Generates the Standard Scores, also known as Z-Scores, from the given data. 
        /// </summary> 
        ///  
        /// <param name="matrix">A number multi-dimensional array containing the matrix values.</param> 
        /// <param name="means">The mean value of the given values, if already known.</param> 
        /// <param name="standardDeviations">The values' standard deviation vector, if already known.</param> 
        ///  
        /// <returns>The Z-Scores for the matrix.</returns> 
        ///  
        public static double[][] ZScores(this double[][] matrix, double[] means, double[] standardDeviations)
        {
            return Center(matrix, means, inPlace: false).Standardize(standardDeviations, inPlace: true);
        }

        /// <summary>
        ///   Centers an observation, subtracting the empirical 
        ///   mean from each element in the observation vector.
        /// </summary>
        /// 
        /// <param name="observation">An array of double precision floating-point numbers.</param>
        /// <param name="result">The destination array where the result of this operation should be stored.</param>
        /// 
        public static double[] Center(this double[] observation, double[] result = null)
        {
            return Center(observation, observation.Mean(), result);
        }

        /// <summary>
        ///   Centers an observation, subtracting the empirical 
        ///   mean from each element in the observation vector.
        /// </summary>
        /// 
        /// <param name="values">An array of double precision floating-point numbers.</param>
        /// <param name="mean">The mean of the <paramref name="values"/>, if already known.</param>
        /// <param name="result">The destination array where the result of this operation should be stored.</param>
        /// 
        public static double[] Center(this double[] values, double mean, double[] result = null)
        {
            if (result == null)
                result = new double[values.Length];

            for (int i = 0; i < values.Length; i++)
                result[i] = values[i] - mean;

            return result;
        }


        /// <summary>
        ///   Centers column data, subtracting the empirical mean from each variable.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[,] Center(this double[,] matrix, bool inPlace = false)
        {
            return Center(matrix, Measures.Mean(matrix, dimension: 0), inPlace);
        }

        /// <summary>
        ///   Centers column data, subtracting the empirical mean from each variable.
        /// </summary>   
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[,] Center(this double[,] matrix, double[] means, bool inPlace = false)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = inPlace ? matrix : new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = matrix[i, j] - means[j];

            return result;
        }

        /// <summary>
        ///   Centers column data, subtracting the empirical mean from each variable.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[][] Center(this double[][] matrix, bool inPlace = false)
        {
            return Center(matrix, Measures.Mean(matrix, dimension: 0), inPlace);
        }

        /// <summary>Centers column data, subtracting the empirical mean from each variable.</summary>
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="means">The mean value of the given values, if already known.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[][] Center(this double[][] matrix, double[] means, bool inPlace = false)
        {
            double[][] result = matrix;

            if (!inPlace)
            {
                result = new double[matrix.Length][];
                for (int i = 0; i < matrix.Length; i++)
                    result[i] = new double[matrix[i].Length];
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                double[] row = result[i];
                for (int j = 0; j < row.Length; j++)
                    row[j] = matrix[i][j] - means[j];
            }

            return result;
        }


        /// <summary>
        ///   Standardizes column data, removing the empirical standard deviation from each variable.
        /// </summary>
        /// 
        /// <remarks>This method does not remove the empirical mean prior to execution.</remarks>
        /// 
        /// <param name="values">An array of double precision floating-point numbers.</param>
        /// <param name="inPlace">True to perform the operation in place, 
        ///   altering the original input matrix.</param>
        /// 
        public static double[] Standardize(this double[] values, bool inPlace = false)
        {
            return Standardize(values, Measures.StandardDeviation(values), inPlace);
        }

        /// <summary>
        ///   Standardizes column data, removing the empirical standard deviation from each variable.
        /// </summary>
        /// 
        /// <remarks>This method does not remove the empirical mean prior to execution.</remarks>
        /// 
        /// <param name="values">An array of double precision floating-point numbers.</param>
        /// <param name="standardDeviation">The standard deviation of the given 
        /// <paramref name="values"/>, if already known.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[] Standardize(this double[] values, double standardDeviation, bool inPlace = false)
        {

            double[] result = inPlace ? values : new double[values.Length];
            for (int i = 0; i < values.Length; i++)
                result[i] = values[i] / standardDeviation;

            return result;
        }

        /// <summary>
        ///   Standardizes column data, removing the empirical standard deviation from each variable.
        /// </summary>
        /// 
        /// <remarks>This method does not remove the empirical mean prior to execution.</remarks>
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[,] Standardize(this double[,] matrix, bool inPlace = false)
        {
            return Standardize(matrix, Measures.StandardDeviation(matrix), inPlace);
        }

        /// <summary>
        ///   Standardizes column data, removing the empirical standard deviation from each variable.
        /// </summary>
        /// 
        /// <remarks>This method does not remove the empirical mean prior to execution.</remarks>
        ///
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="standardDeviations">The values' standard deviation vector, if already known.</param>
        /// <param name="tol">The minimum value that will be taken as the standard deviation in case the deviation is too close to zero.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[,] Standardize(this double[,] matrix, double[] standardDeviations, bool inPlace = false, double tol = 1e-12)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = inPlace ? matrix : new double[rows, cols];

            if (tol == 0)
            {
                for (int i = 0; i < standardDeviations.Length; i++)
                    if (standardDeviations[i] == 0 && tol == 0)
                        throw new ArithmeticException("Standard deviation cannot be" +
                        " zero (cannot standardize the constant variable at column index " + i + ").");
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double stdDev = standardDeviations[j] > 0 ? standardDeviations[j] : tol;
                    result[i, j] = matrix[i, j] / stdDev;
                }
            }

            return result;
        }


        /// <summary>
        ///   Standardizes column data, removing the empirical standard deviation from each variable.
        /// </summary>
        /// 
        /// <remarks>This method does not remove the empirical mean prior to execution.</remarks>
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[][] Standardize(this double[][] matrix, bool inPlace = false)
        {
            return Standardize(matrix, Measures.StandardDeviation(matrix), inPlace);
        }

        /// <summary>
        ///   Standardizes column data, removing the empirical standard deviation from each variable.
        /// </summary>
        ///
        /// <remarks>This method does not remove the empirical mean prior to execution.</remarks>
        /// 
        /// <param name="matrix">A matrix where each column represent a variable and each row represent a observation.</param>
        /// <param name="tol">The minimum value that will be taken as the standard deviation in case the deviation is too close to zero.</param>
        /// <param name="standardDeviations">The values' standard deviation vector, if already known.</param>
        /// <param name="inPlace">True to perform the operation in place, altering the original input matrix.</param>
        /// 
        public static double[][] Standardize(this double[][] matrix, double[] standardDeviations, bool inPlace = false, double tol = 1e-12)
        {
            double[][] result = inPlace ? matrix : Jagged.CreateAs(matrix);

            if (tol == 0)
            {
                for (int i = 0; i < standardDeviations.Length; i++)
                    if (standardDeviations[i] == 0)
                        throw new ArithmeticException("Standard deviation cannot be" +
                        " zero (cannot standardize the constant variable at column index " + i + ").");
            }


            for (int i = 0; i < matrix.Length; i++)
            {
                double[] resultRow = result[i];
                double[] sourceRow = matrix[i];
                for (int j = 0; j < resultRow.Length; j++)
                {
                    double stdDev = standardDeviations[j] > 0 ? standardDeviations[j] : tol;
                    resultRow[j] = sourceRow[j] / stdDev;
                }
            }

            return result;
        }


    }
}

