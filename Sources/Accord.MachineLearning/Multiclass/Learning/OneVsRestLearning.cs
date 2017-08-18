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
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Accord.Statistics;
    using Accord.Compat;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base learning algorithm for <see cref="OneVsRest{TBinary, TModel}"/> multi-class classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TBinary">The type for the inner binary classifiers used in the one-vs-rest approach.</typeparam>
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// 
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.MultilabelSupportVectorLearning{TKernel}"/>
    /// 
    public abstract class OneVsRestLearning<TBinary, TModel> :
        OneVsRestLearning<double[], TBinary, TModel>,
        ISupervisedLearning<TModel, double[], int>
        where TModel : OneVsRest<TBinary>
        where TBinary : class, IBinaryScoreClassifier<double[]>
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
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.MultilabelSupportVectorLearning{TKernel}"/>
    ///
    [Serializable]
    public abstract class OneVsRestLearning<TInput, TBinary, TModel> :
        ParallelLearningBase,
        ISupervisedLearning<TModel, TInput, int>,
        ISupervisedLearning<TModel, TInput, int[]>,
        ISupervisedLearning<TModel, TInput, bool[]>
        where TBinary : IBinaryScoreClassifier<TInput>
        where TModel : OneVsRest<TBinary, TInput>
    {
        private Func<InnerParameters<TBinary, TInput>, ISupervisedLearning<TBinary, TInput, bool>> learner;
        private ConcurrentDictionary<ClassPair, ISupervisedLearning<TBinary, TInput, bool>> teachers;
        private bool aggregateExceptions = true;
        private bool? isMultilabel = false;

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
        ///   Gets or sets a value indicating whether the learning algorithm should generate multi-label
        ///   (as opposed to multi-class) models. If left unspecified, the type of the model will be determined
        ///   automatically depending on which overload of the <see cref="Learn(TInput[], bool[][], double[])"/>
        ///   method will be called first by the executing code.
        /// </summary>
        /// 
        public bool? IsMultilabel
        {
            get { return isMultilabel; }
            set { isMultilabel = value; }
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
        /// Initializes a new instance of the <see cref="OneVsRestLearning{TInput, TBinary, TModel}"/> class.
        /// </summary>
        protected OneVsRestLearning()
        {
        }

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
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(int inputs, int classes, bool multilabel);

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
            if (this.isMultilabel == null)
                this.isMultilabel = false;
            return Learn(x, Jagged.OneHot<bool>(y), weights);
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
        public TModel Learn(TInput[] x, int[][] y, double[] weights = null)
        {
            return Learn(x, Classes.Decide(y), weights);
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
        public virtual TModel Learn(TInput[] x, bool[][] y, double[] weights = null)
        {
            Accord.MachineLearning.Tools.CheckArgs(x, y, weights, () =>
            {
                if (Model == null)
                {
                    this.teachers = null;
                    if (this.isMultilabel == null)
                        this.isMultilabel = Classes.IsMultilabel(y);
                    int numberOfInputs = Tools.GetNumberOfInputs(x);
                    int numberOfClasses = y.Columns();
                    Model = Create(numberOfInputs, numberOfClasses, isMultilabel.Value);
                }

                return Model;
            });

            if (teachers == null)
                teachers = new ConcurrentDictionary<ClassPair, ISupervisedLearning<TBinary, TInput, bool>>();

            int total = Model.NumberOfClasses;
            int progress = 0;

            // Save exceptions but process all machines
            var exceptions = new ConcurrentBag<Exception>();

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                // For each class k
                for (int k = 0; k < total; k++)
                    TrainBinaryMachine(x, y, weights, k, total, ref progress, exceptions);
            }
            else
            {
                // For each class k
                Parallel.For(0, total, ParallelOptions, (int k) =>
                    TrainBinaryMachine(x, y, weights, k, total, ref progress, exceptions));
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more exceptions were thrown when teaching "
                    + "the machines. Please check the InnerException property of this AggregateException "
                    + "to discover what exactly caused this error.", exceptions);
            }

            return Model;
        }

        private void TrainBinaryMachine(TInput[] x, bool[][] y, double[] weights, int k, int total, ref int progress, ConcurrentBag<Exception> exceptions)
        {
            if (ParallelOptions.CancellationToken.IsCancellationRequested)
                return;

            // We will start the binary sub-problem
            var pair = new ClassPair(k, -k);
            var args = new SubproblemEventArgs(k, -k)
            {
                Maximum = total
            };
            OnSubproblemStarted(args);


            // Retrieve the associated machine
            TBinary model = Model.Models[k];

            // Extract outputs for the given label
            bool[] suby = y.GetColumn(k);

            if (aggregateExceptions)
            {
                try
                {
                    // Train the machine on the two-class problem.
                    TrainBinaryMachine(pair, k, model, x, suby, weights);
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
                TrainBinaryMachine(pair, k, model, x, suby, weights);
            }

            // Update and report progress
            args.Progress = Interlocked.Increment(ref progress);
            args.Maximum = total;

            OnSubproblemFinished(args);
        }

        private void TrainBinaryMachine(ClassPair pair, int k, TBinary model, TInput[] x, bool[] suby, double[] weights)
        {
            // Configure the machine on the two-class problem. Check if the learner
            // for this machine has already been created before, and re-use it if it
            // was the case. This is necessary to support mini-batch/online learning.

            ISupervisedLearning<TBinary, TInput, bool> subproblemTeacher;
            if (!teachers.TryGetValue(pair, out subproblemTeacher))
            {
                var p = new InnerParameters<TBinary, TInput>(inputs: x, outputs: suby, pair: pair, model: model);
                subproblemTeacher = Learner(p);
                teachers[pair] = subproblemTeacher;
            }

            if (subproblemTeacher != null)
            {
                // TODO: This check only exists to provide support to previous way of 
                // using the library and should be removed after a few releases. In the
                // current way (without using any Obsolete methods), subproblem should never be null.
                subproblemTeacher.Token = ParallelOptions.CancellationToken;
                Model[k] = subproblemTeacher.Learn(x, suby, weights);
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
