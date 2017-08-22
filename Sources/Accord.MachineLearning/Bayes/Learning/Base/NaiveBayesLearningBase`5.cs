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

namespace Accord.MachineLearning.Bayes
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;

#if !MONO
    /// <summary>
    ///   Base class for Naive Bayes learning algorithms.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the Naive Bayes model to be learned.</typeparam>
    /// <typeparam name="TDistribution">The univariate distribution to be used as components in the Naive Bayes distribution.</typeparam>
    /// <typeparam name="TInput">The type for the samples modeled by the distribution.</typeparam>
    /// <typeparam name="TOptions">The fitting options for the independent distribution.</typeparam>
    /// <typeparam name="TInnerOptions">The individual fitting options for the component distributions.</typeparam>
    /// 
    public abstract class NaiveBayesLearningBase<TModel, TDistribution, TInput, TOptions, TInnerOptions> :
        NaiveBayesLearningBase<TModel, TDistribution, TInput, TOptions>
        where TDistribution : IFittableDistribution<TInput, TInnerOptions>,
                              IUnivariateDistribution<TInput>,
                              IUnivariateDistribution
        where TOptions : IndependentOptions<TInnerOptions>, new()
        where TInnerOptions : class, IFittingOptions, new()
        where TModel : NaiveBayes<TDistribution, TInput>
    {

        /// <summary>
        /// Fits one of the distributions in the naive bayes model.
        /// </summary>
        protected override void Fit(int i, TInput[][] values, double[] weights, bool transposed)
        {
            Options.Transposed = transposed;
            Model.Distributions[i].Fit(values, weights, Options);
            this.optimized = true;
        }
    }
#endif
}