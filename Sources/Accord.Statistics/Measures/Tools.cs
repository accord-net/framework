// Accord Statistics Library
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

namespace Accord.Statistics
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Kernels;
    using AForge;

    /// <summary>
    ///   Sample weight types.
    /// </summary>
    /// 
    public enum WeightType
    {
        /// <summary>
        ///   Weights should be ignored.
        /// </summary>
        /// 
        None,

        /// <summary>
        ///   Weights are integers representing how many times a sample should repeat itself.
        /// </summary>
        /// 
        Repetition,

        /// <summary>
        ///   Weights are fractional numbers that sum up to one.
        /// </summary>
        /// 
        Fraction,

        /// <summary>
        ///   If weights sum up to one, they are handled as <see cref="Fraction">fractional
        ///   weights</see>. If they sum to a whole number, they are handled as <see cref="Repetition">
        ///   integer repetition counts</see>.
        /// </summary>
        /// 
        Automatic,
    }

    /// <summary>
    ///   Set of statistics functions.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents collection of common functions used in statistics.
    ///   Every Matrix function assumes data is organized in a table-like model,
    ///   where Columns represents variables and Rows represents a observation of
    ///   each variable.
    /// </remarks>
    /// 
    public static partial class Tools
    {

        /// <summary>
        ///   Creates Tukey's box plot inner fence.
        /// </summary>
        /// 
        public static DoubleRange InnerFence(DoubleRange quartiles)
        {
            return new DoubleRange(
                quartiles.Min - 1.5 * quartiles.Length,
                quartiles.Max + 1.5 * quartiles.Length);
        }

        /// <summary>
        ///   Creates Tukey's box plot outer fence.
        /// </summary>
        /// 
        public static DoubleRange OuterFence(DoubleRange quartiles)
        {
            return new DoubleRange(
                quartiles.Min - 3 * quartiles.Length,
                quartiles.Max + 3 * quartiles.Length);
        }



        #region Summarizing, grouping and extending operations
        /// <summary>
        ///   Calculates the prevalence of a class for each variable.
        /// </summary>
        /// 
        /// <param name="positives">An array of counts detailing the occurrence of the first class.</param>
        /// <param name="negatives">An array of counts detailing the occurrence of the second class.</param>
        /// 
        /// <returns>An array containing the proportion of the first class over the total of occurrences.</returns>
        /// 
        public static double[] Proportions(int[] positives, int[] negatives)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Proportions(positives, negatives);
        }

        /// <summary>
        ///   Calculates the prevalence of a class.
        /// </summary>
        /// 
        /// <param name="data">A matrix containing counted, grouped data.</param>
        /// <param name="positiveColumn">The index for the column which contains counts for occurrence of the first class.</param>
        /// <param name="negativeColumn">The index for the column which contains counts for occurrence of the second class.</param>
        /// 
        /// <returns>An array containing the proportion of the first class over the total of occurrences.</returns>
        /// 
        public static double[] Proportions(int[][] data, int positiveColumn, int negativeColumn)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Proportions(data, positiveColumn, negativeColumn);
        }

        /// <summary>
        ///   Groups the occurrences contained in data matrix of binary (dichotomous) data.
        /// </summary>
        /// 
        /// <param name="data">A data matrix containing at least a column of binary data.</param>
        /// <param name="labelColumn">Index of the column which contains the group label name.</param>
        /// <param name="dataColumn">Index of the column which contains the binary [0,1] data.</param>
        /// 
        /// <returns>
        ///    A matrix containing the group label in the first column, the number of occurrences of the first class
        ///    in the second column and the number of occurrences of the second class in the third column.
        /// </returns>
        /// 
        public static int[][] Group(int[][] data, int labelColumn, int dataColumn)
        {
            return Accord.Statistics.Groups.Group(data, labelColumn, dataColumn);
        }

        /// <summary>
        ///   Extends a grouped data into a full observation matrix.
        /// </summary>
        /// 
        /// <param name="data">The group labels.</param>
        /// <param name="positives">
        ///   An array containing he occurrence of the positive class
        ///   for each of the groups.</param>
        /// <param name="negatives">
        ///   An array containing he occurrence of the negative class
        ///   for each of the groups.</param>
        ///   
        /// <returns>A full sized observation matrix.</returns>
        /// 
        public static int[][] Expand(int[] data, int[] positives, int[] negatives)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Expand(data, positives, negatives);
        }

        /// <summary>
        ///   Expands a grouped data into a full observation matrix.
        /// </summary>
        /// 
        /// <param name="data">The grouped data matrix.</param>
        /// <param name="labelColumn">Index of the column which contains the labels
        /// in the grouped data matrix. </param>
        /// <param name="positiveColumn">Index of the column which contains
        ///   the occurrences for the first class.</param>
        /// <param name="negativeColumn">Index of the column which contains
        ///   the occurrences for the second class.</param>
        ///   
        /// <returns>A full sized observation matrix.</returns>
        /// 
        public static int[][] Expand(int[][] data, int labelColumn, int positiveColumn, int negativeColumn)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Expand(data, labelColumn, positiveColumn, negativeColumn);
        }

        /// <summary>
        ///   Expands a grouped data into a full observation matrix.
        /// </summary>
        /// 
        /// <param name="labels">The class labels.</param>
        /// 
        /// <returns>A jagged matrix where each row corresponds to each element 
        ///   given in the <paramref name="labels"/> parameter, and each row has
        ///   the same length as the number of <paramref name="labels"/> in the
        ///   problem. Each row contains the value 1 on the position corresponding
        ///   to the label index.</returns>
        /// 
        public static double[][] Expand(int[] labels)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Expand(labels);
        }

        /// <summary>
        ///   Expands a grouped data into a full observation matrix.
        /// </summary>
        /// 
        /// <param name="labels">The class labels.</param>
        /// <param name="negative">The negative value to indicate the absence of the class.</param>
        /// <param name="positive">The positive value to indicate the presence of the class.</param>
        /// 
        /// <returns>A jagged matrix where each row corresponds to each element 
        ///   given in the <paramref name="labels"/> parameter, and each row has
        ///   the same length as the number of <paramref name="labels"/> in the
        ///   problem. Each row contains the positive value on the position corresponding
        ///   to the label index, and the negative value on all others.</returns>
        /// 
        public static double[][] Expand(int[] labels, double negative, double positive)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Expand(labels, negative, positive);
        }

        /// <summary>
        ///   Expands a grouped data into a full observation matrix.
        /// </summary>
        /// 
        /// <param name="labels">The class labels.</param>
        /// <param name="classes">The number of classes.</param>
        /// 
        /// <returns>A jagged matrix where each row corresponds to each element 
        ///   given in the <paramref name="labels"/> parameter, and each row has
        ///   the same length as the number of <paramref name="classes"/> in the
        ///   problem. Each row contains the positive value on the position corresponding
        ///   to the label index, and the negative value on all others.</returns>
        /// 
        public static double[][] Expand(int[] labels, int classes)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Expand(labels, classes);
        }

        /// <summary>
        ///   Expands a grouped data into a full observation matrix.
        /// </summary>
        /// 
        /// <param name="labels">The class labels.</param>
        /// <param name="classes">The number of classes.</param>
        /// <param name="negative">The negative value to indicate the absence of the class.</param>
        /// <param name="positive">The positive value to indicate the presence of the class.</param>
        /// 
        /// <returns>A jagged matrix where each row corresponds to each element 
        ///   given in the <paramref name="labels"/> parameter, and each row has
        ///   the same length as the number of <paramref name="classes"/> in the
        ///   problem. Each row contains the value 1 on the position corresponding
        ///   to the label index.</returns>
        /// 
        public static double[][] Expand(int[] labels, int classes, double negative, double positive)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Expand(labels, classes, negative, positive);
        }
        #endregion


        #region Determination and performance measures
        /// <summary>
        ///   Gets the coefficient of determination, as known as the R-Squared (R²)
        /// </summary>
        /// 
        /// <remarks>
        ///    The coefficient of determination is used in the context of statistical models
        ///    whose main purpose is the prediction of future outcomes on the basis of other
        ///    related information. It is the proportion of variability in a data set that
        ///    is accounted for by the statistical model. It provides a measure of how well
        ///    future outcomes are likely to be predicted by the model.
        ///    
        ///    The R^2 coefficient of determination is a statistical measure of how well the
        ///    regression approximates the real data points. An R^2 of 1.0 indicates that the
        ///    regression perfectly fits the data.
        /// </remarks>
        /// 
        public static double Determination(double[] actual, double[] expected)
        {
            // R-squared = 100 * SS(regression) / SS(total)

            double SSe = 0.0;
            double SSt = 0.0;
            double avg = 0.0;
            double d;

            // Calculate expected output mean
            for (int i = 0; i < expected.Length; i++)
                avg += expected[i];
            avg /= expected.Length;

            // Calculate SSe and SSt
            for (int i = 0; i < expected.Length; i++)
            {
                d = expected[i] - actual[i];
                SSe += d * d;

                d = expected[i] - avg;
                SSt += d * d;
            }

            // Calculate R-Squared
            return 1.0 - (SSe / SSt);
        }
        #endregion


        #region Permutations and combinatorials

        /// <summary>
        ///   Returns a random sample of size k from a population of size n.
        /// </summary>
        /// 
        public static int[] RandomSample(int n, int k)
        {
            // TODO: Mark as obsolete
            return Accord.Math.Indices.Random(n, k);
        }

        /// <summary>
        ///   Returns a random group assignment for a sample.
        /// </summary>
        /// 
        /// <param name="size">The sample size.</param>
        /// <param name="groups">The number of groups.</param>
        /// 
        public static int[] RandomGroups(int size, int groups)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Random(size, groups);
        }

        /// <summary>
        ///   Returns a random group assignment for a sample
        ///   into two mutually exclusive groups.
        /// </summary>
        /// 
        /// <param name="size">The sample size.</param>
        /// <param name="proportion">The proportion of samples between the groups.</param>
        /// 
        public static int[] RandomGroups(int size, double proportion)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Random(size, proportion);
        }

        /// <summary>
        ///   Returns a random group assignment for a sample, making
        ///   sure different class labels are distributed evenly among
        ///   the groups.
        /// </summary>
        /// 
        /// <param name="labels">A vector containing class labels.</param>
        /// <param name="classes">The number of different classes in <paramref name="labels"/>.</param>
        /// <param name="groups">The number of groups.</param>
        /// 
        public static int[] RandomGroups(int[] labels, int classes, int groups)
        {
            // TODO: Mark as obsolete
            return Accord.Statistics.Groups.Random(labels, classes, groups);
        }

        /// <summary>
        ///   Returns a random permutation of size n.
        /// </summary>
        /// 
        public static int[] Random(int n)
        {
            // TODO: Mark as obsolete
            return Indices.Random(n);
        }

        /// <summary>
        ///   Shuffles an array.
        /// </summary>
        /// 
        public static void Shuffle<T>(T[] array)
        {
            // TODO: Mark as obsolete
            Vector.Shuffle(array);
        }

        /// <summary>
        ///   Shuffles a collection.
        /// </summary>
        /// 
        public static void Shuffle<T>(IList<T> array)
        {
            // TODO: Mark as obsolete
            Vector.Shuffle(array);
        }


        #endregion


        // ------------------------------------------------------------

        /// <summary>
        ///   Computes the whitening transform for the given data, making
        ///   its covariance matrix equals the identity matrix.
        /// </summary>
        /// <param name="value">A matrix where each column represent a
        ///   variable and each row represent a observation.</param>
        /// <param name="transformMatrix">The base matrix used in the
        ///   transformation.</param>
        /// <returns>
        ///   The transformed source data (which now has unit variance).
        /// </returns>
        /// 
        public static double[,] Whitening(double[,] value, out double[,] transformMatrix)
        {
            if (value == null)
                throw new ArgumentNullException("value");


            int cols = value.GetLength(1);

            double[,] cov = Tools.Covariance(value);

            // Diagonalizes the covariance matrix
            SingularValueDecomposition svd = new SingularValueDecomposition(cov,
                true,  // compute left vectors (to become a transformation matrix)
                false, // do not compute right vectors since they aren't necessary
                true,  // transpose if necessary to avoid erroneous assumptions in SVD
                true); // perform operation in-place, reducing memory usage


            // Retrieve the transformation matrix
            transformMatrix = svd.LeftSingularVectors;

            // Perform scaling to have unit variance
            double[] singularValues = svd.Diagonal;
            for (int i = 0; i < cols; i++)
                for (int j = 0; j < singularValues.Length; j++)
                    transformMatrix[i, j] /= Math.Sqrt(singularValues[j]);

            // Return the transformed data
            return value.Multiply(transformMatrix);
        }


        /// <summary>
        ///   Gets the rank of a sample, often used with order statistics.
        /// </summary>
        /// 
        public static double[] Rank(double[] samples, bool alreadySorted = false)
        {
            int[] idx = Matrix.Indices(0, samples.Length);

            if (!alreadySorted)
            {
                samples = (double[])samples.Clone();
                Array.Sort(samples, idx);
            }

            double[] ranks = new double[samples.Length];

            double tieSum = 0;
            int tieSize = 0;

            int start = 0;
            while (samples[start] == 0) start++;

            ranks[start] = 1;
            for (int i = start + 1, r = 1; i < ranks.Length; i++)
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
                            ranks[i - j - 1] = (r + tieSum) / (tieSize + 1);

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
                }
            }

            if (!alreadySorted)
                Array.Sort(idx, ranks);

            return ranks;
        }




        /// <summary>
        ///   Gets the number of distinct values 
        ///   present in each column of a matrix.
        /// </summary>
        /// 
        public static int[] DistinctCount(double[,] sourceMatrix)
        {
            double[][] distinct = sourceMatrix.Distinct();

            int[] counts = new int[distinct.Length];
            for (int i = 0; i < counts.Length; i++)
                counts[i] = distinct[i].Length;

            return counts;
        }

        /// <summary>
        ///   Gets the number of distinct values 
        ///   present in each column of a matrix.
        /// </summary>
        /// 
        public static int[] DistinctCount(double[][] sourceMatrix)
        {
            double[][] distinct = sourceMatrix.Distinct();

            int[] counts = new int[distinct.Length];
            for (int i = 0; i < counts.Length; i++)
                counts[i] = distinct[i].Length;

            return counts;
        }

        /// <summary>
        ///   Gets the number of distinct values 
        ///   present in each column of a matrix.
        /// </summary>
        /// 
        public static int DistinctCount(double[] source)
        {
            return source.Distinct().Length;
        }


        /// <summary>
        ///   Generates a random <see cref="Covariance(double[], double[], bool)"/> matrix.
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
            double[,] A = Accord.Math.Matrix.Random(size, true, minValue, maxValue);

            var gso = new GramSchmidtOrthogonalization(A);
            double[,] Q = gso.OrthogonalFactor;

            double[] diagonal = Matrix.Random(size, minValue, maxValue).Abs();
            double[,] psd = Q.TransposeAndMultiplyByDiagonal(diagonal).Multiply(Q);

            System.Diagnostics.Debug.Assert(psd.IsPositiveDefinite());

            return psd;
        }


        /// <summary>
        ///   Computes the kernel distance for a kernel function even if it doesn't
        ///   implement the <see cref="IDistance"/> interface. Can be used to check
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
        public static double Distance(IKernel kernel, double[] x, double[] y)
        {
            return kernel.Function(x, x) + kernel.Function(y, y) - 2 * kernel.Function(x, y);
        }



        private static double correct(bool unbiased, WeightType weightType, double sum, double weightSum, double squareSum)
        {
            if (unbiased)
            {
                if (weightType == WeightType.Automatic)
                {
                    if (weightSum > 1 && weightSum.IsInteger(1e-8))
                        return sum / (weightSum - 1);

                    return sum / (weightSum - (squareSum / weightSum));
                }
                else if (weightType == WeightType.Fraction)
                {
                    /*
                    if (Math.Abs(weightSum - 1.0) >= 1e-8)
                    {
                        throw new ArgumentException("An unbiased variance estimate"
                          + " cannot be computed if weights do not sum to one. The"
                          + " given weights sum up to " + squareSum, "weights");
                    }*/

                    return sum / (weightSum - (squareSum / weightSum));
                }
                else if (weightType == WeightType.Repetition)
                {
                    return sum / (weightSum - (squareSum / weightSum));
                }
            }

            return sum / weightSum;
        }

    }
}

