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
#if !MONO
    using System;
    using System.Linq;
    using Accord.Math;
    using System.Collections.Generic;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Naïve Bayes learning algorithm.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   For basic examples on how to learn a Naive Bayes algorithm, please see
    ///   <see cref="NaiveBayes"/> page. The following examples show how to set
    ///   more specialized learning settings for Normal (Gaussian) models.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_learn" />
    /// </example>
    ///
    [Serializable]
    public class NaiveBayesLearning<TDistribution> :
        NaiveBayesLearningBase<NaiveBayes<TDistribution>,
            TDistribution, double, IndependentOptions>
        where TDistribution : IFittableDistribution<double>,
                              IUnivariateDistribution<double>,
                              IUnivariateDistribution
    {

        /// <summary>
        /// Creates an instance of the model to be learned.
        /// </summary>
        protected override NaiveBayes<TDistribution> Create(double[][] x, int y)
        {
            int inputs = x[0].Length;
            return new NaiveBayes<TDistribution>(inputs: inputs, classes: y, initial: Distribution);
        }
    }

    /// <summary>
    ///   Naïve Bayes learning algorithm.
    /// </summary>
    ///
    /// <example>
    /// <para>
    ///   For basic examples on how to learn a Naive Bayes algorithm, please see
    ///   <see cref="NaiveBayes"/> page. The following examples show how to set
    ///   more specialized learning settings for Normal (Gaussian) models.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_learn_options" />
    /// </example>
    ///
    [Serializable]
    public class NaiveBayesLearning<TDistribution, TOptions> :
        NaiveBayesLearningBase<NaiveBayes<TDistribution>,
            TDistribution, double, IndependentOptions<TOptions>>
        where TDistribution : IFittableDistribution<double>,
                              IUnivariateDistribution<double>,
                              IUnivariateDistribution
        where TOptions : class, IFittingOptions, new()
    {
        /// <summary>
        /// Creates an instance of the model to be learned.
        /// </summary>
        protected override NaiveBayes<TDistribution> Create(double[][] x, int y)
        {
            int inputs = x[0].Length;
            return new NaiveBayes<TDistribution>(inputs: inputs, classes: y, initial: Distribution);
        }
    }

    /// <summary>
    ///   Naïve Bayes learning algorithm.
    /// </summary>
    ///
    /// <example>
    /// <para>
    ///   For basic examples on how to learn a Naive Bayes algorithm, please see
    ///   <see cref="NaiveBayes"/> page. The following examples show how to set
    ///   more specialized learning settings for Normal (Gaussian) models.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_learn_options" />
    /// </example>
    ///
    [Serializable]
    public class NaiveBayesLearning<TDistribution, TOptions, TInput> :
        NaiveBayesLearningBase<NaiveBayes<TDistribution, TInput>,
                               TDistribution, TInput, IndependentOptions<TOptions>>
        where TDistribution : IFittableDistribution<TInput>,
                              IUnivariateDistribution<TInput>,
                              IUnivariateDistribution
        where TOptions : class, IFittingOptions, new()
    {
        /// <summary>
        /// Creates an instance of the model to be learned.
        /// </summary>
        protected override NaiveBayes<TDistribution, TInput> Create(TInput[][] x, int y)
        {
            int inputs = x[0].Length;
            return new NaiveBayes<TDistribution, TInput>(inputs: inputs, classes: y, initial: Distribution);
        }
    }
#endif
}