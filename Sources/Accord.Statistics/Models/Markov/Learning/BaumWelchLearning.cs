// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    ///   <code>
    ///   // We will try to create a Hidden Markov Model which
    ///   //  can detect if a given sequence starts with a zero
    ///   //  and has any number of ones after that.
    ///   int[][] sequences = new int[][] 
    ///   {
    ///       new int[] { 0,1,1,1,1,0,1,1,1,1 },
    ///       new int[] { 0,1,1,1,0,1,1,1,1,1 },
    ///       new int[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new int[] { 0,1,1,1,1,1         },
    ///       new int[] { 0,1,1,1,1,1,1       },
    ///       new int[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new int[] { 0,1,1,1,1,1,1,1,1,1 },
    ///   };
    ///   
    ///   // Creates a new Hidden Markov Model with 3 states for
    ///   //  an output alphabet of two characters (zero and one)
    ///   HiddenMarkovModel hmm = new HiddenMarkovModel(3, 2);
    ///   
    ///   // Try to fit the model to the data until the difference in
    ///   //  the average log-likelihood changes only by as little as 0.0001
    ///   var teacher = new BaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };
    ///   
    ///   double ll = teacher.Run(sequences);
    ///   
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double l1 = Math.Exp(hmm.Evaluate(new int[] { 0, 1 }));       // 0.999
    ///   double l2 = Math.Exp(hmm.Evaluate(new int[] { 0, 1, 1, 1 })); // 0.916
    ///   
    ///   // Sequences which do not start with zero have much lesser probability.
    ///   double l3 = Math.Exp(hmm.Evaluate(new int[] { 1, 1 }));       // 0.000
    ///   double l4 = Math.Exp(hmm.Evaluate(new int[] { 1, 0, 0, 0 })); // 0.000
    ///   
    ///   // Sequences which contains few errors have higher probability
    ///   //  than the ones which do not start with zero. This shows some
    ///   //  of the temporal elasticity and error tolerance of the HMMs.
    ///   double l5 = Math.Exp(hmm.Evaluate(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 })); // 0.034
    ///   double l6 = Math.Exp(hmm.Evaluate(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 })); // 0.034
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="HiddenMarkovModel{TDistribution}"/>
    /// <seealso cref="BaumWelchLearning{TDistribution}"/>
    /// 
    public class BaumWelchLearning : BaseBaumWelchLearning<HiddenMarkovModel, GeneralDiscreteDistribution, int, GeneralDiscreteOptions>
    {

        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaumWelchLearning(HiddenMarkovModel model)
            : base(model)
        {
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
                InnerOptions = options
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
    }
}
