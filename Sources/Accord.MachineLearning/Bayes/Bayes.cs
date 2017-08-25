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
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Bayes decision algorithm (not naive).
    /// </summary>
    /// 
    /// <typeparam name="TDistribution">The type for the distributions used to model each class.</typeparam>
    /// <typeparam name="TInput">The type for the samples modeled by the distributions.</typeparam>
    ///
    /// <seealso cref="NaiveBayes"/>
    /// <seealso cref="NaiveBayes{TDistribution}"/>
    /// <seealso cref="NaiveBayes{TDistribution, TInput}"/>
    /// 
    [Serializable]
    public class Bayes<TDistribution, TInput> : MulticlassLikelihoodClassifierBase<TInput>
        where TDistribution : IFittableDistribution<TInput>, 
        IMultivariateDistribution<TInput>
    {

        private TDistribution[] distributions;
        private double[] priors;


        /// <summary>
        ///   Gets the probability distributions for each class and input.
        /// </summary>
        /// 
        /// <value>A TDistribution[,] array in with each row corresponds to a 
        /// class, each column corresponds to an input variable. Each element
        /// of this double[,] array is a probability distribution modeling
        /// the occurrence of the input variable in the corresponding class.</value>
        /// 
        public TDistribution[] Distributions
        {
            get { return distributions; }
            set
            {
                if (!value.DimensionEquals(distributions))
                    throw new DimensionMismatchException("value");
                distributions = value;
            }
        }

        /// <summary>
        ///   Gets the prior beliefs for each class.
        /// </summary>
        /// 
        public virtual double[] Priors
        {
            get { return priors; }
            set
            {
                if (!value.DimensionEquals(priors))
                    throw new DimensionMismatchException("value");
                priors = value;
            }
        }


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
        public Bayes(int classes, int inputs, Func<TDistribution> initializer)
        {
            init(classes, inputs);
            for (int i = 0; i < distributions.Length; i++)
                distributions[i] = initializer();
        }

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
        public Bayes(int classes, int inputs, Func<int, TDistribution> initializer)
        {
            init(classes, inputs);
            for (int i = 0; i < distributions.Length; i++)
                distributions[i] = initializer(i);
        }


        private void init(int classes, int inputs)
        {
            if (classes < 2)
                throw new ArgumentOutOfRangeException("classes", "The number of classses should be higher than 1 (currently detected: {0}).".Format(classes));

            if (inputs <= 0)
                throw new ArgumentOutOfRangeException("inputs", "The number of inputs should be higher than 0 (currently detected: {0}).".Format(inputs));

            this.NumberOfOutputs = classes;
            this.NumberOfClasses = classes;
            this.NumberOfInputs = inputs;

            this.distributions = new TDistribution[classes];

            this.priors = new double[NumberOfOutputs];
            for (int i = 0; i < priors.Length; i++)
                priors[i] = 1.0 / priors.Length;
        }




        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public override double LogLikelihood(TInput input, int classIndex)
        {
            double log = distributions[classIndex].LogProbabilityFunction(input);
            return Math.Log(priors[classIndex]) + log;
        }

    }
}
