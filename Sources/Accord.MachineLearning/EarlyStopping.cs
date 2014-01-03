// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Modes for storing models.
    /// </summary>
    /// 
    public enum ModelStorageMode
    {
        /// <summary>
        ///   Stores a model on each iteration. This is the most
        ///   intensive method, but enables a quick restoration 
        ///   of any point on the learning history.
        /// </summary>
        /// 
        AllModels,

        /// <summary>
        ///   Stores only the model which had shown the minimum 
        ///   validation value in the training history. All other
        ///   models are discarded and only their validation and
        ///   training values will be registered.
        /// </summary>
        /// 
        MinimumOnly,

        /// <summary>
        ///   Stores only the model which had shown the maximum 
        ///   validation value in the training history. All other
        ///   models are discarded and only their validation and
        ///   training values will be registered.
        /// </summary>
        /// 
        MaximumOnly
    };

    /// <summary>
    ///   Early stopping training procedure.
    /// </summary>
    /// 
    /// <remarks>
    ///   The early stopping training procedure monitors a validation set
    ///   during training to determine when a learning algorithm has stopped
    ///   learning and started to overfit data. This class keeps an history
    ///   of training and validation errors and will keep the best model found
    ///   during learning.
    /// </remarks>
    /// 
    /// <typeparam name="TModel">The type of the model to be trained.</typeparam>
    /// 
    public class EarlyStopping<TModel> where TModel : class, ICloneable
    {
        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the early stopping algorithm. Default
        ///   is 0 (run until convergence).
        /// </summary>
        /// 
        public int MaxIterations { get; set; }

        /// <summary>
        ///   Gets or sets the minimum tolerance value used
        ///   to determine convergence. Default is 1e-5.
        /// </summary>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets the history of training and validation values 
        ///   registered at each iteration of the learning algorithm.
        /// </summary>
        /// 
        public Dictionary<int, CrossValidationValues<TModel>> History { get; private set; }

        /// <summary>
        ///   Gets the model with minimum validation error found during learning.
        /// </summary>
        /// 
        public KeyValuePair<int, CrossValidationValues<TModel>> MinValidationValue { get; private set; }

        /// <summary>
        ///   Gets the model with maximum validation error found during learning.
        /// </summary>
        /// 
        public KeyValuePair<int, CrossValidationValues<TModel>> MaxValidationValue { get; private set; }

        /// <summary>
        ///   Gets or sets the storage policy for the procedure.
        /// </summary>
        public ModelStorageMode Mode { get; set; }

        /// <summary>
        ///   Gets or sets the iteration function for the procedure. This
        ///   function will be called on each iteration and should run one
        ///   iteration of the learning algorithm for the given model.
        /// </summary>
        /// 
        public Func<int, CrossValidationValues<TModel>> IterationFunction { get; set; }


        /// <summary>
        ///   Creates a new early stopping procedure object.
        /// </summary>
        /// 
        public EarlyStopping()
        {
            History = new Dictionary<int, CrossValidationValues<TModel>>();
            Tolerance = 1e-5;
        }

        /// <summary>
        ///   Starts the model training, calling the <see cref="IterationFunction"/>
        ///   on each iteration.
        /// </summary>
        /// 
        /// <returns>True if the model training has converged, false otherwise.</returns>
        /// 
        public bool Compute()
        {
            double lastError = Double.PositiveInfinity;

            for (int i = 0; i < MaxIterations; i++)
            {
                CrossValidationValues<TModel> value = IterationFunction(i);

                double currentError = value.TrainingValue;

                // If the storage mode is set to all models, the history should store
                // all created models alongside with the validation and training errors.

                if (Mode == ModelStorageMode.AllModels)
                {
                    // Create a copy of the model information and of the created model. We 
                    // have to clone it because it will keep changing in further iterations.
                    CrossValidationValues<TModel> clone = cloneValue(value, includeModel: true);

                    History[i] = clone;

                    // Check if we should store the value as current maximum or minimum
                    if (MinValidationValue.Value == null || MinValidationValue.Value == null)
                    {
                        // If this is the first iteration, store the first model as max/min
                        MinValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);
                        MaxValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);
                    }
                    else
                    {
                        // Store information only if the model is better
                        if (value.ValidationValue < MinValidationValue.Value.ValidationValue)
                            MinValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);

                        if (value.ValidationValue > MaxValidationValue.Value.ValidationValue)
                            MaxValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);
                    }
                }
                else // if (Mode == ModelStorageMode.MinimumOnly || Mode == ModelStorageMode.MaximumOnly)
                {
                    // Create a copy of the model information and of the created model. We 
                    // will not include the model at this step because we will be storing it
                    // only if it is a minimum.
                    CrossValidationValues<TModel> copy = cloneValue(value, includeModel: false);

                    History[i] = copy;

                    // Check if we should store the value as current maximum or minimum
                    if (MinValidationValue.Value == null || MinValidationValue.Value == null)
                    {
                        // If this is the first iteration, store the first model as current maximum and minimum
                        CrossValidationValues<TModel> clone = cloneValue(value, includeModel: true);
                        MaxValidationValue = MinValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);
                    }
                    else
                    {
                        // Store information only if the model is better
                        if (value.ValidationValue < MinValidationValue.Value.ValidationValue)
                        {
                            CrossValidationValues<TModel> clone = cloneValue(value, includeModel: true);
                            MinValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);
                        }

                        if (value.ValidationValue > MaxValidationValue.Value.ValidationValue)
                        {
                            CrossValidationValues<TModel> clone = cloneValue(value, includeModel: true);
                            MaxValidationValue = new KeyValuePair<int, CrossValidationValues<TModel>>(i, clone);
                        }

                    }
                }

                // Check for convergence
                if (Math.Abs(currentError - lastError) < Tolerance * Math.Abs(lastError))
                    return true; // converged
            }

            // Maximum iterations reached
            return Tolerance == 0 ? false : true;
        }

        private static CrossValidationValues<TModel> cloneValue(CrossValidationValues<TModel> value, bool includeModel)
        {
            if (includeModel)
            {
                return new CrossValidationValues<TModel>((TModel)value.Model.Clone(), value.TrainingValue, value.ValidationValue)
                {
                    Tag = value.Tag
                };
            }
            else
            {
                return new CrossValidationValues<TModel>(null, value.TrainingValue, value.ValidationValue)
                {
                    Tag = value.Tag
                };
            }
        }

    }
}
