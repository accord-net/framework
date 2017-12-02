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
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Split-Set Validation (with support for stratification and default loss function for classification).
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the machine learning model.</typeparam>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\SplitSetTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\SplitSetTest.cs" region="doc_learn_multiclass" />
    /// </example>
    /// 
    /// <seealso cref="Bootstrap{TModel, TInput, TOutput}"/>
    /// <seealso cref="CrossValidation{TModel, TInput}"/>
    /// <seealso cref="SplitSetValidation{TModel, TInput, TOutput}"/>
    /// 
    public class SplitSetValidation<TModel, TInput>
        : SplitSetValidation<TModel, TInput, int>
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
        /// Initializes a new instance of the <see cref="SplitSetValidation{TModel, TInput}"/> class.
        /// </summary>
        public SplitSetValidation()
        {
            this.Loss = (expected, actual, r) =>
            {
                return new GeneralConfusionMatrix(r.Model.NumberOfClasses, expected, actual).Error;
            };
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
        protected override int[] CreateValidationSplits(TInput[] x, int[] y)
        {
            if (Stratify)
                return Classes.Random(y, TrainingSetProportion);
            return base.CreateValidationSplits(x, y);
        }

       
    }
}
