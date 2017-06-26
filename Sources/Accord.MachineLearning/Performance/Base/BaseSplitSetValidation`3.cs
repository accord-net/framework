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
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics;

    /// <summary>
    ///   Function signature for a function that creates a machine learning <typeparamref name="TModel"/> 
    ///   from a <see cref="DataSubset{TInput, TOutput}"/> subset of the training data. This function 
    ///   should take a subset of the data as input, and create a <see cref="ISupervisedLearning{TModel, TInput, TOutput}"/>
    ///   algorithm that can create a model using this given subset.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    /// <param name="subset">The training subset where .</param>
    /// 
    /// <returns>ISupervisedLearning&lt;TModel, TInput, TOutput&gt;.</returns>
    public delegate ISupervisedLearning<TModel, TInput, TOutput> SplitSetValidationSupervisedLearn<TModel, TInput, TOutput>(DataSubset<TInput, TOutput> subset)
        where TModel : class, ITransform<TInput, TOutput>;

    /// <summary>
    ///   Function signature for a function that creates a machine learning <typeparamref name="TModel"/> 
    ///   from a <see cref="DataSubset{TInput, TOutput}"/> subset of the training data. This function 
    ///   should take a subset of the data as input, and create a <see cref="ISupervisedLearning{TModel, TInput, TOutput}"/>
    ///   algorithm that can create a model using this given subset.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    /// <param name="subset">The training subset where .</param>
    /// 
    /// <returns>A <see cref="ISupervisedLearning{TModel, TInput, TOutput}"/> teaching algorithm for the <typeparamref name="TModel"/>.</returns>
    /// 
    public delegate IUnsupervisedLearning<TModel, TInput, TOutput> SplitSetValidationUnspervisedLearn<TModel, TInput, TOutput>(DataSubset<TInput> subset)
        where TModel : class, ITransform<TInput, TOutput>;

    /// <summary>
    ///   Function signature for a function that can compute a performance metric (i.e. a <see cref="ILoss{T}"/>) from
    ///   a set of <paramref name="expected"/> (ground-truth) and <paramref name="actual"/> (model prediction) output
    ///   values. Additional information about the metric (such as its variance) can be set in the object passed as 
    ///   the <paramref name="result"/> parameter.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    /// <param name="expected">The ground-truth data that the model was supposed to predict.</param>
    /// <param name="actual">The data that the model has actually predicted.</param>
    /// <param name="result">A <see cref="SetResult{TModel}"/> object that can be used to obtain more information
    ///   about the data split being evaluated and store additional information about the computed metric.</param>
    /// 
    /// <returns>A metric that measures how far the model predictions were from the expected ground-truth.</returns>
    /// 
    public delegate double SplitSetValidationLoss<TModel, TInput, TOutput>(TOutput[] expected, TOutput[] actual, SetResult<TModel> result);

    /// <summary>
    ///   Base class for performance measurement methods based on splitting the data into multiple sets,
    ///   such as <see cref="SplitSetValidation{TModel, TInput, TOutput}"/>, <see cref="CrossValidation{TModel, TInput, TOutput}"/>
    ///   and <see cref="Bootstrap{TModel, TInput, TOutput}"/>.
    /// </summary>
    /// 
    /// <typeparam name="TResult">The type of the result learned by the validation method (e.g. <see cref="CrossValidationResult{TModel, TInput, TOutput}"/>).</typeparam>
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data or labels.</typeparam>
    /// 
    public abstract class BaseSplitSetValidation<TResult, TModel, TInput, TOutput> : ParallelLearningBase,
        ISupervisedLearning<TResult, TInput, TOutput>
        where TModel : class, ITransform<TInput, TOutput>
        where TResult : ITransform<TInput, TOutput>
    {
        private SplitSetValidationSupervisedLearn<TModel, TInput, TOutput> learner;
        private SplitSetValidationLoss<TModel, TInput, TOutput> loss;
        private double? defaultValue = null;

        /// <summary>
        ///   Gets or sets a value to be used as the <see cref="Loss"/> in case the model throws
        ///   an exception during learning. Default is null (exceptions will not be ignored).
        /// </summary>
        /// 
        public double? DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        ///   Gets or sets a <see cref="SplitSetValidationSupervisedLearn{TModel, TInput, TOutput}"/> function 
        ///   that can be used to create a <typeparamref name="TModel"/> from a subset of the learning dataset.
        /// </summary>
        /// 
        public SplitSetValidationSupervisedLearn<TModel, TInput, TOutput> Learner
        {
            get { return learner; }
            set { learner = value; }
        }

        /// <summary>
        ///   Gets or sets a <see cref="SplitSetValidationLoss{TModel, TInput, TOutput}"/> function that can
        ///   be used to measure how far the actual model predictions were from the expected ground-truth.
        /// </summary>
        /// 
        public SplitSetValidationLoss<TModel, TInput, TOutput> Loss
        {
            get { return loss; }
            set { loss = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSplitSetValidation{TResult, TModel, TInput, TOutput}"/> class.
        /// </summary>
        protected BaseSplitSetValidation()
        {

        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.</returns>
        public abstract TResult Learn(TInput[] x, TOutput[] y, double[] weights = null);

        /// <summary>
        ///   Learns and evaluates a model in a single subset of the data.
        /// </summary>
        /// 
        /// <param name="subset">The subset of the data containing the training and testing subsets where
        ///   a model should be trained and evaluated, respectively.</param>
        /// <param name="index">The index of this subset, if applicable.</param>
        /// 
        /// <returns>A <see cref="SplitResult{TModel, TInput, TOutput}"/> object containing the created model
        ///   and its performance on the training and validation sets.</returns>
        /// 
        protected SplitResult<TModel, TInput, TOutput> LearnSubset(TrainValDataSplit<TInput, TOutput> subset, int index = 0)
        {
            // Create the learning algorithm
            ISupervisedLearning<TModel, TInput, TOutput> teacher = Learner(subset.Training);

            TInput[] trainInputs = subset.Training.Inputs;
            TOutput[] trainOutputs = subset.Training.Outputs;
            double[] trainWeights = subset.Training.Weights;

            // Use the learning algorithm to learn a new model
            try
            {
                TModel model = teacher.Learn(trainInputs, trainOutputs, trainWeights);

                // Evaluate metrics using the created model
                TInput[] valInputs = subset.Validation.Inputs;
                TOutput[] valOutputs = subset.Validation.Outputs;
                double[] valWeights = subset.Validation.Weights;

                var trainResult = new SetResult<TModel>(model, subset.Training.Indices, "Training", subset.Training.Proportion);
                var valResult = new SetResult<TModel>(model, subset.Validation.Indices, "Validation", subset.Validation.Proportion);

                trainResult.Value = Loss(trainOutputs, model.Transform(trainInputs), trainResult);
                valResult.Value = Loss(valOutputs, model.Transform(valInputs), valResult);

                return new SplitResult<TModel, TInput, TOutput>(model, index: index)
                {
                    Training = trainResult,
                    Validation = valResult
                };
            }
            catch
            {
                if (!defaultValue.HasValue)
                    throw;

                return new SplitResult<TModel, TInput, TOutput>(null, index: index)
                {
                    Training = new SetResult<TModel>(null, subset.Training.Indices, "Training", subset.Training.Proportion)
                    {
                        Value = defaultValue.Value
                    },

                    Validation = new SetResult<TModel>(null, subset.Validation.Indices, "Validation", subset.Validation.Proportion)
                    {
                        Value = defaultValue.Value
                    }
                };
            }
        }

    }
}
