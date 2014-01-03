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
    using System.Linq;
    using Accord.Math;

    /// <summary>
    ///   Fitting function delegate.
    /// </summary>
    /// 
    public delegate SplitSetStatistics<TModel> SplitValidationFittingFunction<TModel>(int[] trainingSamples)
        where TModel : class;

    /// <summary>
    ///   Evaluating function delegate.
    /// </summary>
    /// 
    public delegate SplitSetStatistics<TModel> SplitValidationEvaluateFunction<TModel>(int[] validationSamples, TModel model)
        where TModel : class;

    /// <summary>
    ///   Split-Set Validation.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model being analyzed.</typeparam>
    /// 
    /// <seealso cref="Bootstrap"/>
    /// <seealso cref="CrossValidation{T}"/>
    /// <seealso cref="SplitSetValidation"/>
    /// 
    [Serializable]
    public class SplitSetValidation<TModel> where TModel : class
    {

        /// <summary>
        ///   Gets the group labels assigned to each of the data samples.
        /// </summary>
        /// 
        public int[] Indices { get; private set; }

        /// <summary>
        ///   Gets the desired proportion of cases in
        ///   the training set in comparison to the
        ///   testing set.
        /// </summary>
        /// 
        public double Proportion { get; private set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the prevalence of
        ///   an output label should be balanced between training and
        ///   testing sets.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if this instance is stratified; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsStratified { get; private set; }

        /// <summary>
        ///   Gets the indices of elements in the validation set.
        /// </summary>
        /// 
        public int[] ValidationSet { get; private set; }

        /// <summary>
        ///   Gets the indices of elements in the training set.
        /// </summary>
        /// 
        public int[] TrainingSet { get; private set; }


        [NonSerialized]
        SplitValidationFittingFunction<TModel> fitting;

        [NonSerialized]
        SplitValidationEvaluateFunction<TModel> estimation;

        /// <summary>
        ///   Get or sets the model fitting function.
        /// </summary>
        /// 
        public SplitValidationFittingFunction<TModel> Fitting
        {
            get { return fitting; }
            set { fitting = value; }
        }

        /// <summary>
        ///   Gets or sets the performance estimation function.
        /// </summary>
        /// 
        public SplitValidationEvaluateFunction<TModel> Evaluation
        {
            get { return estimation; }
            set { estimation = value; }
        }

        /// <summary>
        ///   Creates a new split-set validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// <param name="proportion">The desired proportion of samples in the training
        /// set in comparison with the testing set.</param>
        /// 
        public SplitSetValidation(int size, double proportion)
        {
            this.Proportion = proportion;
            this.IsStratified = false;
            this.Indices = Accord.Statistics.Tools.RandomGroups(size, proportion);

            this.ValidationSet = Indices.Find(x => x == 0);
            this.TrainingSet = Indices.Find(x => x == 1);
        }

        /// <summary>
        ///   Creates a new split-set validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The total number of available samples.</param>
        /// <param name="proportion">The desired proportion of samples in the training
        /// set in comparison with the testing set.</param>
        /// <param name="outputs">The output labels to be balanced between the sets.</param>
        /// 
        public SplitSetValidation(int size, double proportion, int[] outputs)
        {
            if (outputs.Length != size)
                throw new DimensionMismatchException("outputs");

            this.IsStratified = true;
            this.Proportion = proportion;

            int negative = outputs.Min();
            int positive = outputs.Max();

            var negativeIndices = outputs.Find(x => x == negative).ToList();
            var positiveIndices = outputs.Find(x => x == positive).ToList();

            int positiveCount = positiveIndices.Count;
            int negativeCount = negativeIndices.Count;

            int firstGroupPositives = (int)((positiveCount / 2.0) * proportion);
            int firstGroupNegatives = (int)((negativeCount / 2.0) * proportion);

            List<int> training = new List<int>();
            List<int> testing = new List<int>();

            // Put positives and negatives into training
            for (int j = 0; j < firstGroupNegatives; j++)
            {
                training.Add(negativeIndices[0]);
                negativeIndices.RemoveAt(0);
            }

            for (int j = 0; j < firstGroupPositives; j++)
            {
                training.Add(positiveIndices[0]);
                positiveIndices.RemoveAt(0);
            }

            testing.AddRange(negativeIndices);
            testing.AddRange(positiveIndices);
            
            this.TrainingSet = training.ToArray();
            this.ValidationSet = testing.ToArray();
        }

        /// <summary>
        ///   Computes the split-set validation algorithm.
        /// </summary>
        /// 
        public SplitSetResult<TModel> Compute()
        {
            if (Fitting == null)
                throw new InvalidOperationException("Fitting function must have been previously defined.");

            // Fit and evaluate the model
            SplitSetStatistics<TModel> training = fitting(TrainingSet);
            SplitSetStatistics<TModel> testing = estimation(ValidationSet, training.Model);

            // Return validation statistics
            return new SplitSetResult<TModel>(this, training, testing);
        }

    }
     
}
