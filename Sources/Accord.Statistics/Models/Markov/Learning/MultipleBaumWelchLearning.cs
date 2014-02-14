// Accord Statistics Library
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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using System.Threading.Tasks;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Multiple-trials Baum-Welch learning.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to perform multiple attempts on <see cref="BaumWelchLearning">
    ///   Baum-Welch learning</see> with multiple different initialization points. It can also
    ///   be used as a replacement inside <see cref="HiddenMarkovClassifierLearning"/> algorithms
    ///   wherever a standard <see cref="BaumWelchLearning"/> class would be used.
    /// </remarks>
    /// 
    public class MultipleBaumWelchLearning : IUnsupervisedLearning, IConvergenceLearning,
        IUnsupervisedLearning<int[]>
    {

        private HiddenMarkovModel template;
        private ITopology topology;


#if !NET35
        private double bestLikelihood;

        /// <summary>
        ///   inner class to hold information about a inner model.
        /// </summary>
        private class Candidate
        {
            public HiddenMarkovModel Model { get; set; }
            public double LogLikelihood { get; set; }
        }
#endif


        /// <summary>
        ///   Gets the template model, used to create all other instances.
        /// </summary>
        /// 
        public HiddenMarkovModel Model
        {
            get { return template; }
        }

        /// <summary>
        ///   Gets the topology used on the inner models.
        /// </summary>
        /// 
        public ITopology Topology
        {
            get { return topology; }
        }

        /// <summary>
        ///   Gets or sets how many trials should be attempted
        ///   before the model with highest log-likelihood is
        ///   selected as the best model found.
        /// </summary>
        /// 
        public int Trials { get; set; }

        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the likelihood convergence limit L between two iterations of the algorithm. The
        ///   algorithm will stop when the change in the likelihood for two consecutive iterations
        ///   has not changed by more than L percent of the likelihood. If left as zero, the
        ///   algorithm will ignore this parameter and iterate over a number of fixed iterations
        ///   specified by the previous parameter.
        /// </remarks>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the maximum number of iterations to be performed by the learning algorithm. If
        ///   specified as zero, the algorithm will learn until convergence of the model average
        ///   likelihood respecting the desired limit.
        /// </remarks>
        /// 
        public int Iterations { get; set; }


        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        /// <param name="attempts">The number of inner models to be learned.</param>
        /// <param name="model">The template model used to create all subsequent inner models.</param>
        /// <param name="topology">The topology to be used by the inner models. To be useful,
        ///   this needs to be a topology configured to create random initialization matrices.</param>
        /// 
        public MultipleBaumWelchLearning(HiddenMarkovModel model, ITopology topology, int attempts)
        {
            this.template = model;
            this.topology = topology;
            this.Trials = attempts;
        }


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// 
        public double Run(params int[][] observations)
        {
#if NET35
            throw new NotSupportedException("This class requires .NET 4.0 or newer.");
        }
#else

            // MarkovHelperMethods.checkArgs(observations, template.Symbols);

            // The following is a parallel map-reduce algorithm. It starts by
            // creating a few candidate models using the same topology as the
            // template model initially stored in the HiddenMarkovClassifier.

            // If the topology parameter is set to be random, this should create
            // multiple "clones" of the original template with varying parameter
            // initializations. 

            // After initialization, those models are assumed as the best models
            // ever created by the local thread. Then, at each iteration, another
            // model is created and its performance is evaluated against the local
            // thread best model. If the new model has better performance than the
            // older model, it is assumed as the next best model for the thread.
            // otherwise, it is discarded and the method goes to the next iteration.

            // When all threads finish processing, the final best solution for each
            // thread will be tested and compared in the reduce step. The best model
            // overall will be copied over the template variable, which is the model
            // we initially started with.


            // PS: A random topology can be created with something like:
            //
            //   var topology = new Ergodic(states: 4)
            //   {
            //       Random = true
            //   }
            //

            int symbols = template.Symbols;

            bestLikelihood = 0;

            Parallel.For(0, Trials,

                // Start
                () => new Candidate()
                {
                    Model = new HiddenMarkovModel(topology, symbols)
                },

                // Map
                (index, state, partial) =>
                {
                    // Create a new candidate hidden Markov model
                    var hmm = new HiddenMarkovModel(topology, symbols);

                    // And estimate it using the learning algorithm
                    BaumWelchLearning teacher = new BaumWelchLearning(hmm)
                    {
                        Tolerance = Tolerance,
                        Iterations = Iterations
                    };

                    double logLikelihood = teacher.Run(observations);

                    // If the model is better than the previous
                    if (logLikelihood > partial.LogLikelihood)
                    {
                        // We will keep it
                        partial.Model = hmm;
                        partial.LogLikelihood = logLikelihood;
                    }

                    return partial;
                },

                // Reduce
                (partial) =>
                {
                    if (partial.LogLikelihood > bestLikelihood)
                    {
                        lock (template)
                        {
                            copy(from: partial.Model, to: template);
                            bestLikelihood = partial.LogLikelihood;
                        }
                    }
                }
            );

            return bestLikelihood;
        }
        
        private static void copy(HiddenMarkovModel from, HiddenMarkovModel to)
        {
            Array.Copy(from.Transitions, to.Transitions, to.Transitions.Length);
            Array.Copy(from.Emissions, to.Emissions, to.Emissions.Length);
            Array.Copy(from.Probabilities, to.Probabilities, to.Probabilities.Length);
        }
#endif


        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="observations">The observations.</param>
        /// 
        double IUnsupervisedLearning.Run(Array[] observations)
        {
            return Run(observations as int[][]);
        }

    }
}
