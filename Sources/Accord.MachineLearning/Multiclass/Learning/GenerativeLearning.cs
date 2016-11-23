// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using Accord.Math;
    using Accord.MachineLearning;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;
    using Accord.Statistics;

    /// <summary>
    ///   Base learning algorithm for <see cref="Generative{TGenerative, TModel}"/> multi-label classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TGenerative">The type for the inner models used in the one-vs-rest approach.</typeparam>
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// 
    public abstract class GenerativeLearning<TInner, TModel> :
        OneVsRestLearning<double[], TInner, TModel>,
        ISupervisedLearning<TModel, double[], int>
        where TModel : Generative<TInner, double[]>
        where TInner : class, IGenerative<double[]>
    {
    }

    /// <summary>
    ///   Base learning algorithm for <see cref="OneVsRest{TBinary, TModel}"/> multi-class classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type for the samples handled by the classifier. Default is double[].</typeparam>
    /// <typeparam name="TBinary">The type for the inner binary classifiers used in the one-vs-rest approach.</typeparam>
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// 
    public abstract class GenerativeLearning<TInput, TInner, TModel> :
        ParallelLearningBase,
        ISupervisedLearning<TModel, TInput, int>,
        ISupervisedLearning<TModel, TInput, int[]>,
        ISupervisedLearning<TModel, TInput, bool[]>
        where TModel : Generative<TInner, TInput>
        where TInner : IGenerative<TInner, TInput>
    {

        /// <summary>
        ///   Gets or sets the model being learned.
        /// </summary>
        /// 
        public TModel Model { get; set; }

        /// <summary>
        ///   Gets or sets a function that takes a set of parameters and creates
        ///   a learning algorithm for learning each of the binary inner classifiers
        ///   needed by the one-vs-rest classification strategy.
        /// </summary>
        /// 
        public Func<int, IUnsupervisedLearning<TInner, TInput, double>> Learner { get; set; }


        /// <summary>
        ///   Occurs when the learning of a subproblem has started.
        /// </summary>
        /// 
        public event EventHandler<SubproblemEventArgs> SubproblemStarted;

        /// <summary>
        ///   Occurs when the learning of a subproblem has finished.
        /// </summary>
        /// 
        public event EventHandler<SubproblemEventArgs> SubproblemFinished;


        protected GenerativeLearning()
        {
        }


        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(int inputs, int classes);

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, int[] y, double[] weights = null)
        {
            return Learn(x, Jagged.OneHot<bool>(y), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, int[][] y, double[] weights = null)
        {
            return Learn(x, Classes.Decide(y), weights);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public virtual TModel Learn(TInput[] x, bool[][] y, double[] weights = null)
        {
            if (Model == null)
            {
                throw new NotSupportedException("");
            }

            int total = Model.NumberOfOutputs;
            int progress = 0;

            // Save exceptions but process all machines
            var exceptions = new ConcurrentBag<Exception>();

            // For each class i
            Parallel.For(0, Model.Models.Length, ParallelOptions, (int i) =>
            {
                if (ParallelOptions.CancellationToken.IsCancellationRequested)
                    return;

                // We will start the binary sub-problem
                var args = new SubproblemEventArgs(i, -i);
                OnSubproblemStarted(args);

                // Retrieve the associated machine
                TInner model = Model.Models[i];

                // Select the input/output set corresponding
                //  to the model's specialization class
                TInput[] observations = x.Get(y.Find(y_j => y_j[i]));


                // Train the machine on the two-class problem.
                try
                {
                    // Configure the machine on the two-class problem.
                    var subproblem = Learner(i);

                    if (subproblem != null)
                    {
                        // TODO: This check only exists to provide support to previous way of 
                        // using the library and should be removed after a few releases. In the
                        // current way (without using any Obsolete methods), subproblem should never be null.
                        Model[i] = subproblem.Learn(x, weights);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                // Update and report progress
                args.Progress = Interlocked.Increment(ref progress);
                args.Maximum = total;

                OnSubproblemFinished(args);
            });

            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more exceptions were thrown when teaching "
                    + "the machines. Please check the InnerException property of this AggregateException "
                    + "to discover what exactly caused this error.", exceptions);
            }

            return Model;
        }

        /// <summary>
        ///   Raises the <see cref="E:SubproblemFinished"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="SubproblemEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnSubproblemFinished(SubproblemEventArgs args)
        {
            if (SubproblemFinished != null)
                SubproblemFinished(this, args);
        }

        /// <summary>
        ///   Raises the <see cref="E:SubproblemStarted"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="SubproblemEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnSubproblemStarted(SubproblemEventArgs args)
        {
            if (SubproblemStarted != null)
                SubproblemStarted(this, args);
        }
    }
}
