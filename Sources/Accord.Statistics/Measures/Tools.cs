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
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Kernels;
    using AForge;

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
        // TODO: Make this class obsolete

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
        [Obsolete("Please use Classes.GetRatio instead.")]
        public static double[] Proportions(int[] positives, int[] negatives)
        {
            return Accord.Statistics.Classes.GetRatio(positives, negatives);
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
        [Obsolete("Please use Classes.GetRatio instead.")]
        public static double[] Proportions(int[][] data, int positiveColumn, int negativeColumn)
        {
            return Accord.Statistics.Classes.GetRatio(data, positiveColumn, negativeColumn);
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
        [Obsolete("Please use Classes.Summarize instead.")]
        public static int[][] Group(int[][] data, int labelColumn, int dataColumn)
        {
            return Accord.Statistics.Classes.Summarize(data, labelColumn, dataColumn);
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
        [Obsolete("Please use Classes.Expand instead.")]
        public static int[][] Expand(int[] data, int[] positives, int[] negatives)
        {
            return Accord.Statistics.Classes.Expand(data, positives, negatives);
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
        [Obsolete("Please use Classes.Expand instead.")]
        public static int[][] Expand(int[][] data, int labelColumn, int positiveColumn, int negativeColumn)
        {
            return Accord.Statistics.Classes.Expand(data, labelColumn, positiveColumn, negativeColumn);
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
        [Obsolete("Please use Jagged.OneHot instead.")]
        public static double[][] Expand(int[] labels)
        {
            return Jagged.OneHot(labels, labels.DistinctCount());
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
        [Obsolete("Please use Jagged.OneHot instead.")]
        public static double[][] Expand(int[] labels, double negative, double positive)
        {
            return Jagged.OneHot(labels).Replace(0, negative).Replace(1, positive);
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
        [Obsolete("Please use Jagged.OneHot instead.")]
        public static double[][] Expand(int[] labels, int classes)
        {
            return Jagged.OneHot(labels, classes);
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
        [Obsolete("Please use Jagged.OneHot instead.")]
        public static double[][] Expand(int[] labels, int classes, double negative, double positive)
        {
            return Jagged.OneHot(labels, classes).Replace(0, negative).Replace(1, positive);
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
        [Obsolete("Please use Vector.Sample instead.")]
        public static int[] RandomSample(int n, int k)
        {
            return Accord.Math.Vector.Sample(k, n);
        }

        /// <summary>
        ///   Returns a random group assignment for a sample.
        /// </summary>
        /// 
        /// <param name="size">The sample size.</param>
        /// <param name="groups">The number of groups.</param>
        /// 
        [Obsolete("Please use Classes.Random instead.")]
        public static int[] RandomGroups(int size, int groups)
        {
            return Accord.Statistics.Classes.Random(size, groups);
        }

        /// <summary>
        ///   Returns a random group assignment for a sample
        ///   into two mutually exclusive groups.
        /// </summary>
        /// 
        /// <param name="size">The sample size.</param>
        /// <param name="proportion">The proportion of samples between the groups.</param>
        /// 
        [Obsolete("Please use Classes.Random instead.")]
        public static int[] RandomGroups(int size, double proportion)
        {
            return Accord.Statistics.Classes.Random(size, proportion);
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
        [Obsolete("Please use Classes.Random instead.")]
        public static int[] RandomGroups(int[] labels, int classes, int groups)
        {
            return Accord.Statistics.Classes.Random(labels, classes, groups);
        }

        /// <summary>
        ///   Returns a random permutation of size n.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Sample instead.")]
        public static int[] Random(int n)
        {
            return Vector.Sample(n);
        }

        /// <summary>
        ///   Shuffles an array.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Shuffle instead.")]
        public static void Shuffle<T>(T[] array)
        {
            Vector.Shuffle(array);
        }

        /// <summary>
        ///   Shuffles a collection.
        /// </summary>
        /// 
        [Obsolete("Please use Vector.Shuffle instead.")]
        public static void Shuffle<T>(IList<T> array)
        {
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
            // TODO: Move into PCA and mark as obsolete
            if (value == null)
                throw new ArgumentNullException("value");


            int cols = value.GetLength(1);

            double[,] cov = value.Covariance();

            // Diagonalizes the covariance matrix
            var svd = new SingularValueDecomposition(cov,
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
            return Matrix.Dot(value, transformMatrix);
        }

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
        public static double[][] Whitening(double[][] value, out double[][] transformMatrix)
        {
            // TODO: Move into PCA and mark as obsolete
            if (value == null)
                throw new ArgumentNullException("value");


            int cols = value.Columns();

            double[][] cov = value.Covariance();

            // Diagonalizes the covariance matrix
            var svd = new JaggedSingularValueDecomposition(cov,
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
                    transformMatrix[i][j] /= Math.Sqrt(singularValues[j]);

            // Return the transformed data
            return Matrix.Dot(value, transformMatrix);
        }

    }
}

