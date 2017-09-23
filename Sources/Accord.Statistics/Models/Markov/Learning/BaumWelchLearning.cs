// Accord Statistics Library
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

namespace Accord.Statistics.Models.Markov.Learning
{
#pragma warning disable 612, 618

    using System;
    using Accord.Math;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Baum-Welch learning algorithm for <see cref="HiddenMarkovModel">
    ///   discrete-density Hidden Markov Models</see>.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The Baum-Welch algorithm is an <see cref="IUnsupervisedLearning">unsupervised algorithm</see>
    ///   used to learn a single hidden Markov model object from a set of observation sequences. It works
    ///   by using a variant of the <see cref="Mixture{T}.Fit(double[], double[], MixtureOptions)">
    ///   Expectation-Maximization</see> algorithm to search a set of model parameters (i.e. the matrix
    ///   of <see cref="IHiddenMarkovModel.Transitions">transition probabilities A</see>, the matrix 
    ///   of <see cref="HiddenMarkovModel.Emissions">emission probabilities B</see>, and the
    ///   <see cref="IHiddenMarkovModel.Probabilities">initial probability vector π</see>) that 
    ///   would result in a model having a high likelihood of being able 
    ///   to <see cref="HiddenMarkovModel{TDistribution, TObservation}.Generate(int)">generate</see> a set of training 
    ///   sequences given to this algorithm.</para>
    ///   
    ///   <para>
    ///   For increased accuracy, this class performs all computations using log-probabilities.</para>
    ///     
    ///   <para>
    ///   For a more thorough explanation on <see cref="HiddenMarkovModel">hidden Markov models</see>
    ///   with practical examples on gesture recognition, please see 
    ///   <a href="http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko">
    ///   Sequence Classifiers in C#, Part I: Hidden Markov Models</a> [1].</para>
    ///     
    /// <para>
    ///   [1]: <a href="http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko"> 
    ///           http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko </a>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModelTest.cs" region="doc_learn"/>
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="HiddenMarkovModel{TDistribution}"/>
    /// <seealso cref="BaumWelchLearning{TDistribution}"/>
    /// 
    public class BaumWelchLearning : BaseBaumWelchLearningOptions<HiddenMarkovModel, GeneralDiscreteDistribution, int, GeneralDiscreteOptions>
    {

        /// <summary>
        ///   Gets or sets the number of symbols that should be used whenever 
        ///   this learning algorithm needs to create a new model. This property
        ///   must be set before learning.
        /// </summary>
        /// 
        /// <value>The number of symbols.</value>
        /// 
        public int NumberOfSymbols { get; set; }

        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaumWelchLearning()
        {
        }

        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaumWelchLearning(HiddenMarkovModel model)
            : base(model)
        {
            NumberOfSymbols = model.NumberOfSymbols;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(int[] observations)
        {
            Learn(new[] { observations });
            return LogLikelihood;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(int[][] observations)
        {
            Learn(observations);
            return LogLikelihood;
        }

        /// <summary>
        ///   Creates a Baum-Welch with default configurations for
        ///   hidden Markov models with normal mixture densities.
        /// </summary>
        /// 
        public static BaumWelchLearning<Mixture<NormalDistribution>> FromMixtureModel(
            HiddenMarkovModel<Mixture<NormalDistribution>> model, NormalOptions options)
        {
            MixtureOptions mixOptions = new MixtureOptions()
            {
                Iterations = 1,
                InnerOptions = options,
                //ParallelOptions = ParallelOptions, // TODO:
            };

            return new BaumWelchLearning<Mixture<NormalDistribution>>(model)
            {
                FittingOptions = mixOptions
            };
        }

        /// <summary>
        ///   Creates a Baum-Welch with default configurations for
        ///   hidden Markov models with normal mixture densities.
        /// </summary>
        /// 
        public static BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>> FromMixtureModel(
            HiddenMarkovModel<MultivariateMixture<MultivariateNormalDistribution>> model, NormalOptions options)
        {
            MixtureOptions mixOptions = new MixtureOptions()
            {
                Iterations = 1,
                InnerOptions = options
            };

            return new BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>>(model)
            {
                FittingOptions = mixOptions
            };
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected override HiddenMarkovModel Create(int[][] x)
        {
            if (Topology == null)
                throw new InvalidOperationException("Please set the Topology property before trying to learn a new hidden Markov model.");

            this.NumberOfSymbols = x.Max() + 1;

            return new HiddenMarkovModel(Topology, NumberOfSymbols);
        }

        // TODO: Uncomment the following lines
        ///// <summary>
        /////   Creates a Baum-Welch with default configurations for
        /////   hidden Markov models with normal mixture densities.
        ///// </summary>
        ///// 
        //public static BaumWelchLearning<Mixture<NormalDistribution>> FromMixtureModel(
        //    HiddenMarkovModel<Mixture<NormalDistribution, double>> model, NormalOptions options)
        //{
        //    MixtureOptions mixOptions = new MixtureOptions()
        //    {
        //        Iterations = 1,
        //        InnerOptions = options,
        //        //ParallelOptions = ParallelOptions, // TODO:
        //    };

        //    return new BaumWelchLearning<Mixture<NormalDistribution>>(model)
        //    {
        //        FittingOptions = mixOptions
        //    };
        //}

        ///// <summary>
        /////   Creates a Baum-Welch with default configurations for
        /////   hidden Markov models with normal mixture densities.
        ///// </summary>
        ///// 
        //public static BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution, double[]>> FromMixtureModel(
        //    HiddenMarkovModel<MultivariateMixture<MultivariateNormalDistribution>> model, NormalOptions options)
        //{
        //    MixtureOptions mixOptions = new MixtureOptions()
        //    {
        //        Iterations = 1,
        //        InnerOptions = options
        //    };

        //    return new BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>>(model)
        //    {
        //        FittingOptions = mixOptions
        //    };
        //}
    }
}
