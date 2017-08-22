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
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Bootstrap method for generalization performance measurements.
    /// </summary>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\BootstrapTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    /// <seealso cref="CrossValidation{TModel, TInput}"/>
    /// <seealso cref="SplitSetValidation{TModel, TInput}"/>
    /// 
    public class Bootstrap<TModel, TInput, TOutput> : BaseSplitSetValidation<BootstrapResult<TModel, TInput, TOutput>, TModel, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
    {
        private int resamplings;
        private int[][] subsampleIndices;
        private int subsampleSize;

        /// <summary>
        ///   Gets or sets the number B of bootstrap samplings
        ///   to be drawn from the population dataset.
        /// </summary>
        /// 
        public int B
        {
            get { return resamplings; }
            set { resamplings = value; }
        }

        /// <summary>
        ///   Gets or sets the number of samples to be drawn in each subsample. If 
        ///   set to zero, all samples in the entire dataset will be selected.
        /// </summary>
        /// 
        public int NumberOfSubsamples
        {
            get { return subsampleSize; }
            set { subsampleSize = value; }
        }

        /// <summary>
        ///   Gets the bootstrap samples drawn from the population dataset as indices.
        /// </summary>
        /// 
        public int[][] SubSampleIndices { get { return subsampleIndices; } }


        /// <summary>
        ///   Initializes a new instance of the <see cref="Bootstrap" /> class.
        /// </summary>
        /// 
        public Bootstrap()
        {

        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Please set the Learner property before calling the Learn(x, y) method.
        /// or
        /// Please set the Learner property before calling the Learn(x, y) method.
        /// </exception>
        public override BootstrapResult<TModel, TInput, TOutput> Learn(TInput[] x, TOutput[] y, double[] weights = null)
        {
            if (Learner == null)
                throw new InvalidOperationException("Please set the Learner property before calling the Learn(x, y) method.");

            if (Loss == null)
                throw new InvalidOperationException("Please set the Learner property before calling the Learn(x, y) method.");

            int sampleSize = this.subsampleSize;
            if (sampleSize == 0)
                sampleSize = x.Length;

            this.subsampleIndices = CreateSubSampleIndices(x, y, resamplings, sampleSize);


            var foldResults = new SplitResult<TModel, TInput, TOutput>[B];

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                for (int i = 0; i < B; i++)
                    foldResults[i] = LearnSubset(GetSubsample(i, x, y, weights), i);
            }
            else
            {
                Parallel.For(0, B, i =>
                {
                    foldResults[i] = LearnSubset(GetSubsample(i, x, y, weights), i);
                });
            }

            return new BootstrapResult<TModel, TInput, TOutput>(foldResults);
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
        protected virtual int[][] CreateSubSampleIndices(TInput[] x, TOutput[] y, int resamplings, int subsampleSize)
        {
            // Create samples with replacement
            int[][] idx = new int[resamplings][];

            int size = x.Length;

            var random = Accord.Math.Random.Generator.Random;

            // For each fold to be created
            for (int i = 0; i < idx.Length; i++)
            {
                idx[i] = new int[subsampleSize];

                // Generate a random sample (with replacement)
                for (int j = 0; j < idx[i].Length; j++)
                    idx[i][j] = random.Next(0, size);
            }

            return idx;
        }

        /// <summary>
        ///   Gets a subset of the training and testing sets.
        /// </summary>
        /// 
        /// <param name="subsampleIndex">Index of the subsample.</param>
        /// <param name="x">The input data x.</param>
        /// <param name="y">The output data y.</param>
        /// <param name="weights">The weights of each sample.</param>
        /// 
        /// <returns>A <see cref="TrainValDataSplit{TInput, TOutput}"/> that defines a 
        ///   data split of a subsample of the dataset.</returns>
        /// 
        public TrainValDataSplit<TInput, TOutput> GetSubsample(int subsampleIndex, TInput[] x, TOutput[] y, double[] weights)
        {
            // The training set is already computed
            int[] trainingSet = this.subsampleIndices[subsampleIndex];

            // The validation set is the complement of the training set
            int[] validationSet = Vector.Range(0, x.Length).Except(trainingSet).ToArray();

            return new TrainValDataSplit<TInput, TOutput>(subsampleIndex, x, y, weights, trainingSet, validationSet);
        }

    }
}
