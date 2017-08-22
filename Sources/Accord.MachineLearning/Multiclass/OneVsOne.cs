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

// TODO: Move out of this namespace, as those methods are not restricted to SVMs
namespace Accord.MachineLearning.VectorMachines
{
    /// <summary>
    ///   Decision strategies for <see cref="MulticlassSupportVectorMachine{TKernel}">
    ///   Multi-class Support Vector Machines</see>.
    /// </summary>
    /// 
    public enum MulticlassComputeMethod
    {
        /// <summary>
        ///   Max-voting method (also known as 1-vs-1 decision).
        /// </summary>
        /// 
        Voting,

        /// <summary>
        ///   Elimination method (also known as DAG decision).
        /// </summary>
        /// 
        Elimination,
    }
}

namespace Accord.MachineLearning
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Accord.Compat;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   One-Vs-One construction for solving multi-class
    ///   classification using a set of binary classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TBinary">The type for the binary classifier to be used.</typeparam>
    /// <typeparam name="TInput">The type for the classifier inputs. Default is double[].</typeparam>
    /// 
    /// <seealso cref="MulticlassSupportVectorMachine{TKernel}"/>
    /// 
    [Serializable]
    public class OneVsOne<TBinary, TInput> :
        MulticlassLikelihoodClassifierBase<TInput>,
        IEnumerable<KeyValuePair<ClassPair, TBinary>>, IParallel
        where TBinary : class, IClassifier<TInput, bool>, ICloneable
    {

        [NonSerialized]
        ParallelOptions options = new ParallelOptions();

        [NonSerialized]
        ThreadLocal<Decision[]> lastDecisionPath;

        private ClassPair[] indices;
        private TBinary[][] models;
        private MulticlassComputeMethod method = MulticlassComputeMethod.Elimination;

        /// <summary>
        ///   Gets the pair of class indices handled by each inner binary classification model.
        /// </summary>
        /// 
        protected ClassPair[] Indices { get { return indices; } }

        /// <summary>
        ///   Gets the inner binary classification models.
        /// </summary>
        /// 
        public TBinary[][] Models { get { return models; } }

        /// <summary>
        /// Gets or sets the parallelization options for this algorithm.
        /// </summary>
        public ParallelOptions ParallelOptions
        {
            get { return options; }
            set { options = value; }
        }

        /// <summary>
        /// Gets or sets a cancellation token that can be used
        /// to cancel the algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return options.CancellationToken; }
            set { options.CancellationToken = value; }
        }

        /// <summary>
        ///   Gets or sets the multi-class classification method to be
        ///   used when deciding for the class of a given input vector.
        ///   Default is <see cref="MulticlassComputeMethod.Elimination"/>.
        /// </summary>
        /// 
        public MulticlassComputeMethod Method
        {
            get { return method; }
            set { method = value; }
        }

        /// <summary>
        ///   Gets or sets whether to track the decision path associated
        ///   with each decision. The track will be available through the
        ///   <see cref="GetLastDecisionPath()"/> method. Default is true.
        /// </summary>
        ///
        public bool Track { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="OneVsOne{TBinary, TInput}"/> class.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the multi-class classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        /// 
        public OneVsOne(int classes, Func<TBinary> initializer)
        {
            Initialize(classes, initializer);
        }

        /// <summary>
        ///   Gets the last decision path used during the last call to any of the
        ///   model evaluation (Decide, Distance, LogLikelihood, Probability) methods
        ///   in the current thread. This method is thread-safe and returns the value 
        ///   obtained in the last call on the current thread. 
        /// </summary>
        /// 
        public Decision[] GetLastDecisionPath()
        {
            return (Decision[])lastDecisionPath.Value.Clone();
        }

        /// <summary>
        ///   Gets the last decision path without cloning.
        /// </summary>
        /// 
        protected Decision[] LastDecisionPath
        {
            get { return lastDecisionPath.Value; }
        }

        /// <summary>
        ///   Gets the inner binary classification model used to distinguish between
        ///   the given pair of classes.
        /// </summary>
        /// <param name="classA">The class index for the first class.</param>
        /// <param name="classB">The class index for the second class.</param>
        /// <returns>A binary classifier that can distinguish between the given classes.</returns>
        public TBinary GetClassifierForClassPair(int classA, int classB)
        {
            if (classA == classB)
                throw new ArgumentException();

            if (classA > classB)
                return models[classA - 1][classB];
            //else
            //return models[classB - 1][classA];
            throw new ArgumentException();
        }

        /// <summary>
        ///   Gets or sets the inner binary classification model used 
        ///   to distinguish between the given pair of classes.
        /// </summary>
        /// <param name="classA">The class index for the first class.</param>
        /// <param name="classB">The class index for the second class.</param>
        /// <returns>A binary classifier that can distinguish between the given classes.</returns>
        public TBinary this[int classA, int classB]
        {
            get
            {
                if (classA == classB)
                    return null;
                if (classA > classB)
                    return models[classA - 1][classB];
                return models[classB - 1][classA];
            }
            set
            {
                if (classA <= classB)
                    throw new ArgumentOutOfRangeException();
                models[classA - 1][classB] = value;
            }
        }

        /// <summary>
        ///   Gets a inner binary classification model inside this <see cref="OneVsOne{T}"/>
        ///   classifier, together with the pair of classes that it has been designed to
        ///   distinguish.
        /// </summary>
        /// 
        /// <param name="index">The index of the model (up to <see cref="Count"/>).</param>
        /// 
        public KeyValuePair<ClassPair, TBinary> this[int index]
        {
            get { return this.ToArray()[index]; }
        }


        private void Initialize(int classes, Func<TBinary> initializer)
        {
            if (classes <= 1)
                throw new ArgumentException("Number of classes must be higher than 1.", "classes");

            this.NumberOfOutputs = classes;
            this.NumberOfClasses = classes;
            int total = (classes * (classes - 1)) / 2;

            models = new TBinary[classes - 1][];
            indices = new ClassPair[total];
            int k = 0;
            for (int i = 0; i < Models.Length; i++)
            {
                models[i] = new TBinary[i + 1];
                for (int j = 0; j < models[i].Length; j++)
                {
                    indices[k++] = new ClassPair(i + 1, j);
                    var model = initializer();
                    models[i][j] = model;
                    if (model != null)
                        NumberOfInputs = model.NumberOfInputs;
                }
            }

            this.lastDecisionPath = new ThreadLocal<Decision[]>(() => new Decision[NumberOfOutputs - 1]);
            this.Track = true;
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            this.lastDecisionPath = new ThreadLocal<Decision[]>(() => new Decision[NumberOfOutputs - 1]);
            this.ParallelOptions = new ParallelOptions();
        }

        /// <summary>
        ///   Gets the number of inner binary classification models used by
        ///   this instance. It should correspond to <c>(c * (c - 1)) / 2</c>
        ///   where <c>c</c> is the number of classes.
        /// </summary>
        /// 
        public int Count
        {
            get { return indices.Length; }
        }



        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public override int Decide(TInput input)
        {
            if (method == MulticlassComputeMethod.Voting)
                return DecideByVoting(input);

            if (Track)
                return DecideByElimination(input, this.lastDecisionPath.Value);
            return DecideByElimination(input);
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result">The location where to store the class-labels.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override int[] Decide(TInput[] input, int[] result)
        {
            if (method == MulticlassComputeMethod.Voting)
            {
                for (int i = 0; i < input.Length; i++)
                    result[i] = DecideByVoting(input[i]);
            }
            else
            {
                if (Track)
                {
                    if (options.MaxDegreeOfParallelism == 1)
                    {
                        for (int i = 0; i < input.Length - 1; i++)
                        {
                            result[i] = DecideByElimination(input[i]);
                        }
                    }
                    else
                    {
                        Parallel.For(0, input.Length - 1, options, i =>
                        {
                            result[i] = DecideByElimination(input[i]);
                        });
                    }
                    result[result.Length - 1] = DecideByElimination(input[input.Length - 1], this.lastDecisionPath.Value);
                }
                else
                {
                    Parallel.For(0, input.Length, options, i =>
                    {
                        result[i] = DecideByElimination(input[i]);
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored,
        /// avoiding unnecessary memory allocations.</param>
        public override double[] Scores(TInput input, double[] result)
        {
            if (method == MulticlassComputeMethod.Voting)
                return DistanceByVoting(input, result);

            if (Track)
                return DistanceByElimination(input, result, lastDecisionPath.Value);
            return DistanceByElimination(input, result);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="result">An array where the probabilities will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns></returns>
        public override double[] LogLikelihoods(TInput input, double[] result)
        {
            return Scores(input, result);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public override double LogLikelihood(TInput input, int classIndex)
        {
            return Scores(input)[classIndex];
        }



#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int DecideByVoting(TInput input)
        {
            return DistanceByVoting(input, new double[NumberOfOutputs]).ArgMax();
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int DecideByElimination(TInput input)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;

            while (i != j)
            {
                if (Models[i - 1][j].Decide(input))
                    j++; // i won, so we advance j
                else
                    i--; // j won, so we advance i
            }

            return i;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int DecideByElimination(TInput input, Decision[] path)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;

            int k = 0;

            while (i != j)
            {
                if (Models[i - 1][j].Decide(input))
                {
                    path[k++] = new Decision(j, i, i);
                    j++; // i won, so we advance j
                }
                else
                {
                    path[k++] = new Decision(i, j, j);
                    i--; // j won, so we advance i
                }
            }

            return i;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private double[] DistanceByElimination(TInput input, double[] result)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;
            double sum = 0;
            double max = Double.NegativeInfinity;

            while (i != j)
            {
                var model = Models[i - 1][j] as IBinaryScoreClassifier<TInput>;

                bool decision;
                sum = model.Score(input, out decision);

                if (decision)
                {
                    result[j] = -sum;
                    j++;

                    if (sum > max)
                        max = sum;
                }
                else
                {
                    result[i] = sum;
                    i--;

                    if (-sum > max)
                        max = -sum;
                }
            }

            result[i] = max;
            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private double[] DistanceByElimination(TInput input, double[] result, Decision[] path)
        {
            int i = NumberOfOutputs - 1;
            int j = 0;
            int k = 0;
            double sum = 0;
            double max = Double.NegativeInfinity;

            while (i != j)
            {
                var model = Models[i - 1][j] as IBinaryScoreClassifier<TInput>;

                bool decision;
                sum = model.Score(input, out decision);

                if (decision)
                {
                    result[j] = -sum;
                    path[k++] = new Decision(j, i, i);
                    j++;

                    if (sum > max)
                        max = sum;
                }
                else
                {
                    result[i] = sum;
                    path[k++] = new Decision(i, j, j);
                    i--;

                    if (-sum > max)
                        max = -sum;
                }
            }

            result[i] = max;
            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private double[] DistanceByVoting(TInput input, double[] result)
        {
            Parallel.For(0, indices.Length, options, k =>
            {
                int i = indices[k].Class1;
                int j = indices[k].Class2;

                if (Models[i - 1][j].Decide(input))
                    InterlockedEx.Increment(ref result[i]);
                else InterlockedEx.Increment(ref result[j]);
            });

            return result;
        }




        /// <summary>
        ///   Returns an enumerator that iterates through all machines
        ///   contained inside this multi-class support vector machine.
        /// </summary>
        /// 
        public IEnumerator<KeyValuePair<ClassPair, TBinary>> GetEnumerator()
        {
            for (int i = 0; i < models.Length; i++)
                for (int j = 0; j < models[i].Length; j++)
                    yield return new KeyValuePair<ClassPair, TBinary>(new ClassPair(i + 1, j), models[i][j]);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through all machines
        ///   contained inside this multi-class support vector machine.
        /// </summary>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    ///   One-Vs-One construction for solving multi-class
    ///   classification using a set of binary classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TBinary">The type for the binary classifier to be used.</typeparam>
    /// 
    /// <seealso cref="MulticlassSupportVectorMachine{TKernel}"/>
    /// 
    [Serializable]
    public class OneVsOne<TBinary> : OneVsOne<TBinary, double[]>
       where TBinary : class, IClassifier<double[], bool>, ICloneable
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="OneVsOne{TBinary}"/> class.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the multi-class classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        /// 
        public OneVsOne(int classes, Func<TBinary> initializer)
            : base(classes, initializer)
        {
        }
    }
}
