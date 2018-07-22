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
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using Accord.Statistics;

    /// <summary>
    ///   Split-Set Validation.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\SplitSetTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\SplitSetTest.cs" region="doc_learn_multiclass" />
    /// </example>
    /// 
    /// <seealso cref="Bootstrap{TModel, TInput, TOutput}"/>
    /// <seealso cref="CrossValidation{TModel, TInput, TOutput}"/>
    /// 
    public class SplitSetValidation<TModel, TInput, TOutput>
        : BaseSplitSetValidation<SplitResult<TModel, TInput, TOutput>, TModel, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
    {
        private double trainSetProportion = 0.8;

        /// <summary>
        ///   Gets the group labels assigned to each of the data samples.
        /// </summary>
        /// 
        public int[] Indices { get; set; }

        /// <summary>
        ///   Gets or sets the proportion of samples that should be 
        ///   reserved in the validation set. Default is 20%.
        /// </summary>
        /// 
        public double ValidationSetProportion
        {
            get { return 1.0 - trainSetProportion; }
            set { trainSetProportion = 1.0 - value; }
        }

        /// <summary>
        ///   Gets or sets the proportion of samples that should be 
        ///   reserved in the training set. Default is 80%.
        /// </summary>
        /// 
        public double TrainingSetProportion
        {
            get { return trainSetProportion; }
            set { trainSetProportion = value; }
        }

        /// <summary>
        ///   Gets the indices of elements in the validation set.
        /// </summary>
        /// 
        public int[] IndicesValidationSet { get; set; }

        /// <summary>
        ///   Gets the indices of elements in the training set.
        /// </summary>
        /// 
        public int[] IndicesTrainingSet { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="SplitSetValidation{TModel, TInput, TOutput}"/> class.
        /// </summary>
        public SplitSetValidation()
        {
            this.ValidationSetProportion = 0.2;
        }

        /// <summary>
        ///   Creates a list of the sample indices that should serve as the validation set.
        /// </summary>
        /// 
        /// <param name="x">The input data from where subsamples should be drawn.</param>
        /// <param name="y">The output data from where subsamples should be drawn.</param>
        /// 
        /// <returns>The indices of the samples in the original set that should compose the validation set.</returns>
        /// 
        protected virtual int[] CreateValidationSplits(TInput[] x, TOutput[] y)
        {
            return Classes.Random(samples: x.Length, proportion: trainSetProportion);
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
        public override SplitResult<TModel, TInput, TOutput> Learn(TInput[] x, TOutput[] y, double[] weights = null)
        {
            if (Learner == null)
                throw new InvalidOperationException("Please set the Learner property before calling the Learn(x, y) method.");

            if (Loss == null)
                throw new InvalidOperationException("Please set the Learner property before calling the Learn(x, y) method.");

            int n = x.Length;

            if (this.Indices == null || this.IndicesValidationSet == null || this.IndicesTrainingSet == null)
            {
                this.Indices = CreateValidationSplits(x, y);
                this.IndicesValidationSet = Indices.Find(i => i == 1);
                this.IndicesTrainingSet = Indices.Find(i => i == 0);
            }
            else
            {
                this.ValidationSetProportion = this.IndicesValidationSet.Length / (double)this.Indices.Length;
            }

            var split = new TrainValDataSplit<TInput, TOutput>(0, x, y, weights, IndicesTrainingSet, IndicesValidationSet);

            SplitResult<TModel, TInput, TOutput> result = LearnSubset(split);

            return result;
        }

    }
}
