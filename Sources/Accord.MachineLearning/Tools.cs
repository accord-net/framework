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

namespace Accord.MachineLearning
{
    using Accord.MachineLearning.Performance;
    using Accord.Math;
    using Accord.Math.Random;
    using Accord.Statistics.Analysis;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    /// <summary>
    ///   Set of machine learning tools.
    /// </summary>
    /// 
    public static partial class Tools
    {
        /// <summary>
        ///   Splits the given text into individual atomic words, 
        ///   irrespective of punctuation and other marks.
        /// </summary>
        /// 
        public static string[][] Tokenize(this string[] x)
        {
            var r = new string[x.Length][];
            for (int i = 0; i < x.Length; i++)
                r[i] = Tokenize(x[i]);
            return r;
        }

        /// <summary>
        ///   Splits the given text into individual atomic words, 
        ///   irrespective of punctuation and other marks.
        /// </summary>
        /// 
        public static string[] Tokenize(this string x)
        {
            string s = Regex.Replace(x, @"[^\w]", " ");
            string[] words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].ToLowerInvariant();
            return words;
        }

        /// <summary>
        ///   Estimates the number of columns (dimensions) in a set of data.
        /// </summary>
        /// 
        /// <typeparam name="TInput">The type of the t input.</typeparam>
        /// 
        /// <param name="x">The input data.</param>
        /// 
        /// <returns>The number of columns (data dimensions) in the data.</returns>
        /// 
        public static int GetNumberOfInputs<TInput>(TInput[] x)
        {
            if (x.Length == 0)
                throw new ArgumentException("Impossible to determine number of inputs because there are no training samples in this set.");

            var first = x[0] as IList;
            if (first == null)
            {
                if (x[0] is int || x[0] is double)
                    return 1;
                return 0;
            }

            int length = first.Count;

            for (int i = 0; i < x.Length; i++)
            {
                IList l = x[i] as IList;
                if (l == null || l.Count != length)
                    return 0;
            }

            return length;
        }

        internal static void CheckArgs<TModel, TInput>(TInput[] x, double[] weights, Func<TModel> create,
            bool allowNull = false, bool allowNaN = false, bool allowNegativeSymbols = false)
            where TModel : ITransform
        {
            preCheckInputs(x, weights, allowNull: allowNull, allowNaN: allowNaN, allowNegativeSymbols: allowNegativeSymbols);

            var model = create();

            postCheck(model, x);
        }

        internal static void CheckArgs<TModel, TInput, TOutput>(TInput[] x, TOutput[] y, double[] weights, Func<TModel> create,
            bool onlyBinary = false, bool allowNull = false, bool allowNaN = false, bool allowNegativeSymbols = false, bool allowNegativeClasses = false)
            where TModel : ITransform<TInput, TOutput>
        {
            preCheckInputs(x, weights, allowNull: allowNull, allowNaN: allowNaN, allowNegativeSymbols: allowNegativeSymbols);
            preCheckOutputs(x, y, onlyBinary: onlyBinary, allowNaN: allowNaN, allowNull: allowNull, allowNegativeClasses: allowNegativeClasses);

            var model = create();

            postCheck(model, x);
        }

        private static void preCheckInputs<TInput>(TInput[] x, double[] weights, bool allowNull, bool allowNaN, bool allowNegativeSymbols)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (weights != null)
            {
                if (x.Length != weights.Length)
                    throw new DimensionMismatchException("weights", "The weights vector must have the same length as the vector of training samples 'x'.");
            }

            if (x.Length == 0)
                throw new ArgumentOutOfRangeException("inputs", "Training algorithm needs at least one training vector.");

            if (!allowNull)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] == null)
                        throw new ArgumentException("Input vector at position {0} is null.".Format(i), "x");
                }
            }

            if (!allowNaN)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    var di = x[i] as double[];
                    if (di != null)
                    {
                        for (int j = 0; j < di.Length; j++)
                        {
                            if (Double.IsNaN(di[j]))
                                throw new ArgumentException("The input vector at index {0} contains a NaN value at position {1}, which is not supported by this algorithm.".Format(i, j));

                            if (Double.IsInfinity(di[j]))
                                throw new ArgumentException("The input vector at index {0} contains a infinity value at position {1}, which is not supported by this algorithm.".Format(i, j));
                        }
                    }
                }
            }

            if (!allowNegativeSymbols)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    var ii = x[i] as int[];
                    if (ii != null)
                    {
                        for (int j = 0; j < ii.Length; j++)
                        {
                            if (ii[j] < 0)
                                throw new ArgumentException("The input vector ad index {0} contains a negative value at position {1}, which is not supported by this algorithm.".Format(i, j));
                        }
                    }
                }
            }
        }



        private static void preCheckOutputs<TInput, TOutput>(TInput[] x, TOutput[] y, bool onlyBinary, bool allowNaN, bool allowNull, bool allowNegativeClasses)
        {
            if (x.Length != y.Length)
                throw new DimensionMismatchException("y", "The number of output labels should match the number of training samples.");

            if (!allowNull)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (y[i] == null)
                        throw new ArgumentException("Input vector at position {0} is null.".Format(i), "x");
                }
            }

            if (onlyBinary)
            {
                int[] binary = y as int[];
                if (binary != null)
                    for (int i = 0; i < binary.Length; i++)
                    {
                        if (binary[i] != 1 && binary[i] != -1)
                        {
                            throw new ArgumentOutOfRangeException("y", "The output label at index {0} should be either +1 or -1.".Format(i));
                        }
                    }
                else
                {
                    bool[] b = y as bool[];
                    if (b == null)
                        throw new ArgumentException("y", "The output labels should had been either boolean or binary integers.");
                }
                return;
            }

            int[] classes = y as int[];
            if (classes != null)
            {
                if (!allowNegativeClasses)
                {
                    for (int i = 0; i < classes.Length; i++)
                    {
                        if (classes[i] < 0)
                            throw new ArgumentException("This learning algorithm does not support negative class labels." +
                                " The class label passed for instance {0} is {1}.".Format(i, classes[i]) + 
                                " If you are want to perform binary classification, please consider converting the" +
                                " labels to zero-one values with the Accord.Statistics.Classes.ToZeroOne() method first.");
                    }
                }

                int[] counts = Vector.Histogram(classes);
                for (int i = 0; i < counts.Length; i++)
                {
                    if (counts[i] < 1)
                        throw new ArgumentException("There are no samples for class label {0}. Please make sure that class labels are contiguous and there is at least one training sample for each label.".Format(i));
                }
            }
            else
            {
                double[][] oneHotDouble = y as double[][];
                int[][] oneHotInt = y as int[][];
                bool[][] oneHotBool = y as bool[][];
                double[] sum = null;

                if (oneHotDouble != null || oneHotInt != null || oneHotBool != null)
                {
                    if (oneHotDouble != null)
                    {
                        if (!allowNaN)
                        {
                            for (int i = 0; i < x.Length; i++)
                            {
                                var di = y[i] as double[];
                                if (di != null)
                                {
                                    for (int j = 0; j < di.Length; j++)
                                    {
                                        if (Double.IsNaN(di[j]))
                                            throw new ArgumentException("The input vector at index " + i + " contains NaN values.");

                                        if (Double.IsInfinity(di[j]))
                                            throw new ArgumentException("The input vector at index " + i + " contains infinity values.");
                                    }
                                }
                            }
                        }

                        sum = new double[oneHotDouble.Columns()];
                        for (int i = 0; i < y.Length; i++)
                            for (int j = 0; j < sum.Length; j++)
                                sum[j] += Math.Abs(oneHotDouble[i][j]);
                    }

                    if (oneHotInt != null)
                    {
                        sum = new double[oneHotInt.Columns()];
                        for (int i = 0; i < y.Length; i++)
                            for (int j = 0; j < sum.Length; j++)
                                sum[j] += Math.Abs(oneHotInt[i][j]);
                    }

                    if (oneHotBool != null)
                    {
                        sum = new double[oneHotBool.Columns()];
                        for (int i = 0; i < y.Length; i++)
                            for (int j = 0; j < sum.Length; j++)
                                sum[j] += oneHotBool[i][j] ? 1 : 0;
                    }

                    for (int j = 0; j < sum.Length; j++)
                    {
                        if (sum[j] == 0)
                        {
                            throw new ArgumentException("There are no samples for class label {0}. Please make sure that class labels are contiguous and there is at least one training sample for each label.".Format(j));
                        }
                    }
                }
            }
        }

        private static void postCheck<TModel, TInput>(TModel model, TInput[] x) where TModel : ITransform
        {
            if (model.NumberOfInputs > 0)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    var xi = x[i] as Array;
                    if (xi != null)
                    {
                        if (xi.Length != model.NumberOfInputs)
                        {
                            throw new DimensionMismatchException("inputs",
                                "The size of the input vector at index " + i + " does not match the expected number of inputs."
                                + " All input vectors must have the same length " + model.NumberOfInputs);
                        }
                    }
                }
            }
        }



        /// <summary>
        ///   Generates a <see cref="GeneralConfusionMatrix"/> from a set of cross-validation results.
        /// </summary>
        /// 
        /// <typeparam name="TModel">The type of the model being evaluated.</typeparam>
        /// <typeparam name="TInput">The type of the inputs accepted by the model.</typeparam>
        /// 
        /// <param name="cv">The cross-validation result.</param>
        /// <param name="inputs">The inputs fed to the cross-validation object.</param>
        /// <param name="outputs">The outputs fed to the cross-validation object.</param>
        /// 
        /// <returns>A <see cref="GeneralConfusionMatrix"/> that captures the performance of the model across all validation folds.</returns>
        /// 
        public static GeneralConfusionMatrix ToConfusionMatrix<TModel, TInput>(this CrossValidationResult<TModel, TInput, int> cv, TInput[] inputs, int[] outputs)
            where TModel : class, ITransform<TInput, int>
        {
            var actual = new List<int>();
            var expected = new List<int>();
            foreach (SplitResult<TModel, TInput, int> model in cv.Models)
            {
                int[] idx = model.Validation.Indices;
                TInput[] x = inputs.Get(idx);
                int[] e = outputs.Get(idx);
                int[] a = model.Transform(x);
                actual.AddRange(a);
                expected.AddRange(e);
            }

            return new GeneralConfusionMatrix(expected: expected.ToArray(), predicted: actual.ToArray());
        }

        /// <summary>
        ///   Generates a <see cref="ConfusionMatrix"/> from a set of cross-validation results.
        /// </summary>
        /// 
        /// <typeparam name="TModel">The type of the model being evaluated.</typeparam>
        /// <typeparam name="TInput">The type of the inputs accepted by the model.</typeparam>
        /// 
        /// <param name="cv">The cross-validation result.</param>
        /// <param name="inputs">The inputs fed to the cross-validation object.</param>
        /// <param name="outputs">The outputs fed to the cross-validation object.</param>
        /// 
        /// <returns>A <see cref="ConfusionMatrix"/> that captures the performance of the model across all validation folds.</returns>
        /// 
        public static ConfusionMatrix ToConfusionMatrix<TModel, TInput>(this CrossValidationResult<TModel, TInput, bool> cv, TInput[] inputs, bool[] outputs)
            where TModel : class, ITransform<TInput, bool>
        {
            var actual = new List<bool>();
            var expected = new List<bool>();
            foreach (SplitResult<TModel, TInput, bool> model in cv.Models)
            {
                int[] idx = model.Validation.Indices;
                TInput[] x = inputs.Get(idx);
                bool[] e = outputs.Get(idx);
                bool[] a = model.Transform(x);
                actual.AddRange(a);
                expected.AddRange(e);
            }

            return new ConfusionMatrix(expected: expected.ToArray(), predicted: actual.ToArray());
        }
    }
}