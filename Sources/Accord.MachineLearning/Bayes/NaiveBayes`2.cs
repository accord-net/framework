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
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Multivariate;
    using System;
    using Accord.Compat;

#if !MONO
    /// <summary>
    ///   Naïve Bayes Classifier for arbitrary distributions of arbitrary elements.
    /// </summary>
    /// 
    /// <seealso cref="NaiveBayes"/>
    /// <seealso cref="NaiveBayes{TDistribution}"/>
    /// <seealso cref="NaiveBayesLearning{TDistribution, TInput}"/>
    /// 
    [Serializable]
    public class NaiveBayes<TDistribution, TInput> : 
        Bayes<Independent<TDistribution, TInput>, TInput[]>
        where TDistribution : IFittableDistribution<TInput>, 
                              IUnivariateDistribution<TInput>,
                              IUnivariateDistribution
    {

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initializer">
        ///   An initialization function used to create the distribution functions for
        ///   each class. Those will be available in the <see cref="Bayes{TDistribution, TInput}.Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, Func<int, TDistribution> initializer)
            : base(classes, inputs, (j) => new Independent<TDistribution, TInput>(inputs, initializer))
        {
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initialization function used to create the distribution functions for
        ///   each class. Those will be available in the <see cref="Bayes{TDistribution, TInput}.Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, Func<int, int, TDistribution> initial)
            : base(classes, inputs, (classIndex) => new Independent<TDistribution, TInput>(inputs, (variableIndex) => (TDistribution)initial(classIndex, variableIndex).Clone()))
        {
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Bayes{TDistribution, TInput}.Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution initial)
            : base(classes, inputs, () => new Independent<TDistribution, TInput>(inputs, (TDistribution)initial.Clone()))
        {
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Bayes{TDistribution, TInput}.Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[] initial)
            : base(classes, inputs, () =>  new Independent<TDistribution, TInput>(inputs, (j) => (TDistribution)initial[j].Clone()))
        {
        } 

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Bayes{TDistribution, TInput}.Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[][] initial)
            : base(classes, inputs, (i) =>  new Independent<TDistribution, TInput>(inputs, (j) =>  (TDistribution)initial[i][j].Clone()))
        {
        } 
    }
#else
    /// <summary>
    ///   This class is currently not supported in Mono due to
    ///   a bug in the Mono compiler.
    /// </summary>
    /// 
    [Obsolete("This class is not supported in Mono.")]
    public class NaiveBayes<T1, T2>
    {

    }
#endif
}
