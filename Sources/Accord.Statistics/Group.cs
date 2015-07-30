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
    using Accord.Math;
    using System;
    using System.Collections.Generic;

    internal static class Groups
    {
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
            double[] r = new double[positives.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = (double)positives[i] / (positives[i] + negatives[i]);
            return r;
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
            double[] r = new double[data.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = (double)data[i][positiveColumn] / (data[i][positiveColumn] + data[i][negativeColumn]);
            return r;
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
            var groups = new List<int>();
            var groupings = new List<int[]>();

            for (int i = 0; i < data.Length; i++)
            {
                int group = data[i][labelColumn];
                if (!groups.Contains(group))
                {
                    groups.Add(group);

                    int positives = 0, negatives = 0;
                    for (int j = 0; j < data.Length; j++)
                    {
                        if (data[j][labelColumn] == group)
                        {
                            if (data[j][dataColumn] == 0)
                                negatives++;
                            else positives++;
                        }
                    }

                    groupings.Add(new int[] { group, positives, negatives });
                }
            }

            return groupings.ToArray();
        }

        /// <summary>
        ///   Divides values into groups given a vector 
        ///   containing the group labels for every value.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">The values to be separated into groups.</param>
        /// <param name="labels">
        ///   A vector containing the class label associated with each of the 
        ///   values. The labels must begin on 0 and its maximum value should
        ///   be the number of groups - 1.</param>
        /// 
        /// <returns>The original values divided into groups.</returns>
        /// 
        public static T[][] Group<T>(T[] values, int[] labels)
        {
            return Group(values, labels, labels.Max() + 1);
        }

        /// <summary>
        ///   Divides values into groups given a vector 
        ///   containing the group labels for every value.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">The values to be separated into groups.</param>
        /// <param name="labels">
        ///   A vector containing the class label associated with each of the 
        ///   values. The labels must begin on 0 and its maximum value should
        ///   be the number of groups - 1.</param>
        /// <param name="groups">The number of groups.</param>
        /// 
        /// <returns>The original values divided into groups.</returns>
        /// 
        public static T[][] Group<T>(T[] values, int[] labels, int groups)
        {
            if (values.Length != labels.Length)
                throw new DimensionMismatchException("labels");

            var result = new T[groups][];

            for (int i = 0; i < result.Length; i++)
            {
                var group = new List<T>();

                for (int j = 0; j < values.Length; j++)
                {
                    if (labels[j] == i)
                        group.Add(values[j]);
                }

                result[i] = group.ToArray();
            }

            return result;
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
            if (data.Length != positives.Length)
                throw new DimensionMismatchException();

            if (positives.Length != negatives.Length)
                throw new DimensionMismatchException();

            List<int[]> rows = new List<int[]>();

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < positives[i]; j++)
                    rows.Add(new int[] { data[i], 1 });

                for (int j = 0; j < negatives[i]; j++)
                    rows.Add(new int[] { data[i], 0 });
            }

            return rows.ToArray();
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
            List<int[]> rows = new List<int[]>();

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i][positiveColumn]; j++)
                    rows.Add(new int[] { data[i][labelColumn], 1 });

                for (int j = 0; j < data[i][negativeColumn]; j++)
                    rows.Add(new int[] { data[i][labelColumn], 0 });
            }

            return rows.ToArray();
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
            return Expand(labels, labels.Max() + 1);
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
            return Expand(labels, labels.Max() + 1, negative, positive);
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
            double[][] outputs = new double[labels.Length][];

            for (int i = 0; i < labels.Length; i++)
            {
                outputs[i] = new double[classes];
                outputs[i][labels[i]] = 1.0;
            }

            return outputs;
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
            double[][] outputs = new double[labels.Length][];

            for (int i = 0; i < labels.Length; i++)
            {
                var row = outputs[i] = new double[classes];
                for (int j = 0; j < row.Length; j++)
                    row[j] = negative;
                row[labels[i]] = positive;
            }

            return outputs;
        }

        /// <summary>
        ///   Returns a random group assignment for a sample.
        /// </summary>
        /// 
        /// <param name="size">The sample size.</param>
        /// <param name="groups">The number of groups.</param>
        /// 
        public static int[] Random(int size, int groups)
        {
            // Create the index vector
            int[] idx = new int[size];

            if (groups == 1)
            {
                for (int i = 0; i < idx.Length; i++)
                    idx[i] = 0;
                return idx;
            }

            double n = groups / (double)size;
            for (int i = 0; i < idx.Length; i++)
                idx[i] = (int)System.Math.Ceiling((i + 0.9) * n) - 1;

            // Shuffle the indices vector
            Vector.Shuffle(idx);

            return idx;
        }

        /// <summary>
        ///   Returns a random group assignment for a sample
        ///   into two mutually exclusive groups.
        /// </summary>
        /// 
        /// <param name="size">The sample size.</param>
        /// <param name="proportion">The proportion of samples between the groups.</param>
        /// 
        public static int[] Random(int size, double proportion)
        {
            // Create the index vector
            int[] idx = new int[size];

            int mid = (int)Math.Floor(proportion * size);

            for (int i = 0; i < mid; i++)
                idx[i] = 0;

            for (int i = mid; i < idx.Length; i++)
                idx[i] = 1;

            // Shuffle the indices vector
            Vector.Shuffle(idx);

            return idx;
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
        public static int[] Random(int[] labels, int classes, int groups)
        {
            int size = labels.Length;

            var buckets = new List<Tuple<int, int>>[classes];
            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = new List<Tuple<int, int>>();

            for (int i = 0; i < labels.Length; i++)
                buckets[labels[i]].Add(Tuple.Create(i, labels[i]));

            for (int i = 0; i < buckets.Length; i++)
                Vector.Shuffle(buckets);

            var partitions = new List<Tuple<int, int>>[groups];
            for (int i = 0; i < partitions.Length; i++)
                partitions[i] = new List<Tuple<int, int>>();

            // We are going to take samples from the buckets and assign to 
            // groups. For this, we will be following the buckets in order,
            // such that new samples are drawn equally from each bucket.

            bool allEmpty = true;
            int bucketIndex = 0;
            int partitionIndex = 0;

            do
            {
                for (int i = 0; i < partitions.Length; i++)
                {
                    allEmpty = true;

                    var currentPartition = partitions[partitionIndex];
                    partitionIndex = (partitionIndex + 1) % partitions.Length;

                    for (int j = 0; j < buckets.Length; j++)
                    {
                        var currentBucket = buckets[bucketIndex];
                        bucketIndex = (bucketIndex + 1) % buckets.Length;

                        if (currentBucket.Count == 0)
                            continue;

                        allEmpty = false;

                        var next = currentBucket[currentBucket.Count - 1];
                        currentBucket.RemoveAt(currentBucket.Count - 1);
                        currentPartition.Add(next);
                    }
                }

            } while (!allEmpty);

            for (int i = 0; i < partitions.Length; i++)
                Vector.Shuffle(partitions[i]);

            int[] splittings = new int[labels.Length];
            for (int i = 0; i < partitions.Length; i++)
                foreach (var index in partitions[i])
                    splittings[index.Item1] = i;

            return splittings;
        }
    }
}
