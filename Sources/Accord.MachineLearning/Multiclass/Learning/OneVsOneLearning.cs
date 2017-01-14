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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Accord.Math;
    using System.Threading;
    using Accord.MachineLearning;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;


    /// <summary>
    ///   Base learning algorithm for <see cref="OneVsOne{TBinary, TBinary}"/> multi-class classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TBinary">The type for the inner binary classifiers used in the one-vs-one approach.</typeparam>
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// 
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.MulticlassSupportVectorLearning"/>
    /// 
    public abstract class OneVsOneLearning<TBinary, TModel> :
        OneVsOneLearning<double[], TBinary, TModel>
        where TModel : OneVsOne<TBinary>
        where TBinary : class, IBinaryClassifier<double[]>, ICloneable
    {
    }

    /// <summary>
    ///   Base learning algorithm for <see cref="OneVsOne{TBinary, TBinary}"/> multi-class classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type for the samples handled by the classifier. Default is double[].</typeparam>
    /// <typeparam name="TBinary">The type for the inner binary classifiers used in the one-vs-one approach.</typeparam>
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// 
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.MulticlassSupportVectorLearning"/>
    /// 
    public abstract class OneVsOneLearning<TInput, TBinary, TModel> : ParallelLearningBase,
        ISupervisedLearning<TModel, TInput, int>
        where TModel : OneVsOne<TBinary, TInput>
        where TBinary : class, IBinaryClassifier<TInput>, ICloneable
    {

        /// <summary>
        ///   Gets or sets the model being learned.
        /// </summary>
        /// 
        public TModel Model { get; set; }

        /// <summary>
        ///   Gets or sets a function that takes a set of parameters and creates
        ///   a learning algorithm for learning each of the binary inner classifiers
        ///   needed by the one-vs-one classification strategy.
        /// </summary>
        /// 
        public Func<InnerParameters<TBinary, TInput>, ISupervisedLearning<TBinary, TInput, bool>> Learner { get; set; }


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

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(int inputs, int classes);


        // Overloads to simplify call from F#
        /// <summary>
        ///   Sets a callback function that takes a set of parameters and creates
        ///   a learning algorithm for learning each of the binary inner classifiers
        ///   needed by the one-vs-rest classification strategy. Calling this method
        ///   sets the <see cref="Learner"/> property.
        /// </summary>
        /// 
        public void Configure<T, TResult>(Func<T, TResult> learner)
            where T : InnerParameters<TBinary, TInput>
            where TResult : ISupervisedLearning<TBinary, TInput, bool>
        {
            Learner = (InnerParameters<TBinary, TInput> p) => learner((T)p);
        }

        /// <summary>
        ///   Sets a callback function that takes a set of parameters and creates
        ///   a learning algorithm for learning each of the binary inner classifiers
        ///   needed by the one-vs-rest classification strategy. Calling this method
        ///   sets the <see cref="Learner"/> property.
        /// </summary>
        /// 
        public void Configure<TResult>(Func<TResult> learner)
            where TResult : ISupervisedLearning<TBinary, TInput, bool>
        {
            Learner = (p) => learner();
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
        public TModel Learn(TInput[] x, int[] y, double[] weights = null)
        {
            if (Model == null)
            {
                int numberOfInputs = SupportVectorLearningHelper.GetNumberOfInputs(new Linear(), x);
                int numberOfClasses = y.DistinctCount();
                Model = Create(numberOfInputs, numberOfClasses);
            }

            int classes = Model.NumberOfOutputs;
            int total = (classes * (classes - 1)) / 2;
            int progress = 0;

            var pairs = new Tuple<int, int>[total];
            for (int i = 0, k = 0; i < classes; i++)
                for (int j = 0; j < i; j++, k++)
                    pairs[k] = Tuple.Create(i, j);


            // Save exceptions but process all machines
            var exceptions = new ConcurrentBag<Exception>();

#if DEBUG
            ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                // For each class k
                for(int k = 0; k < total; k++)
                    TrainBinaryMachine(x, y, weights, k, total, ref progress, pairs, exceptions);
            }
            else
            {
                // For each class k
                Parallel.For(0, total, ParallelOptions, (int k) =>
                    TrainBinaryMachine(x, y, weights, k, total, ref progress, pairs, exceptions));
            }


            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more exceptions were thrown when teaching "
                    + "the machines. Please check the InnerException property of this AggregateException "
                    + "to discover what exactly caused this error.", exceptions);
            }

            return Model;
        }

        private void TrainBinaryMachine(TInput[] x, int[] y, double[] weights, int k, int total, ref int progress, Tuple<int, int>[] pairs, ConcurrentBag<Exception> exceptions)
        {
            if (ParallelOptions.CancellationToken.IsCancellationRequested)
                return;

            int i = pairs[k].Item1;
            int j = pairs[k].Item2;

            // We will start the binary sub-problem
            var args = new SubproblemEventArgs(i, j);
            OnSubproblemStarted(args);

            // Retrieve the associated machine
            TBinary model = Model.GetClassifierForClassPair(classA: i, classB: j);

            // Retrieve the associated classes
            int[] idx = y.Find(y_i => y_i == i || y_i == j);

            if (idx.Length == 0)
            {
                System.Diagnostics.Trace.TraceWarning("Class pair ({0}, {1}) does not have any examples.", i, j);
            }

            TInput[] subx = x.Get(idx);
            bool[] suby = y.Get(idx).Apply(y_i => y_i == i);

            double[] subw = null;
            if (weights != null)
                subw = weights.Get(idx);

            try
            {
                // Configure the machine on the two-class problem.
                var subproblem = Learner(new InnerParameters<TBinary, TInput>(
                    inputs: subx,
                    outputs: suby,
                    pair: new ClassPair(i, j),
                    model: model
                ));

                if (subproblem != null)
                {
                    // TODO: This check only exists to provide support to previous way of 
                    // using the library and should be removed after a few releases. In the
                    // current way (without using any Obsolete methods), subproblem should never be null.
                    subproblem.Token = ParallelOptions.CancellationToken;
                    Model[i, j] = subproblem.Learn(subx, suby, subw);
                }
            }
            catch (Exception ex)
            {
                ex.Data["pair"] = pairs[k];
                exceptions.Add(ex);
            }

            // Update and report progress
            args.Progress = Interlocked.Increment(ref progress);
            args.Maximum = total;

            OnSubproblemFinished(args);
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
