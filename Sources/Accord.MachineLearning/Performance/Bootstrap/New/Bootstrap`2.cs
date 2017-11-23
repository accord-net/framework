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

namespace Accord.MachineLearning.Performance
{
    using System;
    using System.Linq;
    using Accord.Math;
    using Accord.MachineLearning.Performance;
    using Accord.Statistics.Analysis;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Bootstrap method for generalization performance measurements (with 
    ///   support for stratification and default loss function for classification).
    /// </summary>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\BootstrapTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// 
    /// <seealso cref="CrossValidation{TModel, TInput}"/>
    /// <seealso cref="SplitSetValidation{TModel, TInput}"/>
    /// 
    public class Bootstrap<TModel, TInput> : Bootstrap<TModel, TInput, int>
        where TModel : class, IClassifier<TInput, int>
    {


        /// <summary>
        ///   Gets or sets a value indicating whether the prevalence of an output 
        ///   label should be balanced between training and testing sets. Default is false.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if this instance is stratified; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Stratify { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrap{TModel, TInput}"/> class.
        /// </summary>
        public Bootstrap()
        {
            this.Loss = (expected, actual, r) =>
            {
                return new GeneralConfusionMatrix(r.Model.NumberOfClasses, expected, actual).Error;
            };
        }

        /// <summary>
        ///   Draws the bootstrap samples from the population.
        /// </summary>
        /// 
        /// <param name="x">The input data from where subsamples should be drawn.</param>
        /// <param name="y">The output data from where subsamples should be drawn.</param>
        /// <param name="resamplings">The number of samples to drawn.</param>
        /// <param name="subsampleSize">The size of the samples to be drawn.</param>
        /// 
        /// <returns>The indices of the samples in the original set.</returns>
        /// 
        protected override int[][] CreateSubSampleIndices(TInput[] x, int[] y, int resamplings, int subsampleSize)
        {
            if (!Stratify)
                return base.CreateSubSampleIndices(x, y, resamplings, subsampleSize);

            // Create samples with replacement
            int[][] idx = new int[resamplings][];

            int size = x.Length;

            var random = Accord.Math.Random.Generator.Random;

            int numberOfClasses = y.DistinctCount();
            int samplesPerClass = (int)Math.Ceiling(subsampleSize / (double)numberOfClasses);

            var sampleIndicesPerClass = new List<int>[numberOfClasses];
            for (int i = 0; i < y.Length; i++)
                sampleIndicesPerClass[y[i]].Add(i);


            // For each fold to be created
            for (int i = 0; i < idx.Length; i++)
            {
                idx[i] = new int[subsampleSize];

                // Generate a fixed amount of samples from each class
                for (int j = 0, p = 0; j < sampleIndicesPerClass.Length; j++)
                {
                    for (int k = 0; k < samplesPerClass; k++, p++)
                    {
                        int m = sampleIndicesPerClass[j].Count;
                        idx[i][p] = sampleIndicesPerClass[j][random.Next(0, m)];
                    }
                }

                // Fill the rest with random examples
                for (int k = samplesPerClass * numberOfClasses; k < idx[i].Length; k++)
                    idx[i][k] = random.Next(0, size);
            }

            return idx;
        }

    }
}
