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
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Obsolete. Please refer to <see cref="CrossValidation{TModel, TInput}"/> instead.
    /// </summary>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\CrossValidationTest.cs" region="doc_learn_hmm" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\DecisionTreeTest.cs" region="doc_cross_validation" />
    /// </example>
    /// 
    [Serializable]
    // TODO: Mark as obsolete
    // [Obsolete("Please refer to CrossValidation<TModel, TInput> instead.")]
    public class CrossValidationValues<TModel> 
        where TModel : class
    {

        /// <summary>
        ///   Gets the model.
        /// </summary>
        /// 
        public TModel Model { get; private set; }

        /// <summary>
        ///   Gets the validation value for the model.
        /// </summary>
        /// 
        public double ValidationValue { get; private set; }

        /// <summary>
        ///   Gets the variance of the validation 
        ///   value for the model, if available.
        /// </summary>
        /// 
        public double ValidationVariance { get; private set; }

        /// <summary>
        ///   Gets the training value for the model.
        /// </summary>
        /// 
        public double TrainingValue { get; private set; }

        /// <summary>
        ///   Gets the variance of the training 
        ///   value for the model, if available.
        /// </summary>
        /// 
        public double TrainingVariance { get; private set; }

        /// <summary>
        ///   Gets or sets a tag for user-defined information.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Creates a new Cross-Validation Values class.
        /// </summary>
        /// 
        /// <param name="trainingValue">The training value for the model.</param>
        /// <param name="validationValue">The validation value for the model.</param>
        /// 
        public CrossValidationValues(double trainingValue, double validationValue)
        {
            this.TrainingValue = trainingValue;
            this.ValidationValue = validationValue;
        }

        /// <summary>
        ///   Creates a new Cross-Validation Values class.
        /// </summary>
        /// 
        /// <param name="trainingValue">The training value for the model.</param>
        /// <param name="validationValue">The validation value for the model.</param>
        /// <param name="trainingVariance">The variance of the training values.</param>
        /// <param name="validationVariance">The variance of the validation values.</param>
        /// 
        public CrossValidationValues(
            double trainingValue, double trainingVariance,
            double validationValue, double validationVariance)
        {
            this.TrainingValue = trainingValue;
            this.ValidationValue = validationValue;

            this.TrainingVariance = trainingVariance;
            this.ValidationVariance = validationVariance;
        }

        /// <summary>
        ///   Creates a new Cross-Validation Values class.
        /// </summary>
        /// 
        /// <param name="model">The fitted model.</param>
        /// <param name="trainingValue">The training value for the model.</param>
        /// <param name="validationValue">The validation value for the model.</param>
        /// 
        public CrossValidationValues(TModel model, double trainingValue, double validationValue)
            : this(trainingValue, validationValue)
        {
            this.Model = model;
        }

        /// <summary>
        ///   Creates a new Cross-Validation Values class.
        /// </summary>
        /// 
        /// <param name="model">The fitted model.</param>
        /// <param name="trainingValue">The training value for the model.</param>
        /// <param name="validationValue">The validation value for the model.</param>
        /// <param name="trainingVariance">The variance of the training values.</param>
        /// <param name="validationVariance">The variance of the validation values.</param>
        /// 
        public CrossValidationValues(TModel model,
            double trainingValue, double trainingVariance,
            double validationValue, double validationVariance)
            : this(trainingValue, trainingVariance, validationValue, validationVariance)
        {
            this.Model = model;
        }

    }

}
