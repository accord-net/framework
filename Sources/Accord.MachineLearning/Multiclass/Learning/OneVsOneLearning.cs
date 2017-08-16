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
    using System.Collections.Generic;
    using Accord.Math;
    using System.Threading;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using System.Diagnostics;
    using Accord.Compat;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base learning algorithm for <see cref="OneVsOne{TBinary, TBinary}"/> multi-class classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TBinary">The type for the inner binary classifiers used in the one-vs-one approach.</typeparam>
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// 
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.MulticlassSupportVectorLearning{TKernel}"/>
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
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.MulticlassSupportVectorLearning{TKernel}"/>
    /// 
    [Serializable]
    public abstract class OneVsOneLearning<TInput, TBinary, TModel> : ParallelLearningBase,
        ISupervisedLearning<TModel, TInput, int>
        where TModel : OneVsOne<TBinary, TInput>
        where TBinary : class, IBinaryClassifier<TInput>, ICloneable
    {
        private ClassPair[] pairs;
        private Func<InnerParameters<TBinary, TInput>, ISupervisedLearning<TBinary, TInput, bool>> learner;
        private ConcurrentDictionary<ClassPair, ISupervisedLearning<TBinary, TInput, bool>> teachers;
        private bool aggregateExceptions = true;

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
        public Func<InnerParameters<TBinary, TInput>, ISupervisedLearning<TBinary, TInput, bool>> Learner
        {
            get { return learner; }
            set
            {
                learner = value;
                teachers = null; // reset the teaching algorithm cache
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the entire training algorithm should stop
        ///   in case an exception has been detected at just one of the inner binary learning
        ///   problems. Default is true (execution will not be stopped).
        /// </summary>
        /// 
        public bool AggregateExceptions
        {
            get { return aggregateExceptions; }
            set { aggregateExceptions = value; }
        }


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
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public TModel Learn(TInput[] x, int[] y, double[] weights = null)
        {
            Accord.MachineLearning.Tools.CheckArgs(x, y, weights, () =>
            {
                if (Model == null)
                {
                    this.teachers = null;
                    int numberOfInputs = Tools.GetNumberOfInputs(x);
                    int numberOfClasses = y.DistinctCount();
                    Model = Create(numberOfInputs, numberOfClasses);
                }

                return Model;
            });

            if (pairs == null)
            {
                this.teachers = null;
                int classes = Model.NumberOfClasses;
                int total = (classes * (classes - 1)) / 2;
                this.pairs = new ClassPair[total];
                for (int i = 0, k = 0; i < classes; i++)
                    for (int j = 0; j < i; j++, k++)
                        this.pairs[k] = new ClassPair(i, j);
            }

            if (teachers == null)
                teachers = new ConcurrentDictionary<ClassPair, ISupervisedLearning<TBinary, TInput, bool>>();

            int progress = 0;

            // Save exceptions but process all machines
            var exceptions = new ConcurrentBag<Exception>();

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                // For each class k
                for (int k = 0; k < pairs.Length; k++)
                    TrainBinaryMachine(x, y, weights, ref progress, pairs[k], exceptions);
            }
            else
            {
                // For each class k
                Parallel.For(0, pairs.Length, ParallelOptions, (int k) =>
                    TrainBinaryMachine(x, y, weights, ref progress, pairs[k], exceptions));
            }


            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more exceptions were thrown when teaching "
                    + "the machines. Please check the InnerException property of this AggregateException "
                    + "to discover what exactly caused this error.", exceptions);
            }

            return Model;
        }

        private void TrainBinaryMachine(TInput[] x, int[] y, double[] weights, ref int progress, ClassPair pair, ConcurrentBag<Exception> exceptions)
        {
            if (ParallelOptions.CancellationToken.IsCancellationRequested)
                return;

            int i = pair.Class1;
            int j = pair.Class2;

            // We will start the binary sub-problem
            var args = new SubproblemEventArgs(i, j);
            OnSubproblemStarted(args);

            // Retrieve the associated machine
            TBinary model = Model.GetClassifierForClassPair(classA: i, classB: j);

            // Retrieve the associated classes
            int[] idx = y.Find(y_i => y_i == i || y_i == j);

            if (idx.Length == 0)
            {
                Trace.TraceWarning("Class pair ({0}, {1}) does not have any examples.", i, j);
            }

            TInput[] subx = x.Get(idx);
            bool[] suby = y.Get(idx).Apply(y_i => y_i == i);

            double[] subw = null;
            if (weights != null)
                subw = weights.Get(idx);

            if (aggregateExceptions)
            {
                try
                {
                    // Train the machine on the two-class problem.
                    TrainBinaryMachine(pair, i, j, model, subx, suby, subw);
                }
                catch (Exception ex)
                {
                    ex.Data["pair"] = pair;
                    exceptions.Add(ex);
                }
            }
            else
            {
                // Train the machine on the two-class problem.
                TrainBinaryMachine(pair, i, j, model, subx, suby, subw);
            }

            // Update and report progress
            args.Progress = Interlocked.Increment(ref progress);
            args.Maximum = pairs.Length;

            OnSubproblemFinished(args);
        }

        private void TrainBinaryMachine(ClassPair pair, int i, int j, TBinary model, TInput[] subx, bool[] suby, double[] subw)
        {
            // Configure the machine on the two-class problem. Check if the learner
            // for this machine has already been created before, and re-use it if it
            // was the case. This is necessary to support mini-batch/online learning.

            ISupervisedLearning<TBinary, TInput, bool> subproblemTeacher;
            if (!teachers.TryGetValue(pair, out subproblemTeacher))
            {
                var p = new InnerParameters<TBinary, TInput>(inputs: subx, outputs: suby, pair: pair, model: model);
                subproblemTeacher = Learner(p);
                teachers[pair] = subproblemTeacher;
            }

            if (subproblemTeacher != null)
            {
                // TODO: This check only exists to provide support to previous way of 
                // using the library and should be removed after a few releases. In the
                // current way (without using any Obsolete methods), subproblem should never be null.
                subproblemTeacher.Token = ParallelOptions.CancellationToken;
                Model[i, j] = subproblemTeacher.Learn(subx, suby, subw);
            }
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
